# Strings

Demonstrates C# string manipulation including interpolation, common methods, formatting, and character operations.

## Usage

```bash
dotnet run --project Strings/Strings.csproj
```

## Example

```
=== String Operations in C# ===

=== String Interpolation ===
Hello, World! Welcome, C# Developer!

This is a multi-line string.
It preserves formatting and indentation.
Very useful for templates and messages.

=== Concatenation ===
Full name: John Doe

=== String Methods ===
Original: '  Hello, C# World!  '
Length: 20
Trimmed: 'Hello, C# World!'
ToLower: '  hello, c# world!  '
ToUpper: '  HELLO, C# WORLD!  '

=== Substring ===
Original: The quick brown fox jumps over the lazy dog
First 9 chars: The quick
From index 16: fox jumps over the lazy dog

=== Searching ===
Index of 'fox': 16
Last index of 'o': 22
Contains 'quick': True
Starts with 'The': True
Ends with 'dog': True

=== Replace ===
The slow brown fox jumps over the lazy cat

=== Split and Join ===
CSV: apple,banana,cherry,date
Split into array: [apple, banana, cherry, date]
Joined with ' | ': apple | banana | cherry | date

=== String Formatting ===
Price: $49.99
Quantity: 003
Date: 3/31/2026
Date: 2026-03-31
Total: $149.97

=== Padding ===
|Right     |
|      Left|

=== Comparison ===
"Hello" == "hello": False
Equals (ignore case): True

=== Character Operations ===
'C': Letter
'S': Letter
h': Letter
'a': Letter
'r': Letter
'p': Letter

=== StringBuilder ===
Built string: Step 1, Step 2, Step 3, Step 4, Step 5
```

## Concepts Demonstrated

- String declaration and initialization
- String interpolation (`$"{}"`)
- Verbatim and multi-line strings (`"""`)
- String concatenation
- Common string methods (`Trim`, `ToLower`, `ToUpper`)
- Substring extraction
- Searching (`IndexOf`, `Contains`, `StartsWith`, `EndsWith`)
- Replace operations
- Split and Join
- String formatting (currency, dates, padding)
- String comparison (case-sensitive and case-insensitive)
- Character classification
- StringBuilder for efficient string building
