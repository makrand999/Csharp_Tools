using System;
using System.Diagnostics;

namespace GcMonitor;

/// <summary>
/// Monitors garbage collection events and memory statistics.
/// Useful for diagnosing memory issues and understanding GC behavior.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== GC Monitor Demo ===\n");
        Console.WriteLine("Monitors garbage collection and memory usage.\n");

        // Display initial memory info
        DisplayMemoryInfo();

        // Demo 1: GC generations
        Console.WriteLine("\n--- GC Generations Demo ---\n");
        GcGenerationsDemo();

        // Demo 2: Memory pressure
        Console.WriteLine("\n--- Memory Pressure Demo ---\n");
        MemoryPressureDemo();

        // Demo 3: Finalization tracking
        Console.WriteLine("\n--- Finalization Demo ---\n");
        FinalizationDemo();

        // Final stats
        Console.WriteLine("\n--- Final Statistics ---\n");
        DisplayMemoryInfo();
        Console.WriteLine($"Total collections triggered: {FinalizableObject.GetTotalCollections()}");
    }

    static void DisplayMemoryInfo()
    {
        Console.WriteLine("Memory Statistics:");
        Console.WriteLine($"  GC Memory (bytes): {GC.GetTotalMemory(false):N0}");
        Console.WriteLine($"  Total Memory (bytes): {GC.GetTotalMemory(true):N0}");
        Console.WriteLine($"  Max Working Set: {GC.MaxGeneration}");
        
        for (int i = 0; i <= GC.MaxGeneration; i++)
        {
            Console.WriteLine($"  Generation {i} Count: {GC.CollectionCount(i)}");
        }

        var process = Process.GetCurrentProcess();
        Console.WriteLine($"  Working Set (MB): {process.WorkingSet64 / (1024 * 1024):F2}");
        Console.WriteLine($"  Private Memory (MB): {process.PrivateMemorySize64 / (1024 * 1024):F2}");
    }

    static void GcGenerationsDemo()
    {
        // Gen 0 allocation
        var gen0Objects = new List<byte[]>();
        for (int i = 0; i < 100; i++)
        {
            gen0Objects.Add(new byte[1000]); // 1KB each
        }
        Console.WriteLine($"Allocated {gen0Objects.Count * 1000:N0} bytes in Gen 0");
        Console.WriteLine($"Gen 0 count before: {GC.CollectionCount(0)}");

        // Release and collect
        gen0Objects.Clear();
        GC.Collect(0);
        Console.WriteLine($"Gen 0 count after: {GC.CollectionCount(0)}");

        // Gen 1 allocation (survive Gen 0 collection)
        var gen1Objects = new List<byte[]>();
        for (int i = 0; i < 50; i++)
        {
            gen1Objects.Add(new byte[10000]); // 10KB each
        }
        Console.WriteLine($"\nAllocated {gen1Objects.Count * 10000:N0} bytes");
        
        // First collection
        GC.Collect(0);
        // Second collection promotes to Gen 2
        GC.Collect(1);
        
        Console.WriteLine($"Gen 1 count: {GC.CollectionCount(1)}");
        Console.WriteLine($"Gen 2 count: {GC.CollectionCount(2)}");

        gen1Objects.Clear();
    }

    static void MemoryPressureDemo()
    {
        Console.WriteLine("Simulating memory pressure...");
        
        var objects = new List<byte[]>();
        int iteration = 0;

        try
        {
            while (iteration < 10)
            {
                objects.Add(new byte[500_000]); // 500KB each
                iteration++;

                if (iteration % 3 == 0)
                {
                    Console.WriteLine($"  Allocated {iteration * 500}KB, GC Memory: {GC.GetTotalMemory(false) / 1024:N0}KB");
                }
            }
        }
        catch (OutOfMemoryException)
        {
            Console.WriteLine("  Out of memory!");
        }

        Console.WriteLine($"\nTotal allocated: {objects.Count * 500}KB");
        Console.WriteLine($"Releasing memory...");
        objects.Clear();
        
        GC.Collect();
        Console.WriteLine($"After GC: {GC.GetTotalMemory(false) / 1024:N0}KB");
    }

    static void FinalizationDemo()
    {
        Console.WriteLine("Creating objects with finalizers...");

        // Create objects that need finalization
        for (int i = 0; i < 5; i++)
        {
            new FinalizableObject($"Object_{i}");
        }

        Console.WriteLine("Objects created, now out of scope");
        Console.WriteLine("Running GC with finalization...");

        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

        Console.WriteLine("Finalization complete");
    }
}

/// <summary>
/// An object with a finalizer for demonstration
/// </summary>
public class FinalizableObject
{
    private readonly string _name;
    private readonly byte[] _data;
    private static readonly object _lock = new();
    private static long _totalCollections = 0;

    public FinalizableObject(string name)
    {
        _name = name;
        _data = new byte[10_000];
        Console.WriteLine($"  Created {_name}");
    }

    ~FinalizableObject()
    {
        lock (_lock)
        {
            _totalCollections++;
            Console.WriteLine($"  Finalized {_name}");
        }
    }

    public static long GetTotalCollections() => _totalCollections;
}

/// <summary>
/// GC event watcher (for .NET Core 3.0+)
/// </summary>
public class GcEventWatcher
{
    public static void StartMonitoring()
    {
        // Note: GC events are limited in .NET Core
        // This is a placeholder for more advanced monitoring
        Console.WriteLine("GC Event monitoring started (limited in .NET Core)");
    }
}
