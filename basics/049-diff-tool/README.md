# Text Diff Tool

A CLI tool for comparing text files, strings, or stdin input with visual diff output.

## Usage

```bash
dotnet run --project DiffTool.csproj [options]
```

## Options

| Option | Description |
|--------|-------------|
| `<file1> <file2>` | Compare two files |
| `--stdin <file>` | Compare stdin input with file |
| `--string <s1> <s2>` | Compare two strings |
| `--lines` | Show line-by-line diff |
| `--summary` | Show only summary (default) |

## Examples

```bash
# Compare two files
dotnet run --project DiffTool.csproj file1.txt file2.txt

# Compare two strings
dotnet run --project DiffTool.csproj --string "hello world" "hello word"

# Compare stdin with file
echo "new content" | dotnet run --project DiffTool.csproj --stdin file.txt

# Line-by-line comparison
dotnet run --project DiffTool.csproj --lines file1.txt file2.txt
```

## Sample Output

```
Comparing files:
  File 1: /home/user/original.txt
  File 2: /home/user/modified.txt

Diff Summary:
  Identical: False
  Length 1: 120 characters
  Length 2: 125 characters
  Length difference: +5
  First difference at position: 42
  Character changes: 8

Visual Diff:
  Context around position 42:
  String 1: "...the quick brown fox jumps..."
  String 2: "...the quick brown dog jumps..."

  Character comparison:
    [42] 'f' → 'd'
    [43] 'o' → 'o'
    [44] 'x' → 'g'

---

Line-by-line comparison:
- old line content
+ new line content
~ modified line

Legend:
  -  Line removed
  +  Line added
  ~  Line modified
```

## Concepts Demonstrated

- File I/O and text reading
- String comparison algorithms
- Levenshtein distance (edit distance)
- Line-by-line diff calculation
- Command-line argument parsing
- Console output formatting
- Set operations for comparison
- Character-level analysis
