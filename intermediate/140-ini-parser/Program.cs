namespace IniParser;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("INI File Parser");
            Console.WriteLine("Usage: dotnet run --project 140-ini-parser.csproj -- <file.ini>");
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
            var ini = ParseIni(content);

            if (keyFilter != null)
            {
                // Search for key across all sections
                foreach (var section in ini)
                {
                    if (section.Value.ContainsKey(keyFilter))
                    {
                        Console.WriteLine($"[{section.Key}] {keyFilter} = {section.Value[keyFilter]}");
                        return;
                    }
                }
                Console.WriteLine($"Key '{keyFilter}' not found");
                return;
            }

            if (outputJson)
            {
                OutputAsJson(ini, sectionFilter);
            }
            else
            {
                OutputAsIni(ini, sectionFilter);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static Dictionary<string, Dictionary<string, string>> ParseIni(string content)
    {
        var result = new Dictionary<string, Dictionary<string, string>>(StringComparer.OrdinalIgnoreCase);
        var currentSection = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        result[""] = currentSection; // Global section

        var lines = content.Split('\n');

        foreach (var line in lines)
        {
            var trimmed = line.Trim();

            // Skip empty lines and comments
            if (string.IsNullOrEmpty(trimmed) || trimmed.StartsWith(";") || trimmed.StartsWith("#"))
            {
                continue;
            }

            // Section header
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
            var colonIndex = trimmed.IndexOf(':');
            var separatorIndex = Math.Max(eqIndex, colonIndex);

            if (separatorIndex > 0)
            {
                string key = trimmed[..separatorIndex].Trim();
                string value = trimmed[(separatorIndex + 1)..].Trim();

                // Remove quotes if present
                if ((value.StartsWith("\"") && value.EndsWith("\"")) ||
                    (value.StartsWith("'") && value.EndsWith("'")))
                {
                    value = value[1..^1];
                }

                currentSection[key] = value;
            }
        }

        return result;
    }

    static void OutputAsIni(Dictionary<string, Dictionary<string, string>> ini, string? sectionFilter)
    {
        bool first = true;

        foreach (var section in ini)
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

    static void OutputAsJson(Dictionary<string, Dictionary<string, string>> ini, string? sectionFilter)
    {
        Console.WriteLine("{");
        bool firstSection = true;

        foreach (var section in ini)
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
