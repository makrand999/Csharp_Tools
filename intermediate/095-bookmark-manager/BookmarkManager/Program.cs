using System.Text.Json;
using System.Text.RegularExpressions;

const string DataFile = "bookmarks.json";

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "add":
    case "a":
        await AddBookmarkAsync(args.Skip(1).ToArray());
        break;
    case "list":
    case "l":
        ListBookmarks(args.Skip(1).ToArray());
        break;
    case "remove":
    case "rm":
    case "delete":
    case "d":
        RemoveBookmark(args.Skip(1).ToArray());
        break;
    case "search":
    case "s":
        SearchBookmarks(args.Skip(1).ToArray());
        break;
    case "tags":
    case "t":
        ShowTags();
        break;
    case "export":
    case "e":
        ExportBookmarks(args.Skip(1).ToArray());
        break;
    case "import":
    case "i":
        await ImportBookmarksAsync(args.Skip(1).ToArray());
        break;
    case "open":
    case "o":
        OpenBookmark(args.Skip(1).ToArray());
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

static void ShowHelp()
{
    Console.WriteLine("Bookmark Manager CLI - Save and manage your bookmarks");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project BookmarkManager.csproj <command> [options]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  add <url> [title] [--tags tag1,tag2]  Add a new bookmark");
    Console.WriteLine("  list [tag]                            List all bookmarks (optionally filter by tag)");
    Console.WriteLine("  remove <id>                           Remove a bookmark by ID");
    Console.WriteLine("  search <query>                        Search bookmarks by title/URL");
    Console.WriteLine("  tags                                  Show all tags with counts");
    Console.WriteLine("  export [file]                         Export bookmarks to JSON/HTML");
    Console.WriteLine("  import <file>                         Import bookmarks from JSON");
    Console.WriteLine("  open <id>                             Open bookmark in browser");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project BookmarkManager.csproj add https://github.com \"GitHub\"");
    Console.WriteLine("  dotnet run --project BookmarkManager.csproj add https://dotnet.com --tags dotnet,tech");
    Console.WriteLine("  dotnet run --project BookmarkManager.csproj list");
    Console.WriteLine("  dotnet run --project BookmarkManager.csproj list --tags dotnet");
    Console.WriteLine("  dotnet run --project BookmarkManager.csproj search github");
    Console.WriteLine("  dotnet run --project BookmarkManager.csproj export bookmarks.html");
}

static async Task AddBookmarkAsync(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Error: URL required");
        return;
    }

    var url = args[0];
    if (!IsValidUrl(url))
    {
        Console.WriteLine("Error: Invalid URL format");
        return;
    }

    string title = args.Length > 1 && !args[1].StartsWith("--") ? args[1] : "";
    var tags = new List<string>();
    
    for (int i = 1; i < args.Length; i++)
    {
        if (args[i] == "--tags" && i + 1 < args.Length)
        {
            tags.AddRange(args[i + 1].Split(',', StringSplitOptions.RemoveEmptyEntries).Select(t => t.Trim()));
            break;
        }
    }

    if (string.IsNullOrEmpty(title))
    {
        title = await FetchTitleAsync(url) ?? new Uri(url).Host;
    }

    var bookmarks = LoadBookmarks();
    var bookmark = new Bookmark
    {
        Id = bookmarks.Count > 0 ? bookmarks.Max(b => b.Id) + 1 : 1,
        Url = url,
        Title = title,
        Tags = tags,
        CreatedAt = DateTime.UtcNow
    };

    bookmarks.Add(bookmark);
    SaveBookmarks(bookmarks);

    Console.WriteLine($"✓ Added bookmark: {bookmark.Title}");
    Console.WriteLine($"  URL: {bookmark.Url}");
    if (tags.Count > 0)
        Console.WriteLine($"  Tags: {string.Join(", ", tags)}");
}

