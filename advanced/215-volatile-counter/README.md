# Volatile Counter

Demonstrates the `volatile` keyword, `Volatile.Read/Write`, and `Interlocked` operations for atomic operations and memory barriers.

## Usage

```bash
dotnet run --project VolatileCounter.csproj
```

## Example

```
=== Volatile Counter ===

Testing volatile operations vs regular operations...

Test 1: Volatile counter with 8 threads...

Volatile Result: 8,000,000 in 125.45ms

Test 2: Interlocked counter with 8 threads...

Interlocked Result: 8,000,000 in 45.32ms

Test 3: Regular counter (demonstrates race condition)...

Regular Result: 2,345,678 in 89.12ms
Expected: 8,000,000, Lost: 5,654,322

=== Summary ===
Volatile:    125.45ms - Count: 8,000,000 ✓
Interlocked: 45.32ms - Count: 8,000,000 ✓
Regular:     89.12ms - Count: 2,345,678 ✗ (race condition)
```

## Concepts Demonstrated

- `volatile` keyword for memory visibility
- `Volatile.Read` and `Volatile.Write`
- `Thread.MemoryBarrier()` for ordering
- `Interlocked` atomic operations
- `Interlocked.Increment`, `Interlocked.Add`
- `Interlocked.CompareExchange`
- Race conditions in unsynchronized code

## Key Takeaways

1. **Regular operations** without synchronization lose updates due to race conditions
2. **Volatile** ensures visibility but read-modify-write is still not atomic
3. **Interlocked** provides true atomic operations and is fastest for counters
