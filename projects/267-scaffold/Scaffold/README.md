# Project Scaffolder

Creates project templates with pre-configured structure for various .NET project types.

## Usage

```bash
# Basic usage
dotnet run --project Scaffold.csproj [project_name]

# With project type
dotnet run --project Scaffold.csproj MyApi --type webapi
```

## Project Types

| Type | Description |
|------|-------------|
| `console` | .NET Console Application |
| `webapi` | ASP.NET Core Web API with Swagger |
| `classlib` | Class Library |
| `xunit` | xUnit Test Project |
| `sln` | Full solution with API, Core, and Tests |

## Example

```
$ dotnet run --project Scaffold.csproj MyApi --type webapi

🏗️  Project Scaffolder
=====================

Creating project: MyApi
Type: webapi

  Created: MyApi/MyApi.csproj
  Created: MyApi/Program.cs
  Created: MyApi/appsettings.json
  Created: MyApi/Controllers/WeatherForecastController.cs
  Created: MyApi/README.md

✅ Project 'MyApi' created successfully!
📁 Location: /path/to/MyApi
```

## Generated Structure

### Web API
```
MyApi/
├── Controllers/
│   └── WeatherForecastController.cs
├── Models/
├── Services/
├── Program.cs
├── appsettings.json
├── MyApi.csproj
└── README.md
```

### Solution
```
MyApi/
├── MyApi.sln
├── MyApi.Api/
│   └── (full webapi structure)
├── MyApi.Core/
│   └── (classlib structure)
└── MyApi.Tests/
    └── (xunit test structure)
```

## Concepts Demonstrated

- Directory and file creation
- Template-based code generation
- String interpolation
- Project structure conventions
- Multi-project solution creation
