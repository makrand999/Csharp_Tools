# Retry Pattern Implementation

Resilient operations with configurable retry policies including fixed delay, exponential backoff, jitter, selective exception handling, timeout support, and circuit breaker integration.

## Usage

```bash
# Run demo
dotnet run --project RetryPattern.csproj -- demo

# Run interactive mode
dotnet run --project RetryPattern.csproj -- interactive
```

## Example

```
=== Retry Pattern Implementation Demo ===

1. Basic Retry (Fixed Delay)
----------------------------
Success after 3 attempts: Success on attempt 3

2. Exponential Backoff Retry
----------------------------
Success after 4 attempts: Success on attempt 4
Total time: 702ms

3. Exponential Backoff with Jitter
----------------------------------
Success after 3 attempts: Success on attempt 3
Delays: 142ms, 287ms

4. Selective Exception Handling
-------------------------------
Failed with non-retryable exception: ArgumentException

5. Retry with OnRetry Callback
------------------------------
  Retry 1: InvalidOperationException, waiting 300ms
  Retry 2: InvalidOperationException, waiting 300ms
Final success: Success on attempt 3

6. Async Retry Operations
-------------------------
  Attempt 1 failed: Async failure (attempt 1)
  Attempt 2 failed: Async failure (attempt 2)
Async success: Async success on attempt 3

8. Circuit Breaker Integration
------------------------------
Attempting operations (circuit will open after 3 failures):
  Attempt 1: InvalidOperationException - Always fails
  Attempt 2: InvalidOperationException - Always fails
  Attempt 3: InvalidOperationException - Always fails
  Attempt 4: CircuitOpenException - Circuit breaker is open
  Attempt 5: CircuitOpenException - Circuit breaker is open
Circuit state: Open
```

## Concepts Demonstrated

- Retry policy builder pattern
- Fixed delay and exponential backoff strategies
- Jitter for preventing thundering herd
- Selective exception handling
- OnRetry callbacks for logging/monitoring
- Async/await retry support
- Timeout integration
- Circuit breaker pattern integration
- Retry statistics and monitoring
