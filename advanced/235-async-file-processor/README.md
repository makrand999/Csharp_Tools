# Async File Processor

Processes files asynchronously with progress reporting, demonstrating async I/O operations with cancellation support.

## Usage

```bash
dotnet run --project AsyncFileProcessor.csproj
```

## Example

```
=== Async File Processor ===

Creating test files in: /tmp/async_test_...

Found 10 files to process

[1/10] file_0.txt - Hash: a1b2c3d4..., Lines: 150
    Progress: 10.0% | Speed: 256.5 KB/s
...

=== Processing Summary ===
Total Files: 10
Processed: 10
Total Bytes: 45,678
Elapsed Time: 0.15s
Throughput: 297.8 KB/s
```

## Concepts Demonstrated

- Async file I/O with StreamReader
- SemaphoreSlim for concurrent limiting
- ConcurrentBag for thread-safe results
- Progress reporting
- CancellationToken propagation
- MD5 hash computation
