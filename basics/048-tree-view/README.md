# Directory Tree Visualizer

A CLI tool that displays directory structure as a tree, similar to the Unix `tree` command.

## Usage

```bash
dotnet run --project TreeView.csproj [path] [max-depth] [options]
```

## Arguments

| Argument | Description |
|----------|-------------|
| `path` | Directory to visualize (default: current directory) |
| `max-depth` | Maximum depth to traverse (default: 10) |

## Options

| Option | Description |
|--------|-------------|
| `--dirs-only` | Show only directories, no files |
| `--hidden`, `-a` | Include hidden files and directories |

## Examples

```bash
# Show current directory tree
dotnet run --project TreeView.csproj

# Show specific directory
dotnet run --project TreeView.csproj /path/to/project

# Limit depth to 2 levels
dotnet run --project TreeView.csproj . 2

# Show only directories
dotnet run --project TreeView.csproj . 10 --dirs-only

# Include hidden files
dotnet run --project TreeView.csproj . 5 --hidden
```

## Sample Output

```
300
│
├── AGENT.md
├── INSTRUCTION.md
├── basics/
│   ├── 006-hello-world/
│   ├── 007-variables/
│   ├── 008-operators/
│   └── 009-conditionals/
├── tools/
│   ├── FileOrganizer/
│   │   ├── FileOrganizer.csproj (2.1 KB)
│   │   ├── Program.cs (5.3 KB)
│   │   └── README.md (1.2 KB)
│   ├── PasswordGenerator/
│   │   ├── PasswordGenerator.csproj (1.8 KB)
│   │   └── Program.cs (3.2 KB)
│   └── README.md
└── intermediate/
    └── [pending]
```

## Concepts Demonstrated

- Directory and file system traversal
- Recursive algorithms
- FileSystemInfo and DirectoryInfo classes
- File attributes (hidden, system)
- String formatting and tree drawing
- Command-line argument parsing
- Exception handling for access denied scenarios
- LINQ for sorting and filtering
