using System;
using System.Collections.Concurrent;

namespace WeakReference;

/// <summary>
/// Demonstrates WeakReference for memory-sensitive caching.
/// Shows how to create caches that allow GC to collect unused objects.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Weak Reference Cache Demo ===\n");
        Console.WriteLine("Demonstrates memory-sensitive caching with WeakReference.\n");

        // Demo 1: Basic weak reference
        Console.WriteLine("--- Basic WeakReference ---\n");
        BasicWeakReferenceDemo();

        // Demo 2: Weak cache
        Console.WriteLine("\n--- Weak Cache ---\n");
        WeakCacheDemo();

        // Demo 3: Memory pressure simulation
        Console.WriteLine("\n--- Memory Pressure Simulation ---\n");
        MemoryPressureDemo();
    }

    static void BasicWeakReferenceDemo()
    {
        var data = new LargeDataObject("Test Data", 1000);
        var weakRef = new WeakReference<LargeDataObject>(data);

        Console.WriteLine($"Created object: {data.Name}");
        Console.WriteLine($"Can get target: {weakRef.TryGetTarget(out _)}");

        // Try to get the object
        if (weakRef.TryGetTarget(out var target))
        {
            Console.WriteLine($"Retrieved from weak ref: {target.Name}");
        }

        // Release strong reference
        data = null;
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        // Check if still alive
        Console.WriteLine($"After GC, Can get target: {weakRef.TryGetTarget(out _)}");
        if (!weakRef.TryGetTarget(out _))
        {
            Console.WriteLine("Object was collected (no strong references)");
        }
    }

    static void WeakCacheDemo()
    {
        var cache = new WeakCache<string, LargeDataObject>();

        // Add items to cache
        for (int i = 1; i <= 5; i++)
        {
            var data = new LargeDataObject($"Item_{i}", 500);
            cache.Set($"key{i}", data);
            Console.WriteLine($"Cached: key{i}");
        }

        Console.WriteLine($"\nCache count (before cleanup): {cache.Count}");

        // Retrieve items
        for (int i = 1; i <= 5; i++)
        {
            if (cache.TryGet($"key{i}", out var data))
            {
                Console.WriteLine($"Retrieved: key{i} = {data.Name}");
            }
            else
            {
                Console.WriteLine($"Cache miss: key{i}");
            }
        }

        // Cleanup dead references
        cache.Cleanup();
        Console.WriteLine($"\nCache count (after cleanup): {cache.Count}");
    }

    static void MemoryPressureDemo()
    {
        var cache = new WeakCache<int, LargeDataObject>();
        
        Console.WriteLine("Creating 10 large objects in cache...");
        for (int i = 0; i < 10; i++)
        {
            cache.Set(i, new LargeDataObject($"Object_{i}", 5000));
        }

        Console.WriteLine($"Initial cache size: {cache.Count}");
        Console.WriteLine("Simulating memory pressure...");

        // Allocate more memory to trigger GC
        var pressure = new List<byte[]>();
        for (int i = 0; i < 100; i++)
        {
            pressure.Add(new byte[100_000]); // 100KB each
        }

        GC.Collect(2, GCCollectionMode.Forced, blocking: true);
        GC.WaitForPendingFinalizers();
        GC.Collect();

        cache.Cleanup();
        Console.WriteLine($"Cache size after GC: {cache.Count}");
        
        // Show remaining items
        Console.WriteLine("Remaining cached items:");
        foreach (var kvp in cache.GetAll())
        {
            Console.WriteLine($"  - {kvp.Key}: {kvp.Value.Name}");
        }

        // Release pressure
        pressure.Clear();
    }
}

/// <summary>
/// A large data object that simulates memory-intensive data
/// </summary>
public class LargeDataObject
{
    public string Name { get; }
    public byte[] Data { get; }

    public LargeDataObject(string name, int size)
    {
        Name = name;
        Data = new byte[size * 1024]; // Size in KB
        new Random().NextBytes(Data);
    }

    public override string ToString() => Name;
}

/// <summary>
/// Thread-safe cache using WeakReference
/// </summary>
public class WeakCache<TKey, TValue> where TKey : notnull where TValue : class
{
    private readonly ConcurrentDictionary<TKey, WeakReference<TValue>> _cache = new();

    public void Set(TKey key, TValue value)
    {
        _cache[key] = new WeakReference<TValue>(value);
    }

    public bool TryGet(TKey key, out TValue? value)
    {
        if (_cache.TryGetValue(key, out var weakRef))
        {
            return weakRef.TryGetTarget(out value);
        }
        value = null;
        return false;
    }

    public int Count => _cache.Count(kvp => kvp.Value.TryGetTarget(out _));

    public void Cleanup()
    {
        var deadKeys = new List<TKey>();
        
        foreach (var kvp in _cache)
        {
            if (!kvp.Value.TryGetTarget(out _))
            {
                deadKeys.Add(kvp.Key);
            }
        }

        foreach (var key in deadKeys)
        {
            _cache.TryRemove(key, out _);
        }
    }

    public IEnumerable<KeyValuePair<TKey, TValue>> GetAll()
    {
        foreach (var kvp in _cache)
        {
            if (kvp.Value.TryGetTarget(out var value))
            {
                yield return new KeyValuePair<TKey, TValue>(kvp.Key, value);
            }
        }
    }
}
