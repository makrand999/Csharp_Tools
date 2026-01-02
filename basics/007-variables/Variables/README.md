# Variables

Demonstrates C# data types, variable declarations, type inference, and type conversion.

## Usage

```bash
dotnet run --project Variables.csproj
```

## Example

```
=== Value Types ===
Age (int): 25
Price (double): 19.99
Money (decimal): 100.50
Grade (char): A
Is Student (bool): True

=== Reference Types ===
Name: Alice
City: New York

=== Type Inference (var) ===
Country: USA (Type: String)
Score: 95 (Type: Int32)
Is Active: False (Type: Boolean)

=== Type Conversion ===
42 (int) → 42.0 (double)
9.99 (double) → 9 (int) [truncated]
"123" (string) → 123 (int)
Failed to parse "abc" - handled gracefully!

=== Constants ===
Pi: 3.14159
Greeting: Hello, Variables!
```

## Concepts Demonstrated

- Value types: int, double, decimal, char, bool
- Reference types: string
- Type inference with var keyword
- Implicit and explicit type conversion
- Parse and TryParse methods
- Constants with const keyword
- GetType().Name for runtime type inspection
