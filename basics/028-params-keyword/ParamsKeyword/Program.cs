using System;
using System.Linq;

namespace ParamsKeyword;

/// <summary>
/// Demonstrates the params keyword for variable-length argument lists
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Params Keyword: Variable Arguments ===\n");

        // Demo 1: Basic params usage
        Console.WriteLine("--- Basic Params Usage ---");
        Console.WriteLine($"Sum of 1, 2, 3: {Sum(1, 2, 3)}");
        Console.WriteLine($"Sum of 1, 2, 3, 4, 5: {Sum(1, 2, 3, 4, 5)}");
        Console.WriteLine($"Sum of 10, 20, 30, 40, 50, 60: {Sum(10, 20, 30, 40, 50, 60)}");

        // Demo 2: Params with arrays
        Console.WriteLine("\n--- Params with Arrays ---");
        int[] numbers = { 100, 200, 300, 400 };
        Console.WriteLine($"Sum from array: {Sum(numbers)}");

        // Demo 3: Params with strings
        Console.WriteLine("\n--- Params with Strings ---");
        Console.WriteLine($"Concatenate: {Concatenate("Hello", "World", "CSharp")}");
        Console.WriteLine($"Concatenate with separator: {ConcatenateWithSeparator("-", "A", "B", "C")}");

        // Demo 4: Params with mixed parameters
        Console.WriteLine("\n--- Params with Mixed Parameters ---");
        Console.WriteLine(FormatMessage("INFO", "User logged in", "admin", "192.168.1.1"));
        Console.WriteLine(FormatMessage("ERROR", "Connection failed", "server", "timeout"));

        // Demo 5: Multiple methods with params
        Console.WriteLine("\n--- Multiple Params Methods ---");
        Console.WriteLine($"Average: {Average(10, 20, 30, 40, 50)}");
        Console.WriteLine($"Max: {FindMax(5, 12, 8, 95, 3, 47)}");
        Console.WriteLine($"Min: {FindMin(5, 12, 8, 95, 3, 47)}");

        // Demo 6: Params with different types
        Console.WriteLine("\n--- Params with Different Types ---");
        PrintAll(1, "hello", 3.14, true, 'X');
        PrintAll("only", "strings", "here");
        PrintAll(100, 200, 300);

        // Demo 7: Params with LINQ
        Console.WriteLine("\n--- Params with LINQ ---");
        Console.WriteLine($"Even numbers: {string.Join(", ", GetEvenNumbers(1, 2, 3, 4, 5, 6, 7, 8, 9, 10))}");
        Console.WriteLine($"Squares: {string.Join(", ", GetSquares(1, 2, 3, 4, 5))}");

        // Demo 8: Chaining params methods
        Console.WriteLine("\n--- Chaining Params Methods ---");
        int[] data = { 1, 2, 3, 4, 5 };
        Console.WriteLine($"Sum of squares: {Sum(GetSquaresArray(data))}");

        // Demo 9: Params with optional parameters
        Console.WriteLine("\n--- Params with Optional Parameters ---");
        Console.WriteLine(Calculate("Result", 10, 20, 30));
        Console.WriteLine(Calculate("Total", 100, 200));
        Console.WriteLine(Calculate("Sum", 5, 10, 15, 20, 25));

        // Demo 10: Real-world example - logging
        Console.WriteLine("\n--- Real-world: Logging ---");
        var logger = new SimpleLogger();
        logger.Log("Application started");
        logger.Log("Processing", "user123", "login");
        logger.Log("Error", "Database", "Connection timeout", "Retry: 3");
    }

    /// <summary>
    /// Sums any number of integers
    /// </summary>
    static int Sum(params int[] numbers)
    {
        int total = 0;
        foreach (int num in numbers)
        {
            total += num;
        }
        return total;
    }

    /// <summary>
    /// Concatenates strings without separator
    /// </summary>
    static string Concatenate(params string[] texts)
    {
        return string.Join("", texts);
    }

    /// <summary>
    /// Concatenates strings with a separator
    /// </summary>
    static string ConcatenateWithSeparator(string separator, params string[] texts)
    {
        return string.Join(separator, texts);
    }

    /// <summary>
    /// Formats a log message with level and details
    /// </summary>
    static string FormatMessage(string level, string message, params string[] details)
    {
        string detailStr = details.Length > 0 ? $" [{string.Join(", ", details)}]" : "";
        return $"[{DateTime.Now:HH:mm:ss}] {level}: {message}{detailStr}";
    }

    /// <summary>
    /// Calculates average of numbers
    /// </summary>
    static double Average(params int[] numbers)
    {
        if (numbers.Length == 0) return 0;
        return numbers.Average();
    }

    /// <summary>
    /// Finds maximum value
    /// </summary>
    static int FindMax(params int[] numbers)
    {
        if (numbers.Length == 0) return 0;
        return numbers.Max();
    }

    /// <summary>
    /// Finds minimum value
    /// </summary>
    static int FindMin(params int[] numbers)
    {
        if (numbers.Length == 0) return 0;
        return numbers.Min();
    }

    /// <summary>
    /// Prints all arguments regardless of type
    /// </summary>
    static void PrintAll(params object[] items)
    {
        Console.Write("  Items: ");
        foreach (var item in items)
        {
            Console.Write($"{item}({item.GetType().Name}) ");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Returns even numbers from input
    /// </summary>
    static int[] GetEvenNumbers(params int[] numbers)
    {
        return numbers.Where(n => n % 2 == 0).ToArray();
    }

    /// <summary>
    /// Returns squares of numbers
    /// </summary>
    static int[] GetSquaresArray(params int[] numbers)
    {
        return numbers.Select(n => n * n).ToArray();
    }

    /// <summary>
    /// Returns squares using yield
    /// </summary>
    static IEnumerable<int> GetSquares(params int[] numbers)
    {
        foreach (int num in numbers)
        {
            yield return num * num;
        }
    }

    /// <summary>
    /// Calculation with label and optional prefix
    /// </summary>
    static string Calculate(string label, int prefix = 0, params int[] numbers)
    {
        int sum = numbers.Sum() + prefix;
        return $"{label}: {sum}";
    }
}

/// <summary>
/// Simple logger using params for flexible logging
/// </summary>
class SimpleLogger
{
    public void Log(params string[] messages)
    {
        string timestamp = DateTime.Now.ToString("HH:mm:ss");
        string content = string.Join(" | ", messages);
        Console.WriteLine($"[{timestamp}] {content}");
    }
}
