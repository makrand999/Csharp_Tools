# Clipboard Manager

A practical clipboard history manager that captures, stores, and retrieves clipboard entries. Useful for developers who need to track copied items and quickly access history.

## Usage

```bash
dotnet run --project ClipboardManager.csproj -- <command> [args]
```

## Commands

| Command | Description |
|---------|-------------|
| `add <text>` | Add text to clipboard history |
| `list [n]` | List last n entries (default: 10) |
| `get <index>` | Get entry at index |
| `copy <index>` | Copy entry to stdout for piping |
| `search <q>` | Search history for query |
| `clear` | Clear all history |
| `export <file>` | Export history to file |
| `help` | Show help |

## Examples

**Add text from command line:**
```bash
dotnet run --project ClipboardManager.csproj -- add "important API key: abc123"
```

**Add text from stdin (pipe):**
```bash
echo "hello world" | dotnet run --project ClipboardManager.csproj -- add
cat file.txt | dotnet run --project ClipboardManager.csproj -- add
```

**List recent entries:**
```bash
dotnet run --project ClipboardManager.csproj -- list
dotnet run --project ClipboardManager.csproj -- list 20
```

**Get specific entry:**
```bash
dotnet run --project ClipboardManager.csproj -- get 3
```

**Search history:**
```bash
dotnet run --project ClipboardManager.csproj -- search "api"
dotnet run --project ClipboardManager.csproj -- search "TODO"
```

**Export history:**
```bash
dotnet run --project ClipboardManager.csproj -- export backup.txt
```

**Clear history:**
```bash
dotnet run --project ClipboardManager.csproj -- clear
```

## Example Output

```
Clipboard History (showing 5 of 12 entries):
------------------------------------------------------------
[  0] 14:32:15   156 chars | echo "hello world" | dotnet run...
[  1] 14:30:22    45 chars | important API key: abc123
[  2] 14:28:10   234 chars | SELECT * FROM users WHERE id = ...
[  3] 14:25:00    89 chars | https://github.com/user/repo/p...
[  4] 14:20:15    32 chars | git commit -m "fix: bug"
```

## Concepts Demonstrated

- JSON serialization with `System.Text.Json`
- File-based persistence
- Command-line argument parsing
- List manipulation and indexing
- LINQ for searching and filtering
- Console output formatting with colors
- stdin/stdout handling for piping
- Data structures (List, custom classes)
