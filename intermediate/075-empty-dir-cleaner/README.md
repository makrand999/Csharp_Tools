# EmptyDirCleaner

Finds and removes empty directories recursively. Includes dry-run mode and interactive confirmation.

## Usage

```bash
dotnet run --project EmptyDirCleaner.csproj [path] [options]
```

## Options

| Option | Description |
|--------|-------------|
| `--dry-run`, `-n` | Show empty directories without deleting |
| `--verbose`, `-v` | Show detailed deletion progress |

## Examples

```bash
# Clean current directory (interactive)
dotnet run --project EmptyDirCleaner.csproj

# Clean specific directory
dotnet run --project EmptyDirCleaner.csproj /home/user/projects

# Preview what would be deleted
dotnet run --project EmptyDirCleaner.csproj ./src --dry-run

# Verbose deletion
dotnet run --project EmptyDirCleaner.csproj /tmp --verbose
```

## Example Output

```
Found 5 empty directories:

  /home/user/projects/old-module/tests
  /home/user/projects/old-module/docs
  /home/user/projects/old-module
  /home/user/projects/temp/cache
  /home/user/projects/temp

Delete these directories? (y/N): y

Deleted 5 directories.
```

## Concepts Demonstrated

- Recursive directory traversal
- Depth-first search algorithm
- File system enumeration
- Interactive user input
- Safe deletion with confirmation
- Dry-run pattern
- Exception handling for permissions
