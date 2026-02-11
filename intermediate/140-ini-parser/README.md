# INI File Parser

Parse and manipulate INI configuration files.

## Usage

```bash
dotnet run --project 140-ini-parser.csproj -- <file.ini>
```

## Example

```bash
dotnet run --project 140-ini-parser.csproj -- config.ini
dotnet run --project 140-ini-parser.csproj -- config.ini --section Database
dotnet run --project 140-ini-parser.csproj -- config.ini --key ConnectionString
dotnet run --project 140-ini-parser.csproj -- config.ini --json
```

## Options

- `--section NAME` - Show specific section
- `--key KEY` - Get specific key value
- `--json` - Output as JSON

## Sample Output

```ini
[Database]
Host = localhost
Port = 5432
Name = mydb

[Logging]
Level = Debug
File = app.log
```

## Concepts Demonstrated

- INI file parsing
- Section handling
- Key-value extraction
- JSON conversion
- Case-insensitive dictionaries
