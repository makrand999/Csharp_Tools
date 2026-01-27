using System.Text.Json;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowUsage();
    Environment.Exit(args.Contains("--help") || args.Contains("-h") ? 0 : 1);
}

var files = new List<string>();
var recursive = args.Contains("--recursive") || args.Contains("-r");
var showJson = args.Contains("--json") || args.Contains("-j");
var showTotal = args.Contains("--total") || args.Contains("-t");
var sortBy = "name";
var minHeight = 0;
var maxHeight = int.MaxValue;

for (int i = 0; i < args.Length; i++)
{
    var arg = args[i];
    
    switch (arg)
    {
        case "--sort":
            if (i + 1 < args.Length)
            {
                sortBy = args[++i].ToLowerInvariant();
            }
            break;
        case "--min-duration":
            if (i + 1 < args.Length && TimeSpan.TryParse(args[++i], out var minTs))
            {
                minHeight = (int)minTs.TotalMinutes;
            }
            break;
        case "--max-duration":
            if (i + 1 < args.Length && TimeSpan.TryParse(args[++i], out var maxTs))
            {
                maxHeight = (int)maxTs.TotalMinutes;
            }
            break;
        default:
            if (!arg.StartsWith("-"))
            {
                if (Directory.Exists(arg))
                {
                    var searchOption = recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly;
                    var videoExtensions = new[] { ".mp4", ".avi", ".mkv", ".mov", ".wmv", ".flv", ".webm", ".m4v", ".mpeg", ".mpg" };
                    
                    var dirFiles = Directory.GetFiles(arg, "*.*", searchOption)
                        .Where(f => videoExtensions.Contains(Path.GetExtension(f).ToLowerInvariant()));
                    
                    files.AddRange(dirFiles);
                }
                else if (System.IO.File.Exists(arg))
                {
                    files.Add(arg);
                }
            }
            break;
    }
}

if (files.Count == 0)
{
    Console.Error.WriteLine("Error: No video files or directories specified.");
    ShowUsage();
    Environment.Exit(1);
}

var videoInfos = new List<VideoInfo>();

foreach (var filePath in files)
{
    try
    {
        var fileInfo = new FileInfo(filePath);
        var duration = GetVideoDuration(filePath);
        
        // Filter by duration
        var durationMinutes = duration.TotalMinutes;
        if (durationMinutes < minHeight || durationMinutes > maxHeight)
        {
            continue;
        }
        
        var videoInfo = new VideoInfo
        {
            Path = Path.GetFullPath(filePath),
            FileName = Path.GetFileName(filePath),
            Directory = Path.GetDirectoryName(filePath) ?? "",
            Duration = duration,
            DurationFormatted = FormatDuration(duration),
            Size = fileInfo.Length,
            Created = fileInfo.CreationTime,
            Modified = fileInfo.LastWriteTime
        };
        
        videoInfos.Add(videoInfo);
    }
    catch (Exception ex)
    {
        Console.Error.WriteLine($"Warning: Could not process {filePath}: {ex.Message}");
    }
}

// Sort results
videoInfos = sortBy switch
{
    "duration" or "d" => videoInfos.OrderBy(v => v.Duration).ToList(),
    "duration_desc" or "dd" => videoInfos.OrderByDescending(v => v.Duration).ToList(),
    "size" or "s" => videoInfos.OrderBy(v => v.Size).ToList(),
    "size_desc" or "sd" => videoInfos.OrderByDescending(v => v.Size).ToList(),
    "date" or "t" => videoInfos.OrderBy(v => v.Created).ToList(),
    _ => videoInfos.OrderBy(v => v.FileName).ToList()
};

