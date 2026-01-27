# Text Diff Tool

Compare files and text strings with visual diff output. Supports line-by-line file comparison, text string comparison, and Levenshtein distance calculation.

## Usage

```bash
dotnet run --project TextDiffTool.csproj <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `files <f1> <f2>` | Compare two files line by line |
| `text <s1> <s2>` | Compare two text strings |
| `distance <s1> <s2>` | Calculate Levenshtein distance |

## Examples

### Compare Two Files

```bash
dotnet run --project TextDiffTool.csproj files original.txt modified.txt
```

**Output:**
```
Comparing: original.txt vs modified.txt
------------------------------------------------------------

Statistics:
  File 1: 10 lines
  File 2: 12 lines
  Added: 3 lines
  Removed: 1 lines
  Unchanged: 9 lines

Similarity: 90.00%

Diff Output:
------------------------------------------------------------
  line 1 unchanged
  line 2 unchanged
- removed line in red
+ added line in green
  line 3 unchanged
```

### Compare Text Strings

```bash
dotnet run --project TextDiffTool.csproj text "hello world" "hello there"
```

**Output:**
```
Text Comparison:
------------------------------------------------------------
Text 1: 11 chars, 1 lines
Text 2: 11 chars, 1 lines

Levenshtein Distance: 5
Similarity: 54.55%

Line-by-line diff:
------------------------------------------------------------
- hello world
+ hello there
```

### Calculate Levenshtein Distance

```bash
dotnet run --project TextDiffTool.csproj distance "kitten" "sitting"
```

**Output:**
```
String 1: "kitten" (6 chars)
String 2: "sitting" (7 chars)

Levenshtein Distance: 3
Similarity: 57.14%

Character-level analysis:
  Matching: 5
  Different: 1
  Extra: 2
```

## Concepts Demonstrated

- Levenshtein distance algorithm
- Longest Common Subsequence (LCS) algorithm
- Dynamic programming for string comparison
- File I/O and line-by-line processing
- Console color output for visual diff
- Character frequency analysis
- Similarity calculation and statistics

## Algorithms Used

### Levenshtein Distance
Measures the minimum number of single-character edits (insertions, deletions, substitutions) required to transform one string into another.

### LCS-based Diff
Uses the Longest Common Subsequence algorithm to identify which lines are unchanged, added, or removed between two texts.
