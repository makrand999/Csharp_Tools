# Lists

Demonstrates C# `List<T>` generic collection including creation, modification, iteration, and basic LINQ operations.

## Usage

```bash
dotnet run --project Lists/Lists.csproj
```

## Example

```
=== Lists in C# ===

=== Initial List ===
Fruits: [Apple, Banana, Cherry]
Count: 3

=== Adding Items ===
After Add: [Apple, Banana, Cherry, Date, Elderberry]
After AddRange: [Apple, Banana, Cherry, Date, Elderberry, Fig, Grape]

=== Accessing Items ===
First fruit: Apple
Last fruit: Grape
After modifying index 1: [Apple, Blueberry, Cherry, Date, Elderberry, Fig, Grape]

=== Removing Items ===
After Remove('Apple'): [Blueberry, Cherry, Date, Elderberry, Fig, Grape]
After RemoveAt(0): [Cherry, Date, Elderberry, Fig, Grape]
Removed 'Elderberry', remaining: [Cherry, Date, Fig, Grape]

=== Checking Items ===
Contains 'Fig': True
Contains 'Mango': False
Index of 'Fig': 2

=== Iterating ===
Using foreach:
  - Cherry
  - Date
  - Fig
  - Grape

Using for with index:
  [0] Cherry
  [1] Date
  [2] Fig
  [3] Grape

=== LINQ with Lists ===
Numbers: [5, 2, 8, 1, 9, 3, 7, 4, 6]
Even numbers: [2, 8, 4, 6]
Squared: [25, 4, 64, 1, 81, 9, 49, 16, 36]
Sorted: [1, 2, 3, 4, 5, 6, 7, 8, 9]
First > 5: 8
First > 10 (default): 0

After Sort(): [1, 2, 3, 4, 5, 6, 7, 8, 9]
After Reverse(): [9, 8, 7, 6, 5, 4, 3, 2, 1]

=== List of Objects ===
People list:
  Alice, 30 years old
  Bob, 25 years old
  Charlie, 35 years old

Sorted by age:
  Bob, 25
  Alice, 30
  Charlie, 35

=== Clearing ===
After Clear, Count: 0

=== Lists Demo Complete ===
```

## Concepts Demonstrated

- `List<T>` generic collection
- List initialization
- Adding items (`Add`, `AddRange`)
- Accessing items by index
- Modifying items
- Removing items (`Remove`, `RemoveAt`)
- Checking existence (`Contains`, `IndexOf`)
- Iterating with `foreach` and `for`
- Sorting and reversing
- Clearing a list
- LINQ operations (`Where`, `Select`, `OrderBy`, `First`, `FirstOrDefault`)
- List of custom objects
- Auto-implemented properties
- Lambda expressions basics
