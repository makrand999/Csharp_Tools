// Program 19: HashSets - Demonstrates HashSet<T> operations and set theory
// Topics: Unique collections, Add, Remove, Set operations (Union, Intersect, Except)

Console.WriteLine("=== HashSet Basics (Unique Collections) ===\n");

// Create a HashSet of strings (representing unique tags)
var tags = new HashSet<string> { "csharp", "dotnet", "programming" };

Console.WriteLine("Initial tags:");
foreach (var tag in tags)
{
    Console.WriteLine($"  # {tag}");
}

// Add items - duplicates are automatically ignored
Console.WriteLine("\n--- Adding Items ---");
Console.WriteLine($"Adding 'tutorial': {tags.Add("tutorial")}");
Console.WriteLine($"Adding 'csharp' (duplicate): {tags.Add("csharp")}");
Console.WriteLine($"Adding 'beginner': {tags.Add("beginner")}");

Console.WriteLine($"\nCurrent tags (Count: {tags.Count}):");
foreach (var tag in tags)
{
    Console.WriteLine($"  # {tag}");
}

// Remove items
Console.WriteLine("\n--- Removing Items ---");
Console.WriteLine($"Remove 'tutorial': {tags.Remove("tutorial")}");
Console.WriteLine($"Remove 'nonexistent': {tags.Remove("nonexistent")}");

// Contains check
Console.WriteLine("\n--- Contains Check ---");
Console.WriteLine($"Contains 'dotnet': {tags.Contains("dotnet")}");
Console.WriteLine($"Contains 'java': {tags.Contains("java")}");

// HashSet of integers
Console.WriteLine("\n=== HashSet of Integers ===\n");

var numbers = new HashSet<int>();

Console.WriteLine("--- Adding Numbers (Duplicates Ignored) ---");
int[] inputNumbers = { 1, 2, 3, 2, 4, 3, 5, 1 };
foreach (var num in inputNumbers)
{
    bool added = numbers.Add(num);
    Console.WriteLine($"  Added {num}: {(added ? "added" : "duplicate - ignored")}");
}

Console.WriteLine($"\nUnique numbers (Count: {numbers.Count}):");
Console.WriteLine($"  [{string.Join(", ", numbers.OrderBy(n => n))}]");

// Set operations
Console.WriteLine("\n=== Set Operations ===\n");

var setA = new HashSet<int> { 1, 2, 3, 4, 5 };
var setB = new HashSet<int> { 4, 5, 6, 7, 8 };

Console.WriteLine($"Set A: [{string.Join(", ", setA)}]");
Console.WriteLine($"Set B: [{string.Join(", ", setB)}]");

// Union
var union = new HashSet<int>(setA);
union.UnionWith(setB);
Console.WriteLine($"\nA Union B: [{string.Join(", ", union)}]");

// Intersection
var intersection = new HashSet<int>(setA);
intersection.IntersectWith(setB);
Console.WriteLine($"A Intersect B: [{string.Join(", ", intersection)}]");

// Except (difference)
var except = new HashSet<int>(setA);
except.ExceptWith(setB);
Console.WriteLine($"A Except B: [{string.Join(", ", except)}]");

// Symmetric Except (elements in either set, but not both)
var symmetricExcept = new HashSet<int>(setA);
symmetricExcept.SymmetricExceptWith(setB);
Console.WriteLine($"A SymmetricExcept B: [{string.Join(", ", symmetricExcept)}]");

// Subset and Superset checks
Console.WriteLine("\n=== Subset/Superset Checks ===\n");

var smallSet = new HashSet<int> { 1, 2 };
var largeSet = new HashSet<int> { 1, 2, 3, 4, 5 };

Console.WriteLine($"Small Set: [{string.Join(", ", smallSet)}]");
Console.WriteLine($"Large Set: [{string.Join(", ", largeSet)}]");
Console.WriteLine($"\nSmall Set is subset of Large Set: {smallSet.IsSubsetOf(largeSet)}");
Console.WriteLine($"Large Set is superset of Small Set: {largeSet.IsSupersetOf(smallSet)}");
Console.WriteLine($"Large Set is subset of Small Set: {largeSet.IsSubsetOf(smallSet)}");

// Overlaps check
Console.WriteLine("\n=== Overlaps Check ===\n");

var overlappingSet = new HashSet<int> { 3, 4, 9, 10 };
var nonOverlappingSet = new HashSet<int> { 100, 200, 300 };

Console.WriteLine($"Set A: [{string.Join(", ", setA)}]");
Console.WriteLine($"Overlapping Set: [{string.Join(", ", overlappingSet)}]");
Console.WriteLine($"Non-Overlapping Set: [{string.Join(", ", nonOverlappingSet)}]");
Console.WriteLine($"\nSet A overlaps with Overlapping Set: {setA.Overlaps(overlappingSet)}");
Console.WriteLine($"Set A overlaps with Non-Overlapping Set: {setA.Overlaps(nonOverlappingSet)}");

// Real-world example: Removing duplicates from a list
Console.WriteLine("\n=== Real-World: Remove Duplicates ===\n");

var emails = new List<string>
{
    "alice@example.com",
    "bob@example.com",
    "alice@example.com",  // duplicate
    "charlie@example.com",
    "bob@example.com",    // duplicate
    "diana@example.com"
};

Console.WriteLine("Original email list (with duplicates):");
Console.WriteLine($"  Count: {emails.Count}");
foreach (var email in emails)
{
    Console.WriteLine($"  → {email}");
}

var uniqueEmails = new HashSet<string>(emails);

Console.WriteLine("\nUnique emails (duplicates removed):");
Console.WriteLine($"  Count: {uniqueEmails.Count}");
foreach (var email in uniqueEmails)
{
    Console.WriteLine($"  → {email}");
}

Console.WriteLine("\n=== Program Complete ===");
