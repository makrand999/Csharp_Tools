using System.Net;
using System.Text;

namespace HttpServer;

/// <summary>
/// HTTP Server - Simple HTTP server built from scratch
/// Handles basic HTTP requests and serves responses
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        int port = args.Length > 0 ? int.Parse(args[0]) : 8080;
        
        var listener = new HttpListener();
        listener.Prefixes.Add($"http://localhost:{port}/");
        listener.Prefixes.Add($"http://127.0.0.1:{port}/");
        listener.Start();
        
        Console.WriteLine("HTTP Server");
        Console.WriteLine("===========");
        Console.WriteLine($"Listening on http://localhost:{port}");
        Console.WriteLine("Available routes:");
        Console.WriteLine("  GET  /          - Home page");
        Console.WriteLine("  GET  /api/time  - Current time");
        Console.WriteLine("  GET  /api/echo  - Echo query params");
        Console.WriteLine("  POST /api/echo  - Echo body");
        Console.WriteLine("Press 'q' to quit");
        Console.WriteLine();

        var cts = new CancellationTokenSource();
        
        _ = Task.Run(() =>
        {
            while (Console.ReadLine()?.ToLower() != "q") { }
            cts.Cancel();
        });

        try
        {
            while (!cts.Token.IsCancellationRequested)
            {
                var context = await listener.GetContextAsync();
                _ = HandleRequestAsync(context);
            }
        }
        catch (OperationCanceledException)
        {
            // Shutdown requested
        }
        finally
        {
            listener.Stop();
            Console.WriteLine("[INFO] Server stopped");
        }
    }

    static async Task HandleRequestAsync(HttpListenerContext context)
    {
        var request = context.Request;
        var response = context.Response;
        
        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] {request.HttpMethod} {request.Url?.PathAndQuery}");
        
        try
        {
            var path = request.Url?.AbsolutePath ?? "/";
            var method = request.HttpMethod;
            
            var (statusCode, contentType, body) = RouteRequest(method, path, request);
            
            response.StatusCode = statusCode;
            response.ContentType = contentType;
            
            var buffer = Encoding.UTF8.GetBytes(body);
            response.ContentLength64 = buffer.Length;
            
            await response.OutputStream.WriteAsync(buffer);
            response.OutputStream.Close();
        }
        catch (Exception ex)
        {
            response.StatusCode = 500;
            var error = Encoding.UTF8.GetBytes($"Error: {ex.Message}");
            await response.OutputStream.WriteAsync(error);
            response.OutputStream.Close();
        }
    }

    static (int statusCode, string contentType, string body) RouteRequest(string method, string path, HttpListenerRequest request)
    {
        return (method, path) switch
        {
            ("GET", "/") => (200, "text/html", GetHomePage()),
            ("GET", "/api/time") => (200, "application/json", $$"""{"time":"{{DateTime.Now:O}}","utc":"{{DateTime.UtcNow:O}}"}"""),
            ("GET", "/api/echo") => (200, "application/json", $$"""{"query":"{{request.Url?.Query}}","params":{{GetQueryParamsJson(request)}}}"""),
            ("POST", "/api/echo") => (200, "text/plain", new StreamReader(request.InputStream).ReadToEnd()),
            _ => (404, "text/plain", "404 Not Found")
        };
    }

    static string GetHomePage() => """
        <!DOCTYPE html>
        <html>
        <head><title>HTTP Server</title></head>
        <body>
            <h1>🚀 C# HTTP Server</h1>
            <p>Server is running!</p>
            <h2>Endpoints:</h2>
            <ul>
                <li><a href="/api/time">GET /api/time</a> - Current time</li>
                <li><a href="/api/echo?test=hello">GET /api/echo</a> - Echo query params</li>
                <li>POST /api/echo - Echo body</li>
            </ul>
        </body>
        </html>
        """;

    static string GetQueryParamsJson(HttpListenerRequest request)
    {
        var queryParams = request.QueryString.AllKeys
            .Where(k => k != null)
            .ToDictionary(k => k!, k => request.QueryString[k!] ?? "");
        
        return System.Text.Json.JsonSerializer.Serialize(queryParams);
    }
}
