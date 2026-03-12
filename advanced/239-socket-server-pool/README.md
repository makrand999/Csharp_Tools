# Socket Server Pool

Manages a pool of reusable socket connections, demonstrating connection pooling and efficient socket management.

## Usage

```bash
dotnet run --project SocketServerPool.csproj
```

## Example

```
=== Socket Server Pool ===

Echo server started on port 5000

Simulating concurrent socket usage...

Task 0: Borrowed socket (pool available: 4)
Task 1: Borrowed socket (pool available: 3)
...
Task 0: Returned socket

=== Pool Statistics ===
Max Pool Size: 5
Total Created: 5
Total Borrowed: 10
Total Returned: 10
Currently Available: 5

Connection reuse ratio: 200.0%
```

## Concepts Demonstrated

- ConcurrentBag for object pooling
- Socket connection management
- TcpListener for server
- Resource borrowing/returning pattern
- Connection reuse optimization
