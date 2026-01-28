using System.Text.Json;

var dataFile = "snippets.json";
var snippets = LoadSnippets(dataFile);

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "add":
        AddSnippet(snippets, args.Skip(1).ToArray());
        break;
    case "list":
        ListSnippets(snippets, args.Skip(1).ToArray());
        break;
    case "search":
        SearchSnippets(snippets, args.Skip(1).ToArray());
        break;
    case "delete":
        DeleteSnippet(snippets, args.Skip(1).ToArray());
        break;
    case "export":
        ExportSnippets(snippets, args.Skip(1).ToArray());
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

SaveSnippets(dataFile, snippets);

void ShowHelp()
{
    Console.WriteLine("""
        Code Snippet Manager - Store and retrieve code snippets with tags
        
        Usage:
          dotnet run --project CodeSnippetManager.csproj <command> [arguments]
        
        Commands:
          add <title> <language> [tags...]    Add a new snippet (reads code from stdin)
          list [tag]                          List all snippets or filter by tag
          search <query>                      Search snippets by title or code
          delete <id>                         Delete a snippet by ID
          export [file]                       Export snippets to JSON file
        
        Examples:
          echo "console.log('Hello')" | dotnet run -- add "Hello World" javascript js example
          dotnet run -- list js
          dotnet run -- search "hello"
          dotnet run -- delete 1
          dotnet run -- export backup.json
        """);
}

void AddSnippet(List<Snippet> snippets, string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: add <title> <language> [tags...]");
        return;
    }

    var title = args[0];
    var language = args[1];
    var tags = args.Skip(2).Select(t => t.ToLower()).ToList();
    
    Console.WriteLine("Paste your code (end with empty line or Ctrl+D):");
    var codeLines = new List<string>();
    string? line;
    while ((line = Console.ReadLine()) != null && line.Trim() != "")
    {
        codeLines.Add(line);
    }
    var code = string.Join("\n", codeLines);

    if (string.IsNullOrWhiteSpace(code))
    {
        Console.WriteLine("Error: Code cannot be empty");
        return;
    }

    var snippet = new Snippet
    {
        Id = snippets.Count > 0 ? snippets.Max(s => s.Id) + 1 : 1,
        Title = title,
        Language = language,
        Code = code,
        Tags = tags,
        CreatedAt = DateTime.Now
    };

    snippets.Add(snippet);
    Console.WriteLine($"✓ Snippet #{snippet.Id} added: \"{title}\" ({language})");
    if (tags.Count > 0)
        Console.WriteLine($"  Tags: {string.Join(", ", tags)}");
}

void ListSnippets(List<Snippet> snippets, string[] args)
{
    var filtered = snippets;
    
    if (args.Length > 0)
    {
        var tag = args[0].ToLower();
        filtered = snippets.Where(s => s.Tags.Contains(tag)).ToList();
    }

    if (filtered.Count == 0)
    {
        Console.WriteLine("No snippets found.");
        return;
    }

    Console.WriteLine($"Found {filtered.Count} snippet(s):\n");
    foreach (var snippet in filtered)
    {
        Console.WriteLine($"[{snippet.Id}] {snippet.Title} ({snippet.Language})");
        Console.WriteLine($"    Tags: {string.Join(", ", snippet.Tags)}");
        Console.WriteLine($"    Created: {snippet.CreatedAt:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"    Code: {snippet.Code.Split('\n')[0]}...");
        Console.WriteLine();
    }
}

void SearchSnippets(List<Snippet> snippets, string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: search <query>");
        return;
    }

    var query = args[0].ToLower();
    var matches = snippets.Where(s => 
        s.Title.ToLower().Contains(query) || 
        s.Code.ToLower().Contains(query) ||
        s.Tags.Any(t => t.Contains(query))
    ).ToList();

    if (matches.Count == 0)
    {
        Console.WriteLine("No snippets found matching your search.");
        return;
    }

    Console.WriteLine($"Found {matches.Count} snippet(s) matching \"{query}\":\n");
    foreach (var snippet in matches)
    {
        Console.WriteLine($"[{snippet.Id}] {snippet.Title} ({snippet.Language})");
        Console.WriteLine($"    Tags: {string.Join(", ", snippet.Tags)}");
        Console.WriteLine();
    }
}

void DeleteSnippet(List<Snippet> snippets, string[] args)
{
    if (args.Length == 0 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: delete <id>");
        return;
    }

    var snippet = snippets.FirstOrDefault(s => s.Id == id);
    if (snippet == null)
    {
        Console.WriteLine($"Snippet #{id} not found.");
        return;
    }

    snippets.Remove(snippet);
    Console.WriteLine($"✓ Snippet #{id} deleted: \"{snippet.Title}\"");
}

void ExportSnippets(List<Snippet> snippets, string[] args)
{
    var outputFile = args.Length > 0 ? args[0] : "snippets-export.json";
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(snippets, options);
    File.WriteAllText(outputFile, json);
    Console.WriteLine($"✓ Exported {snippets.Count} snippet(s) to {outputFile}");
}

List<Snippet> LoadSnippets(string path)
{
    if (!File.Exists(path))
        return new List<Snippet>();
    
    try
    {
        var json = File.ReadAllText(path);
        var snippets = JsonSerializer.Deserialize<List<Snippet>>(json);
        return snippets ?? new List<Snippet>();
    }
    catch (JsonException)
    {
        Console.WriteLine("Warning: Could not parse snippets.json, starting fresh.");
        return new List<Snippet>();
    }
}

void SaveSnippets(string path, List<Snippet> snippets)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(snippets, options);
    File.WriteAllText(path, json);
}

class Snippet
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Language { get; set; } = "";
    public string Code { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
