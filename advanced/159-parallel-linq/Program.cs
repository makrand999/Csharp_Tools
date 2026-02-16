using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace ParallelLinq;

/// <summary>
/// Parallel LINQ (PLINQ) Demo - Performance utilities for parallel data processing.
/// Compares sequential vs parallel query performance and demonstrates PLINQ operators.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Parallel LINQ (PLINQ) Demo ===");
        Console.WriteLine("Comparing sequential vs parallel query performance.\n");

        Console.WriteLine("Choose operation:");
        Console.WriteLine("1. Performance Comparison (Sequential vs Parallel)");
        Console.WriteLine("2. PLINQ Operators Demo");
        Console.WriteLine("3. Parallel Aggregation");
        Console.WriteLine("4. Run All Demos");
        Console.Write("\nSelection (1-4): ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await PerformanceComparison();
                break;
            case "2":
                PlinqOperatorsDemo();
                break;
            case "3":
                ParallelAggregation();
                break;
            case "4":
                await PerformanceComparison();
                PlinqOperatorsDemo();
                ParallelAggregation();
                break;
            default:
                Console.WriteLine("Invalid selection.");
                break;
        }
    }

    /// <summary>
    /// Compares performance of LINQ vs PLINQ on CPU-intensive operations.
    /// </summary>
    static async Task PerformanceComparison()
    {
        Console.WriteLine("\n--- Performance Comparison ---\n");

        // Generate test data
        var data = Enumerable.Range(1, 1_000_000).ToArray();

        // Sequential LINQ
        Console.WriteLine("Running sequential LINQ...");
        var sw = Stopwatch.StartNew();
        var sequentialResult = data
            .Where(x => x % 2 == 0)
            .Select(x => (long)x * x)
            .Sum();
        sw.Stop();
        var sequentialTime = sw.ElapsedMilliseconds;
        Console.WriteLine($"Sequential: {sequentialTime} ms, Result: {sequentialResult:N0}");

        // Parallel PLINQ
        Console.WriteLine("\nRunning parallel PLINQ...");
        sw.Restart();
        var parallelResult = data
            .AsParallel()
            .WithDegreeOfParallelism(Environment.ProcessorCount)
            .Where(x => x % 2 == 0)
            .Select(x => (long)x * x)
            .Sum();
        sw.Stop();
        var parallelTime = sw.ElapsedMilliseconds;
        Console.WriteLine($"Parallel:   {parallelTime} ms, Result: {parallelResult:N0}");

        var speedup = (double)sequentialTime / parallelTime;
        Console.WriteLine($"\nSpeedup: {speedup:F2}x faster with PLINQ");
    }

    /// <summary>
    /// Demonstrates various PLINQ operators and options.
    /// </summary>
    static void PlinqOperatorsDemo()
    {
        Console.WriteLine("\n--- PLINQ Operators Demo ---\n");

        var numbers = Enumerable.Range(1, 100).ToArray();

        // WithDegreeOfParallelism
        Console.WriteLine("1. WithDegreeOfParallelism(4):");
        var result1 = numbers.AsParallel()
            .WithDegreeOfParallelism(4)
            .Where(x => x % 2 == 0)
            .Take(10)
            .ToArray();
        Console.WriteLine($"   First 10 even numbers: [{string.Join(", ", result1)}]");

        // WithExecutionMode (ForceParallelism)
        Console.WriteLine("\n2. WithExecutionMode(ForceParallelism):");
        var result2 = numbers.AsParallel()
            .WithExecutionMode(ParallelExecutionMode.ForceParallelism)
            .Select(x => x * 2)
            .Where(x => x > 100)
            .Take(5)
            .ToArray();
        Console.WriteLine($"   Doubled values > 100: [{string.Join(", ", result2)}]");

        // Order preservation
        Console.WriteLine("\n3. Order preservation:");
        var ordered = numbers.AsParallel()
            .Where(x => x % 10 == 0)
            .OrderBy(x => x)
            .Take(5);
        Console.WriteLine($"   Ordered multiples of 10: [{string.Join(", ", ordered)}]");

        // AsUnordered for better performance
        Console.WriteLine("\n4. AsUnordered (better performance when order doesn't matter):");
        var unordered = numbers.AsParallel()
            .AsUnordered()
            .Where(x => x % 17 == 0)
            .Take(5);
        Console.WriteLine($"   Unordered multiples of 17: [{string.Join(", ", unordered)}]");
    }

    /// <summary>
    /// Demonstrates parallel aggregation operations.
    /// </summary>
    static void ParallelAggregation()
    {
        Console.WriteLine("\n--- Parallel Aggregation ---\n");

        var data = Enumerable.Range(1, 10_000).ToArray();

        // Count
        Console.WriteLine("Parallel Count:");
        var count = data.AsParallel().Count(x => x % 3 == 0);
        Console.WriteLine($"  Numbers divisible by 3: {count}");

        // Average
        Console.WriteLine("\nParallel Average:");
        var avg = data.AsParallel().Average();
        Console.WriteLine($"  Average of 1-10000: {avg:F2}");

        // Min/Max
        Console.WriteLine("\nParallel Min/Max:");
        var min = data.AsParallel().Min();
        var max = data.AsParallel().Max();
        Console.WriteLine($"  Min: {min}, Max: {max}");

        // Aggregate
        Console.WriteLine("\nParallel Aggregate (Product of first 20):");
        var product = Enumerable.Range(1, 20).AsParallel()
            .Aggregate(1L, (acc, x) => acc * x);
        Console.WriteLine($"  20! = {product:N0}");

        // GroupBy
        Console.WriteLine("\nParallel GroupBy:");
        var groups = data.AsParallel()
            .GroupBy(x => x % 5)
            .Select(g => new { Remainder = g.Key, Count = g.Count() })
            .OrderBy(g => g.Remainder)
            .Take(5);
        foreach (var g in groups)
        {
            Console.WriteLine($"  Remainder {g.Remainder}: {g.Count} items");
        }
    }
}
