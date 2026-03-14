namespace FileSyncUtility;

/// <summary>
/// File Sync Utility - Synchronizes files between two directories.
/// Demonstrates file comparison, copy operations, and sync strategies.
/// </summary>
public class FileSyncService
{
    public enum SyncMode
    {
        Mirror,      // Make destination identical to source
        TwoWay,      // Sync both ways, newer wins
        Backup       // Copy only new/changed files to destination
    }

    public async Task<SyncResult> SynchronizeAsync(
        string sourcePath,
        string destinationPath,
        SyncMode mode = SyncMode.Mirror,
        IProgress<string>? progress = null)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var result = new SyncResult
        {
            SourcePath = sourcePath,
            DestinationPath = destinationPath,
            SyncMode = mode
        };

        Directory.CreateDirectory(destinationPath);

        var sourceFiles = GetRelativeFiles(sourcePath);
        var destFiles = GetRelativeFiles(destinationPath);

        // Files to copy from source to destination
        foreach (var relPath in sourceFiles)
        {
            var sourceFile = Path.Combine(sourcePath, relPath);
            var destFile = Path.Combine(destinationPath, relPath);

            progress?.Report($"Checking: {relPath}");

            if (!File.Exists(destFile))
            {
                // New file
                await CopyFileAsync(sourceFile, destFile);
                result.FilesCopied++;
                result.BytesTransferred += new FileInfo(sourceFile).Length;
            }
            else
            {
                // Check if modified
                var sourceTime = File.GetLastWriteTimeUtc(sourceFile);
                var destTime = File.GetLastWriteTimeUtc(destFile);

                if (sourceTime > destTime)
                {
                    await CopyFileAsync(sourceFile, destFile);
                    result.FilesCopied++;
                    result.BytesTransferred += new FileInfo(sourceFile).Length;
                }
            }
        }

        // Handle deletions for mirror mode
        if (mode == SyncMode.Mirror)
        {
            foreach (var relPath in destFiles)
            {
                if (!sourceFiles.Contains(relPath))
                {
                    var destFile = Path.Combine(destinationPath, relPath);
                    File.Delete(destFile);
                    result.FilesDeleted++;
                    progress?.Report($"Deleted: {relPath}");
                }
            }
        }

        stopwatch.Stop();
        result.ElapsedTime = stopwatch.Elapsed;
        result.Success = true;

        return result;
    }

    private HashSet<string> GetRelativeFiles(string rootPath)
    {
        if (!Directory.Exists(rootPath))
            return new HashSet<string>();

        return new HashSet<string>(
            Directory.GetFiles(rootPath, "*.*", SearchOption.AllDirectories)
                .Select(f => Path.GetRelativePath(rootPath, f))
        );
    }

    private async Task CopyFileAsync(string source, string destination)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        
        await using var sourceStream = File.OpenRead(source);
        await using var destStream = File.Create(destination);
        
        await sourceStream.CopyToAsync(destStream);
        
        // Preserve timestamps
        File.SetLastWriteTimeUtc(destination, File.GetLastWriteTimeUtc(source));
    }
}

public record SyncResult
{
    public string SourcePath { get; set; } = string.Empty;
    public string DestinationPath { get; set; } = string.Empty;
    public FileSyncService.SyncMode SyncMode { get; set; }
    public int FilesCopied { get; set; }
    public int FilesDeleted { get; set; }
    public long BytesTransferred { get; set; }
    public TimeSpan ElapsedTime { get; set; }
    public bool Success { get; set; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== File Sync Utility ===\n");

        var service = new FileSyncService();

        // Create test directories
        var sourceDir = Path.Combine(Path.GetTempPath(), $"sync_source_{Guid.NewGuid()}");
        var destDir = Path.Combine(Path.GetTempPath(), $"sync_dest_{Guid.NewGuid()}");

        Directory.CreateDirectory(sourceDir);

        Console.WriteLine("Creating test files in source directory...");
        
        // Create initial files
        for (int i = 0; i < 5; i++)
        {
            var content = $"File {i} content - {DateTime.UtcNow}";
            await File.WriteAllTextAsync(Path.Combine(sourceDir, $"file{i}.txt"), content);
        }

        Console.WriteLine($"Source files: {Directory.GetFiles(sourceDir).Length}\n");

        // First sync (mirror)
        Console.WriteLine("=== Initial Sync (Mirror Mode) ===");
        var progress = new Progress<string>(msg => Console.WriteLine($"  {msg}"));
        
        var result1 = await service.SynchronizeAsync(sourceDir, destDir, FileSyncService.SyncMode.Mirror, progress);

        Console.WriteLine($"\nSync Result:");
        Console.WriteLine($"  Files copied: {result1.FilesCopied}");
        Console.WriteLine($"  Files deleted: {result1.FilesDeleted}");
        Console.WriteLine($"  Bytes transferred: {result1.BytesTransferred}");
        Console.WriteLine($"  Time: {result1.ElapsedTime.TotalMilliseconds:F0}ms");

        Console.WriteLine($"\nDestination files: {Directory.GetFiles(destDir).Length}");

        // Modify source
        Console.WriteLine("\n=== Modifying Source ===");
        await File.WriteAllTextAsync(Path.Combine(sourceDir, "file0.txt"), "Modified content");
        await File.WriteAllTextAsync(Path.Combine(sourceDir, "file5.txt"), "New file");

        // Second sync
        Console.WriteLine("\n=== Second Sync (Detecting Changes) ===");
        var result2 = await service.SynchronizeAsync(sourceDir, destDir, FileSyncService.SyncMode.Mirror, progress);

        Console.WriteLine($"\nSync Result:");
        Console.WriteLine($"  Files copied: {result2.FilesCopied}");
        Console.WriteLine($"  Files deleted: {result2.FilesDeleted}");
        Console.WriteLine($"  Bytes transferred: {result2.BytesTransferred}");
        Console.WriteLine($"  Time: {result2.ElapsedTime.TotalMilliseconds:F0}ms");

        // Verify
        var sourceFiles = Directory.GetFiles(sourceDir).Length;
        var destFiles = Directory.GetFiles(destDir).Length;
        var verified = sourceFiles == destFiles;

        Console.WriteLine($"\nVerification: {(verified ? "✓ PASSED" : "✗ FAILED")}");

        // Cleanup
        Directory.Delete(sourceDir, true);
        Directory.Delete(destDir, true);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
