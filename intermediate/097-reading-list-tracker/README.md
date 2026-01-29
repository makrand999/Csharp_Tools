# Reading List Tracker

A CLI tool to track books and articles you want to read, are currently reading, or have completed. Includes support for tags, notes, and reading statistics.

## Usage

```bash
dotnet run --project ReadingListTracker.csproj <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `add <title> <author> [tags...]` | Add a new book/article |
| `list [status]` | List all or filter by status |
| `status <id> <status>` | Update reading status |
| `note <id> <text>` | Add a note to a book |
| `delete <id>` | Remove a book from the list |
| `stats` | Show reading statistics |

## Statuses

- `to-read` - Books you plan to read
- `reading` - Currently reading
- `completed` - Finished books
- `abandoned` - Books you stopped reading

## Examples

```bash
# Add books to your list
dotnet run -- add "Clean Code" "Robert Martin" programming classic
dotnet run -- add "The Pragmatic Programmer" "Andrew Hunt" programming

# List all books
dotnet run -- list

# List only books you're currently reading
dotnet run -- list reading

# Update status when you finish a book
dotnet run -- status 1 completed

# Add a personal note
dotnet run -- note 1 "Excellent chapter on clean functions!"

# View your reading statistics
dotnet run -- stats
```

## Example Session

```
$ dotnet run -- add "Clean Code" "Robert Martin" programming
✓ Added: "Clean Code" by Robert Martin
  Status: to-read | ID: 1
  Tags: programming

$ dotnet run -- list

📚 TO-READ
────────────────────────────────────────
[1] Clean Code by Robert Martin
    Tags: programming
    Added: 2026-03-31

$ dotnet run -- status 1 reading
✓ Updated "Clean Code" to reading

$ dotnet run -- stats

📊 Reading Statistics
═══════════════════════════════════
  Total books:     1
  To read:         0
  Currently read:  1
  Completed:       0
  Abandoned:       0
```

## Concepts Demonstrated

- JSON serialization with `System.Text.Json`
- File I/O for data persistence
- Command-line argument parsing
- LINQ for filtering and statistics
- Nullable types (`DateTime?`)
- Object-oriented design with classes
- Date/time calculations
