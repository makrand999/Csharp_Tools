# Character Counter

A CLI tool for analyzing text and counting characters, words, lines, and character frequency.

## Usage

```bash
dotnet run --project CharCounter.csproj [options]
```

## Options

| Option | Description |
|--------|-------------|
| `<file>` | Count characters in file |
| `--stdin` | Count characters from stdin |
| `--string "<text>"` | Count characters in string |
| `--detailed` | Show detailed breakdown |
| `--top <n>` | Show top N frequent chars (default: 10) |
| `--json` | Output as JSON |

## Examples

```bash
# Count characters in a file
dotnet run --project CharCounter.csproj file.txt

# Count from stdin
cat file.txt | dotnet run --project CharCounter.csproj --stdin

# Count characters in a string
dotnet run --project CharCounter.csproj --string "hello world"

# Detailed analysis
dotnet run --project CharCounter.csproj file.txt --detailed

# JSON output
dotnet run --project CharCounter.csproj file.txt --json
```

## Sample Output

```
# Summary output
Character Count Summary
=======================

Total characters: 1,234
Characters (no spaces): 1,050
Words: 250
Lines: 45

---

# Detailed output
Character Count - Detailed Analysis
====================================

Overview:
  Total characters:     1,234
  Characters (no spaces): 1,050
  Words:                250
  Lines:                45

Character Types:
  Letters:     950
    Uppercase: 50
    Lowercase: 900
  Digits:      25
  Spaces:      184
  Punctuation: 50
  Other:       25

Top 10 Most Frequent Characters:
  Char | Count | Percentage | Visual
  -----+------+------------+------------------
    'e' |   150 |     12.16% | ██████
    't' |   120 |      9.72% | ████
    'a' |   100 |      8.10% | ████
    'o' |    90 |      7.29% | ███
    ' ' |    85 |      6.89% | ███
    'n' |    80 |      6.48% | ███
    'i' |    75 |      6.08% | ███
    's' |    70 |      5.67% | ██
    'r' |    65 |      5.27% | ██
    'h' |    60 |      4.86% | ██

Unique characters: 45

---

# JSON output
{
  "totalCharacters": 1234,
  "charactersNoSpaces": 1050,
  "words": 250,
  "lines": 45,
  "letters": 950,
  "uppercase": 50,
  "lowercase": 900,
  "digits": 25,
  "spaces": 184,
  "punctuation": 50,
  "other": 25,
  "topCharacters": [{"char": "e", "count": 150}, ...]
}
```

## Concepts Demonstrated

- File I/O and stdin handling
- Character classification (IsLetter, IsDigit, etc.)
- Dictionary for frequency counting
- LINQ for sorting and aggregation
- String interpolation
- JSON formatting
- Command-line argument parsing
- Text statistics and analysis
