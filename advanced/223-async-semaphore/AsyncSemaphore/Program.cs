using System.Collections.Concurrent;

namespace AsyncSemaphore;

/// <summary>
/// Async semaphore implementation for resource pool management.
/// Demonstrates async-aware synchronization primitives for concurrent access control.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Async Semaphore Resource Pool ===\n");
        
        var pool = new AsyncResourcePool<DatabaseConnection>(3, () => new DatabaseConnection());
        
        var tasks = Enumerable.Range(0, 10)
            .Select(id => Task.Run(() => WorkerAsync(id, pool)))
            .ToList();
        
        await Task.WhenAll(tasks);
        
        Console.WriteLine($"\nTotal queries executed: {pool.TotalQueriesExecuted}");
        Console.WriteLine($"All connections returned: {pool.AllConnectionsReturned}");
        
        // Demo basic async semaphore
        await DemonstrateAsyncSemaphore();
    }
    
    static async Task WorkerAsync(int workerId, AsyncResourcePool<DatabaseConnection> pool)
    {
        await using var connection = await pool.AcquireAsync();
        
        Console.WriteLine($"Worker {workerId}: Acquired connection {connection.Id}");
        
        // Simulate database work
        await Task.Delay(Random.Shared.Next(100, 300));
        await connection.ExecuteQueryAsync($"SELECT * FROM table WHERE id = {workerId}");
        
        Console.WriteLine($"Worker {workerId}: Released connection {connection.Id}");
    }
    
    static async Task DemonstrateAsyncSemaphore()
    {
        Console.WriteLine("\n=== Async Semaphore Demo ===\n");
        
        var semaphore = new System.Threading.SemaphoreSlim(2, 2);
        var tasks = new List<Task>();
        
        Console.WriteLine("Starting 5 tasks with max 2 concurrent...\n");
        
        for (int i = 0; i < 5; i++)
        {
            int taskId = i;
            tasks.Add(Task.Run(async () =>
            {
                Console.WriteLine($"Task {taskId}: Waiting for semaphore...");
                await semaphore.WaitAsync();
                
                try
                {
                    Console.WriteLine($"Task {taskId}: Entered critical section");
                    await Task.Delay(500);
                    Console.WriteLine($"Task {taskId}: Exiting critical section");
                }
                finally
                {
                    semaphore.Release();
                }
            }));
        }
        
        await Task.WhenAll(tasks);
        Console.WriteLine("\nAll tasks completed!");
    }
}

class AsyncResourcePool<T> : IAsyncDisposable where T : class, IAsyncDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Func<T> _factory;
    private readonly ConcurrentBag<T> _available = new();
    private int _totalAcquisitions;
    private readonly int _maxCount;

    public int TotalQueriesExecuted => _totalAcquisitions;
    public bool AllConnectionsReturned => _semaphore.CurrentCount == _maxCount;

    public AsyncResourcePool(int maxCount, Func<T> factory)
    {
        _maxCount = maxCount;
        _semaphore = new SemaphoreSlim(maxCount, maxCount);
        _factory = factory;
    }
    
    public async ValueTask<T> AcquireAsync()
    {
        await _semaphore.WaitAsync();
        Interlocked.Increment(ref _totalAcquisitions);
        
        if (_available.TryTake(out var resource))
        {
            return resource;
        }
        
        return _factory();
    }
    
    public async ValueTask DisposeAsync()
    {
        while (_available.TryTake(out var resource))
        {
            await resource.DisposeAsync();
        }
        _semaphore.Dispose();
    }
}

class DatabaseConnection : IAsyncDisposable
{
    private static int _nextId;
    public int Id { get; } = Interlocked.Increment(ref _nextId);
    
    public async Task ExecuteQueryAsync(string query)
    {
        await Task.Delay(50); // Simulate query execution
        Console.WriteLine($"Connection {Id}: Executed '{query}'");
    }
    
    public ValueTask DisposeAsync()
    {
        Console.WriteLine($"Connection {Id}: Disposed");
        return default;
    }
}
