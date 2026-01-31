namespace TestGenerator;

/// <summary>
/// Generates xUnit test templates from C# source files.
/// Analyzes classes and methods to create structured test skeletons.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        var config = ParseArgs(args);
        if (config == null) return;

        var generator = new TestGenerator();
        generator.Generate(config);
    }

    static GenerateConfig? ParseArgs(string[] args)
    {
        var config = new GenerateConfig
        {
            SourceFiles = new List<string>(),
            OutputDir = "./Tests",
            Namespace = "Tests",
            Framework = "xunit"
        };

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--help":
                    ShowHelp();
                    return null;

                case "--output":
                case "-o":
                    if (i + 1 < args.Length)
                    {
                        config.OutputDir = args[++i];
                    }
                    break;

                case "--namespace":
                case "-n":
                    if (i + 1 < args.Length)
                    {
                        config.Namespace = args[++i];
                    }
                    break;

                case "--framework":
                case "-f":
                    if (i + 1 < args.Length)
                    {
                        config.Framework = args[++i].ToLower();
                    }
                    break;

                case var arg when !arg.StartsWith("--") && !arg.StartsWith("-"):
                    config.SourceFiles.Add(arg);
                    break;
            }
        }

        if (config.SourceFiles.Count == 0)
        {
            Console.Error.WriteLine("Error: Please specify at least one source file");
            ShowHelp();
            return null;
        }

        return config;
    }

    static void ShowHelp()
    {
        Console.WriteLine("Test Generator - Generate xUnit test templates from C# code");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project TestGenerator.csproj -- <files...> [options]");
        Console.WriteLine();
        Console.WriteLine("Arguments:");
        Console.WriteLine("  files            One or more C# source files to analyze");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -o, --output <dir>     Output directory (default: ./Tests)");
        Console.WriteLine("  -n, --namespace <ns>   Test namespace (default: Tests)");
        Console.WriteLine("  -f, --framework <fw>   Test framework: xunit, nunit, mstest (default: xunit)");
        Console.WriteLine("  --help                 Show this help");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project TestGenerator.csproj -- Services/UserService.cs");
        Console.WriteLine("  dotnet run --project TestGenerator.csproj -- ./*.cs -o ./UnitTests");
        Console.WriteLine("  dotnet run --project TestGenerator.csproj -- Models/*.cs -n MyProject.Tests -f xunit");
    }
}

class GenerateConfig
{
    public List<string> SourceFiles { get; set; } = new();
    public string OutputDir { get; set; } = "./Tests";
    public string Namespace { get; set; } = "Tests";
    public string Framework { get; set; } = "xunit";
}

class TestGenerator
{
    public void Generate(GenerateConfig config)
    {
        if (!Directory.Exists(config.OutputDir))
        {
            Directory.CreateDirectory(config.OutputDir);
            Console.WriteLine($"Created output directory: {config.OutputDir}");
        }

        var allClasses = new List<ClassInfo>();

        foreach (var file in config.SourceFiles)
        {
            var files = ExpandWildcards(file);
            
            foreach (var sourceFile in files)
            {
                if (!File.Exists(sourceFile))
                {
                    Console.Error.WriteLine($"Warning: File not found: {sourceFile}");
                    continue;
                }

                Console.WriteLine($"Analyzing: {sourceFile}");
                var classes = AnalyzeFile(sourceFile);
                allClasses.AddRange(classes);
            }
        }

        if (allClasses.Count == 0)
        {
            Console.WriteLine("No classes found to generate tests for.");
            return;
        }

        Console.WriteLine($"\nFound {allClasses.Count} class(es) to test");

        foreach (var cls in allClasses)
        {
            var testCode = GenerateTestClass(cls, config);
            var outputFile = Path.Combine(config.OutputDir, $"{cls.Name}Tests.cs");
            File.WriteAllText(outputFile, testCode);
            Console.WriteLine($"  ✓ Generated: {outputFile}");
        }

        Console.WriteLine($"\nGenerated {allClasses.Count} test file(s) in {config.OutputDir}");
        Console.WriteLine("\nNext steps:");
        Console.WriteLine("1. Add test project dependencies (xunit, xunit.runner.visualstudio)");
        Console.WriteLine("2. Reference your main project");
        Console.WriteLine("3. Fill in test logic and run tests");
    }

    List<string> ExpandWildcards(string pattern)
    {
        if (pattern.Contains('*') || pattern.Contains('?'))
        {
            var dir = Path.GetDirectoryName(pattern) ?? ".";
            var searchPattern = Path.GetFileName(pattern);
            
            if (Directory.Exists(dir))
            {
                return Directory.GetFiles(dir, searchPattern).ToList();
            }
        }
        return new List<string> { pattern };
    }

