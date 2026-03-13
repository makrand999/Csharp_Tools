using System.Collections.Concurrent;

namespace DistributedCache;

/// <summary>
/// Distributed Cache Simulator - Simulates a distributed cache with multiple nodes.
/// Demonstrates consistent hashing, replication, and failover patterns.
/// </summary>
public class CacheNode
{
    private readonly ConcurrentDictionary<string, CacheEntry> _cache = new();
    public string NodeId { get; }
    public bool IsOnline { get; set; } = true;

    public CacheNode(string nodeId)
    {
        NodeId = nodeId;
    }

    public bool TryGet(string key, out string? value)
    {
        value = null;
        if (!IsOnline) return false;

        if (_cache.TryGetValue(key, out var entry))
        {
            if (entry.ExpiresAt > DateTime.UtcNow)
            {
                value = entry.Value;
                return true;
            }
            _cache.TryRemove(key, out _);
        }
        return false;
    }

    public void Set(string key, string value, TimeSpan? ttl = null)
    {
        if (!IsOnline) return;

        var expiresAt = ttl.HasValue ? DateTime.UtcNow + ttl.Value : DateTime.MaxValue;
        _cache[key] = new CacheEntry(value, expiresAt);
    }

    public bool Remove(string key) => _cache.TryRemove(key, out _);

    public int Count => _cache.Count;

    public IEnumerable<string> Keys => _cache.Keys;
}

public record CacheEntry(string Value, DateTime ExpiresAt);

public class DistributedCache
{
    private readonly List<CacheNode> _nodes = new();
    private readonly int _replicationFactor;

    public DistributedCache(int nodeCount = 3, int replicationFactor = 2)
    {
        _replicationFactor = replicationFactor;
        for (int i = 0; i < nodeCount; i++)
        {
            _nodes.Add(new CacheNode($"node-{i}"));
        }
    }

    public void AddNode(string nodeId)
    {
        _nodes.Add(new CacheNode(nodeId));
        Console.WriteLine($"Added node: {nodeId}");
    }

    public void RemoveNode(string nodeId)
    {
        var node = _nodes.FirstOrDefault(n => n.NodeId == nodeId);
        if (node != null)
        {
            node.IsOnline = false;
            Console.WriteLine($"Removed node: {nodeId}");
        }
    }

    public void Set(string key, string value, TimeSpan? ttl = null)
    {
        var targetNodes = GetTargetNodes(key);
        foreach (var node in targetNodes)
        {
            node.Set(key, value, ttl);
        }
    }

    public string? Get(string key)
    {
        var targetNodes = GetTargetNodes(key);
        
        foreach (var node in targetNodes)
        {
            if (node.TryGet(key, out var value))
            {
                return value;
            }
        }

        // Try other nodes (failover)
        foreach (var node in _nodes.Where(n => n != targetNodes.First() && n.IsOnline))
        {
            if (node.TryGet(key, out var value))
            {
                Console.WriteLine($"  [Failover] Retrieved from {node.NodeId}");
                // Re-replicate to primary nodes
                foreach (var primary in targetNodes.Where(n => n.IsOnline))
                {
                    primary.Set(key, value);
                }
                return value;
            }
        }

        return null;
    }

    public void Remove(string key)
    {
        foreach (var node in _nodes.Where(n => n.IsOnline))
        {
            node.Remove(key);
        }
    }

    public CacheStatistics GetStatistics()
    {
        var onlineNodes = _nodes.Count(n => n.IsOnline);
        var totalEntries = _nodes.Where(n => n.IsOnline).Sum(n => n.Count);
        var avgEntriesPerNode = onlineNodes > 0 ? totalEntries / onlineNodes : 0;

        return new CacheStatistics
        {
            TotalNodes = _nodes.Count,
            OnlineNodes = onlineNodes,
            OfflineNodes = _nodes.Count - onlineNodes,
            TotalEntries = totalEntries,
            AvgEntriesPerNode = avgEntriesPerNode,
            ReplicationFactor = _replicationFactor
        };
    }

    private List<CacheNode> GetTargetNodes(string key)
    {
        // Simple hash-based distribution (in production, use consistent hashing)
        var hash = key.GetHashCode();
        var startIndex = Math.Abs(hash) % _nodes.Count;
        
        var targetNodes = new List<CacheNode>();
        for (int i = 0; i < _replicationFactor && i < _nodes.Count; i++)
        {
            var nodeIndex = (startIndex + i) % _nodes.Count;
            targetNodes.Add(_nodes[nodeIndex]);
        }

        return targetNodes;
    }
}

public record CacheStatistics
{
    public int TotalNodes { get; init; }
    public int OnlineNodes { get; init; }
    public int OfflineNodes { get; init; }
    public int TotalEntries { get; init; }
    public int AvgEntriesPerNode { get; init; }
    public int ReplicationFactor { get; init; }
}

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("=== Distributed Cache Simulator ===\n");
        Console.WriteLine("Simulating distributed caching with replication and failover\n");

        var cache = new DistributedCache(nodeCount: 3, replicationFactor: 2);

        // Set some values
        Console.WriteLine("Setting values in distributed cache...");
        for (int i = 0; i < 10; i++)
        {
            var key = $"user:{i}";
            var value = $"User Data {i}";
            cache.Set(key, value, TimeSpan.FromMinutes(30));
            Console.WriteLine($"  Set: {key} = {value}");
        }

        // Get values
        Console.WriteLine("\nGetting values...");
        for (int i = 0; i < 5; i++)
        {
            var key = $"user:{i}";
            var value = cache.Get(key);
            Console.WriteLine($"  Get: {key} = {value}");
        }

        // Display statistics
        var stats = cache.GetStatistics();
        Console.WriteLine($"\n=== Cache Statistics ===");
        Console.WriteLine($"Total Nodes: {stats.TotalNodes}");
        Console.WriteLine($"Online Nodes: {stats.OnlineNodes}");
        Console.WriteLine($"Total Entries: {stats.TotalEntries}");
        Console.WriteLine($"Avg Entries/Node: {stats.AvgEntriesPerNode}");
        Console.WriteLine($"Replication Factor: {stats.ReplicationFactor}");

        // Simulate node failure
        Console.WriteLine("\n=== Simulating Node Failure ===");
        cache.RemoveNode("node-0");
        
        Console.WriteLine("\nTrying to get values after node failure...");
        for (int i = 0; i < 3; i++)
        {
            var key = $"user:{i}";
            var value = cache.Get(key);
            Console.WriteLine($"  Get: {key} = {(value ?? "null")}");
        }

        // Add new node
        Console.WriteLine("\n=== Adding New Node ===");
        cache.AddNode("node-3");

        // Final statistics
        stats = cache.GetStatistics();
        Console.WriteLine($"\n=== Final Statistics ===");
        Console.WriteLine($"Total Nodes: {stats.TotalNodes}");
        Console.WriteLine($"Online Nodes: {stats.OnlineNodes}");
        Console.WriteLine($"Total Entries: {stats.TotalEntries}");

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
