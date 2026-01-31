# Quick Note

A fast CLI note-taking tool for capturing and organizing notes with tags, search, and export capabilities.

## Usage

```bash
dotnet run --project QuickNote.csproj -- <command> [args]
```

## Commands

| Command | Description |
|---------|-------------|
| `add [title] [#tags...]` | Add a new note |
| `list` | List all notes |
| `view <id>` | View note by ID |
| `delete <id>` | Delete a note |
| `search <query>` | Search notes by title or content |
| `tag <tag>` | Filter notes by tag |
| `export <id> [format]` | Export note (txt or md) |
| `stats` | Show statistics |
| `help` | Show help |

## Examples

**Add a note with title and tags:**
```bash
dotnet run --project QuickNote.csproj -- add "Meeting Notes" #work #meeting
```

**Add a quick note from stdin:**
```bash
echo "Remember to review PR #42" | dotnet run --project QuickNote.csproj -- add
```

**Add a note interactively:**
```bash
dotnet run --project QuickNote.csproj -- add "Project Ideas" #ideas
# Then type content, press Enter twice to finish
```

**List all notes:**
```bash
dotnet run --project QuickNote.csproj -- list
```

**View a specific note:**
```bash
dotnet run --project QuickNote.csproj -- view 5
```

**Search notes:**
```bash
dotnet run --project QuickNote.csproj -- search "TODO"
dotnet run --project QuickNote.csproj -- search "API endpoint"
```

**Filter by tag:**
```bash
dotnet run --project QuickNote.csproj -- tag work
dotnet run --project QuickNote.csproj -- tag #ideas
```

**Export a note:**
```bash
dotnet run --project QuickNote.csproj -- export 5 txt
dotnet run --project QuickNote.csproj -- export 3 md
```

**Show statistics:**
```bash
dotnet run --project QuickNote.csproj -- stats
```

## Example Output

**List command:**
```
Notes (12 total):

#12 API Design Notes [#work #api] Dec 15  Design RESTful endpoints for...
#11 Book Recommendations [#personal] Dec 14  The Pragmatic Programmer...
#10 Meeting Notes [#work #meeting] Dec 13  Discussed Q1 roadmap and...
```

**View command:**
```
# API Design Notes
Tags: #work #api
Created: 2024-12-15 14:30:00
Updated: 2024-12-15 14:30:00
--------------------------------------------------
Key points for REST API design:
- Use proper HTTP methods (GET, POST, PUT, DELETE)
- Version your APIs (/api/v1/)
- Return appropriate status codes
```

**Stats command:**
```
Note Statistics:
----------------------------------------
Total notes:     12
Total words:     2,450
Total chars:     15,234
Avg note size:   204 words
First note:      2024-11-01
Latest note:     2024-12-15

Top tags:
  #work: 8
  #api: 4
  #ideas: 3
  #personal: 2
```

## Concepts Demonstrated

- JSON serialization with `System.Text.Json`
- File-based persistence in user profile directory
- Command-line argument parsing with variadic args
- Tag-based organization and filtering
- Full-text search implementation
- Export functionality (TXT, Markdown)
- Statistics calculation with LINQ
- Interactive input handling
- DateTime manipulation and formatting
