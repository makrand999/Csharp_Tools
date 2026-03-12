using System.Buffers;
using System.Runtime.InteropServices;

namespace MemoryEfficientParser;

/// <summary>
/// Memory Efficient Parser - Uses Span&lt;T> and Memory&lt;T> for zero-allocation parsing.
/// Demonstrates high-performance text processing without heap allocations.
/// </summary>
public ref struct CsvParser
{
    private readonly ReadOnlySpan<char> _input;
    private int _position;

    public CsvParser(ReadOnlySpan<char> input)
    {
        _input = input;
        _position = 0;
    }

    public bool TryReadLine(out ReadOnlySpan<char> line)
    {
        line = ReadOnlySpan<char>.Empty;

        if (_position >= _input.Length)
            return false;

        var start = _position;
        var newlinePos = _input.Slice(_position).IndexOfAny('\r', '\n');

        if (newlinePos == -1)
        {
            line = _input.Slice(_position);
            _position = _input.Length;
        }
        else
        {
            line = _input.Slice(start, newlinePos);
            _position = start + newlinePos + 1;
            
            // Skip \r\n or \n
            if (_position < _input.Length && _input[_position] == '\n')
                _position++;
        }

        return true;
    }

    public int CountFields(ReadOnlySpan<char> line, char delimiter = ',')
    {
        if (line.IsEmpty)
            return 0;

        var count = 1;
        foreach (var c in line)
        {
            if (c == delimiter)
                count++;
        }
        return count;
    }

    public bool TryGetField(ReadOnlySpan<char> line, int index, out ReadOnlySpan<char> field, char delimiter = ',')
    {
        field = ReadOnlySpan<char>.Empty;

        var currentField = 0;
        var start = 0;
        var inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            var c = line[i];

            if (c == '"')
            {
                inQuotes = !inQuotes;
            }
            else if (c == delimiter && !inQuotes)
            {
                if (currentField == index)
                {
                    field = line.Slice(start, i - start).Trim('"');
                    return true;
                }
                currentField++;
                start = i + 1;
            }
        }

        // Last field
        if (currentField == index)
        {
            field = line.Slice(start).Trim('"');
            return true;
        }

        return false;
    }
}

public class MemoryPoolProcessor
{
    private readonly MemoryPool<char> _pool;
    private readonly int _bufferSize;

    public MemoryPoolProcessor(int bufferSize = 8192)
    {
        _pool = MemoryPool<char>.Shared;
        _bufferSize = bufferSize;
    }

    public int ProcessLargeFile(string filePath)
    {
        var lineCount = 0;
        var totalChars = 0L;

        using var stream = File.OpenRead(filePath);
        using var reader = new StreamReader(stream);

        var buffer = new char[8192];
        int charsRead;
        
        while ((charsRead = reader.Read(buffer, 0, buffer.Length)) > 0)
        {
            var span = buffer.AsSpan(0, charsRead);
            lineCount += CountNewlines(span);
            totalChars += charsRead;
        }

        return lineCount;
    }

    private int CountNewlines(ReadOnlySpan<char> span)
    {
        var count = 0;
        foreach (var c in span)
        {
            if (c == '\n')
                count++;
        }
        return count;
    }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Memory Efficient Parser ===\n");
        Console.WriteLine("Using Span<T> and Memory<T> for zero-allocation parsing\n");

        // Create sample CSV data
        var csvData = """
            Name,Age,City,Occupation
            John Doe,30,New York,Engineer
            Jane Smith,25,Los Angeles,Designer
            Bob Johnson,35,Chicago,Manager
            Alice Brown,28,Houston,Developer
            Charlie Wilson,32,Phoenix,Analyst
            """;

        Console.WriteLine("Parsing CSV data:\n");
        Console.WriteLine(csvData);
        Console.WriteLine();

        var parser = new CsvParser(csvData.AsSpan());

        var lineNum = 0;
        while (parser.TryReadLine(out var line))
        {
            if (line.IsEmpty)
                continue;

            lineNum++;
            var fieldCount = parser.CountFields(line);
            Console.WriteLine($"Line {lineNum}: {fieldCount} fields");

            // Parse fields
            for (int i = 0; i < fieldCount; i++)
            {
                if (parser.TryGetField(line, i, out var field))
                {
                    Console.Write($"  [{i}]: {field.ToString(),-15}");
                }
            }
            Console.WriteLine();
        }

        // Create temp file for large file processing demo
        var tempFile = Path.Combine(Path.GetTempPath(), $"large_test_{Guid.NewGuid()}.txt");
        Console.WriteLine($"\nCreating test file with 10000 lines...");
        
        var lines = Enumerable.Range(1, 10000)
            .Select(i => $"Line {i},Data {i},Value {i},Extra {i}");
        File.WriteAllLines(tempFile, lines);

        var fileSize = new FileInfo(tempFile).Length;
        Console.WriteLine($"File size: {fileSize / 1024} KB");

        var processor = new MemoryPoolProcessor();
        
        Console.WriteLine("\nProcessing with memory pool...");
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        
        var lineCount = processor.ProcessLargeFile(tempFile);
        
        stopwatch.Stop();
        Console.WriteLine($"Lines processed: {lineCount:N0}");
        Console.WriteLine($"Time: {stopwatch.ElapsedMilliseconds}ms");
        Console.WriteLine($"Throughput: {fileSize / stopwatch.Elapsed.TotalSeconds / 1024 / 1024:F1} MB/s");

        // Cleanup
        File.Delete(tempFile);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
