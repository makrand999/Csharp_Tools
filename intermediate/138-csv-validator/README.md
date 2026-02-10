# CSV Validator

Validate CSV files and detect common issues.

## Usage

```bash
dotnet run --project 138-csv-validator.csproj -- <file.csv>
cat data.csv | dotnet run --project 138-csv-validator.csproj
```

## Example

```bash
dotnet run --project 138-csv-validator.csproj -- users.csv
dotnet run --project 138-csv-validator.csproj -- data.csv --delimiter ";"
dotnet run --project 138-csv-validator.csproj -- data.csv --has-header
```

## Options

- `--delimiter CHAR` - Set delimiter (default: comma)
- `--has-header` - First row is header

## Checks Performed

- Column count consistency
- Unclosed quoted fields
- Empty column names
- Proper escaping

## Concepts Demonstrated

- CSV parsing
- Quote handling
- Delimiter configuration
- Error reporting
- Statistics calculation
