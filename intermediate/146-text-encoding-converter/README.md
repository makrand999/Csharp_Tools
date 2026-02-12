# Text Encoding Converter

Convert text files between different character encodings.

## Usage

```bash
dotnet run --project 146-text-encoding-converter.csproj -- <input> <output> --from <enc> --to <enc>
```

## Example

```bash
dotnet run --project 146-text-encoding-converter.csproj -- input.txt output.txt --from UTF-8 --to UTF-16
dotnet run --project 146-text-encoding-converter.csproj -- latin1.txt utf8.txt --from ISO-8859-1 --to UTF-8
```

## Supported Encodings

- UTF-8, UTF-16, UTF-16LE, UTF-16BE, UTF-32
- ASCII, ISO-8859-1 (Latin-1), Windows-1252

## Concepts Demonstrated

- Encoding conversion
- Byte array manipulation
- Text decoding/encoding
- File I/O with different encodings
