// Thread Local Cache - Per-thread storage for performance optimization
// Demonstrates ThreadLocal<T> for thread-specific data

using System.Text;

Console.WriteLine("=== Thread Local Cache ===\n");
Console.WriteLine("Using ThreadLocal<T> for per-thread caching...\n");

// Example 1: Basic ThreadLocal usage
Console.WriteLine("--- Example 1: Basic ThreadLocal ---\n");
DemonstrateBasicThreadLocal();

// Example 2: ThreadLocal cache for expensive computations
Console.WriteLine("\n--- Example 2: ThreadLocal Cache ---\n");
await DemonstrateThreadLocalCacheAsync();

// Example 3: ThreadLocal vs shared state comparison
Console.WriteLine("\n--- Example 3: Performance Comparison ---\n");
await DemonstratePerformanceComparisonAsync();

Console.WriteLine("\n✓ All ThreadLocal demonstrations completed!");

static void DemonstrateBasicThreadLocal()
{
    var threadLocalCounter = new ThreadLocal<int>(() => 0);
    var tasks = new List<Task>();

    Console.WriteLine("Creating 5 tasks, each with its own counter...\n");

    for (int i = 1; i <= 5; i++)
    {
        int taskId = i;
        tasks.Add(Task.Run(() =>
        {
            for (int j = 0; j < 5; j++)
            {
                threadLocalCounter.Value++;
                Console.WriteLine($"[Task {taskId}] Counter: {threadLocalCounter.Value}");
                Thread.Sleep(50);
            }
            Console.WriteLine($"[Task {taskId}] Final counter value: {threadLocalCounter.Value}");
        }));
    }

    Task.WhenAll(tasks).Wait();

    Console.WriteLine($"\nFinal thread-local value (main thread): {threadLocalCounter.Value}");
    threadLocalCounter.Dispose();
}

static async Task DemonstrateThreadLocalCacheAsync()
{
    var cache = new ThreadLocalCache();
    var tasks = new List<Task>();

    Console.WriteLine("Processing data with thread-local caching...\n");

    for (int i = 0; i < 10; i++)
    {
        int dataId = i;
        tasks.Add(Task.Run(() =>
        {
            var result = cache.ProcessWithCaching(dataId);
            Console.WriteLine($"[Task {Thread.CurrentThread.ManagedThreadId}] Processed {dataId}: {result}");
        }));
    }

    await Task.WhenAll(tasks);
    
    Console.WriteLine($"\nCache stats: {cache.CacheHitCount} hits, {cache.CacheMissCount} misses");
    cache.Dispose();
}

static async Task DemonstratePerformanceComparisonAsync()
{
    const int iterations = 100_000;
    const int threadCount = 8;

    // Test with ThreadLocal
    var threadLocalStopwatch = System.Diagnostics.Stopwatch.StartNew();
    var threadLocalResult = await RunThreadLocalTestAsync(iterations, threadCount);
    threadLocalStopwatch.Stop();

    // Test with shared concurrent dictionary
    var sharedStopwatch = System.Diagnostics.Stopwatch.StartNew();
    var sharedResult = await RunSharedStateTestAsync(iterations, threadCount);
    sharedStopwatch.Stop();

    Console.WriteLine($"ThreadLocal:  {threadLocalStopwatch.ElapsedMilliseconds}ms - Result: {threadLocalResult:N0}");
    Console.WriteLine($"Shared State: {sharedStopwatch.ElapsedMilliseconds}ms - Result: {sharedResult:N0}");
    Console.WriteLine($"Speedup: {sharedStopwatch.ElapsedMilliseconds / (double)threadLocalStopwatch.ElapsedMilliseconds:F2}x");
}

static async Task<long> RunThreadLocalTestAsync(int iterations, int threadCount)
{
    var threadLocalSum = new ThreadLocal<long>(() => 0);
    var tasks = new List<Task>();

    for (int i = 0; i < threadCount; i++)
    {
        tasks.Add(Task.Run(() =>
        {
            for (int j = 0; j < iterations; j++)
            {
                threadLocalSum.Value += j;
            }
        }));
    }

    await Task.WhenAll(tasks);
    
    // Sum up all thread-local values
    long total = threadLocalSum.Values.Sum();
    threadLocalSum.Dispose();
    
    return total;
}

