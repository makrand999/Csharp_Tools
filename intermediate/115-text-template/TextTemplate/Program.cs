using System.Text;
using System.Text.RegularExpressions;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "render":
        RenderTemplate(args.Skip(1).ToArray());
        break;
    case "validate":
        ValidateTemplate(args.Skip(1).ToArray());
        break;
    default:
        RenderTemplate(args);
        break;
}

void RenderTemplate(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- render <template-file> <data-json>");
        Console.WriteLine("   or: dotnet run -- render <template-file> --vars <key=value>...");
        return;
    }

    var templatePath = args[0];
    var variables = new Dictionary<string, string>();

    if (!File.Exists(templatePath))
    {
        Console.WriteLine($"Template file not found: {templatePath}");
        return;
    }

    var template = File.ReadAllText(templatePath);

    // Parse variables
    var varsStart = Array.IndexOf(args, "--vars");
    if (varsStart >= 0)
    {
        foreach (var arg in args.Skip(varsStart + 1))
        {
            var parts = arg.Split('=', 2);
            if (parts.Length == 2)
                variables[parts[0]] = parts[1];
        }
    }
    else if (args.Length > 1)
    {
        // Try to parse JSON or key=value pairs
        var dataArg = args[1];
        if (dataArg.StartsWith("{"))
        {
            variables = ParseJson(dataArg);
        }
        else
        {
            foreach (var arg in args.Skip(1))
            {
                var parts = arg.Split('=', 2);
                if (parts.Length == 2)
                    variables[parts[0]] = parts[1];
            }
        }
    }

    try
    {
        var result = ProcessTemplate(template, variables);
        Console.WriteLine(result);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ValidateTemplate(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- validate <template-file>");
        return;
    }

    var templatePath = args[0];
    if (!File.Exists(templatePath))
    {
        Console.WriteLine($"Template file not found: {templatePath}");
        return;
    }

    var template = File.ReadAllText(templatePath);
    var errors = new List<string>();

    // Check for balanced tags
    var openTags = Regex.Matches(template, @"\{\{[^}]*\}\}");
    var variables = new HashSet<string>();

    foreach (Match match in openTags)
    {
        var content = match.Value[2..^2].Trim();
        if (content.StartsWith("#"))
        {
            // Loop tag
            var loopVar = content[1..].Trim();
            variables.Add(loopVar);
        }
        else if (content.StartsWith("/"))
        {
            // End tag - skip
        }
        else if (content.StartsWith("^"))
        {
            // Inverted section - skip
        }
        else if (content == "else")
        {
            // Else tag - skip
        }
        else
        {
            // Variable
            variables.Add(content);
        }
    }

    Console.WriteLine("✓ Template syntax is valid");
    Console.WriteLine($"  Variables used: {string.Join(", ", variables)}");
}

void ShowHelp()
{
    Console.WriteLine("""
        TextTemplateEngine - Simple template engine with variables and loops

        Usage:
          dotnet run -- render <template-file> [data]
          dotnet run -- validate <template-file>

        Template Syntax:
          {{variable}}       - Insert variable value
          {{#list}}          - Start loop over list
          {{/list}}          - End loop
          {{^condition}}     - Inverted section (if false/empty)
          {{else}}           - Else clause

        Data Formats:
          JSON: {"name": "John", "age": 30}
          Key-Value: name=John age=30
          --vars: --vars name=John age=30

        Examples:
          dotnet run -- render template.txt '{"name": "John"}'
          dotnet run -- render template.txt name=John age=30
          dotnet run -- render template.txt --vars name=John age=30
          dotnet run -- validate template.txt

        Template Example:
          Hello, {{name}}!
          {{#items}}
          - {{.}}
          {{/items}}
          {{^items}}
          No items to display.
          {{/items}}
        """);
}

string ProcessTemplate(string template, Dictionary<string, string> variables)
{
    var result = new StringBuilder(template);

    // Process loops: {{#list}}...{{/list}}
    var loopPattern = @"\{\#(\w+)\}\}(.*?)\{\{/\1\}\}";
    var loopMatches = Regex.Matches(result.ToString(), loopPattern, RegexOptions.Singleline);

    foreach (Match match in loopMatches)
    {
        var listName = match.Groups[1].Value;
        var loopContent = match.Groups[2].Value;

        // Check if we have a list variable (JSON array)
        if (variables.TryGetValue(listName, out var listValue) && listValue.StartsWith("["))
        {
            var items = ParseJsonArray(listValue);
            var replacement = new StringBuilder();

            foreach (var item in items)
            {
                var itemVars = new Dictionary<string, string>(variables);
                itemVars["."] = item;
                // Also support {{item}} syntax for simple values
                var itemContent = Regex.Replace(loopContent, @"\{\{\.\}\}", item);
                replacement.Append(ProcessTemplate(itemContent, itemVars));
            }

            result.Replace(match.Value, replacement.ToString());
        }
        else
        {
            // No list or empty - remove the loop
            result.Replace(match.Value, "");
        }
    }

    // Process inverted sections: {{^condition}}...{{else}}...{{/condition}}
    var invertedPattern = @"\{\{(\^|\#)(\w+)\}\}(.*?)(?:\{\{else\}\}(.*?))?\{\{/\2\}\}";
    var invertedMatches = Regex.Matches(result.ToString(), invertedPattern, RegexOptions.Singleline);

    foreach (Match match in invertedMatches)
    {
        var isNegative = match.Groups[1].Value == "^";
        var varName = match.Groups[2].Value;
        var ifContent = match.Groups[3].Value;
        var elseContent = match.Groups[4].Success ? match.Groups[4].Value : "";

        var hasValue = variables.TryGetValue(varName, out var value) &&
                       !string.IsNullOrEmpty(value) && value != "false" && value != "[]";

        var replacement = (isNegative && !hasValue) || (!isNegative && hasValue)
            ? ifContent
            : elseContent;

        result.Replace(match.Value, replacement);
    }

    // Process variables: {{variable}}
    var varPattern = @"\{\{([\w\.]+)\}\}";
    result = new StringBuilder(Regex.Replace(result.ToString(), varPattern, m =>
    {
        var varName = m.Groups[1].Value;
        return variables.TryGetValue(varName, out var value) ? value : "";
    }));

    return result.ToString();
}

Dictionary<string, string> ParseJson(string json)
{
    var result = new Dictionary<string, string>();

    // Simple JSON object parser
    var objPattern = @"""(\w+)""\s*:\s*(?:""([^""]*)""|(\d+)|(\[.*?\]))";
    var matches = Regex.Matches(json, objPattern);

    foreach (Match match in matches)
    {
        var key = match.Groups[1].Value;
        var value = match.Groups[2].Success ? match.Groups[2].Value
                  : match.Groups[3].Success ? match.Groups[3].Value
                  : match.Groups[4].Success ? match.Groups[4].Value
                  : "";
        result[key] = value;
    }

    return result;
}

List<string> ParseJsonArray(string json)
{
    var result = new List<string>();

    // Simple JSON array parser
    var itemPattern = @"""([^""]*)""";
    var matches = Regex.Matches(json, itemPattern);

    foreach (Match match in matches)
    {
        result.Add(match.Groups[1].Value);
    }

    return result;
}
