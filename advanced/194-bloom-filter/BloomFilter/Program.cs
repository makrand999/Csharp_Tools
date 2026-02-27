using System.Collections;
using System.Security.Cryptography;
using System.Text;

namespace BloomFilter;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Bloom Filter - Space-efficient probabilistic set membership test");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project BloomFilter.csproj -- demo");
            Console.WriteLine("  dotnet run --project BloomFilter.csproj -- interactive");
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
        Console.WriteLine("=== Bloom Filter Demo ===\n");

        // Create bloom filter optimized for 100 elements with 1% false positive rate
        var bloomFilter = new BloomFilter(expectedElements: 100, falsePositiveRate: 0.01);

        Console.WriteLine($"Bloom Filter Configuration:");
        Console.WriteLine($"  Expected elements: 100");
        Console.WriteLine($"  Target false positive rate: 1%");
        Console.WriteLine($"  Bit array size: {bloomFilter.BitSize} bits");
        Console.WriteLine($"  Hash functions: {bloomFilter.HashFunctionCount}");
        Console.WriteLine();

        // Add some items
        Console.WriteLine("Adding items to bloom filter:");
        var items = new[] { "apple", "banana", "cherry", "date", "elderberry", "fig", "grape", "honeydew" };
        foreach (var item in items)
        {
            bloomFilter.Add(item);
            Console.WriteLine($"  Added: {item}");
        }
        Console.WriteLine();

        // Test membership
        Console.WriteLine("Testing membership:");
        var testItems = new[] { "apple", "banana", "kiwi", "mango", "orange", "grape" };
        foreach (var item in testItems)
        {
            var result = bloomFilter.Contains(item);
            var status = result ? "Probably in set" : "Definitely NOT in set";
            Console.WriteLine($"  {item,-12} => {status}");
        }
        Console.WriteLine();

        // Statistics
        Console.WriteLine($"Statistics:");
        Console.WriteLine($"  Items added: {bloomFilter.ItemsAdded}");
        Console.WriteLine($"  Bits set: {bloomFilter.BitsSet}");
        Console.WriteLine($"  Fill ratio: {bloomFilter.FillRatio:P2}");
        Console.WriteLine($"  Estimated false positive rate: {bloomFilter.EstimatedFalsePositiveRate:P4}");
        Console.WriteLine();

        // Demonstrate false positive possibility
        Console.WriteLine("False Positive Demonstration:");
        Console.WriteLine("Adding 50 random words to the filter...");
        var random = new Random(42);
        for (int i = 0; i < 50; i++)
        {
            bloomFilter.Add($"item_{random.Next(1000)}");
        }
        Console.WriteLine($"  Items added: {bloomFilter.ItemsAdded}");
        Console.WriteLine($"  Fill ratio: {bloomFilter.FillRatio:P2}");
        
        // Test some items that were never added
        Console.WriteLine("\nTesting items NOT added (watch for false positives):");
        var notAddedItems = new[] { "zebra", "xylophone", "quasar", "jupiter", "nebula" };
        foreach (var item in notAddedItems)
        {
            var result = bloomFilter.Contains(item);
            var status = result ? "PROBABLY in set (FALSE POSITIVE!)" : "Definitely NOT in set";
            Console.WriteLine($"  {item,-12} => {status}");
        }
        Console.WriteLine();

        // Use case: Password blacklist
        Console.WriteLine("Use Case: Password Blacklist Check");
        var blacklistFilter = new BloomFilter(1000, 0.001);
        var commonPasswords = new[] { "password", "123456", "qwerty", "admin", "letmein", "welcome", "monkey", "dragon" };
        foreach (var pwd in commonPasswords)
        {
            blacklistFilter.Add(pwd);
        }
        
        var testPasswords = new[] { "password", "MyStr0ng!Pass#2024", "qwerty", "X#9kL@mP$vQ2" };
        foreach (var pwd in testPasswords)
        {
            var isBlacklisted = blacklistFilter.Contains(pwd);
            var status = isBlacklisted ? "REJECTED (in blacklist)" : "OK (not in blacklist)";
            Console.WriteLine($"  {pwd,-25} => {status}");
        }
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Bloom Filter (Interactive Mode)");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine();

        var bloomFilter = new BloomFilter(1000, 0.01);
        ShowStats(bloomFilter);

        while (true)
        {
            Console.Write("bf> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var parts = input.Split(' ', 2, StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            switch (cmd)
            {
                case "quit":
                case "exit":
                    return;

                case "help":
                    ShowHelp();
                    break;

                case "add":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: add <item>");
                        break;
                    }
                    bloomFilter.Add(parts[1]);
                    Console.WriteLine($"Added '{parts[1]}'");
                    ShowStats(bloomFilter);
                    break;

                case "contains":
                case "check":
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: contains <item>");
                        break;
                    }
                    var result = bloomFilter.Contains(parts[1]);
                    Console.WriteLine(result 
                        ? $"'{parts[1]}' is PROBABLY in the set" 
                        : $"'{parts[1]}' is DEFINITELY NOT in the set");
                    break;

                case "stats":
                    ShowStats(bloomFilter);
                    break;

                case "info":
                    Console.WriteLine($"Bit array size: {bloomFilter.BitSize} bits");
                    Console.WriteLine($"Hash functions: {bloomFilter.HashFunctionCount}");
                    break;

                default:
                    Console.WriteLine($"Unknown command: {cmd}. Type 'help' for commands.");
                    break;
            }
        }
    }

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  add <item>     - Add item to filter");
        Console.WriteLine("  contains <item>- Check if item exists");
        Console.WriteLine("  stats          - Show statistics");
        Console.WriteLine("  info           - Show configuration");
        Console.WriteLine("  quit           - Exit");
        Console.WriteLine();
    }

    private static void ShowStats(BloomFilter filter)
    {
        Console.WriteLine($"  Items: {filter.ItemsAdded}, Bits set: {filter.BitsSet}, Fill: {filter.FillRatio:P2}");
    }
}

