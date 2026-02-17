# Span-Based Text Parser

High-performance text parser using `Span<T>` and `Memory<T>` for zero-allocation string processing.

## Usage

```bash
dotnet run --project SpanTextParser.csproj
```

## Example

```
=== Span-Based Text Parser ===

1. CSV Line Parsing (zero allocations):
   Field 0: John Doe
   Field 1: 35
   Field 2: Engineer
   Field 3: New York
   Field 4: USA

2. Key-Value Pair Parsing:
   name = John
   age = 35
   city = NYC
   active = true

3. Log Entry Parsing:
   Timestamp: 2025-03-31 14:30:45
   Level: ERROR
   Source: Database
   Message: Connection timeout after 30s

4. URL Query String Parsing:
   search: csharp
   sort: date
   limit: 10
   offset: 20

5. Performance Demo:
   Span<T> parsing: 0.5234ms
   String parsing:  2.1456ms
   Speedup: 4.10x

6. UTF-8 Byte Span Parsing:
   Byte count: 18
   First 5 bytes: [72, 101, 108, 108, 111]
   ASCII chars: 14, Multi-byte sequences: 2
```

## Concepts Demonstrated

- `Span<T>` and `ReadOnlySpan<T>` for stack-only string processing
- Zero-allocation parsing (no intermediate string creation)
- `Memory<T>` for heap-based scenarios
- UTF-8 byte span processing
- High-performance string manipulation
- `Slice()` for efficient substring operations
- `IndexOf()` for fast character searching
- Performance comparison with traditional string methods
