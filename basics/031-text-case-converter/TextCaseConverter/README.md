# Text Case Converter

A practical CLI tool for converting text between different naming conventions (camelCase, PascalCase, snake_case, kebab-case, CONSTANT_CASE).

## Usage

```bash
dotnet run --project TextCaseConverter.csproj "<text>" <target-case>
```

### Target Cases

| Case Type | Example |
|-----------|---------|
| `camel` | camelCase |
| `pascal` | PascalCase |
| `snake` | snake_case |
| `kebab` | kebab-case |
| `constant` | CONSTANT_CASE |

## Examples

```bash
# Convert snake_case to camelCase
dotnet run --project TextCaseConverter.csproj "hello_world" camel
# Output: helloWorld

# Convert PascalCase to snake_case
dotnet run --project TextCaseConverter.csproj "HelloWorld" snake
# Output: hello_world

# Convert kebab-case to PascalCase
dotnet run --project TextCaseConverter.csproj "hello-world" pascal
# Output: HelloWorld

# Convert camelCase to CONSTANT_CASE
dotnet run --project TextCaseConverter.csproj "myVariableName" constant
# Output: MY_VARIABLE_NAME

# Convert to kebab-case
dotnet run --project TextCaseConverter.csproj "MyClassName" kebab
# Output: my-class-name
```

## Concepts Demonstrated

- Enum types and pattern matching
- String manipulation and StringBuilder
- Regular expressions for text parsing
- CLI argument parsing
- Switch expressions (C# 8+)
- Extension methods and LINQ
