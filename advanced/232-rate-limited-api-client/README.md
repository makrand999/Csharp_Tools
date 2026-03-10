# Rate Limited API Client

Implements token bucket algorithm for rate limiting API calls to stay within specified limits and avoid throttling.

## Usage

```bash
dotnet run --project RateLimitedApiClient.csproj
```

## Example

```
=== Rate Limited API Client ===

Using token bucket algorithm for rate limiting

Fetching posts from JSONPlaceholder API...

✓ Post 1 fetched successfully
✓ Post 2 fetched successfully
...

=== Rate Limited Client Statistics ===
Base URL: https://jsonplaceholder.typicode.com
Total Requests: 5
Rate Limited: 0
Rate Limit Rate: 0.0%
```

## Concepts Demonstrated

- Token bucket rate limiting algorithm
- Thread-safe token management
- HTTP client with retry-after handling
- Async/await with cancellation
- Dynamic JSON deserialization
