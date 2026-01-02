// Program #7: Variables - Data types and declarations
// Demonstrates: Value types, reference types, var keyword, type conversion

// Value types (stored on stack)
int age = 25;
double price = 19.99;
decimal money = 100.50m;
char grade = 'A';
bool isStudent = true;

// Reference types (stored on heap)
string name = "Alice";
string city = "New York";

// Using var keyword (type inference)
var country = "USA";
var score = 95;
var isActive = false;

// Display all variables
Console.WriteLine("=== Value Types ===");
Console.WriteLine($"Age (int): {age}");
Console.WriteLine($"Price (double): {price}");
Console.WriteLine($"Money (decimal): {money}");
Console.WriteLine($"Grade (char): {grade}");
Console.WriteLine($"Is Student (bool): {isStudent}");

Console.WriteLine("\n=== Reference Types ===");
Console.WriteLine($"Name: {name}");
Console.WriteLine($"City: {city}");

Console.WriteLine("\n=== Type Inference (var) ===");
Console.WriteLine($"Country: {country} (Type: {country.GetType().Name})");
Console.WriteLine($"Score: {score} (Type: {score.GetType().Name})");
Console.WriteLine($"Is Active: {isActive} (Type: {isActive.GetType().Name})");

// Type conversion examples
Console.WriteLine("\n=== Type Conversion ===");

// Implicit conversion (int to double)
int wholeNumber = 42;
double converted = wholeNumber;
Console.WriteLine($"{wholeNumber} (int) → {converted} (double)");

// Explicit conversion (double to int)
double decimalNumber = 9.99;
int truncated = (int)decimalNumber;
Console.WriteLine($"{decimalNumber} (double) → {truncated} (int) [truncated]");

// Parse string to int
string numberString = "123";
int parsed = int.Parse(numberString);
Console.WriteLine($"\"{numberString}\" (string) → {parsed} (int)");

// TryParse for safe conversion
string invalidNumber = "abc";
if (int.TryParse(invalidNumber, out int result))
{
    Console.WriteLine($"Parsed: {result}");
}
else
{
    Console.WriteLine($"Failed to parse \"{invalidNumber}\" - handled gracefully!");
}

// Constants
const double Pi = 3.14159;
const string Greeting = "Hello, Variables!";
Console.WriteLine($"\n=== Constants ===");
Console.WriteLine($"Pi: {Pi}");
Console.WriteLine($"Greeting: {Greeting}");