public class BloomFilter
{
    private readonly BitArray _bitArray;
    private readonly int _hashFunctionCount;
    private int _itemsAdded;

    public int BitSize => _bitArray.Length;
    public int HashFunctionCount => _hashFunctionCount;
    public int ItemsAdded => _itemsAdded;
    public int BitsSet => _bitArray.Cast<bool>().Count(b => b);
    public double FillRatio => (double)BitsSet / BitSize;

    public BloomFilter(int expectedElements, double falsePositiveRate)
    {
        // Calculate optimal bit array size: m = -n * ln(p) / (ln(2)^2)
        var bitSize = (int)Math.Ceiling(-expectedElements * Math.Log(falsePositiveRate) / (Math.Log(2) * Math.Log(2)));
        
        // Calculate optimal number of hash functions: k = m/n * ln(2)
        _hashFunctionCount = (int)Math.Ceiling(bitSize / (double)expectedElements * Math.Log(2));
        _hashFunctionCount = Math.Max(1, _hashFunctionCount);

        _bitArray = new BitArray(bitSize);
        _itemsAdded = 0;
    }

    public void Add(string item)
    {
        var hashValues = GetHashValues(item);
        foreach (var hash in hashValues)
        {
            _bitArray[hash] = true;
        }
        _itemsAdded++;
    }

    public bool Contains(string item)
    {
        var hashValues = GetHashValues(item);
        foreach (var hash in hashValues)
        {
            if (!_bitArray[hash])
                return false;
        }
        return true;
    }

    public double EstimatedFalsePositiveRate()
    {
        // p = (1 - e^(-kn/m))^k
        var k = _hashFunctionCount;
        var n = _itemsAdded;
        var m = BitSize;
        
        if (n == 0) return 0;
        
        var exponent = -k * n / (double)m;
        var probability = Math.Pow(1 - Math.Exp(exponent), k);
        return probability;
    }

    private IEnumerable<int> GetHashValues(string item)
    {
        var hash1 = Hash(item, 0);
        var hash2 = Hash(item, 1);

        for (int i = 0; i < _hashFunctionCount; i++)
        {
            // Use double hashing: h(i) = h1 + i * h2
            var combinedHash = (hash1 + i * hash2) % _bitArray.Length;
            if (combinedHash < 0) combinedHash += _bitArray.Length;
            yield return combinedHash;
        }
    }

    private int Hash(string item, int seed)
    {
        using var md5 = MD5.Create();
        var bytes = Encoding.UTF8.GetBytes(item + seed);
        var hash = md5.ComputeHash(bytes);
        
        // Convert first 4 bytes to int
        return BitConverter.ToInt32(hash, 0);
    }
}
