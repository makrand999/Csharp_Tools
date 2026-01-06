# Tuples

Demonstrates C# tuple operations including creation, named elements, deconstruction, and using tuples as method return values.

## Usage

```bash
dotnet run --project Tuples.csproj
```

## Example

```
=== Tuple Basics ===

Simple tuple:
  Item1: Alice
  Item2: 30
  Item3: Engineer

--- Named Tuples ---
Employee: Bob, 25, Designer

--- Explicit Tuple Creation ---
Point: X=10, Y=20

--- Tuple Deconstruction ---
Deconstructed: Name=Bob, Age=25, Role=Designer

--- Deconstruction with Discards ---
Only extracting age: 25

--- Swapping Values ---
Before swap: x=5, y=10
After swap: x=10, y=5

=== Tuples as Method Returns ===

Divide 100 by 7:
  Quotient: 14
  Remainder: 2

Test Scores Statistics:
  Average: 86.60
  Min: 78
  Max: 92

=== Tuple Comparison ===

tuple1 = (1, 2, 3)
tuple2 = (1, 2, 3)
tuple3 = (1, 2, 4)

tuple1 == tuple2: True
tuple1 == tuple3: False
tuple1 < tuple3: -1

=== Nested Tuples ===

Rectangle:
  Top-Left: (0, 0)
  Bottom-Right: (100, 50)

=== Tuple with Arrays ===

Coordinate points:
  Point: X=0, Y=0
  Point: X=1, Y=1
  Point: X=2, Y=2
  Point: X=3, Y=3

=== Real-World: Weather Data ===

Weather in Seattle:
  Temperature: 22°C / 15°C
  Condition: Cloudy
  Humidity: 78%

=== Real-World: Database Records ===

User records:
  [1] alice - alice@example.com - Active: True
  [2] bob - bob@example.com - Active: True
  [3] charlie - charlie@example.com - Active: False

Active users (2):
  - alice
  - bob

=== Program Complete ===
```

## Concepts Demonstrated

- ValueTuple creation with inferred names
- Named tuple elements for readability
- Explicit ValueTuple<T1, T2, ...> creation
- Tuple deconstruction into individual variables
- Deconstruction with discards (_)
- Swapping values using tuple syntax
- Tuples as method return values (multiple returns)
- Tuple comparison (equality and ordering)
- Nested tuples for complex structures
- Arrays of tuples
- Real-world examples: weather data, database records
- Pattern matching with tuples
