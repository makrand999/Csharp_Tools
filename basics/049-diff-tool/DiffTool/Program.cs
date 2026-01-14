using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DiffTool;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Text Diff Tool");
            Console.WriteLine("Usage: dotnet run --project DiffTool.csproj [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  <file1> <file2>       Compare two files");
            Console.WriteLine("  --stdin               Compare stdin input with file");
            Console.WriteLine("  --string              Compare two strings from arguments");
            Console.WriteLine("  --lines               Show line-by-line diff");
            Console.WriteLine("  --summary             Show only summary (default)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project DiffTool.csproj file1.txt file2.txt");
            Console.WriteLine("  dotnet run --project DiffTool.csproj --string \"hello\" \"hallo\"");
            Console.WriteLine("  echo 'test' | dotnet run --project DiffTool.csproj --stdin file.txt");
            return 1;
        }

        try
        {
            if (args[0] == "--string" && args.Length >= 3)
            {
                return CompareStrings(args[1], args[2]);
            }
            else if (args[0] == "--stdin" && args.Length >= 2)
            {
                string stdin = Console.In.ReadToEnd();
                return CompareFiles(stdin, args[1]);
            }
            else if (args.Length >= 2)
            {
                bool showLines = args.Contains("--lines");
                int fileArgIndex = args.ToList().FindIndex(a => !a.StartsWith("--"));
                return CompareFiles(args[fileArgIndex], args[fileArgIndex + 1], showLines);
            }
            else
            {
                return CompareFiles(args[0], args[1]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static int CompareStrings(string str1, string str2)
    {
        Console.WriteLine("Comparing strings:");
        Console.WriteLine();
        Console.WriteLine($"String 1: \"{str1}\"");
        Console.WriteLine($"String 2: \"{str2}\"");
        Console.WriteLine();

        var diff = ComputeCharDiff(str1, str2);
        DisplayDiffResult(diff, str1, str2);

        return diff.AreIdentical ? 0 : 1;
    }

    static int CompareFiles(string file1, string file2, bool showLines = false)
    {
        string content1 = File.Exists(file1) ? File.ReadAllText(file1) : file1;
        string content2 = File.Exists(file2) ? File.ReadAllText(file2) : file2;

        if (File.Exists(file1) && File.Exists(file2))
        {
            Console.WriteLine($"Comparing files:");
            Console.WriteLine($"  File 1: {Path.GetFullPath(file1)}");
            Console.WriteLine($"  File 2: {Path.GetFullPath(file2)}");
        }
        Console.WriteLine();

        var lines1 = content1.Split('\n');
        var lines2 = content2.Split('\n');

        if (showLines)
        {
            DisplayLineDiff(lines1, lines2);
        }
        else
        {
            var charDiff = ComputeCharDiff(content1, content2);
            DisplayDiffResult(charDiff, content1, content2);
        }

        return content1 == content2 ? 0 : 1;
    }

    static DiffResult ComputeCharDiff(string str1, string str2)
    {
        var result = new DiffResult
        {
            AreIdentical = str1 == str2,
            Length1 = str1.Length,
            Length2 = str2.Length,
            LengthDiff = str2.Length - str1.Length
        };

        // Find first difference position
        int minLen = Math.Min(str1.Length, str2.Length);
        for (int i = 0; i < minLen; i++)
        {
            if (str1[i] != str2[i])
            {
                result.FirstDiffPosition = i;
                break;
            }
        }

        if (result.FirstDiffPosition == null && str1.Length != str2.Length)
        {
            result.FirstDiffPosition = minLen;
        }

        // Count character differences using LCS-based approach
        result.CharChanges = ComputeEditDistance(str1, str2);

        return result;
    }

    static void DisplayDiffResult(DiffResult diff, string str1, string str2)
    {
        Console.WriteLine("Diff Summary:");
        Console.WriteLine($"  Identical: {diff.AreIdentical}");
        Console.WriteLine($"  Length 1: {diff.Length1} characters");
        Console.WriteLine($"  Length 2: {diff.Length2} characters");
        Console.WriteLine($"  Length difference: {(diff.LengthDiff >= 0 ? "+" : "")}{diff.LengthDiff}");
        
        if (diff.FirstDiffPosition.HasValue)
        {
            Console.WriteLine($"  First difference at position: {diff.FirstDiffPosition}");
        }
        
        Console.WriteLine($"  Character changes: {diff.CharChanges}");
        Console.WriteLine();

        if (!diff.AreIdentical)
        {
            Console.WriteLine("Visual Diff:");
            Console.WriteLine();
            
            // Show side-by-side comparison around first diff
            int pos = diff.FirstDiffPosition ?? 0;
            int context = 10;
            int start = Math.Max(0, pos - context);
            int end = Math.Max(str1.Length, str2.Length);
            
            Console.WriteLine($"  Context around position {pos}:");
            Console.WriteLine($"  String 1: \"...{str1.Substring(start, Math.Min(end - start, str1.Length - start))}\"");
            Console.WriteLine($"  String 2: \"...{str2.Substring(start, Math.Min(end - start, str2.Length - start))}\"");
            Console.WriteLine();
            
            // Show character-level diff
            Console.WriteLine("  Character comparison:");
            var changes = GetCharChanges(str1, str2);
            foreach (var change in changes.Take(20))
            {
                Console.WriteLine($"    {change}");
            }
            if (changes.Count > 20)
            {
                Console.WriteLine($"    ... and {changes.Count - 20} more changes");
            }
        }
    }

    static void DisplayLineDiff(string[] lines1, string[] lines2)
    {
        Console.WriteLine("Line-by-line comparison:");
        Console.WriteLine();

        var changes = GetLineChanges(lines1, lines2);
        
        foreach (var change in changes)
        {
            Console.WriteLine(change);
        }

        Console.WriteLine();
        Console.WriteLine("Legend:");
        Console.WriteLine("  -  Line removed");
        Console.WriteLine("  +  Line added");
        Console.WriteLine("  ~  Line modified");
    }

    static List<string> GetCharChanges(string str1, string str2)
    {
        var changes = new List<string>();
        int minLen = Math.Min(str1.Length, str2.Length);

        for (int i = 0; i < minLen; i++)
        {
            if (str1[i] != str2[i])
            {
                changes.Add($"  [{i}] '{str1[i]}' → '{str2[i]}'");
            }
        }

        if (str1.Length > str2.Length)
        {
            for (int i = str2.Length; i < str1.Length; i++)
            {
                changes.Add($"  [{i}] '{str1[i]}' → (removed)");
            }
        }
        else if (str2.Length > str1.Length)
        {
            for (int i = str1.Length; i < str2.Length; i++)
            {
                changes.Add($"  [{i}] (added) → '{str2[i]}'");
            }
        }

        return changes;
    }

    static List<string> GetLineChanges(string[] lines1, string[] lines2)
    {
        var changes = new List<string>();
        var set1 = new HashSet<string>(lines1);
        var set2 = new HashSet<string>(lines2);

        // Lines only in file1 (removed)
        foreach (var line in lines1)
        {
            if (!set2.Contains(line))
            {
                changes.Add($"- {line}");
            }
        }

        // Lines only in file2 (added)
        foreach (var line in lines2)
        {
            if (!set1.Contains(line))
            {
                changes.Add($"+ {line}");
            }
        }

        if (changes.Count == 0)
        {
            changes.Add("  (no differences)");
        }

        return changes;
    }

    static int ComputeEditDistance(string s1, string s2)
    {
        // Levenshtein distance calculation
        int m = s1.Length;
        int n = s2.Length;
        var dp = new int[m + 1, n + 1];

        for (int i = 0; i <= m; i++) dp[i, 0] = i;
        for (int j = 0; j <= n; j++) dp[0, j] = j;

        for (int i = 1; i <= m; i++)
        {
            for (int j = 1; j <= n; j++)
            {
                int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
                dp[i, j] = Math.Min(Math.Min(
                    dp[i - 1, j] + 1,      // deletion
                    dp[i, j - 1] + 1),     // insertion
                    dp[i - 1, j - 1] + cost); // substitution
            }
        }

        return dp[m, n];
    }
}

class DiffResult
{
    public bool AreIdentical { get; set; }
    public int Length1 { get; set; }
    public int Length2 { get; set; }
    public int LengthDiff { get; set; }
    public int? FirstDiffPosition { get; set; }
    public int CharChanges { get; set; }
}
