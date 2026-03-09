# Work-Stealing Queue

Work-stealing queue implementation for load balancing across workers. Demonstrates concurrent dequeues and work-stealing pattern for efficient parallel processing.

## Usage

```bash
dotnet run --project WorkStealingQueue/WorkStealingQueue.csproj
```

## Example

```
=== Work-Stealing Queue ===

Enqueued work item 0
Worker 0: Processed 10 items
Worker 2: STOLE work item 45
Worker 1: Processed 10 items
...

Worker 0: Done (processed=28, stolen=2)
Worker 1: Done (processed=25, stolen=3)
Worker 2: Done (processed=24, stolen=5)
Worker 3: Done (processed=23, stolen=1)
```

## Concepts Demonstrated

- Work-stealing pattern
- Per-worker local queues
- Load balancing algorithms
- `ConcurrentQueue<T>` for thread-safe queues
- LIFO vs FIFO dequeuing strategies