    List<ClassInfo> AnalyzeFile(string filePath)
    {
        var classes = new List<ClassInfo>();
        var content = File.ReadAllText(filePath);
        var lines = content.Split('\n');

        var namespaceName = ExtractNamespace(content);
        ClassInfo? currentClass = null;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            // Check for class declaration
            var classMatch = System.Text.RegularExpressions.Regex.Match(line, 
                @"^(public|internal|private)?\s*(static)?\s*class\s+(\w+)");
            
            if (classMatch.Success)
            {
                if (currentClass != null)
                {
                    classes.Add(currentClass);
                }

                currentClass = new ClassInfo
                {
                    Name = classMatch.Groups[3].Value,
                    Namespace = namespaceName,
                    IsStatic = classMatch.Groups[2].Success,
                    Methods = new List<MethodInfo>()
                };
            }

            // Check for method declarations within a class
            if (currentClass != null && !line.StartsWith("//") && !line.StartsWith("/*"))
            {
                var methodMatch = System.Text.RegularExpressions.Regex.Match(line,
                    @"^(public|private|internal|protected)?\s*(static)?\s*(\w+(?:<[^>]+>)?)\s+(\w+)\s*\(([^)]*)\)");

                if (methodMatch.Success && !IsConstructor(currentClass.Name, methodMatch.Groups[4].Value))
                {
                    var methodName = methodMatch.Groups[4].Value;
                    var returnType = methodMatch.Groups[3].Value;
                    var parameters = ParseParameters(methodMatch.Groups[5].Value);

                    // Only include public/internal methods, skip properties
                    if ((line.StartsWith("public") || line.StartsWith("internal")) && 
                        !line.Contains("get;") && !line.Contains("set;"))
                    {
                        currentClass.Methods.Add(new MethodInfo
                        {
                            Name = methodName,
                            ReturnType = returnType,
                            Parameters = parameters,
                            IsStatic = methodMatch.Groups[2].Success
                        });
                    }
                }
            }
        }

        if (currentClass != null)
        {
            classes.Add(currentClass);
        }

        return classes.Where(c => c.Methods.Count > 0).ToList();
    }

    string ExtractNamespace(string content)
    {
        var match = System.Text.RegularExpressions.Regex.Match(content, 
            @"namespace\s+([\w.]+)");
        return match.Success ? match.Groups[1].Value : "MyNamespace";
    }

    List<ParameterInfo> ParseParameters(string paramStr)
    {
        var parameters = new List<ParameterInfo>();
        
        if (string.IsNullOrWhiteSpace(paramStr))
        {
            return parameters;
        }

        var parts = paramStr.Split(',', StringSplitOptions.RemoveEmptyEntries);
        foreach (var part in parts)
        {
            var trimmed = part.Trim();
            var paramMatch = System.Text.RegularExpressions.Regex.Match(trimmed, 
                @"(\w+(?:<[^>]+>)?)\s+(\w+)");
            
            if (paramMatch.Success)
            {
                parameters.Add(new ParameterInfo
                {
                    Type = paramMatch.Groups[1].Value,
                    Name = paramMatch.Groups[2].Value
                });
            }
        }

        return parameters;
    }

    bool IsConstructor(string className, string methodName)
    {
        return className == methodName;
    }

    string GenerateTestClass(ClassInfo cls, GenerateConfig config)
    {
        var sb = new System.Text.StringBuilder();

        sb.AppendLine("using Xunit;");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"namespace {config.Namespace};");
        sb.AppendLine();
        sb.AppendLine($"/// <summary>");
        sb.AppendLine($"/// Unit tests for {cls.Name}");
        sb.AppendLine($"/// </summary>");
        sb.AppendLine($"public class {cls.Name}Tests");
        sb.AppendLine("{");
        sb.AppendLine($"    private readonly {cls.Name} _sut;");
        sb.AppendLine();
        sb.AppendLine($"    public {cls.Name}Tests()");
        sb.AppendLine($"    {{");
        sb.AppendLine($"        _sut = new {cls.Name}();");
        sb.AppendLine($"    }}");
        sb.AppendLine();

        foreach (var method in cls.Methods)
        {
            sb.AppendLine($"    [Fact]");
            sb.AppendLine($"    public void {method.Name}_ReturnsExpectedResult()");
            sb.AppendLine($"    {{");
            
            // Generate arrange section
            sb.AppendLine($"        // Arrange");
            foreach (var param in method.Parameters)
            {
                var defaultValue = GetDefaultValue(param.Type);
                sb.AppendLine($"        var {param.Name} = {defaultValue};");
            }
            
            // Generate act section
            sb.AppendLine();
            sb.AppendLine($"        // Act");
            var callParams = string.Join(", ", method.Parameters.Select(p => p.Name));
            var resultVar = method.ReturnType != "void" ? "var result = " : "";
            sb.AppendLine($"        {resultVar}_sut.{method.Name}({callParams});");
            
            // Generate assert section
            sb.AppendLine();
            sb.AppendLine($"        // Assert");
            if (method.ReturnType != "void")
            {
                sb.AppendLine($"        Assert.NotNull(result);");
            }
            sb.AppendLine($"        // TODO: Add assertions");
            sb.AppendLine($"    }}");
            sb.AppendLine();
        }

        sb.AppendLine("}");

        return sb.ToString();
    }

    string GetDefaultValue(string type)
    {
        return type.ToLower() switch
        {
            "string" => "\"test\"",
            "int" => "42",
            "long" => "42L",
            "double" => "3.14",
            "float" => "3.14f",
            "decimal" => "9.99m",
            "bool" => "true",
            "datetime" => "DateTime.Now",
            _ => $"default({type})"
        };
    }
}

class ClassInfo
{
    public string Name { get; set; } = "";
    public string Namespace { get; set; } = "";
    public bool IsStatic { get; set; }
    public List<MethodInfo> Methods { get; set; } = new();
}

class MethodInfo
{
    public string Name { get; set; } = "";
    public string ReturnType { get; set; } = "";
    public List<ParameterInfo> Parameters { get; set; } = new();
    public bool IsStatic { get; set; }
}

class ParameterInfo
{
    public string Type { get; set; } = "";
    public string Name { get; set; } = "";
}
