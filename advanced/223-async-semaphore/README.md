# Async Semaphore

Async semaphore implementation for resource pool management. Demonstrates async-aware synchronization primitives for concurrent access control.

## Usage

```bash
dotnet run --project AsyncSemaphore/AsyncSemaphore.csproj
```

## Example

```
=== Async Semaphore Resource Pool ===

Worker 0: Acquired connection 1
Worker 1: Acquired connection 2
Worker 2: Acquired connection 3
Worker 0: Released connection 1
Worker 3: Acquired connection 1
...

Total queries executed: 10
All connections returned: True
```

## Concepts Demonstrated

- `SemaphoreSlim` for async locking
- Resource pool pattern
- Async resource acquisition
- Connection pooling
- `IAsyncDisposable` pattern
