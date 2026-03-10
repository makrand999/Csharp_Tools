# Parallel Batch Processor

Processes large datasets in parallel batches using TPL (Task Parallel Library) with efficient work distribution.

## Usage

```bash
dotnet run --project ParallelBatchProcessor.csproj
```

## Example

```
=== Parallel Batch Processor ===

Processing 1000 items with 8 threads...
Batch size: 50

Progress: 10% (100/1000)
Progress: 20% (200/1000)
...

=== Results ===
Processed: 1000 items
Time elapsed: 250ms
Items/second: 4000

=== Sequential Comparison ===
Sequential time: 1800ms
Parallel speedup: 7.2x
```

## Concepts Demonstrated

- Parallel.ForEach with Partitioner
- EnumerablePartitionerOptions for efficient distribution
- ConcurrentBag for thread-safe result collection
- Progress reporting with Interlocked
- Performance benchmarking
