using System.Text;
using System.Text.Json;
using System.Xml;
using System.Xml.Linq;

var xmlInput = args.Length > 0 && File.Exists(args[0])
    ? await File.ReadAllTextAsync(args[0])
    : Console.In.ReadToEnd();

if (string.IsNullOrWhiteSpace(xmlInput))
{
    Console.Error.WriteLine("Error: No XML input provided.");
    Console.Error.WriteLine("Usage: dotnet run --project XmlToJson.csproj [input.xml]");
    Environment.Exit(1);
}

try
{
    var xmlDocument = XDocument.Parse(xmlInput);
    var jsonObject = ConvertXmlNode(xmlDocument.Root);
    
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    Console.WriteLine(JsonSerializer.Serialize(jsonObject, options));
}
catch (XmlException ex)
{
    Console.Error.WriteLine($"Error: Invalid XML - {ex.Message}");
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.Exit(1);
}

static object? ConvertXmlNode(XElement element)
{
    var attributes = element.Attributes().ToDictionary(
        a => a.Name.LocalName,
        a => a.Value
    );
    
    var children = element.Nodes().OfType<XElement>().ToList();
    var textContent = element.Nodes()
        .OfType<XText>()
        .Select(t => t.Value.Trim())
        .FirstOrDefault(t => !string.IsNullOrEmpty(t));
    
    if (children.Count == 0 && attributes.Count == 0)
    {
        return string.IsNullOrEmpty(textContent) ? null : textContent;
    }
    
    if (children.Count == 0 && attributes.Count == 0 && !string.IsNullOrEmpty(textContent))
    {
        return textContent;
    }
    
    var result = new Dictionary<string, object?>();
    
    foreach (var attr in attributes)
    {
        result[$"@{attr.Key}"] = attr.Value;
    }
    
    if (children.Count > 0)
    {
        var childrenDict = new Dictionary<string, object?>();
        
        foreach (var child in children.GroupBy(c => c.Name.LocalName))
        {
            var childElements = child.ToList();
            if (childElements.Count == 1)
            {
                childrenDict[child.Key] = ConvertXmlNode(childElements[0]);
            }
            else
            {
                childrenDict[child.Key] = childElements
                    .Select(ConvertXmlNode)
                    .ToList();
            }
        }
        
        if (!string.IsNullOrEmpty(textContent))
        {
            childrenDict["#text"] = textContent;
        }
        
        result[element.Name.LocalName] = childrenDict;
    }
    else if (!string.IsNullOrEmpty(textContent))
    {
        result["#text"] = textContent;
    }
    
    return result;
}
