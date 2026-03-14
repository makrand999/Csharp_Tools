# File Sync Utility

Synchronizes files between two directories with multiple sync strategies.

## Usage

```bash
dotnet run --project FileSyncUtility.csproj
```

## Example

```
=== File Sync Utility ===

Creating test files in source directory...
Source files: 5

=== Initial Sync (Mirror Mode) ===
  Checking: file0.txt
  Checking: file1.txt
...

Sync Result:
  Files copied: 5
  Files deleted: 0
  Bytes transferred: 5000
  Time: 25ms

=== Modifying Source ===

=== Second Sync (Detecting Changes) ===
Sync Result:
  Files copied: 2
  Time: 10ms

Verification: ✓ PASSED
```

## Concepts Demonstrated

- File comparison by timestamp
- Mirror synchronization
- Incremental sync detection
- Async file copying
- Progress reporting
