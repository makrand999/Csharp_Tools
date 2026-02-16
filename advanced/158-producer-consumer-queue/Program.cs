using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ProducerConsumerQueue;

/// <summary>
/// Producer-Consumer Queue - Thread-safe queue implementation with async support.
/// Demonstrates multiple approaches: BlockingCollection, Channel, and custom implementation.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Producer-Consumer Queue ===");
        Console.WriteLine("Demonstrating thread-safe queue implementations.\n");

        Console.WriteLine("Choose implementation:");
        Console.WriteLine("1. BlockingCollection (Thread-safe blocking queue)");
        Console.WriteLine("2. Channel<T> (Async producer-consumer)");
        Console.WriteLine("3. ConcurrentQueue + Semaphore (Custom implementation)");
        Console.WriteLine("4. Run All Demos");
        Console.Write("\nSelection (1-4): ");

        var choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                await DemoBlockingCollection();
                break;
            case "2":
                await DemoChannel();
                break;
            case "3":
                await DemoConcurrentQueueWithSemaphore();
                break;
            case "4":
                await DemoBlockingCollection();
                await DemoChannel();
                await DemoConcurrentQueueWithSemaphore();
                break;
            default:
                Console.WriteLine("Invalid selection.");
                break;
        }
    }

    /// <summary>
    /// Demonstrates BlockingCollection for producer-consumer scenarios.
    /// </summary>
    static async Task DemoBlockingCollection()
    {
        Console.WriteLine("\n--- BlockingCollection Demo ---\n");

        var queue = new BlockingCollection<int>(boundedCapacity: 10);
        var cts = new CancellationTokenSource();

        var producerTask = Task.Run(async () =>
        {
            for (int i = 1; i <= 20; i++)
            {
                await Task.Delay(100);
                queue.Add(i);
                Console.WriteLine($"Produced: {i}");
            }
            queue.CompleteAdding();
            Console.WriteLine("Producer finished.");
        });

        var consumerTask = Task.Run(() =>
        {
            foreach (var item in queue.GetConsumingEnumerable(cts.Token))
            {
                Task.Delay(150).Wait(); // Simulate processing
                Console.WriteLine($"Consumed: {item}");
            }
            Console.WriteLine("Consumer finished.");
        });

        await Task.WhenAll(producerTask, consumerTask);
        Console.WriteLine("\nBlockingCollection demo completed.");
    }

    /// <summary>
    /// Demonstrates Channel<T> for async producer-consumer patterns.
    /// </summary>
    static async Task DemoChannel()
    {
        Console.WriteLine("\n--- Channel<T> Demo ---\n");

        var channel = Channel.CreateBounded<int>(new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait
        });

        var cts = new CancellationTokenSource();

        var producerTask = Task.Run(async () =>
        {
            for (int i = 1; i <= 20; i++)
            {
                await channel.Writer.WriteAsync(i, cts.Token);
                Console.WriteLine($"[Channel] Produced: {i}");
                await Task.Delay(100);
            }
            channel.Writer.Complete();
            Console.WriteLine("[Channel] Producer finished.");
        });

        var consumerTask = Task.Run(async () =>
        {
            await foreach (var item in channel.Reader.ReadAllAsync(cts.Token))
            {
                await Task.Delay(150); // Simulate processing
                Console.WriteLine($"[Channel] Consumed: {item}");
            }
            Console.WriteLine("[Channel] Consumer finished.");
        });

        await Task.WhenAll(producerTask, consumerTask);
        Console.WriteLine("\nChannel demo completed.");
    }

    /// <summary>
    /// Demonstrates custom producer-consumer with ConcurrentQueue and Semaphore.
    /// </summary>
    static async Task DemoConcurrentQueueWithSemaphore()
    {
        Console.WriteLine("\n--- ConcurrentQueue + Semaphore Demo ---\n");

        var queue = new ConcurrentQueue<int>();
        var semaphore = new SemaphoreSlim(0, int.MaxValue);
        var cts = new CancellationTokenSource();
        var completed = false;

        var producerTask = Task.Run(async () =>
        {
            for (int i = 1; i <= 20; i++)
            {
                await Task.Delay(100);
                queue.Enqueue(i);
                semaphore.Release();
                Console.WriteLine($"[Custom] Produced: {i}");
            }
            completed = true;
            semaphore.Release(); // Signal completion
            Console.WriteLine("[Custom] Producer finished.");
        });

        var consumerTask = Task.Run(async () =>
        {
            while (!completed || !queue.IsEmpty)
            {
                await semaphore.WaitAsync();
                
                if (queue.TryDequeue(out var item))
                {
                    await Task.Delay(150); // Simulate processing
                    Console.WriteLine($"[Custom] Consumed: {item}");
                }
            }
            Console.WriteLine("[Custom] Consumer finished.");
        });

        await Task.WhenAll(producerTask, consumerTask);
        Console.WriteLine("\nConcurrentQueue + Semaphore demo completed.");
    }
}
