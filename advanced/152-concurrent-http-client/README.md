# Concurrent HTTP Client with Rate Limiting

A high-performance HTTP client that makes parallel requests with configurable rate limiting and retry logic.

## Usage

```bash
dotnet run --project ConcurrentHttpClient.csproj [options]
```

### Options

| Option | Description | Default |
|--------|-------------|---------|
| `--urls <file>` | File with URLs (one per line) | Demo URLs |
| `--concurrency <n>` | Max concurrent requests | 5 |
| `--rps <n>` | Requests per second limit | 10 |
| `--timeout <ms>` | Request timeout in milliseconds | 30000 |

## Examples

```bash
# Run with demo URLs
dotnet run --project ConcurrentHttpClient.csproj

# Process URLs from file
dotnet run --project ConcurrentHttpClient.csproj --urls urls.txt

# Custom concurrency and rate limit
dotnet run --project ConcurrentHttpClient.csproj --concurrency 10 --rps 20
```

## Example URLs File

```
https://api.example.com/users/1
https://api.example.com/users/2
https://api.example.com/users/3
https://api.example.com/products/1
```

## Example Output

```
Using 15 demo URLs

Configuration:
  Max concurrency: 5
  Rate limit: 10 req/sec
  Timeout: 30s

✓ https://jsonplaceholder.typicode.com/posts/1 - 200 (45ms)
✓ https://jsonplaceholder.typicode.com/posts/2 - 200 (52ms)
✓ https://jsonplaceholder.typicode.com/posts/3 - 200 (48ms)
✗ https://httpbin.org/status/404 - 404 (120ms)
✗ https://httpbin.org/status/500 - 500 (95ms)

==================================================
Completed in 2345ms
Total requests: 15
Successful: 12
Failed: 3
Average response time: 78.45ms
Requests/second: 6.40
```

## Concepts Demonstrated

- **SemaphoreSlim** - Concurrency limiting for parallel operations
- **Rate Limiting** - Controlling requests per second
- **Async/Await** - Non-blocking HTTP operations
- **Retry Logic** - Automatic retry on failure
- **ConcurrentBag<T>** - Thread-safe result collection
- **Stopwatch** - Performance measurement
- **HttpClient** - Efficient HTTP client usage
- **Parallel Processing** - Concurrent task execution
