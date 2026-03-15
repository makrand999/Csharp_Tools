# System Trace Logger

Captures and logs system events with structured logging and trace listeners.

## Usage

```bash
dotnet run --project SystemTraceLogger.csproj
```

## Example

```
=== System Trace Logger ===

[INFO] [Startup] Application started
[DEBUG] [Startup] Initializing components
[INFO] [Processing] Processing data...
[DEBUG] [Processing] Processing item 1/5
...
[WARN] [Performance] High memory usage detected
[ERROR] [Network] Connection timeout

=== All Logs ===
[14:30:25.123] [   INFO] [Startup] Application started
[14:30:25.234] [  DEBUG] [Startup] Initializing components
...

=== Errors Only ===
[14:30:26.789] [Network] Connection timeout
  Exception: Operation timed out
```

## Concepts Demonstrated

- TraceListener pattern
- Structured logging
- Log level filtering
- JSON export
- Concurrent collections
