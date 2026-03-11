using System.Collections.Concurrent;

namespace ConcurrentCache;

/// <summary>
/// Concurrent Cache - Thread-safe LRU (Least Recently Used) cache implementation.
/// Supports automatic expiration and concurrent access from multiple threads.
/// </summary>
public class ConcurrentCache<TKey, TValue> where TKey : notnull
{
    private readonly ConcurrentDictionary<TKey, CacheEntry> _cache;
    private readonly LinkedList<TKey> _lruList;
    private readonly int _maxCapacity;
    private readonly TimeSpan _defaultExpiration;
    private readonly object _lruLock = new();

    private record CacheEntry(TValue Value, DateTime ExpiresAt, LinkedListNode<TKey>? LruNode);

    public ConcurrentCache(int maxCapacity = 100, TimeSpan? defaultExpiration = null)
    {
        _maxCapacity = maxCapacity;
        _defaultExpiration = defaultExpiration ?? TimeSpan.FromMinutes(30);
        _cache = new ConcurrentDictionary<TKey, CacheEntry>();
        _lruList = new LinkedList<TKey>();
    }

    public bool TryGet(TKey key, out TValue value)
    {
        value = default!;

        if (!_cache.TryGetValue(key, out var entry))
            return false;

        // Check expiration
        if (DateTime.UtcNow > entry.ExpiresAt)
        {
            Remove(key);
            return false;
        }

        // Update LRU order
        UpdateLru(key);

        value = entry.Value;
        return true;
    }

    public void Set(TKey key, TValue value, TimeSpan? expiration = null)
    {
        var expiresAt = DateTime.UtcNow + (expiration ?? _defaultExpiration);

        // Check capacity and evict if necessary
        while (_cache.Count >= _maxCapacity)
        {
            EvictLeastRecentlyUsed();
        }

        var entry = new CacheEntry(value, expiresAt, null);

        if (_cache.TryGetValue(key, out var existingEntry))
        {
            // Update existing entry
            _cache[key] = entry;
            UpdateLru(key);
        }
        else
        {
            // Add new entry
            lock (_lruLock)
            {
                var node = _lruList.AddLast(key);
                entry = entry with { LruNode = node };
                _cache[key] = entry;
            }
        }
    }

    public bool Remove(TKey key)
    {
        if (_cache.TryRemove(key, out var entry))
        {
            lock (_lruLock)
            {
                if (entry.LruNode != null)
                {
                    _lruList.Remove(entry.LruNode);
                }
            }
            return true;
        }
        return false;
    }

    public void Clear()
    {
        _cache.Clear();
        lock (_lruLock)
        {
            _lruList.Clear();
        }
    }

    public int Count => _cache.Count;

    public void RemoveExpired()
    {
        var now = DateTime.UtcNow;
        var expiredKeys = _cache
            .Where(kvp => kvp.Value.ExpiresAt < now)
            .Select(kvp => kvp.Key)
            .ToList();

        foreach (var key in expiredKeys)
        {
            Remove(key);
        }

        if (expiredKeys.Count > 0)
        {
            Console.WriteLine($"Removed {expiredKeys.Count} expired entries");
        }
    }

    public CacheStatistics GetStatistics()
    {
        var now = DateTime.UtcNow;
        var expired = _cache.Count(kvp => kvp.Value.ExpiresAt < now);
        
        return new CacheStatistics
        {
            TotalItems = _cache.Count,
            ExpiredItems = expired,
            Capacity = _maxCapacity,
            Utilization = (double)_cache.Count / _maxCapacity * 100
        };
    }

    private void UpdateLru(TKey key)
    {
        lock (_lruLock)
        {
            if (_cache.TryGetValue(key, out var entry) && entry.LruNode != null)
            {
                _lruList.Remove(entry.LruNode);
                var newNode = _lruList.AddLast(key);
                _cache[key] = entry with { LruNode = newNode };
            }
        }
    }

    private void EvictLeastRecentlyUsed()
    {
        lock (_lruLock)
        {
            if (_lruList.First != null)
            {
                var lruKey = _lruList.First.Value;
                _lruList.RemoveFirst();
                _cache.TryRemove(lruKey, out _);
                Console.WriteLine($"Evicted LRU entry: {lruKey}");
            }
        }
    }
}

public record CacheStatistics
{
    public int TotalItems { get; init; }
    public int ExpiredItems { get; init; }
    public int Capacity { get; init; }
    public double Utilization { get; init; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Concurrent LRU Cache ===\n");

        var cache = new ConcurrentCache<string, string>(maxCapacity: 5, TimeSpan.FromSeconds(5));

        // Add items to cache
        Console.WriteLine("Adding items to cache...");
        for (int i = 1; i <= 7; i++)
        {
            var key = $"key{i}";
            var value = $"value{i}";
            cache.Set(key, value);
            Console.WriteLine($"  Set: {key} = {value}");
        }

        Console.WriteLine($"\nCache count: {cache.Count} (max capacity: 5)");

        // Access some items to update LRU order
        Console.WriteLine("\nAccessing key3 and key5...");
        if (cache.TryGet("key3", out var value3))
            Console.WriteLine($"  Got: key3 = {value3}");
        
        if (cache.TryGet("key5", out var value5))
            Console.WriteLine($"  Got: key5 = {value5}");

        // Try to get evicted items
        Console.WriteLine("\nTrying to access evicted items (key1, key2)...");
        if (!cache.TryGet("key1", out _))
            Console.WriteLine("  key1 was evicted (LRU)");
        
        if (!cache.TryGet("key2", out _))
            Console.WriteLine("  key2 was evicted (LRU)");

        // Display statistics
        var stats = cache.GetStatistics();
        Console.WriteLine($"\n=== Cache Statistics ===");
        Console.WriteLine($"Total Items: {stats.TotalItems}");
        Console.WriteLine($"Capacity: {stats.Capacity}");
        Console.WriteLine($"Utilization: {stats.Utilization:F1}%");

        // Test expiration
        Console.WriteLine("\nWaiting 6 seconds for expiration...");
        Thread.Sleep(6000);

        cache.RemoveExpired();

        Console.WriteLine($"\nAfter expiration - Cache count: {cache.Count}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
