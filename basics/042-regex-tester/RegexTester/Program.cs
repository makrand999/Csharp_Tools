using System;
using System.Text.RegularExpressions;

namespace RegexTester;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            PrintUsage();
            return;
        }

        string pattern = args[0];
        string text = string.Join(" ", args[1..]);

        TestRegex(pattern, text);
    }

    static void PrintUsage()
    {
        Console.WriteLine("Regex Tester - Test regular expressions against text");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project RegexTester.csproj <pattern> <text>");
        Console.WriteLine();
        Console.WriteLine("Options (add to pattern):");
        Console.WriteLine("  -i    Case insensitive");
        Console.WriteLine("  -m    Multiline mode");
        Console.WriteLine("  -s    Singleline mode");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project RegexTester.csproj \"\\d+\" \"There are 42 apples\"");
        Console.WriteLine("  dotnet run --project RegexTester.csproj \"(?i)hello\" \"Hello World\"");
        Console.WriteLine("  dotnet run --project RegexTester.csproj \"^test\" \"test line\" -m");
    }

    static void TestRegex(string pattern, string text)
    {
        Console.WriteLine("=== Regex Test ===");
        Console.WriteLine();
        Console.WriteLine($"Pattern: {pattern}");
        Console.WriteLine($"Text:    {text}");
        Console.WriteLine();

        try
        {
            var regex = new Regex(pattern, RegexOptions.None, TimeSpan.FromSeconds(2));
            MatchCollection matches = regex.Matches(text);

            if (matches.Count == 0)
            {
                Console.WriteLine("No matches found.");
            }
            else
            {
                Console.WriteLine($"Found {matches.Count} match(es):");
                Console.WriteLine();

                int i = 1;
                foreach (Match match in matches)
                {
                    Console.WriteLine($"Match {i++}:");
                    Console.WriteLine($"  Value:    \"{match.Value}\"");
                    Console.WriteLine($"  Position: {match.Index}-{match.Index + match.Length}");
                    
                    if (match.Groups.Count > 1)
                    {
                        Console.WriteLine("  Groups:");
                        for (int g = 1; g < match.Groups.Count; g++)
                        {
                            Console.WriteLine($"    {g}: \"{match.Groups[g].Value}\"");
                        }
                    }
                    Console.WriteLine();
                }
            }

            // Show test results
            Console.WriteLine("Test Results:");
            Console.WriteLine($"  IsMatch: {regex.IsMatch(text)}");
            Console.WriteLine();

            // Show replacement example
            Console.WriteLine("Replace with '***':");
            Console.WriteLine($"  {regex.Replace(text, "***")}");

        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Invalid regex pattern: {ex.Message}");
        }
        catch (RegexMatchTimeoutException)
        {
            Console.Error.WriteLine("Regex match timed out (2 second limit)");
        }
    }
}
