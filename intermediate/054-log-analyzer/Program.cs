using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Collections.Generic;

namespace LogAnalyzer;

/// <summary>
/// Log file analyzer for parsing and analyzing application logs.
/// Supports multiple log formats and provides statistics, filtering, and error extraction.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();

        switch (command)
        {
            case "parse":
                ParseLogFile(args.ElementAtOrDefault(1) ?? "app.log");
                break;
            case "errors":
                ExtractErrors(args.ElementAtOrDefault(1) ?? "app.log");
                break;
            case "stats":
                ShowStatistics(args.ElementAtOrDefault(1) ?? "app.log");
                break;
            case "filter":
                FilterByLevel(args.ElementAtOrDefault(1) ?? "app.log", args.ElementAtOrDefault(2) ?? "ERROR");
                break;
            case "search":
                SearchLogs(args.ElementAtOrDefault(1) ?? "app.log", string.Join(" ", args.Skip(2)));
                break;
            case "timeline":
                ShowTimeline(args.ElementAtOrDefault(1) ?? "app.log");
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                ShowMenu();
                break;
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("Log Analyzer - Available Commands:");
        Console.WriteLine("  parse <logfile>           - Parse and display log entries");
        Console.WriteLine("  errors <logfile>          - Extract all ERROR entries");
        Console.WriteLine("  stats <logfile>           - Show log statistics");
        Console.WriteLine("  filter <logfile> <level>  - Filter by log level");
        Console.WriteLine("  search <logfile> <term>   - Search for text in logs");
        Console.WriteLine("  timeline <logfile>        - Show activity timeline");
        Console.WriteLine();
        Console.WriteLine("Example: dotnet run -- errors app.log");
    }

    static List<LogEntry> ReadLogFile(string path)
    {
        var entries = new List<LogEntry>();

        if (!File.Exists(path))
        {
            Console.WriteLine($"Log file not found: {path}");
            return entries;
        }

        // Pattern: [TIMESTAMP] [LEVEL] Message
        var pattern = @"^\[(\d{4}-\d{2}-\d{2} \d{2}:\d{2}:\d{2})\] \[(\w+)\] (.+)$";
        var regex = new Regex(pattern);

        foreach (var line in File.ReadLines(path))
        {
            var match = regex.Match(line);
            if (match.Success)
            {
                entries.Add(new LogEntry
                {
                    Timestamp = DateTime.Parse(match.Groups[1].Value),
                    Level = match.Groups[2].Value,
                    Message = match.Groups[3].Value,
                    Raw = line
                });
            }
        }

        return entries;
    }

    static void ParseLogFile(string path)
    {
        var entries = ReadLogFile(path);
        if (entries.Count == 0) return;

        Console.WriteLine($"Parsed {entries.Count} log entries from '{path}':\n");
        foreach (var entry in entries.Take(20))
        {
            Console.WriteLine($"[{entry.Timestamp:HH:mm:ss}] [{entry.Level,-5}] {entry.Message}");
        }
        if (entries.Count > 20)
            Console.WriteLine($"\n... and {entries.Count - 20} more entries");
    }

    static void ExtractErrors(string path)
    {
        var entries = ReadLogFile(path);
        var errors = entries.Where(e => e.Level == "ERROR" || e.Level == "FATAL").ToList();

        Console.WriteLine($"Found {errors.Count} error(s) in '{path}':\n");
        foreach (var error in errors)
        {
            Console.WriteLine($"[{error.Timestamp:yyyy-MM-dd HH:mm:ss}] {error.Message}");
        }
    }

    static void ShowStatistics(string path)
    {
        var entries = ReadLogFile(path);
        if (entries.Count == 0) return;

        var levelCounts = new Dictionary<string, int>();
        foreach (var entry in entries)
        {
            if (!levelCounts.ContainsKey(entry.Level))
                levelCounts[entry.Level] = 0;
            levelCounts[entry.Level]++;
        }

        Console.WriteLine($"Log Statistics for '{path}':");
        Console.WriteLine($"  Total entries: {entries.Count}");
        Console.WriteLine($"  Time range: {entries.Min(e => e.Timestamp):yyyy-MM-dd HH:mm:ss} to {entries.Max(e => e.Timestamp):yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine();
        Console.WriteLine("  Entries by level:");
        foreach (var level in levelCounts.OrderByDescending(l => l.Value))
        {
            double percentage = (double)level.Value / entries.Count * 100;
            Console.WriteLine($"    {level.Key,-10} {level.Value,6} ({percentage,5:F1}%)");
        }

        var errors = entries.Count(e => e.Level == "ERROR" || e.Level == "FATAL");
        var warnings = entries.Count(e => e.Level == "WARN" || e.Level == "WARNING");
        Console.WriteLine();
        Console.WriteLine($"  Error rate: {(double)errors / entries.Count * 100:F2}%");
        Console.WriteLine($"  Warning rate: {(double)warnings / entries.Count * 100:F2}%");
    }

    static void FilterByLevel(string path, string level)
    {
        var entries = ReadLogFile(path);
        var filtered = entries.Where(e => e.Level.Equals(level, StringComparison.OrdinalIgnoreCase)).ToList();

        Console.WriteLine($"Filtered {filtered.Count} entries with level '{level}':\n");
        foreach (var entry in filtered)
        {
            Console.WriteLine($"[{entry.Timestamp:yyyy-MM-dd HH:mm:ss}] {entry.Message}");
        }
    }

    static void SearchLogs(string path, string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            Console.WriteLine("Please specify a search term");
            return;
        }

        var entries = ReadLogFile(path);
        var matches = entries.Where(e => e.Message.Contains(term, StringComparison.OrdinalIgnoreCase)).ToList();

        Console.WriteLine($"Found {matches.Count} matches for '{term}':\n");
        foreach (var entry in matches.Take(50))
        {
            Console.WriteLine($"[{entry.Timestamp:HH:mm:ss}] [{entry.Level,-5}] {entry.Message}");
        }
        if (matches.Count > 50)
            Console.WriteLine($"\n... and {matches.Count - 50} more matches");
    }

    static void ShowTimeline(string path)
    {
        var entries = ReadLogFile(path);
        if (entries.Count == 0) return;

        var timeline = entries
            .GroupBy(e => e.Timestamp.Hour)
            .OrderBy(g => g.Key)
            .ToList();

        Console.WriteLine($"Activity Timeline for '{path}':\n");
        int maxCount = timeline.Max(g => g.Count());

        foreach (var hour in timeline)
        {
            int barLength = (int)((double)hour.Count() / maxCount * 40);
            string bar = new string('█', barLength);
            Console.WriteLine($"  {hour.Key:D2}:00 │ {bar} {hour.Count()}");
        }
    }
}

/// <summary>
/// Represents a parsed log entry.
/// </summary>
class LogEntry
{
    public DateTime Timestamp { get; set; }
    public string Level { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Raw { get; set; } = string.Empty;
}
