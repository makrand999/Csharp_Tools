# .gitignore Generator

An interactive CLI tool that generates `.gitignore` files based on selected technology stacks.

## Usage

```bash
dotnet run --project GitignoreGenerator.csproj
```

## Supported Technologies

- **dotnet** - .NET / C# projects
- **node** - Node.js / npm / yarn
- **python** - Python projects
- **java** - Java / Gradle / Maven
- **go** - Go modules
- **rust** - Rust / Cargo
- **ruby** - Ruby / Bundler
- **php** - PHP / Composer
- **docker** - Docker
- **vscode** - Visual Studio Code
- **idea** - IntelliJ IDEA / Android Studio
- **os** - Operating system files (macOS, Windows, Linux)

## Example Session

```
.gitignore Generator
------------------------------------------------------------

Select technologies for your project:

   1. dotnet
   2. node
   3. python
   4. java
   5. go
   6. rust
   7. ruby
   8. php
   9. docker
  10. vscode
  11. idea
  12. os

  0. Generate (finish selection)

Enter choice (0 to finish): 1
  ✓ Added: dotnet
Enter choice (0 to finish): 2
  ✓ Added: node
Enter choice (0 to finish): 10
  ✓ Added: vscode
Enter choice (0 to finish): 0

Generating .gitignore for: dotnet, node, vscode
------------------------------------------------------------
# Auto-generated .gitignore
# Generated for: dotnet, node, vscode
# Created: 2024-03-31 10:30:00

# DOTNET
[Bb]in/
[Oo]bj/
*.user
...

# NODE
node_modules/
npm-debug.log*
...

# VSCODE
.vscode/*
!.vscode/settings.json
...

------------------------------------------------------------
Save to .gitignore file? (y/n): y

✓ Saved to: /path/to/project/.gitignore
```

## Concepts Demonstrated

- Dictionary collections
- Multi-selection interactive menu
- String building with StringBuilder
- File I/O operations
- Raw string literals (C# 11+)
- Technology-specific configurations
