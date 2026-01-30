namespace FileWatcher;

/// <summary>
/// Monitors directories for file system changes and logs events.
/// Useful for watching build outputs, sync folders, or detecting new files.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        var config = ParseArgs(args);
        if (config == null) return;

        var watcher = new FileSystemWatcherService();
        watcher.Watch(config);
    }

    static WatchConfig? ParseArgs(string[] args)
    {
        var config = new WatchConfig
        {
            Paths = new List<string>(),
            Filters = new List<string> { "*.*" },
            IncludeSubdirectories = true,
            Verbose = false
        };

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--help":
                    ShowHelp();
                    return null;

                case "--filter":
                    if (i + 1 < args.Length)
                    {
                        config.Filters = args[++i].Split(',', StringSplitOptions.RemoveEmptyEntries).ToList();
                    }
                    break;

                case "--no-subdirs":
                    config.IncludeSubdirectories = false;
                    break;

                case "--verbose":
                    config.Verbose = true;
                    break;

                case var arg when !arg.StartsWith("--"):
                    config.Paths.Add(arg);
                    break;
            }
        }

        if (config.Paths.Count == 0)
        {
            Console.Error.WriteLine("Error: Please specify at least one directory to watch");
            ShowHelp();
            return null;
        }

        return config;
    }

    static void ShowHelp()
    {
        Console.WriteLine("File Watcher - Monitor directories for file system changes");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project FileWatcher.csproj -- <paths...> [options]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  paths            One or more directories to watch");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --filter <f>     File filters (comma-separated, e.g., '*.cs,*.json')");
        Console.WriteLine("  --no-subdirs     Don't watch subdirectories");
        Console.WriteLine("  --verbose        Show detailed event information");
        Console.WriteLine("  --help           Show this help");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project FileWatcher.csproj -- .");
        Console.WriteLine("  dotnet run --project FileWatcher.csproj -- ./src ./test --filter '*.cs'");
        Console.WriteLine("  dotnet run --project FileWatcher.csproj -- ~/Downloads --filter '*.pdf,*.zip' --no-subdirs");
    }
}

class WatchConfig
{
    public List<string> Paths { get; set; } = new();
    public List<string> Filters { get; set; } = new();
    public bool IncludeSubdirectories { get; set; } = true;
    public bool Verbose { get; set; } = false;
}

class FileSystemWatcherService
{
    private readonly List<FileSystemWatcher> _watchers = new();
    private readonly object _lock = new();

    public void Watch(WatchConfig config)
    {
        Console.WriteLine("File Watcher - Monitoring for changes");
        Console.WriteLine("Press Ctrl+C to stop\n");

        foreach (var path in config.Paths)
        {
            var fullPath = Path.GetFullPath(path);
            
            if (!Directory.Exists(fullPath))
            {
                Console.Error.WriteLine($"Warning: Directory does not exist: {fullPath}");
                continue;
            }

            foreach (var filter in config.Filters)
            {
                var watcher = CreateWatcher(fullPath, filter.Trim(), config);
                _watchers.Add(watcher);
            }

            Console.WriteLine($"Watching: {fullPath}");
        }

        if (_watchers.Count == 0)
        {
            Console.Error.WriteLine("Error: No valid directories to watch");
            return;
        }

        Console.WriteLine($"\nMonitoring {_watchers.Count} watcher(s)...");
        Console.WriteLine("Events will be logged below:\n");

        // Keep running until Ctrl+C
        var quitEvent = new ManualResetEvent(false);
        Console.CancelKeyPress += (_, e) =>
        {
            e.Cancel = true;
            quitEvent.Set();
        };
        quitEvent.WaitOne();

        Cleanup();
        Console.WriteLine("\nWatcher stopped.");
    }

    FileSystemWatcher CreateWatcher(string path, string filter, WatchConfig config)
    {
        var watcher = new FileSystemWatcher(path, filter)
        {
            IncludeSubdirectories = config.IncludeSubdirectories,
            NotifyFilter = NotifyFilters.FileName | NotifyFilters.DirectoryName |
                          NotifyFilters.Size | NotifyFilters.LastWrite |
                          NotifyFilters.CreationTime
        };

        watcher.Created += OnChanged;
        watcher.Deleted += OnChanged;
        watcher.Changed += OnChanged;
        watcher.Renamed += OnRenamed;
        watcher.Error += OnError;

        watcher.EnableRaisingEvents = true;
        return watcher;
    }

    void OnChanged(object source, FileSystemEventArgs e)
    {
        LogEvent(e.ChangeType.ToString().PadRight(10), e.FullPath, e.Name);
    }

    void OnRenamed(object source, RenamedEventArgs e)
    {
        LogEvent("RENAMED   ", $"{e.OldFullPath} -> {e.FullPath}", e.Name);
    }

    void OnError(object source, ErrorEventArgs e)
    {
        lock (_lock)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ERROR] {e.GetException().Message}");
            Console.ResetColor();
        }
    }

    void LogEvent(string eventType, string path, string fileName)
    {
        lock (_lock)
        {
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            var color = GetEventColor(eventType.Trim());
            
            Console.ForegroundColor = color;
            Console.Write($"[{timestamp}] {eventType} ");
            Console.ResetColor();
            Console.WriteLine(path);
        }
    }

    ConsoleColor GetEventColor(string eventType) => eventType switch
    {
        "Created" => ConsoleColor.Green,
        "Deleted" => ConsoleColor.Red,
        "Changed" => ConsoleColor.Yellow,
        "Renamed" => ConsoleColor.Cyan,
        _ => ConsoleColor.White
    };

    void Cleanup()
    {
        foreach (var watcher in _watchers)
        {
            watcher.EnableRaisingEvents = false;
            watcher.Dispose();
        }
        _watchers.Clear();
    }
}
