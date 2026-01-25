using System.Text.Json;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowUsage();
    Environment.Exit(args.Contains("--help") || args.Contains("-h") ? 0 : 1);
}

var sourcePath = args.FirstOrDefault(a => !a.StartsWith("-"));
var strategy = "extension";
var dryRun = args.Contains("--dry-run") || args.Contains("-n");
var showJson = args.Contains("--json") || args.Contains("-j");
var createSubdirs = args.Contains("--subdirs") || args.Contains("-s");

// Parse strategy option
for (int i = 0; i < args.Length; i++)
{
    if ((args[i] == "--by" || args[i] == "-b") && i + 1 < args.Length)
    {
        strategy = args[++i].ToLowerInvariant();
    }
}

if (string.IsNullOrEmpty(sourcePath))
{
    Console.Error.WriteLine("Error: No source directory specified.");
    ShowUsage();
    Environment.Exit(1);
}

if (!Directory.Exists(sourcePath))
{
    Console.Error.WriteLine($"Error: Directory not found: {sourcePath}");
    Environment.Exit(1);
}

var validStrategies = new[] { "extension", "date", "size", "type" };
if (!validStrategies.Contains(strategy))
{
    Console.Error.WriteLine($"Error: Invalid strategy '{strategy}'. Valid options: {string.Join(", ", validStrategies)}");
    Environment.Exit(1);
}

var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly)
    .Where(f => !f.StartsWith(Path.Combine(sourcePath, ".")))
    .ToList();

if (files.Count == 0)
{
    Console.WriteLine("No files to organize.");
    Environment.Exit(0);
}

var organizedFiles = new Dictionary<string, List<FileMove>>();
var moves = new List<FileMove>();

foreach (var file in files)
{
    var fileInfo = new FileInfo(file);
    string targetFolder;
    
    switch (strategy)
    {
        case "extension":
            targetFolder = GetExtensionFolder(fileInfo);
            break;
        case "date":
            targetFolder = GetDateFolder(fileInfo);
            break;
        case "size":
            targetFolder = GetSizeFolder(fileInfo);
            break;
        case "type":
            targetFolder = GetTypeFolder(fileInfo);
            break;
        default:
            targetFolder = "Other";
            break;
    }
    
    var targetPath = Path.Combine(sourcePath, targetFolder, fileInfo.Name);
    
    // Handle duplicates
    if (File.Exists(targetPath))
    {
        var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);
        var ext = fileInfo.Extension;
        var counter = 1;
        while (File.Exists(targetPath))
        {
            targetPath = Path.Combine(sourcePath, targetFolder, $"{baseName}_{counter}{ext}");
            counter++;
        }
    }
    
    var fileMove = new FileMove
    {
        Source = file,
        Target = targetPath,
        Size = fileInfo.Length
    };
    
    moves.Add(fileMove);
    
    if (!organizedFiles.ContainsKey(targetFolder))
    {
        organizedFiles[targetFolder] = new List<FileMove>();
    }
    organizedFiles[targetFolder].Add(fileMove);
}

if (showJson)
{
    var output = new
    {
        Strategy = strategy,
        SourceDirectory = Path.GetFullPath(sourcePath),
        TotalFiles = files.Count,
        Folders = organizedFiles.Select(kvp => new
        {
            Folder = kvp.Key,
            FileCount = kvp.Value.Count,
            Files = kvp.Value.Select(f => new
            {
                f.Source,
                f.Target,
                f.Size
            })
        })
    };
    
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    Console.WriteLine(JsonSerializer.Serialize(output, options));
}
else
{
    Console.WriteLine($"\n{'='*60}");
    Console.WriteLine($"Batch File Organizer");
    Console.WriteLine($"Strategy: {strategy}");
    Console.WriteLine($"Source: {Path.GetFullPath(sourcePath)}");
    Console.WriteLine($"{'='*60}\n");
    
    if (dryRun)
    {
        Console.WriteLine("[DRY RUN - No files will be moved]\n");
    }
    
    foreach (var folder in organizedFiles.OrderBy(k => k.Key))
    {
        Console.WriteLine($"\n📁 {folder.Key}/ ({folder.Value.Count} files)");
        
        foreach (var move in folder.Value)
        {
            var sourceName = Path.GetFileName(move.Source);
            var targetName = Path.GetFileName(move.Target);
            var sizeStr = FormatSize(move.Size);
            
            if (sourceName == targetName)
            {
                Console.WriteLine($"  {sourceName,-40} ({sizeStr})");
            }
            else
            {
                Console.WriteLine($"  {sourceName,-40} → {targetName} ({sizeStr})");
            }
        }
    }
    
    Console.WriteLine($"\n{'='*60}");
    Console.WriteLine($"Total: {files.Count} files in {organizedFiles.Count} folders");
    
    if (dryRun)
    {
        Console.WriteLine("\nRun without --dry-run to execute the moves.");
    }
    else
    {
        Console.WriteLine($"\nProceeding with organization...\n");
        
        foreach (var folder in organizedFiles.Keys)
        {
            var folderPath = Path.Combine(sourcePath, folder);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"Created: {folder}/");
            }
        }
        
        int moved = 0, errors = 0;
        
        foreach (var move in moves)
        {
            try
            {
                File.Move(move.Source, move.Target);
                Console.WriteLine($"✓ {Path.GetFileName(move.Source)}");
                moved++;
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"✗ {Path.GetFileName(move.Source)}: {ex.Message}");
                errors++;
            }
        }
        
        Console.WriteLine($"\n{'='*60}");
        Console.WriteLine($"Completed: {moved} moved, {errors} errors");
    }
}

