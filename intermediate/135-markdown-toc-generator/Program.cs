namespace MarkdownTocGenerator;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Markdown Table of Contents Generator");
            Console.WriteLine("Usage: dotnet run --project 135-markdown-toc-generator.csproj -- <file.md>");
            Console.WriteLine("       dotnet run --project 135-markdown-toc-generator.csproj -- <file.md> --output <output.md>");
            Console.WriteLine("\nGenerates a table of contents from markdown headings.");
            return;
        }

        string inputFile = args[0];
        string? outputFile = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--output" && i + 1 < args.Length)
            {
                outputFile = args[i + 1];
            }
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Error: File not found: {inputFile}");
            return;
        }

        try
        {
            var content = File.ReadAllText(inputFile);
            var headings = ExtractHeadings(content);
            var toc = GenerateToc(headings);

            Console.WriteLine("=== Generated Table of Contents ===\n");
            Console.WriteLine(toc);

            if (!string.IsNullOrEmpty(outputFile))
            {
                // Insert TOC after first heading or at beginning
                var newContent = InsertToc(content, toc);
                File.WriteAllText(outputFile, newContent);
                Console.WriteLine($"\nWritten to: {outputFile}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static List<Heading> ExtractHeadings(string markdown)
    {
        var headings = new List<Heading>();
        var lines = markdown.Split('\n');

        foreach (var line in lines)
        {
            var trimmed = line.TrimStart();
            if (trimmed.StartsWith("#"))
            {
                int level = 0;
                foreach (var c in trimmed)
                {
                    if (c == '#') level++;
                    else break;
                }

                if (level <= 6)
                {
                    string text = trimmed.Substring(level).Trim();
                    // Remove inline formatting for anchor
                    string anchor = GenerateAnchor(text);
                    headings.Add(new Heading { Level = level, Text = text, Anchor = anchor });
                }
            }
        }

        return headings;
    }

    static string GenerateAnchor(string text)
    {
        // GitHub-style anchor generation
        var anchor = text.ToLower();
        anchor = System.Text.RegularExpressions.Regex.Replace(anchor, @"[^\w\s-]", "");
        anchor = System.Text.RegularExpressions.Regex.Replace(anchor, @"\s+", "-");
        return anchor;
    }

    static string GenerateToc(List<Heading> headings)
    {
        var toc = new System.Text.StringBuilder();

        foreach (var heading in headings)
        {
            string indent = new string(' ', (heading.Level - 1) * 2);
            toc.AppendLine($"{indent}- [{heading.Text}](#{heading.Anchor})");
        }

        return toc.ToString();
    }

    static string InsertToc(string content, string toc)
    {
        // Find position after first heading
        var lines = content.Split('\n').ToList();
        int insertPos = 0;

        for (int i = 0; i < lines.Count; i++)
        {
            if (lines[i].TrimStart().StartsWith("#"))
            {
                insertPos = i + 1;
                break;
            }
        }

        // Insert blank line, TOC, blank line
        var result = new List<string>();
        for (int i = 0; i < lines.Count; i++)
        {
            result.Add(lines[i]);
            if (i == insertPos - 1)
            {
                result.Add("");
                result.AddRange(toc.Split('\n').Select(l => l.TrimEnd()));
                result.Add("");
            }
        }

        return string.Join('\n', result);
    }
}

class Heading
{
    public int Level { get; set; }
    public string Text { get; set; } = "";
    public string Anchor { get; set; } = "";
}
