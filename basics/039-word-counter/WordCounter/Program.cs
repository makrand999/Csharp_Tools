using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace WordCounter;

class Program
{
    static void Main(string[] args)
    {
        string text;

        if (args.Length > 0)
        {
            string path = args[0];
            if (File.Exists(path))
            {
                try
                {
                    text = File.ReadAllText(path);
                    Console.WriteLine($"Analyzing file: {path}");
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"Error reading file: {ex.Message}");
                    return;
                }
            }
            else
            {
                text = string.Join(" ", args);
            }
        }
        else
        {
            Console.WriteLine("Enter text to analyze (Ctrl+D on Linux/Mac or Ctrl+Z on Windows to end):");
            text = Console.In.ReadToEnd();
        }

        var stats = AnalyzeText(text);
        PrintStats(stats);
    }

    static TextStats AnalyzeText(string text)
    {
        string[] words = Regex.Matches(text, @"\b[\w'-]+\b")
            .Select(m => m.Value.ToLowerInvariant())
            .ToArray();

        var charCounts = new CharCounts
        {
            Total = text.Length,
            WithSpaces = text.Replace("\r", "").Replace("\n", "").Length,
            WithoutSpaces = text.Count(c => !char.IsWhiteSpace(c)),
            Letters = text.Count(char.IsLetter),
            Digits = text.Count(char.IsDigit),
            Spaces = text.Count(c => c == ' '),
            Punctuation = text.Count(char.IsPunctuation)
        };

        var wordFreq = words.GroupBy(w => w)
            .OrderByDescending(g => g.Count())
            .Take(10)
            .ToDictionary(g => g.Key, g => g.Count());

        int sentences = Regex.Matches(text, @"[.!?]+").Count;
        int paragraphs = text.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.RemoveEmptyEntries)
            .Count(p => !string.IsNullOrWhiteSpace(p));

        return new TextStats
        {
            Characters = charCounts,
            Words = words.Length,
            UniqueWords = words.Distinct().Count(),
            Sentences = sentences,
            Paragraphs = paragraphs,
            TopWords = wordFreq
        };
    }

    static void PrintStats(TextStats stats)
    {
        Console.WriteLine();
        Console.WriteLine("=== Text Statistics ===");
        Console.WriteLine();

        Console.WriteLine("Characters:");
        Console.WriteLine($"  Total:         {stats.Characters.Total}");
        Console.WriteLine($"  Without spaces: {stats.Characters.WithoutSpaces}");
        Console.WriteLine($"  Letters:       {stats.Characters.Letters}");
        Console.WriteLine($"  Digits:        {stats.Characters.Digits}");
        Console.WriteLine($"  Spaces:        {stats.Characters.Spaces}");
        Console.WriteLine($"  Punctuation:   {stats.Characters.Punctuation}");
        Console.WriteLine();

        Console.WriteLine("Words:");
        Console.WriteLine($"  Total:   {stats.Words}");
        Console.WriteLine($"  Unique:  {stats.UniqueWords}");
        Console.WriteLine();

        Console.WriteLine("Structure:");
        Console.WriteLine($"  Sentences:   {stats.Sentences}");
        Console.WriteLine($"  Paragraphs:  {stats.Paragraphs}");

        if (stats.Words > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Top 10 Words:");
            int i = 1;
            foreach (var kvp in stats.TopWords)
            {
                Console.WriteLine($"  {i++,2}. {kvp.Key,-15} {kvp.Value}x");
            }
        }
    }
}

record TextStats
{
    public required CharCounts Characters { get; init; }
    public int Words { get; init; }
    public int UniqueWords { get; init; }
    public int Sentences { get; init; }
    public int Paragraphs { get; init; }
    public Dictionary<string, int> TopWords { get; init; } = new();
}

record CharCounts
{
    public int Total { get; init; }
    public int WithSpaces { get; init; }
    public int WithoutSpaces { get; init; }
    public int Letters { get; init; }
    public int Digits { get; init; }
    public int Spaces { get; init; }
    public int Punctuation { get; init; }
}
