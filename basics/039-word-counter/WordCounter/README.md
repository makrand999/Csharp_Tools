# Word/Character Counter

A CLI tool for analyzing text and counting words, characters, sentences, and more.

## Usage

```bash
# Interactive mode (stdin)
dotnet run --project WordCounter.csproj

# Analyze a file
dotnet run --project WordCounter.csproj path/to/file.txt

# Analyze inline text
dotnet run --project WordCounter.csproj "Your text here"
```

## Examples

```bash
# Count words in a file
dotnet run --project WordCounter.csproj README.md

# Count words from argument
dotnet run --project WordCounter.csproj "Hello world! This is a test."

# Output:
# === Text Statistics ===
#
# Characters:
#   Total:         38
#   Without spaces: 31
#   Letters:       28
#   Digits:        0
#   Spaces:        7
#   Punctuation:   3
#
# Words:
#   Total:   7
#   Unique:  7
#
# Structure:
#   Sentences:   2
#   Paragraphs:  1
#
# Top 10 Words:
#   1. hello          1x
#   2. world          1x
#   ...
```

## Features

- Character counts (total, without spaces, letters, digits)
- Word count and unique word count
- Sentence and paragraph detection
- Top 10 most frequent words
- File or stdin input support

## Concepts Demonstrated

- Regular expressions for text parsing
- LINQ grouping and aggregation
- File I/O operations
- Record types for data structures
- Dictionary operations
