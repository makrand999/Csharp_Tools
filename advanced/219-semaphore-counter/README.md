# Semaphore Counter

Demonstrates `SemaphoreSlim` for limiting concurrent access to resources and managing resource pools.

## Usage

```bash
dotnet run --project SemaphoreCounter.csproj
```

## Example

```
=== Semaphore Counter ===

Limiting concurrent access with Semaphore...

--- Example 1: Limiting Concurrent Database Connections ---

Allowing max 3 concurrent connections out of 8 requests...

[Request 1] Waiting for connection...
[Request 2] Waiting for connection...
[Request 3] Waiting for connection...
[Request 1] Got connection (active: 1/3)
[Request 2] Got connection (active: 2/3)
[Request 3] Got connection (active: 3/3)
[Request 4] Waiting for connection...
[Request 1] Database operation complete
[Request 1] Released connection
[Request 4] Got connection (active: 3/3)
...

✓ All database requests completed!
```

## Concepts Demonstrated

- `SemaphoreSlim` for access limiting
- `WaitAsync()` for async acquisition
- `Release()` to return permits
- Timeout-based acquisition with `WaitAsync(TimeSpan)`
- Resource pool management
- Rate limiting patterns
- Concurrent connection pooling

## Common Use Cases

- Database connection pooling
- API rate limiting
- Throttling concurrent operations
- Resource pool management
- Limiting parallelism
