// Async Stream Processor - Demonstrates IAsyncEnumerable and async streams
// Processes data streams asynchronously with pipelined operations

using System.Runtime.CompilerServices;

var processor = new AsyncStreamProcessor();

Console.WriteLine("=== Async Stream Processor ===\n");
Console.WriteLine("Processing data stream asynchronously...\n");

// Create a sample data stream
var dataStream = GenerateDataStream(20);

// Process the stream with multiple async operations
await processor.ProcessStreamAsync(dataStream);

Console.WriteLine("\n✓ Stream processing completed!");

// Generate async stream of data items
static async IAsyncEnumerable<int> GenerateDataStream(int count, [EnumeratorCancellation] CancellationToken ct = default)
{
    for (int i = 1; i <= count; i++)
    {
        await Task.Delay(50, ct); // Simulate async data source
        yield return i;
    }
}

/// <summary>
/// Demonstrates async stream processing with IAsyncEnumerable<T>
/// </summary>
public class AsyncStreamProcessor
{
    private readonly int _maxDegreeOfParallelism = 4;
    private readonly SemaphoreSlim _semaphore;

    public AsyncStreamProcessor()
    {
        _semaphore = new SemaphoreSlim(_maxDegreeOfParallelism);
    }

    /// <summary>
    /// Process an async stream with concurrent operations
    /// </summary>
    public async Task ProcessStreamAsync(IAsyncEnumerable<int> stream, CancellationToken ct = default)
    {
        var processingTasks = new List<Task>();

        await foreach (var item in stream.WithCancellation(ct))
        {
            await _semaphore.WaitAsync(ct);
            
            processingTasks.Add(Task.Run(async () =>
            {
                try
                {
                    await ProcessItemAsync(item, ct);
                }
                finally
                {
                    _semaphore.Release();
                }
            }, ct));
        }

        await Task.WhenAll(processingTasks);
    }

    /// <summary>
    /// Process individual items with simulated async work
    /// </summary>
    private async Task ProcessItemAsync(int item, CancellationToken ct = default)
    {
        // Simulate async processing (e.g., network call, file I/O)
        await Task.Delay(Random.Shared.Next(50, 150), ct);
        
        var result = Transform(item);
        
        Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId:D2}] Processed: {item} → {result}");
    }

    /// <summary>
    /// Synchronous transformation applied to each item
    /// </summary>
    private int Transform(int value) => value * value;

    /// <summary>
    /// Filter async stream based on predicate
    /// </summary>
    public static async IAsyncEnumerable<T> FilterAsync<T>(
        IAsyncEnumerable<T> source, 
        Func<T, bool> predicate,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var item in source.WithCancellation(ct))
        {
            if (predicate(item))
            {
                yield return item;
            }
        }
    }

    /// <summary>
    /// Map async stream with async transformation
    /// </summary>
    public static async IAsyncEnumerable<TResult> MapAsync<TSource, TResult>(
        IAsyncEnumerable<TSource> source,
        Func<TSource, Task<TResult>> selector,
        [EnumeratorCancellation] CancellationToken ct = default)
    {
        await foreach (var item in source.WithCancellation(ct))
        {
            var result = await selector(item);
            yield return result;
        }
    }
}
