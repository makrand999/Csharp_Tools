// Program 16: Dictionaries - Demonstrates Dictionary<TKey, TValue> operations
// Topics: Key-value pairs, adding/removing items, iteration, lookup

var students = new Dictionary<string, int>
{
    ["Alice"] = 95,
    ["Bob"] = 87,
    ["Charlie"] = 92
};

Console.WriteLine("=== Dictionary Basics ===\n");

// Display initial dictionary
Console.WriteLine("Initial students:");
foreach (var student in students)
{
    Console.WriteLine($"  {student.Key}: {student.Value}");
}

// Adding new items
Console.WriteLine("\n--- Adding New Items ---");
students["Diana"] = 88;
students.Add("Eve", 91);
Console.WriteLine($"Added Diana (88) and Eve (91)");

// Accessing values
Console.WriteLine("\n--- Accessing Values ---");
Console.WriteLine($"Bob's score: {students["Bob"]}");
Console.WriteLine($"Charlie's score: {students["Charlie"]}");

// TryGetValue - safe lookup
Console.WriteLine("\n--- Safe Lookup with TryGetValue ---");
if (students.TryGetValue("Alice", out int aliceScore))
{
    Console.WriteLine($"Alice's score: {aliceScore}");
}
if (!students.TryGetValue("Frank", out int frankScore))
{
    Console.WriteLine("Frank not found in dictionary");
}

// Checking existence
Console.WriteLine("\n--- Checking Existence ---");
Console.WriteLine($"Contains key 'Bob': {students.ContainsKey("Bob")}");
Console.WriteLine($"Contains value 92: {students.ContainsValue(92)}");

// Updating values
Console.WriteLine("\n--- Updating Values ---");
students["Bob"] = 90;
Console.WriteLine($"Updated Bob's score to: {students["Bob"]}");

// Removing items
Console.WriteLine("\n--- Removing Items ---");
students.Remove("Charlie");
Console.WriteLine("Removed Charlie from dictionary");

// Iteration methods
Console.WriteLine("\n--- Iteration Methods ---");
Console.WriteLine("Iterating over Key-value pairs:");
foreach (var kvp in students)
{
    Console.WriteLine($"  {kvp.Key}: {kvp.Value}");
}

Console.WriteLine("\nIterating over keys only:");
foreach (var key in students.Keys)
{
    Console.WriteLine($"  Key: {key}");
}

Console.WriteLine("\nIterating over values only:");
foreach (var value in students.Values)
{
    Console.WriteLine($"  Value: {value}");
}

// Dictionary with different types
Console.WriteLine("\n=== Dictionary with Complex Types ===\n");

var productPrices = new Dictionary<int, decimal>
{
    [101] = 29.99m,
    [102] = 49.50m,
    [103] = 15.00m,
    [104] = 99.99m
};

Console.WriteLine("Product Prices:");
foreach (var product in productPrices)
{
    Console.WriteLine($"  Product ID {product.Key}: ${product.Value:F2}");
}

// Calculate average price
var total = productPrices.Values.Sum();
var average = productPrices.Values.Average();
Console.WriteLine($"\nTotal: ${total:F2}");
Console.WriteLine($"Average: ${average:F2}");

// Find most expensive product
var maxPrice = productPrices.Values.Max();
var expensiveProduct = productPrices.First(p => p.Value == maxPrice);
Console.WriteLine($"Most expensive: Product {expensiveProduct.Key} at ${expensiveProduct.Value:F2}");

Console.WriteLine("\n=== Program Complete ===");
