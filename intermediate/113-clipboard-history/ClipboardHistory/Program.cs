using System.Text.Json;

var dataFile = "clipboard.json";
var maxHistory = 50;
var history = LoadHistory(dataFile);

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "copy":
        CopyToClipboard(args.Skip(1).ToArray());
        break;
    case "paste":
        PasteFromClipboard();
        break;
    case "history":
        ShowHistory(args.Skip(1).ToArray());
        break;
    case "get":
        GetHistoryItem(args.Skip(1).ToArray());
        break;
    case "search":
        SearchHistory(args.Skip(1).ToArray());
        break;
    case "delete":
        DeleteHistoryItem(args.Skip(1).ToArray());
        break;
    case "clear":
        ClearHistory();
        break;
    case "snippets":
        ManageSnippets(args);
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

void CopyToClipboard(string[] args)
{
    string content;

    if (args.Length > 0)
    {
        content = string.Join(" ", args);
    }
    else
    {
        Console.WriteLine("Reading from stdin (Ctrl+D to end):");
        content = Console.In.ReadToEnd().Trim();
    }

    if (string.IsNullOrWhiteSpace(content))
    {
        Console.WriteLine("No content to copy");
        return;
    }

    try
    {
        // Simulate clipboard by storing in history and outputting
        var entry = new ClipboardEntry
        {
            Id = history.Count > 0 ? history.Max(h => h.Id) + 1 : 1,
            Content = content,
            CopiedAt = DateTime.Now,
            Type = content.Length > 100 ? "text" : DetectType(content)
        };

        history.Insert(0, entry);
        if (history.Count > maxHistory)
            history.RemoveAt(history.Count - 1);

        SaveHistory(dataFile);
        Console.WriteLine($"✓ Copied {content.Length} characters (ID: {entry.Id})");
        Console.WriteLine($"  Type: {entry.Type}");

        // Also output to stdout for piping
        Console.WriteLine();
        Console.WriteLine("Content (for piping):");
        Console.WriteLine(content);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void PasteFromClipboard()
{
    if (history.Count == 0)
    {
        Console.WriteLine("Clipboard is empty");
        return;
    }

    var latest = history[0];
    Console.WriteLine(latest.Content);
}

void ShowHistory(string[] args)
{
    var limit = 10;
    if (args.Length > 0 && int.TryParse(args[0], out var l))
        limit = l;

    var items = history.Take(limit).ToList();

    if (items.Count == 0)
    {
        Console.WriteLine("Clipboard history is empty");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"{"ID",-6} {"Type",-10} {"Age",-12} Preview");
    Console.WriteLine(new string('-', 70));

    foreach (var item in items)
    {
        var age = FormatTimeAgo(item.CopiedAt);
        var preview = item.Content.Length > 40
            ? item.Content[..37] + "..."
            : item.Content;
        preview = preview.Replace("\n", "\\n").Replace("\r", "\\r");

        Console.WriteLine($"{item.Id,-6} {item.Type,-10} {age,-12} {preview}");
    }
    Console.WriteLine($"\nTotal: {history.Count} item(s) in history");
}

void GetHistoryItem(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- get <id>");
        return;
    }

    if (int.TryParse(args[0], out var id))
    {
        var item = history.FirstOrDefault(h => h.Id == id);
        if (item != null)
        {
            Console.WriteLine(item.Content);
        }
        else
        {
            Console.WriteLine($"Item #{id} not found");
        }
    }
}

void SearchHistory(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- search <query>");
        return;
    }

    var query = string.Join(" ", args).ToLower();
    var results = history.Where(h => h.Content.ToLower().Contains(query)).ToList();

    if (results.Count == 0)
    {
        Console.WriteLine("No matching items found");
        return;
    }

    Console.WriteLine($"Found {results.Count} matching item(s):\n");

    foreach (var item in results)
    {
        var preview = item.Content.Length > 60
            ? item.Content[..57] + "..."
            : item.Content;
        preview = preview.Replace("\n", " ");

        Console.WriteLine($"[{item.Id}] {FormatTimeAgo(item.CopiedAt)} - {item.Type}");
        Console.WriteLine($"    {preview}");
        Console.WriteLine();
    }
}

void DeleteHistoryItem(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- delete <id>");
        return;
    }

    if (int.TryParse(args[0], out var id))
    {
        var item = history.FirstOrDefault(h => h.Id == id);
        if (item != null)
        {
            history.Remove(item);
            SaveHistory(dataFile);
            Console.WriteLine($"✓ Deleted item #{id}");
        }
        else
        {
            Console.WriteLine($"Item #{id} not found");
        }
    }
}

void ClearHistory()
{
    history.Clear();
    SaveHistory(dataFile);
    Console.WriteLine("✓ Clipboard history cleared");
}

