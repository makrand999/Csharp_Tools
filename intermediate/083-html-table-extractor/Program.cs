using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

var (inputPath, outputPath, format, tableIndex) = ParseArguments(args);

string htmlContent = string.Empty;

if (!string.IsNullOrEmpty(inputPath) && File.Exists(inputPath))
{
    htmlContent = await File.ReadAllTextAsync(inputPath);
}
else if (!string.IsNullOrEmpty(inputPath) && inputPath.StartsWith("http"))
{
    using var httpClient = new HttpClient();
    htmlContent = await httpClient.GetStringAsync(inputPath);
}
else if (!string.IsNullOrEmpty(inputPath))
{
    Console.Error.WriteLine($"Error: File not found: {inputPath}");
    Environment.Exit(1);
}
else
{
    htmlContent = Console.In.ReadToEnd();
}

if (string.IsNullOrWhiteSpace(htmlContent))
{
    Console.Error.WriteLine("Error: No HTML input provided.");
    ShowUsage();
    Environment.Exit(1);
}

try
{
    var parser = new HtmlParser();
    var document = await parser.ParseDocumentAsync(htmlContent);
    var tables = document.QuerySelectorAll("table").ToList();
    
    if (tables.Count == 0)
    {
        Console.Error.WriteLine("No tables found in the HTML document.");
        Environment.Exit(1);
    }
    
    List<IHtmlTableElement> tablesToProcess;
    if (tableIndex.HasValue)
    {
        tablesToProcess = new List<IHtmlTableElement> { (IHtmlTableElement)tables[tableIndex.Value] };
    }
    else
    {
        tablesToProcess = tables.Cast<IHtmlTableElement>().ToList();
    }
    
    var results = new List<TableData>();
    
    foreach (var table in tablesToProcess)
    {
        var tableData = ExtractTable(table);
        results.Add(tableData);
    }
    
    string output;
    if (format == "json")
    {
        var options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        output = JsonSerializer.Serialize(results.Count == 1 ? results[0] : results, typeof(object), options);
    }
    else if (format == "csv")
    {
        output = results.Count == 1 
            ? ToCsv(results[0]) 
            : results.Select((t, i) => $"--- Table {i + 1} ---\n{ToCsv(t)}").Aggregate((a, b) => a + "\n\n" + b);
    }
    else if (format == "markdown")
    {
        output = results.Count == 1 
            ? ToMarkdown(results[0]) 
            : results.Select((t, i) => $"### Table {i + 1}\n\n{ToMarkdown(t)}").Aggregate((a, b) => a + "\n\n" + b);
    }
    else
    {
        output = results.Count == 1 
            ? FormatAsText(results[0]) 
            : results.Select((t, i) => $"=== Table {i + 1} ===\n{FormatAsText(t)}").Aggregate((a, b) => a + "\n\n" + b);
    }
    
    if (!string.IsNullOrEmpty(outputPath))
    {
        await File.WriteAllTextAsync(outputPath, output);
        Console.WriteLine($"Output written to: {outputPath}");
    }
    else
    {
        Console.WriteLine(output);
    }
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.Exit(1);
}

static TableData ExtractTable(IHtmlTableElement table)
{
    var headers = new List<string>();
    var rows = new List<string[]>();
    
    var headerRow = table.QuerySelector("thead tr") ?? table.QuerySelector("tr");
    if (headerRow != null)
    {
        foreach (var cell in headerRow.QuerySelectorAll("th, td"))
        {
            headers.Add(cell.TextContent.Trim());
        }
    }
    
    var tbody = table.QuerySelector("tbody");
    var dataRows = tbody != null 
        ? tbody.QuerySelectorAll("tr").ToList()
        : table.QuerySelectorAll("tr").ToList();
    
    if (headerRow != null && tbody == null)
    {
        dataRows = dataRows.Skip(1).ToList();
    }
    
    foreach (var row in dataRows)
    {
        var cells = row.QuerySelectorAll("td, th").Select(c => c.TextContent.Trim()).ToArray();
        if (cells.Length > 0)
        {
            rows.Add(cells);
        }
    }
    
    return new TableData(headers.ToArray(), rows);
}

