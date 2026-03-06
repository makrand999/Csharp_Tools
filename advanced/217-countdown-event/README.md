# Countdown Event

Demonstrates `CountdownEvent` for coordinating multiple operations and waiting for all to complete.

## Usage

```bash
dotnet run --project CountdownEvent.csproj
```

## Example

```
=== Countdown Event ===

Coordinating multiple operations with CountdownEvent...

--- Example 1: Basic Usage ---

Starting 5 tasks...

[Task 0] Starting work...
[Task 1] Starting work...
[Task 2] Starting work...
...
[Task 3] Work complete, signaling countdown.

Main thread waiting for all tasks to complete...

✓ All tasks completed!
```

## Concepts Demonstrated

- `CountdownEvent` for counting completions
- `Signal()` to decrement the count
- `Wait()` to block until count reaches zero
- `AddCount()` for dynamic task addition
- Producer-consumer coordination
- Parallel batch processing patterns

## Common Use Cases

- Waiting for multiple parallel operations
- Producer-consumer coordination
- Dynamic task spawning with completion tracking
- Parallel batch processing
