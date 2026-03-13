# Distributed Cache Simulator

Simulates a distributed cache with multiple nodes, demonstrating consistent hashing, replication, and failover patterns.

## Usage

```bash
dotnet run --project DistributedCache.csproj
```

## Example

```
=== Distributed Cache Simulator ===

Setting values in distributed cache...
  Set: user:0 = User Data 0
...

Getting values...
  Get: user:0 = User Data 0

=== Cache Statistics ===
Total Nodes: 3
Online Nodes: 3
Total Entries: 15
Replication Factor: 2

=== Simulating Node Failure ===
Removed node: node-0

Trying to get values after node failure...
  Get: user:0 = User Data 0
```

## Concepts Demonstrated

- Distributed cache architecture
- Hash-based data distribution
- Replication for fault tolerance
- Failover handling
- Node health monitoring
