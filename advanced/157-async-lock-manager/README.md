# Async Lock Manager

Demonstrates async synchronization primitives for coordinating concurrent access to shared resources. Includes SemaphoreSlim, ReaderWriterLockSlim, and thread-safe counter patterns.

## Usage

```bash
dotnet run --project AsyncLockManager.csproj
```

## Example

```
=== Async Lock Manager ===
Demonstrating async synchronization primitives.

Choose operation:
1. Test Async Lock (SemaphoreSlim)
2. Test Reader-Writer Lock
3. Test Concurrent Access Demo
4. Run All Tests

Selection (1-4): 1

--- Testing Async Lock (SemaphoreSlim) ---

Task 1: Waiting for lock...
Task 1: Acquired lock at 14:35:22.456
Task 2: Waiting for lock...
Task 3: Waiting for lock...
Task 1: Releasing lock at 14:35:22.957
Task 2: Acquired lock at 14:35:22.958
```

## Concepts Demonstrated

- **SemaphoreSlim** - Async-compatible exclusive locking with `WaitAsync()`
- **ReaderWriterLockSlim** - Read/write lock for concurrent read access
- **Thread Safety** - Protecting shared state in async contexts
- **Lock Patterns** - Try-finally patterns for safe lock release
- **Concurrent Collections** - Thread-safe dictionary access
- **Async Coordination** - Multiple tasks coordinating access
