# SpinLock Counter

Demonstrates `SpinLock` for high-performance synchronization in short critical sections, comparing it with regular `lock` statements.

## Usage

```bash
dotnet run --project SpinLockCounter.csproj
```

## Example

```
=== SpinLock Counter ===

Comparing SpinLock vs lock for high-frequency operations...

Running SpinLock test with 8 threads, 100,000 iterations each...

SpinLock Result: 800,000 in 45.23ms

Running lock test with 8 threads, 100,000 iterations each...

Lock Result: 800,000 in 62.18ms

=== Comparison ===
SpinLock: 45.23ms
Regular lock: 62.18ms
Speedup: 1.37x
```

## Concepts Demonstrated

- `SpinLock` for low-level synchronization
- Spin vs block waiting strategies
- When to use SpinLock (short critical sections)
- Lock ownership tracking
- Performance comparison patterns
- High-frequency atomic operations

## When to Use SpinLock

- **Use when:** Critical section is very short (< few microseconds)
- **Use when:** Contention is low to moderate
- **Avoid when:** Critical section might block (I/O, network)
- **Avoid when:** Running on single-core systems
