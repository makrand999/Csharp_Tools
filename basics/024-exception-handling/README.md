# Exception Handling

Demonstrates C# exception handling including try-catch-finally blocks, multiple catch blocks, exception filters, custom exceptions, and common exception types.

## Usage

```bash
dotnet run --project ExceptionHandling.csproj
```

## Example

```
=== Exception Handling Basics ===

--- Basic Try-Catch ---
Error: Cannot divide by zero!
Message: Attempted to divide by zero.

--- Multiple Catch Blocks ---
NullReferenceException: Object reference not set to an instance of an object.

--- Try-Catch-Finally ---
In try block
FormatException: The input string 'abc' was not in a correct format.
Finally block always executes

--- Nested Try-Catch ---
Outer try block
  Inner try block
  Inner catch handled the exception
Outer catch: Inner exception

--- Exception Filters ---
Caught ArgumentException with param 'value': Value cannot be negative (Parameter 'value')

=== Common Exception Types ===

FormatException: Invalid format for parsing
OverflowException: Value exceeded type limits
IndexOutOfRangeException: Array index out of bounds
KeyNotFoundException: Key not in dictionary
InvalidCastException: Cannot cast string to int

=== Custom Exceptions ===

InvalidAgeException: Age must be between 0 and 120
Provided age: -5

InvalidAgeException: Age must be between 0 and 120
Provided age: 150

=== Exception Properties ===

Type: InvalidOperationException
Message: Operation failed
StackTrace:    at Program.ThrowDetailedException()...
InnerException: Original format error

=== Best Practices ===

ArgumentNullException caught: data

=== Program Complete ===
```

## Concepts Demonstrated

- Basic try-catch block
- Multiple catch blocks for different exception types
- Try-catch-finally (finally always executes)
- Nested try-catch blocks
- Re-throwing exceptions with `throw`
- Exception filters (when clause)
- Common exception types:
  - DivideByZeroException
  - NullReferenceException
  - FormatException
  - OverflowException
  - IndexOutOfRangeException
  - KeyNotFoundException
  - InvalidCastException
  - ArgumentException
  - ArgumentNullException
  - InvalidOperationException
- Custom exception classes
- Exception properties (Message, StackTrace, InnerException)
- Checked context for overflow detection
- Best practices for exception handling
