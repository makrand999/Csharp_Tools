# LargeFileFinder

Finds files larger than a specified size threshold and displays them in a sorted table with statistics.

## Usage

```bash
dotnet run --project LargeFileFinder.csproj [path] [options]
```

## Options

| Option | Description | Default |
|--------|-------------|---------|
| `--size <MB>` | Minimum file size in megabytes | 10 |
| `--top <N>` | Limit results to top N files | 50 |
| `--json` | Output results as JSON | false |

## Examples

```bash
# Find files larger than 10MB in current directory
dotnet run --project LargeFileFinder.csproj

# Find files larger than 100MB
dotnet run --project LargeFileFinder.csproj /home/user --size 100

# Find top 20 largest files over 50MB
dotnet run --project LargeFileFinder.csproj /var --size 50 --top 20

# Output as JSON for scripting
dotnet run --project LargeFileFinder.csproj ./projects --size 5 --json
```

## Example Output

```
Found 25 files larger than 10.00 MB:

Size         Extension  Modified             Path
--------------------------------------------------------------------------------
520.00 MB    .iso       2024-01-15 10:30:00  /home/user/downloads/ubuntu.iso
245.50 MB    .zip       2024-02-20 14:22:00  /home/user/backups/archive.zip
128.00 MB    .mp4       2024-03-01 09:15:00  /home/user/videos/recording.mp4
...

Total: 25 files, 2.15 GB

By extension:
  .iso              2 files, 1.04 GB
  .zip              5 files, 512.00 MB
  .mp4              8 files, 350.00 MB
  .dll             10 files, 256.00 MB
```

## JSON Output

```json
{
  "totalFiles": 25,
  "totalSize": 2306867200,
  "files": [
    {
      "path": "/home/user/downloads/ubuntu.iso",
      "size": 545259520,
      "sizeFormatted": "520.00 MB",
      "extension": ".iso",
      "modified": "2024-01-15T10:30:00"
    }
  ]
}
```

## Concepts Demonstrated

- Recursive file system traversal
- File size filtering and sorting
- Formatted table output
- JSON serialization without libraries
- LINQ grouping and aggregation
- Command-line argument parsing
- Extension-based analysis
