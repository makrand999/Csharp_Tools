# Markdown Table Generator

A CLI tool for generating markdown tables from CSV, JSON, or interactive input.

## Usage

```bash
dotnet run --project MarkdownTable.csproj [options]
```

## Options

| Option | Description |
|--------|-------------|
| `--from-csv <file>` | Convert CSV file to markdown table |
| `--from-json <file>` | Convert JSON array to markdown table |
| `--interactive` | Interactive table builder |
| `--demo` | Generate sample tables |

## Examples

```bash
# Convert CSV to markdown
dotnet run --project MarkdownTable.csproj --from-csv data.csv

# Convert JSON to markdown
dotnet run --project MarkdownTable.csproj --from-json users.json

# Interactive mode
dotnet run --project MarkdownTable.csproj --interactive

# See demo output
dotnet run --project MarkdownTable.csproj --demo
```

## Sample Output

```
Example 1 - Simple Table:
| Name      | Age | City        |
|:----------|:----|:------------|
| Alice     | 30  | New York    |
| Bob       | 25  | Los Angeles |
| Charlie   | 35  | Chicago     |

Example 2 - Product List:
| Product | Price | Qty |   Total   |
|:--------|------:|:---:|----------:|
| Widget  |  $9.99 | 10  |    $99.90 |
| Gadget  | $24.99 |  5  |   $124.95 |
| Gizmo   | $14.99 |  8  |   $119.92 |
```

## Input Formats

### CSV Input
```csv
Name,Age,City
Alice,30,New York
Bob,25,Los Angeles
```

### JSON Input
```json
[
  {"Name": "Alice", "Age": "30", "City": "New York"},
  {"Name": "Bob", "Age": "25", "City": "Los Angeles"}
]
```

## Concepts Demonstrated

- File I/O (reading CSV and JSON)
- String parsing and manipulation
- List of lists for 2D data
- Enum for alignment options
- Text formatting and padding
- Command-line argument handling
- Interactive console input