void ManageSnippets(string[] args)
{
    var snippetsFile = "snippets.json";
    var snippets = LoadSnippets(snippetsFile);

    if (args.Length < 2)
    {
        Console.WriteLine("Snippet commands:");
        Console.WriteLine("  snippets save <name> <id>  - Save history item as snippet");
        Console.WriteLine("  snippets list              - List all snippets");
        Console.WriteLine("  snippets get <name>        - Get snippet content");
        Console.WriteLine("  snippets delete <name>     - Delete a snippet");
        return;
    }

    var subCommand = args[1].ToLower();

    switch (subCommand)
    {
        case "save":
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: snippets save <name> <history-id>");
                return;
            }
            var name = args[2];
            if (int.TryParse(args[3], out var histId))
            {
                var item = history.FirstOrDefault(h => h.Id == histId);
                if (item != null)
                {
                    snippets[name] = item.Content;
                    SaveSnippets(snippetsFile, snippets);
                    Console.WriteLine($"✓ Saved snippet '{name}'");
                }
                else
                {
                    Console.WriteLine($"History item #{histId} not found");
                }
            }
            break;

        case "list":
            if (snippets.Count == 0)
            {
                Console.WriteLine("No snippets saved");
            }
            else
            {
                Console.WriteLine("\nSaved snippets:");
                foreach (var kvp in snippets)
                {
                    var preview = kvp.Value.Length > 40
                        ? kvp.Value[..37] + "..."
                        : kvp.Value;
                    Console.WriteLine($"  {kvp.Key,-20} {preview}");
                }
            }
            break;

        case "get":
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: snippets get <name>");
                return;
            }
            var snippetName = args[3];
            if (snippets.TryGetValue(snippetName, out var content))
            {
                Console.WriteLine(content);
            }
            else
            {
                Console.WriteLine($"Snippet '{snippetName}' not found");
            }
            break;

        case "delete":
            if (args.Length < 4)
            {
                Console.WriteLine("Usage: snippets delete <name>");
                return;
            }
            var delName = args[3];
            if (snippets.Remove(delName))
            {
                SaveSnippets(snippetsFile, snippets);
                Console.WriteLine($"✓ Deleted snippet '{delName}'");
            }
            else
            {
                Console.WriteLine($"Snippet '{delName}' not found");
            }
            break;

        default:
            Console.WriteLine($"Unknown snippet command: {subCommand}");
            break;
    }
}

void ShowHelp()
{
    Console.WriteLine("""
        ClipboardHistory - Enhanced clipboard with history and snippets

        Usage:
          dotnet run -- <command> [arguments]

        Commands:
          copy [text]              Copy text to clipboard (or read from stdin)
          paste                    Output the latest clipboard item
          history [limit]          Show clipboard history (default: 10 items)
          get <id>                 Get specific history item by ID
          search <query>           Search clipboard history
          delete <id>              Delete a history item
          clear                    Clear all clipboard history
          snippets <subcommand>    Manage text snippets

        Snippet Subcommands:
          snippets save <name> <id>   Save history item as named snippet
          snippets list               List all snippets
          snippets get <name>         Get snippet content
          snippets delete <name>      Delete a snippet

        Examples:
          dotnet run -- copy "Hello, World!"
          dotnet run -- paste
          dotnet run -- history 20
          dotnet run -- get 5
          dotnet run -- search "https://"
          dotnet run -- snippets save greeting 1
          dotnet run -- snippets get greeting
          echo "test" | dotnet run -- copy
        """);
}

List<ClipboardEntry> LoadHistory(string path)
{
    if (!File.Exists(path))
        return new List<ClipboardEntry>();

    try
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<ClipboardEntry>>(json) ?? new List<ClipboardEntry>();
    }
    catch
    {
        return new List<ClipboardEntry>();
    }
}

void SaveHistory(string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(history, options);
    File.WriteAllText(path, json);
}

Dictionary<string, string> LoadSnippets(string path)
{
    if (!File.Exists(path))
        return new Dictionary<string, string>();

    try
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<Dictionary<string, string>>(json) ?? new Dictionary<string, string>();
    }
    catch
    {
        return new Dictionary<string, string>();
    }
}

void SaveSnippets(string path, Dictionary<string, string> snippets)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(snippets, options);
    File.WriteAllText(path, json);
}

static string DetectType(string content)
{
    if (content.StartsWith("http://") || content.StartsWith("https://"))
        return "url";
    if (content.Contains("@") && content.Contains("."))
        return "email";
    if (content.Contains("\n"))
        return "multiline";
    if (content.Length > 50)
        return "text";
    return "short";
}

static string FormatTimeAgo(DateTime dt)
{
    var span = DateTime.Now - dt;

    if (span.TotalSeconds < 60)
        return $"{(int)span.TotalSeconds}s ago";
    if (span.TotalMinutes < 60)
        return $"{(int)span.TotalMinutes}m ago";
    if (span.TotalHours < 24)
        return $"{(int)span.TotalHours}h ago";
    if (span.TotalDays < 7)
        return $"{(int)span.TotalDays}d ago";

    return dt.ToString("yyyy-MM-dd");
}

class ClipboardEntry
{
    public int Id { get; set; }
    public required string Content { get; set; }
    public DateTime CopiedAt { get; set; }
    public required string Type { get; set; }
}
