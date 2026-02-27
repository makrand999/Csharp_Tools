using System.Security.Cryptography;
using System.Text;

namespace MerkleTreeGenerator;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Merkle Tree Generator - Build and verify Merkle trees");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project MerkleTreeGenerator.csproj -- demo");
            Console.WriteLine("  dotnet run --project MerkleTreeGenerator.csproj -- interactive");
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
        Console.WriteLine("=== Merkle Tree Generator Demo ===\n");

        // Demo 1: Simple tree with transactions
        Console.WriteLine("1. Building Merkle Tree from Transactions");
        Console.WriteLine("-----------------------------------------");
        var transactions = new[]
        {
            "TX1: Alice -> Bob: 10 BTC",
            "TX2: Bob -> Charlie: 5 BTC",
            "TX3: Charlie -> Dave: 2 BTC",
            "TX4: Dave -> Eve: 1 BTC"
        };

        var tree = MerkleTree.Build(transactions);
        Console.WriteLine($"Transactions: {transactions.Length}");
        Console.WriteLine($"Merkle Root: {tree.Root}");
        Console.WriteLine();

        // Print tree visualization
        Console.WriteLine("Tree Structure:");
        PrintTree(tree);
        Console.WriteLine();

        // Demo 2: Generate and verify proof
        Console.WriteLine("2. Merkle Proof Generation & Verification");
        Console.WriteLine("------------------------------------------");
        var proof = tree.GenerateProof(transactions[1]);
        Console.WriteLine($"Generating proof for: {transactions[1]}");
        Console.WriteLine($"Proof hashes: {proof.Hashes.Length}");
        foreach (var item in proof.Hashes.Zip(proof.Directions, (h, dir) => (h, dir)))
        {
            Console.WriteLine($"  {item.dir}: {item.h[..16]}...");
        }
        
        var isValid = tree.VerifyProof(transactions[1], proof);
        Console.WriteLine($"Proof valid: {isValid}");
        Console.WriteLine();

        // Demo 3: Detect tampering
        Console.WriteLine("3. Tamper Detection");
        Console.WriteLine("-------------------");
        var tamperedTx = "TX2: Bob -> Charlie: 999 BTC";
        var tamperedProof = tree.GenerateProof(tamperedTx);
        var isTamperedValid = tree.VerifyProof(tamperedTx, tamperedProof);
        Console.WriteLine($"Original: {transactions[1]}");
        Console.WriteLine($"Tampered: {tamperedTx}");
        Console.WriteLine($"Tampered proof valid: {isTamperedValid} (should be False)");
        Console.WriteLine();

        // Demo 4: File integrity
        Console.WriteLine("4. File Integrity Verification");
        Console.WriteLine("-------------------------------");
        
        // Create test files
        var testFiles = new Dictionary<string, string>
        {
            ["file1.txt"] = "Hello, World!",
            ["file2.txt"] = "Merkle trees are awesome!",
            ["file3.txt"] = "Blockchain technology",
            ["file4.txt"] = "Cryptographic hashes"
        };

        var fileHashes = testFiles.Select(kvp => 
            $"{kvp.Key}:{ComputeSha256Hash(kvp.Value)}").ToArray();
        
        var fileTree = MerkleTree.Build(fileHashes);
        Console.WriteLine($"Files: {testFiles.Count}");
        Console.WriteLine($"File Tree Root: {fileTree.Root[..32]}...");
        
        // Verify a file
        var fileToVerify = "file2.txt";
        var fileContent = testFiles[fileToVerify];
        var fileHash = $"{fileToVerify}:{ComputeSha256Hash(fileContent)}";
        var fileProof = fileTree.GenerateProof(fileHash);
        var fileValid = fileTree.VerifyProof(fileHash, fileProof);
        Console.WriteLine($"File '{fileToVerify}' integrity: {(fileValid ? "VERIFIED" : "FAILED")}");
        Console.WriteLine();

        // Demo 5: Git-like commit history
        Console.WriteLine("5. Git-like Commit Chain");
        Console.WriteLine("------------------------");
        var commits = new[]
        {
            "commit1:Initial commit",
            "commit2:Add feature A",
            "commit3:Fix bug in feature A",
            "commit4:Add feature B",
            "commit5:Refactor code",
            "commit6:Update documentation",
            "commit7:Release v1.0",
            "commit8:Hotfix for critical bug"
        };

        var commitTree = MerkleTree.Build(commits);
        Console.WriteLine($"Commits: {commits.Length}");
        Console.WriteLine($"Commit Tree Root: {commitTree.Root[..32]}...");
        Console.WriteLine("Each commit can be verified against the root hash!");
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Merkle Tree Generator (Interactive Mode)");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine();

        var items = new List<string>();
        MerkleTree? tree = null;

        while (true)
        {
            Console.Write("mt> ");
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
                    items.Add(parts[1]);
                    tree = null; // Invalidate tree
                    Console.WriteLine($"Added item. Total items: {items.Count}");
                    break;

                case "build":
                    if (items.Count == 0)
                    {
                        Console.WriteLine("No items to build tree from. Use 'add' first.");
                        break;
                    }
                    tree = MerkleTree.Build(items.ToArray());
                    Console.WriteLine($"Tree built with {items.Count} leaves");
                    Console.WriteLine($"Root hash: {tree.Root}");
                    break;

                case "show":
                    if (tree == null)
                    {
                        Console.WriteLine("No tree built yet. Use 'build' first.");
                        break;
                    }
                    PrintTree(tree);
                    break;

                case "verify":
                    if (tree == null)
                    {
                        Console.WriteLine("No tree built yet. Use 'build' first.");
                        break;
                    }
                    if (parts.Length < 2)
                    {
                        Console.WriteLine("Usage: verify <item>");
                        break;
                    }
                    var proof = tree.GenerateProof(parts[1]);
                    var valid = tree.VerifyProof(parts[1], proof);
                    Console.WriteLine(valid 
                        ? "Item is VALID (included in tree)" 
                        : "Item is INVALID (not in tree or tampered)");
                    break;

                case "list":
                    if (items.Count == 0)
                        Console.WriteLine("No items added yet.");
                    else
                        foreach (var (item, idx) in items.Select((i, n) => (i, n + 1)))
                            Console.WriteLine($"  {idx}. {item}");
                    break;

                case "clear":
                    items.Clear();
                    tree = null;
                    Console.WriteLine("Cleared all items and tree.");
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
        Console.WriteLine("  add <item>     - Add item to the tree");
        Console.WriteLine("  build          - Build Merkle tree from items");
        Console.WriteLine("  show           - Display tree structure");
        Console.WriteLine("  verify <item>  - Verify item is in tree");
        Console.WriteLine("  list           - List all items");
        Console.WriteLine("  clear          - Clear all items");
        Console.WriteLine("  quit           - Exit");
        Console.WriteLine();
    }

    private static void PrintTree(MerkleTree tree)
    {
        var level = 0;
        var nodes = new List<string> { tree.Root };
        
        // Reconstruct levels from leaves
        var currentLevel = tree.Leaves.ToList();
        while (currentLevel.Count > 0)
        {
            Console.WriteLine($"Level {level}:");
            for (int i = 0; i < currentLevel.Count; i++)
            {
                var display = currentLevel[i].Length > 16 
                    ? currentLevel[i][..8] + "..." + currentLevel[i][^8..] 
                    : currentLevel[i];
                Console.WriteLine($"  [{i}] {display}");
            }
            
            // Build next level
            var nextLevel = new List<string>();
            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                if (i + 1 < currentLevel.Count)
                {
                    nextLevel.Add(HashPair(currentLevel[i], currentLevel[i + 1]));
                }
                else
                {
                    nextLevel.Add(currentLevel[i]);
                }
            }
            currentLevel = nextLevel;
            level++;
        }
        Console.WriteLine($"Root: {tree.Root[..16]}...{tree.Root[^16..]}");
    }

    private static string HashPair(string left, string right)
    {
        var combined = left + right;
        return ComputeSha256Hash(combined);
    }

    private static string ComputeSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

