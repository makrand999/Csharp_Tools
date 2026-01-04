# Dictionaries

Demonstrates C# `Dictionary<TKey, TValue>` operations including creating, adding, removing, and iterating over key-value pairs.

## Usage

```bash
dotnet run --project Dictionaries.csproj
```

## Example

```
=== Dictionary Basics ===

Initial students:
  Alice: 95
  Bob: 87
  Charlie: 92

--- Adding New Items ---
Added Diana (88) and Eve (91)

--- Accessing Values ---
Bob's score: 87
Charlie's score: 92

--- Safe Lookup with TryGetValue ---
Alice's score: 95
Frank not found in dictionary

--- Checking Existence ---
Contains key 'Bob': True
Contains value 92: True

--- Updating Values ---
Updated Bob's score to: 90

--- Removing Items ---
Removed Charlie from dictionary

--- Iteration Methods ---
Iterating over Key-value pairs:
  Alice: 95
  Bob: 90
  Diana: 88
  Eve: 91

Iterating over keys only:
  Key: Alice
  Key: Bob
  Key: Diana
  Key: Eve

Iterating over values only:
  Value: 95
  Value: 90
  Value: 88
  Value: 91

=== Dictionary with Complex Types ===

Product Prices:
  Product ID 101: $29.99
  Product ID 102: $49.50
  Product ID 103: $15.00
  Product ID 104: $99.99

Total: $194.48
Average: $48.62
Most expensive: Product 104 at $99.99

=== Program Complete ===
```

## Concepts Demonstrated

- Dictionary<TKey, TValue> creation and initialization
- Adding items (indexer and Add method)
- Accessing values by key
- TryGetValue for safe lookup
- ContainsKey and ContainsValue methods
- Updating existing values
- Removing items with Remove
- Iterating over keys, values, and key-value pairs
- Using LINQ with dictionary values (Sum, Average, Max)
