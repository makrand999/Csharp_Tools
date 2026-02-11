# TOML Parser

Parse and display TOML configuration files.

## Usage

```bash
dotnet run --project 141-toml-parser.csproj -- <file.toml>
```

## Example

```bash
dotnet run --project 141-toml-parser.csproj -- config.toml
dotnet run --project 141-toml-parser.csproj -- config.toml --section database
dotnet run --project 141-toml-parser.csproj -- config.toml --key host --json
```

## Options

- `--section NAME` - Show specific section
- `--key KEY` - Get specific key value
- `--json` - Output as JSON

## Concepts Demonstrated

- TOML format parsing
- Section handling
- Value type detection
- JSON conversion
- Configuration file processing
