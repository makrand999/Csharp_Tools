# Regex Tester CLI

A CLI tool for testing regular expressions against text and viewing match results.

## Usage

```bash
dotnet run --project RegexTester.csproj <pattern> <text>
```

## Examples

```bash
# Find all numbers in text
dotnet run --project RegexTester.csproj "\d+" "There are 42 apples and 100 oranges"
# Output:
# === Regex Test ===
#
# Pattern: \d+
# Text:    There are 42 apples and 100 oranges
#
# Found 2 match(es):
#
# Match 1:
#   Value:    "42"
#   Position: 10-12
#
# Match 2:
#   Value:    "100"
#   Position: 27-30
#
# Test Results:
#   IsMatch: True
#
# Replace with '***':
#   There are *** apples and *** oranges

# Case-insensitive match
dotnet run --project RegexTester.csproj "(?i)hello" "Hello World"

# Email validation
dotnet run --project RegexTester.csproj "^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$" "test@example.com"
```

## Features

- Shows all matches with positions
- Displays capture groups
- Shows replacement example
- Timeout protection (2 seconds)
- Clear error messages for invalid patterns

## Concepts Demonstrated

- System.Text.RegularExpressions
- Regex.Match and Regex.Matches
- Capture groups
- Regex options (case insensitive, multiline)
- Timeout handling for regex operations
