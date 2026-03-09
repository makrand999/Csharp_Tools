namespace ReactiveStream;

/// <summary>
/// Reactive stream processing with Rx-like patterns.
/// Demonstrates observable sequences, operators, and backpressure handling.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Reactive Stream Processor ===\n");
        
        // Create a reactive stream
        var stream = new ReactiveStream<int>();
        
        // Build a processing pipeline
        var pipeline = stream
            .Filter(x => x % 2 == 0)           // Only even numbers
            .Map(x => x * 10)                   // Transform
            .Buffer(5)                          // Buffer in batches of 5
            .Flatten();                         // Flatten back
        
        // Subscribe to the pipeline
        var subscriber = new ConsoleSubscriber<int>("Pipeline");
        pipeline.Subscribe(subscriber);
        
        // Publish data
        Console.WriteLine("Publishing numbers 1-20...\n");
        for (int i = 1; i <= 20; i++)
        {
            stream.Publish(i);
            await Task.Delay(50);
        }
        
        stream.Complete();
        await Task.Delay(100);
        
        // Demo operators
        await DemonstrateOperators();
    }
    
    static async Task DemonstrateOperators()
    {
        Console.WriteLine("\n=== Reactive Operators Demo ===\n");
        
        var source = new ReactiveStream<string>();
        
        // Chain multiple operators
        var processed = source
            .Filter(s => s.Length > 3)
            .Map(s => s.ToUpper())
            .Distinct()
            .Take(5);
        
        var subscriber = new ConsoleSubscriber<string>("Operators");
        processed.Subscribe(subscriber);
        
        var inputs = new[] { "hi", "hello", "world", "foo", "bar", "baz", "reactive", "stream" };
        
        foreach (var input in inputs)
        {
            source.Publish(input);
            await Task.Delay(30);
        }
        
        source.Complete();
        await Task.Delay(100);
        
        // Demo merge
        await DemonstrateMerge();
    }
    
    static async Task DemonstrateMerge()
    {
        Console.WriteLine("\n=== Stream Merge Demo ===\n");
        
        var stream1 = new ReactiveStream<int>();
        var stream2 = new ReactiveStream<int>();
        
        var merged = stream1.Merge(stream2);
        var subscriber = new ConsoleSubscriber<int>("Merged");
        merged.Subscribe(subscriber);
        
        // Publish to both streams concurrently
        _ = Task.Run(async () =>
        {
            for (int i = 1; i <= 5; i++)
            {
                stream1.Publish(i * 10);
                await Task.Delay(30);
            }
            stream1.Complete();
        });
        
        _ = Task.Run(async () =>
        {
            for (int i = 1; i <= 5; i++)
            {
                stream2.Publish(i * 100);
                await Task.Delay(45);
            }
            stream2.Complete();
        });
        
        await Task.Delay(500);
    }
}

class ConsoleSubscriber<T> : ISubscriber<T>
{
    private readonly string _name;
    private int _count;
    
    public ConsoleSubscriber(string name) => _name = name;
    
    public void OnNext(T value)
    {
        _count++;
        Console.WriteLine($"[{_name}] Received: {value} (total: {_count})");
    }
    
    public void OnError(Exception error)
    {
        Console.WriteLine($"[{_name}] Error: {error.Message}");
    }
    
    public void OnComplete()
    {
        Console.WriteLine($"[{_name}] Completed. Total items: {_count}");
    }
}

interface ISubscriber<T>
{
    void OnNext(T value);
    void OnError(Exception error);
    void OnComplete();
}

interface IStream<T>
{
    IDisposable Subscribe(ISubscriber<T> subscriber);
}

/// <summary>
/// Reactive stream with publish-subscribe pattern.
/// </summary>
class ReactiveStream<T> : IStream<T>
{
    private readonly List<ISubscriber<T>> _subscribers = new();
    private bool _completed;
    
    public void Publish(T value)
    {
        if (_completed) return;
        
        foreach (var subscriber in _subscribers.ToList())
        {
            try
            {
                subscriber.OnNext(value);
            }
            catch (Exception ex)
            {
                subscriber.OnError(ex);
            }
        }
    }
    
    public void Complete()
    {
        _completed = true;
        foreach (var subscriber in _subscribers.ToList())
        {
            try
            {
                subscriber.OnComplete();
            }
            catch { }
        }
    }
    
