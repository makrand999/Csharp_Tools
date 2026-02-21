# Rate Limiter

Token bucket and sliding window rate limiting algorithms to control operation rates and prevent overload.

## Usage

```bash
dotnet run --project 175-rate-limiter/RateLimiter.csproj
```

## Example

```
Rate Limiter - Token Bucket Algorithm
=====================================

Configuration: 5 tokens max, 2 tokens/second refill

Attempting 15 rapid requests...

[     0ms] ✓ Request-1
[    52ms] ✓ Request-2
[   105ms] ✓ Request-3
[   157ms] ✓ Request-4
[   210ms] ✓ Request-5
[   262ms] ✗ Request-6 - Rate limited
[   315ms] ✗ Request-7 - Rate limited

--- Sliding Window Rate Limiter ---

Configuration: 5 requests per 2-second window

Attempting 10 rapid requests...

[     0ms] ✓ Request #1 - Allowed
[   201ms] ✓ Request #2 - Allowed
[   402ms] ✓ Request #3 - Allowed
[   603ms] ✓ Request #4 - Allowed
[   804ms] ✓ Request #5 - Allowed
[  1005ms] ✗ Request #6 - Rate limited
```

## Algorithms

| Algorithm | Description |
|-----------|-------------|
| **Token Bucket** | Tokens refill at constant rate, requests consume tokens |
| **Sliding Window** | Tracks requests in rolling time window |

## Concepts Demonstrated

- Token bucket algorithm
- Sliding window algorithm
- Rate limiting patterns
- Thread-safe state management
- Time-based operations
- Request throttling
