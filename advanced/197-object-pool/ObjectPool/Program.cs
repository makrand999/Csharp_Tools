using System.Collections.Concurrent;
using System.Text;

namespace ObjectPool;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Object Pool Pattern - Efficient object reuse and lifecycle management");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project ObjectPool.csproj -- demo");
            Console.WriteLine("  dotnet run --project ObjectPool.csproj -- interactive");
            return 0;
        }

        if (args[0].Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            RunDemo();
            return 0;
        }

        if (args[0].Equals("interactive", StringComparison.OrdinalIgnoreCase))
        {
            RunInteractiveMode();
            return 0;
        }

        Console.WriteLine($"Unknown command: {args[0]}");
        Console.WriteLine("Use 'demo' or 'interactive'");
        return 1;
    }

    private static void RunDemo()
    {
        Console.WriteLine("=== Object Pool Pattern Demo ===\n");

        // Demo 1: Database Connection Pool
        Console.WriteLine("1. Database Connection Pool");
        Console.WriteLine("---------------------------");
        
        var connectionPool = new ObjectPool<DbConnection>(
            factory: () => new DbConnection(),
            minSize: 2,
            maxSize: 5,
            resetAction: conn => conn.Reset()
        );

        // Simulate multiple clients using connections
        var tasks = new List<Task>();
        for (int i = 0; i < 8; i++)
        {
            int clientId = i;
            tasks.Add(Task.Run(async () =>
            {
                await using var conn = await connectionPool.GetAsync();
                Console.WriteLine($"Client {clientId}: Using connection {conn.Value.Id}");
                await conn.Value.ExecuteQueryAsync($"SELECT * FROM table_{clientId}");
            }));
        }
        Task.WaitAll(tasks.ToArray());

        Console.WriteLine($"\nPool Statistics:");
        Console.WriteLine($"  Total created: {connectionPool.Statistics.TotalCreated}");
        Console.WriteLine($"  Total borrowed: {connectionPool.Statistics.TotalBorrowed}");
        Console.WriteLine($"  Total returned: {connectionPool.Statistics.TotalReturned}");
        Console.WriteLine($"  Current available: {connectionPool.Statistics.AvailableCount}");
        Console.WriteLine();

        // Demo 2: Thread Pool for Expensive Objects
        Console.WriteLine("2. Expensive Object Reuse (StringBuilder Pool)");
        Console.WriteLine("----------------------------------------------");
        
        var stringBuilderPool = new ObjectPool<StringBuilder>(
            factory: () => new StringBuilder(1024),
            minSize: 4,
            maxSize: 10,
            resetAction: sb => sb.Clear()
        );

        var writeTasks = new List<Task>();
        for (int i = 0; i < 15; i++)
        {
            int taskId = i;
            writeTasks.Add(Task.Run(async () =>
            {
                await using var sb = await stringBuilderPool.GetAsync();
                sb.Value.Append($"Task {taskId}: ");
                for (int j = 0; j < 5; j++)
                {
                    sb.Value.Append($"item_{j},");
                }
                Console.WriteLine($"Task {taskId}: {sb.Value.ToString().TrimEnd(',')}");
            }));
        }
        Task.WaitAll(writeTasks.ToArray());

        Console.WriteLine($"\nPool Statistics:");
        Console.WriteLine($"  Total created: {stringBuilderPool.Statistics.TotalCreated}");
        Console.WriteLine($"  Total borrowed: {stringBuilderPool.Statistics.TotalBorrowed}");
        Console.WriteLine($"  Reuse rate: {stringBuilderPool.Statistics.ReuseRate:P1}");
        Console.WriteLine();

        // Demo 3: Buffer Pool for Network Operations
        Console.WriteLine("3. Buffer Pool (Memory Pool)");
        Console.WriteLine("----------------------------");
        
        var bufferPool = new ObjectPool<byte[]>(
            factory: () => new byte[1024],
            minSize: 5,
            maxSize: 20,
            resetAction: buffer => Array.Clear(buffer, 0, buffer.Length)
        );

        // Simulate network packet processing
        var packetTasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            int packetId = i;
            packetTasks.Add(Task.Run(async () =>
            {
                await using var buffer = await bufferPool.GetAsync();
                
                // Simulate writing data to buffer
                var data = System.Text.Encoding.UTF8.GetBytes($"Packet {packetId} data");
                Array.Copy(data, buffer.Value, data.Length);
                
                Console.WriteLine($"Packet {packetId}: Processed {data.Length} bytes");
            }));
        }
        Task.WaitAll(packetTasks.ToArray());

        Console.WriteLine($"\nPool Statistics:");
        Console.WriteLine($"  Total created: {bufferPool.Statistics.TotalCreated}");
        Console.WriteLine($"  Memory saved: ~{bufferPool.Statistics.TotalBorrowed * 1024 / 1024}KB (vs new allocations)");
        Console.WriteLine();

        // Demo 4: Custom Pooled Object (HttpClient-like)
        Console.WriteLine("4. Custom Pooled Resource (Simulated HTTP Client)");
        Console.WriteLine("-------------------------------------------------");
        
        var httpClientPool = new ObjectPool<PooledHttpClient>(
            factory: () => new PooledHttpClient(),
            minSize: 2,
            maxSize: 5,
            resetAction: client => client.Reset()
        );

        var httpTasks = new List<Task>();
        var endpoints = new[] { "/api/users", "/api/products", "/api/orders", "/api/inventory", "/api/reports" };
        
        foreach (var endpoint in endpoints)
        {
            httpTasks.Add(Task.Run(async () =>
            {
                await using var client = await httpClientPool.GetAsync();
                var response = await client.Value.GetAsync(endpoint);
                Console.WriteLine($"GET {endpoint} => {response.StatusCode} ({response.ElapsedMs}ms)");
            }));
        }
        Task.WaitAll(httpTasks.ToArray());

        Console.WriteLine($"\nPool Statistics:");
        Console.WriteLine($"  Total created: {httpClientPool.Statistics.TotalCreated}");
        Console.WriteLine($"  Connections reused: {httpClientPool.Statistics.TotalBorrowed - httpClientPool.Statistics.TotalCreated}");
        Console.WriteLine();

        Console.WriteLine("Demo completed!");
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Object Pool Pattern (Interactive Mode)");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine();

        var pool = new ObjectPool<WorkItem>(
            factory: () => new WorkItem(),
            minSize: 2,
            maxSize: 10,
            resetAction: item => item.Reset()
        );
        var borrowedItems = new Dictionary<string, PooledObject<WorkItem>>();

        while (true)
        {
            Console.Write("op> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            try
            {
                switch (cmd)
                {
                    case "quit":
                    case "exit":
                        return;

                    case "help":
                        ShowHelp();
                        break;

                    case "borrow":
                        {
                            var item = pool.GetAsync().Result;
                            var id = $"item_{borrowedItems.Count + 1}";
                            borrowedItems[id] = item;
                            item.Value.Data = $"Initialized {id}";
                            Console.WriteLine($"Borrowed: {id} (Pool object #{item.ObjectId})");
                            break;
                        }

                    case "return":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: return <item-id>");
                            break;
                        }
                        var returnId = parts[1];
                        if (borrowedItems.TryGetValue(returnId, out var pooledItem))
                        {
                            pooledItem.DisposeAsync().AsTask().Wait();
                            borrowedItems.Remove(returnId);
                            Console.WriteLine($"Returned: {returnId}");
                        }
                        else
                        {
                            Console.WriteLine($"Item {returnId} not found");
                        }
                        break;

                    case "stats":
                        var stats = pool.Statistics;
                        Console.WriteLine($"Total created: {stats.TotalCreated}");
                        Console.WriteLine($"Total borrowed: {stats.TotalBorrowed}");
                        Console.WriteLine($"Total returned: {stats.TotalReturned}");
                        Console.WriteLine($"Available: {stats.AvailableCount}");
                        Console.WriteLine($"Reuse rate: {stats.ReuseRate:P1}");
                        break;

                    case "list":
                        if (borrowedItems.Count == 0)
                        {
                            Console.WriteLine("No items currently borrowed");
                        }
                        else
                        {
                            Console.WriteLine("Borrowed items:");
                            foreach (var kvp in borrowedItems)
                            {
                                Console.WriteLine($"  {kvp.Key}: {kvp.Value.Value.Data}");
                            }
                        }
                        break;

                    case "return-all":
                        foreach (var kvp in borrowedItems.ToList())
                        {
                            kvp.Value.DisposeAsync().AsTask().Wait();
                        }
                        borrowedItems.Clear();
                        Console.WriteLine("All items returned");
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {cmd}. Type 'help' for commands.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  borrow       - Borrow an object from the pool");
        Console.WriteLine("  return <id>  - Return a borrowed object");
        Console.WriteLine("  stats        - Show pool statistics");
        Console.WriteLine("  list         - List borrowed items");
        Console.WriteLine("  return-all   - Return all borrowed items");
        Console.WriteLine("  quit         - Exit");
        Console.WriteLine();
    }
}

// Simulated database connection
public class DbConnection : IAsyncDisposable
{
    private static int _idCounter;
    public int Id { get; } = Interlocked.Increment(ref _idCounter);
    
    public async Task ExecuteQueryAsync(string query)
    {
        await Task.Delay(50); // Simulate query execution
    }
    
    public void Reset()
    {
        // Reset connection state
    }
    
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

// Simulated HTTP client
public class PooledHttpClient : IAsyncDisposable
{
    private static int _idCounter;
    public int Id { get; } = Interlocked.Increment(ref _idCounter);
    
    public async Task<HttpResponse> GetAsync(string endpoint)
    {
        await Task.Delay(30); // Simulate network call
        return new HttpResponse { StatusCode = 200, ElapsedMs = 30 };
    }
    
    public void Reset()
    {
        // Reset client state
    }
    
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

public class HttpResponse
{
    public int StatusCode { get; set; }
    public long ElapsedMs { get; set; }
}

// Work item for interactive mode
public class WorkItem : IAsyncDisposable
{
    public string Data { get; set; } = string.Empty;
    
    public void Reset()
    {
        Data = string.Empty;
    }
    
    public ValueTask DisposeAsync() => ValueTask.CompletedTask;
}

// Pool statistics
public record PoolStatistics(
    int TotalCreated,
    int TotalBorrowed,
    int TotalReturned,
    int AvailableCount
)
{
    public double ReuseRate => TotalBorrowed > 0 ? (double)(TotalBorrowed - TotalCreated) / TotalBorrowed : 0;
}

// Pooled object wrapper with async disposal
public class PooledObject<T> : IAsyncDisposable where T : class
{
    private readonly ObjectPool<T> _pool;
    private bool _disposed;
    
    public T Value { get; }
    public int ObjectId { get; }
    
    public PooledObject(ObjectPool<T> pool, T value, int objectId)
    {
        _pool = pool;
        Value = value;
        ObjectId = objectId;
    }
    
    public async ValueTask DisposeAsync()
    {
        if (!_disposed)
        {
            await _pool.ReturnAsync(this);
            _disposed = true;
        }
    }
}

// Generic object pool implementation
public class ObjectPool<T> : IDisposable where T : class
{
    private readonly ConcurrentBag<PooledObject<T>> _available = new();
    private readonly Func<T> _factory;
    private readonly Action<T> _resetAction;
    private readonly int _minSize;
    private readonly int _maxSize;
    private int _totalCreated;
    private int _totalBorrowed;
    private int _totalReturned;
    private int _currentSize;
    private readonly SemaphoreSlim _semaphore;
    private bool _disposed;

    public ObjectPool(Func<T> factory, int minSize = 2, int maxSize = 10, Action<T>? resetAction = null)
    {
        _factory = factory;
        _minSize = minSize;
        _maxSize = maxSize;
        _resetAction = resetAction ?? (_ => { });
        _semaphore = new SemaphoreSlim(maxSize);
        
        // Pre-create minimum objects
        for (int i = 0; i < minSize; i++)
        {
            var obj = CreatePooledObject();
            _available.Add(obj);
        }
    }

    public async Task<PooledObject<T>> GetAsync()
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ObjectPool<T>));
        
        await _semaphore.WaitAsync();
        
        if (_available.TryTake(out var pooled))
        {
            Interlocked.Increment(ref _totalBorrowed);
            return pooled;
        }
        
        // Create new object if under max
        var newObj = CreatePooledObject();
        Interlocked.Increment(ref _totalBorrowed);
        return newObj;
    }

    public async ValueTask ReturnAsync(PooledObject<T> pooled)
    {
        if (_disposed) return;
        
        _resetAction(pooled.Value);
        _available.Add(pooled);
        Interlocked.Increment(ref _totalReturned);
        _semaphore.Release();
    }

    public PoolStatistics Statistics => new PoolStatistics(
        Volatile.Read(ref _totalCreated),
        Volatile.Read(ref _totalBorrowed),
        Volatile.Read(ref _totalReturned),
        _available.Count
    );

    private PooledObject<T> CreatePooledObject()
    {
        var obj = _factory();
        var objectId = Interlocked.Increment(ref _totalCreated);
        Interlocked.Increment(ref _currentSize);
        return new PooledObject<T>(this, obj, objectId);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _disposed = true;
            _semaphore.Dispose();
            _available.Clear();
        }
    }
}
