using System.Text.Json;

namespace QuickNote;

/// <summary>
/// Quick note-taking CLI tool for capturing and organizing notes.
/// Supports tags, search, and markdown export.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        var notesDir = Path.Combine(Environment.GetFolderPath(
            Environment.SpecialFolder.UserProfile), ".quick-notes");
        
        if (!Directory.Exists(notesDir))
        {
            Directory.CreateDirectory(notesDir);
        }

        var notesFile = Path.Combine(notesDir, "notes.json");
        var manager = new NoteManager(notesFile);

        var command = args.Length > 0 ? args[0].ToLower() : "help";

        switch (command)
        {
            case "add":
                var title = args.Length > 1 ? args[1] : "Untitled";
                var tags = args.Skip(2).Where(a => a.StartsWith("#")).Select(a => a.TrimStart('#')).ToList();
                var content = ReadContent(args.Skip(tags.Count + (args.Length > 1 ? 2 : 1)).ToArray());
                manager.Add(title, content, tags);
                Console.WriteLine($"✓ Note added: {title}");
                break;

            case "list":
            case "ls":
                manager.List();
                break;

            case "view":
            case "show":
                if (args.Length > 1 && int.TryParse(args[1], out var viewId))
                {
                    manager.View(viewId);
                }
                else
                {
                    Console.Error.WriteLine("Usage: view <id>");
                }
                break;

            case "delete":
            case "rm":
                if (args.Length > 1 && int.TryParse(args[1], out var deleteId))
                {
                    manager.Delete(deleteId);
                    Console.WriteLine($"✓ Note {deleteId} deleted");
                }
                else
                {
                    Console.Error.WriteLine("Usage: delete <id>");
                }
                break;

            case "search":
                var query = args.Length > 1 ? string.Join(" ", args[1..]) : "";
                manager.Search(query);
                break;

            case "tag":
                var tagFilter = args.Length > 1 ? args[1].TrimStart('#') : "";
                manager.FilterByTag(tagFilter);
                break;

            case "export":
                var exportId = args.Length > 1 && int.TryParse(args[1], out var id) ? id : -1;
                var format = args.Length > 2 ? args[2].ToLower() : "txt";
                if (exportId >= 0)
                {
                    manager.Export(exportId, format);
                }
                else
                {
                    Console.Error.WriteLine("Usage: export <id> [txt|md]");
                }
                break;

            case "stats":
                manager.Stats();
                break;

            case "help":
            default:
                ShowHelp();
                break;
        }
    }

    static string ReadContent(string[] remainingArgs)
    {
        if (remainingArgs.Length > 0)
        {
            return string.Join(" ", remainingArgs);
        }

        if (Console.IsInputRedirected)
        {
            return Console.In.ReadToEnd().Trim();
        }

        Console.WriteLine("Enter note content (empty line to finish):");
        var lines = new List<string>();
        string? line;
        while ((line = Console.ReadLine()) != null && line != "")
        {
            lines.Add(line);
        }
        return string.Join("\n", lines);
    }

    static void ShowHelp()
    {
        Console.WriteLine("Quick Note - Fast CLI note-taking tool");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project QuickNote.csproj -- <command> [args]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  add [title] [#tags...]  Add a new note");
        Console.WriteLine("  list                    List all notes");
        Console.WriteLine("  view <id>               View note by ID");
        Console.WriteLine("  delete <id>             Delete a note");
        Console.WriteLine("  search <query>          Search notes");
        Console.WriteLine("  tag <tag>               Filter by tag");
        Console.WriteLine("  export <id> [format]    Export note (txt|md)");
        Console.WriteLine("  stats                   Show statistics");
        Console.WriteLine("  help                    Show this help");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project QuickNote.csproj -- add \"Meeting Notes\" #work #meeting");
        Console.WriteLine("  echo \"Quick thought\" | dotnet run --project QuickNote.csproj -- add");
        Console.WriteLine("  dotnet run --project QuickNote.csproj -- list");
        Console.WriteLine("  dotnet run --project QuickNote.csproj -- search \"TODO\"");
        Console.WriteLine("  dotnet run --project QuickNote.csproj -- tag work");
        Console.WriteLine("  dotnet run --project QuickNote.csproj -- export 5 md");
    }
}

