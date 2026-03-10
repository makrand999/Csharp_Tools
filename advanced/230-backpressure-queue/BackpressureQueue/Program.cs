using System.Diagnostics.CodeAnalysis;
using System.Threading.Channels;

namespace BackpressureQueue;

/// <summary>
/// Backpressure queue with flow control for handling producer-consumer rate mismatches.
/// Demonstrates bounded queues, watermarks, and adaptive rate limiting.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Backpressure Queue ===\n");
        
        var queue = new BackpressureQueue<string>(capacity: 10, highWatermark: 8, lowWatermark: 3);
        
        var producerTask = Task.Run(() => ProducerAsync(queue));
        var consumerTask = Task.Run(() => ConsumerAsync(queue));
        
        await Task.WhenAll(producerTask, consumerTask);
        
        Console.WriteLine($"\nStatistics:");
        Console.WriteLine($"  Items produced: {queue.ProducedCount}");
        Console.WriteLine($"  Items consumed: {queue.ConsumedCount}");
        Console.WriteLine($"  Backpressure events: {queue.BackpressureEvents}");
        Console.WriteLine($"  Max queue size reached: {queue.MaxQueueSize}");
        
        // Demo backpressure scenarios
        await DemonstrateBackpressure();
    }
    
    static async Task ProducerAsync(BackpressureQueue<string> queue)
    {
        for (int i = 1; i <= 50; i++)
        {
            var item = $"Item-{i:D3}";
            
            // TryAdd returns false if queue is full (backpressure)
            while (!await queue.TryAddAsync(item))
            {
                Console.WriteLine($"Backpressure! Waiting before producing {item}");
                await Task.Delay(100);
            }
            
            Console.WriteLine($"Produced: {item} (queue: {queue.Count})");
            await Task.Delay(30); // Produce faster than consume
        }
        
        queue.CompleteAdding();
        Console.WriteLine("Producer completed");
    }
    
    static async Task ConsumerAsync(BackpressureQueue<string> queue)
    {
        while (!queue.IsCompleted || queue.Count > 0)
        {
            if (await queue.TryTakeAsync(out var item, TimeSpan.FromMilliseconds(100)))
            {
                Console.WriteLine($"Consumed: {item} (queue: {queue.Count})");
                await Task.Delay(50); // Simulate processing
            }
        }
        
        Console.WriteLine("Consumer completed");
    }
    
    static async Task DemonstrateBackpressure()
    {
        Console.WriteLine("\n=== Backpressure Demo ===\n");
        
        var queue = new BackpressureQueue<int>(capacity: 5, highWatermark: 4, lowWatermark: 2);
        
        // Fast producer
        var producer = Task.Run(async () =>
        {
            for (int i = 0; i < 15; i++)
            {
                bool added = await queue.TryAddAsync(i);
                string status = added ? "OK" : "BACKPRESSURE";
                Console.WriteLine($"Produced {i}: [{status}] Queue size: {queue.Count}");
                await Task.Delay(20);
            }
            queue.CompleteAdding();
        });
        
        // Slow consumer
        var consumer = Task.Run(async () =>
        {
            await Task.Delay(100); // Start late
            
            while (!queue.IsCompleted || queue.Count > 0)
            {
                if (await queue.TryTakeAsync(out var item, TimeSpan.FromMilliseconds(50)))
                {
                    Console.WriteLine($"Consumed: {item}");
                    await Task.Delay(100); // Slow processing
                }
            }
        });
        
        await Task.WhenAll(producer, consumer);
        
        Console.WriteLine($"\nDemo complete - Backpressure events: {queue.BackpressureEvents}");
    }
}

/// <summary>
/// Bounded queue with backpressure signaling when capacity thresholds are exceeded.
/// </summary>
class BackpressureQueue<T>
{
    private readonly Channel<T> _channel;
    private readonly int _highWatermark;
    private readonly int _lowWatermark;
    private int _producedCount;
    private int _consumedCount;
    private int _backpressureEvents;
    private int _maxQueueSize;
    private bool _completed;
    
