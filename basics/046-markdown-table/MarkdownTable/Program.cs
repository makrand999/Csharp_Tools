using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace MarkdownTable;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Markdown Table Generator");
            Console.WriteLine("Usage: dotnet run --project MarkdownTable.csproj [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --from-csv <file>    Convert CSV file to markdown table");
            Console.WriteLine("  --from-json <file>   Convert JSON array to markdown table");
            Console.WriteLine("  --interactive        Interactive table builder");
            Console.WriteLine("  --demo               Generate sample tables");
            return 1;
        }

        try
        {
            string option = args[0].ToLower();
            
            return option switch
            {
                "--from-csv" when args.Length >= 2 => FromCsv(args[1]),
                "--from-json" when args.Length >= 2 => FromJson(args[1]),
                "--interactive" => Interactive(),
                "--demo" => Demo(),
                _ => Interactive()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static int FromCsv(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"CSV file not found: {filePath}");

        var lines = File.ReadAllLines(filePath);
        if (lines.Length < 2)
            throw new ArgumentException("CSV must have header and at least one data row");

        var headers = ParseCsvLine(lines[0]);
        var rows = lines.Skip(1).Select(ParseCsvLine).ToList();

        Console.WriteLine(GenerateMarkdownTable(headers, rows));
        return 0;
    }

    static int FromJson(string filePath)
    {
        if (!File.Exists(filePath))
            throw new FileNotFoundException($"JSON file not found: {filePath}");

        var json = File.ReadAllText(filePath);
        
        // Simple JSON array parser (for flat objects)
        var objects = ParseJsonArray(json);
        if (objects.Count == 0)
            throw new ArgumentException("Empty or invalid JSON array");

        var headers = objects.First().Keys.ToList();
        var rows = objects.Select(o => headers.Select(h => o[h] ?? "").ToList()).ToList();

        Console.WriteLine(GenerateMarkdownTable(headers, rows));
        return 0;
    }

    static int Interactive()
    {
        Console.WriteLine("Markdown Table Generator (Interactive Mode)");
        Console.WriteLine("==========================================");
        Console.WriteLine();

        // Get headers
        Console.WriteLine("Enter column headers (comma-separated):");
        var headers = ParseCsvLine(Console.ReadLine() ?? "");
        
        if (headers.Count == 0)
        {
            Console.WriteLine("No headers provided. Exiting.");
            return 1;
        }

        // Get rows
        var rows = new List<List<string>>();
        Console.WriteLine();
        Console.WriteLine($"Enter data rows ({headers.Count} columns, comma-separated). Empty line to finish:");
        
        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(input)) break;

            var values = ParseCsvLine(input);
            
            // Pad or trim to match header count
            while (values.Count < headers.Count) values.Add("");
            if (values.Count > headers.Count) values = values.Take(headers.Count).ToList();
            
            rows.Add(values);
        }

        if (rows.Count == 0)
        {
            Console.WriteLine("No data rows. Exiting.");
            return 1;
        }

        // Generate and display table
        Console.WriteLine();
        Console.WriteLine("Generated Markdown Table:");
        Console.WriteLine("-------------------------");
        Console.WriteLine(GenerateMarkdownTable(headers, rows));
        
        return 0;
    }

    static int Demo()
    {
        Console.WriteLine("Markdown Table Generator - Demo");
        Console.WriteLine("================================");
        Console.WriteLine();

        // Demo 1: Simple table
        var headers1 = new List<string> { "Name", "Age", "City" };
        var rows1 = new List<List<string>>
        {
            new() { "Alice", "30", "New York" },
            new() { "Bob", "25", "Los Angeles" },
            new() { "Charlie", "35", "Chicago" }
        };
        
        Console.WriteLine("Example 1 - Simple Table:");
        Console.WriteLine(GenerateMarkdownTable(headers1, rows1));
        Console.WriteLine();

        // Demo 2: Alignment
        var headers2 = new List<string> { "Product", "Price", "Qty", "Total" };
        var rows2 = new List<List<string>>
        {
            new() { "Widget", "$9.99", "10", "$99.90" },
            new() { "Gadget", "$24.99", "5", "$124.95" },
            new() { "Gizmo", "$14.99", "8", "$119.92" }
        };

        Console.WriteLine("Example 2 - Product List:");
        Console.WriteLine(GenerateMarkdownTable(headers2, rows2, new[] { Align.Left, Align.Right, Align.Center, Align.Right }));
        Console.WriteLine();

        // Demo 3: Code/data table
        var headers3 = new List<string> { "Method", "Parameters", "Returns", "Description" };
        var rows3 = new List<List<string>>
        {
            new() { "Parse", "string input", "int", "Parses string to integer" },
            new() { "ToString", "int value", "string", "Converts int to string" },
            new() { "Format", "string fmt, object[] args", "string", "Formats objects" }
        };

        Console.WriteLine("Example 3 - API Documentation:");
        Console.WriteLine(GenerateMarkdownTable(headers3, rows3));

        return 0;
    }

    static string GenerateMarkdownTable(List<string> headers, List<List<string>> rows, Align[]? alignment = null)
    {
        alignment ??= Enumerable.Repeat(Align.Left, headers.Count).ToArray();
        
        // Calculate column widths
        var widths = new int[headers.Count];
        for (int i = 0; i < headers.Count; i++)
        {
            widths[i] = Math.Max(headers[i].Length, 3);
            foreach (var row in rows)
            {
                if (i < row.Count)
                    widths[i] = Math.Max(widths[i], row[i].Length);
            }
        }

        var result = new List<string>();

        // Header row
        result.Add("| " + string.Join(" | ", headers.Select((h, i) => h.PadRight(widths[i]))) + " |");

        // Separator row with alignment
        var separators = alignment.Select((align, i) => align switch
        {
            Align.Left => "-" + new string('-', widths[i]) + ":",
            Align.Center => ":" + new string('-', widths[i]) + ":",
            Align.Right => ":" + new string('-', widths[i]),
            _ => "-" + new string('-', widths[i] - 1)
        }).ToList();
        
        result.Add("|" + string.Join("|", separators) + "|");

        // Data rows
        foreach (var row in rows)
        {
            var padded = row.Select((v, i) => i < widths.Length ? v.PadRight(widths[i]) : v).ToList();
            result.Add("| " + string.Join(" | ", padded) + " |");
        }

        return string.Join(Environment.NewLine, result);
    }

    static List<string> ParseCsvLine(string line)
    {
        var result = new List<string>();
        bool inQuotes = false;
        var current = "";

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current += '"';
                    i++;
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == ',' && !inQuotes)
            {
                result.Add(current.Trim());
                current = "";
            }
            else
            {
                current += c;
            }
        }

        result.Add(current.Trim());
        return result;
    }

    static List<Dictionary<string, string>> ParseJsonArray(string json)
    {
        var result = new List<Dictionary<string, string>>();
        
        // Very simple JSON array parser for flat objects
        json = json.Trim();
        if (!json.StartsWith("[") || !json.EndsWith("]"))
            return result;

        json = json[1..^1].Trim();
        if (string.IsNullOrEmpty(json))
            return result;

        // Split by },{ pattern
        var objects = json.Split("},{", StringSplitOptions.None);
        
        foreach (var obj in objects)
        {
            var clean = obj.Trim('{', '}', ' ', '\n', '\r');
            var dict = new Dictionary<string, string>();
            
            // Simple key-value parsing
            var pairs = clean.Split("\",");
            foreach (var pair in pairs)
            {
                var parts = pair.Split(':');
                if (parts.Length >= 2)
                {
                    var key = parts[0].Trim('"', ' ');
                    var value = string.Join(":", parts.Skip(1)).Trim('"', ' ');
                    dict[key] = value;
                }
            }
            
            if (dict.Count > 0)
                result.Add(dict);
        }

        return result;
    }
}

enum Align { Left, Center, Right }
