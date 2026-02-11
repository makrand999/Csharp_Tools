namespace TomlParser;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("TOML File Parser (Basic Implementation)");
            Console.WriteLine("Usage: dotnet run --project 141-toml-parser.csproj -- <file.toml>");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --section NAME   Show specific section");
            Console.WriteLine("  --key KEY        Get specific key value");
            Console.WriteLine("  --json           Output as JSON");
            return;
        }

        string inputFile = args[0];
        string? sectionFilter = null;
        string? keyFilter = null;
        bool outputJson = args.Contains("--json");

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--section" && i + 1 < args.Length)
            {
                sectionFilter = args[i + 1];
            }
            else if (args[i] == "--key" && i + 1 < args.Length)
            {
                keyFilter = args[i + 1];
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
            var toml = ParseToml(content);

            if (keyFilter != null)
            {
                foreach (var section in toml)
                {
                    if (section.Value.ContainsKey(keyFilter))
                    {
                        Console.WriteLine($"{section.Key}.{keyFilter} = {section.Value[keyFilter]}");
                        return;
                    }
                }
                Console.WriteLine($"Key '{keyFilter}' not found");
                return;
            }

            if (outputJson)
            {
                OutputAsJson(toml, sectionFilter);
            }
            else
            {
                OutputAsToml(toml, sectionFilter);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static Dictionary<string, Dictionary<string, string>> ParseToml(string content)
    {
        var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        var currentSection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        result[""] = currentSection; // Global section

        var lines = content.Split('\n');
        string? currentArrayTable = null;

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Skip empty lines and comments
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith("#"))
            {
                continue;
            }

            // Array of tables [[section]]
            if (trimmed.StartsWith("[[") && trimmed.EndsWith("]]"))
            {
                currentArrayTable = trimmed[2..^2].Trim();
                if (!result.ContainsKey(currentArrayTable))
                {
                    currentSection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    result[currentArrayTable] = currentSection;
                }
                continue;
            }

            // Section header [section]
            if (trimmed.StartsWith("[") && trimmed.EndsWith("]"))
            {
                string sectionName = trimmed[1..^1].Trim();
                if (!result.ContainsKey(sectionName))
                {
                    currentSection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                    result[sectionName] = currentSection;
                }
                continue;
            }

            // Key-value pair
            var eqIndex = trimmed.IndexOf('=');
            if (eqIndex > 0)
            {
                string key = trimmed[..eqIndex].Trim();
                string value = trimmed[(eqIndex + 1)..].Trim();

                // Parse TOML value types
                value = ParseTomlValue(value);
                currentSection[key] = value;
            }
        }

        return result;
    }

    static string ParseTomlValue(string value)
    {
        // String (basic or literal)
        if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
            (value.StartsWith("'") && value.EndsWith("'")))
        {
            return value[1..^1];
        }

        // Boolean
        if (value.Equals("true", StringComparison.OrdinalIgnoreCase))
            return "true";
        if (value.Equals("false", StringComparison.OrdinalIgnoreCase))
            return "false";

        // Array (simplified - just return as string)
        if (value.StartsWith("[") && value.EndsWith("]"))
        {
            return value;
        }

        // Inline table (simplified)
        if (value.StartsWith("{") && value.EndsWith("}"))
        {
            return value;
        }

        // Number or date - return as is
        return value;
    }

    static void OutputAsToml(Dictionary<string, Dictionary<string, string>> toml, string? sectionFilter)
    {
        bool first = true;

        foreach (var section in toml)
        {
            if (sectionFilter != null && !section.Key.Equals(sectionFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!first)
            {
                Console.WriteLine();
            }
            first = false;

            if (!string.IsNullOrEmpty(section.Key))
            {
                Console.WriteLine($"[{section.Key}]");
            }

            foreach (var kvp in section.Value)
            {
                Console.WriteLine($"{kvp.Key} = {kvp.Value}");
            }
        }
    }

    static void OutputAsJson(Dictionary<string, Dictionary<string, string>> toml, string? sectionFilter)
    {
        Console.WriteLine("{");
        bool firstSection = true;

        foreach (var section in toml)
        {
            if (sectionFilter != null && !section.Key.Equals(sectionFilter, StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            if (!firstSection)
            {
                Console.WriteLine(",");
            }
            firstSection = false;

            string sectionName = string.IsNullOrEmpty(section.Key) ? "global" : section.Key;
            Console.Write($"  \"{EscapeJson(sectionName)}\": {{");

            bool firstKey = true;
            foreach (var kvp in section.Value)
            {
                if (!firstKey)
                {
                    Console.Write(", ");
                }
                firstKey = false;
                Console.Write($"\"{EscapeJson(kvp.Key)}\": \"{EscapeJson(kvp.Value)}\"");
            }

            Console.Write("}");
        }

        Console.WriteLine("\n}");
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
