using System.Threading.Channels;

namespace ChannelPipeline;

/// <summary>
/// Channel-based pipeline for producer-consumer scenarios using System.Threading.Channels.
/// Demonstrates bounded channels, backpressure, and async stream processing.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Channel-Based Pipeline ===\n");
        
        // Create a bounded channel with backpressure
        var channel = Channel.CreateBounded<string>(new BoundedChannelOptions(10)
        {
            FullMode = BoundedChannelFullMode.Wait,
            SingleReader = false,
            SingleWriter = false
        });
        
        var producerTask = ProduceAsync(channel.Writer);
        var consumerTasks = Enumerable.Range(0, 3)
            .Select(id => ConsumeAsync(channel.Reader, id))
            .ToList();
        
        await producerTask;
        channel.Writer.Complete();
        
        await Task.WhenAll(consumerTasks);
        
        Console.WriteLine("\n=== Pipeline Complete ===");
        
        // Demonstrate different channel types
        await DemonstrateChannelTypes();
    }
    
    static async Task ProduceAsync(ChannelWriter<string> writer)
    {
        for (int i = 1; i <= 20; i++)
        {
            await writer.WriteAsync($"Item-{i:D3}");
            Console.WriteLine($"Produced: Item-{i:D3}");
            await Task.Delay(50);
        }
    }
    
    static async Task ConsumeAsync(ChannelReader<string> reader, int consumerId)
    {
        await foreach (var item in reader.ReadAllAsync())
        {
            Console.WriteLine($"Consumer {consumerId}: Processing {item}");
            await Task.Delay(100); // Simulate processing
        }
        Console.WriteLine($"Consumer {consumerId}: Completed");
    }
    
    static async Task DemonstrateChannelTypes()
    {
        Console.WriteLine("\n=== Channel Types Demo ===\n");
        
        // Unbounded channel - no limit
        var unbounded = Channel.CreateUnbounded<int>();
        await unbounded.Writer.WriteAsync(42);
        Console.WriteLine($"Unbounded: {await unbounded.Reader.ReadAsync()}");
        
        // Bounded channel with drop newest
        var dropNewest = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
        {
            FullMode = BoundedChannelFullMode.DropNewest
        });
        
        for (int i = 0; i < 5; i++)
        {
            bool written = dropNewest.Writer.TryWrite(i);
            Console.WriteLine($"DropNewest - Wrote {i}: {written}");
        }
        
        // Bounded channel with drop oldest
        var dropOldest = Channel.CreateBounded<int>(new BoundedChannelOptions(3)
        {
            FullMode = BoundedChannelFullMode.DropOldest
        });
        
        for (int i = 0; i < 5; i++)
        {
            bool written = dropOldest.Writer.TryWrite(i);
            Console.WriteLine($"DropOldest - Wrote {i}: {written}");
        }
        
        // Read remaining items
        Console.Write("DropOldest contents: ");
        while (dropOldest.Reader.TryRead(out var item))
        {
            Console.Write($"{item} ");
        }
        Console.WriteLine();
    }
}
