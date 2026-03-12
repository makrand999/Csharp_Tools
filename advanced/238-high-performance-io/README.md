# High Performance I/O

Uses System.IO.Pipelines for efficient stream processing with zero-copy I/O and backpressure handling.

## Usage

```bash
dotnet run --project HighPerformanceIo.csproj
```

## Example

```
=== High Performance I/O ===

Creating 50 MB test file...
Test file size: 50 MB

=== Pipeline-based Reading ===
Total Bytes: 52,428,800
Buffers: 6400
Time: 0.85s
Throughput: 58.5 MB/s

=== Direct Async Reading ===
Total Bytes: 52,428,800
Time: 0.92s
Throughput: 54.2 MB/s
```

## Concepts Demonstrated

- System.IO.Pipelines Pipe
- Producer-consumer pattern
- Backpressure handling
- Async FileStream with overlapped I/O
- Buffer management
