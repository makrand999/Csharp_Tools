# ClipboardHistory

Enhanced clipboard manager with history tracking, search functionality, and named snippets. Stores clipboard entries in JSON for persistence across sessions.

## Usage

```bash
dotnet run --project ClipboardHistory.csproj -- <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `copy [text]` | Copy text to clipboard |
| `paste` | Output the latest clipboard item |
| `history [limit]` | Show clipboard history |
| `get <id>` | Get specific history item |
| `search <query>` | Search clipboard history |
| `delete <id>` | Delete a history item |
| `clear` | Clear all history |
| `snippets <sub>` | Manage text snippets |

## Snippet Subcommands

| Subcommand | Description |
|------------|-------------|
| `save <name> <id>` | Save history item as named snippet |
| `list` | List all snippets |
| `get <name>` | Get snippet content |
| `delete <name>` | Delete a snippet |

## Examples

```bash
# Copy text directly
dotnet run --project ClipboardHistory.csproj -- copy "Hello, World!"

# Copy from stdin (pipe)
echo "test content" | dotnet run --project ClipboardHistory.csproj -- copy

# Paste latest item
dotnet run --project ClipboardHistory.csproj -- paste

# View history (last 10 items by default)
dotnet run --project ClipboardHistory.csproj -- history
dotnet run --project ClipboardHistory.csproj -- history 20

# Get specific item
dotnet run --project ClipboardHistory.csproj -- get 5

# Search history
dotnet run --project ClipboardHistory.csproj -- search "https://"
dotnet run --project ClipboardHistory.csproj -- search "TODO"

# Save frequently used text as snippet
dotnet run --project ClipboardHistory.csproj -- copy "Best regards,\nJohn"
dotnet run --project ClipboardHistory.csproj -- snippets save signature 1

# Use saved snippet
dotnet run --project ClipboardHistory.csproj -- snippets get signature

# List all snippets
dotnet run --project ClipboardHistory.csproj -- snippets list

# Delete item or snippet
dotnet run --project ClipboardHistory.csproj -- delete 3
dotnet run --project ClipboardHistory.csproj -- snippets delete signature
```

## Sample Output

```
ID     Type       Age          Preview
----------------------------------------------------------------------
1      short      2m ago       Hello, World!
2      url        5m ago       https://example.com/api/endpoint
3      multiline  1h ago       function test() {\n  return true;\n}
4      email      2h ago       user@example.com
```

## Data Files

- `clipboard.json` - Clipboard history (max 50 items)
- `snippets.json` - Named text snippets

## Content Type Detection

The tool automatically detects content types:
- **url** - Starts with http:// or https://
- **email** - Contains @ and . characters
- **multiline** - Contains newlines
- **text** - Longer than 50 characters
- **short** - Short text snippets

## Concepts Demonstrated

- JSON serialization with System.Text.Json
- Dictionary and List collections
- File I/O for data persistence
- LINQ for searching and filtering
- String manipulation and detection
- Time formatting (relative timestamps)
- stdin/stdout handling for piping
- Nested command structures
