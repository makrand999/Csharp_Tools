# CsvToJson

Converts CSV files to JSON format with configurable delimiters and header options.

## Usage

```bash
dotnet run --project CsvToJson.csproj <input.csv> [options]
```

## Options

| Option | Description | Default |
|--------|-------------|---------|
| `-o, --output <file>` | Output file | stdout |
| `-d, --delimiter <char>` | Field delimiter | `,` |
| `--no-headers` | First row is data (auto-generates column names) | false |
| `--pretty` | Pretty-print JSON output | false |

## Examples

```bash
# Convert CSV to JSON (output to stdout)
dotnet run --project CsvToJson.csproj data.csv

# Convert and save to file
dotnet run --project CsvToJson.csproj data.csv -o output.json

# Pretty-print output
dotnet run --project CsvToJson.csproj data.csv --pretty

# Semicolon-delimited file without headers
dotnet run --project CsvToJson.csproj data.csv -d ";" --no-headers
```

## Example Input

```csv
name,age,city
Alice,30,New York
Bob,25,London
Charlie,35,Tokyo
```

## Example Output

```json
[{"name":"Alice","age":"30","city":"New York"},{"name":"Bob","age":"25","city":"London"},{"name":"Charlie","age":"35","city":"Tokyo"}]
```

### Pretty Output

```json
[
  {
    "name": "Alice",
    "age": "30",
    "city": "New York"
  },
  {
    "name": "Bob",
    "age": "25",
    "city": "London"
  },
  {
    "name": "Charlie",
    "age": "35",
    "city": "Tokyo"
  }
]
```

## CSV Parsing Features

- Quoted fields with embedded delimiters
- Escaped quotes (`""` within quoted fields)
- Custom delimiters (semicolon, tab, pipe, etc.)
- Header row detection

## Concepts Demonstrated

- CSV parsing with quote handling
- JSON serialization with System.Text.Json
- StringBuilder for efficient string building
- Custom delimiter support
- Command-line argument parsing
- Stream-based file I/O
- Dictionary-based record representation
