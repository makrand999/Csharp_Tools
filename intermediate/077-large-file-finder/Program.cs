using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace LargeFileFinder;

/// <summary>
/// Finds files larger than a specified size threshold.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        var path = args.Length > 0 && !args[0].StartsWith("-") ? args[0] : Directory.GetCurrentDirectory();
        var minSizeMb = 10.0; // Default 10 MB
        var topCount = 50;
        var outputJson = false;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--size" when i + 1 < args.Length:
                    if (double.TryParse(args[++i], out var size))
                        minSizeMb = size;
                    break;
                case "--top" when i + 1 < args.Length:
                    if (int.TryParse(args[++i], out var top))
                        topCount = top;
                    break;
                case "--json":
                    outputJson = true;
                    break;
            }
        }

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Error: Directory '{path}' does not exist.");
            return 1;
        }

        try
        {
            var minSizeBytes = (long)(minSizeMb * 1024 * 1024);
            var largeFiles = FindLargeFiles(path, minSizeBytes);
            
            if (largeFiles.Count == 0)
            {
                Console.WriteLine($"No files larger than {FormatSize(minSizeBytes)} found.");
                return 0;
            }

            if (outputJson)
            {
                OutputJson(largeFiles);
            }
            else
            {
                OutputTable(largeFiles, minSizeBytes);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static List<FileInfo> FindLargeFiles(string path, long minSizeBytes)
    {
        var largeFiles = new List<FileInfo>();
        
        try
        {
            var directory = new DirectoryInfo(path);
            
            foreach (var file in directory.EnumerateFiles("*", SearchOption.AllDirectories))
            {
                try
                {
                    if (file.Length >= minSizeBytes)
                    {
                        largeFiles.Add(new FileInfo
                        {
                            Path = file.FullName,
                            Size = file.Length,
                            Extension = file.Extension.ToLower(),
                            Modified = file.LastWriteTime
                        });
                    }
                }
                catch { /* Skip inaccessible files */ }
            }
        }
        catch (UnauthorizedAccessException)
        {
            // Skip directories we don't have permission to access
        }
        catch (Exception)
        {
            // Skip other errors
        }
        
        return largeFiles
            .OrderByDescending(f => f.Size)
            .ToList();
    }

    static void OutputTable(List<FileInfo> files, long minSizeBytes)
    {
        Console.WriteLine($"Found {files.Count} file(s) larger than {FormatSize(minSizeBytes)}:\n");
        
        // Calculate column widths
        var maxSizeWidth = files.Max(f => FormatSize(f.Size).Length);
        
        Console.WriteLine($"{"Size",-12} {"Extension",-10} {"Modified",-20} Path");
        Console.WriteLine(new string('-', 80));
        
        foreach (var file in files)
        {
            Console.WriteLine($"{FormatSize(file.Size),-12} {file.Extension,-10} {file.Modified:yyyy-MM-dd HH:mm:ss,-20} {file.Path}");
        }

        // Summary
        var totalSize = files.Sum(f => f.Size);
        Console.WriteLine($"\nTotal: {files.Count} files, {FormatSize(totalSize)}");
        
        // By extension
        Console.WriteLine("\nBy extension:");
        var byExt = files.GroupBy(f => f.Extension)
            .OrderByDescending(g => g.Sum(f => f.Size))
            .Take(10);
        
        foreach (var group in byExt)
        {
            var ext = string.IsNullOrEmpty(group.Key) ? "(no extension)" : group.Key;
            Console.WriteLine($"  {ext,-15} {group.Count(),3} files, {FormatSize(group.Sum(f => f.Size))}");
        }
    }

    static void OutputJson(List<FileInfo> files)
    {
        var totalSize = files.Sum(f => f.Size);
        Console.WriteLine("{");
        Console.WriteLine($"  \"minSizeThreshold\": {files[0].Size},");
        Console.WriteLine($"  \"totalFiles\": {files.Count},");
        Console.WriteLine($"  \"totalSize\": {totalSize},");
        Console.WriteLine("  \"files\": [");
        
        for (int i = 0; i < files.Count; i++)
        {
            var file = files[i];
            var comma = i < files.Count - 1 ? "," : "";
            Console.WriteLine("    {");
            Console.WriteLine($"      \"path\": \"{EscapeJson(file.Path)}\",");
            Console.WriteLine($"      \"size\": {file.Size},");
            Console.WriteLine($"      \"sizeFormatted\": \"{FormatSize(file.Size)}\",");
            Console.WriteLine($"      \"extension\": \"{EscapeJson(file.Extension)}\",");
            Console.WriteLine($"      \"modified\": \"{file.Modified:yyyy-MM-ddTHH:mm:ss}\"");
            Console.WriteLine($"    }}{comma}");
        }
        
        Console.WriteLine("  ]");
        Console.WriteLine("}");
    }

    static string EscapeJson(string s) => s.Replace("\\", "\\\\")
                                            .Replace("\"", "\\\"")
                                            .Replace("\n", "\\n")
                                            .Replace("\r", "\\r")
                                            .Replace("\t", "\\t");

    static string FormatSize(long bytes)
    {
        string[] sizes = { "B", "KB", "MB", "GB", "TB" };
        int order = 0;
        double size = bytes;
        
        while (size >= 1024 && order < sizes.Length - 1)
        {
            order++;
            size /= 1024;
        }
        
        return $"{size:F2} {sizes[order]}";
    }
}

class FileInfo
{
    public string Path { get; set; } = "";
    public long Size { get; set; }
    public string Extension { get; set; } = "";
    public DateTime Modified { get; set; }
}
