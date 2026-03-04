# Concurrent HashSet

Thread-safe hash set implementation using `ReaderWriterLockSlim` for concurrent read/write operations.

## Usage

```bash
dotnet run --project ConcurrentHashSet.csproj
```

## Example

```
=== Concurrent HashSet ===

Testing thread-safe hash set with multiple readers and writers...

Starting concurrent operations...

Writer 0: Added 0
Writer 1: Added 100
Reader 0: Count=2, HasRandom=False
Writer 2: Added 200
...

✓ Final count: 100 items
✓ All concurrent operations completed successfully!
```

## Concepts Demonstrated

- `ReaderWriterLockSlim` for read/write locking
- Read locks for non-modifying operations
- Write locks for modifying operations
- Upgradeable read locks for check-then-act
- Safe enumeration with snapshot copying
- Thread-safe collection patterns
