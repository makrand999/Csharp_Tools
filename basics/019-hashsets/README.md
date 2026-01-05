# HashSets

Demonstrates C# `HashSet<T>` operations including adding unique items, set operations (union, intersection, difference), and duplicate removal.

## Usage

```bash
dotnet run --project HashSets.csproj
```

## Example

```
=== HashSet Basics (Unique Collections) ===

Initial tags:
  # csharp
  # dotnet
  # programming

--- Adding Items ---
Adding 'tutorial': True
Adding 'csharp' (duplicate): False
Adding 'beginner': True

Current tags (Count: 5):
  # csharp
  # dotnet
  # programming
  # tutorial
  # beginner

--- Removing Items ---
Remove 'tutorial': True
Remove 'nonexistent': False

--- Contains Check ---
Contains 'dotnet': True
Contains 'java': False

=== HashSet of Integers ===

--- Adding Numbers (Duplicates Ignored) ---
  Added 1: added
  Added 2: added
  Added 3: added
  Added 2: duplicate - ignored
  Added 4: added
  Added 3: duplicate - ignored
  Added 5: added
  Added 1: duplicate - ignored

Unique numbers (Count: 5):
  [1, 2, 3, 4, 5]

=== Set Operations ===

Set A: [1, 2, 3, 4, 5]
Set B: [4, 5, 6, 7, 8]

A Union B: [1, 2, 3, 4, 5, 6, 7, 8]
A Intersect B: [4, 5]
A Except B: [1, 2, 3]
A SymmetricExcept B: [1, 2, 3, 6, 7, 8]

=== Subset/Superset Checks ===

Small Set: [1, 2]
Large Set: [1, 2, 3, 4, 5]

Small Set is subset of Large Set: True
Large Set is superset of Small Set: True
Large Set is subset of Small Set: False

=== Overlaps Check ===

Set A: [1, 2, 3, 4, 5]
Overlapping Set: [3, 4, 9, 10]
Non-Overlapping Set: [100, 200, 300]

Set A overlaps with Overlapping Set: True
Set A overlaps with Non-Overlapping Set: False

=== Real-World: Remove Duplicates ===

Original email list (with duplicates):
  Count: 6
  → alice@example.com
  → bob@example.com
  → alice@example.com
  → charlie@example.com
  → bob@example.com
  → diana@example.com

Unique emails (duplicates removed):
  Count: 4
  → alice@example.com
  → bob@example.com
  → charlie@example.com
  → diana@example.com

=== Program Complete ===
```

## Concepts Demonstrated

- HashSet<T> creation and initialization
- Add - adding unique items (returns false for duplicates)
- Remove - removing items
- Contains - checking existence
- Count - getting number of unique items
- UnionWith - combining sets
- IntersectWith - finding common elements
- ExceptWith - finding differences
- SymmetricExceptWith - elements in either set but not both
- IsSubsetOf / IsSupersetOf - subset/superset checks
- Overlaps - checking for common elements
- Real-world: Removing duplicates from collections
