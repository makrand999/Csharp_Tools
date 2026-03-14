# Directory Comparator

Compares two directories and identifies differences using file hashing.

## Usage

```bash
dotnet run --project DirectoryComparator.csproj
```

## Example

```
=== Directory Comparator ===

Setting up test directories...

  Found 3 files in first directory
  Found 3 files in second directory

=== Comparison Results ===
Files in directory 1: 3
Files in directory 2: 3
Identical files: 1

Modified files (1):
  ~ modified.txt

Only in first directory (1):
  + only_in_1.txt

Only in second directory (1):
  + only_in_2.txt

Directories are: ✗ DIFFERENT
```

## Concepts Demonstrated

- MD5 hash computation
- File comparison algorithms
- Directory traversal
- Difference detection
- Set operations
