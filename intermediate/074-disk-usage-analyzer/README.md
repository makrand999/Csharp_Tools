# DiskUsageAnalyzer

Disk usage analyzer that displays directory sizes in a tree view with visual bar charts showing the largest files and folders.

## Usage

```bash
dotnet run --project DiskUsageAnalyzer.csproj [path] [maxDepth] [topCount]
```

## Arguments

| Argument | Description | Default |
|----------|-------------|---------|
| `path` | Directory to analyze | Current directory |
| `maxDepth` | Maximum depth to traverse | 3 |
| `topCount` | Number of top items to display | 10 |

## Examples

```bash
# Analyze current directory
dotnet run --project DiskUsageAnalyzer.csproj

# Analyze specific directory
dotnet run --project DiskUsageAnalyzer.csproj /home/user/downloads

# Analyze with custom depth and top items
dotnet run --project DiskUsageAnalyzer.csproj ./projects 5 20
```

## Example Output

```
Analyzing: /home/user/projects

Total Size:  1.25 GB
Total Files: 1523
Total Directories: 87

Top 10 largest items:

 1.  450.00 MB |██████████████████████████                          | /home/user/projects/bin
 2.  320.50 MB |███████████████████                                 | /home/user/projects/obj
 3.  125.75 MB |███████                                             | /home/user/projects/assets
 4.   85.20 MB |█████                                               | /home/user/projects/data
 ...

Directory Tree:
└─ [ 1.25 GB] (100.0%) projects/
   ├─ [450.00 MB] (35.2%) bin/
   ├─ [320.50 MB] (25.0%) obj/
   └─ [484.50 MB] (39.8%) src/
```

## Concepts Demonstrated

- Recursive directory traversal
- File system enumeration
- Size calculation and formatting
- Tree visualization with ASCII art
- LINQ sorting and filtering
- Exception handling for inaccessible paths
- Percentage calculations with visual bars
