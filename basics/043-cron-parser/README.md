# Cron Expression Parser

A CLI tool that parses and explains cron expressions, showing when scheduled tasks will run.

## Usage

```bash
dotnet run --project CronParser.csproj "<cron-expression>"
```

## Examples

```bash
# Parse a cron expression
dotnet run --project CronParser.csproj "*/15 * * * *"

# With arguments for each field
dotnet run --project CronParser.csproj 30 2 * * 1-5

# Complex expression with ranges and steps
dotnet run --project CronParser.csproj "0 9-17 * * mon-fri"
```

## Sample Output

```
Cron Expression: */15 * * * *

Parsed Fields:
  Minutes:  0, 15, 30, 45
  Hours:    * (every hour)
  Days:     * (every day)
  Months:   * (every month)
  Weekdays: Sun, Mon, Tue, Wed, Thu, Fri, Sat

Next 5 occurrences:
  2024-01-15 10:15 (Monday)
  2024-01-15 10:30 (Monday)
  2024-01-15 10:45 (Monday)
  2024-01-15 11:00 (Monday)
  2024-01-15 11:15 (Monday)
```

## Concepts Demonstrated

- String parsing and splitting
- HashSet for unique value collections
- DateTime manipulation
- Command-line argument handling
- Pattern matching for cron syntax (*, /, -, ranges)
- Enumerable operations (LINQ)
