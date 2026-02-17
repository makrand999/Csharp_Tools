# CancellationToken Manager

Utility for coordinating cancellation across multiple async operations. Demonstrates linked tokens, timeout cancellation, and graceful shutdown patterns.

## Usage

```bash
dotnet run --project CancellationTokenManager.csproj
```

## Example

```
=== CancellationToken Manager ===
Coordinating cancellation across async operations.

Choose operation:
1. Basic Cancellation Demo
2. Linked Tokens Demo
3. Timeout Cancellation
4. Graceful Shutdown Pattern
5. Run All Demos

Selection (1-5): 1

--- Basic Cancellation Demo ---

Working... 1%
Working... 2%
Working... 3%
Working... 4%
Working... 5%
Working... 6%
Working... 7%
Working... 8%
Working... 9%
Working... 10%

Cancelling operation...
Operation was cancelled.
```

## Concepts Demonstrated

- **CancellationTokenSource** - Creating and managing cancellation tokens
- **Token Linking** - `CreateLinkedTokenSource` for multiple sources
- **Timeout Cancellation** - `CancellationTokenSource(TimeSpan)`
- **OperationCanceledException** - Handling cancellation
- **ThrowIfCancellationRequested** - Cooperative cancellation checks
- **Graceful Shutdown** - Coordinating multiple worker tasks
- **Async Cancellation** - Passing tokens to async operations
