# Character Encoding Detector

Detect the character encoding of text files.

## Usage

```bash
dotnet run --project 145-charset-detector.csproj -- <file>
```

## Example

```bash
dotnet run --project 145-charset-detector.csproj -- document.txt
dotnet run --project 145-charset-detector.csproj -- japanese-file.txt
```

### Sample Output

```
File: document.txt
Size: 1,234 bytes

=== Detected Encodings ===
UTF-8        Confidence: 98.5  Valid chars: 1200/1234 (97.2%)
UTF-16LE     Confidence: 45.2  Valid chars: 800/1234 (64.8%)
ASCII        Confidence: 32.1  Valid chars: 600/1234 (48.6%)

=== Preview (using UTF-8) ===
Hello World
This is a test file...
```

## Supported Encodings

- UTF-8, UTF-16LE, UTF-16BE, UTF-32
- ASCII, ISO-8859-1, Windows-1252

## Concepts Demonstrated

- Encoding detection
- BOM detection
- Character validation
- Text decoding
