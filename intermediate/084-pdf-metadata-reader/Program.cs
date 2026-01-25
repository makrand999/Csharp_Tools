using System.Text;
using System.Text.Json;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowUsage();
    Environment.Exit(args.Contains("--help") || args.Contains("-h") ? 0 : 1);
}

var files = args.Where(a => !a.StartsWith("-")).ToList();
var showJson = args.Contains("--json") || args.Contains("-j");
var showAll = args.Contains("--all") || args.Contains("-a");

if (files.Count == 0)
{
    Console.Error.WriteLine("Error: No PDF files specified.");
    ShowUsage();
    Environment.Exit(1);
}

var results = new List<object>();

foreach (var filePath in files)
{
    if (!File.Exists(filePath))
    {
        Console.Error.WriteLine($"Error: File not found: {filePath}");
        continue;
    }
    
    try
    {
        using var document = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
        var metadata = ExtractMetadata(document, showAll);
        
        if (showJson)
        {
            results.Add(new
            {
                File = Path.GetFileName(filePath),
                Path = Path.GetFullPath(filePath),
                Metadata = metadata
            });
        }
        else
        {
            Console.WriteLine($"\n{'='*60}");
            Console.WriteLine($"File: {Path.GetFileName(filePath)}");
            Console.WriteLine($"Path: {Path.GetFullPath(filePath)}");
            Console.WriteLine($"{'='*60}");
            PrintMetadata(metadata);
        }
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Error reading {filePath}: {ex.Message}");
    }
}

if (showJson)
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    if (results.Count == 1)
    {
        Console.WriteLine(JsonSerializer.Serialize(results[0], options));
    }
    else if (results.Count > 1)
    {
        Console.WriteLine(JsonSerializer.Serialize(results, options));
    }
}

static Dictionary<string, object?> ExtractMetadata(PdfDocument document, bool showAll)
{
    var metadata = new Dictionary<string, object?>();
    var info = document.Info;
    
    // Standard PDF metadata fields
    if (!string.IsNullOrEmpty(info.Title) || showAll)
        metadata["Title"] = string.IsNullOrEmpty(info.Title) ? null : info.Title;
    if (!string.IsNullOrEmpty(info.Author) || showAll)
        metadata["Author"] = string.IsNullOrEmpty(info.Author) ? null : info.Author;
    if (!string.IsNullOrEmpty(info.Subject) || showAll)
        metadata["Subject"] = string.IsNullOrEmpty(info.Subject) ? null : info.Subject;
    if (!string.IsNullOrEmpty(info.Keywords) || showAll)
        metadata["Keywords"] = string.IsNullOrEmpty(info.Keywords) ? null : info.Keywords;
    if (!string.IsNullOrEmpty(info.Creator) || showAll)
        metadata["Creator"] = string.IsNullOrEmpty(info.Creator) ? null : info.Creator;
    if (!string.IsNullOrEmpty(info.Producer) || showAll)
        metadata["Producer"] = string.IsNullOrEmpty(info.Producer) ? null : info.Producer;
    
    // Date fields - PdfSharp uses DateTime
    if (info.CreationDate != DateTime.MinValue || showAll)
        metadata["Creation Date"] = info.CreationDate == DateTime.MinValue ? null : info.CreationDate.ToString("yyyy-MM-dd HH:mm:ss");
    if (info.ModificationDate != DateTime.MinValue || showAll)
        metadata["Modified Date"] = info.ModificationDate == DateTime.MinValue ? null : info.ModificationDate.ToString("yyyy-MM-dd HH:mm:ss");
    
    // Document statistics
    metadata["Pages"] = document.PageCount;
    metadata["PDF Version"] = document.Version.ToString();
    
    return metadata;
}

static void PrintMetadata(Dictionary<string, object?> metadata)
{
    foreach (var kvp in metadata)
    {
        if (kvp.Value != null)
        {
            Console.WriteLine($"{kvp.Key,-20}: {kvp.Value}");
        }
    }
}

static void ShowUsage()
{
    Console.WriteLine(@"PDF Metadata Reader - Extract metadata from PDF files

Usage:
  dotnet run --project PdfMetadataReader.csproj [options] <file1.pdf> [file2.pdf] ...

Options:
  -j, --json    Output as JSON
  -a, --all     Show all fields (including empty ones)
  -h, --help    Show this help message

Examples:
  dotnet run --project PdfMetadataReader.csproj document.pdf
  dotnet run --project PdfMetadataReader.csproj -j report.pdf
  dotnet run --project PdfMetadataReader.csproj --all *.pdf
  dotnet run --project PdfMetadataReader.csproj file1.pdf file2.pdf file3.pdf");
}
