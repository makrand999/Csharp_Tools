// Program #12: Methods - Demonstrates method definition, parameters, return values, and overloading

Console.WriteLine("=== Methods in C# ===\n");

// Method with no parameters and no return value
SayHello();

// Method with parameters and no return value
GreetPerson("Alice", 25);

// Method with return value
int result = Add(10, 20);
Console.WriteLine($"\n10 + 20 = {result}");

// Method with multiple return values (C# 7+ tuple syntax)
var (sum, product) = CalculateSumAndProduct(5, 3);
Console.WriteLine($"\n5 and 3:");
Console.WriteLine($"  Sum: {sum}");
Console.WriteLine($"  Product: {product}");

// Method with optional parameters
Console.WriteLine($"\nPower(2, 10) = {Power(2, 10)}");
Console.WriteLine($"Power(3) = {Power(3)}");  // Uses default exponent

// Method with named arguments
Console.WriteLine($"\nNamed arguments:");
Console.WriteLine(CreateMessage(priority: "High", message: "System update", sender: "Admin"));

// Method overloading demonstration (using a helper class)
Console.WriteLine($"\n=== Method Overloading ===");
Console.WriteLine($"Multiply(3, 4) = {MathHelper.Multiply(3, 4)}");           // int * int
Console.WriteLine($"Multiply(2.5, 4.0) = {MathHelper.Multiply(2.5, 4.0)}");   // double * double
Console.WriteLine($"Multiply(2, 3, 4) = {MathHelper.Multiply(2, 3, 4)}");     // Three integers

// Ref and Out parameters
Console.WriteLine($"\n=== Ref and Out Parameters ===");
int original = 100;
Console.WriteLine($"Original value: {original}");
DoubleByRef(ref original);
Console.WriteLine($"After DoubleByRef: {original}");

if (TryParseInt("42", out int parsed))
{
    Console.WriteLine($"\nParsed '42' successfully: {parsed}");
}
if (!TryParseInt("abc", out int failed))
{
    Console.WriteLine("Parsing 'abc' failed as expected");
}

// Local function (function defined inside another function)
Console.WriteLine($"\n=== Local Functions ===");
Console.WriteLine($"Factorial of 5: {CalculateFactorial(5)}");

Console.WriteLine("\n=== All methods completed ===");

// Method definitions

void SayHello()
{
    Console.WriteLine("Hello from SayHello()!");
}

void GreetPerson(string name, int age)
{
    Console.WriteLine($"Hi {name}, you are {age} years old.");
}

int Add(int a, int b)
{
    return a + b;
}

(int sum, int product) CalculateSumAndProduct(int x, int y)
{
    return (x + y, x * y);
}

int Power(int baseValue, int exponent = 2)
{
    return (int)Math.Pow(baseValue, exponent);
}

string CreateMessage(string message, string sender = "Unknown", string priority = "Normal")
{
    return $"[{priority}] From {sender}: {message}";
}

void DoubleByRef(ref int value)
{
    value *= 2;
}

bool TryParseInt(string input, out int result)
{
    return int.TryParse(input, out result);
}

int CalculateFactorial(int n)
{
    // Local helper function
    int Factorial(int num) => num <= 1 ? 1 : num * Factorial(num - 1);
    
    return Factorial(n);
}

// Helper class for method overloading demonstration
static class MathHelper
{
    public static int Multiply(int a, int b) => a * b;
    public static double Multiply(double a, double b) => a * b;
    public static int Multiply(int a, int b, int c) => a * b * c;
}
