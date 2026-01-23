using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;

namespace JsonToCsv;

/// <summary>
/// Converts JSON arrays or objects to CSV format.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return 1;
        }

        var inputFile = "";
        var outputFile = "";
        var delimiter = ",";
        var includeHeaders = true;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-o" or "--output" when i + 1 < args.Length:
                    outputFile = args[++i];
                    break;
                case "-d" or "--delimiter" when i + 1 < args.Length:
                    delimiter = args[++i];
                    break;
                case "--no-headers":
                    includeHeaders = false;
                    break;
                case "-h" or "--help":
                    PrintUsage();
                    return 0;
                default:
                    if (!args[i].StartsWith("-"))
                        inputFile = args[i];
                    break;
            }
        }

        if (string.IsNullOrEmpty(inputFile))
        {
            Console.WriteLine("Error: No input file specified.");
            PrintUsage();
            return 1;
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Error: File '{inputFile}' does not exist.");
            return 1;
        }

        try
        {
            var json = File.ReadAllText(inputFile);
            var csv = ConvertJsonToCsv(json, delimiter, includeHeaders);

            if (!string.IsNullOrEmpty(outputFile))
            {
                File.WriteAllText(outputFile, csv);
                Console.WriteLine($"Converted {inputFile} -> {outputFile}");
            }
            else
            {
                Console.Write(csv);
            }

            return 0;
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Error: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("""
            JsonToCsv - Convert JSON arrays/objects to CSV format

            Usage:
              dotnet run --project JsonToCsv.csproj <input.json> [options]

            Options:
              -o, --output <file>     Output file (default: stdout)
              -d, --delimiter <char>  Field delimiter (default: ,)
              --no-headers            Omit header row
              -h, --help              Show this help

            Examples:
              dotnet run --project JsonToCsv.csproj data.json
              dotnet run --project JsonToCsv.csproj data.json -o output.csv
              dotnet run --project JsonToCsv.csproj data.json -d ";" --no-headers
              dotnet run --project JsonToCsv.csproj data.json | grep "search"
            """);
    }

    static string ConvertJsonToCsv(string json, string delimiter, bool includeHeaders)
    {
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;

        var rows = new List<List<string>>();
        var headers = new List<string>();

        if (root.ValueKind == JsonValueKind.Array)
        {
            // Handle array of objects
            foreach (var item in root.EnumerateArray())
            {
                if (item.ValueKind == JsonValueKind.Object)
                {
                    var row = new List<string>();
                    foreach (var prop in item.EnumerateObject())
                    {
                        if (headers.Count == 0 || !headers.Contains(prop.Name))
                            headers.Add(prop.Name);
                    }
                    rows.Add(row);
                }
                else
                {
                    // Handle array of primitives
                    rows.Add(new List<string> { FormatValue(item) });
                }
            }

            // Fill rows with values in header order
            if (headers.Count > 0)
            {
                rows.Clear();
                foreach (var item in root.EnumerateArray())
                {
                    if (item.ValueKind == JsonValueKind.Object)
                    {
                        var row = new List<string>();
                        foreach (var header in headers)
                        {
                            if (item.TryGetProperty(header, out var prop))
                                row.Add(FormatValue(prop));
                            else
                                row.Add("");
                        }
                        rows.Add(row);
                    }
                }
            }
        }
        else if (root.ValueKind == JsonValueKind.Object)
        {
            // Handle single object
            var row = new List<string>();
            foreach (var prop in root.EnumerateObject())
            {
                headers.Add(prop.Name);
                row.Add(FormatValue(prop.Value));
            }
            rows.Add(row);
        }

        // Build CSV
        var sb = new StringBuilder();

        if (includeHeaders && headers.Count > 0)
        {
            sb.AppendLine(string.Join(delimiter, headers.Select(EscapeCsv)));
        }

        foreach (var row in rows)
        {
            sb.AppendLine(string.Join(delimiter, row.Select(EscapeCsv)));
        }

        return sb.ToString();
    }

    static string FormatValue(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => element.GetString() ?? "",
            JsonValueKind.Number => element.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            JsonValueKind.Null => "",
            JsonValueKind.Array => "[array]",
            JsonValueKind.Object => "[object]",
            _ => element.GetRawText()
        };
    }

    static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        // Escape if contains delimiter, quotes, or newlines
        if (value.Contains('"') || value.Contains(',') || value.Contains('\n') || value.Contains('\r'))
        {
            return "\"" + value.Replace("\"", "\"\"") + "\"";
        }

        return value;
    }
}
