namespace YamlValidator;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("YAML Validator");
            Console.WriteLine("Usage: dotnet run --project 136-yaml-validator.csproj -- <file.yaml>");
            Console.WriteLine("       cat config.yaml | dotnet run --project 136-yaml-validator.csproj");
            Console.WriteLine("\nPerforms basic YAML syntax validation.");
            return;
        }

        string inputFile = args[0];
        string yaml;

        if (File.Exists(inputFile))
        {
            yaml = File.ReadAllText(inputFile);
            Console.WriteLine($"Validating: {inputFile}");
        }
        else
        {
            yaml = Console.In.ReadToEnd();
            Console.WriteLine("Validating YAML from stdin...");
        }

        var errors = ValidateYaml(yaml);

        if (errors.Count == 0)
        {
            Console.WriteLine("✓ YAML syntax appears valid!");
        }
        else
        {
            Console.WriteLine($"\n✗ Found {errors.Count} potential issue(s):\n");
            foreach (var error in errors)
            {
                Console.WriteLine($"  Line {error.Line}: {error.Message}");
                if (!string.IsNullOrEmpty(error.Context))
                {
                    Console.WriteLine($"    {error.Context}");
                }
            }
        }

        // Show statistics
        var stats = GetYamlStats(yaml);
        Console.WriteLine($"\n=== Statistics ===");
        Console.WriteLine($"Total lines: {stats["lines"]}");
        Console.WriteLine($"Key-value pairs: {stats["pairs"]}");
        Console.WriteLine($"List items: {stats["listItems"]}");
        Console.WriteLine($"Comments: {stats["comments"]}");
        Console.WriteLine($"Nested depth: {stats["maxDepth"]}");
    }

    static List<YamlError> ValidateYaml(string yaml)
    {
        var errors = new List<YamlError>();
        var lines = yaml.Split('\n');

        int expectedIndent = 0;
        var indentStack = new Stack<int>();
        indentStack.Push(0);

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i];
            int lineNum = i + 1;

            // Skip empty lines and comments
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
            {
                continue;
            }

            // Check for tabs (YAML prefers spaces)
            if (line.Contains("\t") && !line.TrimStart().StartsWith("#"))
            {
                errors.Add(new YamlError
                {
                    Line = lineNum,
                    Message = "Tab character found (YAML prefers spaces for indentation)",
                    Context = line
                });
            }

            // Calculate indentation
            int indent = line.TakeWhile(char.IsWhiteSpace).Count();
            var trimmed = line.Trim();

            // Check for colon without space
            if (trimmed.Contains(":") && !trimmed.Contains(": ") && !trimmed.EndsWith(":"))
            {
                var colonIndex = trimmed.IndexOf(':');
                if (colonIndex > 0 && colonIndex < trimmed.Length - 1)
                {
                    char nextChar = trimmed[colonIndex + 1];
                    if (nextChar != ' ' && nextChar != '\n')
                    {
                        // Could be an issue, but might be valid (e.g., empty value)
                    }
                }
            }

            // Check for list item format
            if (trimmed.StartsWith("-") && trimmed.Length > 1 && trimmed[1] != ' ')
            {
                errors.Add(new YamlError
                {
                    Line = lineNum,
                    Message = "List item should have space after '-'",
                    Context = line
                });
            }

            // Track indentation for structure validation
            if (indent > indentStack.Peek() && !trimmed.StartsWith("-"))
            {
                // New nested level
                indentStack.Push(indent);
            }
            else if (indent < indentStack.Peek())
            {
                // Going back to previous level
                while (indentStack.Count > 1 && indent < indentStack.Peek())
                {
                    indentStack.Pop();
                }
            }
        }

        return errors;
    }

    static Dictionary<string, int> GetYamlStats(string yaml)
    {
        var stats = new Dictionary<string, int>();
        var lines = yaml.Split('\n');

        stats["lines"] = lines.Length;
        stats["pairs"] = System.Text.RegularExpressions.Regex.Matches(yaml, @"^\s*[\w-]+\s*:", System.Text.RegularExpressions.RegexOptions.Multiline).Count;
        stats["listItems"] = System.Text.RegularExpressions.Regex.Matches(yaml, @"^\s*-\s", System.Text.RegularExpressions.RegexOptions.Multiline).Count;
        stats["comments"] = System.Text.RegularExpressions.Regex.Matches(yaml, @"#.*$", System.Text.RegularExpressions.RegexOptions.Multiline).Count;

        // Calculate max nesting depth
        int maxDepth = 0;
        int currentDepth = 0;
        int lastIndent = 0;

        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;

            int indent = line.TakeWhile(char.IsWhiteSpace).Count();
            if (indent > lastIndent)
            {
                currentDepth++;
                lastIndent = indent;
            }
            else if (indent < lastIndent)
            {
                currentDepth = indent / 2; // Approximate
                lastIndent = indent;
            }
            maxDepth = Math.Max(maxDepth, currentDepth);
        }

        stats["maxDepth"] = maxDepth + 1;
        return stats;
    }
}

class YamlError
{
    public int Line { get; set; }
    public string Message { get; set; } = "";
    public string Context { get; set; } = "";
}
