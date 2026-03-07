# Thread Local Cache

Demonstrates `ThreadLocal<T>` for per-thread storage and caching to avoid contention in multi-threaded scenarios.

## Usage

```bash
dotnet run --project ThreadLocalCache.csproj
```

## Example

```
=== Thread Local Cache ===

Using ThreadLocal<T> for per-thread caching...

--- Example 1: Basic ThreadLocal ---

Creating 5 tasks, each with its own counter...

[Task 1] Counter: 1
[Task 1] Counter: 2
[Task 2] Counter: 1
[Task 2] Counter: 2
...
[Task 1] Final counter value: 5
[Task 2] Final counter value: 5

--- Example 2: ThreadLocal Cache ---

Processing data with thread-local caching...

[Task 5] Processed 0: 0
[Task 6] Processed 1: 2
...

Cache stats: 0 hits, 10 misses

--- Example 3: Performance Comparison ---

ThreadLocal:  45ms - Result: 3,999,960,000
Shared State: 189ms - Result: 3,999,960,000
Speedup: 4.20x
```

## Concepts Demonstrated

- `ThreadLocal<T>` for per-thread storage
- Thread-local caching patterns
- `ThreadLocal.Values` for aggregating results
- Performance benefits of thread-local state
- Thread-local Random for contention-free randomness
- Thread-local StringBuilder for efficient logging
- Request context propagation

## Common Use Cases

- Per-thread caches (avoid contention)
- Thread-safe Random number generation
- Per-thread buffers/builders
- Request context in async applications
- Avoiding lock contention in hot paths
