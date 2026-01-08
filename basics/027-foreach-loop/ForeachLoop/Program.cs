using System;
using System.Collections;
using System.Collections.Generic;

namespace ForeachLoop;

/// <summary>
/// Demonstrates foreach loops, IEnumerable, and IEnumerator
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Foreach Loop, IEnumerable & IEnumerator ===\n");

        // Demo 1: Basic foreach with arrays
        Console.WriteLine("--- Foreach with Arrays ---");
        int[] numbers = { 1, 2, 3, 4, 5 };
        
        Console.Write("Array elements: ");
        foreach (int num in numbers)
        {
            Console.Write($"{num} ");
        }
        Console.WriteLine();

        // Demo 2: Foreach with lists
        Console.WriteLine("\n--- Foreach with Lists ---");
        List<string> names = new() { "Alice", "Bob", "Charlie", "Diana" };
        
        foreach (string name in names)
        {
            Console.WriteLine($"  Hello, {name}!");
        }

        // Demo 3: Foreach with dictionaries
        Console.WriteLine("\n--- Foreach with Dictionaries ---");
        Dictionary<string, int> scores = new()
        {
            { "Alice", 95 },
            { "Bob", 87 },
            { "Charlie", 92 }
        };

        foreach (KeyValuePair<string, int> entry in scores)
        {
            Console.WriteLine($"  {entry.Key}: {entry.Value}");
        }

        // Demo 4: Foreach with deconstruction (C# 7+)
        Console.WriteLine("\n--- Foreach with Deconstruction ---");
        foreach (var (key, value) in scores)
        {
            Console.WriteLine($"  {key} scored {value}");
        }

        // Demo 5: Foreach with custom collection (IEnumerable)
        Console.WriteLine("\n--- Custom IEnumerable Collection ---");
        var customCollection = new NumberCollection { 10, 20, 30, 40, 50 };
        
        foreach (int num in customCollection)
        {
            Console.Write($"{num} ");
        }
        Console.WriteLine();

        // Demo 6: Manual iteration with IEnumerator
        Console.WriteLine("\n--- Manual IEnumerator Usage ---");
        var list = new List<int> { 100, 200, 300 };
        using (IEnumerator<int> enumerator = list.GetEnumerator())
        {
            while (enumerator.MoveNext())
            {
                Console.Write($"{enumerator.Current} ");
            }
        }
        Console.WriteLine();

        // Demo 7: Foreach with filtering
        Console.WriteLine("\n--- Foreach with Filtering ---");
        int[] data = { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
        
        Console.Write("Even numbers: ");
        foreach (int num in data)
        {
            if (num % 2 == 0)
            {
                Console.Write($"{num} ");
            }
        }
        Console.WriteLine();

        // Demo 8: Foreach with index (using Enumerate)
        Console.WriteLine("\n--- Foreach with Index ---");
        int index = 0;
        foreach (string name in names)
        {
            Console.WriteLine($"  [{index}] {name}");
            index++;
        }

        // Demo 9: Nested foreach
        Console.WriteLine("\n--- Nested Foreach Loops ---");
        int[,] matrix = { { 1, 2, 3 }, { 4, 5, 6 }, { 7, 8, 9 } };
        
        for (int i = 0; i < matrix.GetLength(0); i++)
        {
            for (int j = 0; j < matrix.GetLength(1); j++)
            {
                Console.Write($"{matrix[i, j]} ");
            }
            Console.WriteLine();
        }

        // Demo 10: Foreach with readonly collections
        Console.WriteLine("\n--- Foreach with Readonly Collections ---");
        var readonlyList = new ReadOnlyNumberCollection();
        
        foreach (int num in readonlyList)
        {
            Console.Write($"{num} ");
        }
        Console.WriteLine();

        // Demo 11: Yield return demonstration
        Console.WriteLine("\n--- Yield Return (Lazy Evaluation) ---");
        Console.Write("First 5 squares: ");
        foreach (int square in GetSquares(5))
        {
            Console.Write($"{square} ");
        }
        Console.WriteLine();

        // Demo 12: Foreach with break and continue
        Console.WriteLine("\n--- Foreach with Break/Continue ---");
        Console.Write("Skip 3, stop at 7: ");
        foreach (int num in data)
        {
            if (num == 3)
            {
                continue; // Skip 3
            }
            if (num == 7)
            {
                break; // Stop at 7
            }
            Console.Write($"{num} ");
        }
        Console.WriteLine();
    }

    /// <summary>
    /// Iterator method using yield return
    /// </summary>
    static IEnumerable<int> GetSquares(int count)
    {
        for (int i = 1; i <= count; i++)
        {
            yield return i * i;
        }
    }
}

/// <summary>
/// Custom collection implementing IEnumerable
/// </summary>
class NumberCollection : IEnumerable<int>
{
    private readonly List<int> _numbers = new();

    public void Add(int number) => _numbers.Add(number);

    public IEnumerator<int> GetEnumerator() => _numbers.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}

/// <summary>
/// Custom collection with custom enumerator
/// </summary>
class ReadOnlyNumberCollection : IEnumerable<int>
{
    private readonly int[] _data = { 1, 2, 3, 4, 5 };

    public IEnumerator<int> GetEnumerator()
    {
        for (int i = 0; i < _data.Length; i++)
        {
            yield return _data[i] * 2; // Return doubled values
        }
    }

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
