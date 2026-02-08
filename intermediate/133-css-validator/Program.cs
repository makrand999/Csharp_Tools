using System.Text.RegularExpressions;

namespace CssValidator;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("CSS Validator");
            Console.WriteLine("Usage: dotnet run --project 133-css-validator.csproj -- <file.css>");
            Console.WriteLine("       cat styles.css | dotnet run --project 133-css-validator.csproj");
            return;
        }

        string inputFile = args[0];
        string css;

        if (File.Exists(inputFile))
        {
            css = File.ReadAllText(inputFile);
            Console.WriteLine($"Validating: {inputFile}");
        }
        else
        {
            css = Console.In.ReadToEnd();
            Console.WriteLine("Validating CSS from stdin...");
        }

        var errors = ValidateCss(css);

        if (errors.Count == 0)
        {
            Console.WriteLine("✓ No syntax errors found!");
        }
        else
        {
            Console.WriteLine($"\n✗ Found {errors.Count} issue(s):\n");
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
        var stats = GetCssStats(css);
        Console.WriteLine($"\n=== Statistics ===");
        Console.WriteLine($"Total characters: {css.Length:N0}");
        Console.WriteLine($"Rules: {stats["rules"]}");
        Console.WriteLine($"Properties: {stats["properties"]}");
        Console.WriteLine($"Comments: {stats["comments"]}");
    }

    static List<CssError> ValidateCss(string css)
    {
        var errors = new List<CssError>();
        var lines = css.Split('\n');

        // Check for balanced braces
        int openBraces = 0;
        int closeBraces = 0;
        foreach (var c in css)
        {
            if (c == '{') openBraces++;
            if (c == '}') closeBraces++;
        }

        if (openBraces != closeBraces)
        {
            errors.Add(new CssError
            {
                Line = 0,
                Message = $"Unbalanced braces: {openBraces} opening, {closeBraces} closing",
                Context = ""
            });
        }

        // Check for common issues
        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();
            int lineNum = i + 1;

            // Missing semicolon (basic check)
            if (!string.IsNullOrEmpty(line) && 
                !line.StartsWith("/*") && 
                !line.StartsWith("*") &&
                !line.EndsWith("{") && 
                !line.EndsWith("}") &&
                !line.EndsWith(";") &&
                !line.StartsWith("@") &&
                !line.StartsWith("//") &&
                line.Contains(":") &&
                !line.StartsWith("--"))
            {
                // Could be missing semicolon, but might be last property before }
                // This is a heuristic
            }

            // Check for unclosed strings
            var singleQuotes = line.Count(c => c == '\'');
            var doubleQuotes = line.Count(c => c == '"');
            if (singleQuotes % 2 != 0 || doubleQuotes % 2 != 0)
            {
                errors.Add(new CssError
                {
                    Line = lineNum,
                    Message = "Potentially unclosed string",
                    Context = line
                });
            }

            // Check for invalid property names (basic check)
            if (line.Contains(":") && !line.StartsWith("/*"))
            {
                var parts = line.Split(':');
                if (parts.Length >= 2)
                {
                    var propName = parts[0].Trim().ToLower();
                    // Allow custom properties
                    if (!propName.StartsWith("--") && 
                        !IsValidCssProperty(propName) &&
                        !string.IsNullOrEmpty(propName))
                    {
                        // Just a warning, not an error
                        errors.Add(new CssError
                        {
                            Line = lineNum,
                            Message = $"Unknown property: {propName}",
                            Context = line
                        });
                    }
                }
            }
        }

        return errors;
    }

    static bool IsValidCssProperty(string name)
    {
        var commonProps = new HashSet<string>
        {
            "color", "background", "background-color", "background-image",
            "margin", "padding", "border", "width", "height", "max-width",
            "min-width", "max-height", "min-height", "display", "position",
            "top", "right", "bottom", "left", "float", "clear", "overflow",
            "font", "font-size", "font-weight", "font-family", "line-height",
            "text-align", "text-decoration", "text-transform", "color",
            "flex", "flex-direction", "justify-content", "align-items",
            "grid", "grid-template-columns", "grid-template-rows", "gap",
            "transition", "transform", "animation", "opacity", "visibility",
            "z-index", "cursor", "box-shadow", "text-shadow", "border-radius",
            "outline", "list-style", "content", "white-space", "vertical-align"
        };
        return commonProps.Contains(name) || name.StartsWith("-");
    }

    static Dictionary<string, int> GetCssStats(string css)
    {
        var stats = new Dictionary<string, int>();
        
        // Count rules (selectors followed by {)
        stats["rules"] = Regex.Matches(css, @"[^{]+\{").Count;
        
        // Count properties (name: value patterns)
        stats["properties"] = Regex.Matches(css, @"[\w-]+\s*:").Count;
        
        // Count comments
        stats["comments"] = Regex.Matches(css, @"/\*.*?\*/", RegexOptions.Singleline).Count;
        
        return stats;
    }
}

class CssError
{
    public int Line { get; set; }
    public string Message { get; set; } = "";
    public string Context { get; set; } = "";
}
