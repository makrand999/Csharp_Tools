using System.Text;
using System.Text.Json;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

var (inputFormat, outputFormat, inputPath) = ParseArguments(args);

string inputContent = string.Empty;

if (!string.IsNullOrEmpty(inputPath) && File.Exists(inputPath))
{
    inputContent = await File.ReadAllTextAsync(inputPath);
}
else if (!string.IsNullOrEmpty(inputPath))
{
    Console.Error.WriteLine($"Error: File not found: {inputPath}");
    Environment.Exit(1);
}
else
{
    inputContent = Console.In.ReadToEnd();
}

if (string.IsNullOrWhiteSpace(inputContent))
{
    Console.Error.WriteLine("Error: No input provided.");
    ShowUsage();
    Environment.Exit(1);
}

try
{
    var deserializer = new DeserializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
    
    var serializer = new SerializerBuilder()
        .WithNamingConvention(CamelCaseNamingConvention.Instance)
        .Build();
    
    if (inputFormat == "yaml" && outputFormat == "json")
    {
        var yamlObject = deserializer.Deserialize<object>(inputContent);
        var jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };
        Console.WriteLine(JsonSerializer.Serialize(yamlObject, jsonOptions));
    }
    else if (inputFormat == "json" && outputFormat == "yaml")
    {
        var jsonObject = JsonSerializer.Deserialize<object>(inputContent);
        Console.WriteLine(serializer.Serialize(jsonObject));
    }
    else
    {
        Console.Error.WriteLine($"Error: Unsupported conversion: {inputFormat} -> {outputFormat}");
        Environment.Exit(1);
    }
}
catch (JsonException ex)
{
    Console.Error.WriteLine($"Error: Invalid JSON - {ex.Message}");
    Environment.Exit(1);
}
catch (YamlDotNet.Core.YamlException ex)
{
    Console.Error.WriteLine($"Error: Invalid YAML - {ex.Message}");
    Environment.Exit(1);
}
catch (Exception ex)
{
    Console.Error.WriteLine($"Error: {ex.Message}");
    Environment.Exit(1);
}

static (string inputFormat, string outputFormat, string? inputPath) ParseArguments(string[] args)
{
    string inputFormat = "yaml";
    string outputFormat = "json";
    string? inputPath = null;
    
    for (int i = 0; i < args.Length; i++)
    {
        switch (args[i])
        {
            case "--json-to-yaml":
            case "-j2y":
                inputFormat = "json";
                outputFormat = "yaml";
                break;
            case "--yaml-to-json":
            case "-y2j":
                inputFormat = "yaml";
                outputFormat = "json";
                break;
            case "--input":
            case "-i":
                if (i + 1 < args.Length)
                {
                    inputPath = args[++i];
                }
                break;
            case "--help":
            case "-h":
                ShowUsage();
                Environment.Exit(0);
                break;
        }
    }
    
    if (args.Length > 0 && !args[0].StartsWith("-"))
    {
        inputPath = args[^1];
    }
    
    return (inputFormat, outputFormat, inputPath);
}

static void ShowUsage()
{
    Console.WriteLine(@"YAML Converter - Convert between YAML and JSON

Usage:
  dotnet run --project YamlConverter.csproj [options] [input-file]

Options:
  -y2j, --yaml-to-json    Convert YAML to JSON (default)
  -j2y, --json-to-yaml    Convert JSON to YAML
  -i, --input <file>      Input file path (reads from stdin if not specified)
  -h, --help              Show this help message

Examples:
  echo 'name: John' | dotnet run --project YamlConverter.csproj
  dotnet run --project YamlConverter.csproj config.yaml
  dotnet run --project YamlConverter.csproj -j2y data.json
  dotnet run --project YamlConverter.csproj --json-to-yaml -i input.json");
}
