using System.Text.Json;
using System.Net.Http.Json;

namespace BookmarkManager;

class Program
{
    private const string DataFile = "bookmarks.json";
    private static List<Bookmark> _bookmarks = new();
    private static readonly HttpClient HttpClient = new();

    static void Main(string[] args)
    {
        Console.WriteLine("🔖 Bookmark Manager");
        Console.WriteLine("===================\n");

        LoadBookmarks();

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();
        switch (command)
        {
            case "add":
                AddBookmark(args.Skip(1).ToArray());
                break;
            case "list":
                ListBookmarks();
                break;
            case "search":
                SearchBookmarks(args.Skip(1).ToArray());
                break;
            case "open":
                OpenBookmark(args.ElementAtOrDefault(1));
                break;
            case "delete":
                DeleteBookmark(args.ElementAtOrDefault(1));
                break;
            case "tags":
                ShowTags();
                break;
            case "validate":
                ValidateBookmarks();
                break;
            case "export":
                ExportBookmarks();
                break;
            default:
                ShowMenu();
                break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  add <url> [title] [tags]  - Add a bookmark");
        Console.WriteLine("  list                      - List all bookmarks");
        Console.WriteLine("  search <query>            - Search bookmarks");
        Console.WriteLine("  open <id>                 - Open bookmark URL");
        Console.WriteLine("  delete <id>               - Delete a bookmark");
        Console.WriteLine("  tags                      - Show all tags");
        Console.WriteLine("  validate                  - Check if URLs are accessible");
        Console.WriteLine("  export                    - Export to HTML");
        Console.WriteLine();

        Console.Write("Enter command: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        var parts = input.Trim().Split(' ', 3);
        string cmd = parts[0].ToLower();
        string[] remaining = parts.Skip(1).ToArray();

        switch (cmd)
        {
            case "add": AddBookmark(remaining); break;
            case "list": ListBookmarks(); break;
            case "search": SearchBookmarks(remaining); break;
            case "open": OpenBookmark(remaining.FirstOrDefault()); break;
            case "delete": DeleteBookmark(remaining.FirstOrDefault()); break;
            case "tags": ShowTags(); break;
            case "validate": ValidateBookmarks(); break;
            case "export": ExportBookmarks(); break;
        }
    }

    private static void LoadBookmarks()
    {
        if (File.Exists(DataFile))
        {
            string json = File.ReadAllText(DataFile);
            _bookmarks = JsonSerializer.Deserialize<List<Bookmark>>(json) ?? new List<Bookmark>();
        }
    }

    private static void SaveBookmarks()
    {
        string json = JsonSerializer.Serialize(_bookmarks, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFile, json);
    }

    private static void AddBookmark(string[] args)
    {
        if (args.Length == 0 || string.IsNullOrWhiteSpace(args[0]))
        {
            Console.Write("URL: ");
            string? url = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(url))
            {
                Console.WriteLine("URL is required!");
                return;
            }
            args = new[] { url! };
        }

        string url = args[0];
        string title = args.ElementAtOrDefault(1) ?? "";
        var tags = new List<string>();

        if (args.Length > 2)
        {
            tags = args.Skip(2).SelectMany(t => t.Split(',')).Select(t => t.Trim()).Where(t => !string.IsNullOrEmpty(t)).ToList();
        }

        // Try to fetch title from URL
        if (string.IsNullOrEmpty(title))
        {
            title = FetchPageTitle(url).Result ?? url;
        }

        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Url = url,
            Title = title,
            Tags = tags,
            CreatedAt = DateTime.Now
        };

        _bookmarks.Add(bookmark);
        SaveBookmarks();

        Console.WriteLine($"\n✅ Bookmark added: {bookmark.Title}");
        Console.WriteLine($"   ID: {bookmark.Id}");
    }

    private static async Task<string?> FetchPageTitle(string url)
    {
        try
        {
            var response = await HttpClient.GetAsync(url);
            if (response.IsSuccessStatusCode)
            {
                var html = await response.Content.ReadAsStringAsync();
                var titleStart = html.IndexOf("<title>", StringComparison.OrdinalIgnoreCase);
                var titleEnd = html.IndexOf("</title>", StringComparison.OrdinalIgnoreCase);
                if (titleStart >= 0 && titleEnd > titleStart)
                {
                    return html[(titleStart + 7)..titleEnd].Trim();
                }
            }
        }
        catch { }
        return null;
    }

    private static void ListBookmarks()
    {
        if (_bookmarks.Count == 0)
        {
            Console.WriteLine("No bookmarks found.");
            return;
        }

        Console.WriteLine($"{"ID",-10} {"Title",-35} {"Tags",-25} {"Added",-12}");
        Console.WriteLine(new string('-', 85));

        foreach (var bookmark in _bookmarks.OrderByDescending(b => b.CreatedAt))
        {
            string tags = string.Join(",", bookmark.Tags.Take(3));
            Console.WriteLine($"{bookmark.Id,-10} {Truncate(bookmark.Title, 35),-35} {tags,-25} {bookmark.CreatedAt:yyyy-MM-dd}");
        }

        Console.WriteLine($"\nTotal: {_bookmarks.Count} bookmarks");
    }

