# JsonFormatter

Format (pretty-print) or minify JSON data. Supports file input/output and stdin/stdout.

## Usage

```bash
# Format a JSON file
dotnet run --project JsonFormatter.csproj -- -i input.json

# Format and save to file
dotnet run --project JsonFormatter.csproj -- -i input.json -o output.json

# Minify JSON
dotnet run --project JsonFormatter.csproj -- -m -i pretty.json

# Format from stdin
echo '{"name":"John","age":30}' | dotnet run --project JsonFormatter.csproj
```

## Options

| Option | Description |
|--------|-------------|
| `-m, --minify` | Minify JSON (remove whitespace) |
| `-i, --input <file>` | Input JSON file |
| `-o, --output <file>` | Output file |

## Build

```bash
dotnet build -c Release
```
