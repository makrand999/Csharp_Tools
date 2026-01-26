# Download Folder Organizer

Automatically organizes downloaded files into categorized folders by file type. Supports age filtering, dry-run mode, and empty directory cleanup.

## Usage

```bash
# Default (uses ~/Downloads)
dotnet run --project DownloadOrganizer.csproj

# Specify folder
dotnet run --project DownloadOrganizer.csproj ~/Downloads

# Dry run (preview without moving)
dotnet run --project DownloadOrganizer.csproj --dry-run ~/Downloads

# Only organize files older than 7 days
dotnet run --project DownloadOrganizer.csproj --min-age 7 ~/Downloads

# Delete empty directories after organizing
dotnet run --project DownloadOrganizer.csproj --delete-empty ~/Downloads

# JSON output
dotnet run --project DownloadOrganizer.csproj --json ~/Downloads
```

## Categories

| Category | File Extensions |
|----------|-----------------|
| **Installers** | `.exe`, `.msi`, `.dmg`, `.pkg`, `.deb`, `.rpm` |
| **Archives** | `.zip`, `.rar`, `.7z`, `.tar`, `.gz`, `.bz2`, `.xz` |
| **Documents** | `.pdf`, `.doc`, `.docx`, `.xls`, `.xlsx`, `.ppt`, `.pptx` |
| **Text** | `.txt`, `.md`, `.rtf`, `.tex` |
| **Images** | `.jpg`, `.jpeg`, `.png`, `.gif`, `.bmp`, `.svg`, `.webp` |
| **Videos** | `.mp4`, `.avi`, `.mkv`, `.mov`, `.wmv`, `.flv`, `.webm` |
| **Audio** | `.mp3`, `.wav`, `.flac`, `.aac`, `.ogg`, `.wma`, `.m4a` |
| **Code** | `.cs`, `.csproj`, `.js`, `.ts`, `.py`, `.java`, `.cpp` |
| **Web** | `.html`, `.css`, `.json`, `.xml`, `.yaml`, `.yml` |
| **Fonts** | `.ttf`, `.otf`, `.woff`, `.woff2`, `.eot` |
| **Torrents** | `.torrent` |
| **Disk Images** | `.iso`, `.img`, `.vmdk`, `.vdi` |

## Example

**Before:**
```
Downloads/
├── installer.exe
├── document.pdf
├── photo.jpg
├── archive.zip
├── song.mp3
├── video.mp4
└── readme.md
```

**After:**
```
Downloads/
├── Installers/
│   └── installer.exe
├── Documents/
│   └── document.pdf
├── Images/
│   └── photo.jpg
├── Archives/
│   └── archive.zip
├── Audio/
│   └── song.mp3
├── Videos/
│   └── video.mp4
└── Text/
    └── readme.md
```

**Output:**
```
======================================================================
Download Folder Organizer
Source: /home/user/Downloads
======================================================================

📁 Archives/ (1 files, 2.5 MB)
  archive.zip                                 2.5 MB     3d ago

📁 Audio/ (1 files, 4.2 MB)
  song.mp3                                    4.2 MB     today

📁 Documents/ (1 files, 156 KB)
  document.pdf                                156 KB     yesterday

📁 Images/ (1 files, 1.2 MB)
  photo.jpg                                   1.2 MB     5d ago

📁 Installers/ (1 files, 45.8 MB)
  installer.exe                               45.8 MB    10d ago

📁 Text/ (1 files, 2 KB)
  readme.md                                   2 KB       today

📁 Videos/ (1 files, 125.3 MB)
  video.mp4                                   125.3 MB   1d ago

======================================================================
Summary:
  Total files: 7
  Organized: 7
  Unknown types: 0
  Categories: 7
```

## Concepts Demonstrated

- File system operations (Directory, File, FileInfo)
- File extension-based categorization
- Dictionary with HashSet for efficient lookups
- Age-based file filtering
- Duplicate file handling
- JSON serialization
- Dry-run simulation
- Interactive confirmation
- Empty directory cleanup
- Size and date formatting
- Command-line argument parsing
