using System.Diagnostics;
using System.Collections.Concurrent;

namespace SystemTraceLogger;

/// <summary>
/// System Trace Logger - Captures and logs system events and trace messages.
/// Demonstrates TraceListener, diagnostic sources, and structured logging.
/// </summary>
public class TraceLoggerService : IDisposable
{
    private readonly ConcurrentQueue<LogEntry> _logQueue = new();
    private readonly TraceListener? _consoleListener;
    private bool _disposed;

    public TraceLoggerService()
    {
        _consoleListener = new ConsoleTraceListener();
        Trace.Listeners.Add(_consoleListener);
        Trace.AutoFlush = true;
    }

    public void LogInfo(string message, string? category = null)
    {
        var entry = new LogEntry
        {
            Level = LogLevel.Info,
            Message = message,
            Category = category,
            Timestamp = DateTime.UtcNow
        };
        
        _logQueue.Enqueue(entry);
        Trace.WriteLine($"[INFO] {entry.FormattedMessage}");
    }

    public void LogWarning(string message, string? category = null)
    {
        var entry = new LogEntry
        {
            Level = LogLevel.Warning,
            Message = message,
            Category = category,
            Timestamp = DateTime.UtcNow
        };
        
        _logQueue.Enqueue(entry);
        Trace.WriteLine($"[WARN] {entry.FormattedMessage}");
    }

    public void LogError(string message, string? category = null, Exception? exception = null)
    {
        var entry = new LogEntry
        {
            Level = LogLevel.Error,
            Message = message,
            Category = category,
            Timestamp = DateTime.UtcNow,
            Exception = exception
        };
        
        _logQueue.Enqueue(entry);
        Trace.WriteLine($"[ERROR] {entry.FormattedMessage}");
        if (exception != null)
        {
            Trace.WriteLine($"Exception: {exception}");
        }
    }

    public void LogDebug(string message, string? category = null)
    {
        var entry = new LogEntry
        {
            Level = LogLevel.Debug,
            Message = message,
            Category = category,
            Timestamp = DateTime.UtcNow
        };
        
        _logQueue.Enqueue(entry);
        Trace.WriteLine($"[DEBUG] {entry.FormattedMessage}");
    }

    public IReadOnlyList<LogEntry> GetLogs(LogLevel? minLevel = null, string? category = null)
    {
        var query = _logQueue.AsEnumerable();

        if (minLevel.HasValue)
        {
            query = query.Where(e => e.Level >= minLevel.Value);
        }

        if (category != null)
        {
            query = query.Where(e => e.Category == category);
        }

        return query.OrderByDescending(e => e.Timestamp).ToList();
    }

    public void ExportToJson(string filePath)
    {
        var logs = GetLogs().OrderBy(e => e.Timestamp).ToList();
        var json = System.Text.Json.JsonSerializer.Serialize(logs, new System.Text.Json.JsonSerializerOptions
        {
            WriteIndented = true
        });
        File.WriteAllText(filePath, json);
    }

    public void Clear()
    {
        _logQueue.Clear();
    }

    public void Dispose()
    {
        if (_disposed) return;
        
        if (_consoleListener != null)
        {
            Trace.Listeners.Remove(_consoleListener);
            _consoleListener.Dispose();
        }
        
        _disposed = true;
    }
}

public record LogEntry
{
    public LogLevel Level { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Category { get; init; }
    public DateTime Timestamp { get; init; }
    public Exception? Exception { get; init; }

    public string FormattedMessage => 
        Category != null ? $"[{Category}] {Message}" : Message;
}

public enum LogLevel
{
    Debug = 0,
    Info = 1,
    Warning = 2,
    Error = 3
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== System Trace Logger ===\n");

        using var logger = new TraceLoggerService();

        // Log various messages
        logger.LogInfo("Application started", "Startup");
        logger.LogDebug("Initializing components", "Startup");
        
        await Task.Delay(100);
        
        logger.LogInfo("Processing data...", "Processing");
        
        // Simulate some work
        for (int i = 0; i < 5; i++)
        {
            logger.LogDebug($"Processing item {i + 1}/5", "Processing");
            await Task.Delay(50);
        }
        
        logger.LogWarning("High memory usage detected", "Performance");
        logger.LogError("Connection timeout", "Network", new TimeoutException("Operation timed out"));
        logger.LogInfo("Application shutting down", "Shutdown");

        // Display logs
        Console.WriteLine("\n=== All Logs ===");
        var allLogs = logger.GetLogs();
        foreach (var log in allLogs)
        {
            Console.WriteLine($"[{log.Timestamp:HH:mm:ss.fff}] [{log.Level,7}] {log.FormattedMessage}");
        }

        // Filter by level
        Console.WriteLine("\n=== Errors Only ===");
        var errors = logger.GetLogs(minLevel: LogLevel.Error);
        foreach (var log in errors)
        {
            Console.WriteLine($"[{log.Timestamp:HH:mm:ss.fff}] {log.FormattedMessage}");
            if (log.Exception != null)
            {
                Console.WriteLine($"  Exception: {log.Exception.Message}");
            }
        }

        // Filter by category
        Console.WriteLine("\n=== Processing Category ===");
        var processingLogs = logger.GetLogs(category: "Processing");
        foreach (var log in processingLogs)
        {
            Console.WriteLine($"[{log.Timestamp:HH:mm:ss.fff}] [{log.Level,7}] {log.Message}");
        }

        // Export to JSON
        var jsonFile = Path.Combine(Path.GetTempPath(), $"logs_{Guid.NewGuid()}.json");
        logger.ExportToJson(jsonFile);
        Console.WriteLine($"\nLogs exported to: {jsonFile}");

        // Show JSON content
        var jsonContent = File.ReadAllText(jsonFile);
        Console.WriteLine($"\nJSON Preview (first 500 chars):");
        Console.WriteLine(jsonContent.Substring(0, Math.Min(500, jsonContent.Length)));

        // Cleanup
        File.Delete(jsonFile);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
