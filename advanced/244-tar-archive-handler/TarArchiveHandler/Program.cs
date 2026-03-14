namespace TarArchiveHandler;

/// <summary>
/// TAR Archive Handler - Creates and extracts TAR archives.
/// Note: This is a simplified implementation using basic TAR format.
/// </summary>
public class TarService
{
    private const int BlockSize = 512;

    public async Task<TarResult> CreateTarAsync(string sourcePath, string tarPath)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var filesAdded = 0;
        var totalSize = 0L;

        await using var tarStream = File.Create(tarPath);

        if (File.Exists(sourcePath))
        {
            await WriteFileToTarAsync(tarStream, sourcePath, Path.GetFileName(sourcePath));
            filesAdded = 1;
            totalSize = new FileInfo(sourcePath).Length;
        }
        else if (Directory.Exists(sourcePath))
        {
            var files = Directory.GetFiles(sourcePath, "*.*", SearchOption.AllDirectories);
            
            foreach (var file in files)
            {
                var entryName = Path.GetRelativePath(sourcePath, file);
                await WriteFileToTarAsync(tarStream, file, entryName);
                filesAdded++;
                totalSize += new FileInfo(file).Length;
            }
        }

        // Write two empty blocks to mark end of archive
        await tarStream.WriteAsync(new byte[BlockSize * 2]);

        stopwatch.Stop();

        return new TarResult
        {
            FilesProcessed = filesAdded,
            TotalSize = totalSize,
            ArchiveSize = new FileInfo(tarPath).Length,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    public async Task<TarResult> ExtractTarAsync(string tarPath, string destinationPath)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var filesExtracted = 0;
        var totalSize = 0L;

        Directory.CreateDirectory(destinationPath);

        using var tarStream = File.OpenRead(tarPath);
        var buffer = new byte[BlockSize];

        while (true)
        {
            var bytesRead = tarStream.Read(buffer, 0, BlockSize);
            if (bytesRead != BlockSize)
                break;
                
            // Check for end of archive (two zero blocks)
            if (buffer.All(b => b == 0))
            {
                // Read second zero block
                tarStream.Read(buffer, 0, BlockSize);
                break;
            }

            // Parse header
            var header = ParseTarHeader(buffer);
            if (header.FileName == null || header.FileSize == 0)
                continue;

            var filePath = Path.Combine(destinationPath, header.FileName);
            Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);

            // Read file content
            var paddedSize = (int)(((header.FileSize + BlockSize - 1) / BlockSize) * BlockSize);
            var contentBuffer = new byte[paddedSize];

            var contentBytesRead = tarStream.Read(contentBuffer, 0, paddedSize);

            // Write actual content (without padding)
            File.WriteAllBytes(filePath, contentBuffer.Take((int)header.FileSize).ToArray());
            
            filesExtracted++;
            totalSize += header.FileSize;
        }

        stopwatch.Stop();

