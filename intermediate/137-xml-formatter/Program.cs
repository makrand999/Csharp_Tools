using System.Xml;

namespace XmlFormatter;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("XML Formatter");
            Console.WriteLine("Usage: dotnet run --project 137-xml-formatter.csproj -- <input.xml> [output.xml]");
            Console.WriteLine("       cat input.xml | dotnet run --project 137-xml-formatter.csproj");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --minify    Minify XML (remove whitespace)");
            Console.WriteLine("  --indent N  Set indentation (default: 2)");
            return;
        }

        bool minify = args.Contains("--minify");
        int indentSize = 2;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--indent" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out indentSize);
            }
        }

        string inputFile = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "";
        string outputFile = args.SkipWhile(a => !a.StartsWith("--")).Skip(1).FirstOrDefault(a => !a.StartsWith("--")) ?? "";

        string xml;
        if (File.Exists(inputFile))
        {
            xml = File.ReadAllText(inputFile);
            Console.WriteLine($"Formatting: {inputFile}");
        }
        else if (string.IsNullOrEmpty(inputFile))
        {
            xml = Console.In.ReadToEnd();
            Console.WriteLine("Formatting XML from stdin...");
        }
        else
        {
            xml = inputFile; // Treat as inline XML
        }

        try
        {
            string result = minify ? MinifyXml(xml) : PrettyPrintXml(xml, indentSize);

            if (!string.IsNullOrEmpty(outputFile))
            {
                File.WriteAllText(outputFile, result);
                Console.WriteLine($"Written to: {outputFile}");
                Console.WriteLine($"Original size: {xml.Length:N0} bytes");
                Console.WriteLine($"Output size: {result.Length:N0} bytes");
            }
            else
            {
                Console.WriteLine(result);
            }
        }
        catch (XmlException ex)
        {
            Console.WriteLine($"XML Parse Error: {ex.Message}");
            Console.WriteLine($"Line {ex.LineNumber}, Position {ex.LinePosition}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static string PrettyPrintXml(string xml, int indentSize)
    {
        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = new string(' ', indentSize),
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace,
            OmitXmlDeclaration = false
        };

        var document = new XmlDocument();
        document.LoadXml(xml);

        using var stringWriter = new StringWriter();
        using var xmlWriter = XmlWriter.Create(stringWriter, settings);
        document.Save(xmlWriter);
        
        return stringWriter.ToString();
    }

    static string MinifyXml(string xml)
    {
        // Remove whitespace between tags and comments
        var result = System.Text.RegularExpressions.Regex.Replace(xml, @">\s+<", "><");
        result = System.Text.RegularExpressions.Regex.Replace(result, @"/\*.*?\*/", "", System.Text.RegularExpressions.RegexOptions.Singleline);
        return result.Trim();
    }
}
