using System;
using System.Collections.Generic;
using System.IO;
using System.IO.MemoryMappedFiles;
using System.Linq;
using System.Text;

namespace MemoryMappedFileReader;

/// <summary>
/// Memory-mapped file reader for efficient processing of large files.
/// Demonstrates MemoryMappedFile, Span&lt;T&gt;, and low-level memory access.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Memory-Mapped File Reader");
            Console.WriteLine("==========================");
            Console.WriteLine("Usage: dotnet run --project MemoryMappedFileReader.csproj <file> [options]");
            Console.WriteLine();
            Console.WriteLine("Operations:");
            Console.WriteLine("  info        - File information and structure");
            Console.WriteLine("  lines       - Count lines using memory mapping");
            Console.WriteLine("  words       - Count words using memory mapping");
            Console.WriteLine("  search      - Search for text pattern");
            Console.WriteLine("  hex         - Display hex dump of file");
            Console.WriteLine("  stats       - Character frequency analysis");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --op <operation>    Operation to perform (default: info)");
            Console.WriteLine("  --pattern <text>    Search pattern (for search operation)");
            Console.WriteLine("  --view <bytes>      Bytes to display for hex dump (default: 256)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project MemoryMappedFileReader.csproj largefile.log --op lines");
            Console.WriteLine("  dotnet run --project MemoryMappedFileReader.csproj data.bin --op hex --view 512");
            Console.WriteLine("  dotnet run --project MemoryMappedFileReader.csproj text.txt --op search --pattern ERROR");
            return 0;
        }

        var filePath = args[0];
        var operation = GetArgumentValue(args, "--op", "info").ToLower();
        var pattern = GetArgumentValue(args, "--pattern", string.Empty);
        var viewBytes = int.Parse(GetArgumentValue(args, "--view", "256"));

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File '{filePath}' does not exist.");
            return 1;
        }

        var fileInfo = new FileInfo(filePath);
        Console.WriteLine($"Memory-Mapped File Reader");
        Console.WriteLine($"==========================");
        Console.WriteLine($"File: {Path.GetFileName(filePath)}");
        Console.WriteLine($"Size: {FormatSize(fileInfo.Length)}");
        Console.WriteLine($"Operation: {operation}");
        Console.WriteLine();

        try
        {
            using var mmf = MemoryMappedFile.CreateFromFile(filePath, FileMode.Open, null, 0, MemoryMappedFileAccess.Read);

            switch (operation)
            {
                case "info":
                    DisplayFileInfo(mmf, filePath);
                    break;
                case "lines":
                    CountLines(mmf);
                    break;
                case "words":
                    CountWords(mmf);
                    break;
                case "search":
                    if (string.IsNullOrEmpty(pattern))
                    {
                        Console.WriteLine("Error: Please provide a search pattern with --pattern");
                        return 1;
                    }
                    SearchPattern(mmf, pattern);
                    break;
                case "hex":
                    DisplayHexDump(mmf, viewBytes);
                    break;
                case "stats":
                    DisplayCharStats(mmf);
                    break;
                default:
                    Console.WriteLine($"Error: Unknown operation '{operation}'");
                    return 1;
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static void DisplayFileInfo(MemoryMappedFile mmf, string filePath)
    {
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        var length = accessor.Capacity;

        Console.WriteLine($"File Information:");
        Console.WriteLine($"  Path: {Path.GetFullPath(filePath)}");
        Console.WriteLine($"  Size: {length} bytes ({FormatSize(length)})");
        Console.WriteLine($"  Memory mapped: Yes");
        Console.WriteLine($"  Access: Read-only");
        Console.WriteLine();

        // Detect file type by reading first bytes
        var header = new byte[Math.Min(16, length)];
        accessor.ReadArray(0, header, 0, header.Length);

        Console.WriteLine($"File Header (first 16 bytes):");
        Console.WriteLine($"  Hex: {BitConverter.ToString(header).Replace("-", " ")}");
        
        // Check for common signatures
        var signature = DetectFileSignature(header);
        if (!string.IsNullOrEmpty(signature))
        {
            Console.WriteLine($"  Type: {signature}");
        }

        // Check if text file
        var isText = IsTextFile(header);
        Console.WriteLine($"  Text file: {(isText ? "Yes" : "No/Binary")}");
    }

    static void CountLines(MemoryMappedFile mmf)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        long lineCount = 0;

        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        var length = accessor.Capacity;

        // Use Span<T> for efficient byte scanning
        var span = accessor.SafeMemoryMappedViewHandle;
        
        byte previousByte = 0;
        for (long i = 0; i < length; i++)
        {
            byte b = accessor.ReadByte(i);
            
            // Count \n that isn't preceded by \r (or standalone \n)
            if (b == (byte)'\n')
            {
                lineCount++;
            }
            
            previousByte = b;
        }

        // If file doesn't end with newline, count the last line
        if (length > 0 && accessor.ReadByte(length - 1) != (byte)'\n')
        {
            lineCount++;
        }

        stopwatch.Stop();

        Console.WriteLine($"Line Count (memory-mapped):");
        Console.WriteLine($"  Total lines: {lineCount:N0}");
        Console.WriteLine($"  Time: {stopwatch.ElapsedMilliseconds}ms");
    }

    static void CountWords(MemoryMappedFile mmf)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        long wordCount = 0;
        bool inWord = false;

        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        var length = accessor.Capacity;

        for (long i = 0; i < length; i++)
        {
            byte b = accessor.ReadByte(i);
            
            if (char.IsLetterOrDigit((char)b) || b == '_' || b == '-')
            {
                if (!inWord)
                {
                    wordCount++;
                    inWord = true;
                }
            }
            else
            {
                inWord = false;
            }
        }

        stopwatch.Stop();

        Console.WriteLine($"Word Count (memory-mapped):");
        Console.WriteLine($"  Total words: {wordCount:N0}");
        Console.WriteLine($"  Time: {stopwatch.ElapsedMilliseconds}ms");
    }

    static void SearchPattern(MemoryMappedFile mmf, string pattern)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var patternBytes = Encoding.UTF8.GetBytes(pattern);
        var matches = new List<long>();

        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        var length = accessor.Capacity;

        // Simple byte-by-byte search
        for (long i = 0; i <= length - patternBytes.Length; i++)
        {
            bool found = true;
            for (int j = 0; j < patternBytes.Length; j++)
            {
                if (accessor.ReadByte(i + j) != patternBytes[j])
                {
                    found = false;
                    break;
                }
            }

            if (found)
            {
                matches.Add(i);
            }
        }

        stopwatch.Stop();

        Console.WriteLine($"Search Results for '{pattern}':");
        Console.WriteLine($"  Matches found: {matches.Count:N0}");
        Console.WriteLine($"  Time: {stopwatch.ElapsedMilliseconds}ms");

        if (matches.Count > 0 && matches.Count <= 20)
        {
            Console.WriteLine($"\n  Match positions:");
            foreach (var pos in matches.Take(20))
            {
                Console.WriteLine($"    Offset: 0x{pos:X} ({pos})");
            }
        }
        else if (matches.Count > 20)
        {
            Console.WriteLine($"\n  First 20 match positions:");
            foreach (var pos in matches.Take(20))
            {
                Console.WriteLine($"    Offset: 0x{pos:X} ({pos})");
            }
        }
    }

    static void DisplayHexDump(MemoryMappedFile mmf, int bytesToDisplay)
    {
        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        var length = Math.Min(accessor.Capacity, bytesToDisplay);

        Console.WriteLine($"Hex Dump ({length} bytes):");
        Console.WriteLine(new string('-', 70));

        var bytes = new byte[16];
        
        for (long offset = 0; offset < length; offset += 16)
        {
            var bytesRead = (int)Math.Min(16, length - offset);
            accessor.ReadArray(offset, bytes, 0, bytesRead);

            // Offset column
            Console.Write($"{offset:X8}  ");

            // Hex values
            for (int i = 0; i < 16; i++)
            {
                if (i < bytesRead)
                    Console.Write($"{bytes[i]:X2} ");
                else
                    Console.Write("   ");
                
                if (i == 7) Console.Write(" ");
            }

            Console.Write(" |");

            // ASCII representation
            for (int i = 0; i < bytesRead; i++)
            {
                char c = (char)bytes[i];
                Console.Write(char.IsControl(c) ? '.' : c);
            }

            Console.WriteLine("|");
        }

        Console.WriteLine(new string('-', 70));
        Console.WriteLine($"Displayed {length} of {accessor.Capacity} total bytes");
    }

    static void DisplayCharStats(MemoryMappedFile mmf)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var charFrequency = new Dictionary<char, int>();
        int totalChars = 0;

        using var accessor = mmf.CreateViewAccessor(0, 0, MemoryMappedFileAccess.Read);
        var length = accessor.Capacity;

        for (long i = 0; i < length; i++)
        {
            byte b = accessor.ReadByte(i);
            
            if (b < 128) // ASCII only
            {
                char c = (char)b;
                if (!charFrequency.ContainsKey(c))
                    charFrequency[c] = 0;
                charFrequency[c]++;
                totalChars++;
            }
        }

        stopwatch.Stop();

        Console.WriteLine($"Character Frequency Analysis:");
        Console.WriteLine($"  Total ASCII characters: {totalChars:N0}");
        Console.WriteLine($"  Unique characters: {charFrequency.Count}");
        Console.WriteLine($"  Time: {stopwatch.ElapsedMilliseconds}ms");

        Console.WriteLine($"\n  Top 20 characters:");
        foreach (var kvp in charFrequency.OrderByDescending(k => k.Value).Take(20))
        {
            var display = char.IsControl(kvp.Key) ? $"\\x{(int)kvp.Key:X2}" : kvp.Key.ToString();
            var percent = (double)kvp.Value / totalChars * 100;
            Console.WriteLine($"    '{display,-4}' {kvp.Value,10:N0} ({percent,5:F2}%)");
        }

        // Line ending analysis
        var crCount = charFrequency.GetValueOrDefault('\r', 0);
        var lfCount = charFrequency.GetValueOrDefault('\n', 0);
        Console.WriteLine($"\n  Line endings:");
        Console.WriteLine($"    LF (\\n): {lfCount:N0}");
        Console.WriteLine($"    CR (\\r): {crCount:N0}");
        Console.WriteLine($"    Type: {(crCount == lfCount ? "Windows (CRLF)" : lfCount > crCount ? "Unix (LF)" : "Classic Mac (CR)")}");
    }

    static string? DetectFileSignature(byte[] header)
    {
        if (header.Length < 4) return null;

        // Check common magic numbers
        if (header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47)
            return "PNG Image";
        if (header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return "JPEG Image";
        if (header[0] == 0x25 && header[1] == 0x50 && header[2] == 0x44 && header[3] == 0x46)
            return "PDF Document";
        if (header[0] == 0x50 && header[1] == 0x4B && header[2] == 0x03 && header[3] == 0x04)
            return "ZIP Archive";
        if (header[0] == 0x1F && header[1] == 0x8B)
            return "GZIP Compressed";
        if (header[0] == 0x7F && header[1] == 0x45 && header[2] == 0x4C && header[3] == 0x46)
            return "ELF Executable";

        return null;
    }

    static bool IsTextFile(byte[] header)
    {
        // Check for BOM
        if (header.Length >= 3 && header[0] == 0xEF && header[1] == 0xBB && header[2] == 0xBF)
            return true;

        // Check if most bytes are printable ASCII
        int printableCount = 0;
        for (int i = 0; i < Math.Min(512, header.Length); i++)
        {
            byte b = header[i];
            if (b >= 32 && b <= 126 || b == 9 || b == 10 || b == 13)
                printableCount++;
        }

        return (double)printableCount / Math.Min(512, header.Length) > 0.8;
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
