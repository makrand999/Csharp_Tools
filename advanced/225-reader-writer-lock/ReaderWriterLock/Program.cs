namespace ReaderWriterLock;

/// <summary>
/// Async reader-writer lock implementation for concurrent read/write scenarios.
/// Demonstrates multiple-reader single-writer pattern with async support.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Async Reader-Writer Lock ===\n");
        
        var cache = new AsyncCache<string, string>();
        
        // Writer task
        var writerTask = Task.Run(() => WriterAsync(cache));
        
        // Multiple reader tasks
        var readerTasks = Enumerable.Range(0, 5)
            .Select(id => Task.Run(() => ReaderAsync(cache, id)))
            .ToList();
        
        await writerTask;
        await Task.WhenAll(readerTasks);
        
        Console.WriteLine($"\nCache statistics:");
        Console.WriteLine($"  Total reads: {cache.ReadCount}");
        Console.WriteLine($"  Total writes: {cache.WriteCount}");
        
        // Demo basic operations
        await DemonstrateLock();
    }
    
    static async Task WriterAsync(AsyncCache<string, string> cache)
    {
        for (int i = 0; i < 10; i++)
        {
            string key = $"key-{i}";
            string value = $"value-{i:D3}";
            await cache.WriteAsync(key, value);
            Console.WriteLine($"Writer: Wrote {key} = {value}");
            await Task.Delay(200);
        }
    }
    
    static async Task ReaderAsync(AsyncCache<string, string> cache, int readerId)
    {
        for (int i = 0; i < 20; i++)
        {
            string key = $"key-{Random.Shared.Next(0, 10)}";
            string? value = await cache.ReadAsync(key);
            Console.WriteLine($"Reader {readerId}: Read {key} = {value ?? "null"}");
            await Task.Delay(50);
        }
    }
    
    static async Task DemonstrateLock()
    {
        Console.WriteLine("\n=== Reader-Writer Lock Demo ===\n");
        
        var rwLock = new AsyncReaderWriterLock();
        var sharedData = new List<int>();
        
        var tasks = new List<Task>();
        
        // Multiple readers
        for (int i = 0; i < 3; i++)
        {
            int readerId = i;
            tasks.Add(Task.Run(async () =>
            {
                for (int j = 0; j < 3; j++)
                {
                    using (await rwLock.ReadLockAsync())
                    {
                        Console.WriteLine($"Reader {readerId}: Reading (count={sharedData.Count})");
                        await Task.Delay(100);
                    }
                }
            }));
        }
        
        // Single writer
        tasks.Add(Task.Run(async () =>
        {
            for (int i = 0; i < 3; i++)
            {
                using (await rwLock.WriteLockAsync())
                {
                    sharedData.Add(i);
                    Console.WriteLine($"Writer: Writing item {i}");
                    await Task.Delay(150);
                }
            }
        }));
        
        await Task.WhenAll(tasks);
        Console.WriteLine($"\nFinal data count: {sharedData.Count}");
    }
}

/// <summary>
/// Thread-safe async cache with reader-writer locking.
/// </summary>
class AsyncCache<TKey, TValue> where TKey : notnull
{
    private readonly Dictionary<TKey, TValue> _data = new();
    private readonly AsyncReaderWriterLock _lock = new();
    private int _readCount;
    private int _writeCount;
    
    public int ReadCount => Volatile.Read(ref _readCount);
    public int WriteCount => Volatile.Read(ref _writeCount);
    
    public async ValueTask<TValue?> ReadAsync(TKey key)
    {
        using (await _lock.ReadLockAsync())
        {
            Interlocked.Increment(ref _readCount);
            return _data.TryGetValue(key, out var value) ? value : default;
        }
    }
    
    public async ValueTask WriteAsync(TKey key, TValue value)
    {
        using (await _lock.WriteLockAsync())
        {
            Interlocked.Increment(ref _writeCount);
            _data[key] = value;
        }
    }
}

/// <summary>
/// Async reader-writer lock allowing multiple concurrent readers or single writer.
/// </summary>
class AsyncReaderWriterLock : IDisposable
{
    private readonly SemaphoreSlim _readSemaphore = new(int.MaxValue);
    private readonly SemaphoreSlim _writeSemaphore = new(1);
    private int _readers;
    
    public async ValueTask<IDisposable> ReadLockAsync()
    {
        await _readSemaphore.WaitAsync();
        var readers = Interlocked.Increment(ref _readers);
        
        if (readers == 1)
        {
            await _writeSemaphore.WaitAsync();
        }
        
        return new ReadLockReleaser(this);
    }
    
    public async ValueTask<IDisposable> WriteLockAsync()
    {
        await _writeSemaphore.WaitAsync();
        return new WriteLockReleaser(this);
    }
    
    private void ReleaseRead()
    {
        var readers = Interlocked.Decrement(ref _readers);
        _readSemaphore.Release();
        
        if (readers == 0)
        {
            _writeSemaphore.Release();
        }
    }
    
    private void ReleaseWrite() => _writeSemaphore.Release();
    
    public void Dispose()
    {
        _readSemaphore.Dispose();
        _writeSemaphore.Dispose();
    }
    
    private class ReadLockReleaser(AsyncReaderWriterLock owner) : IDisposable
    {
        public void Dispose() => owner.ReleaseRead();
    }
    
    private class WriteLockReleaser(AsyncReaderWriterLock owner) : IDisposable
    {
        public void Dispose() => owner.ReleaseWrite();
    }
}
