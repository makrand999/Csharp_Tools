using System;
using System.Text;

namespace StringBuilderDemo;

/// <summary>
/// Demonstrates StringBuilder for efficient string manipulation
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== StringBuilder: Efficient String Operations ===\n");

        // Demo 1: String concatenation vs StringBuilder
        Console.WriteLine("--- String Concatenation vs StringBuilder ---");
        
        // Inefficient string concatenation
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        string result = "";
        for (int i = 0; i < 10000; i++)
        {
            result += i.ToString();
        }
        stopwatch.Stop();
        Console.WriteLine($"String concatenation (10000 iterations): {stopwatch.ElapsedMilliseconds}ms");

        // Efficient StringBuilder
        stopwatch.Restart();
        StringBuilder sb = new StringBuilder();
        for (int i = 0; i < 10000; i++)
        {
            sb.Append(i);
        }
        string sbResult = sb.ToString();
        stopwatch.Stop();
        Console.WriteLine($"StringBuilder (10000 iterations): {stopwatch.ElapsedMilliseconds}ms");

        // Demo 2: Basic StringBuilder operations
        Console.WriteLine("\n--- Basic StringBuilder Operations ---");
        StringBuilder builder = new StringBuilder();
        
        builder.Append("Hello");
        builder.Append(" ");
        builder.Append("World");
        builder.AppendLine("!");
        builder.Append("C# is ");
        builder.Append("awesome");
        
        Console.WriteLine($"Content: {builder}");
        Console.WriteLine($"Length: {builder.Length}");
        Console.WriteLine($"Capacity: {builder.Capacity}");

        // Demo 3: StringBuilder with capacity
        Console.WriteLine("\n--- Pre-allocated Capacity ---");
        StringBuilder preAllocated = new StringBuilder(1000);
        Console.WriteLine($"Initial capacity: {preAllocated.Capacity}");
        
        for (int i = 0; i < 50; i++)
        {
            preAllocated.Append("Item");
        }
        Console.WriteLine($"After 50 appends: {preAllocated.Length} chars, capacity: {preAllocated.Capacity}");

        // Demo 4: Insert, Remove, Replace
        Console.WriteLine("\n--- Modify Operations ---");
        StringBuilder modify = new StringBuilder("Hello World!");
        Console.WriteLine($"Original: {modify}");
        
        modify.Insert(5, " Beautiful");
        Console.WriteLine($"After Insert: {modify}");
        
        modify.Remove(5, 10);
        Console.WriteLine($"After Remove: {modify}");
        
        modify.Replace("World", "Universe");
        Console.WriteLine($"After Replace: {modify}");

        // Demo 5: StringBuilder with formatting
        Console.WriteLine("\n--- String Formatting ---");
        StringBuilder formatted = new StringBuilder();
        formatted.AppendFormat("Name: {0}\n", "Alice");
        formatted.AppendFormat("Age: {0}\n", 30);
        formatted.AppendFormat("Score: {0:F2}\n", 95.5);
        Console.WriteLine(formatted);

        // Demo 6: Building CSV content
        Console.WriteLine("--- Building CSV Content ---");
        StringBuilder csv = new StringBuilder();
        string[] headers = { "ID", "Name", "Email", "Age" };
        string[,] data = {
            { "1", "Alice", "alice@example.com", "30" },
            { "2", "Bob", "bob@example.com", "25" },
            { "3", "Charlie", "charlie@example.com", "35" }
        };

        // Header row
        csv.AppendLine(string.Join(",", headers));
        
        // Data rows
        for (int i = 0; i < data.GetLength(0); i++)
        {
            for (int j = 0; j < data.GetLength(1); j++)
            {
                csv.Append(data[i, j]);
                if (j < data.GetLength(1) - 1)
                {
                    csv.Append(",");
                }
            }
            csv.AppendLine();
        }
        Console.WriteLine(csv.ToString());

        // Demo 7: Building HTML
        Console.WriteLine("--- Building HTML ---");
        StringBuilder html = new StringBuilder();
        html.AppendLine("<ul>");
        
        string[] items = { "Apple", "Banana", "Cherry" };
        foreach (string item in items)
        {
            html.AppendLine($"  <li>{item}</li>");
        }
        html.AppendLine("</ul>");
        Console.WriteLine(html.ToString());

        // Demo 8: StringBuilder with conditional content
        Console.WriteLine("--- Conditional Content ---");
        StringBuilder report = new StringBuilder();
        report.AppendLine("=== User Report ===");
        
        bool hasErrors = false;
        bool hasWarnings = true;
        
        if (hasErrors)
        {
            report.AppendLine("ERRORS FOUND:");
            report.AppendLine("  - Critical error 1");
        }
        
        if (hasWarnings)
        {
            report.AppendLine("WARNINGS:");
            report.AppendLine("  - Minor warning 1");
            report.AppendLine("  - Minor warning 2");
        }
        
        if (!hasErrors && !hasWarnings)
        {
            report.AppendLine("All systems operational.");
        }
        
        report.AppendLine("===================");
        Console.WriteLine(report);

        // Demo 9: StringBuilder pooling (reuse)
        Console.WriteLine("--- Reusing StringBuilder ---");
        StringBuilder pool = new StringBuilder();
        
        for (int i = 1; i <= 3; i++)
        {
            pool.Clear();
            pool.Append($"Message {i}: ");
            for (int j = 1; j <= i; j++)
            {
                pool.Append($"Part{j} ");
            }
            Console.WriteLine(pool.ToString());
        }

        // Demo 10: StringBuilder with char operations
        Console.WriteLine("\n--- Character Operations ---");
        StringBuilder chars = new StringBuilder("abcdef");
        Console.WriteLine($"Original: {chars}");
        Console.WriteLine($"Char at index 2: {chars[2]}");
        
        chars[2] = 'X';
        Console.WriteLine($"After chars[2] = 'X': {chars}");
        
        // Demo 11: Large text generation
        Console.WriteLine("\n--- Large Text Generation ---");
        StringBuilder largeText = new StringBuilder(10000);
        
        for (int line = 1; line <= 10; line++)
        {
            largeText.Append($"Line {line}: ");
            for (int word = 1; word <= 10; word++)
            {
                largeText.Append($"word{word} ");
            }
            largeText.AppendLine();
        }
        
        Console.WriteLine($"Generated {largeText.Length} characters");
        Console.WriteLine($"First 100 chars: {largeText.ToString(0, 100)}...");

        // Demo 12: ToString with range
        Console.WriteLine("\n--- ToString with Range ---");
        StringBuilder range = new StringBuilder("0123456789");
        Console.WriteLine($"Full: {range}");
        Console.WriteLine($"Substring(0,5): {range.ToString(0, 5)}");
        Console.WriteLine($"Substring(5,5): {range.ToString(5, 5)}");
    }
}