class Note
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Content { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

class NoteManager
{
    private readonly string _notesFile;
    private List<Note> _notes = new();
    private int _nextId = 1;

    public NoteManager(string notesFile)
    {
        _notesFile = notesFile;
        Load();
    }

    public void Add(string title, string content, List<string> tags)
    {
        var now = DateTime.Now;
        var note = new Note
        {
            Id = _nextId++,
            Title = title,
            Content = content,
            Tags = tags,
            CreatedAt = now,
            UpdatedAt = now
        };

        _notes.Add(note);
        Save();
    }

    public void List()
    {
        if (_notes.Count == 0)
        {
            Console.WriteLine("No notes yet. Add one with: add [title] [#tags]");
            return;
        }

        Console.WriteLine($"Notes ({_notes.Count} total):\n");
        
        foreach (var note in _notes.OrderByDescending(n => n.CreatedAt).Take(20))
        {
            var preview = note.Content.Length > 60 
                ? note.Content[..60] + "..." 
                : note.Content;
            preview = preview.Replace("\n", " ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"#{note.Id} ");
            Console.ResetColor();
            Console.Write($"{note.Title} ");
            
            if (note.Tags.Count > 0)
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"[{string.Join(" ", note.Tags.Select(t => $"#{t}"))}] ");
                Console.ResetColor();
            }
            
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write($"{note.CreatedAt:MMM dd} ");
            Console.ResetColor();
            Console.WriteLine(preview);
        }

        if (_notes.Count > 20)
        {
            Console.WriteLine($"\n... and {_notes.Count - 20} more notes");
        }
    }

    public void View(int id)
    {
        var note = _notes.FirstOrDefault(n => n.Id == id);
        if (note == null)
        {
            Console.Error.WriteLine($"Note {id} not found");
            return;
        }

        Console.WriteLine();
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"# {note.Title}");
        Console.ResetColor();
        
        if (note.Tags.Count > 0)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"Tags: {string.Join(" ", note.Tags.Select(t => $"#{t}"))}");
            Console.ResetColor();
        }
        
        Console.WriteLine($"Created: {note.CreatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Updated: {note.UpdatedAt:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine(note.Content);
        Console.WriteLine();
    }

    public void Delete(int id)
    {
        var note = _notes.FirstOrDefault(n => n.Id == id);
        if (note == null)
        {
            Console.Error.WriteLine($"Note {id} not found");
            return;
        }

        _notes.Remove(note);
        Save();
    }

    public void Search(string query)
    {
        if (string.IsNullOrEmpty(query))
        {
            Console.WriteLine("Please provide a search query.");
            return;
        }

        var results = _notes
            .Where(n => n.Title.Contains(query, StringComparison.OrdinalIgnoreCase) ||
                       n.Content.Contains(query, StringComparison.OrdinalIgnoreCase))
            .ToList();

        DisplaySearchResults(results, query);
    }

    public void FilterByTag(string tag)
    {
        if (string.IsNullOrEmpty(tag))
        {
            Console.WriteLine("Please provide a tag.");
            return;
        }

        var results = _notes
            .Where(n => n.Tags.Any(t => t.Contains(tag, StringComparison.OrdinalIgnoreCase)))
            .ToList();

        DisplaySearchResults(results, $"#{tag}");
    }

    void DisplaySearchResults(List<Note> results, string query)
    {
        if (results.Count == 0)
        {
            Console.WriteLine($"No results found for \"{query}\"");
            return;
        }

        Console.WriteLine($"Found {results.Count} note(s) for \"{query}\":\n");
        
        foreach (var note in results)
        {
            var preview = note.Content.Length > 80
                ? note.Content[..80] + "..."
                : note.Content;
            preview = preview.Replace("\n", " ");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write($"#{note.Id} ");
            Console.ResetColor();
            Console.Write($"{note.Title} ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(preview);
        }
    }

    public void Export(int id, string format)
    {
        var note = _notes.FirstOrDefault(n => n.Id == id);
        if (note == null)
        {
            Console.Error.WriteLine($"Note {id} not found");
            return;
        }

        var outputFile = Path.Combine(Path.GetDirectoryName(_notesFile)!, 
            $"{note.Title.Replace(" ", "_").Replace("/", "-")}.{format}");

        var content = format.ToLower() switch
        {
            "md" => $"# {note.Title}\n\n{string.Join(" ", note.Tags.Select(t => $"#{t}"))}\n\n{note.Content}\n",
            _ => $"{note.Title}\n{new string('=', note.Title.Length)}\n\n{note.Content}\n"
        };

        File.WriteAllText(outputFile, content);
        Console.WriteLine($"✓ Exported to {outputFile}");
    }

    public void Stats()
    {
        var totalNotes = _notes.Count;
        var totalWords = _notes.Sum(n => n.Content.Split(' ', StringSplitOptions.RemoveEmptyEntries).Length);
        var totalChars = _notes.Sum(n => n.Content.Length);
        var allTags = _notes.SelectMany(n => n.Tags).GroupBy(t => t).OrderByDescending(g => g.Count()).Take(10);
        var oldest = _notes.MinBy(n => n.CreatedAt)?.CreatedAt;
        var newest = _notes.MaxBy(n => n.CreatedAt)?.CreatedAt;

        Console.WriteLine("Note Statistics:");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Total notes:     {totalNotes}");
        Console.WriteLine($"Total words:     {totalWords:N0}");
        Console.WriteLine($"Total chars:     {totalChars:N0}");
        Console.WriteLine($"Avg note size:   {(totalNotes > 0 ? totalWords / totalNotes : 0)} words");
        Console.WriteLine($"First note:      {(oldest?.ToString("yyyy-MM-dd") ?? "N/A")}");
        Console.WriteLine($"Latest note:     {(newest?.ToString("yyyy-MM-dd") ?? "N/A")}");
        
        if (allTags.Any())
        {
            Console.WriteLine($"\nTop tags:");
            foreach (var tag in allTags)
            {
                Console.WriteLine($"  #{tag.Key}: {tag.Count()}");
            }
        }
    }

    void Load()
    {
        if (File.Exists(_notesFile))
        {
            try
            {
                var json = File.ReadAllText(_notesFile);
                _notes = JsonSerializer.Deserialize<List<Note>>(json) ?? new List<Note>();
                _nextId = _notes.Max(n => n.Id) + 1;
            }
            catch (JsonException)
            {
                _notes = new List<Note>();
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
        var json = JsonSerializer.Serialize(_notes, options);
        File.WriteAllText(_notesFile, json);
    }
}
