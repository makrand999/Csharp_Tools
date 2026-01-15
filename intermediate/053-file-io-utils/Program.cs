using System;
using System.IO;
using System.Linq;

namespace FileIoUtils;

/// <summary>
/// Batch file operations utility for common file management tasks.
/// Supports listing, copying, moving, deleting, and analyzing files.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();

        switch (command)
        {
            case "list":
                ListFiles(args.ElementAtOrDefault(1) ?? ".", args.ElementAtOrDefault(2));
                break;
            case "copy":
                CopyFiles(args.ElementAtOrDefault(1) ?? ".", args.ElementAtOrDefault(2) ?? ".", args.ElementAtOrDefault(3));
                break;
            case "move":
                MoveFiles(args.ElementAtOrDefault(1) ?? ".", args.ElementAtOrDefault(2) ?? ".", args.ElementAtOrDefault(3));
                break;
            case "delete":
                DeleteFiles(args.ElementAtOrDefault(1) ?? ".", args.ElementAtOrDefault(2));
                break;
            case "duplicates":
                FindDuplicates(args.ElementAtOrDefault(1) ?? ".");
                break;
            case "size":
                AnalyzeSize(args.ElementAtOrDefault(1) ?? ".");
                break;
            case "recent":
                ShowRecent(args.ElementAtOrDefault(1) ?? ".", int.TryParse(args.ElementAtOrDefault(2), out var n) ? n : 10);
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                ShowMenu();
                break;
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("File I/O Utility - Available Commands:");
        Console.WriteLine("  list <path> [pattern]     - List files (default: *.*)");
        Console.WriteLine("  copy <src> <dest> [pattern] - Copy files matching pattern");
        Console.WriteLine("  move <src> <dest> [pattern] - Move files matching pattern");
        Console.WriteLine("  delete <path> [pattern]   - Delete files matching pattern");
        Console.WriteLine("  duplicates <path>         - Find duplicate files by hash");
        Console.WriteLine("  size <path>               - Analyze disk usage");
        Console.WriteLine("  recent <path> [count]     - Show recently modified files");
        Console.WriteLine();
        Console.WriteLine("Example: dotnet run -- list . \"*.cs\"");
    }

    static void ListFiles(string path, string? pattern)
    {
        pattern ??= "*";
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Directory not found: {path}");
            return;
        }

        var files = Directory.GetFiles(path, pattern).OrderBy(f => f).ToList();
        Console.WriteLine($"Files in '{path}' matching '{pattern}':");
        foreach (var file in files)
        {
            var info = new FileInfo(file);
            Console.WriteLine($"  {info.Name,-40} {info.Length,12:N0} bytes");
        }
        Console.WriteLine($"\nTotal: {files.Count} files");
    }

    static void CopyFiles(string sourceDir, string destDir, string? pattern)
    {
        pattern ??= "*";
        if (!Directory.Exists(sourceDir))
        {
            Console.WriteLine($"Source directory not found: {sourceDir}");
            return;
        }

        Directory.CreateDirectory(destDir);
        var files = Directory.GetFiles(sourceDir, pattern);
        int copied = 0;

        foreach (var file in files)
        {
            var destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Copy(file, destFile, overwrite: true);
            copied++;
        }

        Console.WriteLine($"Copied {copied} files from '{sourceDir}' to '{destDir}'");
    }

    static void MoveFiles(string sourceDir, string destDir, string? pattern)
    {
        pattern ??= "*";
        if (!Directory.Exists(sourceDir))
        {
            Console.WriteLine($"Source directory not found: {sourceDir}");
            return;
        }

        Directory.CreateDirectory(destDir);
        var files = Directory.GetFiles(sourceDir, pattern);
        int moved = 0;

        foreach (var file in files)
        {
            var destFile = Path.Combine(destDir, Path.GetFileName(file));
            File.Move(file, destFile, overwrite: true);
            moved++;
        }

        Console.WriteLine($"Moved {moved} files from '{sourceDir}' to '{destDir}'");
    }

    static void DeleteFiles(string path, string? pattern)
    {
        pattern ??= "*";
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Directory not found: {path}");
            return;
        }

        var files = Directory.GetFiles(path, pattern);
        int deleted = 0;

        foreach (var file in files)
        {
            File.Delete(file);
            deleted++;
        }

        Console.WriteLine($"Deleted {deleted} files from '{path}'");
    }

    static void FindDuplicates(string path)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Directory not found: {path}");
            return;
        }

        var filesByHash = new Dictionary<string, List<string>>();

        foreach (var file in Directory.GetFiles(path))
        {
            try
            {
                var hash = ComputeFileHash(file);
                if (!filesByHash.ContainsKey(hash))
                    filesByHash[hash] = new List<string>();
                filesByHash[hash].Add(file);
            }
            catch (IOException ex)
            {
                Console.WriteLine($"Error reading {file}: {ex.Message}");
            }
        }

        var duplicates = filesByHash.Values.Where(g => g.Count > 1).ToList();

        if (duplicates.Count == 0)
        {
            Console.WriteLine("No duplicate files found.");
        }
        else
        {
            Console.WriteLine($"Found {duplicates.Count} groups of duplicate files:");
            int group = 1;
            foreach (var dupGroup in duplicates)
            {
                Console.WriteLine($"\n  Group {group++}:");
                foreach (var file in dupGroup)
                    Console.WriteLine($"    {file}");
            }
        }
    }

    static void AnalyzeSize(string path)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Directory not found: {path}");
            return;
        }

        long totalSize = 0;
        int fileCount = 0;
        var extensions = new Dictionary<string, long>();

        foreach (var file in Directory.GetFiles(path))
        {
            try
            {
                var info = new FileInfo(file);
                totalSize += info.Length;
                fileCount++;

                var ext = info.Extension.ToLower();
                if (!extensions.ContainsKey(ext))
                    extensions[ext] = 0;
                extensions[ext] += info.Length;
            }
            catch (IOException) { }
        }

        Console.WriteLine($"Directory Analysis: {path}");
        Console.WriteLine($"  Total files: {fileCount}");
        Console.WriteLine($"  Total size:  {FormatSize(totalSize)}");
        Console.WriteLine("\n  By extension:");
        foreach (var ext in extensions.OrderByDescending(e => e.Value).Take(10))
            Console.WriteLine($"    {ext.Key,-10} {FormatSize(ext.Value),10}");
    }

    static void ShowRecent(string path, int count)
    {
        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Directory not found: {path}");
            return;
        }

        var recent = Directory.GetFiles(path)
            .Select(f => new FileInfo(f))
            .OrderByDescending(f => f.LastWriteTime)
            .Take(count)
            .ToList();

        Console.WriteLine($"Recently modified files in '{path}':");
        foreach (var file in recent)
            Console.WriteLine($"  {file.LastWriteTime:yyyy-MM-dd HH:mm}  {file.Name}");
    }

    static string ComputeFileHash(string path)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        using var stream = File.OpenRead(path);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB" };
        int order = 0;
        double size = bytes;
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        return $"{size:0.##} {sizes[order]}";
    }
}
