namespace TaskScheduler;

/// <summary>
/// Custom task scheduler with priority-based execution.
/// Demonstrates task queuing, priority handling, and custom scheduling strategies.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Priority Task Scheduler ===\n");
        
        var scheduler = new PriorityTaskScheduler(maxConcurrency: 3);
        
        // Submit tasks with different priorities
        var tasks = new List<Task<int>>();
        
        tasks.Add(scheduler.EnqueueAsync(() => WorkAsync("Low priority task", 500), Priority.Low));
        tasks.Add(scheduler.EnqueueAsync(() => WorkAsync("High priority task", 300), Priority.High));
        tasks.Add(scheduler.EnqueueAsync(() => WorkAsync("Normal task 1", 200), Priority.Normal));
        tasks.Add(scheduler.EnqueueAsync(() => WorkAsync("Critical task", 100), Priority.Critical));
        tasks.Add(scheduler.EnqueueAsync(() => WorkAsync("Normal task 2", 250), Priority.Normal));
        tasks.Add(scheduler.EnqueueAsync(() => WorkAsync("High priority task 2", 150), Priority.High));
        
        var results = await Task.WhenAll(tasks);
        
        Console.WriteLine($"\nAll tasks completed!");
        Console.WriteLine($"Results: [{string.Join(", ", results)}]");
        Console.WriteLine($"Total scheduled: {scheduler.TotalScheduled}");
        Console.WriteLine($"Total completed: {scheduler.TotalCompleted}");
        
        // Demo scheduler
        await DemonstrateScheduler();
    }
    
    static async Task<int> WorkAsync(string name, int delay)
    {
        Console.WriteLine($"Starting: {name}");
        await Task.Delay(delay);
        Console.WriteLine($"Completed: {name}");
        return delay;
    }
    
    static async Task DemonstrateScheduler()
    {
        Console.WriteLine("\n=== Scheduler Demo ===\n");
        
        var scheduler = new PriorityTaskScheduler(maxConcurrency: 2);
        
        // Submit 6 tasks with varying priorities
        for (int i = 0; i < 6; i++)
        {
            int taskId = i;
            var priority = (Priority)(i % 4);
            
            _ = scheduler.EnqueueAsync(async () =>
            {
                Console.WriteLine($"Task {taskId} ({priority}) running");
                await Task.Delay(100);
                Console.WriteLine($"Task {taskId} ({priority}) done");
                return taskId;
            }, priority);
        }
        
        await scheduler.WaitForCompletionAsync();
        Console.WriteLine("\nScheduler demo complete!");
    }
}

enum Priority
{
    Low = 0,
    Normal = 1,
    High = 2,
    Critical = 3
}

/// <summary>
/// Priority-based task scheduler with configurable concurrency.
/// </summary>
class PriorityTaskScheduler
{
    private readonly int _maxConcurrency;
    private readonly PriorityQueue<Func<Task>, int> _queue = new();
    private readonly SemaphoreSlim _semaphore;
    private int _runningCount;
    private int _totalScheduled;
    private int _totalCompleted;
    private Task? _completionTask;
    private readonly object _lock = new();
    
    public int TotalScheduled => Volatile.Read(ref _totalScheduled);
    public int TotalCompleted => Volatile.Read(ref _totalCompleted);
    public int RunningCount => Volatile.Read(ref _runningCount);
    
    public PriorityTaskScheduler(int maxConcurrency)
    {
        _maxConcurrency = maxConcurrency;
        _semaphore = new SemaphoreSlim(maxConcurrency);
    }
    
    public Task<TResult> EnqueueAsync<TResult>(Func<Task<TResult>> task, Priority priority)
    {
        var tcs = new TaskCompletionSource<TResult>();
        Interlocked.Increment(ref _totalScheduled);
        
        // Invert priority for min-heap (higher priority = lower number)
        int queuePriority = -(int)priority;
        
        lock (_lock)
        {
            _queue.Enqueue(async () =>
            {
                try
                {
                    var result = await task();
                    tcs.SetResult(result);
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                finally
                {
                    Interlocked.Increment(ref _totalCompleted);
                }
            }, queuePriority);
            
            ProcessQueue();
        }
        
        return tcs.Task;
    }
    
    public Task EnqueueAsync(Func<Task> task, Priority priority)
    {
        var tcs = new TaskCompletionSource();
        Interlocked.Increment(ref _totalScheduled);
        
        int queuePriority = -(int)priority;
        
        lock (_lock)
        {
            _queue.Enqueue(async () =>
            {
                try
                {
                    await task();
                    tcs.SetResult();
                }
                catch (Exception ex)
                {
                    tcs.SetException(ex);
                }
                finally
                {
                    Interlocked.Increment(ref _totalCompleted);
                }
            }, queuePriority);
            
            ProcessQueue();
        }
        
        return tcs.Task;
    }
    
    private void ProcessQueue()
    {
        while (_runningCount < _maxConcurrency && _queue.Count > 0)
        {
            if (_queue.TryDequeue(out var task, out _))
            {
                Interlocked.Increment(ref _runningCount);
                
                _ = Task.Run(async () =>
                {
                    try
                    {
                        await _semaphore.WaitAsync();
                        await task();
                    }
                    finally
                    {
                        _semaphore.Release();
                        Interlocked.Decrement(ref _runningCount);
                        
                        lock (_lock)
                        {
                            ProcessQueue();
                        }
                    }
                });
            }
        }
    }
    
    public async Task WaitForCompletionAsync()
    {
        while (true)
        {
            lock (_lock)
            {
                if (_queue.Count == 0 && _runningCount == 0)
                    return;
            }
            await Task.Delay(10);
        }
    }
}
