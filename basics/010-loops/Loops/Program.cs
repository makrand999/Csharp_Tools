// Program #10: Loops - For, while, foreach
// Demonstrates: for, while, do-while, foreach, nested loops, loop control

Console.WriteLine("=== For Loop ===");

// Basic for loop
Console.WriteLine("Counting from 1 to 5:");
for (int i = 1; i <= 5; i++)
{
    Console.Write($"{i} ");
}
Console.WriteLine();

// For loop with decrement
Console.WriteLine("\nCountdown from 5 to 1:");
for (int i = 5; i >= 1; i--)
{
    Console.Write($"{i} ");
}
Console.WriteLine();

// For loop with step
Console.WriteLine("\nEven numbers from 2 to 10:");
for (int i = 2; i <= 10; i += 2)
{
    Console.Write($"{i} ");
}
Console.WriteLine();

// For loop to iterate array
int[] numbers = { 10, 20, 30, 40, 50 };
Console.WriteLine("\nArray elements:");
for (int i = 0; i < numbers.Length; i++)
{
    Console.Write($"numbers[{i}] = {numbers[i]}  ");
}
Console.WriteLine();

Console.WriteLine("\n=== While Loop ===");

// Basic while loop
int counter = 1;
Console.WriteLine("While loop counting 1 to 5:");
while (counter <= 5)
{
    Console.Write($"{counter} ");
    counter++;
}
Console.WriteLine();

// While loop with user input simulation
Console.WriteLine("\nWhile loop with condition:");
int sum = 0;
int limit = 15;
int current = 1;

while (sum < limit)
{
    sum += current;
    Console.Write($"{current} ");
    current++;
}
Console.WriteLine($"\nSum reached: {sum} (limit was {limit})");

Console.WriteLine("\n=== Do-While Loop ===");

// Do-while executes at least once
int doCounter = 5;
Console.WriteLine("Do-while loop (starts at 5, condition < 5):");
do
{
    Console.WriteLine($"Value: {doCounter}");
    doCounter++;
} while (doCounter < 5);

Console.WriteLine("Note: Executed once even though condition was false!");

// Practical do-while example
Console.WriteLine("\nDo-while for input validation simulation:");
int attempts = 0;
int maxAttempts = 3;
bool success = false;

do
{
    attempts++;
    Console.WriteLine($"Attempt {attempts} of {maxAttempts}");
    
    // Simulate success on second attempt
    if (attempts == 2)
    {
        success = true;
        Console.WriteLine("Success!");
    }
} while (!success && attempts < maxAttempts);

Console.WriteLine("\n=== Foreach Loop ===");

// Foreach with array
string[] fruits = { "Apple", "Banana", "Cherry", "Date" };
Console.WriteLine("Fruits in array:");
foreach (string fruit in fruits)
{
    Console.Write($"{fruit} ");
}
Console.WriteLine();

// Foreach with list
List<int> squares = new List<int> { 1, 4, 9, 16, 25 };
Console.WriteLine("\nSquares in list:");
foreach (int square in squares)
{
    Console.Write($"{square} ");
}
Console.WriteLine();

// Foreach with index (C# 8+)
Console.WriteLine("\nFruits with index:");
int index = 0;
foreach (string fruit in fruits)
{
    Console.WriteLine($"  [{index}] {fruit}");
    index++;
}

Console.WriteLine("\n=== Nested Loops ===");

// Multiplication table
Console.WriteLine("Multiplication Table (1-5):");
for (int i = 1; i <= 5; i++)
{
    for (int j = 1; j <= 5; j++)
    {
        Console.Write($"{i * j,4}");
    }
    Console.WriteLine();
}

// Pattern printing
Console.WriteLine("\nPyramid Pattern:");
int rows = 5;
for (int i = 1; i <= rows; i++)
{
    // Print spaces
    for (int j = 1; j <= rows - i; j++)
    {
        Console.Write(" ");
    }
    // Print stars
    for (int k = 1; k <= (2 * i - 1); k++)
    {
        Console.Write("*");
    }
    Console.WriteLine();
}

Console.WriteLine("\n=== Loop Control: Break and Continue ===");

// Break example
Console.WriteLine("Break at 3:");
for (int i = 1; i <= 5; i++)
{
    if (i == 3)
    {
        Console.WriteLine("Break!");
        break;
    }
    Console.Write($"{i} ");
}

// Continue example
Console.WriteLine("\nContinue (skip 3):");
for (int i = 1; i <= 5; i++)
{
    if (i == 3)
    {
        Console.Write("skip ");
        continue;
    }
    Console.Write($"{i} ");
}
Console.WriteLine();

// Nested loop with labeled break
Console.WriteLine("\nNested loop with break:");
bool found = false;
for (int i = 1; i <= 3 && !found; i++)
{
    for (int j = 1; j <= 3; j++)
    {
        if (i * j == 6)
        {
            Console.WriteLine($"Found: {i} * {j} = 6");
            found = true;
            break;
        }
    }
}

Console.WriteLine("\n=== Infinite Loop (with break) ===");
Console.WriteLine("Counting to 3 with infinite loop:");
int count = 1;
while (true)
{
    Console.Write($"{count} ");
    if (count >= 3)
    {
        break;
    }
    count++;
}
Console.WriteLine("\nExited infinite loop with break!");
