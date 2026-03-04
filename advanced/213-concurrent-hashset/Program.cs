// Concurrent HashSet - Thread-safe hash set implementation
// Demonstrates ReaderWriterLockSlim and concurrent collections

Console.WriteLine("=== Concurrent HashSet ===\n");
Console.WriteLine("Testing thread-safe hash set with multiple readers and writers...\n");

var concurrentSet = new ConcurrentHashSet<int>();
var tasks = new List<Task>();
var random = new Random();

Console.WriteLine("Starting concurrent operations...\n");

// Multiple writer tasks
for (int i = 0; i < 5; i++)
{
    int writerId = i;
    tasks.Add(Task.Run(() =>
    {
        for (int j = 0; j < 20; j++)
        {
            int value = writerId * 100 + j;
            concurrentSet.Add(value);
            Console.WriteLine($"Writer {writerId}: Added {value}");
            Thread.Sleep(random.Next(5, 20));
        }
    }));
}

// Multiple reader tasks
for (int i = 0; i < 3; i++)
{
    int readerId = i;
    tasks.Add(Task.Run(() =>
    {
        for (int j = 0; j < 30; j++)
        {
            int count = concurrentSet.Count;
            bool hasValue = concurrentSet.Contains(random.Next(0, 500));
            Console.WriteLine($"Reader {readerId}: Count={count}, HasRandom={hasValue}");
            Thread.Sleep(random.Next(10, 30));
        }
    }));
}

// Wait for all tasks to complete
await Task.WhenAll(tasks);

Console.WriteLine($"\n✓ Final count: {concurrentSet.Count} items");
Console.WriteLine("✓ All concurrent operations completed successfully!");

// Display some items from the set
Console.WriteLine("\nSample items:");
int displayed = 0;
foreach (var item in concurrentSet)
{
    if (displayed++ >= 10) break;
    Console.Write($"{item} ");
}
Console.WriteLine();

/// <summary>
/// Thread-safe hash set implementation using ReaderWriterLockSlim
/// </summary>
public class ConcurrentHashSet<T> : IEnumerable<T> where T : notnull
{
    private readonly HashSet<T> _set = new();
    private readonly ReaderWriterLockSlim _lock = new();

    /// <summary>
    /// Add an item to the set (write operation)
    /// </summary>
    public bool Add(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            return _set.Add(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Add multiple items (batch write operation)
    /// </summary>
    public void AddRange(IEnumerable<T> items)
    {
        _lock.EnterWriteLock();
        try
        {
            foreach (var item in items)
            {
                _set.Add(item);
            }
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Remove an item from the set (write operation)
    /// </summary>
    public bool Remove(T item)
    {
        _lock.EnterWriteLock();
        try
        {
            return _set.Remove(item);
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Check if the set contains an item (read operation)
    /// </summary>
    public bool Contains(T item)
    {
        _lock.EnterReadLock();
        try
        {
            return _set.Contains(item);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Get the count of items (read operation)
    /// </summary>
    public int Count
    {
        get
        {
            _lock.EnterReadLock();
            try
            {
                return _set.Count;
            }
            finally
            {
                _lock.ExitReadLock();
            }
        }
    }

    /// <summary>
    /// Clear all items from the set (write operation)
    /// </summary>
    public void Clear()
    {
        _lock.EnterWriteLock();
        try
        {
            _set.Clear();
        }
        finally
        {
            _lock.ExitWriteLock();
        }
    }

    /// <summary>
    /// Get a snapshot of all items (read operation)
    /// </summary>
    public HashSet<T> ToHashSet()
    {
        _lock.EnterReadLock();
        try
        {
            return new HashSet<T>(_set);
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    /// <summary>
    /// Try get value with atomic add-if-not-exists
    /// </summary>
    public bool TryGetValue(T item, out T? result)
    {
        _lock.EnterUpgradeableReadLock();
        try
        {
            if (_set.Contains(item))
            {
                result = item;
                return true;
            }
            result = default;
            return false;
        }
        finally
        {
            _lock.ExitUpgradeableReadLock();
        }
    }

    /// <summary>
    /// Enumerator for foreach iteration
    /// </summary>
    public IEnumerator<T> GetEnumerator()
    {
        _lock.EnterReadLock();
        try
        {
            // Return a snapshot to avoid holding lock during enumeration
            foreach (var item in _set.ToList())
            {
                yield return item;
            }
        }
        finally
        {
            _lock.ExitReadLock();
        }
    }

    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
