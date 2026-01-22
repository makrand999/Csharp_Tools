using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace EmptyDirCleaner;

/// <summary>
/// Finds and removes empty directories recursively.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        var path = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
        var dryRun = args.Contains("--dry-run") || args.Contains("-n");
        var verbose = args.Contains("--verbose") || args.Contains("-v");

        if (!Directory.Exists(path))
        {
            Console.WriteLine($"Error: Directory '{path}' does not exist.");
            return 1;
        }

        try
        {
            var emptyDirs = FindEmptyDirectories(path);
            
            if (emptyDirs.Count == 0)
            {
                Console.WriteLine("No empty directories found.");
                return 0;
            }

            Console.WriteLine($"Found {emptyDirs.Count} empty director{(emptyDirs.Count == 1 ? "y" : "ies")}:\n");
            
            foreach (var dir in emptyDirs)
            {
                Console.WriteLine($"  {dir}");
            }

            if (dryRun)
            {
                Console.WriteLine("\n[Dry run] No directories were deleted.");
            }
            else
            {
                Console.WriteLine();
                Console.Write("Delete these directories? (y/N): ");
                var response = Console.ReadLine()?.Trim().ToLower();
                
                if (response == "y" || response == "yes")
                {
                    int deleted = 0;
                    int failed = 0;
                    
                    // Delete from deepest to shallowest
                    foreach (var dir in emptyDirs.OrderByDescending(d => d.Length))
                    {
                        try
                        {
                            Directory.Delete(dir);
                            if (verbose)
                                Console.WriteLine($"  Deleted: {dir}");
                            deleted++;
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"  Failed to delete {dir}: {ex.Message}");
                            failed++;
                        }
                    }
                    
                    Console.WriteLine($"\nDeleted {deleted} director{(deleted == 1 ? "y" : "ies")}.");
                    if (failed > 0)
                        Console.WriteLine($"Failed to delete {failed} director{(failed == 1 ? "y" : "ies")}.");
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

    static List<string> FindEmptyDirectories(string path)
    {
        var emptyDirs = new List<string>();
        
        try
        {
            var directory = new DirectoryInfo(path);
            
            // Process subdirectories first (depth-first)
            foreach (var subDir in directory.EnumerateDirectories())
            {
                var subEmpty = FindEmptyDirectories(subDir.FullName);
                emptyDirs.AddRange(subEmpty);
            }
            
            // Check if current directory is empty (no files and no subdirectories)
            if (!directory.EnumerateFiles().Any() && 
                !directory.EnumerateDirectories().Any())
            {
                emptyDirs.Add(path);
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
        
        return emptyDirs;
    }
}
