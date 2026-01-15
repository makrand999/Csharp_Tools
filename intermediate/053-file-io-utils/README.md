# File I/O Utilities

A practical batch file operations tool for common file management tasks including listing, copying, moving, deleting, and analyzing files.

## Usage

```bash
dotnet run --project FileIoUtils.csproj [command] [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `list <path> [pattern]` | List files matching pattern |
| `copy <src> <dest> [pattern]` | Copy files to destination |
| `move <src> <dest> [pattern]` | Move files to destination |
| `delete <path> [pattern]` | Delete files matching pattern |
| `duplicates <path>` | Find duplicate files by MD5 hash |
| `size <path>` | Analyze disk usage by extension |
| `recent <path> [count]` | Show recently modified files |

## Examples

```bash
# List all C# files
dotnet run --project FileIoUtils.csproj list . "*.cs"

# Copy all text files to backup folder
dotnet run --project FileIoUtils.csproj copy ./docs ./backup "*.txt"

# Find duplicate files
dotnet run --project FileIoUtils.csproj duplicates ./photos

# Analyze disk usage
dotnet run --project FileIoUtils.csproj size ./project

# Show 5 most recently modified files
dotnet run --project FileIoUtils.csproj recent . 5
```

## Sample Output

```
$ dotnet run --project FileIoUtils.csproj size .
Directory Analysis: .
  Total files: 15
  Total size:  1.25 MB

  By extension:
    .cs           850 KB
    .csproj       200 KB
    .md           150 KB
    .json          50 KB
```

## Concepts Demonstrated

- File I/O operations (File, Directory, FileInfo)
- Path manipulation and combining
- File hashing with MD5
- Pattern matching with wildcards
- Batch file operations
- Size formatting and human-readable output
- Error handling for file operations
- LINQ for file filtering and sorting
