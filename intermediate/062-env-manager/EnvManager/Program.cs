using System.Collections;
using System.Text.Json;

var envManager = new EnvironmentVariableManager();

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "list":
    case "ls":
        envManager.ListVariables(args.ElementAtOrDefault(1));
        break;
        
    case "get":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: Variable name required");
            return;
        }
        envManager.GetVariable(args[1]);
        break;
        
    case "set":
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Error: Name and value required");
            return;
        }
        envManager.SetVariable(args[1], args[2]);
        break;
        
    case "remove":
    case "rm":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: Variable name required");
            return;
        }
        envManager.RemoveVariable(args[1]);
        break;
        
    case "export":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: Output file required");
            return;
        }
        envManager.ExportToFile(args[1], args.ElementAtOrDefault(2));
        break;
        
    case "import":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: Input file required");
            return;
        }
        envManager.ImportFromFile(args[1]);
        break;
        
    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

void ShowHelp()
{
    Console.WriteLine("""
        Environment Variable Manager
        
        Usage:
          dotnet run --project EnvManager/EnvManager.csproj <command> [arguments]
        
        Commands:
          list [filter]       List environment variables (optional filter)
          get <name>          Get a specific variable value
          set <name> <value>  Set a variable (current process only)
          remove <name>       Remove a variable (current process only)
          export <file>       Export variables to JSON file
          import <file>       Import variables from JSON file
        
        Examples:
          dotnet run --project EnvManager/EnvManager.csproj list PATH
          dotnet run --project EnvManager/EnvManager.csproj get HOME
          dotnet run --project EnvManager/EnvManager.csproj export env.json
        """);
}

class EnvironmentVariableManager
{
    public void ListVariables(string? filter = null)
    {
        var envVars = Environment.GetEnvironmentVariables()
            .Cast<DictionaryEntry>()
            .OrderBy(e => e.Key.ToString())
            .ToList();
        
        if (!string.IsNullOrEmpty(filter))
        {
            envVars = envVars
                .Where(e => e.Key.ToString()?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true)
                .ToList();
        }
        
        Console.WriteLine($"Found {envVars.Count} environment variables:\n");
        
        foreach (DictionaryEntry env in envVars)
        {
            var key = env.Key.ToString() ?? "";
            var value = env.Value?.ToString() ?? "";
            var displayValue = value.Length > 60 ? value[..57] + "..." : value;
            Console.WriteLine($"{key,-30} = {displayValue}");
        }
    }
    
    public void GetVariable(string name)
    {
        var value = Environment.GetEnvironmentVariable(name);
        if (value == null)
        {
            Console.WriteLine($"Variable '{name}' not found");
        }
        else
        {
            Console.WriteLine($"{name}={value}");
        }
    }
    
    public void SetVariable(string name, string value)
    {
        Environment.SetEnvironmentVariable(name, value);
        Console.WriteLine($"Set {name}={value}");
        Console.WriteLine("Note: This only affects the current process");
    }
    
    public void RemoveVariable(string name)
    {
        Environment.SetEnvironmentVariable(name, null);
        Console.WriteLine($"Removed variable '{name}'");
        Console.WriteLine("Note: This only affects the current process");
    }
    
    public void ExportToFile(string filePath, string? filter = null)
    {
        var envVars = Environment.GetEnvironmentVariables()
            .Cast<DictionaryEntry>()
            .Where(e => filter == null || e.Key.ToString()?.Contains(filter, StringComparison.OrdinalIgnoreCase) == true)
            .ToDictionary(
                e => e.Key.ToString() ?? "",
                e => e.Value?.ToString() ?? ""
            );
        
        var json = JsonSerializer.Serialize(envVars, new JsonSerializerOptions 
        { 
            WriteIndented = true 
        });
        
        File.WriteAllText(filePath, json);
        Console.WriteLine($"Exported {envVars.Count} variables to {filePath}");
    }
    
    public void ImportFromFile(string filePath)
    {
        if (!File.Exists(filePath))
        {
            Console.Error.WriteLine($"File not found: {filePath}");
            return;
        }
        
        var json = File.ReadAllText(filePath);
        var envVars = JsonSerializer.Deserialize<Dictionary<string, string>>(json) 
            ?? new Dictionary<string, string>();
        
        foreach (var kvp in envVars)
        {
            Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            Console.WriteLine($"Set {kvp.Key}");
        }
        
        Console.WriteLine($"Imported {envVars.Count} variables (current process only)");
    }
}
