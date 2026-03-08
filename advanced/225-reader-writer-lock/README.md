# Reader-Writer Lock

Async reader-writer lock implementation for concurrent read/write scenarios. Demonstrates multiple-reader single-writer (MRSW) pattern with async support.

## Usage

```bash
dotnet run --project ReaderWriterLock/ReaderWriterLock.csproj
```

## Example

```
=== Async Reader-Writer Lock ===

Reader 0: Read key-0 = value-000
Reader 1: Read key-1 = value-001
Writer: Wrote key-0 = value-000
Reader 2: Read key-0 = value-000
...

Cache statistics:
  Total reads: 100
  Total writes: 10
```

## Concepts Demonstrated

- Multiple-reader single-writer pattern
- Async synchronization primitives
- Reader-writer lock implementation
- Thread-safe caching
- `IDisposable` for lock release