static string GetExtensionFolder(FileInfo file)
{
    var ext = file.Extension.TrimStart('.').ToLowerInvariant();
    return string.IsNullOrEmpty(ext) ? "No Extension" : ext.ToUpperInvariant();
}

static string GetDateFolder(FileInfo file)
{
    var date = file.CreationTime;
    return $"{date.Year}/{date.Month:D2}_{date:MMMM}";
}

static string GetSizeFolder(FileInfo file)
{
    return file.Length switch
    {
        < 1024 => "Tiny (< 1KB)",
        < 1024 * 1024 => "Small (1KB - 1MB)",
        < 10 * 1024 * 1024 => "Medium (1MB - 10MB)",
        < 100 * 1024 * 1024 => "Large (10MB - 100MB)",
        _ => "Huge (> 100MB)"
    };
}

static string GetTypeFolder(FileInfo file)
{
    var ext = file.Extension.ToLowerInvariant();
    
    return ext switch
    {
        ".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".svg" or ".webp" or ".ico" => "Images",
        ".mp4" or ".avi" or ".mkv" or ".mov" or ".wmv" or ".flv" or ".webm" => "Videos",
        ".mp3" or ".wav" or ".flac" or ".aac" or ".ogg" or ".wma" => "Audio",
        ".pdf" or ".doc" or ".docx" or ".xls" or ".xlsx" or ".ppt" or ".pptx" or ".odt" => "Documents",
        ".txt" or ".md" or ".rtf" => "Text",
        ".zip" or ".rar" or ".7z" or ".tar" or ".gz" or ".bz2" => "Archives",
        ".exe" or ".dll" or ".msi" or ".app" or ".dmg" => "Executables",
        ".cs" or ".csproj" or ".js" or ".ts" or ".py" or ".java" or ".cpp" or ".h" => "Code",
        ".html" or ".css" or ".scss" or ".less" => "Web",
        ".json" or ".xml" or ".yaml" or ".yml" or ".toml" => "Config",
        _ => "Other"
    };
}

static string FormatSize(long bytes)
{
    return bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F1} KB",
        < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024)} MB",
        _ => $"{bytes / (1024.0 * 1024 * 1024)} GB"
    };
}

static void ShowUsage()
{
    Console.WriteLine(@"Batch File Organizer - Organize files into folders

Usage:
  dotnet run --project BatchFileOrganizer.csproj [options] <source-directory>

Options:
  -b, --by <strategy>  Organization strategy: extension, date, size, type (default: extension)
  -n, --dry-run        Show what would be done without moving files
  -j, --json           Output as JSON
  -s, --subdirs        Also process subdirectories
  -h, --help           Show this help message

Strategies:
  extension  - Group by file extension (JPG, PDF, DOCX, etc.)
  date       - Group by creation date (YYYY/MM_Month)
  size       - Group by file size (Tiny, Small, Medium, Large, Huge)
  type       - Group by file type (Images, Videos, Documents, Code, etc.)

Examples:
  dotnet run --project BatchFileOrganizer.csproj ~/Downloads
  dotnet run --project BatchFileOrganizer.csproj --by type ~/Downloads
  dotnet run --project BatchFileOrganizer.csproj --by date --dry-run ~/Downloads
  dotnet run --project BatchFileOrganizer.csproj -b size -j ~/Documents");
}

class FileMove
{
    public string Source { get; set; } = "";
    public string Target { get; set; } = "";
    public long Size { get; set; }
}
