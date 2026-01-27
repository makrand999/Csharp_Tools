using System.Text;
using TextCopy;

const string HistoryFile = "clipboard_history.json";

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();
var clipboard = new Clipboard();

switch (command)
{
    case "copy":
    case "c":
        await CopyAsync(clipboard, args.Skip(1).ToArray());
        break;
    case "paste":
    case "p":
        await PasteAsync(clipboard);
        break;
    case "clear":
        Clear();
        break;
    case "history":
    case "h":
        ShowHistory();
        break;
    case "save":
        await SaveClipboardAsync(clipboard);
        break;
    case "load":
        await LoadClipboardAsync(clipboard);
        break;
    case "count":
        await CountCharactersAsync(clipboard);
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

static void ShowHelp()
{
    Console.WriteLine("Clipboard Manager - CLI clipboard operations");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project ClipboardManager.csproj <command> [options]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  copy, c <text>     Copy text to clipboard");
    Console.WriteLine("  paste, p           Paste clipboard content to stdout");
    Console.WriteLine("  clear              Clear clipboard history file");
    Console.WriteLine("  history, h         Show clipboard history");
    Console.WriteLine("  save               Save current clipboard to history");
    Console.WriteLine("  load               Load last item from history to clipboard");
    Console.WriteLine("  count              Count characters/words in clipboard");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project ClipboardManager.csproj copy \"Hello World\"");
    Console.WriteLine("  dotnet run --project ClipboardManager.csproj paste");
    Console.WriteLine("  dotnet run --project ClipboardManager.csproj history");
}

static async Task CopyAsync(Clipboard clipboard, string[] args)
{
    string text;
    
    if (args.Length > 0)
    {
        text = string.Join(" ", args);
    }
    else
    {
        Console.WriteLine("Enter text to copy (Ctrl+D to finish):");
        var lines = new List<string>();
        string? line;
        while ((line = Console.ReadLine()) != null)
        {
            lines.Add(line);
        }
        text = string.Join("\n", lines);
    }
    
    if (!string.IsNullOrWhiteSpace(text))
    {
        await clipboard.SetTextAsync(text);
        Console.WriteLine($"✓ Copied {text.Length} characters to clipboard");
        SaveToHistory(text);
    }
    else
    {
        Console.WriteLine("No text provided.");
    }
}

static async Task PasteAsync(Clipboard clipboard)
{
    try
    {
        var text = await clipboard.GetTextAsync();
        if (text != null)
        {
            Console.WriteLine(text);
        }
        else
        {
            Console.WriteLine("Clipboard is empty or contains non-text data.");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error pasting: {ex.Message}");
        Console.Error.WriteLine("Note: Clipboard requires a GUI environment on some systems.");
    }
}

static void Clear()
{
    if (File.Exists(HistoryFile))
    {
        File.Delete(HistoryFile);
        Console.WriteLine("✓ Clipboard history cleared");
    }
    else
    {
        Console.WriteLine("No history file found.");
    }
}

static void ShowHistory()
{
    if (!File.Exists(HistoryFile))
    {
        Console.WriteLine("No clipboard history available.");
        return;
    }
    
    var history = LoadHistory();
    if (history.Count == 0)
    {
        Console.WriteLine("Clipboard history is empty.");
        return;
    }
    
    Console.WriteLine($"Clipboard History ({history.Count} items):\n");
    for (int i = 0; i < history.Count; i++)
    {
        var item = history[i];
        var preview = item.Length > 60 ? item[..60] + "..." : item;
        preview = preview.Replace("\n", "\\n").Replace("\r", "\\r");
        Console.WriteLine($"[{i + 1}] {preview}");
    }
}

static async Task SaveClipboardAsync(Clipboard clipboard)
{
    try
    {
        var text = await clipboard.GetTextAsync();
        if (!string.IsNullOrWhiteSpace(text))
        {
            SaveToHistory(text);
            Console.WriteLine("✓ Clipboard saved to history");
        }
        else
        {
            Console.WriteLine("Clipboard is empty.");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
    }
}

static async Task LoadClipboardAsync(Clipboard clipboard)
{
    var history = LoadHistory();
    if (history.Count > 0)
    {
        var lastItem = history.Last();
        await clipboard.SetTextAsync(lastItem);
        Console.WriteLine($"✓ Loaded from history ({lastItem.Length} characters)");
    }
    else
    {
        Console.WriteLine("No history to load.");
    }
}

static async Task CountCharactersAsync(Clipboard clipboard)
{
    try
    {
        var text = await clipboard.GetTextAsync();
        if (!string.IsNullOrWhiteSpace(text))
        {
            var charCount = text.Length;
            var wordCount = text.Split(new[] { ' ', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries).Length;
            var lineCount = text.Split('\n').Length;
            
            Console.WriteLine("Clipboard Statistics:");
            Console.WriteLine($"  Characters: {charCount}");
            Console.WriteLine($"  Words: {wordCount}");
            Console.WriteLine($"  Lines: {lineCount}");
        }
        else
        {
            Console.WriteLine("Clipboard is empty or contains non-text data.");
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error: {ex.Message}");
    }
}

static List<string> LoadHistory()
{
    try
    {
        var json = File.ReadAllText(HistoryFile);
        return System.Text.Json.JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
    }
    catch
    {
        return new List<string>();
    }
}

static void SaveToHistory(string text)
{
    var history = LoadHistory();
    history.Add(text);
    
    // Keep only last 50 items
    if (history.Count > 50)
    {
        history = history.Skip(history.Count - 50).ToList();
    }
    
    var json = System.Text.Json.JsonSerializer.Serialize(history, new System.Text.Json.JsonSerializerOptions
    {
        WriteIndented = true
    });
    File.WriteAllText(HistoryFile, json);
}
