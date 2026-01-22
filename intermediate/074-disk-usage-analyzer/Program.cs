using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiskUsageAnalyzer;

/// <summary>
/// Analyzes disk usage of directories and displays size information in a tree view.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        var path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        var maxDepth = args.Length > 1 && int.TryParse(args[1], out var d) ? d : 3;
        var topCount = args.Length > 2 && int.TryParse(args[2], out var t) ? t : 10;

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Error: Directory '{path}' does not exist.");
            return 1;
        }

        try
        {
            AnalyzeDiskUsage(path, maxDepth, topCount);
            return 0;
        }
        catch (UnauthorizedAccessException ex)
        {
            Console.WriteLine($"Permission error: {ex.Message}");
            return 1;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static void AnalyzeDiskUsage(string path, int maxDepth, int topCount)
    {
        Console.WriteLine($"Analyzing: {Path.GetFullPath(path)}\n");

        var rootInfo = GetDirectorySize(path, maxDepth, 0);
        
        Console.WriteLine($"Total Size: {FormatSize(rootInfo.Size)}");
        Console.WriteLine($"Total Files: {rootInfo.FileCount}");
        Console.WriteLine($"Total Directories: {rootInfo.DirectoryCount}\n");

        Console.WriteLine($"Top {topCount} largest items:\n");
        
        var allItems = new List<SizeItem>();
        CollectAllItems(rootInfo, allItems);
        
        var topItems = allItems
            .OrderByDescending(x => x.Size)
            .Take(topCount)
            .ToList();

        int rank = 1;
        foreach (var item in topItems)
        {
            var bar = new string('█', (int)(item.Percentage / 2));
            Console.WriteLine($"{rank++,2}. {FormatSize(item.Size),10} |{bar,-50}| {item.Path}");
        }

        Console.WriteLine("\n\nDirectory Tree:");
        PrintTree(rootInfo, "", true);
    }

    static void CollectAllItems(DirectorySizeInfo dir, List<SizeItem> items)
    {
        foreach (var file in dir.Files)
        {
            items.Add(file);
        }

        foreach (var subDir in dir.SubDirectories)
        {
            items.Add(new SizeItem 
            { 
                Path = subDir.Path, 
                Size = subDir.Size,
                Percentage = 0 
            });
            CollectAllItems(subDir, items);
        }
    }

    static DirectorySizeInfo GetDirectorySize(string path, int maxDepth, int currentDepth)
    {
        var info = new DirectorySizeInfo
        {
            Path = path,
            Name = Path.GetFileName(path) ?? path
        };

        try
        {
            var directory = new DirectoryInfo(path);
            
            // Get files
            foreach (var file in directory.EnumerateFiles())
            {
                try
                {
                    var fileSize = file.Length;
                    info.Size += fileSize;
                    info.FileCount++;
                    info.Files.Add(new SizeItem
                    {
                        Path = file.FullName,
                        Name = file.Name,
                        Size = fileSize,
                        Percentage = 0
                    });
                }
                catch { /* Skip inaccessible files */ }
            }

            // Get subdirectories
            if (currentDepth < maxDepth)
            {
                foreach (var subDir in directory.EnumerateDirectories())
                {
                    try
                    {
                        var subDirSize = GetDirectorySize(subDir.FullName, maxDepth, currentDepth + 1);
                        info.Size += subDirSize.Size;
                        info.DirectoryCount += subDirSize.DirectoryCount + 1;
                        info.FileCount += subDirSize.FileCount;
                        info.SubDirectories.Add(subDirSize);
                    }
                    catch { /* Skip inaccessible directories */ }
                }
            }
        }
        catch { /* Skip inaccessible directories */ }

        // Calculate percentages
        if (info.Size > 0)
        {
            foreach (var file in info.Files)
            {
                file.Percentage = (double)file.Size / info.Size * 100;
            }
            foreach (var subDir in info.SubDirectories)
            {
                subDir.Percentage = (double)subDir.Size / info.Size * 100;
            }
        }

        return info;
    }

    static void PrintTree(DirectorySizeInfo dir, string indent, bool isLast)
    {
        var prefix = isLast ? "└─ " : "├─ ";
        var sizeStr = FormatSize(dir.Size);
        var percentStr = dir.Percentage > 0 ? $" ({dir.Percentage:F1}%)" : "";
        
        Console.WriteLine($"{indent}{prefix}[{sizeStr}]{percentStr} {dir.Name}/");

        var newIndent = indent + (isLast ? "   " : "│  ");
        
        var subDirs = dir.SubDirectories.OrderByDescending(d => d.Size).ToList();
        for (int i = 0; i < subDirs.Count; i++)
        {
            PrintTree(subDirs[i], newIndent, i == subDirs.Count - 1);
        }
    }

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
        
        return $"{size,6:F2} {sizes[order]}";
    }
}

class SizeItem
{
    public string Path { get; set; } = "";
    public string Name { get; set; } = "";
    public long Size { get; set; }
    public double Percentage { get; set; }
}

class DirectorySizeInfo : SizeItem
{
    public List<SizeItem> Files { get; set; } = new();
    public List<DirectorySizeInfo> SubDirectories { get; set; } = new();
    public int FileCount { get; set; }
    public int DirectoryCount { get; set; }
}
