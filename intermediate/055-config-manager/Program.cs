using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace ConfigManager;

/// <summary>
/// JSON configuration file manager for reading, writing, and manipulating appsettings.json files.
/// Supports nested configurations, environment variable substitution, and validation.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();

        switch (command)
        {
            case "read":
                ReadConfig(args.ElementAtOrDefault(1) ?? "appsettings.json");
                break;
            case "get":
                GetConfigValue(args.ElementAtOrDefault(1) ?? "appsettings.json", args.ElementAtOrDefault(2) ?? "");
                break;
            case "set":
                SetConfigValue(args.ElementAtOrDefault(1) ?? "appsettings.json", 
                               args.ElementAtOrDefault(2) ?? "", 
                               args.ElementAtOrDefault(3) ?? "");
                break;
            case "validate":
                ValidateConfig(args.ElementAtOrDefault(1) ?? "appsettings.json");
                break;
            case "env":
                SubstituteEnvVars(args.ElementAtOrDefault(1) ?? "appsettings.json");
                break;
            case "merge":
                MergeConfigs(args.ElementAtOrDefault(1) ?? "appsettings.json", 
                             args.ElementAtOrDefault(2) ?? "appsettings.production.json");
                break;
            case "flatten":
                FlattenConfig(args.ElementAtOrDefault(1) ?? "appsettings.json");
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                ShowMenu();
                break;
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("Config Manager - JSON Configuration Utility");
        Console.WriteLine();
        Console.WriteLine("  read <config>              - Display entire config");
        Console.WriteLine("  get <config> <key>         - Get value by key (dot notation)");
        Console.WriteLine("  set <config> <key> <value> - Set value by key");
        Console.WriteLine("  validate <config>          - Validate JSON syntax");
        Console.WriteLine("  env <config>               - Substitute ${ENV} variables");
        Console.WriteLine("  merge <base> <overlay>     - Merge two configs");
        Console.WriteLine("  flatten <config>           - Show flattened key paths");
        Console.WriteLine();
        Console.WriteLine("Example: dotnet run -- get appsettings.json Database:ConnectionString");
    }

    static JsonNode? LoadConfig(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Config file not found: {path}");
            return null;
        }

        try
        {
            var json = File.ReadAllText(path);
            return JsonNode.Parse(json);
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"Invalid JSON: {ex.Message}");
            return null;
        }
    }

    static void ReadConfig(string path)
    {
        var config = LoadConfig(path);
        if (config == null) return;

        Console.WriteLine($"Configuration from '{path}':\n");
        var options = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine(config.ToJsonString(options));
    }

    static void GetConfigValue(string path, string key)
    {
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("Please specify a key (use dot notation for nested keys)");
            return;
        }

        var config = LoadConfig(path);
        if (config == null) return;

        var value = GetNestedValue(config, key);
        if (value == null)
        {
            Console.WriteLine($"Key '{key}' not found in config");
        }
        else
        {
            Console.WriteLine($"{key} = {value}");
        }
    }

    static void SetConfigValue(string path, string key, string value)
    {
        if (string.IsNullOrEmpty(key))
        {
            Console.WriteLine("Please specify a key and value");
            return;
        }

        var config = LoadConfig(path);
        if (config == null) return;

        SetNestedValue(config, key, value);

        var options = new JsonSerializerOptions { WriteIndented = true };
        File.WriteAllText(path, config.ToJsonString(options));
        Console.WriteLine($"Set {key} = {value}");
    }

    static void ValidateConfig(string path)
    {
        if (!File.Exists(path))
        {
            Console.WriteLine($"Config file not found: {path}");
            return;
        }

        try
        {
            var json = File.ReadAllText(path);
            JsonNode.Parse(json);
            Console.WriteLine($"✓ Valid JSON: {path}");

            // Basic validation checks
            var config = JsonNode.Parse(json)!;
            int keyCount = CountKeys(config);
            Console.WriteLine($"  Total keys: {keyCount}");

            if (json.Length > 1024 * 1024)
                Console.WriteLine($"  ⚠ Large file: {json.Length / 1024} KB");
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"✗ Invalid JSON: {ex.Message}");
        }
    }

    static void SubstituteEnvVars(string path)
    {
        var config = LoadConfig(path);
        if (config == null) return;

        SubstituteInNode(config);

        var options = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine("Configuration with environment variables substituted:\n");
        Console.WriteLine(config.ToJsonString(options));
    }

    static void MergeConfigs(string basePath, string overlayPath)
    {
        var baseConfig = LoadConfig(basePath);
        var overlayConfig = LoadConfig(overlayPath);

        if (baseConfig == null || overlayConfig == null) return;

        MergeNodes(baseConfig, overlayConfig);

        var options = new JsonSerializerOptions { WriteIndented = true };
        Console.WriteLine("Merged configuration:\n");
        Console.WriteLine(baseConfig.ToJsonString(options));
    }

    static void FlattenConfig(string path)
    {
        var config = LoadConfig(path);
        if (config == null) return;

        Console.WriteLine($"Flattened keys from '{path}':\n");
        var flatKeys = FlattenNode(config, "");
        foreach (var key in flatKeys.OrderBy(k => k))
        {
            Console.WriteLine($"  {key}");
        }
    }

    // Helper methods

    static JsonNode? GetNestedValue(JsonNode config, string key)
    {
        var parts = key.Split(':', '.');
        JsonNode? current = config;

        foreach (var part in parts)
        {
            if (current is JsonObject obj && obj.ContainsKey(part))
                current = obj[part];
            else
                return null;
        }

        return current;
    }

    static void SetNestedValue(JsonNode config, string key, string value)
    {
        var parts = key.Split(':', '.');
        JsonNode? current = config;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            if (current is JsonObject obj)
            {
                if (!obj.ContainsKey(parts[i]))
                    obj[parts[i]] = new JsonObject();
                current = obj[parts[i]];
            }
        }

        if (current is JsonObject finalObj)
        {
            // Try to parse the value type
            if (int.TryParse(value, out int intVal))
                finalObj[parts[^1]] = intVal;
            else if (double.TryParse(value, out double doubleVal))
                finalObj[parts[^1]] = doubleVal;
            else if (bool.TryParse(value, out bool boolVal))
                finalObj[parts[^1]] = boolVal;
            else
                finalObj[parts[^1]] = value;
        }
    }

    static int CountKeys(JsonNode node)
    {
        int count = 0;
        if (node is JsonObject obj)
        {
            foreach (var kvp in obj)
            {
                count++;
                count += CountKeys(kvp.Value!);
            }
        }
        else if (node is JsonArray arr)
        {
            foreach (var item in arr)
                count += CountKeys(item!);
        }
        return count;
    }

    static void SubstituteInNode(JsonNode? node)
    {
        if (node is JsonObject obj)
        {
            var keys = obj.ToArray().Select(k => k.Key).ToList();
            foreach (var key in keys)
            {
                if (obj[key] is JsonValue val && val.TryGetValue<string>(out var str))
                {
                    obj[key] = SubstituteEnvString(str);
                }
                else
                {
                    SubstituteInNode(obj[key]);
                }
            }
        }
        else if (node is JsonArray arr)
        {
            for (int i = 0; i < arr.Count; i++)
            {
                SubstituteInNode(arr[i]);
            }
        }
    }

    static string SubstituteEnvString(string input)
    {
        return System.Text.RegularExpressions.Regex.Replace(input, @"\$\{(\w+)\}", match =>
        {
            var envVar = match.Groups[1].Value;
            return Environment.GetEnvironmentVariable(envVar) ?? match.Value;
        });
    }

    static void MergeNodes(JsonNode target, JsonNode source)
    {
        if (target is JsonObject targetObj && source is JsonObject sourceObj)
        {
            foreach (var kvp in sourceObj)
            {
                if (targetObj.ContainsKey(kvp.Key))
                {
                    if (targetObj[kvp.Key] is JsonObject && kvp.Value is JsonObject)
                        MergeNodes(targetObj[kvp.Key]!, kvp.Value);
                    else
                        targetObj[kvp.Key] = kvp.Value;
                }
                else
                {
                    targetObj.Add(kvp.Key, kvp.Value);
                }
            }
        }
    }

    static List<string> FlattenNode(JsonNode? node, string prefix)
    {
        var keys = new List<string>();
        if (node is JsonObject obj)
        {
            foreach (var kvp in obj)
            {
                string newKey = string.IsNullOrEmpty(prefix) ? kvp.Key : $"{prefix}:{kvp.Key}";
                if (kvp.Value is JsonObject)
                    keys.AddRange(FlattenNode(kvp.Value, newKey));
                else
                    keys.Add(newKey);
            }
        }
        return keys;
    }
}
