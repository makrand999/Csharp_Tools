// Program 24: Exception Handling - Demonstrates try-catch-finally, throwing exceptions
// Topics: Try-catch-finally, multiple catch blocks, exception filters, custom exceptions

Console.WriteLine("=== Exception Handling Basics ===\n");

// Basic try-catch
Console.WriteLine("--- Basic Try-Catch ---");
try
{
    int result = Divide(10, 0);
    Console.WriteLine($"Result: {result}");
}
catch (DivideByZeroException ex)
{
    Console.WriteLine($"Error: Cannot divide by zero!");
    Console.WriteLine($"Message: {ex.Message}");
}

// Multiple catch blocks
Console.WriteLine("\n--- Multiple Catch Blocks ---");
try
{
    string? input = null;
    Console.WriteLine($"Length: {input.Length}");
}
catch (NullReferenceException ex)
{
    Console.WriteLine($"NullReferenceException: {ex.Message}");
}
catch (DivideByZeroException ex)
{
    Console.WriteLine($"DivideByZeroException: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"General Exception: {ex.Message}");
}

// Try-catch-finally
Console.WriteLine("\n--- Try-Catch-Finally ---");
try
{
    Console.WriteLine("In try block");
    int number = int.Parse("abc"); // Throws FormatException
}
catch (FormatException ex)
{
    Console.WriteLine($"FormatException: {ex.Message}");
}
finally
{
    Console.WriteLine("Finally block always executes");
}

// Exception with no catch (caught by outer)
Console.WriteLine("\n--- Nested Try-Catch ---");
try
{
    Console.WriteLine("Outer try block");
    try
    {
        Console.WriteLine("  Inner try block");
        throw new InvalidOperationException("Inner exception");
    }
    catch (InvalidOperationException)
    {
        Console.WriteLine("  Inner catch handled the exception");
        throw; // Re-throw to outer
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Outer catch: {ex.Message}");
}

// Exception filters (when clause)
Console.WriteLine("\n--- Exception Filters ---");
try
{
    ThrowSpecificException(5);
}
catch (ArgumentException ex) when (ex.ParamName == "value")
{
    Console.WriteLine($"Caught ArgumentException with param 'value': {ex.Message}");
}
catch (ArgumentException ex) when (ex.ParamName == "input")
{
    Console.WriteLine($"Caught ArgumentException with param 'input': {ex.Message}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Caught ArgumentException (no filter match): {ex.Message}");
}

// Catching specific exception types
Console.WriteLine("\n=== Common Exception Types ===\n");

// FormatException
try
{
    int.Parse("not a number");
}
catch (FormatException)
{
    Console.WriteLine("FormatException: Invalid format for parsing");
}

// OverflowException
try
{
    checked
    {
        int big = int.MaxValue;
        big = big + 1;
    }
}
catch (OverflowException)
{
    Console.WriteLine("OverflowException: Value exceeded type limits");
}

// IndexOutOfRangeException
try
{
    int[] array = { 1, 2, 3 };
    int value = array[10];
}
catch (IndexOutOfRangeException)
{
    Console.WriteLine("IndexOutOfRangeException: Array index out of bounds");
}

// KeyNotFoundException
try
{
    var dict = new Dictionary<string, int>();
    int val = dict["nonexistent"];
}
catch (KeyNotFoundException)
{
    Console.WriteLine("KeyNotFoundException: Key not in dictionary");
}

// InvalidCastException
try
{
    object obj = "hello";
    int num = (int)obj;
}
catch (InvalidCastException)
{
    Console.WriteLine("InvalidCastException: Cannot cast string to int");
}

// Throwing custom exceptions
Console.WriteLine("\n=== Custom Exceptions ===\n");

try
{
    ValidateAge(-5);
}
catch (InvalidAgeException ex)
{
    Console.WriteLine($"InvalidAgeException: {ex.Message}");
    Console.WriteLine($"Provided age: {ex.ProvidedAge}");
}

try
{
    ValidateAge(150);
}
catch (InvalidAgeException ex)
{
    Console.WriteLine($"\nInvalidAgeException: {ex.Message}");
    Console.WriteLine($"Provided age: {ex.ProvidedAge}");
}

// Exception properties
Console.WriteLine("\n=== Exception Properties ===\n");
try
{
    ThrowDetailedException();
}
catch (Exception ex)
{
    Console.WriteLine($"Type: {ex.GetType().Name}");
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"StackTrace: {ex.StackTrace?.Substring(0, Math.Min(100, ex.StackTrace.Length))}...");
    Console.WriteLine($"InnerException: {ex.InnerException?.Message ?? "None"}");
}

// Best practices: Catch specific exceptions first
Console.WriteLine("\n=== Best Practices ===\n");

try
{
    ProcessData(null);
}
catch (ArgumentNullException ex)
{
    Console.WriteLine($"ArgumentNullException caught: {ex.ParamName}");
}
catch (Exception ex)
{
    Console.WriteLine($"General exception: {ex.Message}");
}

Console.WriteLine("\n=== Program Complete ===");

// Helper methods
static int Divide(int a, int b)
{
    return a / b;
}

static void ThrowSpecificException(int value)
{
    if (value < 0)
    {
        throw new ArgumentException("Value cannot be negative", "value");
    }
    else if (value > 10)
    {
        throw new ArgumentException("Value cannot be greater than 10", "input");
    }
    else
    {
        throw new ArgumentException("Some other error");
    }
}

static void ValidateAge(int age)
{
    if (age < 0 || age > 120)
    {
        throw new InvalidAgeException($"Age must be between 0 and 120", age);
    }
}

static void ThrowDetailedException()
{
    try
    {
        throw new FormatException("Original format error");
    }
    catch (Exception ex)
    {
        throw new InvalidOperationException("Operation failed", ex);
    }
}

static void ProcessData(string? data)
{
    if (data == null)
    {
        throw new ArgumentNullException(nameof(data));
    }
    Console.WriteLine($"Processing: {data}");
}

// Custom exception class
class InvalidAgeException : Exception
{
    public int ProvidedAge { get; }

    public InvalidAgeException(string message, int providedAge)
        : base(message)
    {
        ProvidedAge = providedAge;
    }
}
