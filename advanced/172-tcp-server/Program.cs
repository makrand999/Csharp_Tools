using System.Net;
using System.Net.Sockets;
using System.Text;

namespace TcpServer;

/// <summary>
/// TCP Server - Multi-client network server with async handling
/// Accepts multiple clients and handles them concurrently
/// </summary>
class Program
{
    static readonly List<TcpClient> _clients = [];
    static readonly object _lock = new();
    
    static async Task Main(string[] args)
    {
        int port = args.Length > 0 ? int.Parse(args[0]) : 5000;
        
        var listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        
        Console.WriteLine("TCP Server");
        Console.WriteLine("==========");
        Console.WriteLine($"Listening on port {port}");
        Console.WriteLine("Press 'q' to quit");
        Console.WriteLine();

        var acceptTask = AcceptClients(listener);
        var inputTask = HandleUserInput(listener);
        
        await Task.WhenAll(acceptTask, inputTask);
    }

    static async Task AcceptClients(TcpListener listener)
    {
        while (true)
        {
            var client = await listener.AcceptTcpClientAsync();
            
            lock (_lock)
            {
                _clients.Add(client);
                Console.WriteLine($"[INFO] Client connected ({_clients.Count} total)");
            }
            
            _ = HandleClientAsync(client);
        }
    }

    static async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        using (var stream = client.GetStream())
        {
            var buffer = new byte[1024];
            
            try
            {
                while (client.Connected)
                {
                    var bytesRead = await stream.ReadAsync(buffer);
                    if (bytesRead == 0) break;
                    
                    var message = Encoding.UTF8.GetString(buffer, 0, bytesRead).Trim();
                    Console.WriteLine($"[CLIENT] {message}");
                    
                    // Echo back
                    var response = $"Server received: {message}";
                    var responseBytes = Encoding.UTF8.GetBytes(response);
                    await stream.WriteAsync(responseBytes);
                }
            }
            catch
            {
                // Client disconnected
            }
            finally
            {
                lock (_lock)
                {
                    _clients.Remove(client);
                    Console.WriteLine($"[INFO] Client disconnected ({_clients.Count} remaining)");
                }
            }
        }
    }

    static async Task HandleUserInput(TcpListener listener)
    {
        while (true)
        {
            var input = await Task.Run(() => Console.ReadLine());
            if (input?.ToLower() == "q")
            {
                Console.WriteLine("[INFO] Shutting down server...");
                listener.Stop();
                break;
            }
        }
    }
}
