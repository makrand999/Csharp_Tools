# TempFileCleaner

Finds and removes temporary files based on common patterns (.tmp, .bak, .log, .cache, etc.) and temporary directory names.

## Usage

```bash
dotnet run --project TempFileCleaner.csproj [path] [options]
```

## Options

| Option | Description |
|--------|-------------|
| `--dry-run`, `-n` | Show temp files without deleting |
| `--verbose`, `-v` | Show detailed deletion progress |
| `--include-cache-dirs` | Also include files in cache/temp directories |

## Detected Patterns

**File extensions:** `.tmp`, `.temp`, `.bak`, `.old`, `.swp`, `.swo`, `.log`, `.cache`

**Directory names:** `tmp`, `temp`, `cache`, `.cache`

## Examples

```bash
# Clean current directory
dotnet run --project TempFileCleaner.csproj

# Clean specific directory
dotnet run --project TempFileCleaner.csproj /home/user/projects

# Preview what would be deleted
dotnet run --project TempFileCleaner.csproj ./src --dry-run

# Include cache directories
dotnet run --project TempFileCleaner.csproj /var/log --include-cache-dirs --verbose
```

## Example Output

```
Scanning for temporary files in: /home/user/projects

Found 15 temporary files (25.50 MB):

  /home/user/projects/bin/Debug/app.pdb (12.00 MB)
  /home/user/projects/obj/project.assets.json (8.50 MB)
  /home/user/projects/logs/build.log (3.00 MB)
  ...

Delete these files? (y/N): y

Deleted 15 files.
Freed 25.50 MB of disk space.
```

## Concepts Demonstrated

- File pattern matching with wildcards
- Recursive directory scanning
- File size calculation
- Interactive confirmation
- Duplicate removal with LINQ
- Safe file deletion with error handling
- Dry-run pattern for preview