    public int Count => _channel.Count;
    public bool IsCompleted => _completed && _channel.Count == 0;
    public int ProducedCount => Volatile.Read(ref _producedCount);
    public int ConsumedCount => Volatile.Read(ref _consumedCount);
    public int BackpressureEvents => Volatile.Read(ref _backpressureEvents);
    public int MaxQueueSize => Volatile.Read(ref _maxQueueSize);
    
    public BackpressureQueue(int capacity, int highWatermark, int lowWatermark)
    {
        _channel = new Channel<T>(capacity);
        _highWatermark = highWatermark;
        _lowWatermark = lowWatermark;
    }
    
    public async ValueTask<bool> TryAddAsync(T item)
    {
        // Check if we're above high watermark (backpressure zone)
        if (_channel.Count >= _highWatermark)
        {
            Interlocked.Increment(ref _backpressureEvents);
            return false; // Signal backpressure to producer
        }
        
        await _channel.Writer.WriteAsync(item);
        Interlocked.Increment(ref _producedCount);
        
        // Track max size
        int currentSize = _channel.Count;
        Interlocked.Exchange(ref _maxQueueSize, Math.Max(Volatile.Read(ref _maxQueueSize), currentSize));
        
        return true;
    }
    
    public ValueTask<bool> TryTakeAsync([MaybeNullWhen(false)] out T item, TimeSpan timeout)
    {
        var cts = new CancellationTokenSource(timeout);
        
        if (_channel.Reader.TryRead(out item))
        {
            Interlocked.Increment(ref _consumedCount);
            return new ValueTask<bool>(true);
        }
        
        item = default!;
        return new ValueTask<bool>(false);
    }
    
    public void CompleteAdding()
    {
        _completed = true;
        _channel.Writer.Complete();
    }
}

/// <summary>
/// Simple channel implementation for backpressure queue.
/// </summary>
class Channel<T>
{
    private readonly SemaphoreSlim _semaphore;
    private readonly Queue<T> _queue;
    private readonly int _capacity;
    private bool _completed;
    
    public ChannelWriter<T> Writer { get; }
    public ChannelReader<T> Reader { get; }
    public int Count => _queue.Count;
    
    public Channel(int capacity)
    {
        _capacity = capacity;
        _semaphore = new SemaphoreSlim(0, capacity);
        _queue = new Queue<T>();
        Writer = new ChannelWriter<T>(this);
        Reader = new ChannelReader<T>(this);
    }
    
    internal async Task WriteAsync(T item)
    {
        await _semaphore.WaitAsync();
        lock (_queue)
        {
            _queue.Enqueue(item);
        }
    }
    
    internal bool TryRead([MaybeNullWhen(false)] out T item)
    {
        lock (_queue)
        {
            if (_queue.Count > 0)
            {
                item = _queue.Dequeue();
                _semaphore.Release();
                return true;
            }
        }
        item = default!;
        return false;
    }
    
    internal async Task<bool> WaitToReadAsync(CancellationToken ct)
    {
        if (_queue.Count > 0) return true;
        if (_completed) return false;
        
        try
        {
            await _semaphore.WaitAsync(ct);
            return true;
        }
        catch (OperationCanceledException)
        {
            return false;
        }
    }
    
    internal void Complete()
    {
        _completed = true;
    }
}

class ChannelWriter<T>(Channel<T> channel)
{
    public async ValueTask WriteAsync(T item) => await channel.WriteAsync(item);
    
    public void Complete() => channel.Complete();
}

class ChannelReader<T>(Channel<T> channel)
{
    public bool TryRead([MaybeNullWhen(false)] out T item) => channel.TryRead(out item);
    
    public async Task<bool> WaitToReadAsync(CancellationToken ct) => await channel.WaitToReadAsync(ct);
}
