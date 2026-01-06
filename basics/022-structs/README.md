# Structs

Demonstrates C# struct (value type) definition, constructors, properties, methods, and the difference between value types and reference types.

## Usage

```bash
dotnet run --project Structs.csproj
```

## Example

```
=== Struct Basics ===

Point p1:
Point(3, 4)
Distance from origin: 5.00

Point p2: (10, 20)

Default point p3: (0, 0)

=== Value Semantics (Copy on Assignment) ===

Original: (5, 5)
Copy: (5, 5)

After modifying copy:
Original: (5, 5) - unchanged
Copy: (100, 200) - modified

=== Readonly Struct ===

Red: rgb(255, 0, 0) = #FF0000
Green: rgb(0, 255, 0) = #00FF00
Blue: rgb(0, 0, 255) = #0000FF

=== Complex Struct with Nested Structs ===

Main Rectangle:
  Top-Left: (0, 0)
  Bottom-Right: (100, 50)
  Width: 100, Height: 50
  Area: 5000

=== Array of Structs ===

Points array:
Point(0, 0)
Point(10, 5)
Point(20, 10)
Point(30, 15)
Point(40, 20)

=== List of Structs ===

Custom colors:
  rgb(255, 128, 0) = #FF8000
  rgb(128, 0, 255) = #8000FF
  rgb(0, 255, 255) = #00FFFF
  rgb(255, 255, 0) = #FFFF00

=== Struct vs Class (Value vs Reference) ===

Struct: struct1.Value = 10, struct2.Value = 20
  (Structs are copied on assignment - independent values)

Class: class1.Value = 20, class2.Value = 20
  (Classes are references - same object)

=== Nullable Structs ===

Nullable point (null): False
Nullable point (assigned): True
Value: (50, 50)
X value: 50

=== Program Complete ===
```

## Concepts Demonstrated

- Struct definition with properties
- Struct constructors
- Struct methods (instance and readonly)
- Object initializer syntax
- Default struct values
- Value semantics (copy on assignment)
- Readonly structs for immutability
- Computed properties with expressions
- Nested structs
- Arrays of structs
- Lists of structs
- Struct vs Class (value type vs reference type)
- Nullable structs (T?)
- Null-conditional operator with structs
- When to use structs (small, immutable data)
