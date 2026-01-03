# Arrays

Demonstrates C# array fundamentals including declaration, initialization, indexing, iteration, and common array operations.

## Usage

```bash
dotnet run --project Arrays/Arrays.csproj
```

## Example

```
=== Array Basics ===

Array length: 5

First element: 10
Middle element: 30
Last element: 50

After modifying index 1: 25

=== Using for loop ===
Index 0: 10
Index 1: 25
Index 2: 30
Index 3: 40
Index 4: 50

=== Using foreach ===
Value: 10
Value: 25
Value: 30
Value: 40
Value: 50

=== 2D Array ===
1 2 3 
4 5 6 
7 8 9 

=== Array Methods ===
Scores: [85, 92, 78, 96, 88]
Sum: 439
Average: 87.8
Max: 96
Min: 78

Index of 88: 4

Sorted scores: [78, 85, 88, 92, 96]
Reversed scores: [96, 92, 88, 85, 78]
```

## Concepts Demonstrated

- Single-dimensional array declaration and initialization
- Accessing and modifying elements by index
- Iterating with `for` and `foreach` loops
- Multi-dimensional (2D) arrays
- Array properties (`Length`, `GetLength`)
- LINQ extension methods (`Sum`, `Average`, `Max`, `Min`)
- Array class methods (`IndexOf`, `Sort`, `Reverse`)
