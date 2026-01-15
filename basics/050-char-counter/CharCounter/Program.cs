using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CharCounter;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Character Counter");
            Console.WriteLine("Usage: dotnet run --project CharCounter.csproj [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  <file>              Count characters in file");
            Console.WriteLine("  --stdin             Count characters from stdin");
            Console.WriteLine("  --string \"<text>\"   Count characters in string");
            Console.WriteLine("  --detailed          Show detailed breakdown");
            Console.WriteLine("  --top <n>           Show top N frequent chars (default: 10)");
            Console.WriteLine("  --json              Output as JSON");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project CharCounter.csproj file.txt");
            Console.WriteLine("  dotnet run --project CharCounter.csproj --stdin < file.txt");
            Console.WriteLine("  dotnet run --project CharCounter.csproj --string \"hello world\"");
            Console.WriteLine("  dotnet run --project CharCounter.csproj file.txt --detailed");
            return 1;
        }

        try
        {
            string? text = null;
            bool detailed = args.Contains("--detailed");
            bool json = args.Contains("--json");
            int topN = 10;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--top" && i + 1 < args.Length)
                {
                    if (int.TryParse(args[++i], out int n)) topN = n;
                }
            }

            if (args.Contains("--stdin"))
            {
                text = Console.In.ReadToEnd();
            }
            else if (args.Contains("--string"))
            {
                int idx = Array.IndexOf(args, "--string");
                if (idx + 1 < args.Length)
                {
                    text = args[idx + 1];
                }
            }
            else
            {
                string filePath = args.First(a => !a.StartsWith("--") && a != "--string");
                if (!File.Exists(filePath))
                    throw new FileNotFoundException($"File not found: {filePath}");
                text = File.ReadAllText(filePath);
            }

            if (string.IsNullOrEmpty(text))
            {
                Console.WriteLine("No input provided.");
                return 1;
            }

            var stats = AnalyzeText(text, topN);

            if (json)
            {
                Console.WriteLine(ToJson(stats));
            }
            else if (detailed)
            {
                DisplayDetailed(stats);
            }
            else
            {
                DisplaySummary(stats);
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static TextStats AnalyzeText(string text, int topN)
    {
        var charFrequency = new Dictionary<char, int>();
        int letters = 0, digits = 0, spaces = 0, punctuation = 0, other = 0;
        int uppercase = 0, lowercase = 0;

        foreach (char c in text)
        {
            // Frequency count
            if (!charFrequency.ContainsKey(c))
                charFrequency[c] = 0;
            charFrequency[c]++;

            // Category count
            if (char.IsLetter(c))
            {
                letters++;
                if (char.IsUpper(c)) uppercase++;
                else if (char.IsLower(c)) lowercase++;
            }
            else if (char.IsDigit(c))
            {
                digits++;
            }
            else if (char.IsWhiteSpace(c))
            {
                spaces++;
            }
            else if (char.IsPunctuation(c))
            {
                punctuation++;
            }
            else
            {
                other++;
            }
        }

        var lines = text.Split('\n');
        var words = text.Split(new char[] { ' ', '\n', '\r', '\t' }, StringSplitOptions.RemoveEmptyEntries);

        return new TextStats
        {
            TotalCharacters = text.Length,
            CharactersNoSpaces = text.Count(c => !char.IsWhiteSpace(c)),
            Letters = letters,
            Digits = digits,
            Spaces = spaces,
            Punctuation = punctuation,
            Other = other,
            Uppercase = uppercase,
            Lowercase = lowercase,
            Lines = lines.Length,
            Words = words.Length,
            TopCharacters = charFrequency.OrderByDescending(x => x.Value).Take(topN).ToList(),
            AllCharacters = charFrequency
        };
    }

    static void DisplaySummary(TextStats stats)
    {
        Console.WriteLine("Character Count Summary");
        Console.WriteLine("=======================");
        Console.WriteLine();
        Console.WriteLine($"Total characters: {stats.TotalCharacters:N0}");
        Console.WriteLine($"Characters (no spaces): {stats.CharactersNoSpaces:N0}");
        Console.WriteLine($"Words: {stats.Words:N0}");
        Console.WriteLine($"Lines: {stats.Lines:N0}");
    }

    static void DisplayDetailed(TextStats stats)
    {
        Console.WriteLine("Character Count - Detailed Analysis");
        Console.WriteLine("====================================");
        Console.WriteLine();

        Console.WriteLine("Overview:");
        Console.WriteLine($"  Total characters:     {stats.TotalCharacters:N0}");
        Console.WriteLine($"  Characters (no spaces): {stats.CharactersNoSpaces:N0}");
        Console.WriteLine($"  Words:                {stats.Words:N0}");
        Console.WriteLine($"  Lines:                {stats.Lines:N0}");
        Console.WriteLine();

        Console.WriteLine("Character Types:");
        Console.WriteLine($"  Letters:     {stats.Letters:N0}");
        Console.WriteLine($"    Uppercase: {stats.Uppercase:N0}");
        Console.WriteLine($"    Lowercase: {stats.Lowercase:N0}");
        Console.WriteLine($"  Digits:      {stats.Digits:N0}");
        Console.WriteLine($"  Spaces:      {stats.Spaces:N0}");
        Console.WriteLine($"  Punctuation: {stats.Punctuation:N0}");
        Console.WriteLine($"  Other:       {stats.Other:N0}");
        Console.WriteLine();

        Console.WriteLine($"Top {stats.TopCharacters.Count} Most Frequent Characters:");
        Console.WriteLine("  Char | Count | Percentage | Visual");
        Console.WriteLine("  -----+------+------------+------------------");
        
        foreach (var kv in stats.TopCharacters)
        {
            string display = GetCharDisplay(kv.Key);
            double percent = (double)kv.Value / stats.TotalCharacters * 100;
            string bar = new string('█', (int)(percent / 2));
            Console.WriteLine($"  {display,5} | {kv.Value,5} | {percent,10:F2}% | {bar}");
        }
        Console.WriteLine();

        // Unique characters
        Console.WriteLine($"Unique characters: {stats.AllCharacters.Count}");
    }

    static string GetCharDisplay(char c)
    {
        return c switch
        {
            ' ' => "' '",
            '\n' => @"\n",
            '\r' => @"\r",
            '\t' => @"\t",
            < 32 => $"0x{(int)c:X2}",
            _ => $"'{c}'"
        };
    }

    static string ToJson(TextStats stats)
    {
        var topChars = string.Join(", ", stats.TopCharacters.Select(kv => 
            $"{{\"char\": \"{EscapeJson(kv.Key.ToString())}\", \"count\": {kv.Value}}}"));

        return $$"""
        {
          "totalCharacters": {{stats.TotalCharacters}},
          "charactersNoSpaces": {{stats.CharactersNoSpaces}},
          "words": {{stats.Words}},
          "lines": {{stats.Lines}},
          "letters": {{stats.Letters}},
          "uppercase": {{stats.Uppercase}},
          "lowercase": {{stats.Lowercase}},
          "digits": {{stats.Digits}},
          "spaces": {{stats.Spaces}},
          "punctuation": {{stats.Punctuation}},
          "other": {{stats.Other}},
          "topCharacters": [{{topChars}}]
        }
        """;
    }

    static string EscapeJson(string s)
    {
        return s.Replace("\\", "\\\\")
                .Replace("\"", "\\\"")
                .Replace("\n", "\\n")
                .Replace("\r", "\\r")
                .Replace("\t", "\\t");
    }
}

class TextStats
{
    public int TotalCharacters { get; set; }
    public int CharactersNoSpaces { get; set; }
    public int Letters { get; set; }
    public int Digits { get; set; }
    public int Spaces { get; set; }
    public int Punctuation { get; set; }
    public int Other { get; set; }
    public int Uppercase { get; set; }
    public int Lowercase { get; set; }
    public int Lines { get; set; }
    public int Words { get; set; }
    public List<KeyValuePair<char, int>> TopCharacters { get; set; } = new();
    public Dictionary<char, int> AllCharacters { get; set; } = new();
}
