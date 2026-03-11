using System.Collections.Concurrent;

namespace TaskParallelProcessor;

/// <summary>
/// Task Parallel Processor - Demonstrates TPL (Task Parallel Library) patterns.
/// Uses Task.WhenAll, ContinueWith, and parallel task execution.
/// </summary>
public class ParallelTaskProcessor<T>
{
    private readonly int _maxConcurrency;
    private readonly Func<T, CancellationToken, Task> _processor;

    public ParallelTaskProcessor(
        Func<T, CancellationToken, Task> processor,
        int maxConcurrency = -1)
    {
        _processor = processor;
        _maxConcurrency = maxConcurrency == -1 ? Environment.ProcessorCount : maxConcurrency;
    }

    public async Task<ProcessingStats> ProcessAsync(
        IEnumerable<T> items,
        CancellationToken cancellationToken = default)
    {
        var itemList = items.ToList();
        var semaphore = new SemaphoreSlim(_maxConcurrency);
        var processedCount = 0;
        var failedCount = 0;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var tasks = itemList.Select(async item =>
        {
            await semaphore.WaitAsync(cancellationToken);
            try
            {
                await _processor(item, cancellationToken);
                var count = Interlocked.Increment(ref processedCount);
                Console.WriteLine($"✓ Processed item {count}/{itemList.Count}");
            }
            catch (Exception ex)
            {
                var count = Interlocked.Increment(ref failedCount);
                Console.WriteLine($"✗ Failed: {ex.Message}");
            }
            finally
            {
                semaphore.Release();
            }
        });

        await Task.WhenAll(tasks);
        stopwatch.Stop();

        return new ProcessingStats
        {
            TotalItems = itemList.Count,
            ProcessedCount = processedCount,
            FailedCount = failedCount,
            ElapsedTime = stopwatch.Elapsed,
            ItemsPerSecond = processedCount / stopwatch.Elapsed.TotalSeconds
        };
    }

    public async Task ProcessWithProgressAsync(
        IEnumerable<T> items,
        IProgress<double>? progress = null,
        CancellationToken cancellationToken = default)
    {
        var itemList = items.ToList();
        var total = itemList.Count;
        var processed = 0;

        var tasks = itemList.Select(async item =>
        {
            await _processor(item, cancellationToken);
            var count = Interlocked.Increment(ref processed);
            progress?.Report(count * 100.0 / total);
        });

        await Task.WhenAll(tasks);
    }
}

public record ProcessingStats
{
    public int TotalItems { get; init; }
    public int ProcessedCount { get; init; }
    public int FailedCount { get; init; }
    public TimeSpan ElapsedTime { get; init; }
    public double ItemsPerSecond { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Task Parallel Processor ===\n");

        // Create sample work items
        var workItems = Enumerable.Range(1, 20).Select(i => new WorkItem
        {
            Id = i,
            Name = $"Task-{i}",
            DurationMs = Random.Shared.Next(100, 500)
        }).ToList();

        var processor = new ParallelTaskProcessor<WorkItem>(
            async (item, token) =>
            {
                // Simulate async work
                await Task.Delay(item.DurationMs, token);
                
                // Simulate some processing
                await Task.Run(() =>
                {
                    Thread.SpinWait(10000);
                }, token);
            },
            maxConcurrency: 5
        );

        var progress = new Progress<double>(percent =>
        {
            Console.Write($"\rProgress: {percent:F1}%");
        });

        Console.WriteLine($"Processing {workItems.Count} tasks with 5 concurrent workers...\n");

        var stats = await processor.ProcessAsync(workItems);

        Console.WriteLine($"\n\n=== Processing Statistics ===");
        Console.WriteLine($"Total Items: {stats.TotalItems}");
        Console.WriteLine($"Processed: {stats.ProcessedCount}");
        Console.WriteLine($"Failed: {stats.FailedCount}");
        Console.WriteLine($"Elapsed Time: {stats.ElapsedTime.TotalSeconds:F2}s");
        Console.WriteLine($"Throughput: {stats.ItemsPerSecond:F1} items/sec");

        // Calculate theoretical vs actual time
        var totalWorkTime = workItems.Sum(w => w.DurationMs);
        var theoreticalParallelTime = totalWorkTime / 5.0;
        Console.WriteLine($"\nTheoretical parallel time: {theoreticalParallelTime / 1000:F2}s");
        Console.WriteLine($"Actual time: {stats.ElapsedTime.TotalSeconds:F2}s");
        Console.WriteLine($"Efficiency: {theoreticalParallelTime / stats.ElapsedTime.TotalMilliseconds * 100:F1}%");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}

public record WorkItem
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public int DurationMs { get; init; }
}
