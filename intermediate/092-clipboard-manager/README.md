# Clipboard Manager

CLI utility for clipboard operations with history tracking. Copy, paste, save history, and analyze clipboard content from the command line.

## Usage

```bash
dotnet run --project ClipboardManager.csproj <command> [options]
```

## Commands

| Command | Description |
|---------|-------------|
| `copy`, `c` | Copy text to clipboard |
| `paste`, `p` | Paste clipboard content to stdout |
| `clear` | Clear clipboard history |
| `history`, `h` | Show clipboard history |
| `save` | Save current clipboard to history |
| `load` | Load last item from history |
| `count` | Count characters/words/lines |

## Examples

```bash
# Copy text directly
dotnet run --project ClipboardManager.csproj copy "Hello World"

# Copy from stdin (pipe)
echo "Sample text" | dotnet run --project ClipboardManager.csproj copy

# Paste to stdout
dotnet run --project ClipboardManager.csproj paste

# View history
dotnet run --project ClipboardManager.csproj history

# Save current clipboard to history
dotnet run --project ClipboardManager.csproj save

# Get clipboard statistics
dotnet run --project ClipboardManager.csproj count
```

## Example Output

```
$ dotnet run --project ClipboardManager.csproj copy "Hello World"
✓ Copied 11 characters to clipboard

$ dotnet run --project ClipboardManager.csproj history
Clipboard History (3 items):

[1] First copied text
[2] Second item with longer content here...
[3] Hello World

$ dotnet run --project ClipboardManager.csproj count
Clipboard Statistics:
  Characters: 11
  Words: 2
  Lines: 1
```

## Concepts Demonstrated

- External NuGet package integration (TextCopy)
- Async/await for clipboard operations
- File I/O for history persistence
- JSON serialization for data storage
- Command-line argument parsing
- Text analysis and statistics
- Error handling for platform-specific limitations

## Notes

- Clipboard operations require a GUI environment on some systems (X11, Windows)
- History is stored in `clipboard_history.json` in the current directory
- History is limited to the last 50 items