if (showJson)
{
    var output = new
    {
        TotalFiles = videoInfos.Count,
        TotalDuration = videoInfos.Sum(v => v.Duration.TotalSeconds),
        TotalSize = videoInfos.Sum(v => v.Size),
        Videos = videoInfos.Select(v => new
        {
            v.FileName,
            v.Path,
            v.DurationFormatted,
            v.Size,
            v.Created,
            v.Modified
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
    Console.WriteLine($"\n{'='*80}");
    Console.WriteLine($"Video Duration Extractor");
    Console.WriteLine($"{'='*80}\n");
    
    if (videoInfos.Count == 0)
    {
        Console.WriteLine("No video files found.");
        Environment.Exit(0);
    }
    
    // Print table header
    Console.WriteLine($"{"File",-50} {"Duration",-10} {"Size",-10} {"Created",-12}");
    Console.WriteLine($"{"",-50} {"",-10} {"",-10} {"",-12}");
    
    foreach (var video in videoInfos)
    {
        var displayPath = video.FileName.Length > 48 ? video.FileName[..45] + "..." : video.FileName;
        Console.WriteLine($"{displayPath,-50} {video.DurationFormatted,-10} {FormatSize(video.Size),-10} {video.Created:yyyy-MM-dd,-12}");
    }

    if (showTotal)
    {
        var totalDuration = videoInfos.Aggregate(TimeSpan.Zero, (acc, v) => acc + v.Duration);
        var totalSize = videoInfos.Sum(v => v.Size);

        Console.WriteLine($"\n{'='*80}");
        Console.WriteLine($"Totals:");
        Console.WriteLine($"  Files: {videoInfos.Count}");
        Console.WriteLine($"  Total Duration: {FormatDuration(totalDuration)} ({totalDuration.TotalHours:F2} hours)");
        Console.WriteLine($"  Total Size: {FormatSize(totalSize)}");
        
        if (videoInfos.Any(v => v.Duration > TimeSpan.FromHours(1)))
        {
            var longVideos = videoInfos.Where(v => v.Duration > TimeSpan.FromHours(1)).ToList();
            Console.WriteLine($"\n  Videos > 1 hour: {longVideos.Count}");
            foreach (var v in longVideos)
            {
                Console.WriteLine($"    - {v.FileName} ({v.DurationFormatted})");
            }
        }
    }
    
    Console.WriteLine($"\n{'='*80}");
}

static TimeSpan GetVideoDuration(string filePath)
{
    // Try to read duration from file metadata (Windows only)
    if (OperatingSystem.IsWindows())
    {
        try
        {
            var shellType = Type.GetTypeFromProgID("Shell.Application");
            if (shellType != null)
            {
                var shell = Activator.CreateInstance(shellType);
                if (shell != null)
                {
                    var folder = shellType.InvokeMember("NameSpace", System.Reflection.BindingFlags.InvokeMethod, null, shell, new object[] { Path.GetDirectoryName(filePath) });
                    if (folder != null)
                    {
                        var folderItem = folder.GetType().InvokeMember("ParseName", System.Reflection.BindingFlags.InvokeMethod, null, folder, new object[] { Path.GetFileName(filePath) });
                        if (folderItem != null)
                        {
                            // Property 279 is "Length" for media files
                            var durationObj = folder.GetType().InvokeMember("GetDetailsOf", System.Reflection.BindingFlags.InvokeMethod, null, folder, new object[] { folderItem, 279 });
                            if (durationObj is string durationStr && !string.IsNullOrEmpty(durationStr))
                            {
                                if (TimeSpan.TryParse(durationStr, out var duration))
                                {
                                    return duration;
                                }
                            }
                        }
                    }
                }
            }
        }
        catch
        {
            // Fall back to file size-based estimation
        }
    }
    
    // Estimate duration based on file size and typical bitrates
    // This is a rough estimation: assume 1 MB ≈ 10 seconds of 720p video
    var fileSize = new FileInfo(filePath).Length;
    var estimatedSeconds = (fileSize / (1024 * 1024)) * 10;
    return TimeSpan.FromSeconds(estimatedSeconds);
}

static string FormatDuration(TimeSpan duration)
{
    if (duration.TotalHours >= 1)
    {
        return duration.ToString(@"h\:mm\:ss");
    }
    return duration.ToString(@"mm\:ss");
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
    Console.WriteLine(@"Video Duration Extractor - Extract duration and metadata from video files

Usage:
  dotnet run --project VideoDuration.csproj [options] <file|directory> ...

Options:
  -r, --recursive         Search directories recursively
  -j, --json              Output as JSON
  -t, --total             Show totals (duration, size)
  --sort <field>          Sort by: name, duration, size, date (default: name)
  --min-duration <time>   Filter videos shorter than specified (e.g., 5:00, 1:30:00)
  --max-duration <time>   Filter videos longer than specified
  -h, --help              Show this help message

Supported Formats:
  MP4, AVI, MKV, MOV, WMV, FLV, WebM, M4V, MPEG

Examples:
  dotnet run --project VideoDuration.csproj video.mp4
  dotnet run --project VideoDuration.csproj ~/Videos
  dotnet run --project VideoDuration.csproj -r ~/Movies
  dotnet run --project VideoDuration.csproj --total --sort duration ~/Videos
  dotnet run --project VideoDuration.csproj --json --total ~/Videos
  dotnet run --project VideoDuration.csproj --min-duration 10:00 ~/Videos");
}

class VideoInfo
{
    public string Path { get; set; } = "";
    public string FileName { get; set; } = "";
    public string Directory { get; set; } = "";
    public TimeSpan Duration { get; set; }
    public string DurationFormatted { get; set; } = "";
    public long Size { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
}
