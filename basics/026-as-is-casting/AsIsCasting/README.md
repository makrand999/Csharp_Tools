# Type Casting: 'as' and 'is' Operators

Demonstrates safe type casting using the `as` operator and type checking with the `is` operator, including pattern matching features.

## Usage

```bash
dotnet run --project AsIsCasting.csproj
```

## Example

```
=== Type Casting: 'as' and 'is' Operators ===

--- Traditional Cast vs 'as' Operator ---
Traditional cast success: Hello, World!
Traditional cast failed: Unable to cast object of type 'System.Int32' to type 'System.String'.

'as' operator with string: Success: Hello, World!
'as' operator with int: Failed (null)

--- 'is' Operator for Type Checking ---
text is:
  - string: True
  - int: False
  ...

--- Pattern Matching with 'is' ---
String found: "hello" (length: 5)
Positive integer: 100
Double greater than 3: 3.14
Null value detected

--- Casting with Inheritance ---
Dog: Woof! Woof!
Cat: Meow! Meow!
Bird: Chirp! Chirp!
```

## Concepts Demonstrated

- Traditional casting with `(type)` syntax
- Safe casting with `as` operator (returns null on failure)
- Type checking with `is` operator
- Pattern matching with `is` (C# 7+)
- Type patterns with variable declaration
- Conditional patterns with `and`
- `is not` pattern (C# 9+)
- Casting with inheritance hierarchies
- Null handling in type operations
