# Batch File Organizer

Automatically organizes files in a directory by grouping them into folders based on extension, date, size, or file type.

## Usage

```bash
# Organize by extension (default)
dotnet run --project BatchFileOrganizer.csproj ~/Downloads

# Organize by file type
dotnet run --project BatchFileOrganizer.csproj --by type ~/Downloads

# Organize by creation date
dotnet run --project BatchFileOrganizer.csproj --by date ~/Documents

# Organize by file size
dotnet run --project BatchFileOrganizer.csproj --by size ~/Projects

# Dry run (preview without moving)
dotnet run --project BatchFileOrganizer.csproj --dry-run ~/Downloads

# JSON output
dotnet run --project BatchFileOrganizer.csproj --json ~/Downloads
```

## Organization Strategies

| Strategy | Description | Example Folders |
|----------|-------------|-----------------|
| `extension` | Group by file extension | `JPG/`, `PDF/`, `DOCX/` |
| `date` | Group by creation date | `2024/01_January/`, `2024/02_February/` |
| `size` | Group by file size | `Tiny/`, `Small/`, `Medium/`, `Large/`, `Huge/` |
| `type` | Group by file type | `Images/`, `Videos/`, `Documents/`, `Code/` |

## Example

**Before:**
```
Downloads/
├── photo1.jpg
├── photo2.jpg
├── document.pdf
├── report.docx
├── video.mp4
└── archive.zip
```

**After (by type):**
```
Downloads/
├── Images/
│   ├── photo1.jpg
│   └── photo2.jpg
├── Documents/
│   ├── document.pdf
│   └── report.docx
├── Videos/
│   └── video.mp4
└── Archives/
    └── archive.zip
```

**Output:**
```
============================================================
Batch File Organizer
Strategy: type
Source: /home/user/Downloads
============================================================

📁 Archives/ (1 files)
  archive.zip                              (2.5 MB)

📁 Documents/ (2 files)
  document.pdf                             (156.3 KB)
  report.docx                              (45.2 KB)

📁 Images/ (2 files)
  photo1.jpg                               (1.2 MB)
  photo2.jpg                               (1.5 MB)

📁 Videos/ (1 files)
  video.mp4                                (15.8 MB)

============================================================
Total: 6 files in 4 folders
```

## Concepts Demonstrated

- File system operations (Directory, File, FileInfo)
- File organization strategies
- Pattern matching with switch expressions
- Dictionary-based grouping
- JSON serialization
- Dry-run simulation
- Duplicate file handling
- Size formatting
- Command-line argument parsing
- Error handling for file operations
