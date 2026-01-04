// Program #15: Lists - Demonstrates List<T>, adding/removing items, and LINQ basics

Console.WriteLine("=== Lists in C# ===\n");

// Create and initialize a list
List<string> fruits = new List<string> { "Apple", "Banana", "Cherry" };
Console.WriteLine($"=== Initial List ===");
Console.WriteLine($"Fruits: [{string.Join(", ", fruits)}]");
Console.WriteLine($"Count: {fruits.Count}\n");

// Add items
Console.WriteLine($"=== Adding Items ===");
fruits.Add("Date");
fruits.Add("Elderberry");
Console.WriteLine($"After Add: [{string.Join(", ", fruits)}]");

// Add range
fruits.AddRange(new[] { "Fig", "Grape" });
Console.WriteLine($"After AddRange: [{string.Join(", ", fruits)}]\n");

// Access by index
Console.WriteLine($"=== Accessing Items ===");
Console.WriteLine($"First fruit: {fruits[0]}");
Console.WriteLine($"Last fruit: {fruits[fruits.Count - 1]}");

// Modify item
fruits[1] = "Blueberry";
Console.WriteLine($"After modifying index 1: [{string.Join(", ", fruits)}]\n");

// Remove items
Console.WriteLine($"=== Removing Items ===");
fruits.Remove("Apple");
Console.WriteLine($"After Remove('Apple'): [{string.Join(", ", fruits)}]");

fruits.RemoveAt(0);
Console.WriteLine($"After RemoveAt(0): [{string.Join(", ", fruits)}]");

string removed = fruits[2];
fruits.RemoveAt(2);
Console.WriteLine($"Removed '{removed}', remaining: [{string.Join(", ", fruits)}]\n");

// Check if exists
Console.WriteLine($"=== Checking Items ===");
Console.WriteLine($"Contains 'Fig': {fruits.Contains("Fig")}");
Console.WriteLine($"Contains 'Mango': {fruits.Contains("Mango")}");
Console.WriteLine($"Index of 'Fig': {fruits.IndexOf("Fig")}\n");

// Iterate through list
Console.WriteLine($"=== Iterating ===");
Console.WriteLine("Using foreach:");
foreach (var fruit in fruits)
{
    Console.WriteLine($"  - {fruit}");
}

Console.WriteLine("\nUsing for with index:");
for (int i = 0; i < fruits.Count; i++)
{
    Console.WriteLine($"  [{i}] {fruits[i]}");
}
Console.WriteLine();

// List of numbers with LINQ
Console.WriteLine($"=== LINQ with Lists ===");
List<int> numbers = new List<int> { 5, 2, 8, 1, 9, 3, 7, 4, 6 };
Console.WriteLine($"Numbers: [{string.Join(", ", numbers)}]");

// LINQ Where (filter)
var evenNumbers = numbers.Where(n => n % 2 == 0).ToList();
Console.WriteLine($"Even numbers: [{string.Join(", ", evenNumbers)}]");

// LINQ Select (transform)
var squared = numbers.Select(n => n * n).ToList();
Console.WriteLine($"Squared: [{string.Join(", ", squared)}]");

// LINQ OrderBy
var sorted = numbers.OrderBy(n => n).ToList();
Console.WriteLine($"Sorted: [{string.Join(", ", sorted)}]");

// LINQ First/FirstOrDefault
Console.WriteLine($"First > 5: {numbers.First(n => n > 5)}");
Console.WriteLine($"First > 10 (default): {numbers.FirstOrDefault(n => n > 10)}\n");

// List sorting (in-place)
numbers.Sort();
Console.WriteLine($"After Sort(): [{string.Join(", ", numbers)}]");
numbers.Reverse();
Console.WriteLine($"After Reverse(): [{string.Join(", ", numbers)}]\n");

// List of custom objects
Console.WriteLine($"=== List of Objects ===");
var people = new List<Person>
{
    new Person { Name = "Alice", Age = 30 },
    new Person { Name = "Bob", Age = 25 },
    new Person { Name = "Charlie", Age = 35 }
};

Console.WriteLine("People list:");
foreach (var person in people)
{
    Console.WriteLine($"  {person.Name}, {person.Age} years old");
}

// LINQ on objects
var sortedByAge = people.OrderBy(p => p.Age).ToList();
Console.WriteLine("\nSorted by age:");
foreach (var person in sortedByAge)
{
    Console.WriteLine($"  {person.Name}, {person.Age}");
}

// Clear the list
Console.WriteLine($"\n=== Clearing ===");
fruits.Clear();
Console.WriteLine($"After Clear, Count: {fruits.Count}");

Console.WriteLine("\n=== Lists Demo Complete ===");

// Person class for object list demo
class Person
{
    public string Name { get; set; } = string.Empty;
    public int Age { get; set; }
    
    public override string ToString() => $"{Name} ({Age})";
}
