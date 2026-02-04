# CsvMerger

Merge multiple CSV files with schema handling. Supports vertical (append rows) and horizontal (append columns) merging, splitting, and column selection.

## Usage

```bash
dotnet run --project CsvMerger.csproj -- merge <file1.csv> <file2.csv> [options]
dotnet run --project CsvMerger.csproj -- info <file.csv>
dotnet run --project CsvMerger.csproj -- split <file.csv> <rows-per-file>
dotnet run --project CsvMerger.csproj -- select <file.csv> <columns>
```

## Commands

| Command | Description |
|---------|-------------|
| `merge` | Merge multiple CSV files |
| `info` | Show CSV file information |
| `split` | Split CSV into smaller files |
| `select` | Extract specific columns |

## Merge Options

| Option | Description |
|--------|-------------|
| `--output <file>` | Output file (default: merged.csv) |
| `--mode <mode>` | `vertical` (append rows) or `horizontal` (append columns) |
| `--delimiter <char>` | CSV delimiter (default: `,`) |

## Examples

### Merge Files (Vertical - Append Rows)

```bash
# Merge files with same schema
dotnet run --project CsvMerger.csproj -- merge data1.csv data2.csv data3.csv

# Specify output file
dotnet run --project CsvMerger.csproj -- merge q1.csv q2.csv -o half-year.csv

# Horizontal merge (combine columns)
dotnet run --project CsvMerger.csproj -- merge names.csv scores.csv --mode horizontal
```

### Show CSV Info

```bash
dotnet run --project CsvMerger.csproj -- info data.csv
```

**Sample Output:**
```
CSV Information: data.csv
--------------------------------------------------
File size: 15,234 bytes
Columns: 5
Rows: 1,000

Columns:
  1. id
  2. name
  3. email
  4. phone
  5. city

Sample (first data row):
  id: 1
  name: John Doe
  email: john@example.com
```

### Split Large Files

```bash
# Split into files of 1000 rows each
dotnet run --project CsvMerger.csproj -- split large.csv 1000

# Custom output pattern
dotnet run --project CsvMerger.csproj -- split large.csv 500 --output "part_{n}.csv"
```

### Select Columns

```bash
# Extract specific columns
dotnet run --project CsvMerger.csproj -- select data.csv name,email,phone

# Save to new file
dotnet run --project CsvMerger.csproj -- select data.csv name,email --output contacts.csv
```

## Features

- **Automatic header detection**: Handles mismatched headers gracefully
- **Quoted field support**: Properly parses fields with commas and quotes
- **Large file handling**: Stream-based processing for memory efficiency
- **Flexible output**: Custom delimiters and file patterns

## Concepts Demonstrated

- CSV parsing without external libraries
- Quote escaping and field handling
- File streaming for large datasets
- Schema validation and comparison
- StringBuilder for efficient string construction
- Command-line option parsing