static void ListBookmarks(string[] args)
{
    var bookmarks = LoadBookmarks();
    string? filterTag = null;

    for (int i = 0; i < args.Length; i++)
    {
        if (args[i] == "--tags" && i + 1 < args.Length)
        {
            filterTag = args[i + 1].Trim();
            break;
        }
        else if (!args[i].StartsWith("--"))
        {
            filterTag = args[i];
        }
    }

    if (filterTag != null)
    {
        bookmarks = bookmarks.Where(b => b.Tags.Any(t => t.Contains(filterTag, StringComparison.OrdinalIgnoreCase))).ToList();
    }

    if (bookmarks.Count == 0)
    {
        Console.WriteLine("No bookmarks found.");
        return;
    }

    Console.WriteLine($"Bookmarks ({bookmarks.Count} items):\n");
    Console.WriteLine(new string('-', 70));

    foreach (var bookmark in bookmarks.OrderBy(b => b.Title))
    {
        Console.WriteLine($"[{bookmark.Id}] {bookmark.Title}");
        Console.WriteLine($"    {bookmark.Url}");
        if (bookmark.Tags.Count > 0)
            Console.WriteLine($"    Tags: {string.Join(", ", bookmark.Tags)}");
        Console.WriteLine($"    Added: {bookmark.CreatedAt:yyyy-MM-dd}");
        Console.WriteLine();
    }
}

static void RemoveBookmark(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Error: Bookmark ID required");
        return;
    }

    if (!int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Error: Invalid ID");
        return;
    }

    var bookmarks = LoadBookmarks();
    var bookmark = bookmarks.FirstOrDefault(b => b.Id == id);

    if (bookmark == null)
    {
        Console.WriteLine($"Bookmark with ID {id} not found");
        return;
    }

    bookmarks.Remove(bookmark);
    SaveBookmarks(bookmarks);

    Console.WriteLine($"✓ Removed: {bookmark.Title}");
}

static void SearchBookmarks(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Error: Search query required");
        return;
    }

    var query = string.Join(" ", args).ToLower();
    var bookmarks = LoadBookmarks()
        .Where(b => b.Title.ToLower().Contains(query) || 
                    b.Url.ToLower().Contains(query) ||
                    b.Tags.Any(t => t.ToLower().Contains(query)))
        .ToList();

    if (bookmarks.Count == 0)
    {
        Console.WriteLine("No bookmarks found matching your search.");
        return;
    }

    Console.WriteLine($"Search results for '{query}' ({bookmarks.Count} found):\n");
    
    foreach (var bookmark in bookmarks)
    {
        Console.WriteLine($"[{bookmark.Id}] {bookmark.Title}");
        Console.WriteLine($"    {bookmark.Url}");
    }
}

static void ShowTags()
{
    var bookmarks = LoadBookmarks();
    var tagCounts = bookmarks
        .SelectMany(b => b.Tags)
        .GroupBy(t => t.ToLower())
        .OrderByDescending(g => g.Count())
        .ToList();

    if (tagCounts.Count == 0)
    {
        Console.WriteLine("No tags found.");
        return;
    }

    Console.WriteLine($"Tags ({tagCounts.Count} unique):\n");

    foreach (var tag in tagCounts)
    {
        var barLength = Math.Min(tag.Count(), 50);
        var bar = "".PadLeft(barLength, '#');
        Console.WriteLine($"  {tag.Key,-20} {bar} ({tag.Count()})");
    }
}

