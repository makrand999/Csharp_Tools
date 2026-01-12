# URL Encoder/Decoder

A CLI tool for encoding and decoding URL-safe strings.

## Usage

```bash
dotnet run --project UrlEncoder.csproj [command] <text>
```

## Commands

- `encode`, `-e`, `--encode` - URL encode text (default if no command)
- `decode`, `-d`, `--decode` - URL decode text
- `encode-form` - Encode form data (spaces as `+`)
- `decode-form` - Decode form data

## Examples

```bash
# Encode a URL string
dotnet run --project UrlEncoder.csproj encode "Hello World!"
# Output: Hello%20World%21

# Decode a URL string
dotnet run --project UrlEncoder.csproj decode "Hello%20World%21"
# Output: Hello World!

# Quick encode (default)
dotnet run --project UrlEncoder.csproj "https://example.com/search?q=test"
# Output: https%3A%2F%2Fexample.com%2Fsearch%3Fq%3Dtest

# Encode form data (spaces become +)
dotnet run --project UrlEncoder.csproj encode-form "name=John Doe"
# Output: name%3DJohn+Doe
```

## Features

- Standard URL encoding (%20 for spaces)
- Form data encoding (+ for spaces)
- UTF-8 character support
- Multiple command aliases

## Concepts Demonstrated

- System.Web.HttpUtility for URL encoding/decoding
- Command-line argument parsing
- UTF-8 encoding handling
- String manipulation
