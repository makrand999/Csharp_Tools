using System.Net.WebSockets;
using System.Text;

namespace WebSocketClient;

/// <summary>
/// WebSocket Client - Real-time bidirectional communication tool
/// Connects to WebSocket servers and sends/receives messages
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        string url = args.Length > 0 ? args[0] : "wss://echo.websocket.org";
        
        Console.WriteLine("WebSocket Client");
        Console.WriteLine("================");
        Console.WriteLine($"Connecting to: {url}");
        Console.WriteLine("Type messages to send (type 'quit' to exit)");
        Console.WriteLine();

        using var ws = new ClientWebSocket();
        
        try
        {
            await ws.ConnectAsync(new Uri(url), CancellationToken.None);
            Console.WriteLine("Connected! Press Enter to start receiving messages...");
            
            var receiveTask = ReceiveMessages(ws);
            var sendTask = SendMessages(ws);
            
            await Task.WhenAll(receiveTask, sendTask);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static async Task ReceiveMessages(ClientWebSocket ws)
    {
        var buffer = new byte[1024 * 4];
        
        while (ws.State == WebSocketState.Open)
        {
            var result = await ws.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            
            if (result.MessageType == WebSocketMessageType.Close)
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "Closing", CancellationToken.None);
                break;
            }
            
            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            Console.WriteLine($"\n[RECEIVED] {message}");
            Console.Write("[SEND] > ");
        }
    }

    static async Task SendMessages(ClientWebSocket ws)
    {
        while (ws.State == WebSocketState.Open)
        {
            Console.Write("[SEND] > ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrEmpty(input) || input.ToLower() == "quit")
            {
                await ws.CloseAsync(WebSocketCloseStatus.NormalClosure, "User quit", CancellationToken.None);
                break;
            }
            
            var buffer = Encoding.UTF8.GetBytes(input);
            await ws.SendAsync(new ArraySegment<byte>(buffer), WebSocketMessageType.Text, true, CancellationToken.None);
            Console.WriteLine("[SENT] Message sent");
        }
    }
}