    private static void SearchBookmarks(string[] args)
    {
        string? query = args.FirstOrDefault();
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

        var results = _bookmarks.Where(b =>
            b.Title.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            b.Url.Contains(query!, StringComparison.OrdinalIgnoreCase) ||
            b.Tags.Any(t => t.Contains(query!, StringComparison.OrdinalIgnoreCase))
        ).ToList();

        if (results.Count == 0)
        {
            Console.WriteLine($"No bookmarks found matching '{query}'.");
            return;
        }

        Console.WriteLine($"\nFound {results.Count} bookmark(s):\n");
        foreach (var bookmark in results)
        {
            Console.WriteLine($"[{bookmark.Id}] {bookmark.Title}");
            Console.WriteLine($"  URL: {bookmark.Url}");
            Console.WriteLine($"  Tags: {string.Join(", ", bookmark.Tags)}");
            Console.WriteLine();
        }
    }

    private static void OpenBookmark(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Bookmark ID: ");
            id = Console.ReadLine();
        }

        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == id);
        if (bookmark == null)
        {
            Console.WriteLine($"Bookmark '{id}' not found.");
            return;
        }

        Console.WriteLine($"Opening: {bookmark.Url}");
        
        // Try to open in default browser
        try
        {
            var psi = new System.Diagnostics.ProcessStartInfo
            {
                FileName = bookmark.Url,
                UseShellExecute = true
            };
            System.Diagnostics.Process.Start(psi);
        }
        catch
        {
            Console.WriteLine("Could not open browser. Please copy the URL manually.");
        }
    }

    private static void DeleteBookmark(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Bookmark ID to delete: ");
            id = Console.ReadLine();
        }

        var bookmark = _bookmarks.FirstOrDefault(b => b.Id == id);
        if (bookmark == null)
        {
            Console.WriteLine($"Bookmark '{id}' not found.");
            return;
        }

        Console.Write($"Delete '{bookmark.Title}'? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _bookmarks.Remove(bookmark);
            SaveBookmarks();
            Console.WriteLine("✅ Bookmark deleted.");
        }
    }

    private static void ShowTags()
    {
        var allTags = _bookmarks.SelectMany(b => b.Tags)
            .GroupBy(t => t.ToLower())
            .Select(g => new { Tag = g.Key, Count = g.Count() })
            .OrderByDescending(t => t.Count)
            .ToList();

        if (allTags.Count == 0)
        {
            Console.WriteLine("No tags found.");
            return;
        }

        Console.WriteLine($"{"Tag",-20} {"Count",-10}");
        Console.WriteLine(new string('-', 30));

        foreach (var tag in allTags)
        {
            Console.WriteLine($"{tag.Tag,-20} {tag.Count,-10}");
        }

        Console.WriteLine($"\nTotal unique tags: {allTags.Count}");
    }

    private static async Task ValidateBookmarks()
    {
        Console.WriteLine("Validating bookmarks...\n");

        int valid = 0, invalid = 0;

        foreach (var bookmark in _bookmarks)
        {
            Console.Write($"Checking {bookmark.Title}... ");
            try
            {
                var response = await HttpClient.GetAsync(bookmark.Url);
                if (response.IsSuccessStatusCode)
                {
                    Console.WriteLine("✅ OK");
                    valid++;
                }
                else
                {
                    Console.WriteLine($"❌ {response.StatusCode}");
                    invalid++;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error: {ex.Message}");
                invalid++;
            }
        }

        Console.WriteLine($"\nResults: {valid} valid, {invalid} invalid");
    }

    private static void ExportBookmarks()
    {
        string outputFile = $"bookmarks_{DateTime.Now:yyyyMMdd_HHmmss}.html";
        var html = new System.Text.StringBuilder();

        html.AppendLine("<!DOCTYPE html>");
        html.AppendLine("<html><head><title>My Bookmarks</title>");
        html.AppendLine("<meta charset='utf-8'></head><body>");
        html.AppendLine("<h1>My Bookmarks</h1>");
        html.AppendLine($"<p>Exported: {DateTime.Now:yyyy-MM-dd HH:mm:ss}</p>");
        html.AppendLine($"<p>Total: {_bookmarks.Count} bookmarks</p>");

        var grouped = _bookmarks.GroupBy(b => b.Tags.FirstOrDefault() ?? "Uncategorized");

        foreach (var group in grouped)
        {
            html.AppendLine($"\n<h2>{group.Key}</h2>");
            html.AppendLine("<ul>");

            foreach (var bookmark in group.OrderBy(b => b.Title))
            {
                html.AppendLine($"  <li><a href=\"{bookmark.Url}\">{bookmark.Title}</a>");
                if (bookmark.Tags.Any())
                {
                    html.AppendLine($"    <small>[{string.Join(", ", bookmark.Tags)}]</small>");
                }
                html.AppendLine("  </li>");
            }

            html.AppendLine("</ul>");
        }

        html.AppendLine("</body></html>");

        File.WriteAllText(outputFile, html.ToString());
        Console.WriteLine($"✅ Exported to: {outputFile}");
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}

public class Bookmark
{
    public string Id { get; set; } = "";
    public string Url { get; set; } = "";
    public string Title { get; set; } = "";
    public List<string> Tags { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}
