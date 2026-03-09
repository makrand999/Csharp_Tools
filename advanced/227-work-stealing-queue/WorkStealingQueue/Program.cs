using System.Diagnostics.CodeAnalysis;
using System.Collections.Concurrent;

namespace WorkStealingQueue;

/// <summary>
/// Work-stealing queue implementation for load balancing across workers.
/// Demonstrates concurrent dequeues and work-stealing pattern for parallel processing.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Work-Stealing Queue ===\n");
        
        var workStealingQueue = new WorkStealingQueue<WorkItem>(4);
        
        // Producer: Add work items
        var producerTask = Task.Run(() =>
        {
            for (int i = 0; i < 100; i++)
            {
                workStealingQueue.Enqueue(new WorkItem { Id = i, Data = $"Task-{i}" });
                if (i % 20 == 0)
                    Console.WriteLine($"Enqueued work item {i}");
            }
        });
        
        // Workers: Process items, steal from others when idle
        var workers = Enumerable.Range(0, 4)
            .Select(id => Task.Run(() => WorkerAsync(workStealingQueue, id)))
            .ToList();
        
        await producerTask;
        await Task.Delay(500); // Wait for remaining work
        workStealingQueue.CompleteAdding();
        
        await Task.WhenAll(workers);
        
        Console.WriteLine($"\nAll workers completed!");
        
        // Demo work stealing
        DemonstrateWorkStealing();
    }
    
    static async Task WorkerAsync(WorkStealingQueue<WorkItem> queue, int workerId)
    {
        int processedCount = 0;
        int stolenCount = 0;
        
        while (!queue.IsCompleted)
        {
            if (queue.TryDequeue(workerId, out var item))
            {
                await ProcessWorkAsync(item);
                processedCount++;
                
                if (processedCount % 10 == 0)
                    Console.WriteLine($"Worker {workerId}: Processed {processedCount} items");
            }
            else if (queue.TrySteal(workerId, out var stolenItem))
            {
                await ProcessWorkAsync(stolenItem);
                processedCount++;
                stolenCount++;
                Console.WriteLine($"Worker {workerId}: STOLE work item {stolenItem.Id}");
            }
            else
            {
                await Task.Delay(10);
            }
        }
        
        Console.WriteLine($"Worker {workerId}: Done (processed={processedCount}, stolen={stolenCount})");
    }
    
    static async Task ProcessWorkAsync(WorkItem item)
    {
        await Task.Delay(Random.Shared.Next(5, 20));
    }
    
    static void DemonstrateWorkStealing()
    {
        Console.WriteLine("\n=== Work-Stealing Demo ===\n");
        
        var queue = new WorkStealingQueue<int>(3);
        
        // Add items to different local queues
        queue.EnqueueToQueue(0, 1);
        queue.EnqueueToQueue(0, 2);
        queue.EnqueueToQueue(0, 3);
        queue.EnqueueToQueue(1, 10);
        queue.EnqueueToQueue(1, 20);
        queue.EnqueueToQueue(2, 100);
        queue.EnqueueToQueue(2, 200);
        
        Console.WriteLine("Initial distribution:");
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"  Queue {i}: {queue.GetQueueCount(i)} items");
        }
        
        // Dequeue from own queue
        if (queue.TryDequeue(0, out var item))
        {
            Console.WriteLine($"\nWorker 0 dequeued: {item}");
        }
        
        // Steal from another queue
        if (queue.TrySteal(2, out var stolen))
        {
            Console.WriteLine($"Worker 2 stole: {stolen}");
        }
        
        Console.WriteLine("\nAfter operations:");
        for (int i = 0; i < 3; i++)
        {
            Console.WriteLine($"  Queue {i}: {queue.GetQueueCount(i)} items");
        }
    }
}

record WorkItem
{
    public int Id { get; init; }
    public required string Data { get; init; }
}

/// <summary>
/// Work-stealing queue with per-worker local queues.
/// Workers dequeue from their own queue (LIFO) and steal from others (FIFO).
/// </summary>
class WorkStealingQueue<T>
{
    private readonly ConcurrentQueue<T>[] _queues;
    private readonly Random _random = new();
    private bool _completed;
    
    public bool IsCompleted => _completed;
    
    public WorkStealingQueue(int workerCount)
    {
        _queues = Enumerable.Range(0, workerCount)
            .Select(_ => new ConcurrentQueue<T>())
            .ToArray();
    }
    
    public void Enqueue(T item)
    {
        // Round-robin distribution
        var queueIndex = Random.Shared.Next(_queues.Length);
        _queues[queueIndex].Enqueue(item);
    }
    
    public void EnqueueToQueue(int queueIndex, T item)
    {
        if (queueIndex >= 0 && queueIndex < _queues.Length)
        {
            _queues[queueIndex].Enqueue(item);
        }
    }
    
    public bool TryDequeue(int workerId, [MaybeNullWhen(false)] out T item)
    {
        var queue = _queues[workerId % _queues.Length];
        // Dequeue from own queue (FIFO for simplicity)
        return queue.TryDequeue(out item);
    }
    
    public bool TrySteal(int workerId, [MaybeNullWhen(false)] out T item)
    {
        // Try to steal from a random other queue
        var victimIndex = (workerId + 1 + Random.Shared.Next(_queues.Length - 1)) % _queues.Length;
        var victimQueue = _queues[victimIndex];
        
        // Steal from front of victim's queue
        return victimQueue.TryDequeue(out item);
    }
    
    public int GetQueueCount(int queueIndex)
    {
        return queueIndex >= 0 && queueIndex < _queues.Length 
            ? _queues[queueIndex].Count 
            : 0;
    }
    
    public void CompleteAdding() => _completed = true;
}
