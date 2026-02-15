# Memory-Mapped File Reader

A high-performance file reader using memory-mapped files for efficient processing of large files without loading them entirely into memory.

## Usage

```bash
dotnet run --project MemoryMappedFileReader.csproj <file> [options]
```

### Operations

| Operation | Description |
|-----------|-------------|
| `info` | File information and structure |
| `lines` | Count lines using memory mapping |
| `words` | Count words using memory mapping |
| `search` | Search for text pattern |
| `hex` | Display hex dump of file |
| `stats` | Character frequency analysis |

### Options

| Option | Description | Default |
|--------|-------------|---------|
| `--op <operation>` | Operation to perform | info |
| `--pattern <text>` | Search pattern (for search) | - |
| `--view <bytes>` | Bytes to display for hex dump | 256 |

## Examples

```bash
# Get file information
dotnet run --project MemoryMappedFileReader.csproj largefile.log --op info

# Count lines in a large log file
dotnet run --project MemoryMappedFileReader.csproj /var/log/syslog --op lines

# Search for error patterns
dotnet run --project MemoryMappedFileReader.csproj app.log --op search --pattern "ERROR"

# View hex dump of binary file
dotnet run --project MemoryMappedFileReader.csproj image.png --op hex --view 512

# Character frequency analysis
dotnet run --project MemoryMappedFileReader.csproj book.txt --op stats
```

## Example Output

```
Memory-Mapped File Reader
==========================
File: application.log
Size: 125.50 MB
Operation: lines

Line Count (memory-mapped):
  Total lines: 2,456,789
  Time: 145ms

==================================================

Memory-Mapped File Reader
==========================
File: data.bin
Size: 1.00 MB
Operation: hex

Hex Dump (256 bytes):
----------------------------------------------------------------------
00000000  89 50 4E 47 0D 0A 1A 0A  00 00 00 0D 49 48 44 52  |.PNG........IHDR|
00000010  00 00 01 00 00 00 01 00  01 03 00 00 00 96 6C 02  |..............l.|
00000020  41 44 4C 54 00 00 00 00  00 00 00 00 00 00 00 00  |ADLT............|
...
----------------------------------------------------------------------
Displayed 256 of 1048576 total bytes
```

## Concepts Demonstrated

- **MemoryMappedFile** - Memory-mapped file access
- **MemoryMappedViewAccessor** - Direct memory access
- **Span<T>** - Stack-only memory span
- **Unsafe memory access** - Low-level byte operations
- **File signatures** - Magic number detection
- **Character encoding** - ASCII/UTF-8 handling
- **Hex dump** - Binary data visualization
- **Frequency analysis** - Statistical processing

## Performance Benefits

Memory-mapped files provide:
- **Zero-copy** - No need to read file into buffer
- **Lazy loading** - OS loads pages on demand
- **Large file support** - Process files larger than RAM
- **Shared memory** - Multiple processes can access same mapping
- **Random access** - Efficient seeking within file
