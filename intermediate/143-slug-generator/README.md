# Slug Generator

Convert text to URL-friendly slugs.

## Usage

```bash
dotnet run --project 143-slug-generator.csproj -- "<text>"
echo "Hello World" | dotnet run --project 143-slug-generator.csproj
```

## Example

```bash
dotnet run --project 143-slug-generator.csproj -- "Hello World!"
dotnet run --project 143-slug-generator.csproj -- "C# Programming Guide" --separator _
dotnet run --project 143-slug-generator.csproj -- "Long Title Here" --max-length 20
```

### Sample Output

```
hello-world
c_programming_guide
long-title-here
```

## Options

- `--lowercase` - Convert to lowercase (default)
- `--separator C` - Use custom separator (default: -)
- `--max-length N` - Truncate to N characters

## Concepts Demonstrated

- String normalization
- Diacritic removal
- Regex pattern matching
- URL-safe string generation
