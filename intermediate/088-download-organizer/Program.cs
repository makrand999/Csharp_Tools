using System.Text.Json;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowUsage();
    Environment.Exit(args.Contains("--help") || args.Contains("-h") ? 0 : 1);
}

var sourcePath = args.FirstOrDefault(a => !a.StartsWith("-"));
var dryRun = args.Contains("--dry-run") || args.Contains("-n");
var showJson = args.Contains("--json") || args.Contains("-j");
var deleteEmpty = args.Contains("--delete-empty") || args.Contains("-d");
var minAge = 0;

// Parse age option
for (int i = 0; i < args.Length; i++)
{
    if ((args[i] == "--min-age" || args[i] == "-a") && i + 1 < args.Length)
    {
        if (int.TryParse(args[++i], out var days))
        {
            minAge = days;
        }
    }
}

if (string.IsNullOrEmpty(sourcePath))
{
    // Default to Downloads folder
    var home = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile);
    sourcePath = Path.Combine(home, "Downloads");
    
    if (!Directory.Exists(sourcePath))
    {
        Console.Error.WriteLine("Error: Could not find Downloads folder. Please specify a path.");
        ShowUsage();
        Environment.Exit(1);
    }
}

if (!Directory.Exists(sourcePath))
{
    Console.Error.WriteLine($"Error: Directory not found: {sourcePath}");
    Environment.Exit(1);
}

// File type categories
var categories = new Dictionary<string, HashSet<string>>(StringComparer.OrdinalIgnoreCase)
{
    { "Installers", new HashSet<string> { ".exe", ".msi", ".dmg", ".pkg", ".deb", ".rpm", ".appimage" } },
    { "Archives", new HashSet<string> { ".zip", ".rar", ".7z", ".tar", ".gz", ".bz2", ".xz", ".tgz" } },
    { "Documents", new HashSet<string> { ".pdf", ".doc", ".docx", ".xls", ".xlsx", ".ppt", ".pptx", ".odt", ".ods", ".odp" } },
    { "Text", new HashSet<string> { ".txt", ".md", ".rtf", ".tex" } },
    { "Images", new HashSet<string> { ".jpg", ".jpeg", ".png", ".gif", ".bmp", ".svg", ".webp", ".ico", ".tiff" } },
    { "Videos", new HashSet<string> { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".m4v" } },
    { "Audio", new HashSet<string> { ".mp3", ".wav", ".flac", ".aac", ".ogg", ".wma", ".m4a" } },
    { "Code", new HashSet<string> { ".cs", ".csproj", ".js", ".ts", ".py", ".java", ".cpp", ".h", ".hpp", ".cshtml", ".razor" } },
    { "Web", new HashSet<string> { ".html", ".htm", ".css", ".scss", ".less", ".json", ".xml", ".yaml", ".yml" } },
    { "Fonts", new HashSet<string> { ".ttf", ".otf", ".woff", ".woff2", ".eot" } },
    { "Torrents", new HashSet<string> { ".torrent" } },
    { "Disk Images", new HashSet<string> { ".iso", ".img", ".vmdk", ".vdi" } },
};

var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.TopDirectoryOnly)
    .Where(f => !f.StartsWith(Path.Combine(sourcePath, ".")))
    .ToList();

// Filter by age if specified
if (minAge > 0)
{
    var cutoffDate = DateTime.Now.AddDays(-minAge);
    files = files.Where(f => new FileInfo(f).CreationTime < cutoffDate).ToList();
}

if (files.Count == 0)
{
    Console.WriteLine("No files to organize.");
    if (minAge > 0)
    {
        Console.WriteLine($"(Filtering files older than {minAge} days)");
    }
    Environment.Exit(0);
}

var organizedFiles = new Dictionary<string, List<FileMove>>();
var moves = new List<FileMove>();
var unknownFiles = new List<FileInfo>();

