# Circuit Breaker

Resilience pattern implementation that prevents cascading failures by failing fast when a service is unhealthy.

## Usage

```bash
dotnet run --project 174-circuit-breaker/CircuitBreaker.csproj
```

## Example

```
Circuit Breaker Pattern
=======================

Simulating service calls with 70% failure rate...
Circuit will open after 3 consecutive failures

Call #1: ✗ Service Error: Service unavailable
   State: Closed (Failures: 1)
Call #2: ✗ Service Error: Service unavailable
   State: Closed (Failures: 2)
Call #3: ✗ Service Error: Service unavailable
   State: Closed (Failures: 3)
   [Circuit] Opening circuit for 5s
Call #4: ⚡ Circuit OPEN - Failing fast
   State: Open (Failures: 3)
Call #5: ⚡ Circuit OPEN - Failing fast
   State: Open (Failures: 3)

Waiting for circuit to recover...

Attempting recovery calls:
   [Circuit] Transitioning to HALF-OPEN for test
Call #16: ✓ Success (Response: OK-a1b2c3d4)
   [Circuit] Recovery successful - Circuit CLOSED
   State: Closed (Failures: 0)
```

## Circuit States

| State | Behavior |
|-------|----------|
| **Closed** | Normal operation, tracking failures |
| **Open** | Failing fast, no calls to service |
| **Half-Open** | Testing if service recovered |

## Concepts Demonstrated

- Circuit breaker pattern
- State machine implementation
- Failure threshold tracking
- Recovery timeout handling
- Resilience patterns
- Async execution wrapper
