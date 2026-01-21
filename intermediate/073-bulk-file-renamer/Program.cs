using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace BulkFileRenamer;

/// <summary>
/// Bulk file renamer with pattern support (prefix, suffix, numbering, find/replace, regex).
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            PrintUsage();
            return 1;
        }

        var options = ParseOptions(args);
        if (options == null)
        {
            PrintUsage();
            return 1;
        }

        try
        {
            return RenameFiles(options);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("""
            BulkFileRenamer - Rename multiple files with patterns

            Usage:
              dotnet run --project BulkFileRenamer.csproj [OPTIONS] <directory>

            Options:
              --prefix <text>       Add prefix to filenames
              --suffix <text>       Add suffix to filenames (before extension)
              --number              Add sequential numbering (001, 002, ...)
              --number-start <n>    Start numbering from n (default: 1)
              --find <old>          Find and replace text in filename
              --replace <new>       Replacement text for --find
              --regex <pattern>     Regex pattern to remove from filename
              --extension <ext>     Filter by file extension (e.g., .jpg, .txt)
              --dry-run             Show what would be renamed without actually renaming

            Examples:
              dotnet run --project BulkFileRenamer.csproj --prefix "IMG_" ./photos
              dotnet run --project BulkFileRenamer.csproj --number --suffix "_backup" ./docs
              dotnet run --project BulkFileRenamer.csproj --find "old" --replace "new" ./files
              dotnet run --project BulkFileRenamer.csproj --regex "\\d+" --dry-run ./mixed
            """);
    }

    static RenameOptions? ParseOptions(string[] args)
    {
        var options = new RenameOptions();
        var directory = args.Last();

        if (!Directory.Exists(directory))
        {
            Console.WriteLine($"Error: Directory '{directory}' does not exist.");
            return null;
        }

        options.Directory = directory;

        for (int i = 0; i < args.Length - 1; i++)
        {
            switch (args[i])
            {
                case "--prefix" when i + 1 < args.Length:
                    options.Prefix = args[++i];
                    break;
                case "--suffix" when i + 1 < args.Length:
                    options.Suffix = args[++i];
                    break;
                case "--number":
                    options.AddNumbering = true;
                    break;
                case "--number-start" when i + 1 < args.Length:
                    if (int.TryParse(args[++i], out var start))
                        options.NumberStart = start;
                    break;
                case "--find" when i + 1 < args.Length:
                    options.FindText = args[++i];
                    break;
                case "--replace" when i + 1 < args.Length:
                    options.ReplaceText = args[++i];
                    break;
                case "--regex" when i + 1 < args.Length:
                    options.RegexPattern = args[++i];
                    break;
                case "--extension" when i + 1 < args.Length:
                    options.Extension = args[++i];
                    break;
                case "--dry-run":
                    options.DryRun = true;
                    break;
            }
        }

        return options;
    }

    static int RenameFiles(RenameOptions options)
    {
        var files = Directory.GetFiles(options.Directory)
            .Where(f => options.Extension == null || 
                        Path.GetExtension(f).Equals(options.Extension, StringComparison.OrdinalIgnoreCase))
            .OrderBy(f => f)
            .ToList();

        if (files.Count == 0)
        {
            Console.WriteLine("No files found matching the criteria.");
            return 0;
        }

        Console.WriteLine($"Found {files.Count} file(s) to rename:\n");

        var number = options.NumberStart;
        foreach (var file in files)
        {
            var fileName = Path.GetFileNameWithoutExtension(file);
            var extension = Path.GetExtension(file);
            var newName = fileName;

            // Apply find/replace
            if (!string.IsNullOrEmpty(options.FindText))
            {
                newName = options.ReplaceText != null
                    ? newName.Replace(options.FindText, options.ReplaceText)
                    : newName.Replace(options.FindText, "");
            }

            // Apply regex removal
            if (!string.IsNullOrEmpty(options.RegexPattern))
            {
                newName = Regex.Replace(newName, options.RegexPattern, "");
            }

            // Apply prefix/suffix
            if (!string.IsNullOrEmpty(options.Prefix))
                newName = options.Prefix + newName;
            if (!string.IsNullOrEmpty(options.Suffix))
                newName = newName + options.Suffix;

            // Apply numbering
            if (options.AddNumbering)
            {
                newName = $"{newName}_{number:D3}";
                number++;
            }

            var newPath = Path.Combine(options.Directory, newName + extension);

            // Handle conflicts
            if (newPath != file && File.Exists(newPath))
            {
                var counter = 1;
                var baseName = newName;
                while (File.Exists(newPath))
                {
                    newName = $"{baseName}_{counter++}";
                    newPath = Path.Combine(options.Directory, newName + extension);
                }
            }

            var status = options.DryRun ? "[DRY-RUN]" : "";
            if (newPath != file)
            {
                Console.WriteLine($"{status} {Path.GetFileName(file)} -> {Path.GetFileName(newPath)}");
                if (!options.DryRun)
                {
                    File.Move(file, newPath);
                }
            }
            else
            {
                Console.WriteLine($"{status} {Path.GetFileName(file)} (unchanged)");
            }
        }

        Console.WriteLine($"\n{(options.DryRun ? "Dry run complete" : "Renaming complete")}.");
        return 0;
    }
}

class RenameOptions
{
    public string Directory { get; set; } = "";
    public string? Prefix { get; set; }
    public string? Suffix { get; set; }
    public bool AddNumbering { get; set; }
    public int NumberStart { get; set; } = 1;
    public string? FindText { get; set; }
    public string? ReplaceText { get; set; }
    public string? RegexPattern { get; set; }
    public string? Extension { get; set; }
    public bool DryRun { get; set; }
}
