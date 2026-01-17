# Duplicate File Finder

A CLI tool that finds duplicate files in a directory by computing SHA256 hashes.

## Usage

```bash
dotnet run --project DuplicateFinder.csproj
```

## Example

```
Duplicate File Finder
=====================

Enter directory path to scan: /home/user/Pictures
Include subdirectories? (y/n): y
Minimum file size in bytes (0 for all): 1024

🔍 Scanning: /home/user/Pictures
   Subdirectories: Yes
   Minimum size: 1024 bytes

📁 Found 1523 files to analyze.

🔎 Found 47 size groups with potential duplicates.

   Computing hashes...
   Progress: 47/47 size groups

❌ Found 12 sets of duplicate files:

📋 Duplicate Set #1 (3 files, 2.5 MB each):
   - /home/user/Pictures/vacation.jpg
   - /home/user/Pictures/backup/vacation.jpg
   - /home/user/Pictures/old/vacation_copy.jpg
   💾 Wasted space: 5 MB

📋 Duplicate Set #2 (2 files, 1.2 MB each):
   - /home/user/Pictures/screenshot.png
   - /home/user/Pictures/downloads/screenshot.png
   💾 Wasted space: 1.2 MB

════════════════════════════════════════
📊 Total duplicate sets: 12
💾 Total wasted space: 45.3 MB
════════════════════════════════════════

✅ Done!
```

## Concepts Demonstrated

- File system traversal with Directory.EnumerateFiles
- SHA256 hash computation for file comparison
- Dictionary-based grouping and deduplication
- Efficient two-pass algorithm (size then hash)
- Recursive directory scanning
- File size formatting and statistics
- Exception handling for inaccessible files
