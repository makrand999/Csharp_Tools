# Weak Reference Cache

Demonstrates `WeakReference<T>` for memory-sensitive caching. Shows how to create caches that allow the garbage collector to reclaim memory when under pressure.

## Usage

```bash
dotnet run --project WeakReference.csproj
```

## Example

```
=== Weak Reference Cache Demo ===

--- Basic WeakReference ---

Created object: Test Data
IsAlive: True
Retrieved from weak ref: Test Data
After GC, IsAlive: False
Object was collected (no strong references)

--- Weak Cache ---

Cached: key1
Cached: key2
Cached: key3
Cached: key4
Cached: key5

Cache count (before cleanup): 5
Retrieved: key1 = Item_1
Retrieved: key2 = Item_2
Retrieved: key3 = Item_3
Retrieved: key4 = Item_4
Retrieved: key5 = Item_5

Cache count (after cleanup): 5

--- Memory Pressure Simulation ---

Creating 10 large objects in cache...
Initial cache size: 10
Simulating memory pressure...
Cache size after GC: 3
Remaining cached items:
  - 2: Object_2
  - 5: Object_5
  - 7: Object_7
```

## Concepts Demonstrated

- WeakReference<T> for non-rooting references
- IsAlive property checking
- TryGetTarget for safe retrieval
- Memory-sensitive caching patterns
- ConcurrentDictionary for thread safety
- GC pressure simulation
- Automatic cleanup of dead references
