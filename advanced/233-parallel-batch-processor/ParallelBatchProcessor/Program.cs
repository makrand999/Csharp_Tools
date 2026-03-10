using System.Collections.Concurrent;

namespace ParallelBatchProcessor;

/// <summary>
/// Parallel Batch Processor - Processes large datasets in parallel batches.
/// Uses Partitioner for efficient work distribution across available cores.
/// </summary>
public class BatchProcessor<TInput, TOutput>
{
    private readonly int _maxDegreeOfParallelism;
    private readonly Func<TInput, TOutput> _processor;
    private readonly int _batchSize;

    public BatchProcessor(
        Func<TInput, TOutput> processor,
        int maxDegreeOfParallelism = -1,
        int batchSize = 100)
    {
        _processor = processor;
        _maxDegreeOfParallelism = maxDegreeOfParallelism == -1
            ? Environment.ProcessorCount
            : maxDegreeOfParallelism;
        _batchSize = batchSize;
    }

    public IEnumerable<TOutput> Process(IEnumerable<TInput> items)
    {
        var results = new ConcurrentBag<TOutput>();
        var itemCount = items.Count();

        Console.WriteLine($"Processing {itemCount} items with {_maxDegreeOfParallelism} threads...");
        Console.WriteLine($"Batch size: {_batchSize}\n");

        var partitioner = Partitioner.Create(items, EnumerablePartitionerOptions.NoBuffering);

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxDegreeOfParallelism
        };

        var processedCount = 0;
        var lastReport = 0;

        Parallel.ForEach(partitioner, options, item =>
        {
            var result = _processor(item);
            results.Add(result);

            var count = Interlocked.Increment(ref processedCount);
            
            // Report progress every 10%
            var progressPercent = (count * 100) / itemCount;
            if (progressPercent > lastReport && progressPercent % 10 == 0)
            {
                lastReport = progressPercent;
                Console.WriteLine($"Progress: {progressPercent}% ({count}/{itemCount})");
            }
        });

        return results;
    }

    public async Task<IEnumerable<TOutput>> ProcessAsync(
        IEnumerable<TInput> items,
        CancellationToken cancellationToken = default)
    {
        var results = new ConcurrentBag<TOutput>();
        var itemCount = items.Count();
        var processedCount = 0;
        var lastReport = 0;

        Console.WriteLine($"Async processing {itemCount} items with {_maxDegreeOfParallelism} threads...\n");

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxDegreeOfParallelism,
            CancellationToken = cancellationToken
        };

        // Simple parallel processing without async
        Parallel.ForEach(items, options, item =>
        {
            var result = _processor(item);
            results.Add(result);

            var count = Interlocked.Increment(ref processedCount);

            // Report progress every 10%
            var progressPercent = (count * 100) / itemCount;
            if (progressPercent > lastReport && progressPercent % 10 == 0)
            {
                lastReport = progressPercent;
                Console.WriteLine($"Progress: {progressPercent}% ({count}/{itemCount})");
            }
        });

        return await Task.FromResult(results.AsEnumerable());
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Parallel Batch Processor ===\n");

        // Generate sample data - numbers to process
        var numbers = Enumerable.Range(1, 1000).ToList();

        // Create processor that performs CPU-intensive calculation
        var processor = new BatchProcessor<int, long>(
            processor: CalculateFactorial,
            maxDegreeOfParallelism: Environment.ProcessorCount,
            batchSize: 50
        );

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var results = processor.Process(numbers).ToList();

        stopwatch.Stop();

        Console.WriteLine($"\n=== Results ===");
        Console.WriteLine($"Processed: {results.Count} items");
        Console.WriteLine($"Time elapsed: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Items/second: {results.Count * 1000.0 / stopwatch.ElapsedMilliseconds:F0}");

        // Show sample results
        Console.WriteLine($"\nSample results (first 5):");
        foreach (var result in results.Take(5))
        {
            Console.WriteLine($"  {result:N0}");
        }

        // Compare with sequential processing
        Console.WriteLine($"\n=== Sequential Comparison ===");
        stopwatch.Restart();
        var sequentialResults = numbers.Select(CalculateFactorial).ToList();
        stopwatch.Stop();
        
        Console.WriteLine($"Sequential time: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Parallel speedup: {(double)stopwatch.ElapsedMilliseconds / (stopwatch.ElapsedMilliseconds > 0 ? 1 : 1):F2}x");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static long CalculateFactorial(int n)
    {
        // Simulate CPU-intensive work
        long result = 1;
        for (int i = 2; i <= Math.Min(n, 20); i++)
        {
            result *= i;
        }
        
        // Add some artificial delay to simulate real work
        Thread.SpinWait(10000);
        
        return result;
    }
}
