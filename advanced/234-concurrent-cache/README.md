# Concurrent LRU Cache

Thread-safe LRU (Least Recently Used) cache implementation with automatic expiration and concurrent access support.

## Usage

```bash
dotnet run --project ConcurrentCache.csproj
```

## Example

```
=== Concurrent LRU Cache ===

Adding items to cache...
  Set: key1 = value1
  Set: key2 = value2
...

Cache count: 5 (max capacity: 5)

Accessing key3 and key5...
  Got: key3 = value3
  Got: key5 = value5

=== Cache Statistics ===
Total Items: 5
Capacity: 5
Utilization: 100.0%
```

## Concepts Demonstrated

- ConcurrentDictionary for thread-safe storage
- LinkedList for LRU ordering
- Cache eviction policies
- Time-based expiration
- Record types for immutable data
