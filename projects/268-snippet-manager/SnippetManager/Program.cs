using System.Text.Json;

namespace SnippetManager;

class Program
{
    private const string DataFile = "snippets.json";
    private static List<CodeSnippet> _snippets = new();

    static void Main(string[] args)
    {
        Console.WriteLine("📋 Code Snippet Manager");
        Console.WriteLine("=======================\n");

        LoadSnippets();

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();
        switch (command)
        {
            case "add":
                AddSnippet(args.Skip(1).ToArray());
                break;
            case "list":
                ListSnippets();
                break;
            case "search":
                SearchSnippets(args.Skip(1).ToArray());
                break;
            case "get":
                GetSnippet(args.ElementAtOrDefault(1));
                break;
            case "delete":
                DeleteSnippet(args.ElementAtOrDefault(1));
                break;
            case "export":
                ExportSnippets();
                break;
            default:
                ShowMenu();
                break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  add              - Add a new snippet");
        Console.WriteLine("  list             - List all snippets");
        Console.WriteLine("  search <query>   - Search snippets");
        Console.WriteLine("  get <id>         - Get snippet by ID");
        Console.WriteLine("  delete <id>      - Delete a snippet");
        Console.WriteLine("  export           - Export to markdown");
        Console.WriteLine();

        Console.Write("Enter command: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        var parts = input.Trim().Split(' ', 2);
        string cmd = parts[0].ToLower();
        string? arg = parts.Length > 1 ? parts[1] : null;

        switch (cmd)
        {
            case "add": AddSnippetInteractive(); break;
            case "list": ListSnippets(); break;
            case "search": SearchSnippets(arg); break;
            case "get": GetSnippet(arg); break;
            case "delete": DeleteSnippet(arg); break;
            case "export": ExportSnippets(); break;
        }
    }

    private static void LoadSnippets()
    {
        if (File.Exists(DataFile))
        {
            string json = File.ReadAllText(DataFile);
            _snippets = JsonSerializer.Deserialize<List<CodeSnippet>>(json) ?? new List<CodeSnippet>();
        }
    }

    private static void SaveSnippets()
    {
        string json = JsonSerializer.Serialize(_snippets, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFile, json);
    }

    private static void AddSnippet(string[] args)
    {
        Console.Write("Title: ");
        string? title = Console.ReadLine();

        Console.Write("Language (csharp, python, js, etc.): ");
        string? language = Console.ReadLine();

        Console.Write("Tags (comma-separated): ");
        string? tagsInput = Console.ReadLine();

        Console.WriteLine("Enter code (end with empty line):");
        var codeLines = new List<string>();
        string? line;
        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            codeLines.Add(line);
        }

        if (string.IsNullOrWhiteSpace(title))
        {
            Console.WriteLine("Title is required!");
            return;
        }

        var snippet = new CodeSnippet
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Title = title!,
            Language = language ?? "text",
            Tags = tagsInput?.Split(',').Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList() ?? new List<string>(),
            Code = string.Join("\n", codeLines),
            CreatedAt = DateTime.Now
        };

        _snippets.Add(snippet);
        SaveSnippets();

        Console.WriteLine($"\n✅ Snippet added with ID: {snippet.Id}");
    }

    private static void AddSnippetInteractive()
    {
        AddSnippet(Array.Empty<string>());
    }

    private static void ListSnippets()
    {
        if (_snippets.Count == 0)
        {
            Console.WriteLine("No snippets found.");
            return;
        }

        Console.WriteLine($"{"ID",-10} {"Title",-30} {"Language",-12} {"Tags",-20}");
        Console.WriteLine(new string('-', 75));

        foreach (var snippet in _snippets.OrderByDescending(s => s.CreatedAt))
        {
            string tags = string.Join(",", snippet.Tags.Take(3));
            Console.WriteLine($"{snippet.Id,-10} {Truncate(snippet.Title, 30),-30} {snippet.Language,-12} {tags,-20}");
        }

        Console.WriteLine($"\nTotal: {_snippets.Count} snippets");
    }

    private static void SearchSnippets(string? query)
    {
        if (string.IsNullOrWhiteSpace(query))
        {
            Console.Write("Search query: ");
            query = Console.ReadLine();
        }

        if (string.IsNullOrWhiteSpace(query))
        {
            Console.WriteLine("No search query provided.");
            return;
        }

        var results = _snippets.Where(s =>
            s.Title.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            s.Code.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            s.Tags.Any(t => t.Contains(query!, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        if (results.Count == 0)
        {
            Console.WriteLine($"No snippets found matching '{query}'.");
            return;
        }

        Console.WriteLine($"\nFound {results.Count} snippet(s):\n");
        foreach (var snippet in results)
        {
            Console.WriteLine($"[{snippet.Id}] {snippet.Title}");
            Console.WriteLine($"  Language: {snippet.Language} | Tags: {string.Join(", ", snippet.Tags)}");
            Console.WriteLine();
        }
    }

    private static void GetSnippet(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Snippet ID: ");
            id = Console.ReadLine();
        }

        var snippet = _snippets.FirstOrDefault(s => s.Id == id);
        if (snippet == null)
        {
            Console.WriteLine($"Snippet '{id}' not found.");
            return;
        }

        Console.WriteLine($"\n=== {snippet.Title} ===\n");
        Console.WriteLine($"ID: {snippet.Id}");
        Console.WriteLine($"Language: {snippet.Language}");
        Console.WriteLine($"Tags: {string.Join(", ", snippet.Tags)}");
        Console.WriteLine($"Created: {snippet.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine();
        Console.WriteLine("```" + snippet.Language);
        Console.WriteLine(snippet.Code);
        Console.WriteLine("```");
    }

    private static void DeleteSnippet(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Snippet ID to delete: ");
            id = Console.ReadLine();
        }

        var snippet = _snippets.FirstOrDefault(s => s.Id == id);
        if (snippet == null)
        {
            Console.WriteLine($"Snippet '{id}' not found.");
            return;
        }

        Console.Write($"Are you sure you want to delete '{snippet.Title}'? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _snippets.Remove(snippet);
            SaveSnippets();
            Console.WriteLine("✅ Snippet deleted.");
        }
    }

    private static void ExportSnippets()
    {
        string outputFile = $"snippets_{DateTime.Now:yyyyMMdd_HHmmss}.md";
        var md = new System.Text.StringBuilder();

        md.AppendLine("# Code Snippets\n");
        md.AppendLine($"Exported: {DateTime.Now:yyyy-MM-dd HH:mm:ss}\n");
        md.AppendLine($"Total: {_snippets.Count} snippets\n");
        md.AppendLine("---\n");

        foreach (var snippet in _snippets.OrderByDescending(s => s.CreatedAt))
        {
            md.AppendLine($"## {snippet.Title}\n");
            md.AppendLine($"**ID:** `{snippet.Id}`  ");
            md.AppendLine($"**Language:** {snippet.Language}  ");
            md.AppendLine($"**Tags:** {string.Join(", ", snippet.Tags)}  ");
            md.AppendLine($"**Created:** {snippet.CreatedAt:yyyy-MM-dd HH:mm:ss}\n");
            md.AppendLine($"```{snippet.Language}\n{snippet.Code}\n```\n");
            md.AppendLine("---\n");
        }

        File.WriteAllText(outputFile, md.ToString());
        Console.WriteLine($"✅ Exported to: {outputFile}");
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}

public class CodeSnippet
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Language { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public string Code { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}