        return new TarResult
        {
            FilesProcessed = filesExtracted,
            TotalSize = totalSize,
            ArchiveSize = new FileInfo(tarPath).Length,
            ElapsedTime = stopwatch.Elapsed
        };
    }

    private async Task WriteFileToTarAsync(Stream tarStream, string filePath, string entryName)
    {
        var fileInfo = new FileInfo(filePath);
        var content = await File.ReadAllBytesAsync(filePath);

        // Create header
        var header = new byte[BlockSize];
        
        // File name (offset 0, 100 bytes)
        var nameBytes = System.Text.Encoding.ASCII.GetBytes(entryName);
        Array.Copy(nameBytes, 0, header, 0, Math.Min(nameBytes.Length, 100));

        // File size in octal (offset 124, 12 bytes)
        var sizeString = Convert.ToString(fileInfo.Length, 8).PadLeft(11, '0');
        var sizeBytes = System.Text.Encoding.ASCII.GetBytes(sizeString);
        Array.Copy(sizeBytes, 0, header, 124, sizeBytes.Length);

        // Checksum placeholder (offset 148, 8 bytes)
        Array.Fill(header, (byte)' ', 148, 8);

        // Calculate and set checksum
        var checksum = header.Sum(b => (byte)b);
        var checksumString = Convert.ToString(checksum, 8).PadLeft(6, '0');
        var checksumBytes = System.Text.Encoding.ASCII.GetBytes(checksumString);
        Array.Copy(checksumBytes, 0, header, 148, checksumBytes.Length);
        header[154] = (byte)' '; // Null terminator

        await tarStream.WriteAsync(header);
        await tarStream.WriteAsync(content);

        // Pad to block boundary
        var padding = BlockSize - (content.Length % BlockSize);
        if (padding < BlockSize)
        {
            await tarStream.WriteAsync(new byte[padding]);
        }
    }

    private TarHeader ParseTarHeader(byte[] header)
    {
        var fileName = System.Text.Encoding.ASCII.GetString(header.Take(100).ToArray()).TrimEnd('\0');

        var sizeStr = System.Text.Encoding.ASCII.GetString(header.Skip(124).Take(12).ToArray()).Trim();
        
        // Parse octal number manually
        var fileSize = ParseOctal(sizeStr);

        return new TarHeader
        {
            FileName = string.IsNullOrEmpty(fileName) ? null : fileName,
            FileSize = fileSize
        };
    }

    private long ParseOctal(string octal)
    {
        long result = 0;
        foreach (var c in octal.TrimStart('0'))
        {
            if (c >= '0' && c <= '7')
            {
                result = result * 8 + (c - '0');
            }
        }
        return result;
    }

    private record TarHeader
    {
        public string? FileName { get; init; }
        public long FileSize { get; init; }
    }
}

public record TarResult
{
    public int FilesProcessed { get; init; }
    public long TotalSize { get; init; }
    public long ArchiveSize { get; init; }
    public TimeSpan ElapsedTime { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== TAR Archive Handler ===\n");

        var service = new TarService();

        // Create test files
        var testDir = Path.Combine(Path.GetTempPath(), $"tar_test_{Guid.NewGuid()}");
        Directory.CreateDirectory(testDir);

        Console.WriteLine("Creating test files...");
        for (int i = 0; i < 3; i++)
        {
            var content = $"TAR test file {i} - " + new string('X', 1000);
            await File.WriteAllTextAsync(Path.Combine(testDir, $"file{i}.txt"), content);
        }

        var tarPath = Path.Combine(Path.GetTempPath(), $"test_archive_{Guid.NewGuid()}.tar");
        var extractDir = Path.Combine(Path.GetTempPath(), $"extracted_{Guid.NewGuid()}");

        // Create TAR
        Console.WriteLine("\nCreating TAR archive...");
        var createResult = await service.CreateTarAsync(testDir, tarPath);

        Console.WriteLine($"Files added: {createResult.FilesProcessed}");
        Console.WriteLine($"Total size: {createResult.TotalSize} bytes");
        Console.WriteLine($"Archive size: {createResult.ArchiveSize} bytes");
        Console.WriteLine($"Time: {createResult.ElapsedTime.TotalMilliseconds:F0}ms");

        // Extract TAR
        Console.WriteLine("\nExtracting TAR archive...");
        var extractResult = await service.ExtractTarAsync(tarPath, extractDir);

        Console.WriteLine($"Files extracted: {extractResult.FilesProcessed}");
        Console.WriteLine($"Total size: {extractResult.TotalSize} bytes");
        Console.WriteLine($"Time: {extractResult.ElapsedTime.TotalMilliseconds:F0}ms");

        // Verify
        var originalFiles = Directory.GetFiles(testDir).Length;
        var extractedFiles = Directory.GetFiles(extractDir).Length;
        var verified = originalFiles == extractedFiles;

        Console.WriteLine($"\nVerification: {(verified ? "✓ PASSED" : "✗ FAILED")}");

        // Cleanup
        Directory.Delete(testDir, true);
        Directory.Delete(extractDir, true);
        File.Delete(tarPath);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
