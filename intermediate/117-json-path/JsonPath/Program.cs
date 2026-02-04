using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "query":
        QueryJson(args.Skip(1).ToArray());
        break;
    case "get":
        GetPath(args.Skip(1).ToArray());
        break;
    case "keys":
        ListKeys(args.Skip(1).ToArray());
        break;
    case "flatten":
        FlattenJson(args.Skip(1).ToArray());
        break;
    default:
        QueryJson(args);
        break;
}

void QueryJson(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- query <file.json> <path>");
        Console.WriteLine("   or: dotnet run -- query '<json>' <path>");
        return;
    }

    var source = args[0];
    var path = args[1];

    var jsonNode = LoadJson(source);
    if (jsonNode == null)
        return;

    try
    {
        var result = EvaluatePath(jsonNode, path);
        Console.WriteLine(FormatOutput(result));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void GetPath(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- get <file.json> <json-path>");
        Console.WriteLine("Path examples: $.store.book[0].title, $..author");
        return;
    }

    var source = args[0];
    var path = args[1];

    var jsonNode = LoadJson(source);
    if (jsonNode == null)
        return;

    try
    {
        var result = EvaluatePath(jsonNode, path);
        Console.WriteLine(FormatOutput(result));
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ListKeys(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- keys <file.json>");
        Console.WriteLine("   or: dotnet run -- keys '<json>'");
        return;
    }

    var source = args[0];
    var jsonNode = LoadJson(source);
    if (jsonNode == null)
        return;

    if (jsonNode is JsonObject obj)
    {
        Console.WriteLine("Keys:");
        foreach (var key in obj.Select(kvp => kvp.Key))
        {
            var value = obj[key];
            var type = value switch
            {
                JsonObject => "object",
                JsonArray => "array",
                JsonValue v => v.GetValueKind().ToString().ToLower(),
                _ => "unknown"
            };
            Console.WriteLine($"  {key} ({type})");
        }
    }
    else if (jsonNode is JsonArray arr)
    {
        Console.WriteLine($"Array with {arr.Count} items");
    }
    else
    {
        Console.WriteLine($"Value: {jsonNode}");
    }
}

void FlattenJson(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- flatten <file.json>");
        Console.WriteLine("   or: dotnet run -- flatten '<json>'");
        return;
    }

    var source = args[0];
    var jsonNode = LoadJson(source);
    if (jsonNode == null)
        return;

    var flattened = Flatten(jsonNode, "");

    Console.WriteLine("Flattened JSON:");
    foreach (var kvp in flattened)
    {
        Console.WriteLine($"  {kvp.Key} = {kvp.Value}");
    }
}

void ShowHelp()
{
    Console.WriteLine("""
        JsonPathQuery - Query JSON files using path expressions

        Usage:
          dotnet run -- query <file.json> <path>
          dotnet run -- query '<json>' <path>
          dotnet run -- get <file.json> <json-path>
          dotnet run -- keys <file.json>
          dotnet run -- flatten <file.json>

        Path Syntax:
          $                    - Root
          .property            - Child property
          ['property']         - Child property (bracket notation)
          [0]                  - Array index
          [*]                  - All array items
          ..property           - Recursive descent
          .property[?(@.x>5)]  - Filter (basic support)

        Examples:
          dotnet run -- query data.json $
          dotnet run -- query data.json $.store
          dotnet run -- query data.json $.store.book[0]
          dotnet run -- query data.json '$.store.book[*].title'
          dotnet run -- query data.json '$..author'
          dotnet run -- keys data.json
          dotnet run -- flatten data.json
        """);
}

