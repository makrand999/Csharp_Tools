namespace DirectoryComparator;

/// <summary>
/// Directory Comparator - Compares two directories and identifies differences.
/// Demonstrates file hashing, comparison algorithms, and diff reporting.
/// </summary>
public class DirectoryComparisonService
{
    public async Task<ComparisonResult> CompareAsync(string path1, string path2, IProgress<string>? progress = null)
    {
        var result = new ComparisonResult
        {
            Path1 = path1,
            Path2 = path2
        };

        var files1 = GetFileHashes(path1);
        var files2 = GetFileHashes(path2);

        progress?.Report($"Found {files1.Count} files in first directory");
        progress?.Report($"Found {files2.Count} files in second directory");

        // Files only in path1
        foreach (var kvp in files1)
        {
            if (!files2.ContainsKey(kvp.Key))
            {
                result.OnlyInPath1.Add(kvp.Key);
            }
            else if (files2[kvp.Key] != kvp.Value)
            {
                result.ModifiedFiles.Add(kvp.Key);
            }
        }

        // Files only in path2
        foreach (var kvp in files2)
        {
            if (!files1.ContainsKey(kvp.Key))
            {
                result.OnlyInPath2.Add(kvp.Key);
            }
        }

        result.TotalFilesPath1 = files1.Count;
        result.TotalFilesPath2 = files2.Count;
        result.IdenticalFiles = files1.Count - result.ModifiedFiles.Count - result.OnlyInPath1.Count;

        return result;
    }

    private Dictionary<string, string> GetFileHashes(string path)
    {
        var hashes = new Dictionary<string, string>();

        if (!Directory.Exists(path))
            return hashes;

        var files = Directory.GetFiles(path, "*.*", SearchOption.AllDirectories);

        foreach (var file in files)
        {
            var relPath = Path.GetRelativePath(path, file);
            var hash = ComputeHash(file);
            hashes[relPath] = hash;
        }

        return hashes;
    }

    private string ComputeHash(string filePath)
    {
        using var md5 = System.Security.Cryptography.MD5.Create();
        using var stream = File.OpenRead(filePath);
        var hash = md5.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

public record ComparisonResult
{
    public string Path1 { get; init; } = string.Empty;
    public string Path2 { get; init; } = string.Empty;
    public int TotalFilesPath1 { get; set; }
    public int TotalFilesPath2 { get; set; }
    public int IdenticalFiles { get; set; }
    public List<string> ModifiedFiles { get; init; } = new();
    public List<string> OnlyInPath1 { get; init; } = new();
    public List<string> OnlyInPath2 { get; init; } = new();
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Directory Comparator ===\n");

        var service = new DirectoryComparisonService();

        // Create test directories
        var dir1 = Path.Combine(Path.GetTempPath(), $"compare_dir1_{Guid.NewGuid()}");
        var dir2 = Path.Combine(Path.GetTempPath(), $"compare_dir2_{Guid.NewGuid()}");

        Directory.CreateDirectory(dir1);
        Directory.CreateDirectory(dir2);

        Console.WriteLine("Setting up test directories...\n");

        // Create identical files
        await File.WriteAllTextAsync(Path.Combine(dir1, "same.txt"), "Same content");
        await File.WriteAllTextAsync(Path.Combine(dir2, "same.txt"), "Same content");

        // Create modified files
        await File.WriteAllTextAsync(Path.Combine(dir1, "modified.txt"), "Original content");
        await File.WriteAllTextAsync(Path.Combine(dir2, "modified.txt"), "Modified content");

        // Create unique files
        await File.WriteAllTextAsync(Path.Combine(dir1, "only_in_1.txt"), "Only in dir1");
        await File.WriteAllTextAsync(Path.Combine(dir2, "only_in_2.txt"), "Only in dir2");

        // Compare
        var progress = new Progress<string>(msg => Console.WriteLine($"  {msg}"));
        var result = await service.CompareAsync(dir1, dir2, progress);

        Console.WriteLine($"\n=== Comparison Results ===");
        Console.WriteLine($"Directory 1: {result.Path1}");
        Console.WriteLine($"Directory 2: {result.Path2}");
        Console.WriteLine();
        Console.WriteLine($"Files in directory 1: {result.TotalFilesPath1}");
        Console.WriteLine($"Files in directory 2: {result.TotalFilesPath2}");
        Console.WriteLine($"Identical files: {result.IdenticalFiles}");
        Console.WriteLine();

        if (result.ModifiedFiles.Count > 0)
        {
            Console.WriteLine($"Modified files ({result.ModifiedFiles.Count}):");
            foreach (var file in result.ModifiedFiles)
            {
                Console.WriteLine($"  ~ {file}");
            }
            Console.WriteLine();
        }

        if (result.OnlyInPath1.Count > 0)
        {
            Console.WriteLine($"Only in first directory ({result.OnlyInPath1.Count}):");
            foreach (var file in result.OnlyInPath1)
            {
                Console.WriteLine($"  + {file}");
            }
            Console.WriteLine();
        }

        if (result.OnlyInPath2.Count > 0)
        {
            Console.WriteLine($"Only in second directory ({result.OnlyInPath2.Count}):");
            foreach (var file in result.OnlyInPath2)
            {
                Console.WriteLine($"  + {file}");
            }
            Console.WriteLine();
        }

        // Summary
        var isIdentical = result.ModifiedFiles.Count == 0 && 
                         result.OnlyInPath1.Count == 0 && 
                         result.OnlyInPath2.Count == 0;

        Console.WriteLine($"Directories are: {(isIdentical ? "✓ IDENTICAL" : "✗ DIFFERENT")}");

        // Cleanup
        Directory.Delete(dir1, true);
        Directory.Delete(dir2, true);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
