using System.IO.Compression;

namespace ZipArchiveManager;

/// <summary>
/// ZIP Archive Manager - Creates, extracts, and manages ZIP archives.
/// Demonstrates ZIP file operations with compression and entry management.
/// </summary>
public class ZipArchiveService
{
    public async Task<ArchiveResult> CreateArchiveAsync(
        string sourcePath, string archivePath, CompressionLevel compression = CompressionLevel.Optimal)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var filesAdded = 0;
        var totalSize = 0L;

        if (File.Exists(sourcePath))
        {
            using var stream = File.Create(archivePath);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            var entry = archive.CreateEntry(Path.GetFileName(sourcePath), compression);
            using var entryStream = entry.Open();
            using var fileStream = File.OpenRead(sourcePath);

            fileStream.CopyTo(entryStream);

            filesAdded = 1;
            totalSize = new FileInfo(sourcePath).Length;
        }
        else if (Directory.Exists(sourcePath))
        {
            using var stream = File.Create(archivePath);
            using var archive = new ZipArchive(stream, ZipArchiveMode.Create);

            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var entryName = Path.GetRelativePath(sourcePath, file);
                var entry = archive.CreateEntry(entryName, compression);

                using var entryStream = entry.Open();
                using var fileStream = File.OpenRead(file);

                fileStream.CopyTo(entryStream);

                filesAdded++;
                totalSize += new FileInfo(file).Length;
            }
        }

        stopwatch.Stop();
        var compressedSize = new FileInfo(archivePath).Length;

        return new ArchiveResult
        {
            FilesProcessed = filesAdded,
            OriginalSize = totalSize,
            CompressedSize = compressedSize,
            CompressionRatio = totalSize > 0 ? (1.0 - (double)compressedSize / totalSize) * 100 : 0,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    public async Task<ArchiveResult> ExtractArchiveAsync(string archivePath, string destinationPath)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var filesExtracted = 0;
        var totalSize = 0L;

        Directory.CreateDirectory(destinationPath);

        using var archive = ZipFile.OpenRead(archivePath);
        
        foreach (var entry in archive.Entries)
        {
            if (string.IsNullOrEmpty(entry.Name))
                continue;

            var fullPath = Path.Combine(destinationPath, entry.FullName);
            Directory.CreateDirectory(Path.GetDirectoryName(fullPath)!);

            await using var entryStream = entry.Open();
            await using var fileStream = File.Create(fullPath);

            await entryStream.CopyToAsync(fileStream);
            
            filesExtracted++;
            totalSize += entry.Length;
        }

        stopwatch.Stop();

        return new ArchiveResult
        {
            FilesProcessed = filesExtracted,
            OriginalSize = totalSize,
            CompressedSize = new FileInfo(archivePath).Length,
            CompressionRatio = 0,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    public ArchiveInfo GetArchiveInfo(string archivePath)
    {
        using var archive = ZipFile.OpenRead(archivePath);
        
        var entries = archive.Entries
            .Where(e => !string.IsNullOrEmpty(e.Name))
            .Select(e => new ArchiveEntryInfo
            {
                Name = e.FullName,
                CompressedSize = e.CompressedLength,
                UncompressedSize = e.Length,
                CompressionRatio = e.Length > 0 ? (1.0 - (double)e.CompressedLength / e.Length) * 100 : 0,
                LastModified = e.LastWriteTime.DateTime
            })
            .ToList();

        return new ArchiveInfo
        {
            TotalEntries = entries.Count,
            TotalCompressedSize = entries.Sum(e => e.CompressedSize),
            TotalUncompressedSize = entries.Sum(e => e.UncompressedSize),
            Entries = entries
        };
    }
}

public record ArchiveResult
{
    public int FilesProcessed { get; init; }
    public long OriginalSize { get; init; }
    public long CompressedSize { get; init; }
    public double CompressionRatio { get; init; }
    public TimeSpan ElapsedTime { get; init; }
}

public record ArchiveInfo
{
    public int TotalEntries { get; init; }
    public long TotalCompressedSize { get; init; }
    public long TotalUncompressedSize { get; init; }
    public List<ArchiveEntryInfo> Entries { get; init; } = new();
}

public record ArchiveEntryInfo
{
    public string Name { get; init; } = string.Empty;
    public long CompressedSize { get; init; }
    public long UncompressedSize { get; init; }
    public double CompressionRatio { get; init; }
    public DateTime LastModified { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== ZIP Archive Manager ===\n");

        var service = new ZipArchiveService();

        // Create test files
        var testDir = Path.Combine(Path.GetTempPath(), $"zip_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(testDir);

        Console.WriteLine("Creating test files...");
        for (int i = 0; i < 5; i++)
        {
            var content = new string((char)('A' + i), 1000 * (i + 1));
            await File.WriteAllTextAsync(Path.Combine(testDir, $"file{i}.txt"), content);
        }

        var archivePath = Path.Combine(Path.GetTempPath(), $"test_archive_{Guid.NewGuid()}.zip");
        var extractDir = Path.Combine(Path.GetTempPath(), $"extracted_{Guid.NewGuid()}");

        // Create archive
        Console.WriteLine($"\nCreating ZIP archive...");
        var createResult = await service.CreateArchiveAsync(testDir, archivePath);

        Console.WriteLine($"Files added: {createResult.FilesProcessed}");
        Console.WriteLine($"Original size: {createResult.OriginalSize / 1024} KB");
        Console.WriteLine($"Compressed size: {createResult.CompressedSize / 1024} KB");
        Console.WriteLine($"Compression ratio: {createResult.CompressionRatio:F1}%");
        Console.WriteLine($"Time: {createResult.ElapsedTime.TotalMilliseconds:F0}ms");

        // Show archive info
        Console.WriteLine($"\n=== Archive Contents ===");
        var archiveInfo = service.GetArchiveInfo(archivePath);
        
        Console.WriteLine($"Total entries: {archiveInfo.TotalEntries}");
        Console.WriteLine($"Total uncompressed: {archiveInfo.TotalUncompressedSize / 1024} KB");
        Console.WriteLine($"Total compressed: {archiveInfo.TotalCompressedSize / 1024} KB\n");

        foreach (var entry in archiveInfo.Entries)
        {
            Console.WriteLine($"  {entry.Name}");
            Console.WriteLine($"    Size: {entry.UncompressedSize} -> {entry.CompressedSize} bytes ({entry.CompressionRatio:F1}%)");
        }

        // Extract archive
        Console.WriteLine($"\nExtracting archive...");
        var extractResult = await service.ExtractArchiveAsync(archivePath, extractDir);

        Console.WriteLine($"Files extracted: {extractResult.FilesProcessed}");
        Console.WriteLine($"Total size: {extractResult.OriginalSize / 1024} KB");
        Console.WriteLine($"Time: {extractResult.ElapsedTime.TotalMilliseconds:F0}ms");

        // Verify
        var originalFiles = Directory.GetFiles(testDir, "*.*", SearchOption.AllDirectories).Count();
        var extractedFiles = Directory.GetFiles(extractDir, "*.*", SearchOption.AllDirectories).Count();
        var verified = originalFiles == extractedFiles;

        Console.WriteLine($"\nVerification: {(verified ? "✓ PASSED" : "✗ FAILED")}");

        // Cleanup
        Directory.Delete(testDir, true);
        Directory.Delete(extractDir, true);
        File.Delete(archivePath);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
