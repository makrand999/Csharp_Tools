using System.Net;
using System.Net.Sockets;
using System.Collections.Concurrent;

namespace SocketServerPool;

/// <summary>
/// Socket Server Pool - Manages a pool of reusable socket connections.
/// Demonstrates connection pooling and efficient socket management.
/// </summary>
public class SocketPool : IDisposable
{
    private readonly ConcurrentBag<Socket> _availableSockets;
    private readonly ConcurrentBag<Socket> _inUseSockets;
    private readonly IPEndPoint _endPoint;
    private readonly int _maxPoolSize;
    private readonly int _connectionTimeout;
    private int _totalCreated;
    private int _totalBorrowed;
    private int _totalReturned;

    public SocketPool(string host, int port, int maxPoolSize = 10, int connectionTimeout = 5000)
    {
        _endPoint = new IPEndPoint(IPAddress.Parse(host), port);
        _maxPoolSize = maxPoolSize;
        _connectionTimeout = connectionTimeout;
        _availableSockets = new ConcurrentBag<Socket>();
        _inUseSockets = new ConcurrentBag<Socket>();
    }

    public Socket Borrow()
    {
        Interlocked.Increment(ref _totalBorrowed);

        // Try to get an existing socket from the pool
        if (_availableSockets.TryTake(out var socket))
        {
            if (socket.Connected)
            {
                _inUseSockets.Add(socket);
                return socket;
            }
            else
            {
                socket.Dispose();
            }
        }

        // Create new socket if pool is not at capacity
        if (_availableSockets.Count + _inUseSockets.Count < _maxPoolSize)
        {
            socket = CreateSocket();
            Interlocked.Increment(ref _totalCreated);
            _inUseSockets.Add(socket);
            return socket;
        }

        throw new InvalidOperationException("Socket pool exhausted");
    }

    public void Return(Socket socket)
    {
        if (socket != null && socket.Connected)
        {
            _inUseSockets.TryTake(out socket);
            _availableSockets.Add(socket);
            Interlocked.Increment(ref _totalReturned);
        }
        else if (socket != null)
        {
            _inUseSockets.TryTake(out socket);
            socket.Dispose();
        }
    }

    private Socket CreateSocket()
    {
        var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        socket.SendTimeout = _connectionTimeout;
        socket.ReceiveTimeout = _connectionTimeout;
        socket.Connect(_endPoint);
        return socket;
    }

    public PoolStatistics GetStatistics()
    {
        return new PoolStatistics
        {
            AvailableCount = _availableSockets.Count,
            InUseCount = _inUseSockets.Count,
            TotalCreated = _totalCreated,
            TotalBorrowed = _totalBorrowed,
            TotalReturned = _totalReturned,
            MaxPoolSize = _maxPoolSize
        };
    }

    public void Dispose()
    {
        foreach (var socket in _availableSockets.Concat(_inUseSockets))
        {
            socket.Dispose();
        }
    }
}

public record PoolStatistics
{
    public int AvailableCount { get; init; }
    public int InUseCount { get; init; }
    public int TotalCreated { get; init; }
    public int TotalBorrowed { get; init; }
    public int TotalReturned { get; init; }
    public int MaxPoolSize { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Socket Server Pool ===\n");
        Console.WriteLine("Demonstrating connection pooling pattern\n");

        // Start a simple echo server for testing
        var serverTask = StartEchoServerAsync(5000);
        await Task.Delay(100); // Give server time to start

        // Create socket pool
        using var pool = new SocketPool("127.0.0.1", 5000, maxPoolSize: 5);

        Console.WriteLine("Simulating concurrent socket usage...\n");

        var tasks = new List<Task>();
        for (int i = 0; i < 10; i++)
        {
            var taskId = i;
            tasks.Add(Task.Run(async () =>
            {
                try
                {
                    var socket = pool.Borrow();
                    Console.WriteLine($"Task {taskId}: Borrowed socket (pool available: {pool.GetStatistics().AvailableCount})");

                    // Simulate some socket work
                    await Task.Delay(100);

                    pool.Return(socket);
                    Console.WriteLine($"Task {taskId}: Returned socket");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Task {taskId}: Error - {ex.Message}");
                }
            }));
        }

        await Task.WhenAll(tasks);

        // Display statistics
        var stats = pool.GetStatistics();
        Console.WriteLine($"\n=== Pool Statistics ===");
        Console.WriteLine($"Max Pool Size: {stats.MaxPoolSize}");
        Console.WriteLine($"Total Created: {stats.TotalCreated}");
        Console.WriteLine($"Total Borrowed: {stats.TotalBorrowed}");
        Console.WriteLine($"Total Returned: {stats.TotalReturned}");
        Console.WriteLine($"Currently Available: {stats.AvailableCount}");
        Console.WriteLine($"Currently In Use: {stats.InUseCount}");

        Console.WriteLine($"\nWithout pooling, we would have created {stats.TotalBorrowed} connections.");
        Console.WriteLine($"With pooling, we only created {stats.TotalCreated} connections.");
        Console.WriteLine($"Connection reuse ratio: {stats.TotalBorrowed * 100.0 / stats.TotalCreated:F1}%");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static async Task StartEchoServerAsync(int port)
    {
        await Task.Run(() =>
        {
            var listener = new TcpListener(IPAddress.Loopback, port);
            listener.Start();
            Console.WriteLine($"Echo server started on port {port}\n");

            try
            {
                while (true)
                {
                    var client = listener.AcceptTcpClient();
                    _ = Task.Run(() =>
                    {
                        using var stream = client.GetStream();
                        var buffer = new byte[1024];
                        try
                        {
                            while (true)
                            {
                                var bytesRead = stream.Read(buffer, 0, buffer.Length);
                                if (bytesRead == 0) break;
                                stream.Write(buffer, 0, bytesRead);
                            }
                        }
                        catch { }
                        finally
                        {
                            client.Close();
                        }
                    });
                }
            }
            catch { }
            finally
            {
                listener.Stop();
            }
        });
    }
}
