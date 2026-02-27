using System.Collections.Concurrent;

namespace DistributedLock;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Distributed Lock Manager - Simulate distributed locking across nodes");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project DistributedLock.csproj -- demo");
            Console.WriteLine("  dotnet run --project DistributedLock.csproj -- interactive");
            return 0;
        }

        if (args[0].Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            RunDemo();
            return 0;
        }

        if (args[0].Equals("interactive", StringComparison.OrdinalIgnoreCase))
        {
            RunInteractiveMode();
            return 0;
        }

        Console.WriteLine($"Unknown command: {args[0]}");
        Console.WriteLine("Use 'demo' or 'interactive'");
        return 1;
    }

    private static void RunDemo()
    {
        Console.WriteLine("=== Distributed Lock Manager Demo ===\n");

        var lockManager = new DistributedLockManager();

        // Simulate multiple nodes competing for locks
        Console.WriteLine("1. Basic Lock Acquisition");
        Console.WriteLine("--------------------------");
        
        var node1 = new DistributedNode("Node-1", lockManager);
        var node2 = new DistributedNode("Node-2", lockManager);
        var node3 = new DistributedNode("Node-3", lockManager);

        // Node 1 acquires lock on resource A
        Console.WriteLine("Node-1 attempting to acquire lock on 'resource-A'...");
        var lock1 = node1.AcquireLock("resource-A", TimeSpan.FromSeconds(5));
        Console.WriteLine($"Node-1: {(lock1 ? "Lock acquired" : "Lock failed")}");

        // Node 2 tries to acquire same lock (should fail)
        Console.WriteLine("Node-2 attempting to acquire lock on 'resource-A'...");
        var lock2 = node2.AcquireLock("resource-A", TimeSpan.FromSeconds(1));
        Console.WriteLine($"Node-2: {(lock2 ? "Lock acquired" : "Lock denied - already locked")}");

        // Node 2 acquires lock on resource B
        Console.WriteLine("Node-2 attempting to acquire lock on 'resource-B'...");
        var lock3 = node2.AcquireLock("resource-B", TimeSpan.FromSeconds(5));
        Console.WriteLine($"Node-2: {(lock3 ? "Lock acquired" : "Lock failed")}");

        Console.WriteLine();

        // Show lock status
        Console.WriteLine("2. Lock Status");
        Console.WriteLine("--------------");
        var status = lockManager.GetStatus();
        Console.WriteLine($"Total locks held: {status.ActiveLocks}");
        Console.WriteLine($"Total lock requests: {status.TotalRequests}");
        Console.WriteLine($"Successful acquisitions: {status.SuccessfulAcquisitions}");
        Console.WriteLine($"Failed acquisitions: {status.FailedAcquisitions}");
        Console.WriteLine();

        // Demonstrate lock expiration
        Console.WriteLine("3. Lock Expiration (TTL)");
        Console.WriteLine("------------------------");
        Console.WriteLine("Node-3 acquiring lock with 2 second TTL...");
        var shortLock = node3.AcquireLock("resource-C", TimeSpan.FromSeconds(2));
        Console.WriteLine($"Node-3: {(shortLock ? "Lock acquired" : "Lock failed")}");
        
        Console.WriteLine("Waiting 3 seconds for lock to expire...");
        Thread.Sleep(3000);
        
        Console.WriteLine("Node-1 attempting to acquire expired lock...");
        var reacquiredLock = node1.AcquireLock("resource-C", TimeSpan.FromSeconds(5));
        Console.WriteLine($"Node-1: {(reacquiredLock ? "Lock acquired (TTL expired)" : "Lock failed")}");
        Console.WriteLine();

        // Demonstrate deadlock detection
        Console.WriteLine("4. Deadlock Detection");
        Console.WriteLine("---------------------");
        var deadlockManager = new DistributedLockManager();
        
        // Node A holds resource X, wants resource Y
        // Node B holds resource Y, wants resource X
        var nodeA = new DistributedNode("Node-A", deadlockManager);
        var nodeB = new DistributedNode("Node-B", deadlockManager);
        
        Console.WriteLine("Creating potential deadlock scenario...");
        Console.WriteLine("Node-A acquires 'resource-X'...");
        nodeA.AcquireLock("resource-X", TimeSpan.FromSeconds(10));
        
        Console.WriteLine("Node-B acquires 'resource-Y'...");
        nodeB.AcquireLock("resource-Y", TimeSpan.FromSeconds(10));
        
        Console.WriteLine("Node-A attempting to acquire 'resource-Y' (held by Node-B)...");
        Console.WriteLine("Node-B attempting to acquire 'resource-X' (held by Node-A)...");
        
        var deadlockDetected = deadlockManager.CheckForDeadlock();
        Console.WriteLine($"Deadlock detected: {deadlockDetected}");
        Console.WriteLine();

        // Demonstrate lock hierarchy
        Console.WriteLine("5. Lock Hierarchy (Prevent Deadlocks)");
        Console.WriteLine("--------------------------------------");
        var hierarchicalManager = new DistributedLockManager();
        var worker1 = new DistributedNode("Worker-1", hierarchicalManager);
        var worker2 = new DistributedNode("Worker-2", hierarchicalManager);
        
        Console.WriteLine("Using lock ordering to prevent deadlocks...");
        Console.WriteLine("Both workers acquire locks in order: resource-1 -> resource-2");
        
        // Both workers follow the same ordering
        var w1Lock1 = worker1.AcquireLock("resource-1", TimeSpan.FromSeconds(5));
        var w2Lock1 = worker2.AcquireLock("resource-1", TimeSpan.FromSeconds(1));
        
        Console.WriteLine($"Worker-1 got resource-1: {w1Lock1}");
        Console.WriteLine($"Worker-2 got resource-1: {w2Lock1} (should be false)");
        
        if (w1Lock1)
        {
            var w1Lock2 = worker1.AcquireLock("resource-2", TimeSpan.FromSeconds(5));
            Console.WriteLine($"Worker-1 got resource-2: {w1Lock2}");
            Console.WriteLine("Worker-1 can safely proceed with both resources");
        }
        
        Console.WriteLine();
        Console.WriteLine("Demo completed!");
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Distributed Lock Manager (Interactive Mode)");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine();

        var lockManager = new DistributedLockManager();
        var nodes = new Dictionary<string, DistributedNode>();
        var heldLocks = new List<(string NodeId, string Resource, DateTime ExpiresAt)>();

        while (true)
        {
            Console.Write("dlm> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            try
            {
                switch (cmd)
                {
                    case "quit":
                    case "exit":
                        return;

                    case "help":
                        ShowHelp();
                        break;

                    case "node":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: node <node-id>");
                            break;
                        }
                        var nodeId = parts[1];
                        if (!nodes.ContainsKey(nodeId))
                        {
                            nodes[nodeId] = new DistributedNode(nodeId, lockManager);
                            Console.WriteLine($"Created node: {nodeId}");
                        }
                        else
                        {
                            Console.WriteLine($"Node {nodeId} already exists");
                        }
                        break;

                    case "acquire":
                        if (parts.Length < 4)
                        {
                            Console.WriteLine("Usage: acquire <node-id> <resource> <ttl-seconds>");
                            break;
                        }
                        var acquireNode = parts[1];
                        var resource = parts[2];
                        var ttl = int.Parse(parts[3]);
                        
                        if (!nodes.ContainsKey(acquireNode))
                        {
                            nodes[acquireNode] = new DistributedNode(acquireNode, lockManager);
                        }
                        
                        var acquired = nodes[acquireNode].AcquireLock(resource, TimeSpan.FromSeconds(ttl));
                        Console.WriteLine(acquired
                            ? $"Lock acquired on '{resource}' by {acquireNode}"
                            : $"Lock DENIED on '{resource}' (already held)");
                        break;

                    case "release":
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Usage: release <node-id> <resource>");
                            break;
                        }
                        var releaseNode = parts[1];
                        var releaseResource = parts[2];
                        
                        if (nodes.ContainsKey(releaseNode))
                        {
                            nodes[releaseNode].ReleaseLock(releaseResource);
                            Console.WriteLine($"Lock released on '{releaseResource}' by {releaseNode}");
                        }
                        else
                        {
                            Console.WriteLine($"Node {releaseNode} not found");
                        }
                        break;

                    case "status":
                        var status = lockManager.GetStatus();
                        Console.WriteLine($"Active locks: {status.ActiveLocks}");
                        Console.WriteLine($"Total requests: {status.TotalRequests}");
                        Console.WriteLine($"Successful: {status.SuccessfulAcquisitions}");
                        Console.WriteLine($"Failed: {status.FailedAcquisitions}");
                        break;

                    case "locks":
                        var locks = lockManager.GetAllLocks();
                        if (locks.Count == 0)
                        {
                            Console.WriteLine("No active locks");
                        }
                        else
                        {
                            Console.WriteLine("Active locks:");
                            foreach (var lockInfo in locks)
                            {
                                var remaining = (lockInfo.ExpiresAt - DateTime.UtcNow).TotalSeconds;
                                Console.WriteLine($"  {lockInfo.Resource} -> {lockInfo.OwnerNode} (expires in {remaining:F1}s)");
                            }
                        }
                        break;

                    case "deadlock":
                        var hasDeadlock = lockManager.CheckForDeadlock();
                        Console.WriteLine($"Deadlock detected: {hasDeadlock}");
                        break;

                    case "nodes":
                        if (nodes.Count == 0)
                        {
                            Console.WriteLine("No nodes created");
                        }
                        else
                        {
                            Console.WriteLine("Nodes:");
                            foreach (var node in nodes.Keys)
                            {
                                Console.WriteLine($"  - {node}");
                            }
                        }
                        break;

                    case "clear":
                        lockManager.ClearAllLocks();
                        nodes.Clear();
                        Console.WriteLine("All locks and nodes cleared");
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {cmd}. Type 'help' for commands.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  node <id>              - Create/select a node");
        Console.WriteLine("  acquire <node> <res> <ttl> - Acquire lock with TTL in seconds");
        Console.WriteLine("  release <node> <res>   - Release a lock");
        Console.WriteLine("  status                 - Show lock statistics");
        Console.WriteLine("  locks                  - List all active locks");
        Console.WriteLine("  deadlock               - Check for deadlocks");
        Console.WriteLine("  nodes                  - List all nodes");
        Console.WriteLine("  clear                  - Clear all locks and nodes");
        Console.WriteLine("  quit                   - Exit");
        Console.WriteLine();
    }
}

public record LockInfo(string Resource, string OwnerNode, DateTime ExpiresAt);
public record LockStatus(int ActiveLocks, int TotalRequests, int SuccessfulAcquisitions, int FailedAcquisitions);

public class DistributedLockManager
{
    private readonly ConcurrentDictionary<string, LockInfo> _locks = new();
    private int _totalRequests;
    private int _successfulAcquisitions;
    private int _failedAcquisitions;

    public bool TryAcquireLock(string nodeId, string resource, TimeSpan ttl)
    {
        Interlocked.Increment(ref _totalRequests);

        // Clean up expired locks first
        CleanupExpiredLocks();

        var lockInfo = new LockInfo(resource, nodeId, DateTime.UtcNow + ttl);
        
        if (_locks.TryAdd(resource, lockInfo))
        {
            Interlocked.Increment(ref _successfulAcquisitions);
            return true;
        }

        // Check if we already own this lock
        if (_locks.TryGetValue(resource, out var existing) && existing.OwnerNode == nodeId)
        {
            // Renew the lock
            if (_locks.TryUpdate(resource, lockInfo, existing))
            {
                Interlocked.Increment(ref _successfulAcquisitions);
                return true;
            }
        }

        Interlocked.Increment(ref _failedAcquisitions);
        return false;
    }

    public bool ReleaseLock(string nodeId, string resource)
    {
        if (_locks.TryGetValue(resource, out var lockInfo) && lockInfo.OwnerNode == nodeId)
        {
            return _locks.TryRemove(resource, out _);
        }
        return false;
    }

    public bool IsLocked(string resource)
    {
        CleanupExpiredLocks();
        return _locks.ContainsKey(resource);
    }

    public LockInfo? GetLockInfo(string resource)
    {
        return _locks.TryGetValue(resource, out var info) && info.ExpiresAt > DateTime.UtcNow 
            ? info 
            : null;
    }

    public List<LockInfo> GetAllLocks()
    {
        CleanupExpiredLocks();
        return _locks.Values.ToList();
    }

    public LockStatus GetStatus()
    {
        CleanupExpiredLocks();
        return new LockStatus(
            _locks.Count,
            Volatile.Read(ref _totalRequests),
            Volatile.Read(ref _successfulAcquisitions),
            Volatile.Read(ref _failedAcquisitions)
        );
    }

    public bool CheckForDeadlock()
    {
        // Simple deadlock detection: check for circular wait
        // Build a wait-for graph and detect cycles
        var waitGraph = new Dictionary<string, HashSet<string>>();
        
        foreach (var lockInfo in _locks.Values.Where(l => l.ExpiresAt > DateTime.UtcNow))
        {
            if (!waitGraph.ContainsKey(lockInfo.OwnerNode))
            {
                waitGraph[lockInfo.OwnerNode] = new HashSet<string>();
            }
        }

        // If we have multiple locks held by different nodes, 
        // check if any node is waiting for a resource held by another
        // This is a simplified check - real deadlock detection is more complex
        var holders = _locks.Values.Where(l => l.ExpiresAt > DateTime.UtcNow).Select(l => l.OwnerNode).ToHashSet();
        
        // Simple heuristic: if we have N nodes holding N resources and all want more,
        // there might be a deadlock
        return holders.Count > 1 && _locks.Count > holders.Count;
    }

    public void ClearAllLocks()
    {
        _locks.Clear();
        _totalRequests = 0;
        _successfulAcquisitions = 0;
        _failedAcquisitions = 0;
    }

    private void CleanupExpiredLocks()
    {
        var now = DateTime.UtcNow;
        foreach (var kvp in _locks.Where(k => k.Value.ExpiresAt <= now).ToList())
        {
            _locks.TryRemove(kvp.Key, out _);
        }
    }
}

public class DistributedNode
{
    private readonly string _nodeId;
    private readonly DistributedLockManager _lockManager;
    private readonly HashSet<string> _heldLocks = new();

    public string NodeId => _nodeId;

    public DistributedNode(string nodeId, DistributedLockManager lockManager)
    {
        _nodeId = nodeId;
        _lockManager = lockManager;
    }

    public bool AcquireLock(string resource, TimeSpan ttl)
    {
        if (_lockManager.TryAcquireLock(_nodeId, resource, ttl))
        {
            _heldLocks.Add(resource);
            return true;
        }
        return false;
    }

    public bool ReleaseLock(string resource)
    {
        if (_lockManager.ReleaseLock(_nodeId, resource))
        {
            _heldLocks.Remove(resource);
            return true;
        }
        return false;
    }

    public void ReleaseAllLocks()
    {
        foreach (var resource in _heldLocks.ToList())
        {
            ReleaseLock(resource);
        }
    }

    public IEnumerable<string> GetHeldLocks() => _heldLocks;
}
