using System;
using System.Threading;
using System.Threading.Tasks;

namespace ThreadPoolMonitor;

/// <summary>
/// ThreadPool Monitor - Real-time thread pool statistics and monitoring utility.
/// Displays worker thread and completion port thread usage with live updates.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== ThreadPool Monitor ===");
        Console.WriteLine("Monitoring thread pool statistics in real-time.\n");
        Console.WriteLine("Press 'q' to quit.\n");

        using var cts = new CancellationTokenSource();
        var monitorTask = Task.Run(() => MonitorThreadPool(cts.Token));

        // Simulate some workload to show thread pool dynamics
        var workloadTask = Task.Run(() => SimulateWorkload(cts.Token));

        // Wait for user input
        await Task.Run(() =>
        {
            while (Console.ReadKey(intercept: true).Key != ConsoleKey.Q)
            {
                Thread.Sleep(100);
            }
            cts.Cancel();
        });

        await Task.WhenAll(monitorTask, workloadTask);
        
        Console.WriteLine("\nMonitoring stopped.");
    }

    static void MonitorThreadPool(CancellationToken cancellationToken)
    {
        int previousLineCount = 0;
        
        while (!cancellationToken.IsCancellationRequested)
        {
            ThreadPool.GetMinThreads(out int minWorker, out int minCompletion);
            ThreadPool.GetMaxThreads(out int maxWorker, out int maxCompletion);
            ThreadPool.GetAvailableThreads(out int availableWorker, out int availableCompletion);

            int currentWorker = maxWorker - availableWorker;
            int currentCompletion = maxCompletion - availableCompletion;

            // Clear previous output
            if (previousLineCount > 0)
            {
                Console.CursorTop = Console.CursorTop - previousLineCount;
            }

            Console.WriteLine($"┌─────────────────────────────────────────┐");
            Console.WriteLine($"│ ThreadPool Statistics                   │");
            Console.WriteLine($"├─────────────────────────────────────────┤");
            Console.WriteLine($"│ Worker Threads:                         │");
            Console.WriteLine($"│   Min: {minWorker,4}  Max: {maxWorker,4}  Current: {currentWorker,4}  Available: {availableWorker,4} │");
            Console.WriteLine($"│                                         │");
            Console.WriteLine($"│ Completion Port Threads:                │");
            Console.WriteLine($"│   Min: {minCompletion,4}  Max: {maxCompletion,4}  Current: {currentCompletion,4}  Available: {availableCompletion,4} │");
            Console.WriteLine($"├─────────────────────────────────────────┤");
            Console.WriteLine($"│ CPU Usage: {Environment.ProcessorCount,2} processors                    │");
            Console.WriteLine($"│ GC Memory: {(GC.GetTotalMemory(false) / 1024 / 1024),6} MB                       │");
            Console.WriteLine($"└─────────────────────────────────────────┘");
            Console.WriteLine($"[Timestamp: {DateTime.Now:HH:mm:ss.fff}]");

            previousLineCount = 10;
            Thread.Sleep(500);
        }
    }

    static async Task SimulateWorkload(CancellationToken cancellationToken)
    {
        var random = new Random();
        
        while (!cancellationToken.IsCancellationRequested)
        {
            // Simulate burst of parallel work
            var tasks = Enumerable.Range(0, random.Next(5, 15))
                .Select(_ => Task.Run(async () =>
                {
                    await Task.Delay(random.Next(50, 200), cancellationToken);
                    return await Task.FromResult(0);
                }))
                .ToArray();

            await Task.WhenAll(tasks);
            await Task.Delay(500, cancellationToken);
        }
    }
}
