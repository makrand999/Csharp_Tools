namespace Scaffold;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🏗️  Project Scaffolder");
        Console.WriteLine("=====================\n");

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run --project Scaffold.csproj [project_name] [--type TYPE]");
            Console.WriteLine("\nProject types:");
            Console.WriteLine("  webapi     - ASP.NET Core Web API");
            Console.WriteLine("  console    - .NET Console App");
            Console.WriteLine("  classlib   - Class Library");
            Console.WriteLine("  xunit      - xUnit Test Project");
            Console.WriteLine("  sln        - Solution file with projects");
            return;
        }

        string projectName = args[0];
        string projectType = "console";
        var typeArg = args.Skip(1).FirstOrDefault(a => a == "--type");
        if (typeArg != null)
        {
            var typeIndex = Array.IndexOf(args, "--type");
            if (typeIndex >= 0 && typeIndex + 1 < args.Length)
                projectType = args[typeIndex + 1];
        }

        Console.WriteLine($"Creating project: {projectName}");
        Console.WriteLine($"Type: {projectType}\n");

        string baseDir = Path.Combine(Directory.GetCurrentDirectory(), projectName);
        Directory.CreateDirectory(baseDir);

        switch (projectType)
        {
            case "webapi":
                CreateWebApi(baseDir, projectName);
                break;
            case "console":
                CreateConsole(baseDir, projectName);
                break;
            case "classlib":
                CreateClassLib(baseDir, projectName);
                break;
            case "xunit":
                CreateXUnitTest(baseDir, projectName);
                break;
            case "sln":
                CreateSolution(baseDir, projectName);
                break;
            default:
                Console.WriteLine($"Unknown project type: {projectType}");
                return;
        }

        Console.WriteLine($"\n✅ Project '{projectName}' created successfully!");
        Console.WriteLine($"📁 Location: {baseDir}");
        Console.WriteLine("\nTo build and run:");
        Console.WriteLine($"  cd {projectName}");
        Console.WriteLine("  dotnet build");
        Console.WriteLine("  dotnet run");
    }

    private static void CreateWebApi(string baseDir, string name)
    {
        Directory.CreateDirectory(Path.Combine(baseDir, "Controllers"));
        Directory.CreateDirectory(Path.Combine(baseDir, "Models"));
        Directory.CreateDirectory(Path.Combine(baseDir, "Services"));

        CreateCsproj(baseDir, name, "Microsoft.NET.Sdk.Web");
        CreateFile(Path.Combine(baseDir, "Program.cs"), GetWebApiProgram(name));
        CreateFile(Path.Combine(baseDir, "appsettings.json"), GetAppSettings());
        CreateFile(Path.Combine(baseDir, "Controllers", "WeatherForecastController.cs"), GetWeatherController(name));
        CreateReadme(baseDir, name, "ASP.NET Core Web API");
    }

    private static void CreateConsole(string baseDir, string name)
    {
        CreateCsproj(baseDir, name, "Microsoft.NET.Sdk");
        CreateFile(Path.Combine(baseDir, "Program.cs"), GetConsoleProgram(name));
        CreateReadme(baseDir, name, ".NET Console Application");
    }

    private static void CreateClassLib(string baseDir, string name)
    {
        Directory.CreateDirectory(Path.Combine(baseDir, "Models"));
        Directory.CreateDirectory(Path.Combine(baseDir, "Services"));

        CreateCsproj(baseDir, name, "Microsoft.NET.Sdk");
        CreateFile(Path.Combine(baseDir, "Class1.cs"), GetClass1Content());
        CreateReadme(baseDir, name, "Class Library");
    }

    private static void CreateXUnitTest(string baseDir, string name)
    {
        CreateCsproj(baseDir, name, "Microsoft.NET.Sdk", new[]
        {
            ("PackageReference", "xunit", "2.6.2"),
            ("PackageReference", "xunit.runner.visualstudio", "2.5.4"),
            ("PackageReference", "Microsoft.NET.Test.Sdk", "17.8.0"),
            ("PackageReference", "FluentAssertions", "6.12.0"),
        });

        CreateFile(Path.Combine(baseDir, "UnitTest1.cs"), GetUnitTestContent(name));
        CreateReadme(baseDir, name, "xUnit Test Project");
    }

    private static void CreateSolution(string baseDir, string name)
    {
        // Create main solution
        string solutionName = $"{name}.sln";
        CreateFile(Path.Combine(baseDir, solutionName), GetSolutionContent(name));

        // Create API project
        string apiDir = Path.Combine(baseDir, $"{name}.Api");
        Directory.CreateDirectory(apiDir);
        CreateWebApi(apiDir, $"{name}.Api");

        // Create Core project
        string coreDir = Path.Combine(baseDir, $"{name}.Core");
        Directory.CreateDirectory(coreDir);
        CreateClassLib(coreDir, $"{name}.Core");

        // Create Tests project
        string testsDir = Path.Combine(baseDir, $"{name}.Tests");
        Directory.CreateDirectory(testsDir);
        CreateXUnitTest(testsDir, $"{name}.Tests");

        Console.WriteLine($"  Created: {name}.Api/");
        Console.WriteLine($"  Created: {name}.Core/");
        Console.WriteLine($"  Created: {name}.Tests/");
    }

    private static void CreateCsproj(string baseDir, string name, string sdk, (string type, string name, string version)[]? packages = null)
    {
        var csproj = $$"""
            <Project Sdk="{{sdk}}">
              <PropertyGroup>
                <TargetFramework>net8.0</TargetFramework>
                <Nullable>enable</Nullable>
                <ImplicitUsings>enable</ImplicitUsings>
              </PropertyGroup>
            {{(packages != null ? string.Join("\n", packages.Select(p => 
                p.type == "PackageReference" 
                    ? $"  <ItemGroup>\n    <PackageReference Include=\"{p.name}\" Version=\"{p.version}\" />\n  </ItemGroup>" 
                    : "")) : "")}}
            </Project>
            """;
        CreateFile(Path.Combine(baseDir, $"{name}.csproj"), csproj);
    }

    private static void CreateFile(string path, string content)
    {
        File.WriteAllText(path, content);
        Console.WriteLine($"  Created: {Path.GetRelativePath(Directory.GetCurrentDirectory(), path)}");
    }

    private static void CreateReadme(string baseDir, string name, string description)
    {
        string readme = $$"""
            # {{name}}

            {{description}}

            ## Getting Started

            ### Build
            ```bash
            dotnet build
            ```

            ### Run
            ```bash
            dotnet run
            ```

            ### Test
            ```bash
            dotnet test
            ```

            ## Project Structure

            ```
            {{name}}/
            ├── Program.cs
            ├── {{name}}.csproj
            └── README.md
            ```
            """;
        CreateFile(Path.Combine(baseDir, "README.md"), readme);
    }

    // Template content methods
    private static string GetWebApiProgram(string name) => """
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
        """;

    private static string GetConsoleProgram(string name) => $$"""
        namespace {{name}};

        class Program
        {
            static void Main(string[] args)
            {
                Console.WriteLine("Hello from {{name}}!");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
        }
        """;

    private static string GetClass1Content() => """
        namespace ClassLibrary;

        public class Class1
        {
            public static string GetMessage()
            {
                return "Hello from Class Library!";
            }
        }
        """;

    private static string GetUnitTestContent(string name) => $$"""
        namespace {{name}}Tests;

        public class UnitTest1
        {
            [Fact]
            public void Test1()
            {
                // Arrange
                var expected = true;

                // Act
                var result = true;

                // Assert
                Assert.Equal(expected, result);
            }
        }
        """;

    private static string GetWeatherController(string name) => $$"""
        using Microsoft.AspNetCore.Mvc;

        namespace {{name}}.Controllers;

        [ApiController]
        [Route("[controller]")]
        public class WeatherForecastController : ControllerBase
        {
            private static readonly string[] Summaries = new[]
            {
                "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
            };

            [HttpGet]
            public IEnumerable<WeatherForecast> Get()
            {
                return Enumerable.Range(1, 5).Select(index => new WeatherForecast
                {
                    Date = DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    TemperatureC = Random.Shared.Next(-20, 55),
                    Summary = Summaries[Random.Shared.Next(Summaries.Length)]
                })
                .ToArray();
            }
        }

        public class WeatherForecast
        {
            public DateOnly Date { get; set; }
            public int TemperatureC { get; set; }
            public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
            public string? Summary { get; set; }
        }
        """;

    private static string GetAppSettings() => """
        {
          "Logging": {
            "LogLevel": {
              "Default": "Information",
              "Microsoft.AspNetCore": "Warning"
            }
          },
          "AllowedHosts": "*"
        }
        """;

    private static string GetSolutionContent(string name)
    {
        string guid1 = Guid.NewGuid().ToString("B").ToUpper();
        string guid2 = Guid.NewGuid().ToString("B").ToUpper();
        string guid3 = Guid.NewGuid().ToString("B").ToUpper();
        string guid4 = Guid.NewGuid().ToString("B").ToUpper();
        string guid5 = Guid.NewGuid().ToString("B").ToUpper();
        string guid6 = Guid.NewGuid().ToString("B").ToUpper();
        string guid7 = Guid.NewGuid().ToString("B").ToUpper();
        
        return $$"""
        Microsoft Visual Studio Solution File, Format Version 12.00
        # Visual Studio Version 17
        VisualStudioVersion = 17.0.31903.59
        MinimumVisualStudioVersion = 10.0.40219.1
        Project("{FA20D68D-2D3A-416B-AE09-96E43D0A37C3}") = "{{name}}.Api", "{{name}}.Api\{{name}}.Api.csproj", "{{guid1}}"
        EndProject
        Project("{FA20D68D-2D3A-416B-AE09-96E43D0A37C3}") = "{{name}}.Core", "{{name}}.Core\{{name}}.Core.csproj", "{{guid2}}"
        EndProject
        Project("{FA20D68D-2D3A-416B-AE09-96E43D0A37C3}") = "{{name}}.Tests", "{{name}}.Tests\{{name}}.Tests.csproj", "{{guid3}}"
        EndProject
        Global
        	GlobalSection(SolutionConfigurationPlatforms) = preSolution
        		Debug|Any CPU = Debug|Any CPU
        		Release|Any CPU = Release|Any CPU
        	EndGlobalSection
        	GlobalSection(ProjectConfigurationPlatforms) = postSolution
        		{{guid4}}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
        		{{guid4}}.Debug|Any CPU.Build.0 = Debug|Any CPU
        		{{guid5}}.Release|Any CPU.ActiveCfg = Release|Any CPU
        		{{guid5}}.Release|Any CPU.Build.0 = Release|Any CPU
        	EndGlobalSection
        EndGlobal
        """;
    }

    private static string GenerateGuid() => Guid.NewGuid().ToString("B").ToUpper();
}
