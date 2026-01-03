# Methods

Demonstrates C# method fundamentals including definition, parameters, return values, overloading, and advanced parameter types.

## Usage

```bash
dotnet run --project Methods/Methods.csproj
```

## Example

```
=== Methods in C# ===

Hello from SayHello()!
Hi Alice, you are 25 years old.

10 + 20 = 30

5 and 3:
  Sum: 8
  Product: 15

Power(2, 10) = 1024
Power(3) = 9

Named arguments:
[High] From Admin: System update

=== Method Overloading ===
12
10
24

=== Ref and Out Parameters ===
Original value: 100
After DoubleByRef: 200

Parsed '42' successfully: 42
Parsing 'abc' failed as expected

=== Local Functions ===
Factorial of 5: 120

=== All methods completed ===
```

## Concepts Demonstrated

- Method declaration and invocation
- Parameters (value types)
- Return values
- Tuple return types (C# 7+)
- Optional parameters with default values
- Named arguments
- Method overloading
- `ref` parameters (pass by reference)
- `out` parameters (output parameters)
- Expression-bodied methods
- Local functions (functions inside functions)
- Recursion
