namespace FileWatchService;

/// <summary>
/// File Watch Service - Monitors directories for file system changes.
/// Demonstrates FileSystemWatcher with event handling and debouncing.
/// </summary>
public class FileWatchService : IDisposable
{
    private readonly List<FileSystemWatcher> _watchers = new();
    private readonly List<FileChangeEvent> _events = new();
    private readonly object _lock = new();
    private readonly int _debounceMs;
    private bool _disposed;

    public event EventHandler<FileChangeEvent>? FileChanged;
    public event EventHandler<FileChangeEvent>? FileCreated;
    public event EventHandler<FileChangeEvent>? FileDeleted;
    public event EventHandler<FileChangeEvent>? FileRenamed;

    public FileWatchService(int debounceMs = 500)
    {
        _debounceMs = debounceMs;
    }

    public void Watch(string path, string filter = "*.*", bool includeSubdirectories = true)
    {
        var watcher = new FileSystemWatcher
        {
            Path = path,
            Filter = filter,
            IncludeSubdirectories = includeSubdirectories,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName |
                          NotifyFilters.LastWrite | NotifyFilters.Size |
                          NotifyFilters.CreationTime
        };

        watcher.Changed += OnChanged;
        watcher.Created += OnCreated;
        watcher.Deleted += OnDeleted;
        watcher.Renamed += OnRenamed;

        watcher.EnableRaisingEvents = true;
        _watchers.Add(watcher);

        Console.WriteLine($"Watching: {path} (filter: {filter})");
    }

    private void OnChanged(object sender, FileSystemEventArgs e)
    {
        var changeEvent = new FileChangeEvent
        {
            ChangeType = WatcherChangeTypes.Changed,
            FullPath = e.FullPath,
            Name = e.Name,
            Timestamp = DateTime.UtcNow
        };

        RaiseEvent(FileChanged, changeEvent);
    }

    private void OnCreated(object sender, FileSystemEventArgs e)
    {
        var changeEvent = new FileChangeEvent
        {
            ChangeType = WatcherChangeTypes.Created,
            FullPath = e.FullPath,
            Name = e.Name,
            Timestamp = DateTime.UtcNow
        };

        RaiseEvent(FileCreated, changeEvent);
    }

    private void OnDeleted(object sender, FileSystemEventArgs e)
    {
        var changeEvent = new FileChangeEvent
        {
            ChangeType = WatcherChangeTypes.Deleted,
            FullPath = e.FullPath,
            Name = e.Name,
            Timestamp = DateTime.UtcNow
        };

        RaiseEvent(FileDeleted, changeEvent);
    }

    private void OnRenamed(object sender, RenamedEventArgs e)
    {
        var changeEvent = new FileChangeEvent
        {
            ChangeType = WatcherChangeTypes.Renamed,
            FullPath = e.FullPath,
            Name = e.Name,
            OldPath = e.OldFullPath,
            OldName = e.OldName,
            Timestamp = DateTime.UtcNow
        };

        RaiseEvent(FileRenamed, changeEvent);
    }

    private void RaiseEvent(EventHandler<FileChangeEvent>? handler, FileChangeEvent e)
    {
        lock (_lock)
        {
            _events.Add(e);
        }

        handler?.Invoke(this, e);
    }

    public IReadOnlyList<FileChangeEvent> GetEvents()
    {
        lock (_lock)
        {
            return _events.ToList().AsReadOnly();
        }
    }

    public void ClearEvents()
    {
        lock (_lock)
        {
            _events.Clear();
        }
    }

    public void Dispose()
    {
        if (_disposed) return;

        foreach (var watcher in _watchers)
        {
            watcher.Dispose();
        }

        _watchers.Clear();
        _disposed = true;
    }
}

public record FileChangeEvent
{
    public WatcherChangeTypes ChangeType { get; init; }
    public string FullPath { get; init; } = string.Empty;
    public string Name { get; init; } = string.Empty;
    public string? OldPath { get; init; }
    public string? OldName { get; init; }
    public DateTime Timestamp { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== File Watch Service ===\n");
        Console.WriteLine("Monitoring directory for file changes...\n");

        var watchDir = Path.Combine(Path.GetTempPath(), $"watch_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(watchDir);

        using var service = new FileWatchService();

        service.FileCreated += (s, e) =>
        {
            Console.WriteLine($"[CREATED] {e.Name} at {e.Timestamp:HH:mm:ss.fff}");
        };

        service.FileChanged += (s, e) =>
        {
            Console.WriteLine($"[CHANGED] {e.Name} at {e.Timestamp:HH:mm:ss.fff}");
        };

        service.FileDeleted += (s, e) =>
        {
            Console.WriteLine($"[DELETED] {e.Name} at {e.Timestamp:HH:mm:ss.fff}");
        };

        service.FileRenamed += (s, e) =>
        {
            Console.WriteLine($"[RENAMED] {e.OldName} -> {e.Name} at {e.Timestamp:HH:mm:ss.fff}");
        };

        service.Watch(watchDir);

        Console.WriteLine("\nPerforming test operations...\n");

        // Test create
        var testFile = Path.Combine(watchDir, "test.txt");
        await File.WriteAllTextAsync(testFile, "Initial content");
        await Task.Delay(100);

        // Test change
        await File.WriteAllTextAsync(testFile, "Modified content");
        await Task.Delay(100);

        // Test rename
        var renamedFile = Path.Combine(watchDir, "renamed.txt");
        File.Move(testFile, renamedFile);
        await Task.Delay(100);

        // Test delete
        File.Delete(renamedFile);
        await Task.Delay(100);

        // Show event summary
        var events = service.GetEvents();
        Console.WriteLine($"\n=== Event Summary ===");
        Console.WriteLine($"Total events captured: {events.Count}");

        var created = events.Count(e => e.ChangeType == WatcherChangeTypes.Created);
        var changed = events.Count(e => e.ChangeType == WatcherChangeTypes.Changed);
        var deleted = events.Count(e => e.ChangeType == WatcherChangeTypes.Deleted);
        var renamed = events.Count(e => e.ChangeType == WatcherChangeTypes.Renamed);

        Console.WriteLine($"  Created: {created}");
        Console.WriteLine($"  Changed: {changed}");
        Console.WriteLine($"  Deleted: {deleted}");
        Console.WriteLine($"  Renamed: {renamed}");

        // Cleanup
        Directory.Delete(watchDir, true);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