    public IDisposable Subscribe(ISubscriber<T> subscriber)
    {
        _subscribers.Add(subscriber);
        return new Subscription(this, subscriber);
    }
    
    private void Unsubscribe(ISubscriber<T> subscriber)
    {
        _subscribers.Remove(subscriber);
    }
    
    private class Subscription(ReactiveStream<T> stream, ISubscriber<T> subscriber) : IDisposable
    {
        public void Dispose() => stream.Unsubscribe(subscriber);
    }
}

/// <summary>
/// Stream operators extension methods.
/// </summary>
static class ReactiveStreamExtensions
{
    public static IStream<T> Filter<T>(this IStream<T> source, Func<T, bool> predicate)
    {
        return new FilterStream<T>(source, predicate);
    }
    
    public static IStream<TResult> Map<T, TResult>(this IStream<T> source, Func<T, TResult> selector)
    {
        return new MapStream<T, TResult>(source, selector);
    }
    
    public static IStream<T> Take<T>(this IStream<T> source, int count)
    {
        return new TakeStream<T>(source, count);
    }
    
    public static IStream<T> Distinct<T>(this IStream<T> source) where T : notnull
    {
        return new DistinctStream<T>(source);
    }
    
    public static IStream<List<T>> Buffer<T>(this IStream<T> source, int size)
    {
        return new BufferStream<T>(source, size);
    }
    
    public static IStream<T> Flatten<T>(this IStream<List<T>> source)
    {
        return new FlattenStream<T>(source);
    }
    
    public static IStream<T> Merge<T>(this IStream<T> source1, IStream<T> source2)
    {
        return new MergeStream<T>(source1, source2);
    }
}

class FilterStream<T> : IStream<T>
{
    private readonly IStream<T> _source;
    private readonly Func<T, bool> _predicate;
    
    public FilterStream(IStream<T> source, Func<T, bool> predicate)
    {
        _source = source;
        _predicate = predicate;
    }
    
    public IDisposable Subscribe(ISubscriber<T> subscriber)
    {
        return _source.Subscribe(new FilterSubscriber(_predicate, subscriber));
    }
    
    private class FilterSubscriber : ISubscriber<T>
    {
        private readonly Func<T, bool> _predicate;
        private readonly ISubscriber<T> _inner;
        
        public FilterSubscriber(Func<T, bool> predicate, ISubscriber<T> inner)
        {
            _predicate = predicate;
            _inner = inner;
        }
        
        public void OnNext(T value)
        {
            if (_predicate(value))
                _inner.OnNext(value);
        }
        
        public void OnError(Exception error) => _inner.OnError(error);
        public void OnComplete() => _inner.OnComplete();
    }
}

class MapStream<T, TResult> : IStream<TResult>
{
    private readonly IStream<T> _source;
    private readonly Func<T, TResult> _selector;
    
    public MapStream(IStream<T> source, Func<T, TResult> selector)
    {
        _source = source;
        _selector = selector;
    }
    
    public IDisposable Subscribe(ISubscriber<TResult> subscriber)
    {
        return _source.Subscribe(new MapSubscriber(_selector, subscriber));
    }
    
    private class MapSubscriber : ISubscriber<T>
    {
        private readonly Func<T, TResult> _selector;
        private readonly ISubscriber<TResult> _inner;
        
        public MapSubscriber(Func<T, TResult> selector, ISubscriber<TResult> inner)
        {
            _selector = selector;
            _inner = inner;
        }
        
        public void OnNext(T value) => _inner.OnNext(_selector(value));
        public void OnError(Exception error) => _inner.OnError(error);
        public void OnComplete() => _inner.OnComplete();
    }
}

class TakeStream<T> : IStream<T>
{
    private readonly IStream<T> _source;
    private readonly int _count;
    
    public TakeStream(IStream<T> source, int count)
    {
        _source = source;
        _count = count;
    }
    
    public IDisposable Subscribe(ISubscriber<T> subscriber)
    {
        return _source.Subscribe(new TakeSubscriber(_count, subscriber));
    }
    
    private class TakeSubscriber : ISubscriber<T>
    {
        private readonly int _count;
        private readonly ISubscriber<T> _inner;
        private int _remaining;
        
