using System.IO.Compression;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "rotate":
        RotateLogs(args.Skip(1).ToArray());
        break;
    case "compress":
        CompressLogs(args.Skip(1).ToArray());
        break;
    case "cleanup":
        CleanupOldLogs(args.Skip(1).ToArray());
        break;
    case "status":
        ShowLogStatus(args.Skip(1).ToArray());
        break;
    default:
        RotateLogs(args);
        break;
}

void RotateLogs(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- rotate <log-file> [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  --max-size <bytes>   Rotate when file exceeds this size");
        Console.WriteLine("  --max-files <count>  Keep this many rotated files (default: 5)");
        Console.WriteLine("  --compress           Compress rotated files");
        Console.WriteLine("  --pattern <pattern>  Rotated file pattern (default: {file}.{n}.gz)");
        return;
    }

    var logFile = args[0];
    var maxSize = 10L * 1024 * 1024; // 10 MB default
    var maxFiles = 5;
    var compress = false;
    var pattern = "{file}.{n}.gz";

    for (var i = 1; i < args.Length; i++)
    {
        if (args[i] == "--max-size" && i + 1 < args.Length)
        {
            if (long.TryParse(args[++i], out var size))
                maxSize = size;
        }
        else if (args[i] == "--max-files" && i + 1 < args.Length)
        {
            if (int.TryParse(args[++i], out var count))
                maxFiles = count;
        }
        else if (args[i] == "--compress")
        {
            compress = true;
        }
        else if (args[i] == "--pattern" && i + 1 < args.Length)
        {
            pattern = args[++i];
        }
    }

    if (!File.Exists(logFile))
    {
        Console.WriteLine($"Log file not found: {logFile}");
        return;
    }

    try
    {
        var fileInfo = new FileInfo(logFile);
        
        if (fileInfo.Length < maxSize)
        {
            Console.WriteLine($"✓ No rotation needed ({fileInfo.Length:N0} bytes < {maxSize:N0} bytes)");
            return;
        }

        Console.WriteLine($"Rotating {logFile} ({fileInfo.Length:N0} bytes)...");

        // Shift existing rotated files
        ShiftRotatedFiles(logFile, maxFiles, pattern);

        // Create new rotated file
        var rotatedName = pattern.Replace("{file}", Path.GetFileNameWithoutExtension(logFile))
                                  .Replace("{n}", "1");
        var rotatedPath = Path.Combine(Path.GetDirectoryName(logFile) ?? ".", rotatedName);

        if (compress)
        {
            CompressFile(logFile, rotatedPath);
            Console.WriteLine($"  Created: {rotatedPath} (compressed)");
        }
        else
        {
            File.Move(logFile, rotatedPath);
            Console.WriteLine($"  Created: {rotatedPath}");
        }

        // Create empty log file
        File.WriteAllText(logFile, "");
        Console.WriteLine($"  Reset: {logFile}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void CompressLogs(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- compress <log-file> [output-file]");
        Console.WriteLine("   or: dotnet run -- compress <directory>");
        return;
    }

    var target = args[0];
    var outputFile = args.Length > 1 ? args[1] : null;

    if (!File.Exists(target) && !Directory.Exists(target))
    {
        Console.WriteLine($"File/directory not found: {target}");
        return;
    }

    try
    {
        if (File.Exists(target))
        {
            outputFile ??= target + ".gz";
            CompressFile(target, outputFile);
            var originalSize = new FileInfo(target).Length;
            var compressedSize = new FileInfo(outputFile).Length;
            var ratio = (1.0 - (double)compressedSize / originalSize) * 100;
            Console.WriteLine($"✓ Compressed: {Path.GetFileName(target)}");
            Console.WriteLine($"  Original: {originalSize:N0} bytes");
            Console.WriteLine($"  Compressed: {compressedSize:N0} bytes ({ratio:F1}% reduction)");
        }
        else
        {
            // Compress all .log files in directory
            var logFiles = Directory.GetFiles(target, "*.log");
            Console.WriteLine($"Compressing {logFiles.Length} log files in {target}...");

            foreach (var logFile in logFiles)
            {
                var gzFile = logFile + ".gz";
                CompressFile(logFile, gzFile);
                Console.WriteLine($"  {Path.GetFileName(logFile)} -> {Path.GetFileName(gzFile)}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void CleanupOldLogs(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- cleanup <directory> [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  --older-than <days>  Remove files older than this (default: 30)");
        Console.WriteLine("  --pattern <pattern>  File pattern (default: *.gz)");
        Console.WriteLine("  --dry-run            Show what would be deleted");
        return;
    }

    var directory = args[0];
    var olderThan = 30;
    var pattern = "*.gz";
    var dryRun = false;

    for (var i = 1; i < args.Length; i++)
    {
        if (args[i] == "--older-than" && i + 1 < args.Length)
        {
            if (int.TryParse(args[++i], out var days))
                olderThan = days;
        }
        else if (args[i] == "--pattern" && i + 1 < args.Length)
        {
            pattern = args[++i];
        }
        else if (args[i] == "--dry-run")
        {
            dryRun = true;
        }
    }

    if (!Directory.Exists(directory))
    {
        Console.WriteLine($"Directory not found: {directory}");
        return;
    }

    try
    {
        var cutoff = DateTime.Now.AddDays(-olderThan);
        var files = Directory.GetFiles(directory, pattern);
        var deleted = 0;
        var savedBytes = 0L;

        foreach (var file in files)
        {
            var fileInfo = new FileInfo(file);
            if (fileInfo.LastWriteTime < cutoff)
            {
                if (dryRun)
                {
                    Console.WriteLine($"  Would delete: {fileInfo.Name} ({fileInfo.Length:N0} bytes, {fileInfo.LastWriteTime:yyyy-MM-dd})");
                }
                else
                {
                    File.Delete(file);
                    Console.WriteLine($"  Deleted: {fileInfo.Name}");
                }
                deleted++;
                savedBytes += fileInfo.Length;
            }
        }

        if (dryRun)
        {
            Console.WriteLine($"\nDry run: {deleted} file(s) would be deleted ({savedBytes:N0} bytes)");
        }
        else
        {
            Console.WriteLine($"\n✓ Cleaned up {deleted} old log file(s) ({savedBytes:N0} bytes freed)");
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ShowLogStatus(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- status <directory>");
        return;
    }

    var directory = args[0];
    if (!Directory.Exists(directory))
    {
        Console.WriteLine($"Directory not found: {directory}");
        return;
    }

    try
    {
        var logFiles = Directory.GetFiles(directory, "*.log");
        var gzFiles = Directory.GetFiles(directory, "*.gz");

        var totalLogSize = logFiles.Sum(f => new FileInfo(f).Length);
        var totalGzSize = gzFiles.Sum(f => new FileInfo(f).Length);

        Console.WriteLine($"Log Status: {directory}");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"Active log files: {logFiles.Length} ({totalLogSize:N0} bytes)");
        Console.WriteLine($"Compressed archives: {gzFiles.Length} ({totalGzSize:N0} bytes)");
        Console.WriteLine();

        if (logFiles.Length > 0)
        {
            Console.WriteLine("Active logs:");
            foreach (var file in logFiles.OrderByDescending(f => new FileInfo(f).Length).Take(10))
            {
                var info = new FileInfo(file);
                Console.WriteLine($"  {info.Name,-30} {info.Length,12:N0} bytes");
            }
        }

        if (gzFiles.Length > 0)
        {
            Console.WriteLine("\nCompressed archives:");
            foreach (var file in gzFiles.OrderByDescending(f => new FileInfo(f).LastWriteTime).Take(10))
            {
                var info = new FileInfo(file);
                Console.WriteLine($"  {info.Name,-30} {info.Length,12:N0} bytes  {info.LastWriteTime:yyyy-MM-dd}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ShowHelp()
{
    Console.WriteLine("""
        LogRotator - Automatic log file rotation and compression

        Usage:
          dotnet run -- rotate <log-file> [options]
          dotnet run -- compress <log-file> [output-file]
          dotnet run -- compress <directory>
          dotnet run -- cleanup <directory> [options]
          dotnet run -- status <directory>

        Rotate Options:
          --max-size <bytes>   Rotate when file exceeds this size (default: 10MB)
          --max-files <count>  Keep this many rotated files (default: 5)
          --compress           Compress rotated files
          --pattern <pattern>  Rotated file pattern (default: {file}.{n}.gz)

        Cleanup Options:
          --older-than <days>  Remove files older than this (default: 30)
          --pattern <pattern>  File pattern (default: *.gz)
          --dry-run            Show what would be deleted

        Examples:
          dotnet run -- rotate app.log --max-size 5MB --compress
          dotnet run -- compress app.log
          dotnet run -- compress ./logs
          dotnet run -- cleanup ./logs --older-than 7 --dry-run
          dotnet run -- status ./logs
        """);
}

void ShiftRotatedFiles(string logFile, int maxFiles, string pattern)
{
    var dir = Path.GetDirectoryName(logFile) ?? ".";
    var baseName = Path.GetFileNameWithoutExtension(logFile);

    // Delete oldest if at max
    var oldestPattern = pattern.Replace("{file}", baseName).Replace("{n}", maxFiles.ToString());
    var oldestPath = Path.Combine(dir, oldestPattern);
    if (File.Exists(oldestPath))
    {
        File.Delete(oldestPath);
    }

    // Shift files: 4->5, 3->4, 2->3, 1->2
    for (var i = maxFiles - 1; i >= 1; i--)
    {
        var fromPattern = pattern.Replace("{file}", baseName).Replace("{n}", i.ToString());
        var toPattern = pattern.Replace("{file}", baseName).Replace("{n}", (i + 1).ToString());
        var fromPath = Path.Combine(dir, fromPattern);
        var toPath = Path.Combine(dir, toPattern);

        if (File.Exists(fromPath))
        {
            File.Move(fromPath, toPath);
        }
    }
}

void CompressFile(string source, string destination)
{
    using var sourceStream = File.OpenRead(source);
    using var destStream = File.Create(destination);
    using var gzipStream = new GZipStream(destStream, CompressionLevel.Optimal);
    sourceStream.CopyTo(gzipStream);
}
