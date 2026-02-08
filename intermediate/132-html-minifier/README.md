# HTML Minifier

Minify HTML by removing unnecessary whitespace and optional comments.

## Usage

```bash
dotnet run --project 132-html-minifier.csproj -- <input.html> [output.html]
cat input.html | dotnet run --project 132-html-minifier.csproj
```

## Example

```bash
dotnet run --project 132-html-minifier.csproj -- index.html minified.html
```

## Options

- `--remove-comments` - Remove HTML comments
- `--remove-whitespace` - Remove extra whitespace (default: true)

## Concepts Demonstrated

- Regular expressions
- File I/O
- String manipulation
- Size reduction calculation
- Stream processing
