// Volatile Counter - Demonstrates volatile keyword and memory barriers
// Shows atomic operations and low-level thread synchronization

Console.WriteLine("=== Volatile Counter ===\n");
Console.WriteLine("Testing volatile operations vs regular operations...\n");

const int iterations = 1_000_000;
const int threadCount = 8;

// Test 1: Volatile counter
Console.WriteLine($"Test 1: Volatile counter with {threadCount} threads...\n");
var volatileCounter = new VolatileCounter();
var volatileTime = await RunVolatileTestAsync(volatileCounter, iterations, threadCount);
Console.WriteLine($"\nVolatile Result: {volatileCounter.Count:N0} in {volatileTime.TotalMilliseconds:F2}ms");

// Test 2: Interlocked counter
Console.WriteLine($"\nTest 2: Interlocked counter with {threadCount} threads...\n");
var interlockedCounter = new InterlockedCounter();
var interlockedTime = await RunInterlockedTestAsync(interlockedCounter, iterations, threadCount);
Console.WriteLine($"\nInterlocked Result: {interlockedCounter.Count:N0} in {interlockedTime.TotalMilliseconds:F2}ms");

// Test 3: Regular counter (demonstrates race condition)
Console.WriteLine($"\nTest 3: Regular counter (demonstrates race condition)...\n");
var regularCounter = new RegularCounter();
var regularTime = await RunRegularTestAsync(regularCounter, iterations, threadCount);
Console.WriteLine($"\nRegular Result: {regularCounter.Count:N0} in {regularTime.TotalMilliseconds:F2}ms");
Console.WriteLine($"Expected: {iterations * threadCount:N0}, Lost: {iterations * threadCount - regularCounter.Count:N0}");

Console.WriteLine($"\n=== Summary ===");
Console.WriteLine($"Volatile:    {volatileTime.TotalMilliseconds:F2}ms - Count: {volatileCounter.Count:N0} ✓");
Console.WriteLine($"Interlocked: {interlockedTime.TotalMilliseconds:F2}ms - Count: {interlockedCounter.Count:N0} ✓");
Console.WriteLine($"Regular:     {regularTime.TotalMilliseconds:F2}ms - Count: {regularCounter.Count:N0} ✗ (race condition)");

static async Task<TimeSpan> RunVolatileTestAsync(VolatileCounter counter, int iterations, int threadCount)
{
    var tasks = new List<Task>();
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    for (int i = 0; i < threadCount; i++)
    {
        tasks.Add(Task.Run(() =>
        {
            for (int j = 0; j < iterations; j++)
            {
                counter.IncrementVolatile();
            }
        }));
    }

    await Task.WhenAll(tasks);
    stopwatch.Stop();
    return stopwatch.Elapsed;
}

static async Task<TimeSpan> RunInterlockedTestAsync(InterlockedCounter counter, int iterations, int threadCount)
{
    var tasks = new List<Task>();
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    for (int i = 0; i < threadCount; i++)
    {
        tasks.Add(Task.Run(() =>
        {
            for (int j = 0; j < iterations; j++)
            {
                counter.Increment();
            }
        }));
    }

    await Task.WhenAll(tasks);
    stopwatch.Stop();
    return stopwatch.Elapsed;
}

static async Task<TimeSpan> RunRegularTestAsync(RegularCounter counter, int iterations, int threadCount)
{
    var tasks = new List<Task>();
    var stopwatch = System.Diagnostics.Stopwatch.StartNew();

    for (int i = 0; i < threadCount; i++)
    {
        tasks.Add(Task.Run(() =>
        {
            for (int j = 0; j < iterations; j++)
            {
                counter.Increment();
            }
        }));
    }

    await Task.WhenAll(tasks);
    stopwatch.Stop();
    return stopwatch.Elapsed;
}

/// <summary>
/// Counter using volatile and manual memory barriers
/// Demonstrates proper use of volatile for visibility across threads
/// </summary>
public class VolatileCounter
{
    private volatile int _count;

    /// <summary>
    /// Increment using volatile read/write with memory barrier
    /// </summary>
    public void IncrementVolatile()
    {
        // Volatile.Read ensures we get the most recent value
        int current = Volatile.Read(ref _count);
        
        // Memory barrier ensures ordering
        Thread.MemoryBarrier();
        
        // Volatile.Write ensures the write is visible to other threads immediately
        Volatile.Write(ref _count, current + 1);
    }

    /// <summary>
    /// Get current count with volatile read
    /// </summary>
    public int Count => Volatile.Read(ref _count);

    /// <summary>
    /// Reset counter
    /// </summary>
    public void Reset()
    {
        Volatile.Write(ref _count, 0);
    }
}

/// <summary>
/// Counter using Interlocked for atomic operations
/// This is the recommended approach for simple atomic counters
/// </summary>
public class InterlockedCounter
{
    private long _count;

    /// <summary>
    /// Increment using Interlocked (atomic operation)
    /// </summary>
    public void Increment()
    {
        Interlocked.Increment(ref _count);
    }

    /// <summary>
    /// Decrement using Interlocked
    /// </summary>
    public void Decrement()
    {
        Interlocked.Decrement(ref _count);
    }

    /// <summary>
    /// Add value using Interlocked
    /// </summary>
    public void Add(long value)
    {
        Interlocked.Add(ref _count, value);
    }

    /// <summary>
    /// Exchange value atomically
    /// </summary>
    public long Exchange(long newValue)
    {
        return Interlocked.Exchange(ref _count, newValue);
    }

    /// <summary>
    /// Compare and swap atomically
    /// </summary>
    public long CompareExchange(long value, long comparand)
    {
        return Interlocked.CompareExchange(ref _count, value, comparand);
    }

    /// <summary>
    /// Get current count
    /// </summary>
    public long Count => Volatile.Read(ref _count);

    /// <summary>
    /// Reset counter
    /// </summary>
    public void Reset()
    {
        Interlocked.Exchange(ref _count, 0);
    }
}

/// <summary>
/// Regular counter without synchronization
/// Demonstrates race conditions in multi-threaded scenarios
/// </summary>
public class RegularCounter
{
    private long _count;

    /// <summary>
    /// Increment without synchronization (NOT THREAD-SAFE!)
    /// </summary>
    public void Increment()
    {
        _count++; // Race condition: read-modify-write is not atomic
    }

    /// <summary>
    /// Get current count (may be stale due to no memory barriers)
    /// </summary>
    public long Count => _count;
}
