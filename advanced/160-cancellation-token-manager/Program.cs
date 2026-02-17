using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CancellationTokenManager;

/// <summary>
/// CancellationToken Manager - Utility for coordinating cancellation across multiple operations.
/// Demonstrates linked tokens, timeouts, and graceful shutdown patterns.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== CancellationToken Manager ===");
        Console.WriteLine("Coordinating cancellation across async operations.\n");

        Console.WriteLine("Choose operation:");
        Console.WriteLine("1. Basic Cancellation Demo");
        Console.WriteLine("2. Linked Tokens Demo");
        Console.WriteLine("3. Timeout Cancellation");
        Console.WriteLine("4. Graceful Shutdown Pattern");
        Console.WriteLine("5. Run All Demos");
        Console.Write("\nSelection (1-5): ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await BasicCancellationDemo();
                break;
            case "2":
                await LinkedTokensDemo();
                break;
            case "3":
                await TimeoutCancellationDemo();
                break;
            case "4":
                await GracefulShutdownDemo();
                break;
            case "5":
                await BasicCancellationDemo();
                await LinkedTokensDemo();
                await TimeoutCancellationDemo();
                await GracefulShutdownDemo();
                break;
            default:
                Console.WriteLine("Invalid selection.");
                break;
        }
    }

    /// <summary>
    /// Demonstrates basic cancellation token usage.
    /// </summary>
    static async Task BasicCancellationDemo()
    {
        Console.WriteLine("\n--- Basic Cancellation Demo ---\n");

        using var cts = new CancellationTokenSource();
        
        // Start a cancellable operation
        var task = Task.Run(async () =>
        {
            for (int i = 1; i <= 100; i++)
            {
                cts.Token.ThrowIfCancellationRequested();
                
                Console.WriteLine($"Working... {i}%");
                await Task.Delay(200, cts.Token);
            }
            return "Completed!";
        }, cts.Token);

        // Cancel after 2 seconds
        await Task.Delay(2000);
        Console.WriteLine("\nCancelling operation...");
        cts.Cancel();

        try
        {
            await task;
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation was cancelled.");
        }
    }

    /// <summary>
    /// Demonstrates linked cancellation tokens for multiple cancellation sources.
    /// </summary>
    static async Task LinkedTokensDemo()
    {
        Console.WriteLine("\n--- Linked Tokens Demo ---\n");

        using var cts1 = new CancellationTokenSource();
        using var cts2 = new CancellationTokenSource();
        
        // Create a linked token that cancels when either source is cancelled
        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
            cts1.Token, 
            cts2.Token
        );

        var tasks = new List<Task>();

        // Task 1: Monitors first cancellation source
        tasks.Add(Task.Run(async () =>
        {
            await Task.Delay(1500);
            Console.WriteLine("Source 1 triggered cancellation.");
            cts1.Cancel();
        }));

        // Task 2: Monitors second cancellation source
        tasks.Add(Task.Run(async () =>
        {
            await Task.Delay(3000);
            Console.WriteLine("Source 2 triggered cancellation.");
            cts2.Cancel();
        }));

        // Task 3: Uses the linked token
        tasks.Add(Task.Run(async () =>
        {
            try
            {
                for (int i = 1; i <= 10; i++)
                {
                    linkedCts.Token.ThrowIfCancellationRequested();
                    Console.WriteLine($"Linked task working... {i}");
                    await Task.Delay(500, linkedCts.Token);
                }
            }
            catch (OperationCanceledException)
            {
                Console.WriteLine("Linked task was cancelled by one of the sources.");
            }
        }));

        await Task.WhenAll(tasks);
    }

    /// <summary>
    /// Demonstrates timeout-based cancellation.
    /// </summary>
    static async Task TimeoutCancellationDemo()
    {
        Console.WriteLine("\n--- Timeout Cancellation Demo ---\n");

        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));

        try
        {
            Console.WriteLine("Starting operation with 3-second timeout...");
            await Task.Run(async () =>
            {
                for (int i = 1; i <= 10; i++)
                {
                    cts.Token.ThrowIfCancellationRequested();
                    Console.WriteLine($"Working... {i}/10");
                    await Task.Delay(500, cts.Token);
                }
            }, cts.Token);
            
            Console.WriteLine("Operation completed before timeout.");
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine("Operation timed out and was cancelled.");
        }
        catch (TimeoutException)
        {
            Console.WriteLine("Operation timed out.");
        }
    }

    /// <summary>
    /// Demonstrates graceful shutdown pattern for multiple long-running tasks.
    /// </summary>
    static async Task GracefulShutdownDemo()
    {
        Console.WriteLine("\n--- Graceful Shutdown Pattern Demo ---\n");

        using var cts = new CancellationTokenSource();

        // Start multiple worker tasks
        var workers = new List<Task>();
        for (int i = 1; i <= 3; i++)
        {
            int workerId = i;
            workers.Add(Task.Run(async () =>
            {
                int itemProcessed = 0;
                try
                {
                    for (int j = 1; j <= 10; j++)
                    {
                        cts.Token.ThrowIfCancellationRequested();
                        Console.WriteLine($"Worker {workerId}: Processing item {j}");
                        itemProcessed = j;
                        await Task.Delay(300, cts.Token);
                    }
                    Console.WriteLine($"Worker {workerId}: Completed all items.");
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine($"Worker {workerId}: Gracefully stopped at item {itemProcessed}.");
                }
            }));
        }

        // Simulate shutdown request after 2 seconds
        await Task.Delay(2000);
        Console.WriteLine("\n*** Shutdown requested ***\n");
        cts.Cancel();

        // Wait for all workers to finish gracefully
        await Task.WhenAll(workers);
        Console.WriteLine("\nAll workers have shut down gracefully.");
    }
}
