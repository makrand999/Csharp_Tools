using System.Collections.Generic;
using System.Linq;

namespace LruCache;

/// <summary>
/// LRU Cache - Least Recently Used cache implementation
/// Automatically evicts least recently used items when capacity is reached
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("LRU Cache Implementation");
        Console.WriteLine("========================\n");

        var cache = new LruCache<string, string>(capacity: 3);

        Console.WriteLine("Cache capacity: 3 items\n");

        // Add items to cache
        Console.WriteLine("--- Adding items ---");
        cache.Set("key1", "value1");
        Console.WriteLine($"Set key1=value1 -> {cache}");

        cache.Set("key2", "value2");
        Console.WriteLine($"Set key2=value2 -> {cache}");

        cache.Set("key3", "value3");
        Console.WriteLine($"Set key3=value3 -> {cache}");

        // Access key1 to make it recently used
        Console.WriteLine("\n--- Accessing key1 (making it recently used) ---");
        var val = cache.Get("key1");
        Console.WriteLine($"Get key1 = {val} -> {cache}");

        // Add new item - should evict key2 (least recently used)
        Console.WriteLine("\n--- Adding key4 (should evict key2) ---");
        cache.Set("key4", "value4");
        Console.WriteLine($"Set key4=value4 -> {cache}");

        // Try to get evicted key
        Console.WriteLine("\n--- Trying to access evicted key ---");
        val = cache.Get("key2");
        Console.WriteLine($"Get key2 = {(val ?? "null")} (was evicted)");

        // Access pattern demonstration with int values
        Console.WriteLine("\n--- Access pattern demonstration ---");
        var intCache = new LruCache<string, int>(capacity: 3);

        intCache.Set("A", 1);
        intCache.Set("B", 2);
        intCache.Set("C", 3);
        Console.WriteLine($"Initial: {intCache}");

        intCache.Get("A"); // Access A
        intCache.Set("D", 4); // Should evict B
        Console.WriteLine($"After Get(A) and Set(D): {intCache}");

        intCache.Get("C"); // Access C
        intCache.Set("E", 5); // Should evict D
        Console.WriteLine($"After Get(C) and Set(E): {intCache}");

        // Performance demo
        Console.WriteLine("\n--- Performance Test ---");
        var perfCache = new LruCache<int, string>(capacity: 1000);
        var random = new Random(42);

        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            var key = random.Next(1000);
            if (random.NextDouble() < 0.7)
            {
                perfCache.Set(key, $"value-{key}");
            }
            else
            {
                perfCache.Get(key);
            }
        }
        sw.Stop();

        Console.WriteLine($"10,000 operations in {sw.ElapsedMilliseconds}ms");
        Console.WriteLine($"Final cache state: {perfCache.Count} items (capacity: 1000)");
    }
}

class LruCache<TKey, TValue> where TKey : notnull where TValue : notnull
{
    private readonly int _capacity;
    private readonly Dictionary<TKey, LinkedListNode<CacheEntry<TKey, TValue>>> _map = new();
    private readonly LinkedList<CacheEntry<TKey, TValue>> _lruList = new();

    public int Count => _map.Count;
    public int Capacity => _capacity;

    public LruCache(int capacity)
    {
        _capacity = capacity;
    }

    public TValue? Get(TKey key)
    {
        if (!_map.TryGetValue(key, out var node))
        {
            return default;
        }

        // Move to front (most recently used)
        _lruList.Remove(node);
        _lruList.AddFirst(node);

        return node.Value.Value;
    }

    public void Set(TKey key, TValue value)
    {
        if (_map.TryGetValue(key, out var existingNode))
        {
            // Update existing
            existingNode.Value.Value = value;
            _lruList.Remove(existingNode);
            _lruList.AddFirst(existingNode);
        }
        else
        {
            // Add new
            var entry = new CacheEntry<TKey, TValue>(key, value);
            var newNode = new LinkedListNode<CacheEntry<TKey, TValue>>(entry);
            _map[key] = newNode;
            _lruList.AddFirst(newNode);

            // Evict if over capacity
            if (_lruList.Count > _capacity)
            {
                var lruNode = _lruList.Last!;
                _lruList.Remove(lruNode);
                _map.Remove(lruNode.Value.Key);
            }
        }
    }

    public override string ToString()
    {
        var items = new List<string>();
        foreach (var entry in _lruList)
        {
            items.Add($"[{entry.Key}:{entry.Value}]");
        }
        return $"[{string.Join(", ", items)}]";
    }
}

class CacheEntry<TKey, TValue> where TKey : notnull
{
    public TKey Key { get; set; }
    public TValue Value { get; set; }

    public CacheEntry(TKey key, TValue value)
    {
        Key = key;
        Value = value;
    }
}
