# Distributed Lock Manager

Simulates distributed locking across multiple nodes with TTL-based expiration, deadlock detection, and lock hierarchy support.

## Usage

```bash
# Run demo
dotnet run --project DistributedLock.csproj -- demo

# Run interactive mode
dotnet run --project DistributedLock.csproj -- interactive
```

## Example

```
=== Distributed Lock Manager Demo ===

1. Basic Lock Acquisition
--------------------------
Node-1 attempting to acquire lock on 'resource-A'...
Node-1: Lock acquired
Node-2 attempting to acquire lock on 'resource-A'...
Node-2: Lock denied - already locked
Node-2 attempting to acquire lock on 'resource-B'...
Node-2: Lock acquired

2. Lock Status
--------------
Total locks held: 2
Total lock requests: 3
Successful acquisitions: 2
Failed acquisitions: 1

3. Lock Expiration (TTL)
------------------------
Node-3 acquiring lock with 2 second TTL...
Node-3: Lock acquired
Waiting 3 seconds for lock to expire...
Node-1 attempting to acquire expired lock...
Node-1: Lock acquired (TTL expired)
```

## Concepts Demonstrated

- Concurrent collections for thread-safe operations
- TTL-based lock expiration
- Deadlock detection algorithms
- Lock hierarchy for deadlock prevention
- Distributed node simulation
- Interlocked operations for atomic counters
