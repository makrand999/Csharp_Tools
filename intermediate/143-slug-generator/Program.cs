using System.Text.RegularExpressions;

namespace SlugGenerator;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Slug Generator");
            Console.WriteLine("Usage: dotnet run --project 143-slug-generator.csproj -- \"<text>\"");
            Console.WriteLine("       echo \"Hello World\" | dotnet run --project 143-slug-generator.csproj");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --lowercase     Convert to lowercase (default)");
            Console.WriteLine("  --separator C   Use custom separator (default: -)");
            Console.WriteLine("  --max-length N  Truncate to N characters");
            return;
        }

        bool lowercase = !args.Contains("--uppercase");
        char separator = '-';
        int? maxLength = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--separator" && i + 1 < args.Length)
            {
                separator = args[i + 1][0];
            }
            else if (args[i] == "--max-length" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out var n);
                maxLength = n;
            }
        }

        string text = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "";
        
        if (string.IsNullOrEmpty(text))
        {
            text = Console.In.ReadLine() ?? "";
        }

        var slug = GenerateSlug(text, lowercase, separator, maxLength);
        Console.WriteLine(slug);
    }

    static string GenerateSlug(string text, bool lowercase, char separator, int? maxLength)
    {
        if (lowercase)
        {
            text = text.ToLowerInvariant();
        }

        // Remove diacritics
        text = RemoveDiacritics(text);

        // Replace spaces and underscores with separator
        text = Regex.Replace(text, @"[\s_]+", separator.ToString());

        // Remove special characters
        text = Regex.Replace(text, $@"[^a-z0-9{Regex.Escape(separator.ToString())}-]", "", RegexOptions.IgnoreCase);

        // Remove consecutive separators
        text = Regex.Replace(text, $@"{Regex.Escape(separator.ToString())}+", separator.ToString());

        // Trim separators from ends
        text = text.Trim(separator);

        // Truncate
        if (maxLength.HasValue && text.Length > maxLength.Value)
        {
            text = text[..maxLength.Value];
            // Don't end with separator after truncation
            text = text.TrimEnd(separator);
        }

        return text;
    }

    static string RemoveDiacritics(string text)
    {
        var normalized = text.Normalize(System.Text.NormalizationForm.FormD);
        var sb = new System.Text.StringBuilder();

        foreach (var c in normalized)
        {
            var uc = System.Globalization.CharUnicodeInfo.GetUnicodeCategory(c);
            if (uc != System.Globalization.UnicodeCategory.NonSpacingMark)
            {
                sb.Append(c);
            }
        }

        return sb.ToString().Normalize(System.Text.NormalizationForm.FormC);
    }
}
