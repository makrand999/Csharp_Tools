# Code Snippet Manager

A CLI tool to store, organize, and retrieve code snippets with tags. Snippets are persisted in a JSON file for easy backup and portability.

## Usage

```bash
dotnet run --project CodeSnippetManager.csproj <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `add <title> <language> [tags...]` | Add a new snippet (reads code from stdin) |
| `list [tag]` | List all snippets or filter by tag |
| `search <query>` | Search snippets by title, code, or tags |
| `delete <id>` | Delete a snippet by ID |
| `export [file]` | Export snippets to a JSON file |

## Examples

```bash
# Add a snippet (paste code, then press Enter twice)
echo "console.log('Hello World')" | dotnet run -- add "Hello World" javascript js example

# List all snippets
dotnet run -- list

# List snippets with a specific tag
dotnet run -- list js

# Search for snippets
dotnet run -- search "hello"

# Delete a snippet
dotnet run -- delete 1

# Export all snippets to a backup file
dotnet run -- export backup.json
```

## Example Session

```
$ dotnet run -- add "Python Hello" python py
Paste your code (end with empty line or Ctrl+D):
print("Hello, World!")

✓ Snippet #1 added: "Python Hello" (python)
  Tags: py

$ dotnet run -- list
Found 1 snippet(s):

[1] Python Hello (python)
    Tags: py
    Created: 2026-03-31 10:30
    Code: print("Hello, World!")...
```

## Concepts Demonstrated

- JSON serialization with `System.Text.Json`
- File I/O for data persistence
- Command-line argument parsing
- LINQ for filtering and searching
- Object-oriented design with classes
- Collection manipulation (List<T>)
