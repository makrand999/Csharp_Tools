// Program #11: Arrays - Demonstrates array declaration, initialization, indexing, and iteration

int[] numbers = { 10, 20, 30, 40, 50 };

Console.WriteLine("=== Array Basics ===\n");

// Display array length
Console.WriteLine($"Array length: {numbers.Length}");

// Access elements by index
Console.WriteLine($"\nFirst element: {numbers[0]}");
Console.WriteLine($"Middle element: {numbers[2]}");
Console.WriteLine($"Last element: {numbers[numbers.Length - 1]}");

// Modify an element
numbers[1] = 25;
Console.WriteLine($"\nAfter modifying index 1: {numbers[1]}");

// Iterate using for loop
Console.WriteLine("\n=== Using for loop ===");
for (int i = 0; i < numbers.Length; i++)
{
    Console.WriteLine($"Index {i}: {numbers[i]}");
}

// Iterate using foreach
Console.WriteLine("\n=== Using foreach ===");
foreach (int num in numbers)
{
    Console.WriteLine($"Value: {num}");
}

// Multi-dimensional array
Console.WriteLine("\n=== 2D Array ===");
int[,] matrix = {
    { 1, 2, 3 },
    { 4, 5, 6 },
    { 7, 8, 9 }
};

for (int row = 0; row < matrix.GetLength(0); row++)
{
    for (int col = 0; col < matrix.GetLength(1); col++)
    {
        Console.Write($"{matrix[row, col]} ");
    }
    Console.WriteLine();
}

// Array methods
Console.WriteLine("\n=== Array Methods ===");
int[] scores = { 85, 92, 78, 96, 88 };
Console.WriteLine($"Scores: [{string.Join(", ", scores)}]");
Console.WriteLine($"Sum: {scores.Sum()}");
Console.WriteLine($"Average: {scores.Average()}");
Console.WriteLine($"Max: {scores.Max()}");
Console.WriteLine($"Min: {scores.Min()}");

// Search in array
int searchValue = 88;
int index = Array.IndexOf(scores, searchValue);
Console.WriteLine($"\nIndex of {searchValue}: {(index >= 0 ? index.ToString() : "Not found")}");

// Sort array
Array.Sort(scores);
Console.WriteLine($"\nSorted scores: [{string.Join(", ", scores)}]");

// Reverse array
Array.Reverse(scores);
Console.WriteLine($"Reversed scores: [{string.Join(", ", scores)}]");
