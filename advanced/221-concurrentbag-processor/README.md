# ConcurrentBag Processor

Thread-safe collection processor using `ConcurrentBag` for parallel data processing with multiple producers and consumers.

## Usage

```bash
dotnet run --project ConcurrentBagProcessor/ConcurrentBagProcessor.csproj
```

## Example

```
=== ConcurrentBag Processor ===

Producer 0: Added 0
Producer 1: Added 100
Producer 2: Added 200
Consumer 0: Processed 200 -> 400
Consumer 1: Processed 100 -> 200
...

Produced 30 items

Processed 30 items:
  Total value: 8970
  Avg duration: 2.45ms
```

## Concepts Demonstrated

- `ConcurrentBag<T>` for thread-safe collections
- Multiple producer-consumer pattern
- Lock-free concurrent operations
- `TryTake` and `TryPeek` operations
- Parallel data processing
