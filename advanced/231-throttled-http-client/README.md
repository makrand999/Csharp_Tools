# Throttled HTTP Client

Controls concurrent HTTP requests with configurable limits to prevent overwhelming servers and handle rate limiting gracefully.

## Usage

```bash
dotnet run --project ThrottledHttpClient.csproj
```

## Example

```
=== Throttled HTTP Client ===

Downloading 8 URLs with max 3 concurrent requests...

✓ https://httpbin.org/delay/1 - 267 bytes
✓ https://httpbin.org/delay/1 - 267 bytes
...

Total time: 3500ms

=== HTTP Client Statistics ===
Max Concurrency: 3
Total Requests: 8
Successful: 8
Failed: 0
Success Rate: 100.0%
```

## Concepts Demonstrated

- SemaphoreSlim for concurrency control
- HttpClient with async/await
- Thread-safe counters with Interlocked
- ConcurrentBag for thread-safe collections
- IDisposable pattern for resource cleanup
