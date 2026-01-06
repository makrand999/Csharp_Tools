# Enums

Demonstrates C# enumeration (enum) definition, usage, parsing, and flags for bitwise operations.

## Usage

```bash
dotnet run --project Enums.csproj
```

## Example

```
=== Enum Basics ===

Today is: Tuesday
Underlying value: 2

--- Enum Comparison ---
Is today Tuesday? True
Is today Monday? False

--- Switch on Enum ---
Tuesday: Weekday

--- All Days of Week ---
  Sunday = 0
  Monday = 1
  Tuesday = 2
  Wednesday = 3
  Thursday = 4
  Friday = 5
  Saturday = 6

--- Enum Names ---
  Sunday
  Monday
  Tuesday
  Wednesday
  Thursday
  Friday
  Saturday

=== Enum with Explicit Values ===

Task priority: High
Underlying value: 3

--- Priority Comparison ---
High > Medium: True
Low < High: True

=== Parsing Strings to Enums ===

Parsed 'Thursday' to: Thursday
Failed to parse 'Funday'

--- Case-Insensitive Parsing ---
Parsed 'monday' (lowercase) to: Monday

=== Enum Flags (Bitwise Operations) ===

User permissions: Read, Write
Underlying value: 3

--- Checking Flags ---
Has Read permission: True
Has Write permission: True
Has Execute permission: False

--- Adding Flags ---
After adding Execute: Read, Write, Execute
Underlying value: 7

--- Removing Flags ---
After removing Write: Read, Execute

--- All Permissions ---
All permissions: Read, Write, Execute, Delete
Underlying value: 15

=== Real-World: Order Status ===

Order Status Report:
-------------------
  Order #1001: ⏳ Pending - Alice
  Order #1002: 📦 Shipped - Bob
  Order #1003: ✅ Delivered - Charlie
  Order #1004: ❌ Cancelled - Diana
  Order #1005: 🔄 Processing - Eve

Pending orders: 1

=== Real-World: Traffic Light ===

Current light: Red
Action: STOP

=== Program Complete ===
```

## Concepts Demonstrated

- Enum definition with implicit values (0-based)
- Enum definition with explicit values
- Using enum values in variables
- Enum comparison and equality checks
- Switch expressions with enums
- Enum.GetValues() - getting all enum values
- Enum.GetNames() - getting all enum names
- Enum.TryParse() - parsing strings to enums
- Case-insensitive parsing
- [Flags] attribute for bitwise operations
- Combining flags with OR (|)
- Checking flags with HasFlag()
- Adding flags with |=
- Removing flags with &= ~
- Real-world examples: order status, traffic light, permissions
