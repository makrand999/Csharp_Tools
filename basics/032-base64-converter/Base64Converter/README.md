# Base64 Converter

A practical CLI tool for encoding and decoding Base64 strings. Useful for working with encoded data, JWT tokens, API keys, and binary-to-text conversions.

## Usage

```bash
dotnet run --project Base64Converter.csproj <command> [text]
```

### Commands

| Command | Description |
|---------|-------------|
| `encode`, `enc`, `e` | Encode text to Base64 |
| `decode`, `dec`, `d` | Decode Base64 to text |
| `help`, `-h` | Show help message |

### Auto-Detect Mode

If no command is specified, the tool automatically detects whether the input is Base64 and acts accordingly.

## Examples

```bash
# Encode a string
dotnet run --project Base64Converter.csproj encode "Hello World"
# Output: SGVsbG8gV29ybGQ=

# Decode a Base64 string
dotnet run --project Base64Converter.csproj decode SGVsbG8gV29ybGQ=
# Output: Hello World

# Auto-detect mode (decodes Base64)
dotnet run --project Base64Converter.csproj SGVsbG8gV29ybGQ=
# Output: Hello World

# Auto-detect mode (encodes plain text)
dotnet run --project Base64Converter.csproj "Hello World"
# Output: SGVsbG8gV29ybGQ=

# Encode JSON or special characters
dotnet run --project Base64Converter.csproj encode '{"key":"value"}'
# Output: eyJrZXkiOiJ2YWx1ZSJ9
```

## Use Cases

- **JWT Tokens**: Decode JWT header/payload for inspection
- **API Keys**: Encode/decode API credentials
- **Data URIs**: Work with Base64-encoded images in CSS/HTML
- **Email Attachments**: Handle Base64-encoded email content
- **Binary Data**: Convert binary files to text representation

## Concepts Demonstrated

- Base64 encoding/decoding with `Convert.ToBase64String` and `Convert.FromBase64String`
- UTF-8 text encoding
- String manipulation and validation
- CLI argument parsing
- Exception handling
