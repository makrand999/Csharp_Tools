# ManualResetEventSlim

Demonstrates `ManualResetEventSlim` (MRES) for efficient thread signaling and coordination.

## Usage

```bash
dotnet run --project ManualResetEvent.csproj
```

## Example

```
=== ManualResetEventSlim ===

Coordinating threads with ManualResetEventSlim...

--- Example 1: Basic Signaling ---

[Worker] Waiting for signal...
[Main] Setting signal...
[Worker] Signal received! Continuing work...
[Main] Worker completed!

--- Example 2: Producer-Consumer ---

[Producer] Produced: Item-1
[Consumer] Consumed: Item-1
[Producer] Produced: Item-2
...

--- Example 3: Multiple Waiters ---

Creating 5 worker tasks waiting for start signal...

[Worker 1] Waiting for start signal...
[Worker 2] Waiting for start signal...
...
[Main] Setting start signal - all workers begin!

[Worker 1] Starting work!
[Worker 2] Starting work!
...
```

## Concepts Demonstrated

- `ManualResetEventSlim` for thread signaling
- `Set()` to signal all waiting threads
- `Reset()` to unsignal
- `Wait()` to block until signaled
- `WaitHandle.WaitAny` for multiple wait handles
- Producer-consumer patterns
- Starting multiple threads simultaneously

## MRES vs ManualResetEvent

- **MRES:** Uses spinning initially, then kernel wait (faster for short waits)
- **ManualResetEvent:** Always uses kernel wait (better for long waits)
