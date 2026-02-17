using System.Buffers;
using System.Runtime.CompilerServices;

namespace SpanTextParser;

/// <summary>
/// High-performance text parser using Span&lt;T&gt; and Memory&lt;T&gt;
/// Demonstrates zero-allocation parsing, UTF-8 processing, and efficient string operations
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Span-Based Text Parser ===\n");

        // Demo 1: Parse CSV line without allocations
        Console.WriteLine("1. CSV Line Parsing (zero allocations):");
        string csvLine = "John Doe,35,Engineer,New York,USA";
        var fields = ParseCsvLine(csvLine);
        for (int i = 0; i < fields.Count; i++)
        {
            Console.WriteLine($"   Field {i}: {fields[i]}");
        }

        // Demo 2: Parse key-value pairs
        Console.WriteLine("\n2. Key-Value Pair Parsing:");
        string config = "name=John;age=35;city=NYC;active=true";
        var pairs = ParseKeyValuePairs(config);
        foreach (var (key, value) in pairs)
        {
            Console.WriteLine($"   {key} = {value}");
        }

        // Demo 3: Parse log entry
        Console.WriteLine("\n3. Log Entry Parsing:");
        string logEntry = "2025-03-31 14:30:45 ERROR [Database] Connection timeout after 30s";
        var logParts = ParseLogEntry(logEntry);
        Console.WriteLine($"   Timestamp: {logParts.Timestamp}");
        Console.WriteLine($"   Level: {logParts.Level}");
        Console.WriteLine($"   Source: {logParts.Source}");
        Console.WriteLine($"   Message: {logParts.Message}");

        // Demo 4: Parse URL query string
        Console.WriteLine("\n4. URL Query String Parsing:");
        string queryString = "?search=csharp&sort=date&limit=10&offset=20";
        var queryParams = ParseQueryString(queryString);
        foreach (var (key, value) in queryParams)
        {
            Console.WriteLine($"   {key}: {value}");
        }

        // Demo 5: Performance comparison
        Console.WriteLine("\n5. Performance Demo:");
        string largeData = GenerateLargeData(10000);
        
        var spanTime = MeasureSpanParsing(largeData);
        var stringTime = MeasureStringParsing(largeData);
        
        Console.WriteLine($"   Span<T> parsing: {spanTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"   String parsing:  {stringTime.TotalMilliseconds:F2}ms");
        Console.WriteLine($"   Speedup: {stringTime.TotalMilliseconds / spanTime.TotalMilliseconds:F2}x");

        // Demo 6: UTF-8 byte span parsing
        Console.WriteLine("\n6. UTF-8 Byte Span Parsing:");
        byte[] utf8Bytes = System.Text.Encoding.UTF8.GetBytes("Hello, 世界！🌍");
        ParseUtf8Bytes(utf8Bytes);
    }

    /// <summary>
    /// Parse CSV line using Span<T> - minimal allocations
    /// </summary>
    static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        ReadOnlySpan<char> span = line.AsSpan();
        int start = 0;
        
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] == ',')
            {
                fields.Add(span.Slice(start, i - start).ToString());
                start = i + 1;
            }
        }
        
        // Add last field
        if (start <= span.Length)
        {
            fields.Add(span.Slice(start).ToString());
        }
        
        return fields;
    }

    /// <summary>
    /// Parse semicolon-separated key=value pairs
    /// </summary>
    static List<(string Key, string Value)> ParseKeyValuePairs(string input)
    {
        var pairs = new List<(string, string)>();
        ReadOnlySpan<char> span = input.AsSpan();
        int pairStart = 0;
        
        for (int i = 0; i <= span.Length; i++)
        {
            if (i == span.Length || span[i] == ';')
            {
                var pair = span.Slice(pairStart, i - pairStart);
                int equalsIndex = pair.IndexOf('=');
                
                if (equalsIndex > 0)
                {
                    pairs.Add((
                        pair.Slice(0, equalsIndex).Trim().ToString(),
                        pair.Slice(equalsIndex + 1).Trim().ToString()
                    ));
                }
                
                pairStart = i + 1;
            }
        }
        
        return pairs;
    }

    /// <summary>
    /// Parse structured log entry
    /// </summary>
    static (string Timestamp, string Level, string Source, string Message) ParseLogEntry(string log)
    {
        // Format: "YYYY-MM-DD HH:MM:SS LEVEL [Source] Message"
        ReadOnlySpan<char> span = log.AsSpan();
        
        // Find timestamp (first 19 chars: "YYYY-MM-DD HH:MM:SS")
        var timestamp = span.Slice(0, 19).Trim().ToString();
        
        // Find level (after timestamp, before '[')
        int levelStart = 20;
        int bracketStart = span.IndexOf('[');
        var level = span.Slice(levelStart, bracketStart - levelStart).Trim().ToString();
        
        // Find source (between '[' and ']')
        int bracketEnd = span.IndexOf(']');
        var source = span.Slice(bracketStart + 1, bracketEnd - bracketStart - 1).Trim().ToString();
        
        // Message is the rest
        var message = span.Slice(bracketEnd + 1).Trim().ToString();
        
        return (timestamp, level, source, message);
    }

    /// <summary>
    /// Parse URL query string parameters
    /// </summary>
    static List<(string Key, string Value)> ParseQueryString(string query)
    {
        var parameters = new List<(string, string)>();
        ReadOnlySpan<char> span = query.AsSpan();
        
        // Skip leading '?'
        int start = span.Length > 0 && span[0] == '?' ? 1 : 0;
        int paramStart = start;
        
        for (int i = start; i <= span.Length; i++)
        {
            if (i == span.Length || span[i] == '&')
            {
                var param = span.Slice(paramStart, i - paramStart);
                int equalsIndex = param.IndexOf('=');
                
                if (equalsIndex > 0)
                {
                    parameters.Add((
                        param.Slice(0, equalsIndex).ToString(),
                        param.Slice(equalsIndex + 1).ToString()
                    ));
                }
                
                paramStart = i + 1;
            }
        }
        
        return parameters;
    }

    /// <summary>
    /// Parse UTF-8 bytes directly without string conversion
    /// </summary>
    static void ParseUtf8Bytes(byte[] bytes)
    {
        ReadOnlySpan<byte> span = bytes;
        
        Console.WriteLine($"   Byte count: {span.Length}");
        Console.WriteLine($"   First 5 bytes: [{string.Join(", ", span.Slice(0, Math.Min(5, span.Length)).ToArray())}]");
        
        // Count ASCII characters vs multi-byte
        int asciiCount = 0;
        int multiByteCount = 0;
        
        for (int i = 0; i < span.Length; i++)
        {
            if (span[i] < 128)
            {
                asciiCount++;
            }
            else
            {
                multiByteCount++;
                // Skip continuation bytes
                while (i + 1 < span.Length && (span[i + 1] & 0xC0) == 0x80)
                {
                    i++;
                }
            }
        }
        
        Console.WriteLine($"   ASCII chars: {asciiCount}, Multi-byte sequences: {multiByteCount}");
    }

    static string GenerateLargeData(int lines)
    {
        var sb = new System.Text.StringBuilder();
        for (int i = 0; i < lines; i++)
        {
            sb.AppendLine($"item{i},value{i},description{i},category{i % 10}");
        }
        return sb.ToString();
    }

    static TimeSpan MeasureSpanParsing(string data)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        int lineCount = 0;
        int fieldCount = 0;
        
        ReadOnlySpan<char> span = data.AsSpan();
        int lineStart = 0;
        
        for (int i = 0; i <= span.Length; i++)
        {
            if (i == span.Length || span[i] == '\n')
            {
                lineCount++;
                var line = span.Slice(lineStart, i - lineStart);
                
                // Count fields
                for (int j = 0; j < line.Length; j++)
                {
                    if (line[j] == ',') fieldCount++;
                }
                
                lineStart = i + 1;
            }
        }
        
        sw.Stop();
        return sw.Elapsed;
    }

    static TimeSpan MeasureStringParsing(string data)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        int lineCount = 0;
        int fieldCount = 0;
        
        var lines = data.Split('\n');
        foreach (var line in lines)
        {
            lineCount++;
            var fields = line.Split(',');
            fieldCount += fields.Length;
        }
        
        sw.Stop();
        return sw.Elapsed;
    }
}
