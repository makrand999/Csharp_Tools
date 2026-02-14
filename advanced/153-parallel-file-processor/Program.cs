using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ParallelFileProcessor;

/// <summary>
/// Parallel file processor that processes multiple files concurrently.
/// Demonstrates Parallel.ForEach, thread-safe collections, and file hashing.
/// </summary>
class Program
{
    private static readonly ConcurrentBag<FileResult> Results = new();
    private static int _maxDegreeOfParallelism = Environment.ProcessorCount;
    private static string _operation = "hash";
    private static readonly string[] SupportedOperations = { "hash", "wordcount", "lines", "stats" };

    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Parallel File Processor");
            Console.WriteLine("========================");
            Console.WriteLine("Usage: dotnet run --project ParallelFileProcessor.csproj <directory> [options]");
            Console.WriteLine();
            Console.WriteLine("Operations:");
            Console.WriteLine("  hash      - Calculate SHA256 hash for each file");
            Console.WriteLine("  wordcount - Count words in text files");
            Console.WriteLine("  lines     - Count lines in text files");
            Console.WriteLine("  stats     - File statistics (size, extension analysis)");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --op <operation>    Operation to perform (default: hash)");
            Console.WriteLine("  --parallel <n>      Max parallel operations (default: CPU count)");
            Console.WriteLine("  --pattern <glob>    File pattern (default: *)");
            Console.WriteLine("  --recursive         Search subdirectories");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project ParallelFileProcessor.csproj ./docs --op wordcount");
            Console.WriteLine("  dotnet run --project ParallelFileProcessor.csproj ./src --op hash --pattern *.cs");
            Console.WriteLine("  dotnet run --project ParallelFileProcessor.csproj /var/log --op stats --recursive");
            return 0;
        }

        var directory = args[0];
        var recursive = args.Contains("--recursive") || args.Contains("-r");
        var pattern = GetArgumentValue(args, "--pattern", "*");

        if (!Directory.Exists(directory))
        {
            Console.WriteLine($"Error: Directory '{directory}' does not exist.");
            return 1;
        }

        _operation = GetArgumentValue(args, "--op", "hash").ToLower();
        if (!SupportedOperations.Contains(_operation))
        {
            Console.WriteLine($"Error: Unsupported operation '{_operation}'. Valid: {string.Join(", ", SupportedOperations)}");
            return 1;
        }

        var parallelValue = GetArgumentValue(args, "--parallel", Environment.ProcessorCount.ToString());
        _maxDegreeOfParallelism = int.Parse(parallelValue);

        Console.WriteLine($"Parallel File Processor");
        Console.WriteLine($"Directory: {Path.GetFullPath(directory)}");
        Console.WriteLine($"Operation: {_operation}");
        Console.WriteLine($"Parallel threads: {_maxDegreeOfParallelism}");
        Console.WriteLine($"Pattern: {pattern}");
        Console.WriteLine($"Recursive: {recursive}");
        Console.WriteLine();

        var files = Directory.GetFiles(directory, pattern, recursive ? SearchOption.AllDirectories : SearchOption.TopDirectoryOnly)
            .ToList();

        Console.WriteLine($"Found {files.Count} files to process\n");

        if (files.Count == 0)
        {
            Console.WriteLine("No files to process.");
            return 0;
        }

        var stopwatch = Stopwatch.StartNew();

        try
        {
            ProcessFiles(files);

            stopwatch.Stop();

            Console.WriteLine($"\n{'=',-60}");
            Console.WriteLine($"Processing completed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Files processed: {Results.Count}");
            Console.WriteLine($"Throughput: {Results.Count / (stopwatch.ElapsedMilliseconds / 1000.0):F2} files/sec");

            PrintSummary();

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static void ProcessFiles(List<string> files)
    {
        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = _maxDegreeOfParallelism
        };

        Parallel.ForEach(files, options, file =>
        {
            var result = ProcessFile(file);
            Results.Add(result);
        });
    }

    static FileResult ProcessFile(string filePath)
    {
        var result = new FileResult
        {
            FilePath = filePath,
            FileName = Path.GetFileName(filePath),
            Extension = Path.GetExtension(filePath),
        };

        try
        {
            var fileInfo = new FileInfo(filePath);
            result.Size = fileInfo.Length;
            result.Created = fileInfo.CreationTimeUtc;
            result.Modified = fileInfo.LastWriteTimeUtc;

            switch (_operation)
            {
                case "hash":
                    result.Hash = CalculateHash(filePath);
                    break;

                case "wordcount":
                    result.WordCount = CountWords(filePath);
                    break;

                case "lines":
                    result.LineCount = CountLines(filePath);
                    break;

                case "stats":
                    // Stats are already collected in basic properties
                    break;
            }

            result.Success = true;
            Console.WriteLine($"✓ {result.FileName} ({FormatSize(result.Size)})");
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
            Console.WriteLine($"✗ {result.FileName} - Error: {ex.Message}");
        }

        return result;
    }

    static string? CalculateHash(string filePath)
    {
        using var sha256 = SHA256.Create();
        using var stream = File.OpenRead(filePath);
        var hash = sha256.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }

    static int CountWords(string filePath)
    {
        var content = File.ReadAllText(filePath);
        return content.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).Length;
    }

    static int CountLines(string filePath)
    {
        int lineCount = 0;
        using var stream = File.OpenRead(filePath);
        using var reader = new StreamReader(stream);
        while (!reader.EndOfStream)
        {
            reader.ReadLine();
            lineCount++;
        }
        return lineCount;
    }

    static void PrintSummary()
    {
        var successful = Results.Count(r => r.Success);
        var failed = Results.Count(r => !r.Success);

        Console.WriteLine($"\nResults Summary:");
        Console.WriteLine($"  Successful: {successful}");
        Console.WriteLine($"  Failed: {failed}");

        if (_operation == "hash")
        {
            Console.WriteLine($"\nFile Hashes:");
            foreach (var result in Results.Where(r => r.Success).OrderBy(r => r.FileName))
            {
                Console.WriteLine($"  {result.Hash}  {result.FileName}");
            }
        }
        else if (_operation == "wordcount")
        {
            var totalWords = Results.Sum(r => r.WordCount);
            Console.WriteLine($"\nTotal words: {totalWords}");
            Console.WriteLine($"\nWord count by file:");
            foreach (var result in Results.Where(r => r.Success).OrderByDescending(r => r.WordCount).Take(20))
            {
                Console.WriteLine($"  {result.WordCount,8}  {result.FileName}");
            }
        }
        else if (_operation == "lines")
        {
            var totalLines = Results.Sum(r => r.LineCount);
            Console.WriteLine($"\nTotal lines: {totalLines}");
            Console.WriteLine($"\nLine count by file:");
            foreach (var result in Results.Where(r => r.Success).OrderByDescending(r => r.LineCount).Take(20))
            {
                Console.WriteLine($"  {result.LineCount,8}  {result.FileName}");
            }
        }
        else if (_operation == "stats")
        {
            var totalSize = Results.Sum(r => r.Size);
            var avgSize = Results.Average(r => r.Size);

            Console.WriteLine($"\nSize Statistics:");
            Console.WriteLine($"  Total size: {FormatSize(totalSize)}");
            Console.WriteLine($"  Average file size: {FormatSize((long)avgSize)}");
            Console.WriteLine($"  Largest file: {FormatSize(Results.Max(r => r.Size))}");
            Console.WriteLine($"  Smallest file: {FormatSize(Results.Min(r => r.Size))}");

            Console.WriteLine($"\nFiles by extension:");
            foreach (var group in Results.GroupBy(r => r.Extension).OrderByDescending(g => g.Count()).Take(10))
            {
                var ext = string.IsNullOrEmpty(group.Key) ? "(no extension)" : group.Key;
                Console.WriteLine($"  {ext,-10} {group.Count(),4} files ({FormatSize(group.Sum(r => r.Size))})");
            }
        }
    }

    static string GetArgumentValue(string[] args, string key, string defaultValue)
    {
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == key && i + 1 < args.Length)
            {
                return args[i + 1];
            }
        }
        return defaultValue;
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

class FileResult
{
    public string FilePath { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string Extension { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime Created { get; set; }
    public DateTime Modified { get; set; }
    public string? Hash { get; set; }
    public int WordCount { get; set; }
    public int LineCount { get; set; }
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}
