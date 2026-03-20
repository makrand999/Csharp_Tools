# .gitignore Generator

Generates .gitignore files from templates for various technologies.

## Usage

```bash
dotnet run --project GitignoreGenerator.csproj
```

## Example

```
$ dotnet run --project GitignoreGenerator.csproj

🔧 .gitignore Generator
=======================

Available templates:
  1. dotnet
  2. node
  3. python
  4. java
  5. go
  6. rust
  7. docker
  8. rails

Select templates (comma-separated numbers, e.g., 1,2,3):
> 1,2,3

Generating .gitignore for: dotnet, node, python

✅ .gitignore generated successfully!
📁 Saved to: /path/to/.gitignore

--- Preview ---

# Auto-generated .gitignore
# Generated: 2024-01-15 10:30:00
# Templates: dotnet, node, python

# === DOTNET ===
[Bb]in/
[Oo]bj/
...
```

## Supported Templates

- **dotnet** - .NET/C# projects
- **node** - Node.js/npm projects
- **python** - Python projects
- **java** - Java/Maven/Gradle projects
- **go** - Go projects
- **rust** - Rust/Cargo projects
- **docker** - Docker projects
- **rails** - Ruby on Rails projects

## Concepts Demonstrated

- Dictionary collections
- Multi-template selection
- File I/O
- String interpolation
- Raw string literals (C# 11+)
