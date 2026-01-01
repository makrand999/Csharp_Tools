// Program #6: Hello World - Basic console output
// Demonstrates: Console.WriteLine, string interpolation, basic program structure

Console.WriteLine("Hello, World!");
Console.WriteLine("Welcome to C# programming!");

// String interpolation example
string language = "C#";
int version = 12;
Console.WriteLine($"You are learning {language} version {version}+");

// Read user input
Console.Write("\nWhat is your name? ");
string? name = Console.ReadLine();

if (!string.IsNullOrEmpty(name))
{
    Console.WriteLine($"\nHello, {name}! Nice to meet you!");
}
else
{
    Console.WriteLine("\nHello, anonymous coder!");
}

Console.WriteLine("\nPress any key to exit...");
Console.ReadKey();
