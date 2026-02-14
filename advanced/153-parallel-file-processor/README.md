# Parallel File Processor

A high-performance file processor that uses parallel processing to analyze multiple files concurrently.

## Usage

```bash
dotnet run --project ParallelFileProcessor.csproj <directory> [options]
```

### Operations

| Operation | Description |
|-----------|-------------|
| `hash` | Calculate SHA256 hash for each file |
| `wordcount` | Count words in text files |
| `lines` | Count lines in text files |
| `stats` | File statistics (size, extension analysis) |

### Options

| Option | Description | Default |
|--------|-------------|---------|
| `--op <operation>` | Operation to perform | hash |
| `--parallel <n>` | Max parallel operations | CPU count |
| `--pattern <glob>` | File pattern filter | * |
| `--recursive` | Search subdirectories | false |

## Examples

```bash
# Calculate hashes for all files in a directory
dotnet run --project ParallelFileProcessor.csproj ./docs --op hash

# Count words in all text files recursively
dotnet run --project ParallelFileProcessor.csproj ./src --op wordcount --pattern *.cs --recursive

# Get file statistics for a log directory
dotnet run --project ParallelFileProcessor.csproj /var/log --op stats --recursive

# Count lines with custom parallelism
dotnet run --project ParallelFileProcessor.csproj ./project --op lines --parallel 8
```

## Example Output

```
Parallel File Processor
Directory: /home/user/docs
Operation: wordcount
Parallel threads: 8
Pattern: *.txt
Recursive: true

Found 150 files to process

✓ readme.txt (2.45 KB)
✓ documentation.md (15.32 KB)
✓ notes.txt (1.02 KB)
...

============================================================
Processing completed in 234ms
Files processed: 150
Throughput: 641.03 files/sec

Total words: 45678

Word count by file:
     12450  documentation.md
      8934  readme.txt
      5621  guide.md
      3456  tutorial.txt
      2890  notes.txt
```

## Concepts Demonstrated

- **Parallel.ForEach** - Parallel loop processing
- **ConcurrentBag<T>** - Thread-safe collection
- **ParallelOptions** - Configuring parallelism
- **SHA256** - Cryptographic hashing
- **StreamReader** - Efficient file reading
- **FileInfo** - File metadata access
- **LINQ** - Data aggregation and grouping
- **Path manipulation** - File path operations