static async Task<long> RunSharedStateTestAsync(int iterations, int threadCount)
{
    long sharedSum = 0;
    var tasks = new List<Task>();

    for (int i = 0; i < threadCount; i++)
    {
        tasks.Add(Task.Run(() =>
        {
            for (int j = 0; j < iterations; j++)
            {
                Interlocked.Add(ref sharedSum, j);
            }
        }));
    }

    await Task.WhenAll(tasks);
    return sharedSum;
}

/// <summary>
/// Thread-local cache for expensive computations
/// </summary>
public class ThreadLocalCache : IDisposable
{
    private readonly ThreadLocal<Dictionary<int, int>> _cache;
    private readonly Random _random = new();

    public ThreadLocalCache()
    {
        _cache = new ThreadLocal<Dictionary<int, int>>(() => new Dictionary<int, int>());
    }

    /// <summary>
    /// Process data with thread-local caching
    /// </summary>
    public int ProcessWithCaching(int dataId)
    {
        var cache = _cache.Value!;
        
        if (cache.TryGetValue(dataId, out var cachedResult))
        {
            CacheHitCount++;
            return cachedResult;
        }

        CacheMissCount++;
        
        // Simulate expensive computation
        Thread.Sleep(_random.Next(1, 10));
        var result = dataId * dataId + dataId;
        
        cache[dataId] = result;
        return result;
    }

    public long CacheHitCount { get; private set; }
    public long CacheMissCount { get; private set; }

    public void Dispose()
    {
        _cache.Dispose();
    }
}

/// <summary>
/// Thread-local random number generator
/// Avoids contention issues with shared Random instance
/// </summary>
public class ThreadLocalRandom
{
    private static readonly ThreadLocal<Random> _random = new(() => new Random());

    /// <summary>
    /// Get thread-local Random instance
    /// </summary>
    public static Random Instance => _random.Value!;

    /// <summary>
    /// Generate random integer
    /// </summary>
    public static int Next(int minValue, int maxValue)
    {
        return Instance.Next(minValue, maxValue);
    }

    /// <summary>
    /// Generate random double between 0.0 and 1.0
    /// </summary>
    public static double NextDouble()
    {
        return Instance.NextDouble();
    }
}

/// <summary>
/// Thread-local StringBuilder for efficient string building in multi-threaded scenarios
/// </summary>
public class ThreadLocalStringBuilder : IDisposable
{
    private readonly ThreadLocal<StringBuilder> _stringBuilders;

    public ThreadLocalStringBuilder()
    {
        _stringBuilders = new ThreadLocal<StringBuilder>(() => new StringBuilder());
    }

    /// <summary>
    /// Get current thread's StringBuilder
    /// </summary>
    public StringBuilder Current => _stringBuilders.Value!;

    /// <summary>
    /// Append line to current thread's builder
    /// </summary>
    public ThreadLocalStringBuilder AppendLine(string value)
    {
        Current.AppendLine(value);
        return this;
    }

    /// <summary>
    /// Get all string builders' contents
    /// </summary>
    public IEnumerable<string> GetAllContents()
    {
        return _stringBuilders.Values
            .Where(sb => sb != null && sb.Length > 0)
            .Select(sb => sb!.ToString());
    }

    public void Dispose()
    {
        _stringBuilders.Dispose();
    }
}

/// <summary>
/// Thread-local context for carrying request-specific data
/// </summary>
public class RequestContext : IDisposable
{
    private static readonly ThreadLocal<RequestContext?> _current = new();

    public static RequestContext? Current => _current.Value;

    public string RequestId { get; set; } = Guid.NewGuid().ToString("N")[..8];
    public DateTime StartTime { get; set; } = DateTime.UtcNow;
    public Dictionary<string, string> Metadata { get; set; } = new();

    /// <summary>
    /// Set current thread's request context
    /// </summary>
    public static IDisposable BeginContext(RequestContext context)
    {
        _current.Value = context;
        return new ContextDisposable();
    }

    private class ContextDisposable : IDisposable
    {
        public void Dispose()
        {
            _current.Value = null;
        }
    }

    public void Dispose()
    {
        _current.Value = null;
    }
}
