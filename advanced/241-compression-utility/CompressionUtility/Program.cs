using System.IO.Compression;

namespace CompressionUtility;

/// <summary>
/// Compression Utility - Compresses and decompresses files using GZip/Deflate algorithms.
/// Demonstrates stream-based compression with progress reporting.
/// </summary>
public class CompressionService
{
    public async Task<CompressionResult> CompressFileAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
    {
        var inputFile = new FileInfo(inputPath);
        var originalSize = inputFile.Length;
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await using var input = File.OpenRead(inputPath);
        await using var output = File.Create(outputPath);
        await using var compressStream = new GZipStream(output, CompressionLevel.Optimal);

        await input.CopyToAsync(compressStream, 81920, cancellationToken);

        stopwatch.Stop();
        var compressedSize = new FileInfo(outputPath).Length;

        return new CompressionResult
        {
            OriginalSize = originalSize,
            CompressedSize = compressedSize,
            CompressionRatio = (1.0 - (double)compressedSize / originalSize) * 100,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    public async Task<CompressionResult> DecompressFileAsync(string inputPath, string outputPath, CancellationToken cancellationToken = default)
    {
        var inputFile = new FileInfo(inputPath);
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await using var input = File.OpenRead(inputPath);
        await using var output = File.Create(outputPath);
        await using var decompressStream = new GZipStream(input, CompressionMode.Decompress);

        await decompressStream.CopyToAsync(output, 81920, cancellationToken);

        stopwatch.Stop();
        var decompressedSize = new FileInfo(outputPath).Length;

        return new CompressionResult
        {
            OriginalSize = decompressedSize,
            CompressedSize = inputFile.Length,
            CompressionRatio = 0,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    public async Task<DirectoryCompressionResult> CompressDirectoryAsync(
        string sourceDir, string outputFile, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var files = Directory.GetFiles(sourceDir, "*.*", SearchOption.AllDirectories).ToList();
        var totalOriginalSize = files.Sum(f => new FileInfo(f).Length);

        using var archiveStream = File.Create(outputFile);
        using var archive = new ZipArchive(archiveStream, ZipArchiveMode.Create);

        foreach (var file in files)
        {
            var entryName = Path.GetRelativePath(sourceDir, file);
            var entry = archive.CreateEntry(entryName, CompressionLevel.Optimal);

            await using var entryStream = entry.Open();
            await using var fileStream = File.OpenRead(file);

            await fileStream.CopyToAsync(entryStream, 81920, cancellationToken);
        }

        stopwatch.Stop();
        var compressedSize = new FileInfo(outputFile).Length;

        return new DirectoryCompressionResult
        {
            FileCount = files.Count,
            TotalOriginalSize = totalOriginalSize,
            CompressedSize = compressedSize,
            CompressionRatio = (1.0 - (double)compressedSize / totalOriginalSize) * 100,
            ElapsedTime = stopwatch.Elapsed
        };
    }
}

public record CompressionResult
{
    public long OriginalSize { get; init; }
    public long CompressedSize { get; init; }
    public double CompressionRatio { get; init; }
    public TimeSpan ElapsedTime { get; init; }
}

public record DirectoryCompressionResult
{
    public int FileCount { get; init; }
    public long TotalOriginalSize { get; init; }
    public long CompressedSize { get; init; }
    public double CompressionRatio { get; init; }
    public TimeSpan ElapsedTime { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Compression Utility ===\n");

        var service = new CompressionService();

        // Create test file
        var testFile = Path.Combine(Path.GetTempPath(), $"test_data_{Guid.NewGuid()}.txt");
        var compressedFile = testFile + ".gz";
        var decompressedFile = testFile + "_decompressed.txt";

        Console.WriteLine("Creating test file (1 MB of data)...");
        var content = new string('A', 1024 * 1024); // 1 MB of repetitive data (compresses well)
        await File.WriteAllTextAsync(testFile, content);

        var originalSize = new FileInfo(testFile).Length;
        Console.WriteLine($"Original size: {originalSize / 1024} KB\n");

        // Compress
        Console.WriteLine("Compressing with GZip...");
        var compressResult = await service.CompressFileAsync(testFile, compressedFile);

        Console.WriteLine($"Compressed size: {compressResult.CompressedSize / 1024} KB");
        Console.WriteLine($"Compression ratio: {compressResult.CompressionRatio:F1}%");
        Console.WriteLine($"Time: {compressResult.ElapsedTime.TotalMilliseconds:F0}ms\n");

        // Decompress
        Console.WriteLine("Decompressing...");
        var decompressResult = await service.DecompressFileAsync(compressedFile, decompressedFile);

        Console.WriteLine($"Decompressed size: {decompressResult.OriginalSize / 1024} KB");
        Console.WriteLine($"Time: {decompressResult.ElapsedTime.TotalMilliseconds:F0}ms\n");

        // Verify
        var originalContent = await File.ReadAllTextAsync(testFile);
        var decompressedContent = await File.ReadAllTextAsync(decompressedFile);
        var verified = originalContent == decompressedContent;

        Console.WriteLine($"Verification: {(verified ? "✓ PASSED" : "✗ FAILED")}");

        // Directory compression
        Console.WriteLine("\n=== Directory Compression ===");
        var testDir = Path.Combine(Path.GetTempPath(), $"test_dir_{Guid.NewGuid()}");
        Directory.CreateDirectory(testDir);

        // Create multiple test files
        for (int i = 0; i < 5; i++)
        {
            await File.WriteAllTextAsync(Path.Combine(testDir, $"file{i}.txt"), content);
        }

        var zipFile = Path.Combine(Path.GetTempPath(), $"test_archive_{Guid.NewGuid()}.zip");
        var dirResult = await service.CompressDirectoryAsync(testDir, zipFile);

        Console.WriteLine($"Files: {dirResult.FileCount}");
        Console.WriteLine($"Original: {dirResult.TotalOriginalSize / 1024} KB");
        Console.WriteLine($"Compressed: {dirResult.CompressedSize / 1024} KB");
        Console.WriteLine($"Ratio: {dirResult.CompressionRatio:F1}%");

        // Cleanup
        File.Delete(testFile);
        File.Delete(compressedFile);
        File.Delete(decompressedFile);
        Directory.Delete(testDir, true);
        File.Delete(zipFile);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
