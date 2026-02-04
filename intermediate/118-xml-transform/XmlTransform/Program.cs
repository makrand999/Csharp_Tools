using System.Xml;
using System.Xml.Xsl;
using System.Text;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "transform":
        TransformXml(args.Skip(1).ToArray());
        break;
    case "validate":
        ValidateXml(args.Skip(1).ToArray());
        break;
    case "pretty":
        PrettyPrintXml(args.Skip(1).ToArray());
        break;
    case "info":
        ShowXmlInfo(args.Skip(1).ToArray());
        break;
    default:
        TransformXml(args);
        break;
}

void TransformXml(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- transform <xml-file> <xslt-file> [output-file]");
        Console.WriteLine("   or: dotnet run -- transform <xml-file> --inline '<xslt>'");
        return;
    }

    var xmlFile = args[0];
    string? xsltFile = null;
    string? inlineXslt = null;
    string? outputFile = null;

    for (var i = 1; i < args.Length; i++)
    {
        if (args[i] == "--inline" && i + 1 < args.Length)
        {
            inlineXslt = args[++i];
        }
        else if (args[i] == "--output" || args[i] == "-o")
        {
            outputFile = args[++i];
        }
        else if (xsltFile == null)
        {
            xsltFile = args[i];
        }
    }

    if (!File.Exists(xmlFile))
    {
        Console.WriteLine($"XML file not found: {xmlFile}");
        return;
    }

    string xsltContent;

    if (inlineXslt != null)
    {
        xsltContent = inlineXslt;
    }
    else if (xsltFile != null && File.Exists(xsltFile))
    {
        xsltContent = File.ReadAllText(xsltFile);
    }
    else
    {
        Console.WriteLine("XSLT file not found or --inline not provided");
        return;
    }

    try
    {
        var xmlContent = File.ReadAllText(xmlFile);
        var result = Transform(xmlContent, xsltContent);

        if (!string.IsNullOrEmpty(outputFile))
        {
            File.WriteAllText(outputFile, result);
            Console.WriteLine($"✓ Transformed to {outputFile}");
        }
        else
        {
            Console.WriteLine(result);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ValidateXml(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- validate <xml-file>");
        return;
    }

    var xmlFile = args[0];
    if (!File.Exists(xmlFile))
    {
        Console.WriteLine($"File not found: {xmlFile}");
        return;
    }

    try
    {
        var settings = new XmlReaderSettings
        {
            DtdProcessing = DtdProcessing.Prohibit,
            ValidationType = ValidationType.None
        };

        using var reader = XmlReader.Create(xmlFile, settings);
        while (reader.Read()) { }

        Console.WriteLine("✓ XML is well-formed");
    }
    catch (XmlException ex)
    {
        Console.WriteLine($"✗ XML validation error:");
        Console.WriteLine($"  Line {ex.LineNumber}, Position {ex.LinePosition}: {ex.Message}");
    }
}

void PrettyPrintXml(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- pretty <xml-file> [output-file]");
        Console.WriteLine("   or: dotnet run -- pretty --stdin");
        return;
    }

    string xmlContent;

    if (args.Contains("--stdin"))
    {
        xmlContent = Console.In.ReadToEnd();
    }
    else
    {
        var xmlFile = args[0];
        if (!File.Exists(xmlFile))
        {
            Console.WriteLine($"File not found: {xmlFile}");
            return;
        }
        xmlContent = File.ReadAllText(xmlFile);
    }

    try
    {
        var doc = new XmlDocument();
        doc.LoadXml(xmlContent);

        var settings = new XmlWriterSettings
        {
            Indent = true,
            IndentChars = "  ",
            NewLineChars = "\n",
            NewLineHandling = NewLineHandling.Replace
        };

        var output = new StringBuilder();
        using (var writer = XmlWriter.Create(output, settings))
        {
            doc.Save(writer);
        }

        var outputFile = args.Length > 1 && args[1] != "--stdin" ? args[1] : null;
        if (!string.IsNullOrEmpty(outputFile))
        {
            File.WriteAllText(outputFile, output.ToString());
            Console.WriteLine($"✓ Formatted XML saved to {outputFile}");
        }
        else
        {
            Console.WriteLine(output.ToString());
        }
    }
    catch (XmlException ex)
    {
        Console.WriteLine($"Error parsing XML: {ex.Message}");
    }
}

