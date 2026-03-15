# File Watch Service

Monitors directories for file system changes with event handling and debouncing.

## Usage

```bash
dotnet run --project FileWatchService.csproj
```

## Example

```
=== File Watch Service ===

Watching: /tmp/watch_test_... (filter: *.*)

Performing test operations...

[CREATED] test.txt at 14:30:25.123
[CHANGED] test.txt at 14:30:25.234
[RENAMED] test.txt -> renamed.txt at 14:30:25.345
[DELETED] renamed.txt at 14:30:25.456

=== Event Summary ===
Total events captured: 4
  Created: 1
  Changed: 1
  Deleted: 1
  Renamed: 1
```

## Concepts Demonstrated

- FileSystemWatcher class
- Event handling patterns
- Change type detection
- Real-time monitoring
- Event history tracking
