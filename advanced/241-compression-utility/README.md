# Compression Utility

Compresses and decompresses files using GZip/Deflate algorithms with progress reporting.

## Usage

```bash
dotnet run --project CompressionUtility.csproj
```

## Example

```
=== Compression Utility ===

Creating test file (1 MB of data)...
Original size: 1024 KB

Compressing with GZip...
Compressed size: 2 KB
Compression ratio: 99.8%
Time: 15ms

Decompressing...
Decompressed size: 1024 KB
Time: 10ms

Verification: ✓ PASSED
```

## Concepts Demonstrated

- GZipStream for compression
- ZipArchive for directory archiving
- Stream-based file operations
- Compression ratio calculation
- Async file I/O