foreach (var file in files)
{
    var fileInfo = new FileInfo(file);
    var ext = fileInfo.Extension.ToLowerInvariant();
    
    string? targetFolder = null;
    
    foreach (var category in categories)
    {
        if (category.Value.Contains(ext))
        {
            targetFolder = category.Key;
            break;
        }
    }
    
    if (targetFolder == null)
    {
        unknownFiles.Add(fileInfo);
        continue;
    }
    
    var targetPath = Path.Combine(sourcePath, targetFolder, fileInfo.Name);
    
    // Handle duplicates
    if (File.Exists(targetPath))
    {
        var baseName = Path.GetFileNameWithoutExtension(fileInfo.Name);
        var counter = 1;
        while (File.Exists(targetPath))
        {
            targetPath = Path.Combine(sourcePath, targetFolder, $"{baseName}_{counter}{fileInfo.Extension}");
            counter++;
        }
    }
    
    var fileMove = new FileMove
    {
        Source = file,
        Target = targetPath,
        Size = fileInfo.Length,
        Category = targetFolder,
        Age = DateTime.Now - fileInfo.CreationTime
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
        SourceDirectory = Path.GetFullPath(sourcePath),
        TotalFiles = files.Count,
        OrganizedFiles = moves.Count,
        UnknownFiles = unknownFiles.Count,
        Categories = organizedFiles.Select(kvp => new
        {
            Category = kvp.Key,
            FileCount = kvp.Value.Count,
            TotalSize = kvp.Value.Sum(f => f.Size),
            Files = kvp.Value.Select(f => new
            {
                f.Source,
                f.Target,
                f.Size,
                AgeDays = f.Age.TotalDays
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
    Console.WriteLine($"\n{'='*70}");
    Console.WriteLine($"Download Folder Organizer");
    Console.WriteLine($"Source: {Path.GetFullPath(sourcePath)}");
    Console.WriteLine($"{'='*70}\n");
    
    if (dryRun)
    {
        Console.WriteLine("[DRY RUN - No files will be moved]\n");
    }
    
    if (minAge > 0)
    {
        Console.WriteLine($"Filter: Files older than {minAge} days\n");
    }
    
    foreach (var category in organizedFiles.OrderBy(k => k.Key))
    {
        var totalSize = category.Value.Sum(f => f.Size);
        
        Console.WriteLine($"\n📁 {category.Key}/ ({category.Value.Count} files, {FormatSize(totalSize)})");
        
        foreach (var move in category.Value.OrderBy(f => f.Size).Reverse())
        {
            var sourceName = Path.GetFileName(move.Source);
            var ageDays = move.Age.TotalDays;
            var ageStr = ageDays < 1 ? "today" : ageDays < 2 ? "yesterday" : $"{(int)ageDays}d ago";
            
            Console.WriteLine($"  {sourceName,-45} {FormatSize(move.Size),-10} {ageStr}");
        }
    }
    
    if (unknownFiles.Count > 0)
    {
        Console.WriteLine($"\n⚠️  Unknown file types ({unknownFiles.Count} files):");
        foreach (var file in unknownFiles.Take(10))
        {
            Console.WriteLine($"  {file.Name,-45} {FormatSize(file.Length),-10} ({file.Extension})");
        }
        if (unknownFiles.Count > 10)
        {
            Console.WriteLine($"  ... and {unknownFiles.Count - 10} more");
        }
    }
    
    Console.WriteLine($"\n{'='*70}");
    Console.WriteLine($"Summary:");
    Console.WriteLine($"  Total files: {files.Count}");
    Console.WriteLine($"  Organized: {moves.Count}");
    Console.WriteLine($"  Unknown types: {unknownFiles.Count}");
    Console.WriteLine($"  Categories: {organizedFiles.Count}");
    
    if (dryRun)
    {
        Console.WriteLine("\nRun without --dry-run to execute the moves.");
    }
    else
    {
        Console.Write("\nProceed with organization? (y/N): ");
        var response = Console.ReadLine();
        
        if (response?.ToLowerInvariant() != "y" && response?.ToLowerInvariant() != "yes")
        {
            Console.WriteLine("Cancelled.");
            Environment.Exit(0);
        }
        
        Console.WriteLine($"\nOrganizing downloads...\n");
        
        foreach (var category in organizedFiles.Keys)
        {
            var folderPath = Path.Combine(sourcePath, category);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                Console.WriteLine($"Created: {category}/");
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
        
        // Clean up empty directories if requested
        if (deleteEmpty)
        {
            Console.WriteLine("\nCleaning up empty directories...");
            var emptyDirs = Directory.GetDirectories(sourcePath, "*", SearchOption.AllDirectories)
                .OrderByDescending(d => d.Length)
                .Where(d => !Directory.EnumerateFileSystemEntries(d).Any());
            
            foreach (var dir in emptyDirs)
            {
                try
                {
                    Directory.Delete(dir);
                    Console.WriteLine($"Removed: {dir.Replace(sourcePath, ".")}");
                }
                catch { }
            }
        }
        
        Console.WriteLine($"\n{'='*70}");
        Console.WriteLine($"Completed: {moved} moved, {errors} errors");
    }
}

static string FormatSize(long bytes)
{
    return bytes switch
    {
        < 1024 => $"{bytes} B",
        < 1024 * 1024 => $"{bytes / 1024.0:F0} KB",
        < 1024 * 1024 * 1024 => $"{bytes / (1024.0 * 1024):F1} MB",
        _ => $"{bytes / (1024.0 * 1024 * 1024):F2} GB"
    };
}

static void ShowUsage()
{
    Console.WriteLine(@"Download Folder Organizer - Organize downloaded files by type

Usage:
  dotnet run --project DownloadOrganizer.csproj [options] [download-folder]

Options:
  -n, --dry-run         Show what would be done without moving files
  -j, --json            Output as JSON
  -a, --min-age <days>  Only organize files older than N days
  -d, --delete-empty    Delete empty directories after organizing
  -h, --help            Show this help message

If no folder is specified, uses ~/Downloads by default.

Categories:
  Installers  - .exe, .msi, .dmg, .deb, etc.
  Archives    - .zip, .rar, .7z, .tar, .gz, etc.
  Documents   - .pdf, .doc, .docx, .xls, .xlsx, etc.
  Images      - .jpg, .png, .gif, .svg, .webp, etc.
  Videos      - .mp4, .avi, .mkv, .mov, .webm, etc.
  Audio       - .mp3, .wav, .flac, .aac, .ogg, etc.
  Code        - .cs, .js, .ts, .py, .java, .cpp, etc.
  Web         - .html, .css, .json, .xml, .yaml, etc.
  Fonts       - .ttf, .otf, .woff, .woff2
  Torrents    - .torrent
  Disk Images - .iso, .img, .vmdk

Examples:
  dotnet run --project DownloadOrganizer.csproj
  dotnet run --project DownloadOrganizer.csproj ~/Downloads
  dotnet run --project DownloadOrganizer.csproj --dry-run ~/Downloads
  dotnet run --project DownloadOrganizer.csproj --min-age 7 ~/Downloads
  dotnet run --project DownloadOrganizer.csproj --delete-empty ~/Downloads");
}

class FileMove
{
    public string Source { get; set; } = "";
    public string Target { get; set; } = "";
    public long Size { get; set; }
    public string Category { get; set; } = "";
    public TimeSpan Age { get; set; }
}
