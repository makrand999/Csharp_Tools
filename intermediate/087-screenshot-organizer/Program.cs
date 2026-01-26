using System.Text.RegularExpressions;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowUsage();
    Environment.Exit(args.Contains("--help") || args.Contains("-h") ? 0 : 1);
}

var sourcePath = args.FirstOrDefault(a => !a.StartsWith("-"));
var strategy = "date";
var dryRun = args.Contains("--dry-run") || args.Contains("-n");
var recursive = args.Contains("--recursive") || args.Contains("-r");

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

var screenshotExtensions = new[] { ".png", ".jpg", ".jpeg", ".bmp", ".gif", ".webp" };
var screenshotPatterns = new[]
{
    @"screenshot", @"screenshot_\d+", @"Screenshot_\d+", @"Screenshot \d+",
    @"snip", @"snipping", @"Snip", @"Snipping",
    @"^\d{4}-\d{2}-\d{2}", @"^\d{8}_", @"^\d{4}\d{2}\d{2}",
    @"_screenshot", @"-screenshot", @"\.screenshot",
    @"^IMG", @"^DSC", @"^SCR"
};

var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
var allFiles = Directory.GetFiles(sourcePath, "*.*", searchOption);

var screenshots = allFiles
    .Where(f => screenshotExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()))
    .Where(f =>
    {
        var fileName = Path.GetFileName(f);
        var directoryName = Path.GetFileName(Path.GetDirectoryName(f) ?? "");
        return screenshotPatterns.Any(p => Regex.IsMatch(fileName, p, RegexOptions.IgnoreCase)) ||
               directoryName.Equals("screenshots", StringComparison.OrdinalIgnoreCase);
    })
    .ToList();

if (screenshots.Count == 0)
{
    Console.WriteLine("No screenshots found in the specified directory.");
    Console.WriteLine("\nLooking for files matching patterns like:");
    Console.WriteLine("  - Screenshot_*.png, screenshot_*.jpg");
    Console.WriteLine("  - Snip_*.png, Snipping_*.png");
    Console.WriteLine("  - 2024-01-15_*.png, 20240115_*.png");
    Console.WriteLine("  - IMG_*.jpg, DSC_*.jpg");
    Environment.Exit(0);
}

var organizedScreenshots = new Dictionary<string, List<ScreenshotMove>>();
var moves = new List<ScreenshotMove>();

