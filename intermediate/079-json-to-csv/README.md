# JsonToCsv

Converts JSON arrays or objects to CSV format with configurable delimiters and header options.

## Usage

```bash
dotnet run --project JsonToCsv.csproj <input.json> [options]
```

## Options

| Option | Description | Default |
|--------|-------------|---------|
| `-o, --output <file>` | Output file | stdout |
| `-d, --delimiter <char>` | Field delimiter | `,` |
| `--no-headers` | Omit header row | false |

## Examples

```bash
# Convert JSON to CSV (output to stdout)
dotnet run --project JsonToCsv.csproj data.json

# Convert and save to file
dotnet run --project JsonToCsv.csproj data.json -o output.csv

# Use semicolon delimiter
dotnet run --project JsonToCsv.csproj data.json -d ";"

# No headers
dotnet run --project JsonToCsv.csproj data.json --no-headers

# Pipe to other tools
dotnet run --project JsonToCsv.csproj data.json | grep "search"
```

## Example Input

```json
[
  {"name": "Alice", "age": 30, "city": "New York"},
  {"name": "Bob", "age": 25, "city": "London"},
  {"name": "Charlie", "age": 35, "city": "Tokyo"}
]
```

## Example Output

```csv
name,age,city
Alice,30,New York
Bob,25,London
Charlie,35,Tokyo
```

## Nested Objects

Nested objects and arrays are represented as `[object]` and `[array]`:

```json
[
  {"id": 1, "user": {"name": "Alice"}, "tags": ["a", "b"]}
]
```

```csv
id,user,tags
1,[object],[array]
```

## Concepts Demonstrated

- JSON parsing with System.Text.Json
- JsonDocument and JsonElement traversal
- CSV escaping (RFC 4180)
- StringBuilder for efficient output
- Command-line argument parsing
- Stream-based file I/O
- Handling different JSON value types
