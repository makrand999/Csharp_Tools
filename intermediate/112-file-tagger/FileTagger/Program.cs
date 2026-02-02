using System.Text.Json;

var dataFile = "filetags.json";
var tags = LoadTags(dataFile);

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "add":
        AddTags(args.Skip(1).ToArray());
        break;
    case "remove":
        RemoveTags(args.Skip(1).ToArray());
        break;
    case "list":
        ListTags(args.Skip(1).ToArray());
        break;
    case "search":
        SearchTags(args.Skip(1).ToArray());
        break;
    case "stats":
        ShowStats();
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

void AddTags(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- add <file-path> <tag1> [tag2] [tag3]...");
        return;
    }

    var filePath = Path.GetFullPath(args[0]);
    var newTags = args.Skip(1).Select(t => t.ToLower().Trim()).Distinct().ToList();

    if (!File.Exists(filePath))
    {
        Console.WriteLine($"File not found: {filePath}");
        return;
    }

    var fileEntry = tags.FirstOrDefault(t => t.FilePath == filePath);
    if (fileEntry == null)
    {
        fileEntry = new FileTagEntry { FilePath = filePath, Tags = [] };
        tags.Add(fileEntry);
    }

    var added = new List<string>();
    foreach (var tag in newTags)
    {
        if (!fileEntry.Tags.Contains(tag))
        {
            fileEntry.Tags.Add(tag);
            added.Add(tag);
        }
    }

    SaveTags(dataFile);
    Console.WriteLine($"✓ Added {added.Count} tag(s) to {Path.GetFileName(filePath)}");
    if (added.Count > 0)
        Console.WriteLine($"  Tags: {string.Join(", ", added)}");
}

void RemoveTags(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- remove <file-path> <tag1> [tag2]...");
        return;
    }

    var filePath = Path.GetFullPath(args[0]);
    var removeTags = args.Skip(1).Select(t => t.ToLower().Trim()).ToList();

    var fileEntry = tags.FirstOrDefault(t => t.FilePath == filePath);
    if (fileEntry == null)
    {
        Console.WriteLine("No tags found for this file");
        return;
    }

    var removed = fileEntry.Tags.Where(t => removeTags.Contains(t)).ToList();
    fileEntry.Tags.RemoveAll(t => removeTags.Contains(t));

    if (fileEntry.Tags.Count == 0)
        tags.Remove(fileEntry);

    SaveTags(dataFile);
    Console.WriteLine($"✓ Removed {removed.Count} tag(s) from {Path.GetFileName(filePath)}");
    if (removed.Count > 0)
        Console.WriteLine($"  Removed: {string.Join(", ", removed)}");
}

void ListTags(string[] args)
{
    var path = args.Length > 0 ? Path.GetFullPath(args[0]) : null;

    IEnumerable<FileTagEntry> query = tags;

    if (path != null)
        query = query.Where(t => t.FilePath.StartsWith(path));

    var result = query.OrderBy(t => t.FilePath).ToList();

    if (result.Count == 0)
    {
        Console.WriteLine("No tagged files found.");
        return;
    }

    Console.WriteLine();
    Console.WriteLine($"File{"", -50} Tags");
    Console.WriteLine(new string('-', 80));

    foreach (var entry in result)
    {
        var fileName = entry.FilePath;
        if (fileName.Length > 50)
            fileName = "..." + fileName[^47..];

        var tagStr = string.Join(", ", entry.Tags.Select(t => $"#{t}"));
        Console.WriteLine($"{fileName,-50} {tagStr}");
    }
    Console.WriteLine($"\nTotal: {result.Count} file(s)");
}

void SearchTags(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- search <tag1> [tag2]...");
        Console.WriteLine("Options: --all (match all tags) or --any (match any tag, default)");
        return;
    }

    var matchAll = args.Contains("--all");
    var searchTags = args.Where(t => t != "--all").Select(t => t.ToLower().Trim()).ToList();

    var results = tags.Where(entry =>
        matchAll
            ? searchTags.All(t => entry.Tags.Contains(t))
            : searchTags.Any(t => entry.Tags.Contains(t))
    ).ToList();

    if (results.Count == 0)
    {
        Console.WriteLine("No matching files found.");
        return;
    }

    Console.WriteLine($"\nFound {results.Count} file(s) matching tags: {string.Join(", ", searchTags.Select(t => $"#{t}"))}");
    Console.WriteLine(new string('-', 80));

    foreach (var entry in results)
    {
        Console.WriteLine($"{entry.FilePath}");
        Console.WriteLine($"  Tags: {string.Join(", ", entry.Tags.Select(t => $"#{t}"))}");
    }
}

void ShowStats()
{
    if (tags.Count == 0)
    {
        Console.WriteLine("No tags recorded yet.");
        return;
    }

    var allTags = tags.SelectMany(t => t.Tags).ToList();
    var tagCounts = allTags.GroupBy(t => t).OrderByDescending(g => g.Count()).ToList();
    var filesWithMultipleTags = tags.Count(t => t.Tags.Count > 1);

    Console.WriteLine();
    Console.WriteLine("=== File Tagger Statistics ===");
    Console.WriteLine($"Total tagged files: {tags.Count}");
    Console.WriteLine($"Files with multiple tags: {filesWithMultipleTags}");
    Console.WriteLine($"Total unique tags: {tagCounts.Count}");
    Console.WriteLine($"Total tag assignments: {allTags.Count}");
    Console.WriteLine();
    Console.WriteLine("Top tags:");

    foreach (var tag in tagCounts.Take(10))
    {
        Console.WriteLine($"  #{tag.Key,-20} {tag.Count()} file(s)");
    }
}

void ShowHelp()
{
    Console.WriteLine("""
        FileTagger - Add metadata tags to files (stored in sidecar JSON)

        Usage:
          dotnet run -- <command> [arguments]

        Commands:
          add <file> <tag1> [tag2]...   Add tags to a file
          remove <file> <tag1> [tag2]... Remove tags from a file
          list [path]                   List tagged files (optionally filter by path)
          search <tag1> [tag2]...       Search files by tags
          stats                         Show tag statistics

        Search Options:
          --all    Match all specified tags (AND)
          --any    Match any tag (OR, default)

        Examples:
          dotnet run -- add document.pdf work important
          dotnet run -- add photo.jpg vacation 2025 summer
          dotnet run -- remove document.pdf work
          dotnet run -- list
          dotnet run -- search work --all
          dotnet run -- search vacation summer
          dotnet run -- stats
        """);
}

static List<FileTagEntry> LoadTags(string path)
{
    if (!File.Exists(path))
        return [];

    try
    {
        var json = File.ReadAllText(path);
        return JsonSerializer.Deserialize<List<FileTagEntry>>(json) ?? [];
    }
    catch
    {
        return new List<FileTagEntry>();
    }
}

void SaveTags(string path)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(tags, options);
    File.WriteAllText(path, json);
}

class FileTagEntry
{
    public required string FilePath { get; set; }
    public required List<string> Tags { get; set; }
}
