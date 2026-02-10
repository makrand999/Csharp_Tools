using System.Text.RegularExpressions;

namespace LogParser;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Log File Parser");
            Console.WriteLine("Usage: dotnet run --project 139-log-parser.csproj -- <logfile.log>");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --level LEVEL    Filter by level (ERROR, WARN, INFO, DEBUG)");
            Console.WriteLine("  --pattern REGEX  Filter by pattern");
            Console.WriteLine("  --from DATE      Filter from date (yyyy-MM-dd)");
            Console.WriteLine("  --to DATE        Filter to date (yyyy-MM-dd)");
            Console.WriteLine("  --stats          Show statistics only");
            return;
        }

        string inputFile = args[0];
        string? levelFilter = null;
        string? patternFilter = null;
        DateTime? fromDate = null;
        DateTime? toDate = null;
        bool statsOnly = args.Contains("--stats");

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--level" && i + 1 < args.Length)
            {
                levelFilter = args[i + 1].ToUpper();
            }
            else if (args[i] == "--pattern" && i + 1 < args.Length)
            {
                patternFilter = args[i + 1];
            }
            else if (args[i] == "--from" && i + 1 < args.Length)
            {
                DateTime.TryParse(args[i + 1], out var d);
                fromDate = d;
            }
            else if (args[i] == "--to" && i + 1 < args.Length)
            {
                DateTime.TryParse(args[i + 1], out var d);
                toDate = d;
            }
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Error: File not found: {inputFile}");
            return;
        }

        try
        {
            var lines = File.ReadAllLines(inputFile);
            var entries = ParseLogLines(lines);

            // Apply filters
            var filtered = entries.Where(e =>
            {
                if (levelFilter != null && e.Level != levelFilter) return false;
                if (patternFilter != null && !Regex.IsMatch(e.Message, patternFilter)) return false;
                if (fromDate != null && e.Timestamp < fromDate) return false;
                if (toDate != null && e.Timestamp > toDate) return false;
                return true;
            }).ToList();

            if (statsOnly)
            {
                ShowStatistics(entries);
            }
            else
            {
                Console.WriteLine($"=== Log Entries ({filtered.Count} of {entries.Count}) ===\n");
                foreach (var entry in filtered.Take(100))
                {
                    Console.WriteLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] [{entry.Level}] {entry.Message}");
                }
                if (filtered.Count > 100)
                {
                    Console.WriteLine($"\n... and {filtered.Count - 100} more entries");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static List<LogEntry> ParseLogLines(string[] lines)
    {
        var entries = new List<LogEntry>();

        // Common log patterns
        var patterns = new[]
        {
            // Pattern: 2024-01-15 10:30:45 [INFO] Message
            @"^(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})\s+\[(\w+)\]\s+(.*)$",
            // Pattern: [2024-01-15 10:30:45] INFO: Message
            @"^\[(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})\]\s+(\w+):\s*(.*)$",
            // Pattern: INFO 2024-01-15 10:30:45 - Message
            @"^(\w+)\s+(\d{4}-\d{2}-\d{2}\s+\d{2}:\d{2}:\d{2})\s+-\s+(.*)$",
        };

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line))
                continue;

            LogEntry? entry = null;

            foreach (var pattern in patterns)
            {
                var match = Regex.Match(line, pattern);
                if (match.Success)
                {
                    entry = new LogEntry
                    {
                        Timestamp = DateTime.Parse(match.Groups[1].Value),
                        Level = match.Groups[2].Value.ToUpper(),
                        Message = match.Groups[3].Value
                    };
                    break;
                }
            }

            // If no pattern matched, treat as raw message
            if (entry == null)
            {
                entry = new LogEntry
                {
                    Timestamp = DateTime.Now,
                    Level = "UNKNOWN",
                    Message = line
                };
            }

            entries.Add(entry);
        }

        return entries;
    }

    static void ShowStatistics(List<LogEntry> entries)
    {
        Console.WriteLine("=== Log Statistics ===\n");

        var byLevel = entries.GroupBy(e => e.Level)
            .OrderByDescending(g => g.Count())
            .ToList();

        Console.WriteLine("Entries by Level:");
        foreach (var group in byLevel)
        {
            Console.WriteLine($"  {group.Key,-10} {group.Count(),8:N0} ({group.Count() * 100.0 / entries.Count,5:F1}%)");
        }

        if (entries.Any())
        {
            Console.WriteLine($"\nTime Range:");
            Console.WriteLine($"  First entry: {entries.Min(e => e.Timestamp):yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Last entry:  {entries.Max(e => e.Timestamp):yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"  Duration:    {entries.Max(e => e.Timestamp) - entries.Min(e => e.Timestamp)}");
        }

        Console.WriteLine($"\nTotal entries: {entries.Count:N0}");

        // Find most common words in messages
        var words = entries
            .SelectMany(e => e.Message.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            .Where(w => w.Length > 3)
            .GroupBy(w => w.ToLower())
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToList();

        Console.WriteLine("\nMost common words:");
        foreach (var word in words)
        {
            Console.WriteLine($"  {word.Key,-20} {word.Count(),5}");
        }
    }
}

class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = "";
    public string Message { get; set; } = "";
}
