# TAR Archive Handler

Creates and extracts TAR archives with basic TAR format implementation.

## Usage

```bash
dotnet run --project TarArchiveHandler.csproj
```

## Example

```
=== TAR Archive Handler ===

Creating test files...

Creating TAR archive...
Files added: 3
Total size: 9000 bytes
Archive size: 10240 bytes
Time: 20ms

Extracting TAR archive...
Files extracted: 3
Total size: 9000 bytes
Time: 15ms

Verification: ✓ PASSED
```

## Concepts Demonstrated

- TAR file format parsing
- Header creation and parsing
- Octal number encoding
- Block-aligned writing
- Binary file I/O
