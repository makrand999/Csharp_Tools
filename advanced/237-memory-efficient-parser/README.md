# Memory Efficient Parser

Uses Span<T> and Memory<T> for zero-allocation parsing, demonstrating high-performance text processing.

## Usage

```bash
dotnet run --project MemoryEfficientParser.csproj
```

## Example

```
=== Memory Efficient Parser ===

Parsing CSV data:

Name,Age,City,Occupation
John Doe,30,New York,Engineer
...

Line 1: 4 fields
  [0]: Name           [1]: Age            [2]: City           [3]: Occupation
...

Processing with memory pool...
Lines processed: 10,000
Time: 45ms
Throughput: 2.1 MB/s
```

## Concepts Demonstrated

- Span<T> for stack-based memory
- ReadOnlySpan<char> for zero-allocation parsing
- MemoryPool<T> for buffer pooling
- ref struct for stack-only types
- High-performance string parsing
