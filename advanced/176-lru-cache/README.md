# LRU Cache

Least Recently Used (LRU) cache implementation with automatic eviction of least recently accessed items when capacity is reached.

## Usage

```bash
dotnet run --project 176-lru-cache/LruCache.csproj
```

## Example

```
LRU Cache Implementation
========================

Cache capacity: 3 items

--- Adding items ---
Set key1=value1 -> [[key1:value1]]
Set key2=value2 -> [[key2:value2], [key1:value1]]
Set key3=value3 -> [[key3:value3], [key2:value2], [key1:value1]]

--- Accessing key1 (making it recently used) ---
Get key1 = value1 -> [[key1:value1], [key3:value3], [key2:value2]]

--- Adding key4 (should evict key2) ---
Set key4=value4 -> [[key4:value4], [key1:value1], [key3:value3]]

--- Trying to access evicted key ---
Get key2 = null (was evicted)

--- Performance Test ---
10,000 operations in 45ms
Final cache state: 1000 items (capacity: 1000)
```

## How LRU Works

1. **Get**: Access moves item to front (most recently used)
2. **Set**: New items added to front
3. **Eviction**: When full, remove from back (least recently used)

## Concepts Demonstrated

- Doubly linked list for O(1) operations
- Dictionary + LinkedList combination
- O(1) get and put operations
- Cache eviction policies
- Generic type constraints
- Custom ToString formatting
