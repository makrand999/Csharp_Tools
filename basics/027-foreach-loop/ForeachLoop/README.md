# Foreach Loop, IEnumerable & IEnumerator

Demonstrates the foreach loop construct, IEnumerable interface, IEnumerator, and custom collection iteration patterns.

## Usage

```bash
dotnet run --project ForeachLoop.csproj
```

## Example

```
=== Foreach Loop, IEnumerable & IEnumerator ===

--- Foreach with Arrays ---
Array elements: 1 2 3 4 5 

--- Foreach with Lists ---
  Hello, Alice!
  Hello, Bob!
  Hello, Charlie!
  Hello, Diana!

--- Foreach with Dictionaries ---
  Alice: 95
  Bob: 87
  Charlie: 92

--- Foreach with Deconstruction ---
  Alice scored 95
  Bob scored 87

--- Custom IEnumerable Collection ---
10 20 30 40 50 

--- Yield Return (Lazy Evaluation) ---
First 5 squares: 1 4 9 16 25 
```

## Concepts Demonstrated

- Foreach loop syntax with arrays, lists, and dictionaries
- IEnumerable and IEnumerator interfaces
- Custom collection implementation
- KeyValuePair iteration
- Deconstruction in foreach (C# 7+)
- Yield return for lazy evaluation
- Manual enumerator usage
- Nested foreach loops
- Break and continue in foreach
- Read-only collections iteration