JsonNode? LoadJson(string source)
{
    try
    {
        if (source.StartsWith("{") || source.StartsWith("["))
        {
            return JsonNode.Parse(source);
        }
        else if (File.Exists(source))
        {
            var content = File.ReadAllText(source);
            return JsonNode.Parse(content);
        }
        else
        {
            Console.WriteLine($"File not found: {source}");
            return null;
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error parsing JSON: {ex.Message}");
        return null;
    }
}

JsonNode? EvaluatePath(JsonNode root, string path)
{
    if (path == "$")
        return root;

    path = path.TrimStart('$').TrimStart('.');

    var current = root;
    var parts = SplitPath(path);

    foreach (var part in parts)
    {
        if (current == null)
            throw new Exception($"Path not found at: {part}");

        if (part == "..")
            continue; // Handle recursive descent separately

        if (part.StartsWith(".."))
        {
            // Recursive descent
            var prop = part[2..];
            return FindRecursive(current, prop);
        }

        if (part.StartsWith("[") && part.EndsWith("]"))
        {
            var indexStr = part[1..^1];

            if (indexStr == "*")
            {
                // All items
                if (current is JsonArray arr)
                    return arr;
                throw new Exception("Expected array");
            }
            else if (int.TryParse(indexStr, out var index))
            {
                if (current is JsonArray array)
                {
                    if (index < 0 || index >= array.Count)
                        throw new Exception($"Array index out of range: {index}");
                    current = array[index]?.DeepClone();
                }
                else
                    throw new Exception("Expected array");
            }
            else
            {
                // Property access with bracket notation
                var prop = indexStr.Trim('\'', '"');
                if (current is JsonObject obj)
                    current = obj[prop]?.DeepClone();
                else
                    throw new Exception($"Expected object, got {current?.GetType().Name}");
            }
        }
        else
        {
            // Property access
            if (current is JsonObject jsonObject)
                current = jsonObject[part]?.DeepClone();
            else
                throw new Exception($"Expected object, got {current?.GetType().Name}");
        }
    }

    return current;
}

List<string> SplitPath(string path)
{
    var parts = new List<string>();
    var current = new StringBuilder();
    var inBrackets = 0;

    foreach (var c in path)
    {
        if (c == '[')
        {
            if (current.Length > 0 && inBrackets == 0)
            {
                parts.Add(current.ToString());
                current.Clear();
            }
            inBrackets++;
            current.Append(c);
        }
        else if (c == ']')
        {
            inBrackets--;
            current.Append(c);
            if (inBrackets == 0)
            {
                parts.Add(current.ToString());
                current.Clear();
            }
        }
        else if (c == '.' && inBrackets == 0)
        {
            if (current.Length > 0)
            {
                parts.Add(current.ToString());
                current.Clear();
            }
        }
        else
        {
            current.Append(c);
        }
    }

    if (current.Length > 0)
        parts.Add(current.ToString());

    return parts;
}

JsonNode? FindRecursive(JsonNode node, string property)
{
    if (node is JsonObject obj)
    {
        if (obj.ContainsKey(property))
            return obj[property]?.DeepClone();

        foreach (var kvp in obj)
        {
            var result = FindRecursive(kvp.Value, property);
            if (result != null)
                return result;
        }
    }
    else if (node is JsonArray arr)
    {
        foreach (var item in arr)
        {
            var result = FindRecursive(item, property);
            if (result != null)
                return result;
        }
    }

    return null;
}

Dictionary<string, string> Flatten(JsonNode node, string prefix)
{
    var result = new Dictionary<string, string>();

    if (node is JsonObject obj)
    {
        foreach (var kvp in obj)
        {
            var key = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}.{kvp.Key}";
            var flattened = Flatten(kvp.Value, key);
            foreach (var f in flattened)
                result[f.Key] = f.Value;
        }
    }
    else if (node is JsonArray arr)
    {
        for (var i = 0; i < arr.Count; i++)
        {
            var key = string.IsNullOrEmpty(prefix) ? $"[{i}]" : $"{prefix}[{i}]";
            var flattened = Flatten(arr[i], key);
            foreach (var f in flattened)
                result[f.Key] = f.Value;
        }
    }
    else
    {
        result[prefix] = node?.ToString() ?? "null";
    }

    return result;
}

string FormatOutput(JsonNode? node)
{
    if (node == null)
        return "null";

    var options = new JsonSerializerOptions { WriteIndented = true };
    return node.ToJsonString(options);
}
