using System.Collections.Concurrent;

namespace ConcurrentBagProcessor;

/// <summary>
/// Thread-safe collection processor using ConcurrentBag for parallel data processing.
/// Demonstrates lock-free concurrent collection operations with multiple producers/consumers.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        var data = new ConcurrentBag<int>();
        var results = new ConcurrentBag<ProcessingResult>();
        
        Console.WriteLine("=== ConcurrentBag Processor ===\n");
        
        // Producer tasks: Add items concurrently
        var producers = Enumerable.Range(0, 3)
            .Select(id => Task.Run(() => Produce(data, id)))
            .ToList();
        
        await Task.WhenAll(producers);
        Console.WriteLine($"\nProduced {data.Count} items\n");
        
        // Consumer tasks: Process items concurrently
        var consumers = Enumerable.Range(0, 4)
            .Select(id => Task.Run(() => Consume(data, results, id)))
            .ToList();
        
        await Task.WhenAll(consumers);
        
        // Display results
        Console.WriteLine($"\nProcessed {results.Count} items:");
        Console.WriteLine($"  Total value: {results.Sum(r => r.Value)}");
        Console.WriteLine($"  Avg duration: {results.Average(r => r.DurationMs):F2}ms");
        
        // Demonstrate ConcurrentBag-specific operations
        DemonstrateBagOperations();
    }
    
    static void Produce(ConcurrentBag<int> bag, int producerId)
    {
        for (int i = 0; i < 10; i++)
        {
            int value = producerId * 100 + i;
            bag.Add(value);
            Console.WriteLine($"Producer {producerId}: Added {value}");
            Thread.Sleep(10);
        }
    }
    
    static async Task Consume(ConcurrentBag<int> source, ConcurrentBag<ProcessingResult> results, int consumerId)
    {
        while (!source.IsEmpty || !Task.Delay(100).IsCompleted)
        {
            if (source.TryTake(out int item))
            {
                var result = ProcessItem(item, consumerId);
                results.Add(result);
                Console.WriteLine($"Consumer {consumerId}: Processed {item} -> {result.Value}");
            }
            else
            {
                await Task.Delay(5);
            }
        }
    }
    
    static ProcessingResult ProcessItem(int item, int consumerId)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        Thread.Sleep(Random.Shared.Next(1, 5)); // Simulate work
        sw.Stop();
        
        return new ProcessingResult
        {
            OriginalItem = item,
            Value = item * 2,
            ConsumerId = consumerId,
            DurationMs = sw.ElapsedMilliseconds
        };
    }
    
    static void DemonstrateBagOperations()
    {
        Console.WriteLine("\n=== ConcurrentBag Operations Demo ===\n");
        
        var bag = new ConcurrentBag<string>();
        
        // Add items
        bag.Add("Apple");
        bag.Add("Banana");
        bag.Add("Cherry");
        
        Console.WriteLine($"Count: {bag.Count}");
        Console.WriteLine($"IsEmpty: {bag.IsEmpty}");
        
        // TryTake - removes and returns (LIFO or FIFO depending on thread)
        if (bag.TryTake(out string? item))
        {
            Console.WriteLine($"Took: {item}");
        }
        
        // TryPeek - returns without removing
        if (bag.TryPeek(out string? peeked))
        {
            Console.WriteLine($"Peeked: {peeked}");
        }
        
        Console.WriteLine($"Final count: {bag.Count}");
    }
}

record ProcessingResult
{
    public int OriginalItem { get; init; }
    public int Value { get; init; }
    public int ConsumerId { get; init; }
    public long DurationMs { get; init; }
}
