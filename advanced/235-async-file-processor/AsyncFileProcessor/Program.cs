using System.Collections.Concurrent;
using System.Text;

namespace AsyncFileProcessor;

/// <summary>
/// Async File Processor - Processes files asynchronously with progress reporting.
/// Demonstrates async I/O operations with cancellation support.
/// </summary>
public class FileProcessor
{
    private readonly int _maxConcurrency;
    private readonly CancellationTokenSource _cancellationTokenSource = new();

    public FileProcessor(int maxConcurrency = 4)
    {
        _maxConcurrency = maxConcurrency;
    }

    public async Task<ProcessingResult> ProcessDirectoryAsync(
        string directoryPath,
        Func<string, CancellationToken, Task<FileResult>> processor,
        string? searchPattern = null,
        CancellationToken cancellationToken = default)
    {
        var files = Directory.GetFiles(
            directoryPath,
            searchPattern ?? "*.*",
            SearchOption.AllDirectories);

        Console.WriteLine($"Found {files.Length} files to process\n");

        var semaphore = new SemaphoreSlim(_maxConcurrency);
        var results = new ConcurrentBag<FileResult>();
        var processedCount = 0;
        var totalBytes = 0L;
        var startTime = DateTime.UtcNow;

        var tasks = files.Select(async file =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                var result = await processor(file, cancellationToken);
                results.Add(result);
                
                var count = Interlocked.Increment(ref processedCount);
                Interlocked.Add(ref totalBytes, result.BytesProcessed);

                var elapsed = DateTime.UtcNow - startTime;
                var speed = totalBytes / (elapsed.TotalSeconds > 0 ? elapsed.TotalSeconds : 1);

                Console.WriteLine($"[{count}/{files.Length}] {Path.GetFileName(file)} - {result.Status}");
                Console.WriteLine($"    Progress: {count * 100.0 / files.Length:F1}% | Speed: {speed / 1024:F1} KB/s");
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);

        return new ProcessingResult
        {
            TotalFiles = files.Length,
            ProcessedFiles = processedCount,
            TotalBytes = totalBytes,
            Results = results.ToList(),
            ElapsedTime = DateTime.UtcNow - startTime
        };
    }

    public void Cancel() => _cancellationTokenSource.Cancel();
}

public record FileResult
{
    public string FilePath { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public long BytesProcessed { get; init; }
    public TimeSpan Duration { get; init; }
}

public record ProcessingResult
{
    public int TotalFiles { get; init; }
    public int ProcessedFiles { get; init; }
    public long TotalBytes { get; init; }
    public List<FileResult> Results { get; init; } = new();
    public TimeSpan ElapsedTime { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Async File Processor ===\n");

        // Create test directory with sample files
        var testDir = Path.Combine(Path.GetTempPath(), $"async_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(testDir);

        Console.WriteLine($"Creating test files in: {testDir}\n");

        // Create sample files of various sizes
        var random = new Random();
        for (int i = 0; i < 10; i++)
        {
            var filePath = Path.Combine(testDir, $"file_{i}.txt");
            var content = GenerateRandomContent(random.Next(1000, 10000));
            await File.WriteAllTextAsync(filePath, content);
        }

        // Create processor
        var processor = new FileProcessor(maxConcurrency: 3);

        // Process files - calculate hash and count lines
        var result = await processor.ProcessDirectoryAsync(
            testDir,
            async (filePath, token) =>
            {
                var stopwatch = System.Diagnostics.Stopwatch.StartNew();
                
                // Simulate async file processing
                var content = await File.ReadAllTextAsync(filePath, token);
                var bytes = Encoding.UTF8.GetByteCount(content);
                var hash = await ComputeHashAsync(filePath, token);
                var lines = content.Split('\n').Length;

                stopwatch.Stop();

                return new FileResult
                {
                    FilePath = filePath,
                    Status = $"Hash: {hash[..8]}..., Lines: {lines}",
                    BytesProcessed = bytes,
                    Duration = stopwatch.Elapsed
                };
            },
            "*.txt"
        );

        // Display summary
        Console.WriteLine($"\n=== Processing Summary ===");
        Console.WriteLine($"Total Files: {result.TotalFiles}");
        Console.WriteLine($"Processed: {result.ProcessedFiles}");
        Console.WriteLine($"Total Bytes: {result.TotalBytes:N0}");
        Console.WriteLine($"Elapsed Time: {result.ElapsedTime.TotalSeconds:F2}s");
        Console.WriteLine($"Throughput: {result.TotalBytes / result.ElapsedTime.TotalSeconds / 1024:F1} KB/s");

        // Cleanup
        Console.WriteLine($"\nCleaning up test directory...");
        Directory.Delete(testDir, true);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static string GenerateRandomContent(int length)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789 \n";
        return new string(Enumerable.Repeat(chars, length)
            .Select(s => s[new Random().Next(s.Length)]).ToArray());
    }

    private static async Task<string> ComputeHashAsync(string filePath, CancellationToken token)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        await using var stream = File.OpenRead(filePath);
        var hash = await md5.ComputeHashAsync(stream, token);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
