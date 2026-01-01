// JSON Formatter - Format (pretty-print) or minify JSON
using System.Text.Json;

class Program
{
    static void Main(string[] args)
    {
        bool minify = false;
        string? inputFile = null;
        string? outputFile = null;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-m":
                case "--minify":
                    minify = true;
                    break;
                case "-i":
                case "--input":
                    if (i + 1 < args.Length)
                        inputFile = args[++i];
                    break;
                case "-o":
                case "--output":
                    if (i + 1 < args.Length)
                        outputFile = args[++i];
                    break;
                case "-h":
                case "--help":
                    PrintHelp();
                    return;
            }
        }

        string json;

        if (!string.IsNullOrEmpty(inputFile))
        {
            if (!File.Exists(inputFile))
            {
                Console.WriteLine($"Error: File '{inputFile}' not found.");
                return;
            }
            json = File.ReadAllText(inputFile);
        }
        else
        {
            Console.WriteLine("Enter JSON (end with empty line or Ctrl+D):");
            var lines = new List<string>();
            string? line;
            while ((line = Console.ReadLine()) != null && line != "")
            {
                lines.Add(line);
            }
            json = string.Join("\n", lines);
        }

        try
        {
            var document = JsonDocument.Parse(json);
            string result;

            if (minify)
            {
                result = JsonSerializer.Serialize(document, new JsonSerializerOptions
                {
                    WriteIndented = false
                });
            }
            else
            {
                result = JsonSerializer.Serialize(document, new JsonSerializerOptions
                {
                    WriteIndented = true
                });
            }

            if (!string.IsNullOrEmpty(outputFile))
            {
                File.WriteAllText(outputFile, result);
                Console.WriteLine($"Output written to: {outputFile}");
            }
            else
            {
                Console.WriteLine(result);
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Invalid JSON: {ex.Message}");
        }
    }

    static void PrintHelp()
    {
        Console.WriteLine(@"
JSON Formatter - Format (pretty-print) or minify JSON

Usage: JsonFormatter [options]

Options:
  -m, --minify         Minify JSON (remove whitespace)
  -i, --input <file>   Input JSON file (reads from stdin if not specified)
  -o, --output <file>  Output file (prints to stdout if not specified)
  -h, --help           Show this help message

Examples:
  JsonFormatter -i input.json                    # Format file
  JsonFormatter -i input.json -o output.json     # Format and save
  JsonFormatter --minify -i pretty.json          # Minify file
  echo '{""a"":1}' | JsonFormatter                # Format from stdin
");
    }
}
