# ThreadPool Monitor

Real-time thread pool statistics and monitoring utility. Displays worker thread and completion port thread usage with live updates, CPU information, and memory usage.

## Usage

```bash
dotnet run --project ThreadPoolMonitor.csproj
```

## Example

```
=== ThreadPool Monitor ===
Monitoring thread pool statistics in real-time.

Press 'q' to quit.

┌─────────────────────────────────────────┐
│ ThreadPool Statistics                   │
├─────────────────────────────────────────┤
│ Worker Threads:                         │
│   Min:    8  Max: 32767  Current:    4  Available: 32763 │
│                                         │
│ Completion Port Threads:                │
│   Min:    1  Max: 1000  Current:    0  Available: 1000 │
├─────────────────────────────────────────┤
│ CPU Usage:  8 processors                    │
│ GC Memory:     25 MB                       │
└─────────────────────────────────────────┘
[Timestamp: 14:32:45.123]
```

## Concepts Demonstrated

- **ThreadPool API** - `GetMinThreads`, `GetMaxThreads`, `GetAvailableThreads`
- **Async/Await** - Non-blocking monitoring and workload simulation
- **CancellationToken** - Graceful shutdown of monitoring tasks
- **Console Output** - Real-time display updates
- **Task Parallelism** - Concurrent monitoring and workload tasks
- **System.Diagnostics** - Process and memory information
