# Process Monitor

Monitors system processes and their resource usage in real-time.

## Usage

```bash
dotnet run --project ProcessMonitor.csproj
```

## Example

```
=== Process Monitor ===

=== System Information ===
Machine: mycomputer
OS: Unix 5.15.0
Processors: 8
Total Memory: 16384 MB
Uptime: 5d 3h 25m

=== Top 10 Processes by Memory ===
ID       Name                      Memory (MB)     Threads    CPU Time
75       chrome                    512.5           25         01:25:30
...

=== Monitoring Current Process ===
[14:30:25] CPU: 2.5% | Memory: 45 MB | Threads: 12
[14:30:26] CPU: 1.8% | Memory: 46 MB | Threads: 12
```

## Concepts Demonstrated

- Process enumeration
- Performance counters
- CPU usage calculation
- Memory monitoring
- System information retrieval
