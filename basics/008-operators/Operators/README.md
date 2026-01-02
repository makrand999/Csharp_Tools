# Operators

Comprehensive demonstration of C# operators: arithmetic, comparison, logical, assignment, bitwise, and null-coalescing.

## Usage

```bash
dotnet run --project Operators.csproj
```

## Example

```
=== Arithmetic Operators ===
a = 15, b = 4
a + b = 19
a - b = 11
a * b = 60
a / b = 3
a % b = 3
a++ = 15, then a = 16
++a = 17, now a = 17

=== Comparison Operators ===
x = 10, y = 20
x == y: False
x != y: True
x > y:  False
x < y:  True
x >= 10: True
y <= 20: True

=== Logical Operators ===
isSunny = True, isWarm = False
isSunny && isWarm: False
isSunny || isWarm: True
!isSunny: False
isNiceDay (complex): True

=== Assignment Operators ===
Initial: num = 10
num += 5  → 15
num -= 3  → 12
num *= 2  → 24
num /= 4  → 6
num %= 3  → 0

=== Bitwise Operators ===
p = 5 (binary: 0101)
q = 3 (binary: 0011)
p & q = 1 (AND)
p | q = 7 (OR)
p ^ q = 6 (XOR)
~p = -6 (NOT)
p << 1 = 10 (Left shift)
p >> 1 = 2 (Right shift)

=== Ternary Operator ===
Score: 85 → Result: Pass
Score: 85 → Grade: 2

=== Null-coalescing Operators ===
userInput ?? defaultName = "Guest"
nullableNumber ?? 0 = 0
userInput ??= "Default User" → "Default User"
```

## Concepts Demonstrated

- Arithmetic operators: +, -, *, /, %, ++, --
- Comparison operators: ==, !=, >, <, >=, <=
- Logical operators: &&, ||, !
- Assignment operators: =, +=, -=, *=, /=, %=
- Bitwise operators: &, |, ^, ~, <<, >>
- Ternary conditional operator: ? :
- Null-coalescing operators: ??, ??=