static void ExportBookmarks(string[] args)
{
    var bookmarks = LoadBookmarks();
    var fileName = args.Length > 0 ? args[0] : "bookmarks_export.json";

    if (fileName.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
    {
        ExportHtml(bookmarks, fileName);
        Console.WriteLine($"✓ Exported {bookmarks.Count} bookmarks to {fileName}");
    }
    else
    {
        var json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(fileName, json);
        Console.WriteLine($"✓ Exported {bookmarks.Count} bookmarks to {fileName}");
    }
}

static async Task ImportBookmarksAsync(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Error: File path required");
        return;
    }

    var fileName = args[0];
    if (!File.Exists(fileName))
    {
        Console.WriteLine($"Error: File '{fileName}' not found");
        return;
    }

    try
    {
        var json = File.ReadAllText(fileName);
        var imported = JsonSerializer.Deserialize<List<Bookmark>>(json) ?? new List<Bookmark>();
        
        var existing = LoadBookmarks();
        var maxId = existing.Count > 0 ? existing.Max(b => b.Id) : 0;
        
        foreach (var bookmark in imported)
        {
            bookmark.Id = ++maxId;
            existing.Add(bookmark);
        }

        SaveBookmarks(existing);
        Console.WriteLine($"✓ Imported {imported.Count} bookmarks");
    }
    catch (JsonException ex)
    {
        Console.WriteLine($"Error parsing JSON: {ex.Message}");
    }
}

static void OpenBookmark(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Error: Bookmark ID required");
        return;
    }

    if (!int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Error: Invalid ID");
        return;
    }

    var bookmarks = LoadBookmarks();
    var bookmark = bookmarks.FirstOrDefault(b => b.Id == id);

    if (bookmark == null)
    {
        Console.WriteLine($"Bookmark with ID {id} not found");
        return;
    }

    try
    {
        var psi = new System.Diagnostics.ProcessStartInfo
        {
            FileName = bookmark.Url,
            UseShellExecute = true
        };
        System.Diagnostics.Process.Start(psi);
        Console.WriteLine($"Opening: {bookmark.Title}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Could not open URL: {ex.Message}");
        Console.WriteLine($"URL: {bookmark.Url}");
    }
}

static List<Bookmark> LoadBookmarks()
{
    if (!File.Exists(DataFile))
        return new List<Bookmark>();

    try
    {
        var json = File.ReadAllText(DataFile);
        return JsonSerializer.Deserialize<List<Bookmark>>(json) ?? new List<Bookmark>();
    }
    catch
    {
        return new List<Bookmark>();
    }
}

static void SaveBookmarks(List<Bookmark> bookmarks)
{
    var json = JsonSerializer.Serialize(bookmarks, new JsonSerializerOptions { WriteIndented = true });
    File.WriteAllText(DataFile, json);
}

static bool IsValidUrl(string url)
{
    return Uri.TryCreate(url, UriKind.Absolute, out var uriResult) &&
           (uriResult.Scheme == Uri.UriSchemeHttp || uriResult.Scheme == Uri.UriSchemeHttps);
}

static async Task<string?> FetchTitleAsync(string url)
{
    try
    {
        using var client = new HttpClient();
        client.Timeout = TimeSpan.FromSeconds(5);
        var html = await client.GetStringAsync(url);
        
        var match = Regex.Match(html, "<title\\s*>([^<]+)</title>", RegexOptions.IgnoreCase);
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }
    catch
    {
        return null;
    }
}

static void ExportHtml(List<Bookmark> bookmarks, string fileName)
{
    var sb = new System.Text.StringBuilder();
    sb.AppendLine("<!DOCTYPE NETSCAPE-Bookmark-file-1>");
    sb.AppendLine("<META HTTP-EQUIV=\"Content-Type\" CONTENT=\"text/html; charset=UTF-8\">");
    sb.AppendLine("<TITLE>Bookmarks</TITLE>");
    sb.AppendLine("<H1>Bookmarks</H1>");
    sb.AppendLine("<DL><p>");

    foreach (var bookmark in bookmarks.OrderBy(b => b.Title))
    {
        sb.AppendLine($"    <DT><A HREF=\"{bookmark.Url}\" ADD_DATE=\"{new DateTimeOffset(bookmark.CreatedAt).ToUnixTimeSeconds()}\">{bookmark.Title}</A>");
        if (bookmark.Tags.Count > 0)
        {
            sb.AppendLine($"    <DD>Tags: {string.Join(", ", bookmark.Tags)}");
        }
    }

    sb.AppendLine("</DL><p>");
    File.WriteAllText(fileName, sb.ToString());
}

class Bookmark
{
    public int Id { get; set; }
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
