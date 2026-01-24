using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

namespace CsvToJson;

/// <summary>
/// Converts CSV files to JSON format.
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
        var delimiter = ',';
        var hasHeaders = true;
        var pretty = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "-o" or "--output" when i + 1 < args.Length:
                    outputFile = args[++i];
                    break;
                case "-d" or "--delimiter" when i + 1 < args.Length:
                    if (args[++i].Length == 1)
                        delimiter = args[i][0];
                    break;
                case "--no-headers":
                    hasHeaders = false;
                    break;
                case "--pretty":
                    pretty = true;
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
            var csv = File.ReadAllText(inputFile);
            var json = ConvertCsvToJson(csv, delimiter, hasHeaders);

            var output = pretty 
                ? JsonDocument.Parse(json).ToString() 
                : json;

            if (!string.IsNullOrEmpty(outputFile))
            {
                File.WriteAllText(outputFile, output);
                Console.WriteLine($"Converted {inputFile} -> {outputFile}");
            }
            else
            {
                Console.Write(output);
            }

            return 0;
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
            CsvToJson - Convert CSV files to JSON format

            Usage:
              dotnet run --project CsvToJson.csproj <input.csv> [options]

            Options:
              -o, --output <file>     Output file (default: stdout)
              -d, --delimiter <char>  Field delimiter (default: ,)
              --no-headers            First row is data, not headers
              --pretty                Pretty-print JSON output
              -h, --help              Show this help

            Examples:
              dotnet run --project CsvToJson.csproj data.csv
              dotnet run --project CsvToJson.csproj data.csv -o output.json
              dotnet run --project CsvToJson.csproj data.csv --pretty
              dotnet run --project CsvToJson.csproj data.csv -d ";" --no-headers
            """);
    }

    static string ConvertCsvToJson(string csv, char delimiter, bool hasHeaders)
    {
        var lines = ParseCsvLines(csv, delimiter);
        if (lines.Count == 0)
            return "[]";

        var headers = hasHeaders ? lines[0] : GenerateHeaders(lines[0].Count);
        var dataStart = hasHeaders ? 1 : 0;

        var records = new List<Dictionary<string, string?>>();

        for (int i = dataStart; i < lines.Count; i++)
        {
            var record = new Dictionary<string, string?>();
            var line = lines[i];

            for (int j = 0; j < headers.Count; j++)
            {
                var key = headers[j];
                var value = j < line.Count ? line[j] : null;
                record[key] = value;
            }

            records.Add(record);
        }

        var options = new JsonSerializerOptions { WriteIndented = false };
        return JsonSerializer.Serialize(records, options);
    }

    static List<List<string>> ParseCsvLines(string csv, char delimiter)
    {
        var lines = new List<List<string>>();
        var reader = new StringReader(csv);

        while (true)
        {
            var line = ReadCsvLine(reader, delimiter);
            if (line == null || line.Count == 0)
                break;

            // Skip empty lines
            if (line.Count == 1 && string.IsNullOrEmpty(line[0]))
                continue;

            lines.Add(line);
        }

        return lines;
    }

    static List<string>? ReadCsvLine(StringReader reader, char delimiter)
    {
        var line = reader.ReadLine();
        if (line == null)
            return null;

        var fields = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (inQuotes)
            {
                if (c == '"')
                {
                    // Check for escaped quote
                    if (i + 1 < line.Length && line[i + 1] == '"')
                    {
                        current.Append('"');
                        i++; // Skip next quote
                    }
                    else
                    {
                        inQuotes = false;
                    }
                }
                else
                {
                    current.Append(c);
                }
            }
            else
            {
                if (c == '"')
                {
                    inQuotes = true;
                }
                else if (c == delimiter)
                {
                    fields.Add(current.ToString());
                    current.Clear();
                }
                else
                {
                    current.Append(c);
                }
            }
        }

        fields.Add(current.ToString());
        return fields;
    }

    static List<string> GenerateHeaders(int count)
    {
        var headers = new List<string>();
        for (int i = 0; i < count; i++)
        {
            headers.Add($"column{i + 1}");
        }
        return headers;
    }
}
