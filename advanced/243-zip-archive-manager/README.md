# ZIP Archive Manager

Creates, extracts, and manages ZIP archives with compression level options.

## Usage

```bash
dotnet run --project ZipArchiveManager.csproj
```

## Example

```
=== ZIP Archive Manager ===

Creating test files...

Creating ZIP archive...
Files added: 5
Original size: 15 KB
Compressed size: 2 KB
Compression ratio: 87.5%
Time: 25ms

=== Archive Contents ===
Total entries: 5
  file0.txt
    Size: 1000 -> 50 bytes (95.0%)
...

Extracting archive...
Files extracted: 5
Time: 15ms

Verification: ✓ PASSED
```

## Concepts Demonstrated

- ZipArchive for archive creation
- ZipFile for archive extraction
- CompressionLevel options
- Entry metadata reading
- Directory traversal
