using System.Text.RegularExpressions;

namespace HtmlMinifier;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("HTML Minifier");
            Console.WriteLine("Usage: dotnet run --project 132-html-minifier.csproj -- <input.html> [output.html]");
            Console.WriteLine("       cat input.html | dotnet run --project 132-html-minifier.csproj");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --remove-comments    Remove HTML comments (default: keep)");
            Console.WriteLine("  --remove-whitespace  Remove extra whitespace (default: true)");
            return;
        }

        bool removeComments = args.Contains("--remove-comments");
        string inputFile = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "";
        string outputFile = args.Length > 1 && !args[1].StartsWith("--") ? args[1] : "";

        string html;
        if (File.Exists(inputFile))
        {
            html = File.ReadAllText(inputFile);
        }
        else if (string.IsNullOrEmpty(inputFile))
        {
            html = Console.In.ReadToEnd();
        }
        else
        {
            html = inputFile; // Treat as inline HTML
        }

        var minified = MinifyHtml(html, removeComments);

        if (!string.IsNullOrEmpty(outputFile))
        {
            File.WriteAllText(outputFile, minified);
            Console.WriteLine($"Minified HTML written to: {outputFile}");
            Console.WriteLine($"Original size: {html.Length:N0} bytes");
            Console.WriteLine($"Minified size: {minified.Length:N0} bytes");
            Console.WriteLine($"Reduction: {(1.0 - (double)minified.Length / html.Length) * 100:F1}%");
        }
        else
        {
            Console.WriteLine(minified);
        }
    }

    static string MinifyHtml(string html, bool removeComments)
    {
        var result = html;

        // Remove comments
        if (removeComments)
        {
            result = Regex.Replace(result, @"<!--.*?-->", "", RegexOptions.Singleline);
        }

        // Remove whitespace between tags
        result = Regex.Replace(result, @">\s+<", "><");

        // Remove leading/trailing whitespace from lines
        var lines = result.Split('\n')
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l));
        result = string.Join("", lines);

        // Collapse multiple spaces
        result = Regex.Replace(result, @"\s{2,}", " ");

        // Remove space around certain tags
        result = Regex.Replace(result, @">\s+", ">");
        result = Regex.Replace(result, @"\s+<", "<");

        return result.Trim();
    }
}
