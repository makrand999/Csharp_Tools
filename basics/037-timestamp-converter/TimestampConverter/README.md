# Unix Timestamp Converter

A CLI tool for converting between Unix timestamps and human-readable dates.

## Usage

```bash
dotnet run --project TimestampConverter.csproj <command> [arguments]
```

## Commands

- `to-unix <date>` - Convert a date to Unix timestamp
- `from-unix <timestamp>` - Convert Unix timestamp to readable date
- `now` - Show current Unix timestamp

## Examples

```bash
# Convert a date to Unix timestamp
dotnet run --project TimestampConverter.csproj to-unix "2024-01-15 10:30:00"
# Output:
# Input:    01/15/2024 10:30:00
# UTC:      2024-01-15 10:30:00
# Timestamp: 1705312200

# Convert Unix timestamp to date
dotnet run --project TimestampConverter.csproj from-unix 1705312200
# Output:
# Timestamp: 1705312200
# UTC:       2024-01-15 10:30:00
# Local:     2024-01-15 11:30:00

# Get current timestamp
dotnet run --project TimestampConverter.csproj now
# Output:
# Current Unix Timestamp: 1705312200
# UTC Time:               2024-01-15 10:30:00
# Local Time:             2024-01-15 11:30:00
```

## Concepts Demonstrated

- Unix epoch time calculations
- DateTime parsing with multiple formats
- UTC and local time zone conversion
- Command-line subcommands pattern
- String array slicing (args[1..])
