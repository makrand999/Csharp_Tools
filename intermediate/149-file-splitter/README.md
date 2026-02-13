# File Splitter/Joiner

Split large files into parts and join them back together.

## Usage

```bash
dotnet run --project 149-file-splitter.csproj -- split <file> --parts N
dotnet run --project 149-file-splitter.csproj -- split <file> --size 10MB
dotnet run --project 149-file-splitter.csproj -- join <output> <part1> <part2> ...
```

## Example

```bash
# Split into 5 equal parts
dotnet run --project 149-file-splitter.csproj -- split largefile.zip --parts 5

# Split by size
dotnet run --project 149-file-splitter.csproj -- split video.mp4 --size 100MB
dotnet run --project 149-file-splitter.csproj -- split data.bin --size 1GB

# Join parts back
dotnet run --project 149-file-splitter.csproj -- join restored.zip file.part001 file.part002 ...
```

## Size Formats

- KB - Kilobytes (1024 bytes)
- MB - Megabytes
- GB - Gigabytes

## Concepts Demonstrated

- File streaming
- Binary file I/O
- Chunked processing
- File manipulation