void ShowXmlInfo(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- info <xml-file>");
        return;
    }

    var xmlFile = args[0];
    if (!File.Exists(xmlFile))
    {
        Console.WriteLine($"File not found: {xmlFile}");
        return;
    }

    try
    {
        var doc = new XmlDocument();
        doc.Load(xmlFile);

        var stats = GetXmlStats(doc);

        Console.WriteLine($"XML Information: {Path.GetFileName(xmlFile)}");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"File size: {new FileInfo(xmlFile).Length:N0} bytes");
        Console.WriteLine($"Root element: <{doc.DocumentElement?.Name ?? "none"}>");
        Console.WriteLine($"Total elements: {stats["elements"]}");
        Console.WriteLine($"Total attributes: {stats["attributes"]}");
        Console.WriteLine($"Max depth: {stats["depth"]}");
        Console.WriteLine($"Namespaces: {stats["namespaces"]}");

        if (doc.DocumentElement != null)
        {
            Console.WriteLine("\nRoot element children:");
            foreach (XmlNode node in doc.DocumentElement.ChildNodes)
            {
                if (node.NodeType == XmlNodeType.Element)
                {
                    var attrs = node.Attributes?.Count ?? 0;
                    Console.WriteLine($"  <{node.Name}> ({attrs} attrs)");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ShowHelp()
{
    Console.WriteLine("""
        XmlTransformer - Transform XML with XSLT stylesheets

        Usage:
          dotnet run -- transform <xml-file> <xslt-file> [output-file]
          dotnet run -- transform <xml-file> --inline '<xslt>'
          dotnet run -- validate <xml-file>
          dotnet run -- pretty <xml-file> [output-file]
          dotnet run -- pretty --stdin
          dotnet run -- info <xml-file>

        Transform Options:
          --output, -o <file>  Save output to file
          --inline '<xslt>'    Provide XSLT inline

        Examples:
          dotnet run -- transform data.xml stylesheet.xsl
          dotnet run -- transform data.xml stylesheet.xsl output.html
          dotnet run -- transform data.xml --inline '<xsl:stylesheet>...</xsl:stylesheet>'
          dotnet run -- validate data.xml
          dotnet run -- pretty data.xml formatted.xml
          cat data.xml | dotnet run -- pretty --stdin
          dotnet run -- info data.xml

        Sample XSLT:
          <?xml version="1.0"?>
          <xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
            <xsl:template match="/">
              <html>
                <body>
                  <xsl:for-each select="catalog/book">
                    <p><xsl:value-of select="title"/></p>
                  </xsl:for-each>
                </body>
              </html>
            </xsl:template>
          </xsl:stylesheet>
        """);
}

string Transform(string xmlContent, string xsltContent)
{
    using var xmlReader = XmlReader.Create(new StringReader(xmlContent));
    using var xsltReader = XmlReader.Create(new StringReader(xsltContent));

    var xslt = new XslCompiledTransform();
    xslt.Load(xsltReader);

    var output = new StringBuilder();
    using var writer = XmlWriter.Create(output, xslt.OutputSettings);
    xslt.Transform(xmlReader, null, writer);

    return output.ToString();
}

Dictionary<string, int> GetXmlStats(XmlDocument doc)
{
    var stats = new Dictionary<string, int>
    {
        ["elements"] = 0,
        ["attributes"] = 0,
        ["depth"] = 0,
        ["namespaces"] = 0
    };

    var namespaces = new HashSet<string>();
    CountNodes(doc.DocumentElement, 0, stats, namespaces);
    stats["namespaces"] = namespaces.Count;

    return stats;
}

void CountNodes(XmlNode? node, int depth, Dictionary<string, int> stats, HashSet<string> namespaces)
{
    if (node == null)
        return;

    stats["depth"] = Math.Max(stats["depth"], depth);

    if (node.NodeType == XmlNodeType.Element)
    {
        stats["elements"]++;

        if (!string.IsNullOrEmpty(node.NamespaceURI))
            namespaces.Add(node.NamespaceURI);

        if (node.Attributes != null)
        {
            stats["attributes"] += node.Attributes.Count;
            foreach (XmlAttribute attr in node.Attributes)
            {
                if (!string.IsNullOrEmpty(attr.NamespaceURI))
                    namespaces.Add(attr.NamespaceURI);
            }
        }
    }

    foreach (XmlNode child in node.ChildNodes)
    {
        CountNodes(child, depth + 1, stats, namespaces);
    }
}
