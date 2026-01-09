using System;
using System.Text;
using System.Text.RegularExpressions;

namespace TextCaseConverter;

/// <summary>
/// A practical CLI tool for converting text between different naming conventions.
/// Supports camelCase, PascalCase, snake_case, kebab-case, and CONSTANT_CASE.
/// </summary>
class Program
{
    enum CaseType
    {
        Camel,
        Pascal,
        Snake,
        Kebab,
        Constant
    }

    static int Main(string[] args)
    {
        if (args.Length < 2)
        {
            PrintUsage();
            return 1;
        }

        string text = args[0];
        string targetCase = args[1].ToLower();

        try
        {
            CaseType caseType = ParseCaseType(targetCase);
            string result = ConvertCase(text, caseType);
            Console.WriteLine(result);
            return 0;
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            PrintUsage();
            return 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Text Case Converter - Convert between naming conventions");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project TextCaseConverter.csproj <text> <target-case>");
        Console.WriteLine();
        Console.WriteLine("Target cases:");
        Console.WriteLine("  camel    - camelCase");
        Console.WriteLine("  pascal   - PascalCase");
        Console.WriteLine("  snake    - snake_case");
        Console.WriteLine("  kebab    - kebab-case");
        Console.WriteLine("  constant - CONSTANT_CASE");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project TextCaseConverter.csproj \"hello_world\" camel");
        Console.WriteLine("  dotnet run --project TextCaseConverter.csproj \"HelloWorld\" snake");
        Console.WriteLine("  dotnet run --project TextCaseConverter.csproj \"hello-world\" pascal");
    }

    static CaseType ParseCaseType(string input)
    {
        return input switch
        {
            "camel" => CaseType.Camel,
            "pascal" => CaseType.Pascal,
            "snake" => CaseType.Snake,
            "kebab" => CaseType.Kebab,
            "constant" => CaseType.Constant,
            _ => throw new ArgumentException($"Unknown case type: {input}")
        };
    }

    static string ConvertCase(string input, CaseType targetCase)
    {
        // Split input into words by detecting case changes, underscores, hyphens, and spaces
        string[] words = SplitIntoWords(input);

        if (words.Length == 0)
            return input;

        return targetCase switch
        {
            CaseType.Camel => ToCamelCase(words),
            CaseType.Pascal => ToPascalCase(words),
            CaseType.Snake => ToSnakeCase(words, false),
            CaseType.Kebab => ToKebabCase(words),
            CaseType.Constant => ToSnakeCase(words, true),
            _ => input
        };
    }

    static string[] SplitIntoWords(string input)
    {
        // Replace separators with spaces
        string normalized = input.Replace('_', ' ').Replace('-', ' ');

        // Insert space before uppercase letters that follow lowercase letters (camelCase detection)
        var sb = new StringBuilder();
        for (int i = 0; i < normalized.Length; i++)
        {
            char c = normalized[i];
            if (i > 0 && char.IsUpper(c) && char.IsLower(normalized[i - 1]))
            {
                sb.Append(' ');
            }
            sb.Append(c);
        }

        // Handle consecutive uppercase followed by lowercase (e.g., "XMLParser" -> "XML Parser")
        string temp = sb.ToString();
        var result = new StringBuilder();
        for (int i = 0; i < temp.Length; i++)
        {
            result.Append(temp[i]);
            if (i < temp.Length - 2 && 
                char.IsUpper(temp[i]) && 
                char.IsUpper(temp[i + 1]) && 
                char.IsLower(temp[i + 2]))
            {
                result.Append(' ');
            }
        }

        // Split by spaces and filter empty entries
        return result.ToString()
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(w => w.ToLower())
            .ToArray();
    }

    static string ToCamelCase(string[] words)
    {
        if (words.Length == 0) return string.Empty;

        var result = new StringBuilder();
        result.Append(words[0]);

        for (int i = 1; i < words.Length; i++)
        {
            result.Append(CapitalizeFirst(words[i]));
        }

        return result.ToString();
    }

    static string ToPascalCase(string[] words)
    {
        var result = new StringBuilder();
        foreach (string word in words)
        {
            result.Append(CapitalizeFirst(word));
        }
        return result.ToString();
    }

    static string ToSnakeCase(string[] words, bool uppercase)
    {
        return uppercase 
            ? string.Join("_", words.Select(w => w.ToUpper()))
            : string.Join("_", words);
    }

    static string ToKebabCase(string[] words)
    {
        return string.Join("-", words);
    }

    static string CapitalizeFirst(string word)
    {
        if (string.IsNullOrEmpty(word))
            return word;
        
        return char.ToUpper(word[0]) + word[1..];
    }
}
