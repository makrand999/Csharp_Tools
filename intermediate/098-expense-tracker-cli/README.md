# Expense Tracker CLI

A CLI tool to track personal expenses with categories, generate monthly reports, and view spending summaries.

## Usage

```bash
dotnet run --project ExpenseTrackerCli.csproj <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `add <amount> <category> <description>` | Add a new expense |
| `list [category]` | List all expenses or filter by category |
| `delete <id>` | Remove an expense |
| `report <month>` | Show monthly report (YYYY-MM format) |
| `summary` | Show overall summary |

## Categories

Standard categories: `food`, `transport`, `utilities`, `entertainment`, `shopping`, `health`, `other`

## Examples

```bash
# Add expenses
dotnet run -- add 25.50 food "Lunch at restaurant"
dotnet run -- add 50.00 transport "Gas station"
dotnet run -- add 120.00 utilities "Electric bill"

# List all expenses
dotnet run -- list

# List expenses in a specific category
dotnet run -- list food

# View monthly report
dotnet run -- report 2026-03

# View overall summary
dotnet run -- summary

# Delete an expense
dotnet run -- delete 1
```

## Example Session

```
$ dotnet run -- add 25.50 food "Lunch at restaurant"
✓ Added: $25.50 - food - Lunch at restaurant
  ID: 1 | Date: 2026-03-31

$ dotnet run -- add 50.00 transport "Gas"
✓ Added: $50.00 - transport - Gas
  ID: 2 | Date: 2026-03-31

$ dotnet run -- summary

═══════════════════════════════════════════
 Expense Summary
═══════════════════════════════════════════

  Period:          2026-03-31 to 2026-03-31 (1 days)
  Total expenses:  2
  Total spent:     $75.50
  Daily average:   $75.50
  Monthly average: $2265.00

  By Category:
  ─────────────────────────────────────
  transport      $  50.00   66.2% █████████████
  food           $  25.50   33.8% ███████
```

## Concepts Demonstrated

- JSON serialization with `System.Text.Json`
- File I/O for data persistence
- Command-line argument parsing
- LINQ grouping and aggregation
- Decimal arithmetic for financial calculations
- Date/time manipulation
- Data visualization with ASCII charts
