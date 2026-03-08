# Lock-Free Stack

Lock-free stack implementation using `Interlocked` operations. Demonstrates atomic compare-and-swap (CAS) for thread-safe data structures without traditional locks.

## Usage

```bash
dotnet run --project LockFreeStack/LockFreeStack.csproj
```

## Example

```
=== Lock-Free Stack ===

Task 0: Pushed 0
Task 1: Pushed 100
Popper 0: Popped 203
Popper 1: Popped 305
...

Final stack count: 0
Total pushed: 400
Total popped: 400
```

## Concepts Demonstrated

- `Interlocked.CompareExchange` for CAS operations
- Lock-free data structures
- Atomic operations
- Thread-safe push/pop
- Memory barriers with `Volatile`
