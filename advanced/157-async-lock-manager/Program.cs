using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace AsyncLockManager;

/// <summary>
/// Async Lock Manager - Demonstrates async synchronization primitives
/// for coordinating concurrent access to shared resources.
/// </summary>
class Program
{
    // Async lock for coordinating async operations
    private static readonly SemaphoreSlim _asyncLock = new(1, 1);
    private static readonly ReaderWriterLockSlim _rwLock = new();
    private static readonly Dictionary<string, object> _sharedData = new();
    private static int _accessCounter;

    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Async Lock Manager ===");
        Console.WriteLine("Demonstrating async synchronization primitives.\n");

        Console.WriteLine("Choose operation:");
        Console.WriteLine("1. Test Async Lock (SemaphoreSlim)");
        Console.WriteLine("2. Test Reader-Writer Lock");
        Console.WriteLine("3. Test Concurrent Access Demo");
        Console.WriteLine("4. Run All Tests");
        Console.Write("\nSelection (1-4): ");

        var choice = Console.ReadLine();
        
        switch (choice)
        {
            case "1":
                await TestAsyncLock();
                break;
            case "2":
                await TestReaderWriterLock();
                break;
            case "3":
                await TestConcurrentAccess();
                break;
            case "4":
                await TestAsyncLock();
                await TestReaderWriterLock();
                await TestConcurrentAccess();
                break;
            default:
                Console.WriteLine("Invalid selection.");
                break;
        }
    }

    /// <summary>
    /// Demonstrates SemaphoreSlim for async-exclusive access.
    /// </summary>
    static async Task TestAsyncLock()
    {
        Console.WriteLine("\n--- Testing Async Lock (SemaphoreSlim) ---\n");

        var tasks = Enumerable.Range(1, 5).Select(async id =>
        {
            Console.WriteLine($"Task {id}: Waiting for lock...");
            await _asyncLock.WaitAsync();
            
            try
            {
                Console.WriteLine($"Task {id}: Acquired lock at {DateTime.Now:HH:mm:ss.fff}");
                await Task.Delay(500); // Simulate async work
                Console.WriteLine($"Task {id}: Releasing lock at {DateTime.Now:HH:mm:ss.fff}");
            }
            finally
            {
                _asyncLock.Release();
            }
        });

        await Task.WhenAll(tasks);
        Console.WriteLine("\nAsync lock test completed.");
    }

    /// <summary>
    /// Demonstrates ReaderWriterLockSlim for read/write coordination.
    /// </summary>
    static async Task TestReaderWriterLock()
    {
        Console.WriteLine("\n--- Testing Reader-Writer Lock ---\n");

        // Initialize shared data
        _sharedData["counter"] = 0;

        var tasks = new List<Task>();

        // Create reader tasks
        for (int i = 0; i < 3; i++)
        {
            int readerId = i;
            tasks.Add(Task.Run(async () =>
            {
                for (int j = 0; j < 3; j++)
                {
                    _rwLock.EnterReadLock();
                    try
                    {
                        var value = _sharedData["counter"];
                        Console.WriteLine($"Reader {readerId}: Read value = {value}");
                        await Task.Delay(100);
                    }
                    finally
                    {
                        _rwLock.ExitReadLock();
                    }
                }
            }));
        }

        // Create writer tasks
        for (int i = 0; i < 2; i++)
        {
            int writerId = i;
            tasks.Add(Task.Run(async () =>
            {
                for (int j = 0; j < 2; j++)
                {
                    _rwLock.EnterWriteLock();
                    try
                    {
                        var current = (int)_sharedData["counter"];
                        _sharedData["counter"] = current + 1;
                        Console.WriteLine($"Writer {writerId}: Updated value to {_sharedData["counter"]}");
                        await Task.Delay(200);
                    }
                    finally
                    {
                        _rwLock.ExitWriteLock();
                    }
                }
            }));
        }

        await Task.WhenAll(tasks);
        Console.WriteLine($"\nFinal counter value: {_sharedData["counter"]}");
    }

    /// <summary>
    /// Demonstrates thread-safe counter with async locking.
    /// </summary>
    static async Task TestConcurrentAccess()
    {
        Console.WriteLine("\n--- Testing Concurrent Access Demo ---\n");

        _accessCounter = 0;
        var tasks = Enumerable.Range(1, 10).Select(async id =>
        {
            await _asyncLock.WaitAsync();
            try
            {
                int current = _accessCounter;
                await Task.Delay(50); // Simulate work
                _accessCounter = current + 1;
                Console.WriteLine($"Task {id}: Incremented counter to {_accessCounter}");
            }
            finally
            {
                _asyncLock.Release();
            }
        });

        await Task.WhenAll(tasks);
        Console.WriteLine($"\nFinal access counter: {_accessCounter} (expected: 10)");
    }
}
