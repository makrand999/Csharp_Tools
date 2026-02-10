# Log File Parser

Parse and analyze log files with filtering capabilities.

## Usage

```bash
dotnet run --project 139-log-parser.csproj -- <logfile.log>
```

## Example

```bash
dotnet run --project 139-log-parser.csproj -- app.log --level ERROR
dotnet run --project 139-log-parser.csproj -- app.log --pattern "exception" --stats
dotnet run --project 139-log-parser.csproj -- app.log --from 2024-01-01 --to 2024-01-31
```

## Options

- `--level LEVEL` - Filter by level (ERROR, WARN, INFO, DEBUG)
- `--pattern REGEX` - Filter by pattern
- `--from DATE` - Filter from date (yyyy-MM-dd)
- `--to DATE` - Filter to date (yyyy-MM-dd)
- `--stats` - Show statistics only

## Supported Log Formats

- `2024-01-15 10:30:45 [INFO] Message`
- `[2024-01-15 10:30:45] INFO: Message`
- `INFO 2024-01-15 10:30:45 - Message`

## Concepts Demonstrated

- Regular expressions
- Pattern matching
- Date/time filtering
- LINQ grouping
- Statistics calculation