static string ToCsv(TableData table)
{
    var sb = new StringBuilder();
    
    if (table.Headers.Length > 0)
    {
        sb.AppendLine(string.Join(",", table.Headers.Select(EscapeCsv)));
    }
    
    foreach (var row in table.Rows)
    {
        sb.AppendLine(string.Join(",", row.Select(EscapeCsv)));
    }
    
    return sb.ToString();
}

static string EscapeCsv(string value)
{
    if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
    {
        return $"\"{value.Replace("\"", "\"\"")}\"";
    }
    return value;
}

static string ToMarkdown(TableData table)
{
    var sb = new StringBuilder();
    
    if (table.Headers.Length > 0)
    {
        sb.AppendLine("| " + string.Join(" | ", table.Headers) + " |");
        sb.AppendLine("|" + string.Join("|", table.Headers.Select(_ => "---")) + "|");
    }
    
    foreach (var row in table.Rows)
    {
        sb.AppendLine("| " + string.Join(" | ", row) + " |");
    }
    
    return sb.ToString();
}

static string FormatAsText(TableData table)
{
    var sb = new StringBuilder();
    var columnWidths = new int[table.Headers.Length];
    
    for (int i = 0; i < table.Headers.Length; i++)
    {
        columnWidths[i] = table.Headers[i].Length;
    }
    
    foreach (var row in table.Rows)
    {
        for (int i = 0; i < row.Length && i < columnWidths.Length; i++)
        {
            columnWidths[i] = Math.Max(columnWidths[i], row[i].Length);
        }
    }
    
    if (table.Headers.Length > 0)
    {
        sb.AppendLine(string.Join(" │ ", table.Headers.Select((h, i) => h.PadRight(columnWidths[i]))));
        sb.AppendLine(string.Join("─┼─", columnWidths.Select(w => new string('─', w))));
    }
    
    foreach (var row in table.Rows)
    {
        sb.AppendLine(string.Join(" │ ", row.Select((c, i) => c.PadRight(columnWidths[i]))));
    }
    
    return sb.ToString();
}

static (string? inputPath, string? outputPath, string format, int? tableIndex) ParseArguments(string[] args)
{
    string? inputPath = null;
    string? outputPath = null;
    string format = "text";
    int? tableIndex = null;
    
    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--json":
            case "-j":
                format = "json";
                break;
            case "--csv":
            case "-c":
                format = "csv";
                break;
            case "--markdown":
            case "-m":
                format = "markdown";
                break;
            case "--table":
            case "-t":
                if (i + 1 < args.Length && int.TryParse(args[i + 1], out var idx))
                {
                    tableIndex = idx;
                    i++;
                }
                break;
            case "--output":
            case "-o":
                if (i + 1 < args.Length)
                {
                    outputPath = args[++i];
                }
                break;
            case "--help":
            case "-h":
                ShowUsage();
                Environment.Exit(0);
                break;
            default:
                if (!args[i].StartsWith("-"))
                {
                    if (inputPath == null) inputPath = args[i];
                    else outputPath = args[i];
                }
                break;
        }
    }
    
    return (inputPath, outputPath, format, tableIndex);
}

static void ShowUsage()
{
    Console.WriteLine(@"HTML Table Extractor - Extract tables from HTML documents

Usage:
  dotnet run --project HtmlTableExtractor.csproj [options] [input] [output]

Options:
  -j, --json          Output as JSON (default: text)
  -c, --csv           Output as CSV
  -m, --markdown      Output as Markdown table
  -t, --table <n>     Extract specific table (0-indexed)
  -o, --output <file> Write output to file
  -h, --help          Show this help message

Input:
  - File path (local HTML file)
  - URL (fetches HTML from web)
  - Stdin (pipe HTML content)

Examples:
  cat page.html | dotnet run --project HtmlTableExtractor.csproj
  dotnet run --project HtmlTableExtractor.csproj -j page.html
  dotnet run --project HtmlTableExtractor.csproj -c -t 0 data.html output.csv
  dotnet run --project HtmlTableExtractor.csproj https://example.com/table.html");
}

record TableData(string[] Headers, List<string[]> Rows);
