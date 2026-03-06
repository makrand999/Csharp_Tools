// Semaphore Counter - Resource counting and access control
// Demonstrates Semaphore for limiting concurrent access

using System.Collections.Concurrent;

Console.WriteLine("=== Semaphore Counter ===\n");
Console.WriteLine("Limiting concurrent access with Semaphore...\n");

// Example 1: Basic semaphore usage
Console.WriteLine("--- Example 1: Limiting Concurrent Database Connections ---\n");
await DemonstrateDatabaseConnectionsAsync();

// Example 2: Semaphore as a counter
Console.WriteLine("\n--- Example 2: Resource Pool Management ---\n");
await DemonstrateResourcePoolAsync();

// Example 3: Semaphore with timeout
Console.WriteLine("\n--- Example 3: Semaphore with Timeout ---\n");
await DemonstrateTimeoutAsync();

Console.WriteLine("\n✓ All Semaphore demonstrations completed!");

static async Task DemonstrateDatabaseConnectionsAsync()
{
    const int maxConnections = 3;
    const int totalRequests = 8;
    
    using var semaphore = new SemaphoreSlim(maxConnections);
    var tasks = new List<Task>();

    Console.WriteLine($"Allowing max {maxConnections} concurrent connections out of {totalRequests} requests...\n");

    for (int i = 1; i <= totalRequests; i++)
    {
        int requestId = i;
        tasks.Add(Task.Run(async () =>
        {
            Console.WriteLine($"[Request {requestId}] Waiting for connection...");
            await semaphore.WaitAsync();
            
            try
            {
                int currentCount = maxConnections - semaphore.CurrentCount;
                Console.WriteLine($"[Request {requestId}] Got connection (active: {currentCount}/{maxConnections})");
                
                // Simulate database operation
                await Task.Delay(Random.Shared.Next(300, 600));
                
                Console.WriteLine($"[Request {requestId}] Database operation complete");
            }
            finally
            {
                semaphore.Release();
                Console.WriteLine($"[Request {requestId}] Released connection");
            }
        }));
    }

    await Task.WhenAll(tasks);
    Console.WriteLine("\n✓ All database requests completed!");
}

static async Task DemonstrateResourcePoolAsync()
{
    const int poolSize = 5;
    var resourcePool = new ResourcePool<string>(poolSize);
    
    // Initialize pool with resources
    for (int i = 1; i <= poolSize; i++)
    {
        resourcePool.AddResource($"Resource-{i}");
    }

    var tasks = new List<Task>();
    var random = new Random();

    Console.WriteLine($"Created resource pool with {poolSize} resources\n");

    for (int i = 1; i <= 10; i++)
    {
        int taskId = i;
        tasks.Add(Task.Run(async () =>
        {
            Console.WriteLine($"[Task {taskId}] Requesting resource...");
            var resource = await resourcePool.AcquireAsync();
            
            try
            {
                Console.WriteLine($"[Task {taskId}] Acquired: {resource}");
                await Task.Delay(random.Next(200, 500));
                Console.WriteLine($"[Task {taskId}] Using: {resource}");
            }
            finally
            {
                await resourcePool.ReleaseAsync(resource);
                Console.WriteLine($"[Task {taskId}] Released: {resource}");
            }
        }));
    }

    await Task.WhenAll(tasks);
    Console.WriteLine($"\n✓ Resource pool stats: {resourcePool.AcquireCount} acquisitions, {resourcePool.ReleaseCount} releases");
}

static async Task DemonstrateTimeoutAsync()
{
    using var semaphore = new SemaphoreSlim(2);
    var tasks = new List<Task>();

    Console.WriteLine("Testing semaphore with timeout (300ms)...\n");

    for (int i = 1; i <= 5; i++)
    {
        int taskId = i;
        tasks.Add(Task.Run(async () =>
        {
            Console.WriteLine($"[Task {taskId}] Trying to acquire...");
            
            bool acquired = await semaphore.WaitAsync(TimeSpan.FromMilliseconds(300));
            
            if (acquired)
            {
                try
                {
                    Console.WriteLine($"[Task {taskId}] Acquired semaphore!");
                    await Task.Delay(500);
                    Console.WriteLine($"[Task {taskId}] Work complete");
                }
                finally
                {
                    semaphore.Release();
                }
            }
            else
            {
                Console.WriteLine($"[Task {taskId}] Timeout - could not acquire semaphore");
            }
        }));
    }

    await Task.WhenAll(tasks);
}

/// <summary>
/// Generic resource pool using SemaphoreSlim for access control
/// </summary>
public class ResourcePool<T> where T : notnull
{
    private readonly SemaphoreSlim _semaphore;
    private readonly ConcurrentBag<T> _availableResources = new();
    private readonly int _maxSize;

    public ResourcePool(int maxSize)
    {
        _maxSize = maxSize;
        _semaphore = new SemaphoreSlim(maxSize);
    }

    /// <summary>
    /// Add a resource to the pool
    /// </summary>
    public void AddResource(T resource)
    {
        _availableResources.Add(resource);
    }

    /// <summary>
    /// Acquire a resource from the pool (async)
    /// </summary>
    public async Task<T> AcquireAsync(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
        
        if (_availableResources.TryTake(out var resource))
        {
            AcquireCount++;
            return resource;
        }
        
        throw new InvalidOperationException("No available resources in pool");
    }

    /// <summary>
    /// Release a resource back to the pool
    /// </summary>
    public async Task ReleaseAsync(T resource)
    {
        _availableResources.Add(resource);
        _semaphore.Release();
        ReleaseCount++;
    }

    /// <summary>
    /// Get count of available resources
    /// </summary>
    public int AvailableCount => _availableResources.Count;

    /// <summary>
    /// Get count of resources in use
    /// </summary>
    public int InUseCount => _maxSize - _semaphore.CurrentCount;

    public int AcquireCount { get; private set; }
    public int ReleaseCount { get; private set; }
}

/// <summary>
/// Rate limiter using SemaphoreSlim
/// </summary>
public class RateLimiter : IDisposable
{
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxRequests;
    private readonly TimeSpan _timeWindow;
    private readonly CancellationTokenSource _cts;
    private readonly Task _refillTask;

    public RateLimiter(int maxRequests, TimeSpan timeWindow)
    {
        _maxRequests = maxRequests;
        _timeWindow = timeWindow;
        _semaphore = new SemaphoreSlim(maxRequests);
        _cts = new CancellationTokenSource();
        
        // Start background task to refill permits
        _refillTask = RefillAsync(_cts.Token);
    }

    private async Task RefillAsync(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(_timeWindow, ct);
            
            // Refill up to max
            int current = _semaphore.CurrentCount;
            int toAdd = _maxRequests - current;
            
            if (toAdd > 0)
            {
                _semaphore.Release(toAdd);
            }
        }
    }

    /// <summary>
    /// Wait for rate limit permit
    /// </summary>
    public async Task WaitAsync(CancellationToken ct = default)
    {
        await _semaphore.WaitAsync(ct);
    }

    /// <summary>
    /// Try to acquire permit with timeout
    /// </summary>
    public async Task<bool> TryWaitAsync(TimeSpan timeout, CancellationToken ct = default)
    {
        return await _semaphore.WaitAsync(timeout, ct);
    }

    public void Dispose()
    {
        _cts.Cancel();
        _cts.Dispose();
        _semaphore.Dispose();
    }
}
