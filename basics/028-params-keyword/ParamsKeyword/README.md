# Params Keyword: Variable Arguments

Demonstrates the `params` keyword for creating methods that accept a variable number of arguments.

## Usage

```bash
dotnet run --project ParamsKeyword.csproj
```

## Example

```
=== Params Keyword: Variable Arguments ===

--- Basic Params Usage ---
Sum of 1, 2, 3: 6
Sum of 1, 2, 3, 4, 5: 15
Sum of 10, 20, 30, 40, 50, 60: 210

--- Params with Arrays ---
Sum from array: 1000

--- Params with Strings ---
Concatenate: HelloWorldCSharp
Concatenate with separator: A-B-C

--- Params with Mixed Parameters ---
[14:30:45] INFO: User logged in [admin, 192.168.1.1]
[14:30:45] ERROR: Connection failed [server, timeout]

--- Params with Different Types ---
  Items: 1(Int32) hello(String) 3.14(Double) True(Boolean) X(Char)
```

## Concepts Demonstrated

- Params keyword syntax
- Variable-length argument lists
- Params with different data types
- Params with arrays
- Params combined with regular parameters
- Params with optional parameters
- Params with LINQ operations
- Real-world logging scenarios
- Method chaining with params
- Yield return with params
