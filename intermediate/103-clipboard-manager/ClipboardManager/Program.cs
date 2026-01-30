using System.Text.Json;

namespace ClipboardManager;

/// <summary>
/// Clipboard history manager - captures, stores, and retrieves clipboard entries.
/// Useful for developers who need to track copied items and quickly access history.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        var command = args.Length > 0 ? args[0].ToLower() : "help";
        var historyFile = Path.Combine(AppContext.BaseDirectory, "clipboard_history.json");
        var manager = new ClipboardHistoryManager(historyFile);

        switch (command)
        {
            case "add":
                var text = args.Length > 1 ? string.Join(" ", args[1..]) : ReadStdin();
                if (!string.IsNullOrEmpty(text))
                {
                    manager.Add(text);
                    Console.WriteLine("✓ Added to clipboard history");
                }
                break;

            case "list":
            case "ls":
                var limit = args.Length > 1 && int.TryParse(args[1], out var n) ? n : 10;
                manager.List(limit);
                break;

            case "get":
                if (args.Length > 1 && int.TryParse(args[1], out var index))
                {
                    var item = manager.Get(index);
                    if (item != null)
                    {
                        Console.WriteLine(item);
                    }
                }
                else
                {
                    Console.Error.WriteLine("Usage: get <index>");
                }
                break;

            case "copy":
                if (args.Length > 1 && int.TryParse(args[1], out var copyIndex))
                {
                    var item = manager.Get(copyIndex);
                    if (item != null)
                    {
                        // On Linux, we'd need xclip/xsel; on Windows, use Clipboard.SetText
                        Console.WriteLine(item);
                        Console.WriteLine("↑ Copy this output to your clipboard");
                    }
                }
                else
                {
                    Console.Error.WriteLine("Usage: copy <index>");
                }
                break;

            case "clear":
                manager.Clear();
                Console.WriteLine("✓ Clipboard history cleared");
                break;

            case "export":
                var outputFile = args.Length > 1 ? args[1] : "clipboard_export.txt";
                manager.Export(outputFile);
                Console.WriteLine($"✓ Exported to {outputFile}");
                break;

            case "search":
                var query = args.Length > 1 ? string.Join(" ", args[1..]) : "";
                manager.Search(query);
                break;

            case "help":
            default:
                ShowHelp();
                break;
        }
    }

    static string ReadStdin()
    {
        if (Console.IsInputRedirected)
        {
            return Console.In.ReadToEnd().Trim();
        }
        return "";
    }

    static void ShowHelp()
    {
        Console.WriteLine("Clipboard Manager - Track and manage clipboard history");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project ClipboardManager.csproj -- <command> [args]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  add <text>     Add text to clipboard history");
        Console.WriteLine("  list [n]       List last n entries (default: 10)");
        Console.WriteLine("  get <index>    Get entry at index");
        Console.WriteLine("  copy <index>   Copy entry to stdout for piping");
        Console.WriteLine("  search <q>     Search history for query");
        Console.WriteLine("  clear          Clear all history");
        Console.WriteLine("  export <file>  Export history to file");
        Console.WriteLine("  help           Show this help");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  echo 'hello' | dotnet run --project ClipboardManager.csproj -- add");
        Console.WriteLine("  dotnet run --project ClipboardManager.csproj -- add \"important text\"");
        Console.WriteLine("  dotnet run --project ClipboardManager.csproj -- list 20");
        Console.WriteLine("  dotnet run --project ClipboardManager.csproj -- get 3");
        Console.WriteLine("  dotnet run --project ClipboardManager.csproj -- search \"api\"");
        Console.WriteLine("  dotnet run --project ClipboardManager.csproj -- export backup.txt");
    }
}

class ClipboardEntry
{
    public string Content { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public int Length => Content?.Length ?? 0;
}

class ClipboardHistoryManager
{
    private readonly string _historyFile;
    private List<ClipboardEntry> _history = new();

    public ClipboardHistoryManager(string historyFile)
    {
        _historyFile = historyFile;
        Load();
    }

    public void Add(string content)
    {
        _history.Insert(0, new ClipboardEntry
        {
            Content = content,
            Timestamp = DateTime.Now
        });

        // Keep last 100 entries
        if (_history.Count > 100)
        {
            _history = _history.Take(100).ToList();
        }

        Save();
    }

    public void List(int limit)
    {
        var count = Math.Min(limit, _history.Count);
        
        if (count == 0)
        {
            Console.WriteLine("No clipboard history yet.");
            return;
        }

        Console.WriteLine($"Clipboard History (showing {count} of {_history.Count} entries):");
        Console.WriteLine(new string('-', 60));

        for (int i = 0; i < count; i++)
        {
            var entry = _history[i];
            var preview = entry.Content.Length > 50 
                ? entry.Content[..50] + "..." 
                : entry.Content;
            
            preview = preview.Replace("\n", "\\n").Replace("\r", "\\r").Replace("\t", "    ");
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{i,3}] ");
            Console.ResetColor();
            Console.Write($"{entry.Timestamp:HH:mm:ss} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{entry.Length,5} chars | ");
            Console.ResetColor();
            Console.WriteLine(preview);
        }
    }

    public string? Get(int index)
    {
        if (index >= 0 && index < _history.Count)
        {
            return _history[index].Content;
        }
        Console.Error.WriteLine($"Index {index} out of range (0-{_history.Count - 1})");
        return null;
    }

    public void Clear()
    {
        _history.Clear();
        Save();
    }

    public void Export(string outputFile)
    {
        var content = string.Join("\n\n", _history.Select((e, i) => 
            $"--- Entry {i} ({e.Timestamp:yyyy-MM-dd HH:mm:ss}) ---\n{e.Content}"));
        File.WriteAllText(outputFile, content);
    }

    public void Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            Console.WriteLine("Please provide a search query.");
            return;
        }

        var results = _history
            .Select((e, i) => (Index: i, Entry: e))
            .Where(x => x.Entry.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
            .Take(20)
            .ToList();

        if (results.Count == 0)
        {
            Console.WriteLine($"No results found for \"{query}\"");
            return;
        }

        Console.WriteLine($"Search results for \"{query}\" ({results.Count} found):\n");
        foreach (var result in results)
        {
            var preview = result.Entry.Content.Length > 80
                ? result.Entry.Content[..80] + "..."
                : result.Entry.Content;
            preview = preview.Replace("\n", " ").Replace("\r", "");
            
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"[{result.Index,3}] ");
            Console.ResetColor();
            Console.WriteLine(preview);
        }
    }

    void Load()
    {
        if (File.Exists(_historyFile))
        {
            try
            {
                var json = File.ReadAllText(_historyFile);
                _history = JsonSerializer.Deserialize<List<ClipboardEntry>>(json) ?? new List<ClipboardEntry>();
            }
            catch (JsonException)
            {
                _history = new List<ClipboardEntry>();
            }
        }
    }

    void Save()
    {
        var options = new JsonSerializerOptions 
        { 
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };
        var json = JsonSerializer.Serialize(_history, options);
        File.WriteAllText(_historyFile, json);
    }
}