public record MerkleProof(string[] Hashes, bool[] Directions);

public record MerkleTree(string Root, string[] Leaves)
{
    public static MerkleTree Build(string[] items)
    {
        if (items.Length == 0)
            throw new ArgumentException("Cannot build Merkle tree from empty set");

        // Hash all items
        var hashes = items.Select(ComputeSha256Hash).ToList();
        
        // Pad to power of 2
        while ((hashes.Count & (hashes.Count - 1)) != 0)
        {
            hashes.Add(hashes[^1]); // Duplicate last hash
        }

        // Build tree bottom-up
        var currentLevel = hashes;
        while (currentLevel.Count > 1)
        {
            var nextLevel = new List<string>();
            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                var combined = currentLevel[i] + currentLevel[i + 1];
                nextLevel.Add(ComputeSha256Hash(combined));
            }
            currentLevel = nextLevel;
        }

        return new MerkleTree(currentLevel[0], hashes.ToArray());
    }

    public MerkleProof GenerateProof(string item)
    {
        var itemHash = ComputeSha256Hash(item);
        var index = Array.IndexOf(Leaves, itemHash);
        
        if (index == -1)
        {
            // Item not in tree, generate proof for first leaf
            index = 0;
        }

        var hashes = new List<string>();
        var directions = new List<bool>(); // true = right, false = left
        
        var currentLevel = Leaves.ToList();
        var currentIndex = index;

        while (currentLevel.Count > 1)
        {
            var siblingIndex = (currentIndex % 2 == 0) ? currentIndex + 1 : currentIndex - 1;
            var isRight = (currentIndex % 2 == 0);
            
            if (siblingIndex < currentLevel.Count)
            {
                hashes.Add(currentLevel[siblingIndex]);
                directions.Add(isRight);
            }

            // Build next level
            var nextLevel = new List<string>();
            for (int i = 0; i < currentLevel.Count; i += 2)
            {
                if (i + 1 < currentLevel.Count)
                {
                    nextLevel.Add(ComputeSha256Hash(currentLevel[i] + currentLevel[i + 1]));
                }
                else
                {
                    nextLevel.Add(currentLevel[i]);
                }
            }

            currentIndex /= 2;
            currentLevel = nextLevel;
        }

        return new MerkleProof(hashes.ToArray(), directions.ToArray());
    }

    public bool VerifyProof(string item, MerkleProof proof)
    {
        var currentHash = ComputeSha256Hash(item);

        for (int i = 0; i < proof.Hashes.Length; i++)
        {
            var siblingHash = proof.Hashes[i];
            var isRight = proof.Directions[i];

            currentHash = isRight 
                ? ComputeSha256Hash(currentHash + siblingHash)
                : ComputeSha256Hash(siblingHash + currentHash);
        }

        return currentHash == Root;
    }

    private static string ComputeSha256Hash(string input)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var hash = sha256.ComputeHash(bytes);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}
