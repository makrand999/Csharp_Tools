# Task Parallel Processor

Demonstrates TPL (Task Parallel Library) patterns including Task.WhenAll, ContinueWith, and parallel task execution.

## Usage

```bash
dotnet run --project TaskParallelProcessor.csproj
```

## Example

```
=== Task Parallel Processor ===

Processing 20 tasks with 5 concurrent workers...

✓ Processed item 1/20
✓ Processed item 2/20
...

=== Processing Statistics ===
Total Items: 20
Processed: 20
Failed: 0
Elapsed Time: 2.15s
Throughput: 9.3 items/sec

Efficiency: 95.2%
```

## Concepts Demonstrated

- Task.WhenAll for parallel execution
- SemaphoreSlim for concurrency control
- IProgress<T> for progress reporting
- Interlocked for thread-safe counters
- Performance metrics calculation
