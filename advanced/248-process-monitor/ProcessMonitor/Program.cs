using System.Diagnostics;

namespace ProcessMonitor;

/// <summary>
/// Process Monitor - Monitors system processes and their resource usage.
/// Demonstrates process enumeration, performance counters, and real-time monitoring.
/// </summary>
public class ProcessMonitoringService
{
    public List<ProcessInfo> GetProcessList()
    {
        var processes = new List<ProcessInfo>();

        foreach (var process in Process.GetProcesses())
        {
            try
            {
                processes.Add(new ProcessInfo
                {
                    Id = process.Id,
                    Name = process.ProcessName,
                    StartTime = process.StartTime,
                    TotalProcessorTime = process.TotalProcessorTime,
                    WorkingSet64 = process.WorkingSet64,
                    VirtualMemorySize64 = process.VirtualMemorySize64,
                    ThreadCount = process.Threads.Count,
                    HandleCount = process.HandleCount,
                    Responding = process.Responding
                });
            }
            catch
            {
                // Skip processes we can't access
            }
            finally
            {
                process.Dispose();
            }
        }

        return processes.OrderByDescending(p => p.WorkingSet64).ToList();
    }

    public async Task<ProcessSnapshot> MonitorProcessAsync(int processId, int sampleCount = 5, int intervalMs = 1000)
    {
        var snapshots = new List<ProcessSnapshot>();

        for (int i = 0; i < sampleCount; i++)
        {
            var process = Process.GetProcessById(processId);
            var snapshot = new ProcessSnapshot
            {
                Timestamp = DateTime.UtcNow,
                ProcessId = process.Id,
                ProcessName = process.ProcessName,
                CpuPercent = GetCpuUsage(process),
                MemoryBytes = process.WorkingSet64,
                ThreadCount = process.Threads.Count,
                HandleCount = process.HandleCount
            };

            snapshots.Add(snapshot);
            process.Dispose();

            if (i < sampleCount - 1)
                await Task.Delay(intervalMs);
        }

        return snapshots.Last();
    }

    public SystemInfo GetSystemInfo()
    {
        return new SystemInfo
        {
            ProcessorCount = Environment.ProcessorCount,
            TotalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes,
            MachineName = Environment.MachineName,
            OSVersion = Environment.OSVersion.ToString(),
            Uptime = TimeSpan.FromMilliseconds(Environment.TickCount64)
        };
    }

    private double GetCpuUsage(Process process)
    {
        try
        {
            var cpuTime = process.TotalProcessorTime.TotalMilliseconds;
            var uptime = (DateTime.UtcNow - process.StartTime.ToUniversalTime()).TotalMilliseconds;
            
            if (uptime <= 0) return 0;
            
            return (cpuTime / uptime) * 100 * Environment.ProcessorCount;
        }
        catch
        {
            return 0;
        }
    }
}

public record ProcessInfo
{
    public int Id { get; init; }
    public string Name { get; init; } = string.Empty;
    public DateTime StartTime { get; init; }
    public TimeSpan TotalProcessorTime { get; init; }
    public long WorkingSet64 { get; init; }
    public long VirtualMemorySize64 { get; init; }
    public int ThreadCount { get; init; }
    public int HandleCount { get; init; }
    public bool Responding { get; init; }
}

public record ProcessSnapshot
{
    public DateTime Timestamp { get; init; }
    public int ProcessId { get; init; }
    public string ProcessName { get; init; } = string.Empty;
    public double CpuPercent { get; init; }
    public long MemoryBytes { get; init; }
    public int ThreadCount { get; init; }
    public int HandleCount { get; init; }
}

public record SystemInfo
{
    public int ProcessorCount { get; init; }
    public long TotalMemory { get; init; }
    public string MachineName { get; init; } = string.Empty;
    public string OSVersion { get; init; } = string.Empty;
    public TimeSpan Uptime { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Process Monitor ===\n");

        var service = new ProcessMonitoringService();

        // System info
        var systemInfo = service.GetSystemInfo();
        Console.WriteLine("=== System Information ===");
        Console.WriteLine($"Machine: {systemInfo.MachineName}");
        Console.WriteLine($"OS: {systemInfo.OSVersion}");
        Console.WriteLine($"Processors: {systemInfo.ProcessorCount}");
        Console.WriteLine($"Total Memory: {systemInfo.TotalMemory / 1024 / 1024} MB");
        Console.WriteLine($"Uptime: {systemInfo.Uptime.Days}d {systemInfo.Uptime.Hours}h {systemInfo.Uptime.Minutes}m");

        // Process list
        Console.WriteLine("\n=== Top 10 Processes by Memory ===");
        var processes = service.GetProcessList();

        Console.WriteLine($"{"ID",-8} {"Name",-25} {"Memory (MB)",-15} {"Threads",-10} {"CPU Time"}");
        Console.WriteLine(new string('-', 75));

        foreach (var p in processes.Take(10))
        {
            Console.WriteLine($"{p.Id,-8} {p.Name,-25} {p.WorkingSet64 / 1024 / 1024,-15:F1} {p.ThreadCount,-10} {p.TotalProcessorTime}");
        }

        // Monitor current process
        Console.WriteLine($"\n=== Monitoring Current Process (ID: {Environment.ProcessId}) ===");
        Console.WriteLine("Sampling CPU and memory usage...\n");

        for (int i = 0; i < 3; i++)
        {
            var snapshot = await service.MonitorProcessAsync(Environment.ProcessId, sampleCount: 1);
            
            Console.WriteLine($"[{DateTime.UtcNow:HH:mm:ss}] CPU: {snapshot.CpuPercent:F1}% | Memory: {snapshot.MemoryBytes / 1024 / 1024} MB | Threads: {snapshot.ThreadCount}");
            
            await Task.Delay(500);
        }

        // Process statistics
        Console.WriteLine($"\n=== Process Statistics ===");
        Console.WriteLine($"Total processes: {processes.Count}");
        Console.WriteLine($"Total memory used: {processes.Sum(p => p.WorkingSet64) / 1024 / 1024} MB");
        Console.WriteLine($"Total threads: {processes.Sum(p => p.ThreadCount)}");
        Console.WriteLine($"Total handles: {processes.Sum(p => p.HandleCount)}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
