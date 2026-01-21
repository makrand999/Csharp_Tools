# BulkFileRenamer

Bulk file renaming utility with pattern support including prefix, suffix, sequential numbering, find/replace, and regex-based renaming.

## Usage

```bash
dotnet run --project BulkFileRenamer.csproj [OPTIONS] <directory>
```

## Options

| Option | Description |
|--------|-------------|
| `--prefix <text>` | Add prefix to filenames |
| `--suffix <text>` | Add suffix to filenames (before extension) |
| `--number` | Add sequential numbering (001, 002, ...) |
| `--number-start <n>` | Start numbering from n (default: 1) |
| `--find <old>` | Find and replace text in filename |
| `--replace <new>` | Replacement text for --find |
| `--regex <pattern>` | Regex pattern to remove from filename |
| `--extension <ext>` | Filter by file extension (e.g., .jpg, .txt) |
| `--dry-run` | Show what would be renamed without actually renaming |

## Examples

```bash
# Add prefix to all files
dotnet run --project BulkFileRenamer.csproj --prefix "IMG_" ./photos

# Add numbering and suffix
dotnet run --project BulkFileRenamer.csproj --number --suffix "_backup" ./docs

# Find and replace text in filenames
dotnet run --project BulkFileRenamer.csproj --find "old" --replace "new" ./files

# Remove numbers from filenames (dry run)
dotnet run --project BulkFileRenamer.csproj --regex "\d+" --dry-run ./mixed

# Rename only .txt files with prefix and numbering
dotnet run --project BulkFileRenamer.csproj --prefix "doc_" --number --extension .txt ./notes
```

## Example Output

```
Found 5 file(s) to rename:

[DRY-RUN] photo1.jpg -> IMG_photo1_001.jpg
[DRY-RUN] photo2.jpg -> IMG_photo2_002.jpg
[DRY-RUN] photo3.jpg -> IMG_photo3_003.jpg

Dry run complete.
```

## Concepts Demonstrated

- File system operations (Directory, File, Path)
- Command-line argument parsing
- String manipulation and regex
- LINQ filtering and ordering
- Dry-run pattern for safe operations
- Conflict handling for duplicate filenames
