# Screenshot Organizer

Automatically detects and organizes screenshots from various sources (Windows Snipping Tool, macOS screenshots, Steam, browsers, etc.) into organized folders.

## Usage

```bash
# Organize by date (default)
dotnet run --project ScreenshotOrganizer.csproj ~/Pictures

# Organize by application
dotnet run --project ScreenshotOrganizer.csproj --by app ~/Pictures

# Organize by resolution
dotnet run --project ScreenshotOrganizer.csproj --by resolution ~/Pictures

# Organize by month
dotnet run --project ScreenshotOrganizer.csproj --by month ~/Pictures

# Dry run (preview without moving)
dotnet run --project ScreenshotOrganizer.csproj --dry-run ~/Pictures

# Recursive search (include subdirectories)
dotnet run --project ScreenshotOrganizer.csproj --recursive ~/Pictures
```

## Organization Strategies

| Strategy | Description | Example Folders |
|----------|-------------|-----------------|
| `date` | Group by creation date (YYYY/MM_Month/DD) | `Screenshots/2024/01_January/15/` |
| `app` | Group by detected application | `Screenshots/By App/Chrome/`, `Screenshots/By App/Discord/` |
| `resolution` | Group by resolution | `Screenshots/By Resolution/4K (2160p)/`, `Full HD (1080p)/` |
| `month` | Group by month | `Screenshots/2024-01_January/` |

## Detected Screenshot Patterns

- **Windows**: `Screenshot_*.png`, `Snip_*.png`, `Snipping_*.png`
- **macOS**: `Screenshot 2024-01-15 at 10.30.00.png`
- **Linux**: `Screenshot from 2024-01-15 10-30-00.png`
- **Steam**: `steam_screenshot_*.jpg`
- **General**: `IMG_*.jpg`, `DSC_*.jpg`, `20240115_*.png`

## Example

**Before:**
```
Pictures/
├── Screenshot_2024-01-15_10-30-00.png
├── Screenshot_2024-01-15_11-45-00.png
├── Snip_20240116_143022.png
├── Screenshot_2024-01-17_Chrome.png
└── Screenshot_2024-01-17_Discord.png
```

**After (by app):**
```
Pictures/
└── Screenshots/
    └── By App/
        ├── Chrome/
        │   └── Screenshot_2024-01-17_Chrome.png
        ├── Discord/
        │   └── Screenshot_2024-01-17_Discord.png
        └── Other/
            ├── Screenshot_2024-01-15_10-30-00.png
            ├── Screenshot_2024-01-15_11-45-00.png
            └── Snip_20240116_143022.png
```

**Output:**
```
============================================================
Screenshot Organizer
Strategy: app
Source: /home/user/Pictures
Found: 5 screenshots
============================================================

📁 Screenshots/By App/Chrome/ (1 files)
  Screenshot_2024-01-17_Chrome.png         1.2 MB     2024-01-17 10:30

📁 Screenshots/By App/Discord/ (1 files)
  Screenshot_2024-01-17_Discord.png        856.3 KB   2024-01-17 11:45

📁 Screenshots/By App/Other/ (3 files)
  Screenshot_2024-01-15_10-30-00.png       234.5 KB   2024-01-15 10:30
  Screenshot_2024-01-15_11-45-00.png       567.8 KB   2024-01-15 11:45
  Snip_20240116_143022.png                 123.4 KB   2024-01-16 14:30

============================================================
Total: 5 screenshots in 3 folders
```

## Concepts Demonstrated

- File system operations and pattern matching
- Regex-based file detection
- Screenshot pattern recognition
- Application name extraction from filenames
- Resolution detection from filenames
- Date-based organization
- Interactive confirmation
- Duplicate file handling
- Recursive directory traversal
- Size and date formatting
