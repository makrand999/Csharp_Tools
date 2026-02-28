# Object Pool Pattern

Efficient object reuse and lifecycle management with a generic object pool implementation supporting async operations, statistics tracking, and automatic reset.

## Usage

```bash
# Run demo
dotnet run --project ObjectPool.csproj -- demo

# Run interactive mode
dotnet run --project ObjectPool.csproj -- interactive
```

## Example

```
=== Object Pool Pattern Demo ===

1. Database Connection Pool
---------------------------
Client 0: Using connection 1
Client 1: Using connection 2
Client 2: Using connection 1
Client 3: Using connection 3
...

Pool Statistics:
  Total created: 3
  Total borrowed: 8
  Total returned: 8
  Current available: 3
  Reuse rate: 62.5%

2. Expensive Object Reuse (StringBuilder Pool)
----------------------------------------------
Task 0: Task 0: item_0,item_1,item_2,item_3,item_4
Task 1: Task 1: item_0,item_1,item_2,item_3,item_4
...
```

## Concepts Demonstrated

- Object pooling for resource reuse
- ConcurrentBag for thread-safe object storage
- SemaphoreSlim for pool size limiting
- IAsyncDisposable pattern
- Factory pattern for object creation
- Pool statistics and monitoring
- Pre-allocation and lazy expansion
