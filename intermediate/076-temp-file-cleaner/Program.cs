using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace TempFileCleaner;

/// <summary>
/// Finds and removes temporary files based on common patterns and extensions.
/// </summary>
class Program
{
    // Common temporary file patterns
    private static readonly string[] TempPatterns =
    {
        "*.tmp", "*.temp", "*.bak", "*.old", "*.swp", "*.swo",
        "*~", "*.log", "*.cache", "*.tempfile"
    };

    // Common temporary directory names
    private static readonly string[] TempDirNames =
    {
        "tmp", "temp", "cache", ".cache", "tmpfiles", "tempfiles"
    };

    static int Main(string[] args)
    {
        var path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        var dryRun = args.Contains("--dry-run") || args.Contains("-n");
        var verbose = args.Contains("--verbose") || args.Contains("-v");
        var includeCacheDirs = args.Contains("--include-cache-dirs");

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Error: Directory '{path}' does not exist.");
            return 1;
        }

        try
        {
            Console.WriteLine($"Scanning for temporary files in: {Path.GetFullPath(path)}\n");

            var tempFiles = FindTempFiles(path, includeCacheDirs);
            
            if (tempFiles.Count == 0)
            {
                Console.WriteLine("No temporary files found.");
                return 0;
            }

            var totalSize = tempFiles.Sum(f => f.Size);
            Console.WriteLine($"Found {tempFiles.Count} temporary file(s) ({FormatSize(totalSize)}):\n");

            foreach (var file in tempFiles)
            {
                Console.WriteLine($"  {file.Path} ({FormatSize(file.Size)})");
            }

            if (dryRun)
            {
                Console.WriteLine("\n[Dry run] No files were deleted.");
            }
            else
            {
                Console.WriteLine();
                Console.Write("Delete these files? (y/N): ");
                var response = Console.ReadLine()?.Trim().ToLower();
                
                if (response == "y" || response == "yes")
                {
                    int deleted = 0;
                    long freedSpace = 0;
                    int failed = 0;
                    
                    foreach (var file in tempFiles)
                    {
                        try
                        {
                            File.Delete(file.Path);
                            if (verbose)
                                Console.WriteLine($"  Deleted: {file.Path}");
                            deleted++;
                            freedSpace += file.Size;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"  Failed to delete {file.Path}: {ex.Message}");
                            failed++;
                        }
                    }
                    
                    Console.WriteLine($"\nDeleted {deleted} file(s).");
                    Console.WriteLine($"Freed {FormatSize(freedSpace)} of disk space.");
                    if (failed > 0)
                        Console.WriteLine($"Failed to delete {failed} file(s).");
                }
                else
                {
                    Console.WriteLine("Operation cancelled.");
                }
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static List<TempFileInfo> FindTempFiles(string path, bool includeCacheDirs)
    {
        var tempFiles = new List<TempFileInfo>();
        
        try
        {
            var directory = new DirectoryInfo(path);
            
            // Find files matching temp patterns
            foreach (var pattern in TempPatterns)
            {
                try
                {
                    var files = directory.EnumerateFiles(pattern, SearchOption.AllDirectories);
                    foreach (var file in files)
                    {
                        tempFiles.Add(new TempFileInfo
                        {
                            Path = file.FullName,
                            Size = file.Length
                        });
                    }
                }
                catch { /* Skip patterns that cause errors */ }
            }

            // Find files in temp directories
            if (includeCacheDirs)
            {
                foreach (var tempDirName in TempDirNames)
                {
                    try
                    {
                        var tempDirs = directory.EnumerateDirectories(tempDirName, SearchOption.AllDirectories);
                        foreach (var tempDir in tempDirs)
                        {
                            var files = tempDir.EnumerateFiles();
                            foreach (var file in files)
                            {
                                // Avoid duplicates
                                if (!tempFiles.Any(f => f.Path == file.FullName))
                                {
                                    tempFiles.Add(new TempFileInfo
                                    {
                                        Path = file.FullName,
                                        Size = file.Length
                                    });
                                }
                            }
                        }
                    }
                    catch { /* Skip directories that cause errors */ }
                }
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
        
        // Remove duplicates and sort by size (largest first)
        return tempFiles
            .GroupBy(f => f.Path)
            .Select(g => g.First())
            .OrderByDescending(f => f.Size)
            .ToList();
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
        
        return $"{size:F2} {sizes[order]}";
    }
}

class TempFileInfo
{
    public string Path { get; set; } = "";
    public long Size { get; set; }
}
