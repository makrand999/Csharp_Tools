// Countdown Event - Thread signaling with CountdownEvent
// Demonstrates waiting for multiple operations to complete

using System.Collections.Concurrent;

Console.WriteLine("=== Countdown Event ===\n");
Console.WriteLine("Coordinating multiple operations with CountdownEvent...\n");

// Example 1: Basic CountdownEvent usage
Console.WriteLine("--- Example 1: Basic Usage ---\n");
await DemonstrateBasicCountdownAsync();

// Example 2: Parallel task coordination
Console.WriteLine("\n--- Example 2: Parallel Task Coordination ---\n");
await DemonstrateParallelCoordinationAsync();

// Example 3: Dynamic task addition
Console.WriteLine("\n--- Example 3: Dynamic Task Addition ---\n");
await DemonstrateDynamicTasksAsync();

Console.WriteLine("\n✓ All CountdownEvent demonstrations completed!");

static async Task DemonstrateBasicCountdownAsync()
{
    const int taskCount = 5;
    using var countdown = new CountdownEvent(taskCount);
    var tasks = new List<Task>();

    Console.WriteLine($"Starting {taskCount} tasks...\n");

    for (int i = 0; i < taskCount; i++)
    {
        int taskId = i;
        tasks.Add(Task.Run(() =>
        {
            Console.WriteLine($"[Task {taskId}] Starting work...");
            Thread.Sleep(Random.Shared.Next(100, 300));
            Console.WriteLine($"[Task {taskId}] Work complete, signaling countdown.");
            countdown.Signal();
        }));
    }

    // Wait for all tasks to signal
    Console.WriteLine("\nMain thread waiting for all tasks to complete...\n");
    countdown.Wait();

    await Task.WhenAll(tasks);
    Console.WriteLine("\n✓ All tasks completed!");
}

static async Task DemonstrateParallelCoordinationAsync()
{
    const int producerCount = 3;
    const int consumerCount = 2;
    
    var dataQueue = new Queue<int>();
    var dataLock = new object();
    
    using var producersDone = new CountdownEvent(producerCount);
    using var consumersDone = new CountdownEvent(consumerCount);
    
    var tasks = new List<Task>();

    // Create producers
    for (int i = 0; i < producerCount; i++)
    {
        int producerId = i;
        tasks.Add(Task.Run(() =>
        {
            for (int j = 0; j < 5; j++)
            {
                int data = producerId * 100 + j;
                lock (dataLock)
                {
                    dataQueue.Enqueue(data);
                    Console.WriteLine($"[Producer {producerId}] Produced: {data}");
                }
                Thread.Sleep(50);
            }
            Console.WriteLine($"[Producer {producerId}] Finished producing.");
            producersDone.Signal();
        }));
    }

    // Create consumers
    for (int i = 0; i < consumerCount; i++)
    {
        int consumerId = i;
        tasks.Add(Task.Run(() =>
        {
            while (true)
            {
                int data;
                lock (dataLock)
                {
                    if (dataQueue.Count > 0)
                    {
                        data = dataQueue.Dequeue();
                        Console.WriteLine($"[Consumer {consumerId}] Consumed: {data}");
                    }
                    else if (producersDone.IsSet)
                    {
                        break;
                    }
                    else
                    {
                        Thread.Sleep(10);
                        continue;
                    }
                }
                Thread.Sleep(30);
            }
            Console.WriteLine($"[Consumer {consumerId}] Finished consuming.");
            consumersDone.Signal();
        }));
    }

    // Wait for producers
    producersDone.Wait();
    Console.WriteLine("\n✓ All producers finished!");

    // Wait for consumers
    consumersDone.Wait();
    Console.WriteLine("✓ All consumers finished!");

    await Task.WhenAll(tasks);
}

static async Task DemonstrateDynamicTasksAsync()
{
    using var countdown = new CountdownEvent(1); // Start with 1
    var completedTasks = new List<int>();
    var lockObj = new object();

    Console.WriteLine("Starting with initial task, adding more dynamically...\n");

    // Initial task
    Task.Run(() =>
    {
        Console.WriteLine("[Initial Task] Running...");
        Thread.Sleep(200);
        
        // Add more tasks dynamically
        Console.WriteLine("[Initial Task] Adding 3 more tasks...");
        countdown.AddCount(3);
        
        for (int i = 1; i <= 3; i++)
        {
            int taskId = i;
            Task.Run(() =>
            {
                Console.WriteLine($"[Dynamic Task {taskId}] Running...");
                Thread.Sleep(Random.Shared.Next(100, 200));
                lock (lockObj)
                {
                    completedTasks.Add(taskId);
                }
                Console.WriteLine($"[Dynamic Task {taskId}] Complete.");
                countdown.Signal();
            });
        }
        
        Console.WriteLine("[Initial Task] Complete.");
        countdown.Signal();
    });

    // Wait for all tasks (initial + dynamic)
    countdown.Wait();
    
    Console.WriteLine($"\n✓ All {completedTasks.Count + 1} tasks completed!");
}

/// <summary>
/// Demonstrates CountdownEvent for coordinating parallel operations
/// </summary>
public class ParallelBatchProcessor
{
    private readonly int _batchSize;
    private readonly CountdownEvent _countdown;

    public ParallelBatchProcessor(int batchSize)
    {
        _batchSize = batchSize;
        _countdown = new CountdownEvent(batchSize);
    }

    /// <summary>
    /// Process items in parallel, waiting for all to complete
    /// </summary>
    public async Task<IEnumerable<TResult>> ProcessBatchAsync<TSource, TResult>(
        IEnumerable<TSource> items,
        Func<TSource, Task<TResult>> processor)
    {
        var results = new ConcurrentBag<TResult>();
        var tasks = new List<Task>();

        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                var result = await processor(item);
                results.Add(result);
                _countdown.Signal();
            }));
        }

        // Wait for all items to be processed
        _countdown.Wait();
        await Task.WhenAll(tasks);

        return results;
    }

    /// <summary>
    /// Reset for next batch
    /// </summary>
    public void Reset(int newBatchSize)
    {
        _countdown.Reset(newBatchSize);
    }
}
