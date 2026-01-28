# Bookmark Manager CLI

Save and manage your bookmarks from the command line. Supports tagging, searching, import/export, and browser integration.

## Usage

```bash
dotnet run --project BookmarkManager.csproj <command> [options]
```

## Commands

| Command | Aliases | Description |
|---------|---------|-------------|
| `add` | `a` | Add a new bookmark |
| `list` | `l` | List all bookmarks |
| `remove` | `rm`, `d`, `delete` | Remove a bookmark by ID |
| `search` | `s` | Search bookmarks |
| `tags` | `t` | Show all tags |
| `export` | `e` | Export to JSON/HTML |
| `import` | `i` | Import from JSON |
| `open` | `o` | Open in browser |

## Examples

### Add Bookmarks

```bash
# Add with URL only (title fetched automatically)
dotnet run --project BookmarkManager.csproj add https://github.com

# Add with custom title
dotnet run --project BookmarkManager.csproj add https://github.com "GitHub"

# Add with tags
dotnet run --project BookmarkManager.csproj add https://dotnet.com "Microsoft .NET" --tags dotnet,tech,development
```

### List Bookmarks

```bash
# List all bookmarks
dotnet run --project BookmarkManager.csproj list

# Filter by tag
dotnet run --project BookmarkManager.csproj list dotnet
```

**Output:**
```
Bookmarks (5 items):

[1] GitHub
    https://github.com
    Tags: dev, code
    Added: 2024-03-15

[2] Microsoft .NET
    https://dotnet.com
    Tags: dotnet, tech, development
    Added: 2024-03-16
```

### Search Bookmarks

```bash
dotnet run --project BookmarkManager.csproj search github
```

**Output:**
```
Search results for 'github' (2 found):

[1] GitHub
    https://github.com
[5] GitHub Actions Docs
    https://docs.github.com/actions
```

### View Tags

```bash
dotnet run --project BookmarkManager.csproj tags
```

**Output:**
```
Tags (8 unique):

  dotnet              ████████████████████ (5)
  tech                ████████████ (3)
  development         ████████ (2)
  code                ████ (1)
```

### Export/Import

```bash
# Export to JSON
dotnet run --project BookmarkManager.csproj export bookmarks.json

# Export to HTML (browser-compatible)
dotnet run --project BookmarkManager.csproj export bookmarks.html

# Import from JSON
dotnet run --project BookmarkManager.csproj import bookmarks.json
```

### Open Bookmark

```bash
# Open bookmark ID 1 in default browser
dotnet run --project BookmarkManager.csproj open 1
```

## Data Storage

Bookmarks are stored in `bookmarks.json` in the current directory:

```json
[
  {
    "Id": 1,
    "Url": "https://github.com",
    "Title": "GitHub",
    "Tags": ["dev", "code"],
    "CreatedAt": "2024-03-15T10:30:00Z"
  }
]
```

## Concepts Demonstrated

- JSON serialization with System.Text.Json
- File I/O for data persistence
- HTTP client for fetching page titles
- Regular expressions for HTML parsing
- Command-line argument parsing
- Tag-based filtering and search
- HTML export (Netscape bookmark format)
- Process launching for browser integration
- LINQ for data manipulation
- Async/await for I/O operations

## HTML Export Format

The HTML export uses the standard Netscape bookmark format, compatible with:
- Chrome
- Firefox
- Edge
- Safari
- Most other browsers
