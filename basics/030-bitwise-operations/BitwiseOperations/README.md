# Bitwise Operations & Bit Manipulation

Demonstrates bitwise operators (AND, OR, XOR, NOT), bit shifting, flags, masks, and common bit manipulation patterns.

## Usage

```bash
dotnet run --project BitwiseOperations.csproj
```

## Example

```
=== Bitwise Operations & Bit Manipulation ===

--- Basic Bitwise Operators ---
a = 12 (binary: 00001100)
b = 5 (binary: 00000101)

a & b (AND)      =   4 (binary: 00000100)
a | b (OR)       =  13 (binary: 00001101)
a ^ b (XOR)      =   9 (binary: 00001001)
~a (NOT)         = -13 (binary: 11110011)

--- Bit Shifting ---
Original: 8 (binary: 00001000)
Left shift << 1:   16 (binary: 00010000)
Left shift << 2:   32 (binary: 00100000)
Right shift >> 1:   4 (binary: 00000100)

--- Flags Enum ---
User permissions: Read, Write
Has Read: True
Has Write: True
Has Execute: False

--- Check If Power of 2 ---
  1: Yes
  2: Yes
  4: Yes
  5: No
  8: Yes
 16: Yes

--- RGB Color Manipulation ---
RGB Color: 0xFF8040
Red:   255
Green: 128
Blue:  64
```

## Concepts Demonstrated

- AND, OR, XOR, NOT operators
- Left and right bit shifting
- Setting, clearing, and toggling bits
- Checking if a bit is set
- Flags enum with [Flags] attribute
- Bit masking operations
- Extracting specific bit ranges
- XOR swap algorithm
- Power of 2 detection
- Counting set bits (Hamming weight)
- Even/odd detection using bits
- RGB color encoding/decoding
- Fast multiplication/division by powers of 2
