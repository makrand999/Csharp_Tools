using System.IO.Pipelines;
using System.Buffers;

namespace HighPerformanceIo;

/// <summary>
/// High Performance I/O - Uses System.IO.Pipelines for efficient stream processing.
/// Demonstrates zero-copy I/O and backpressure handling.
/// </summary>
public class HighPerformanceReader
{
    private readonly int _bufferSize;

    public HighPerformanceReader(int bufferSize = 8192)
    {
        _bufferSize = bufferSize;
    }

    public async Task<ReadStatistics> ReadFileAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var pipe = new Pipe();
        var totalBytes = 0L;
        var bufferCount = 0;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Producer task - read from file
        var producerTask = Task.Run(async () =>
        {
            await using var stream = File.OpenRead(filePath);
            var buffer = new byte[8192];
            var writer = pipe.Writer;

            try
            {
                int bytesRead;
                while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
                {
                    await writer.WriteAsync(buffer.AsMemory(0, bytesRead), cancellationToken);
                    totalBytes += bytesRead;
                    bufferCount++;
                    await writer.FlushAsync(cancellationToken);
                }
            }
            finally
            {
                await writer.CompleteAsync();
            }
        });

        // Consumer task - process data
        var consumerTask = Task.Run(async () =>
        {
            var reader = pipe.Reader;
            var processedBytes = 0L;

            try
            {
                while (true)
                {
                    var result = await reader.ReadAsync(cancellationToken);
                    var data = result.Buffer;

                    if (data.IsEmpty && result.IsCompleted)
                        break;

                    // Process data (simulate work)
                    foreach (var segment in data)
                    {
                        ProcessSegment(segment.Span);
                        processedBytes += segment.Length;
                    }

                    reader.AdvanceTo(data.End);
                }

                return processedBytes;
            }
            finally
            {
                await reader.CompleteAsync();
            }
        });

        await Task.WhenAll(producerTask, consumerTask);
        stopwatch.Stop();

        var processedBytes = await consumerTask;

        return new ReadStatistics
        {
            TotalBytes = totalBytes,
            ProcessedBytes = processedBytes,
            BufferCount = bufferCount,
            ElapsedTime = stopwatch.Elapsed,
            ThroughputMbps = totalBytes / stopwatch.Elapsed.TotalSeconds / 1024 / 1024
        };
    }

    private void ProcessSegment(ReadOnlySpan<byte> data)
    {
        // Simulate processing - count bytes
        foreach (var b in data)
        {
            _ = b;
        }
    }
}

public class DirectMemoryReader
{
    public async Task<long> ReadWithDirectBufferAsync(string filePath, CancellationToken cancellationToken = default)
    {
        var totalBytes = 0L;

        await using var stream = new FileStream(
            filePath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            useAsync: true); // Enables overlapped I/O on Windows

        var buffer = new byte[8192];

        int bytesRead;
        while ((bytesRead = await stream.ReadAsync(buffer.AsMemory(0, buffer.Length), cancellationToken)) > 0)
        {
            totalBytes += bytesRead;
            
            // Process the buffer
            ProcessBuffer(buffer.AsSpan(0, bytesRead));
        }

        return totalBytes;
    }

    private void ProcessBuffer(ReadOnlySpan<byte> buffer)
    {
        // Simulate processing
        foreach (var b in buffer)
        {
            _ = b;
        }
    }
}

public record ReadStatistics
{
    public long TotalBytes { get; init; }
    public long ProcessedBytes { get; init; }
    public int BufferCount { get; init; }
    public TimeSpan ElapsedTime { get; init; }
    public double ThroughputMbps { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== High Performance I/O ===\n");
        Console.WriteLine("Using System.IO.Pipelines and async streams\n");

        // Create test file
        var testFile = Path.Combine(Path.GetTempPath(), $"perf_test_{Guid.NewGuid()}.dat");
        Console.WriteLine($"Creating 50 MB test file...");

        await using (var stream = File.Create(testFile))
        {
            var buffer = new byte[1024 * 1024]; // 1 MB buffer
            new Random().NextBytes(buffer);

            for (int i = 0; i < 50; i++)
            {
                await stream.WriteAsync(buffer);
            }
        }

        var fileSize = new FileInfo(testFile).Length;
        Console.WriteLine($"Test file size: {fileSize / 1024 / 1024} MB\n");

        // Test with pipelines
        Console.WriteLine("=== Pipeline-based Reading ===");
        var reader = new HighPerformanceReader();
        var stats = await reader.ReadFileAsync(testFile);

        Console.WriteLine($"Total Bytes: {stats.TotalBytes:N0}");
        Console.WriteLine($"Buffers: {stats.BufferCount}");
        Console.WriteLine($"Time: {stats.ElapsedTime.TotalSeconds:F2}s");
        Console.WriteLine($"Throughput: {stats.ThroughputMbps:F1} MB/s");

        // Test with direct async
        Console.WriteLine("\n=== Direct Async Reading ===");
        var directReader = new DirectMemoryReader();
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var directBytes = await directReader.ReadWithDirectBufferAsync(testFile);
        stopwatch.Stop();

        Console.WriteLine($"Total Bytes: {directBytes:N0}");
        Console.WriteLine($"Time: {stopwatch.Elapsed.TotalSeconds:F2}s");
        Console.WriteLine($"Throughput: {directBytes / stopwatch.Elapsed.TotalSeconds / 1024 / 1024:F1} MB/s");

        // Cleanup
        File.Delete(testFile);
        Console.WriteLine($"\nCleaned up test file");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
