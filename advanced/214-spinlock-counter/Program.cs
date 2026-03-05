// SpinLock Counter - High-performance locking with SpinLock
// Demonstrates low-level synchronization for short critical sections

Console.WriteLine("=== SpinLock Counter ===\n");
Console.WriteLine("Comparing SpinLock vs lock for high-frequency operations...\n");

const int iterations = 100_000;
const int threadCount = 8;

// Test with SpinLock
Console.WriteLine($"Running SpinLock test with {threadCount} threads, {iterations:N0} iterations each...\n");
var spinLockCounter = new SpinLockCounter();
var spinLockTime = await RunSpinLockTestAsync(spinLockCounter, iterations, threadCount);
Console.WriteLine($"\nSpinLock Result: {spinLockCounter.Count:N0} in {spinLockTime.TotalMilliseconds:F2}ms");

// Test with regular lock
Console.WriteLine($"\nRunning lock test with {threadCount} threads, {iterations:N0} iterations each...\n");
var lockCounter = new LockCounter();
var lockTime = await RunLockTestAsync(lockCounter, iterations, threadCount);
Console.WriteLine($"\nLock Result: {lockCounter.Count:N0} in {lockTime.TotalMilliseconds:F2}ms");

// Compare results
Console.WriteLine($"\n=== Comparison ===");
Console.WriteLine($"SpinLock: {spinLockTime.TotalMilliseconds:F2}ms");
Console.WriteLine($"Regular lock: {lockTime.TotalMilliseconds:F2}ms");
Console.WriteLine($"Speedup: {lockTime.TotalMilliseconds / spinLockTime.TotalMilliseconds:F2}x");

static async Task<TimeSpan> RunSpinLockTestAsync(SpinLockCounter counter, int iterations, int threadCount)
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

static async Task<TimeSpan> RunLockTestAsync(LockCounter counter, int iterations, int threadCount)
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
/// Counter using SpinLock for high-performance synchronization
/// Best for very short critical sections where waiting threads should spin instead of block
/// </summary>
public class SpinLockCounter
{
    private SpinLock _spinLock = new(false); // false = don't track owner
    private long _count;

    /// <summary>
    /// Increment counter using SpinLock
    /// </summary>
    public void Increment()
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            _count++;
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }
    }

    /// <summary>
    /// Decrement counter using SpinLock
    /// </summary>
    public void Decrement()
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            _count--;
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }
    }

    /// <summary>
    /// Add value to counter using SpinLock
    /// </summary>
    public void Add(long value)
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            _count += value;
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }
    }

    /// <summary>
    /// Get current count
    /// </summary>
    public long Count => Volatile.Read(ref _count);

    /// <summary>
    /// Reset counter to zero
    /// </summary>
    public void Reset()
    {
        bool lockTaken = false;
        try
        {
            _spinLock.Enter(ref lockTaken);
            _count = 0;
        }
        finally
        {
            if (lockTaken)
            {
                _spinLock.Exit();
            }
        }
    }
}

/// <summary>
/// Counter using regular lock statement for comparison
/// </summary>
public class LockCounter
{
    private readonly object _lock = new();
    private long _count;

    public void Increment()
    {
        lock (_lock)
        {
            _count++;
        }
    }

    public long Count
    {
        get
        {
            lock (_lock)
            {
                return _count;
            }
        }
    }

    public void Reset()
    {
        lock (_lock)
        {
            _count = 0;
        }
    }
}
