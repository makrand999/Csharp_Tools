# Test Generator

Automatically generates xUnit test templates from C# source files. Analyzes classes and methods to create structured test skeletons with Arrange-Act-Assert pattern.

## Usage

```bash
dotnet run --project TestGenerator.csproj -- <files...> [options]
```

## Arguments

| Argument | Description |
|----------|-------------|
| `files` | One or more C# source files to analyze |

## Options

| Option | Description |
|--------|-------------|
| `-o, --output <dir>` | Output directory (default: `./Tests`) |
| `-n, --namespace <ns>` | Test namespace (default: `Tests`) |
| `-f, --framework <fw>` | Test framework: xunit, nunit, mstest (default: xunit) |
| `--help` | Show this help |

## Examples

**Generate tests for a single file:**
```bash
dotnet run --project TestGenerator.csproj -- Services/UserService.cs
```

**Generate tests with custom output directory:**
```bash
dotnet run --project TestGenerator.csproj -- Services/*.cs -o ./UnitTests
```

**Generate tests with custom namespace:**
```bash
dotnet run --project TestGenerator.csproj -- Models/*.cs -n MyProject.Tests
```

**Generate for multiple files:**
```bash
dotnet run --project TestGenerator.csproj -- Services/UserService.cs Services/AuthService.cs -o ./Tests
```

## Example Output

**Generated test file (`UserServiceTests.cs`):**
```csharp
using Xunit;
using System;

namespace Tests;

/// <summary>
/// Unit tests for UserService
/// </summary>
public class UserServiceTests
{
    private readonly UserService _sut;

    public UserServiceTests()
    {
        _sut = new UserService();
    }

    [Fact]
    public void GetUserById_ReturnsExpectedResult()
    {
        // Arrange
        var id = 42;

        // Act
        var result = _sut.GetUserById(id);

        // Assert
        Assert.NotNull(result);
        // TODO: Add assertions
    }

    [Fact]
    public void CreateUser_ReturnsExpectedResult()
    {
        // Arrange
        var username = "test";
        var email = "test@example.com";

        // Act
        var result = _sut.CreateUser(username, email);

        // Assert
        Assert.NotNull(result);
        // TODO: Add assertions
    }

    [Fact]
    public void DeleteUser_ReturnsExpectedResult()
    {
        // Arrange
        var id = 42;

        // Act
        _sut.DeleteUser(id);

        // Assert
        // TODO: Add assertions
    }
}
```

## Features

- **Automatic method detection**: Finds all public/internal methods
- **Parameter analysis**: Generates appropriate test variables
- **Return type handling**: Creates assertions for return values
- **Wildcard support**: Use `*.cs` patterns for multiple files
- **Clean structure**: Follows Arrange-Act-Assert pattern
- **Customizable output**: Configure output directory and namespace

## Next Steps After Generation

1. **Add test project dependencies:**
   ```bash
   dotnet add package xunit
   dotnet add package xunit.runner.visualstudio
   dotnet add reference ../YourMainProject/YourMainProject.csproj
   ```

2. **Fill in test logic:** Replace TODO comments with actual assertions

3. **Run tests:**
   ```bash
   dotnet test
   ```

## Concepts Demonstrated

- Regular expressions for code parsing
- File I/O and wildcard expansion
- Code generation and string building
- C# reflection and pattern matching
- Template-based code generation
- Command-line argument parsing
- Directory and file management
