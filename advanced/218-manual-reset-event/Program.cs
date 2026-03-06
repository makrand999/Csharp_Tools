// Manual Reset Event Slim - Thread signaling with MRES
// Demonstrates efficient wait handles for thread coordination

Console.WriteLine("=== ManualResetEventSlim ===\n");
Console.WriteLine("Coordinating threads with ManualResetEventSlim...\n");

// Example 1: Basic signaling
Console.WriteLine("--- Example 1: Basic Signaling ---\n");
await DemonstrateBasicSignalingAsync();

// Example 2: Producer-Consumer coordination
Console.WriteLine("\n--- Example 2: Producer-Consumer ---\n");
await DemonstrateProducerConsumerAsync();

// Example 3: Multiple waiters
Console.WriteLine("\n--- Example 3: Multiple Waiters ---\n");
await DemonstrateMultipleWaitersAsync();

Console.WriteLine("\n✓ All MRES demonstrations completed!");

static async Task DemonstrateBasicSignalingAsync()
{
    using var mres = new ManualResetEventSlim(false);
    var workerTask = Task.Run(() =>
    {
        Console.WriteLine("[Worker] Waiting for signal...");
        mres.Wait();
        Console.WriteLine("[Worker] Signal received! Continuing work...");
    });

    await Task.Delay(500);
    Console.WriteLine("[Main] Setting signal...");
    mres.Set();
    
    await workerTask;
    Console.WriteLine("[Main] Worker completed!");
}

static async Task DemonstrateProducerConsumerAsync()
{
    var queue = new Queue<string>();
    var queueLock = new object();
    using var dataAvailable = new ManualResetEventSlim(false);
    using var completion = new ManualResetEventSlim(false);
    
    var producerTask = Task.Run(() =>
    {
        for (int i = 1; i <= 5; i++)
        {
            var data = $"Item-{i}";
            lock (queue)
            {
                queue.Enqueue(data);
                Console.WriteLine($"[Producer] Produced: {data}");
            }
            dataAvailable.Set(); // Signal that data is available
            Thread.Sleep(200);
        }
        Console.WriteLine("[Producer] Production complete.");
        completion.Set(); // Signal completion
    });

    var consumerTask = Task.Run(() =>
    {
        while (true)
        {
            // Wait for either data or completion
            int index = WaitHandle.WaitAny(new[] { dataAvailable.WaitHandle, completion.WaitHandle }, TimeSpan.FromSeconds(1));
            
            if (index == 1) // Completion signaled
            {
                // Drain remaining items
                lock (queue)
                {
                    while (queue.Count > 0)
                    {
                        var item = queue.Dequeue();
                        Console.WriteLine($"[Consumer] Consumed (final): {item}");
                    }
                }
                Console.WriteLine("[Consumer] All items consumed.");
                break;
            }
            
            if (index == 0) // Data available
            {
                string? item = null;
                lock (queue)
                {
                    if (queue.Count > 0)
                    {
                        item = queue.Dequeue();
                    }
                }
                
                if (item != null)
                {
                    Console.WriteLine($"[Consumer] Consumed: {item}");
                    Thread.Sleep(100); // Simulate processing
                    
                    // Reset if queue is empty
                    lock (queue)
                    {
                        if (queue.Count == 0)
                        {
                            dataAvailable.Reset();
                        }
                    }
                }
            }
        }
    });

    await Task.WhenAll(producerTask, consumerTask);
}

static async Task DemonstrateMultipleWaitersAsync()
{
    using var startSignal = new ManualResetEventSlim(false);
    var tasks = new List<Task>();

    Console.WriteLine("Creating 5 worker tasks waiting for start signal...\n");

    for (int i = 1; i <= 5; i++)
    {
        int workerId = i;
        tasks.Add(Task.Run(() =>
        {
            Console.WriteLine($"[Worker {workerId}] Waiting for start signal...");
            startSignal.Wait();
            
            Console.WriteLine($"[Worker {workerId}] Starting work!");
            Thread.Sleep(Random.Shared.Next(100, 300));
            Console.WriteLine($"[Worker {workerId}] Work complete!");
        }));
    }

    await Task.Delay(500);
    Console.WriteLine("\n[Main] Setting start signal - all workers begin!\n");
    startSignal.Set();

    await Task.WhenAll(tasks);
    Console.WriteLine("\n✓ All workers completed!");
}

/// <summary>
/// Demonstrates ManualResetEventSlim for thread coordination
/// </summary>
public class ThreadCoordinator
{
    private readonly ManualResetEventSlim _startSignal;
    private readonly ManualResetEventSlim _completionSignal;
    private readonly List<Task> _workers;
    private int _workerCount;

    public ThreadCoordinator(int workerCount)
    {
        _workerCount = workerCount;
        _startSignal = new ManualResetEventSlim(false);
        _completionSignal = new ManualResetEventSlim(false);
        _workers = new List<Task>();
    }

    /// <summary>
    /// Start workers and wait for completion
    /// </summary>
    public async Task RunAsync(Func<int, Task> workerAction)
    {
        int remainingWorkers = _workerCount;
        
        // Create worker tasks
        for (int i = 0; i < _workerCount; i++)
        {
            int workerId = i;
            _workers.Add(Task.Run(async () =>
            {
                _startSignal.Wait(); // Wait for start signal
                
                try
                {
                    await workerAction(workerId);
                }
                finally
                {
                    // Signal if this is the last worker
                    if (Interlocked.Decrement(ref remainingWorkers) == 0)
                    {
                        _completionSignal.Set();
                    }
                }
            }));
        }

        // Release all workers
        _startSignal.Set();

        // Wait for all to complete
        _completionSignal.Wait();
        await Task.WhenAll(_workers);
    }

    public void Dispose()
    {
        _startSignal.Dispose();
        _completionSignal.Dispose();
    }
}

/// <summary>
/// Simple barrier implementation using ManualResetEventSlim
/// </summary>
public class SimpleBarrier
{
    private readonly int _participantCount;
    private int _remainingParticipants;
    private readonly ManualResetEventSlim _barrier;
    private readonly object _lock = new();

    public SimpleBarrier(int participantCount)
    {
        _participantCount = participantCount;
        _remainingParticipants = participantCount;
        _barrier = new ManualResetEventSlim(false);
    }

    /// <summary>
    /// Signal arrival and wait for all participants
    /// </summary>
    public void SignalAndWait()
    {
        int remaining;
        
        lock (_lock)
        {
            remaining = --_remainingParticipants;
            
            if (remaining == 0)
            {
                // Last participant - release all
                _barrier.Set();
            }
        }

        // Wait for the barrier to be released
        _barrier.Wait();
    }

    /// <summary>
    /// Reset barrier for next phase
    /// </summary>
    public void Reset()
    {
        lock (_lock)
        {
            _remainingParticipants = _participantCount;
            _barrier.Reset();
        }
    }
}
