# FileTagger

Add metadata tags to files, stored in a sidecar JSON file. Organize and search files using custom tags without modifying the original files.

## Usage

```bash
dotnet run --project FileTagger.csproj -- <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `add <file> <tags...>` | Add tags to a file |
| `remove <file> <tags...>` | Remove tags from a file |
| `list [path]` | List tagged files |
| `search <tags...>` | Search files by tags |
| `stats` | Show tag statistics |

## Search Options

- `--all` - Match ALL specified tags (AND logic)
- `--any` - Match ANY tag (OR logic, default)

## Examples

```bash
# Add tags to files
dotnet run --project FileTagger.csproj -- add document.pdf work important
dotnet run --project FileTagger.csproj -- add photo.jpg vacation 2025 summer

# Remove tags
dotnet run --project FileTagger.csproj -- remove document.pdf important

# List all tagged files
dotnet run --project FileTagger.csproj -- list

# List tagged files in a specific directory
dotnet run --project FileTagger.csproj -- list ./documents

# Search for files with specific tags
dotnet run --project FileTagger.csproj -- search work
dotnet run --project FileTagger.csproj -- search vacation summer
dotnet run --project FileTagger.csproj -- search work important --all

# View statistics
dotnet run --project FileTagger.csproj -- stats
```

## Sample Output

```
=== File Tagger Statistics ===
Total tagged files: 15
Files with multiple tags: 8
Total unique tags: 12
Total tag assignments: 35

Top tags:
  #work                 8 file(s)
  #important            5 file(s)
  #vacation             4 file(s)
  #2025                 3 file(s)
```

## Data Storage

Tags are stored in `filetags.json` in the current directory. Each entry contains:
- `FilePath`: Absolute path to the file
- `Tags`: Array of tag strings

## Concepts Demonstrated

- JSON serialization with System.Text.Json
- File system path handling
- Sidecar file pattern for metadata
- LINQ for filtering and searching
- Set operations on collections
- Command-line argument parsing
- Statistics aggregation