        public TakeSubscriber(int count, ISubscriber<T> inner)
        {
            _count = count;
            _inner = inner;
            _remaining = count;
        }
        
        public void OnNext(T value)
        {
            if (_remaining > 0)
            {
                _remaining--;
                _inner.OnNext(value);
                
                if (_remaining == 0)
                    _inner.OnComplete();
            }
        }
        
        public void OnError(Exception error) => _inner.OnError(error);
        public void OnComplete()
        {
            if (_remaining > 0)
                _inner.OnComplete();
        }
    }
}

class DistinctStream<T> : IStream<T> where T : notnull
{
    private readonly IStream<T> _source;
    private readonly HashSet<T> _seen = new();
    
    public DistinctStream(IStream<T> source) => _source = source;
    
    public IDisposable Subscribe(ISubscriber<T> subscriber)
    {
        return _source.Subscribe(new DistinctSubscriber(_seen, subscriber));
    }
    
    private class DistinctSubscriber : ISubscriber<T>
    {
        private readonly HashSet<T> _seen;
        private readonly ISubscriber<T> _inner;
        
        public DistinctSubscriber(HashSet<T> seen, ISubscriber<T> inner)
        {
            _seen = seen;
            _inner = inner;
        }
        
        public void OnNext(T value)
        {
            if (_seen.Add(value))
                _inner.OnNext(value);
        }
        
        public void OnError(Exception error) => _inner.OnError(error);
        public void OnComplete() => _inner.OnComplete();
    }
}

class BufferStream<T> : IStream<List<T>>
{
    private readonly IStream<T> _source;
    private readonly int _size;
    
    public BufferStream(IStream<T> source, int size)
    {
        _source = source;
        _size = size;
    }
    
    public IDisposable Subscribe(ISubscriber<List<T>> subscriber)
    {
        return _source.Subscribe(new BufferSubscriber(_size, subscriber));
    }
    
    private class BufferSubscriber : ISubscriber<T>
    {
        private readonly int _size;
        private readonly ISubscriber<List<T>> _inner;
        private List<T> _buffer = new();
        
        public BufferSubscriber(int size, ISubscriber<List<T>> inner)
        {
            _size = size;
            _inner = inner;
        }
        
        public void OnNext(T value)
        {
            _buffer.Add(value);
            if (_buffer.Count >= _size)
            {
                _inner.OnNext(_buffer);
                _buffer = new List<T>();
            }
        }
        
        public void OnError(Exception error) => _inner.OnError(error);
        
        public void OnComplete()
        {
            if (_buffer.Count > 0)
                _inner.OnNext(_buffer);
            _inner.OnComplete();
        }
    }
}

class FlattenStream<T> : IStream<T>
{
    private readonly IStream<List<T>> _source;
    
    public FlattenStream(IStream<List<T>> source) => _source = source;
    
    public IDisposable Subscribe(ISubscriber<T> subscriber)
    {
        return _source.Subscribe(new FlattenSubscriber(subscriber));
    }
    
    private class FlattenSubscriber : ISubscriber<List<T>>
    {
        private readonly ISubscriber<T> _inner;
        
        public FlattenSubscriber(ISubscriber<T> inner) => _inner = inner;
        
        public void OnNext(List<T> value)
        {
            foreach (var item in value)
                _inner.OnNext(item);
        }
        
        public void OnError(Exception error) => _inner.OnError(error);
        public void OnComplete() => _inner.OnComplete();
    }
}

class MergeStream<T> : IStream<T>
{
    private readonly IStream<T> _source1;
    private readonly IStream<T> _source2;
    
    public MergeStream(IStream<T> source1, IStream<T> source2)
    {
        _source1 = source1;
        _source2 = source2;
    }
    
    public IDisposable Subscribe(ISubscriber<T> subscriber)
    {
        var sub1 = _source1.Subscribe(subscriber);
        var sub2 = _source2.Subscribe(subscriber);
        
        return new MergeSubscription(sub1, sub2);
    }
    
    private class MergeSubscription(IDisposable sub1, IDisposable sub2) : IDisposable
    {
        public void Dispose()
        {
            sub1.Dispose();
            sub2.Dispose();
        }
    }
}
