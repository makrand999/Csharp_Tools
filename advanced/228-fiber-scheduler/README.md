# Fiber Scheduler

Lightweight fiber/green thread scheduler for cooperative multitasking. Demonstrates user-mode scheduling, yield points, and coroutine-like execution patterns.

## Usage

```bash
dotnet run --project FiberScheduler/FiberScheduler.csproj
```

## Example

```
=== Fiber Scheduler ===

Fiber-A: Iteration 1/5
Fiber-B: Iteration 1/8
Fiber-C: Iteration 1/3
Fiber-A: Iteration 2/5
Fiber-B: Iteration 2/8
...

Fiber-A: Completed
Fiber-B: Completed
Fiber-C: Completed

All fibers completed!
Total yield points: 16
```

## Concepts Demonstrated

- Cooperative multitasking
- Fiber/coroutine patterns
- Yield points and scheduling
- User-mode task switching
- Async/await for scheduling
