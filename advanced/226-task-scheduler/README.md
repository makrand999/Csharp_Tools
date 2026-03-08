# Task Scheduler

Custom task scheduler with priority-based execution. Demonstrates task queuing, priority handling, and custom scheduling strategies for workload management.

## Usage

```bash
dotnet run --project TaskScheduler/TaskScheduler.csproj
```

## Example

```
=== Priority Task Scheduler ===

Starting: Critical task
Starting: High priority task
Starting: High priority task 2
Completed: Critical task
Starting: Normal task 1
...

All tasks completed!
Results: [100, 300, 150, 500, 200, 250]
```

## Concepts Demonstrated

- `PriorityQueue<T>` for priority ordering
- Custom task scheduling
- Concurrency limiting with `SemaphoreSlim`
- Task completion sources
- Priority inversion handling
