using System.Diagnostics;

namespace PerformanceProfiler;

/// <summary>
/// Performance Profiler - Profiles code execution time and memory allocation.
/// Demonstrates timing, GC monitoring, and performance metrics collection.
/// </summary>
public class PerformanceProfiler
{
    private readonly List<ProfileResult> _results = new();
    private readonly Dictionary<string, List<long>> _timings = new();

    public ProfileResult Profile(Action action, string name)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memoryBefore = GC.GetTotalMemory(false);
        var stopwatch = Stopwatch.StartNew();

        action();

        stopwatch.Stop();
        var memoryAfter = GC.GetTotalMemory(false);

        var result = new ProfileResult
        {
            Name = name,
            ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
            MemoryAllocatedBytes = memoryAfter - memoryBefore,
            Timestamp = DateTime.UtcNow
        };

        _results.Add(result);

        if (!_timings.ContainsKey(name))
            _timings[name] = new List<long>();
        
        _timings[name].Add(stopwatch.ElapsedTicks);

        return result;
    }

    public async Task<ProfileResult> ProfileAsync(Func<Task> action, string name)
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        var memoryBefore = GC.GetTotalMemory(false);
        var stopwatch = Stopwatch.StartNew();

        await action();

        stopwatch.Stop();
        var memoryAfter = GC.GetTotalMemory(false);

        var result = new ProfileResult
        {
            Name = name,
            ElapsedMilliseconds = stopwatch.Elapsed.TotalMilliseconds,
            MemoryAllocatedBytes = memoryAfter - memoryBefore,
            Timestamp = DateTime.UtcNow
        };

        _results.Add(result);

        if (!_timings.ContainsKey(name))
            _timings[name] = new List<long>();
        
        _timings[name].Add(stopwatch.ElapsedTicks);

        return result;
    }

    public void Benchmark(Action action, string name, int iterations = 10)
    {
        var times = new List<long>();

        for (int i = 0; i < iterations; i++)
        {
            var result = Profile(action, $"{name} (Iteration {i + 1})");
            times.Add((long)(result.ElapsedMilliseconds * TimeSpan.TicksPerMillisecond));
        }

        var avg = times.Average();
        var min = times.Min();
        var max = times.Max();
        var stdDev = Math.Sqrt(times.Average(v => Math.Pow(v - avg, 2)));

        Console.WriteLine($"\n=== Benchmark: {name} ===");
        Console.WriteLine($"Iterations: {iterations}");
        Console.WriteLine($"Average: {avg / TimeSpan.TicksPerMillisecond:F2}ms");
        Console.WriteLine($"Min: {min / TimeSpan.TicksPerMillisecond:F2}ms");
        Console.WriteLine($"Max: {max / TimeSpan.TicksPerMillisecond:F2}ms");
        Console.WriteLine($"StdDev: {stdDev / TimeSpan.TicksPerMillisecond:F2}ms");
    }

    public void PrintSummary()
    {
        Console.WriteLine("\n=== Profile Summary ===");
        Console.WriteLine($"{"Name",-30} {"Time (ms)",-15} {"Memory (KB)",-15}");
        Console.WriteLine(new string('-', 60));

        foreach (var result in _results.OrderByDescending(r => r.ElapsedMilliseconds))
        {
            Console.WriteLine($"{result.Name,-30} {result.ElapsedMilliseconds,-15:F2} {result.MemoryAllocatedBytes / 1024,-15:F1}");
        }
    }

    public void Clear()
    {
        _results.Clear();
        _timings.Clear();
    }
}

public record ProfileResult
{
    public string Name { get; init; } = string.Empty;
    public double ElapsedMilliseconds { get; init; }
    public long MemoryAllocatedBytes { get; init; }
    public DateTime Timestamp { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Performance Profiler ===\n");

        var profiler = new PerformanceProfiler();

        // Profile different operations
        Console.WriteLine("Profiling various operations...\n");

        // String concatenation
        profiler.Profile(() =>
        {
            var result = "";
            for (int i = 0; i < 1000; i++)
            {
                result += i.ToString();
            }
        }, "String Concatenation (1000)");

        // StringBuilder
        profiler.Profile(() =>
        {
            var sb = new System.Text.StringBuilder();
            for (int i = 0; i < 1000; i++)
            {
                sb.Append(i);
            }
        }, "StringBuilder (1000)");

        // List operations
        profiler.Profile(() =>
        {
            var list = new List<int>();
            for (int i = 0; i < 10000; i++)
            {
                list.Add(i);
            }
        }, "List.Add (10000)");

        // LINQ operations
        profiler.Profile(() =>
        {
            var numbers = Enumerable.Range(0, 10000).ToList();
            var result = numbers.Where(n => n % 2 == 0).Select(n => n * 2).ToList();
        }, "LINQ Where+Select (10000)");

        // Dictionary lookups
        profiler.Profile(() =>
        {
            var dict = new Dictionary<int, string>();
            for (int i = 0; i < 1000; i++)
            {
                dict[i] = i.ToString();
            }
            for (int i = 0; i < 1000; i++)
            {
                _ = dict.TryGetValue(i, out _);
            }
        }, "Dictionary Operations (1000)");

        // Async operation
        await profiler.ProfileAsync(async () =>
        {
            await Task.Delay(100);
        }, "Task.Delay (100ms)");

        // Print summary
        profiler.PrintSummary();

        // Run benchmark
        Console.WriteLine("\n=== Running Benchmark ===");
        profiler.Benchmark(() =>
        {
            var numbers = Enumerable.Range(0, 10000).ToList();
            _ = numbers.Sum();
        }, "Sum of 10000 numbers", iterations: 5);

        // GC Info
        Console.WriteLine($"\n=== GC Information ===");
        var gcInfo = GC.GetGCMemoryInfo();
        Console.WriteLine($"Total available memory: {gcInfo.TotalAvailableMemoryBytes / 1024 / 1024} MB");
        Console.WriteLine($"Heap size: {GC.GetGCMemoryInfo().HeapSizeBytes / 1024 / 1024} MB");
        Console.WriteLine($"Total memory: {GC.GetTotalMemory(false)} bytes");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
