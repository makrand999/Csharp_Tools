using Markdig;

Console.WriteLine("Markdown to HTML Converter");
Console.WriteLine("==========================\n");

Console.WriteLine("Options:");
Console.WriteLine("  1. Convert markdown file to HTML");
Console.WriteLine("  2. Convert markdown text (inline) to HTML");
Console.WriteLine();
Console.Write("Choose option (1 or 2): ");
var option = Console.ReadLine()?.Trim();

string markdown;

if (option == "1")
{
    Console.Write("Enter path to markdown file: ");
    var filePath = Console.ReadLine()?.Trim();
    
    if (string.IsNullOrEmpty(filePath) || !File.Exists(filePath))
    {
        Console.WriteLine("Error: Invalid file path.");
        return;
    }
    
    markdown = await File.ReadAllTextAsync(filePath);
    Console.WriteLine($"\n✅ Loaded: {Path.GetFileName(filePath)} ({markdown.Length} characters)");
}
else
{
    Console.WriteLine("\nEnter markdown text (type '---' on a new line to finish):");
    var lines = new List<string>();
    string? line;
    
    while ((line = Console.ReadLine()) != "---")
    {
        lines.Add(line ?? "");
    }
    
    markdown = string.Join("\n", lines);
}

if (string.IsNullOrWhiteSpace(markdown))
{
    Console.WriteLine("Error: No markdown content provided.");
    return;
}

// Configure pipeline with common extensions
var pipeline = new MarkdownPipelineBuilder()
    .UseAdvancedExtensions()
    .UseEmojiAndSmiley()
    .UseAutoLinks()
    .Build();

var html = Markdown.ToHtml(markdown, pipeline);

Console.WriteLine("\n--- HTML Output ---\n");
Console.WriteLine(html);
Console.WriteLine("--- End HTML ---");

// Option to save to file
Console.WriteLine("\nSave HTML to file? (y/n): ");
var saveOption = Console.ReadLine()?.Trim().ToLower();

if (saveOption == "y" || saveOption == "yes")
{
    Console.Write("Enter output file path: ");
    var outputPath = Console.ReadLine()?.Trim();
    
    if (!string.IsNullOrEmpty(outputPath))
    {
        if (!outputPath.EndsWith(".html"))
            outputPath += ".html";
        
        await File.WriteAllTextAsync(outputPath, html);
        Console.WriteLine($"✅ Saved to: {outputPath}");
    }
}

Console.WriteLine("\n✅ Done!");
