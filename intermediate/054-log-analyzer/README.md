# Log Analyzer

A practical command-line tool for parsing, analyzing, and extracting insights from application log files.

## Usage

```bash
dotnet run --project LogAnalyzer.csproj [command] [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `parse <logfile>` | Parse and display log entries |
| `errors <logfile>` | Extract all ERROR and FATAL entries |
| `stats <logfile>` | Show log statistics and level distribution |
| `filter <logfile> <level>` | Filter entries by log level |
| `search <logfile> <term>` | Search for text in log messages |
| `timeline <logfile>` | Show hourly activity timeline |

## Log Format

The analyzer expects logs in this format:
```
[2024-01-15 10:30:45] [INFO] Application started successfully
[2024-01-15 10:31:02] [ERROR] Database connection failed
[2024-01-15 10:31:15] [WARN] Retrying connection...
```

## Examples

```bash
# Show all parsed entries
dotnet run --project LogAnalyzer.csproj parse app.log

# Extract errors only
dotnet run --project LogAnalyzer.csproj errors app.log

# Show statistics
dotnet run --project LogAnalyzer.csproj stats app.log

# Filter by level
dotnet run --project LogAnalyzer.csproj filter app.log WARN

# Search for specific text
dotnet run --project LogAnalyzer.csproj search app.log "database"

# Show activity timeline
dotnet run --project LogAnalyzer.csproj timeline app.log
```

## Sample Output

```
$ dotnet run --project LogAnalyzer.csproj stats app.log
Log Statistics for 'app.log':
  Total entries: 1250
  Time range: 2024-01-15 08:00:00 to 2024-01-15 18:30:00

  Entries by level:
    INFO         850 ( 68.0%)
    WARN         250 ( 20.0%)
    ERROR        125 ( 10.0%)
    FATAL         25 (  2.0%)

  Error rate: 12.00%
  Warning rate: 20.00%
```

## Concepts Demonstrated

- File I/O with `File.ReadLines()` for efficient streaming
- Regular expressions for log parsing
- DateTime parsing and manipulation
- LINQ grouping and aggregation
- Dictionary-based counting
- Text visualization (ASCII bar charts)
- Custom class design for data modeling
- Percentage calculations and statistics
