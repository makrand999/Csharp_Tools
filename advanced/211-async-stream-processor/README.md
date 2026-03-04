# Async Stream Processor

Demonstrates `IAsyncEnumerable<T>` and async streams for processing data streams asynchronously with pipelined operations.

## Usage

```bash
dotnet run --project AsyncStreamProcessor.csproj
```

## Example

```
=== Async Stream Processor ===

Processing data stream asynchronously...

[05] Processed: 1 → 1
[06] Processed: 2 → 4
[07] Processed: 3 → 9
[05] Processed: 4 → 16
...

✓ Stream processing completed!
```

## Concepts Demonstrated

- `IAsyncEnumerable<T>` for async streams
- `await foreach` for consuming async streams
- `yield return` with async methods
- Concurrent processing with `SemaphoreSlim`
- Cancellation token propagation
- Async stream transformation and filtering
