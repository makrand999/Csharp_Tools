using System;
using System.Collections.Generic;
using System.Linq;

namespace Lorem;

class Program
{
    // Classic Lorem Ipsum word list
    private static readonly string[] Words =
    {
        "lorem", "ipsum", "dolor", "sit", "amet", "consectetur", "adipiscing", "elit",
        "sed", "do", "eiusmod", "tempor", "incididunt", "ut", "labore", "et", "dolore",
        "magna", "aliqua", "enim", "ad", "minim", "veniam", "quis", "nostrud",
        "exercitation", "ullamco", "laboris", "nisi", "aliquip", "ex", "ea", "commodo",
        "consequat", "duis", "aute", "irure", "in", "reprehenderit", "voluptate",
        "velit", "esse", "cillum", "fugiat", "nulla", "pariatur", "excepteur", "sint",
        "occaecat", "cupidatat", "non", "proident", "sunt", "culpa", "qui", "officia",
        "deserunt", "mollit", "anim", "id", "est", "laborum"
    };

    private static readonly string[] StartPhrase = { "lorem", "ipsum", "dolor", "sit", "amet" };

    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Lorem Ipsum Generator");
            Console.WriteLine("Usage: dotnet run --project Lorem.csproj [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --words <n>      Generate n words (default: 50)");
            Console.WriteLine("  --sentences <n>  Generate n sentences (default: 5)");
            Console.WriteLine("  --paragraphs <n> Generate n paragraphs (default: 3)");
            Console.WriteLine("  --html           Wrap output in HTML tags");
            Console.WriteLine("  --plain          Plain text output (default)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project Lorem.csproj --words 100");
            Console.WriteLine("  dotnet run --project Lorem.csproj --sentences 10");
            Console.WriteLine("  dotnet run --project Lorem.csproj --paragraphs 3 --html");
            return 1;
        }

        var options = ParseOptions(args);

        try
        {
            if (options.Paragraphs.HasValue)
            {
                Console.WriteLine(GenerateParagraphs(options.Paragraphs.Value, options.Html));
            }
            else if (options.Sentences.HasValue)
            {
                Console.WriteLine(GenerateSentences(options.Sentences.Value, options.Html));
            }
            else
            {
                Console.WriteLine(GenerateWords(options.Words ?? 50));
            }
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static Options ParseOptions(string[] args)
    {
        var options = new Options();
        
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "--words":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int words))
                        options.Words = words;
                    break;
                case "--sentences":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int sentences))
                        options.Sentences = sentences;
                    break;
                case "--paragraphs":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int paragraphs))
                        options.Paragraphs = paragraphs;
                    break;
                case "--html":
                    options.Html = true;
                    break;
                case "--plain":
                    options.Html = false;
                    break;
            }
        }

        return options;
    }

    static string GenerateWords(int count)
    {
        var random = new Random();
        var words = new List<string>();

        // Start with classic phrase
        if (count >= StartPhrase.Length)
        {
            words.AddRange(StartPhrase);
            count -= StartPhrase.Length;
        }

        while (count > 0)
        {
            words.Add(Words[random.Next(Words.Length)]);
            count--;
        }

        return string.Join(" ", words);
    }

    static string GenerateSentences(int count, bool html = false)
    {
        var sentences = new List<string>();
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            sentences.Add(GenerateSentence(random.Next(8, 15)));
        }

        string result = html 
            ? sentences.Select(s => $"<p>{s}</p>").Aggregate((a, b) => a + "\n" + b)
            : string.Join(" ", sentences);

        return result;
    }

    static string GenerateParagraphs(int count, bool html = false)
    {
        var paragraphs = new List<string>();
        var random = new Random();

        for (int i = 0; i < count; i++)
        {
            int sentenceCount = random.Next(3, 6);
            var sentences = new List<string>();
            
            for (int j = 0; j < sentenceCount; j++)
            {
                sentences.Add(GenerateSentence(random.Next(8, 15)));
            }
            
            paragraphs.Add(string.Join(" ", sentences));
        }

        if (html)
        {
            return paragraphs.Select(p => $"<p>{p}</p>").Aggregate((a, b) => a + "\n\n" + b);
        }

        return string.Join("\n\n", paragraphs);
    }

    static string GenerateSentence(int wordCount)
    {
        var random = new Random();
        var words = new List<string>();

        // First word capitalized
        string first = Words[random.Next(Words.Length)];
        words.Add(char.ToUpper(first[0]) + first[1..]);

        while (words.Count < wordCount)
        {
            words.Add(Words[random.Next(Words.Length)]);
        }

        // Add comma occasionally
        if (words.Count > 5 && random.NextDouble() > 0.5)
        {
            int commaPos = random.Next(2, words.Count - 2);
            words[commaPos] = words[commaPos] + ",";
        }

        return string.Join(" ", words) + ".";
    }
}

class Options
{
    public int? Words { get; set; }
    public int? Sentences { get; set; }
    public int? Paragraphs { get; set; }
    public bool Html { get; set; }
}