foreach (var file in screenshots)
{
    var fileInfo = new FileInfo(file);
    string targetFolder;
    
    switch (strategy)
    {
        case "date":
            targetFolder = GetDateFolder(fileInfo);
            break;
        case "app":
            targetFolder = GetAppFolder(fileInfo);
            break;
        case "resolution":
            targetFolder = GetResolutionFolder(fileInfo);
            break;
        case "month":
            targetFolder = GetMonthFolder(fileInfo);
            break;
        default:
            targetFolder = "Screenshots";
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
    
    var screenshotMove = new ScreenshotMove
    {
        Source = file,
        Target = targetPath,
        Size = fileInfo.Length,
        Date = fileInfo.CreationTime
    };
    
    moves.Add(screenshotMove);
    
    if (!organizedScreenshots.ContainsKey(targetFolder))
    {
        organizedScreenshots[targetFolder] = new List<ScreenshotMove>();
    }
    organizedScreenshots[targetFolder].Add(screenshotMove);
}

Console.WriteLine($"\n{'='*60}");
Console.WriteLine($"Screenshot Organizer");
Console.WriteLine($"Strategy: {strategy}");
Console.WriteLine($"Source: {Path.GetFullPath(sourcePath)}");
Console.WriteLine($"Found: {screenshots.Count} screenshots");
Console.WriteLine($"{'='*60}\n");

if (dryRun)
{
    Console.WriteLine("[DRY RUN - No files will be moved]\n");
}

foreach (var folder in organizedScreenshots.OrderBy(k => k.Key))
{
    Console.WriteLine($"\n📁 {folder.Key}/ ({folder.Value.Count} files)");
    
    foreach (var move in folder.Value)
    {
        var sourceName = Path.GetFileName(move.Source);
        var sizeStr = FormatSize(move.Size);
        var dateStr = move.Date.ToString("yyyy-MM-dd HH:mm");
        
        Console.WriteLine($"  {sourceName,-50} {sizeStr,-10} {dateStr}");
    }
}

Console.WriteLine($"\n{'='*60}");
Console.WriteLine($"Total: {screenshots.Count} screenshots in {organizedScreenshots.Count} folders");

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
    
    Console.WriteLine($"\nOrganizing screenshots...\n");
    
    foreach (var folder in organizedScreenshots.Keys)
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

static string GetDateFolder(FileInfo file)
{
    var date = file.CreationTime;
    return $"Screenshots/{date:yyyy}/{date:MM}_{date:MMMM}/{date:dd}";
}

static string GetMonthFolder(FileInfo file)
{
    var date = file.CreationTime;
    return $"Screenshots/{date:yyyy-MM}_{date:MMMM}";
}

static string GetAppFolder(FileInfo file)
{
    var fileName = file.Name;
    
    // Try to extract app name from common screenshot patterns
    var patterns = new Dictionary<string, string>
    {
        { @"chrome", "Chrome" },
        { @"firefox", "Firefox" },
        { @"edge", "Edge" },
        { @"safari", "Safari" },
        { @"discord", "Discord" },
        { @"slack", "Slack" },
        { @"teams", "Teams" },
        { @"zoom", "Zoom" },
        { @"spotify", "Spotify" },
        { @"steam", "Steam" },
        { @"photoshop", "Photoshop" },
        { @"illustrator", "Illustrator" },
        { @"word", "Word" },
        { @"excel", "Excel" },
        { @"powerpoint", "PowerPoint" },
        { @"vscode", "VSCode" },
        { @"visual.studio", "Visual Studio" },
        { @"terminal", "Terminal" },
        { @"powershell", "PowerShell" },
    };
    
    foreach (var pattern in patterns)
    {
        if (Regex.IsMatch(fileName, pattern.Key, RegexOptions.IgnoreCase))
        {
            return $"Screenshots/By App/{pattern.Value}";
        }
    }
    
    // Check for window title patterns like "Screenshot_2024-01-15_Chrome"
    var match = Regex.Match(fileName, @"[_\-\s]([a-zA-Z]+)[_\-\s]?\d");
    if (match.Success)
    {
        var potentialApp = match.Groups[1].Value;
        if (potentialApp.Length > 2 && potentialApp.Length < 20)
        {
            return $"Screenshots/By App/{char.ToUpper(potentialApp[0]) + potentialApp[1..].ToLower()}";
        }
    }
    
    return "Screenshots/By App/Other";
}

static string GetResolutionFolder(FileInfo file)
{
    var fileName = file.Name;
    
    // Try to extract resolution from filename
    var resolutionPattern = @"(\d{3,4})[x_×](\d{3,4})";
    var match = Regex.Match(fileName, resolutionPattern);
    
    if (match.Success)
    {
        var width = int.Parse(match.Groups[1].Value);
        var height = int.Parse(match.Groups[2].Value);
        
        if (width >= 3840 || height >= 2160)
            return "Screenshots/By Resolution/4K (2160p)";
        if (width >= 2560 || height >= 1440)
            return "Screenshots/By Resolution/2K (1440p)";
        if (width >= 1920 || height >= 1080)
            return "Screenshots/By Resolution/Full HD (1080p)";
        if (width >= 1280 || height >= 720)
            return "Screenshots/By Resolution/HD (720p)";
        
        return $"Screenshots/By Resolution/{width}x{height}";
    }
    
    return "Screenshots/By Resolution/Unknown";
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
    Console.WriteLine(@"Screenshot Organizer - Organize screenshots into folders

Usage:
  dotnet run --project ScreenshotOrganizer.csproj [options] <source-directory>

Options:
  -b, --by <strategy>  Organization strategy: date, app, resolution, month (default: date)
  -n, --dry-run        Show what would be done without moving files
  -r, --recursive      Search subdirectories for screenshots
  -h, --help           Show this help message

Strategies:
  date        - Group by creation date (YYYY/MM_Month/DD)
  app         - Group by detected application (Chrome, Discord, etc.)
  resolution  - Group by resolution (4K, 2K, 1080p, 720p)
  month       - Group by month (YYYY-MM_Month)

Detected Patterns:
  - Screenshot_*.png, screenshot_*.jpg
  - Snip_*.png, Snipping_*.png
  - 2024-01-15_*.png, 20240115_*.png
  - IMG_*.jpg, DSC_*.jpg

Examples:
  dotnet run --project ScreenshotOrganizer.csproj ~/Pictures
  dotnet run --project ScreenshotOrganizer.csproj --by app ~/Pictures
  dotnet run --project ScreenshotOrganizer.csproj --by resolution --dry-run ~/Pictures
  dotnet run --project ScreenshotOrganizer.csproj -r ~/Pictures/Screenshots");
}

class ScreenshotMove
{
    public string Source { get; set; } = "";
    public string Target { get; set; } = "";
    public long Size { get; set; }
    public DateTime Date { get; set; }
}
