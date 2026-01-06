# Nullable Types

Demonstrates C# nullable value types, null-coalescing operators, null-conditional operators, and safe null handling patterns.

## Usage

```bash
dotnet run --project NullableTypes.csproj
```

## Example

```
=== Nullable Value Types ===

Nullable int (null):
HasValue: False

Nullable int (42): 42
HasValue: True
Value: 42

Price: $19.99
Discount: No discount

Last login: Never logged in
Last login: 12/31/2025 10:30:45

=== Null-Coalescing Operator (??) ===

UserName is null, DisplayName: Guest
UserName is 'Alice', DisplayName: Alice

--- Chaining Null-Coalescing ---
Result: Default User

--- Null-Coalescing Assignment (??=) ---
Count before:
Count after ??= 10: 10
Count after ??= 20: 10

=== Null-Conditional Operator (?.) ===

Null string length:
'Hello, World!' length: 13

--- Null-Conditional with Methods ---
ToUpper on null:
ToUpper on 'hello': HELLO

--- Null-Conditional with Arrays ---
First element of null array:
First element of [1,2,3,4,5]: 1

--- Chaining Null-Conditional ---
City from null person:
City from person: Seattle

=== Nullable Arithmetic ===

10 + 5 = 15
10 - 5 = 5
10 * 5 = 50
10 / 5 = 2

10 +  =
 * 5 =

=== Nullable Comparisons ===

10 == 10: True
10 == : False
 == null: True
10 > 10: False
10 < 10: False

=== Nullable Boolean ===

Status (null verified): Not Verified
Status (true verified): Verified

=== GetValueOrDefault() ===

Null value.GetValueOrDefault(): 0
Null value.GetValueOrDefault(100): 100
50 value.GetValueOrDefault(): 50
50 value.GetValueOrDefault(100): 50

=== Real-World: Database Record Simulation ===

User Profile:
  ID: 1
  Name: Charlie
  Age: 0 (default if null)
  Email: charlie@example.com
  Phone: Not provided
  Last Login: 2025-12-26

=== Program Complete ===
```

## Concepts Demonstrated

- Nullable value types (int?, double?, DateTime?, bool?)
- HasValue and Value properties
- Null-coalescing operator (??)
- Chaining null-coalescing operators
- Null-coalescing assignment (??=)
- Null-conditional operator (?.)
- Null-conditional with methods
- Null-conditional with arrays (?[])
- Chaining null-conditional operators
- Nullable arithmetic operations
- Nullable comparisons
- GetValueOrDefault() method
- Real-world: Handling nullable database fields
- Safe null handling patterns
