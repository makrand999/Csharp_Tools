using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;

if (args.Length < 3)
{
    ShowHelp();
    return;
}

var sourcePath = args[0];
var outputPath = args[1];
var width = 0;
var height = 0;

// Parse dimensions
for (int i = 2; i < args.Length; i++)
{
    var arg = args[i].ToLower();
    if (arg.StartsWith("-w=") && int.TryParse(arg[3..], out var w))
        width = w;
    else if (arg.StartsWith("-h=") && int.TryParse(arg[3..], out var h))
        height = h;
    else if (arg == "-r" || arg == "--recursive")
        ProcessDirectoryRecursive(sourcePath, outputPath, width, height);
}

if (width == 0 && height == 0)
{
    Console.Error.WriteLine("Error: Must specify at least -W=<width> or -H=<height>");
    return;
}

if (Directory.Exists(sourcePath))
{
    ProcessDirectory(sourcePath, outputPath, width, height);
}
else if (File.Exists(sourcePath))
{
    ProcessImage(sourcePath, outputPath, width, height);
}
else
{
    Console.Error.WriteLine($"Error: Source not found: {sourcePath}");
}

void ShowHelp()
{
    Console.WriteLine("""
        Bulk Image Resizer
        
        Usage:
          dotnet run --project BulkImageResizer/BulkImageResizer.csproj <source> <output> -W=<width> [-H=<height>] [-r]
        
        Options:
          -W=<width>    Target width in pixels
          -H=<height>   Target height in pixels (optional, maintains aspect ratio if omitted)
          -r, --recursive  Process subdirectories recursively
        
        Examples:
          # Resize single image to 800px width
          dotnet run --project BulkImageResizer/BulkImageResizer.csproj photo.jpg output/ -W=800
          
          # Resize all images in folder to 1920x1080
          dotnet run --project BulkImageResizer/BulkImageResizer.csproj images/ resized/ -W=1920 -H=1080
          
          # Recursively process directory
          dotnet run --project BulkImageResizer/BulkImageResizer.csproj photos/ output/ -W=1200 -r
        """);
}

void ProcessDirectory(string sourceDir, string outputDir, int targetWidth, int targetHeight)
{
    if (!Directory.Exists(outputDir))
        Directory.CreateDirectory(outputDir);
    
    var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
    var files = Directory.GetFiles(sourceDir)
        .Where(f => imageExtensions.Contains(Path.GetExtension(f).ToLower()))
        .ToArray();
    
    Console.WriteLine($"Processing {files.Length} images in {sourceDir}...");
    
    int success = 0, failed = 0;
    foreach (var file in files)
    {
        try
        {
            var outputFile = Path.Combine(outputDir, Path.GetFileName(file));
            ProcessImage(file, outputFile, targetWidth, targetHeight);
            success++;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Failed: {file} - {ex.Message}");
            failed++;
        }
    }
    
    Console.WriteLine($"Done: {success} succeeded, {failed} failed");
}

void ProcessDirectoryRecursive(string sourceDir, string outputDir, int targetWidth, int targetHeight)
{
    if (!Directory.Exists(outputDir))
        Directory.CreateDirectory(outputDir);
    
    var imageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".webp" };
    
    foreach (var dir in Directory.GetDirectories(sourceDir))
    {
        var subOutputDir = Path.Combine(outputDir, Path.GetFileName(dir));
        ProcessDirectoryRecursive(dir, subOutputDir, targetWidth, targetHeight);
    }
    
    ProcessDirectory(sourceDir, outputDir, targetWidth, targetHeight);
}

void ProcessImage(string inputFile, string outputFile, int targetWidth, int targetHeight)
{
    using var image = Image.Load(inputFile);
    
    var originalWidth = image.Width;
    var originalHeight = image.Height;
    
    // Calculate dimensions
    int newWidth, newHeight;
    if (targetWidth > 0 && targetHeight > 0)
    {
        newWidth = targetWidth;
        newHeight = targetHeight;
    }
    else if (targetWidth > 0)
    {
        newWidth = targetWidth;
        newHeight = (int)(originalHeight * (targetWidth / (float)originalWidth));
    }
    else
    {
        newHeight = targetHeight;
        newWidth = (int)(originalWidth * (targetHeight / (float)originalHeight));
    }
    
    image.Mutate(ctx => ctx.Resize(newWidth, newHeight));
    image.Save(outputFile);
    
    Console.WriteLine($"Resized: {Path.GetFileName(inputFile)} ({originalWidth}x{originalHeight} -> {newWidth}x{newHeight})");
}
