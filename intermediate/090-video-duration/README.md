# Video Duration Extractor

Extract duration and metadata from video files. Supports batch processing, duration filtering, and various output formats.

## Usage

```bash
# Single file
dotnet run --project VideoDuration.csproj video.mp4

# Directory
dotnet run --project VideoDuration.csproj ~/Videos

# Recursive search
dotnet run --project VideoDuration.csproj --recursive ~/Movies

# Show totals
dotnet run --project VideoDuration.csproj --total ~/Videos

# Sort by duration
dotnet run --project VideoDuration.csproj --sort duration ~/Videos

# Filter by duration (videos between 5 and 30 minutes)
dotnet run --project VideoDuration.csproj --min-duration 5:00 --max-duration 30:00 ~/Videos

# JSON output
dotnet run --project VideoDuration.csproj --json --total ~/Videos
```

## Example

**Output:**
```
================================================================================
Video Duration Extractor
================================================================================

File                                        Duration   Resolution   Size      
                                                                              
tutorial_part1.mp4                          15:32      1920x1080    245.6 MB  
tutorial_part2.mp4                          18:45      1920x1080    312.4 MB  
presentation.mp4                            45:20      1280x720     189.2 MB  
webinar_recording.mp4                       1:32:15    1920x1080    1.2 GB    
short_clip.mp4                              2:30       854x480      15.8 MB   

================================================================================
Totals:
  Files: 5
  Total Duration: 2:54:22 (2.91 hours)
  Total Size: 1.9 GB

  Videos > 1 hour: 1
    - webinar_recording.mp4 (1:32:15)

================================================================================
```

**JSON Output:**
```json
{
  "TotalFiles": 5,
  "TotalDuration": 10462,
  "TotalSize": 2040109465,
  "Videos": [
    {
      "FileName": "tutorial_part1.mp4",
      "Path": "/home/user/Videos/tutorial_part1.mp4",
      "DurationFormatted": "15:32",
      "Resolution": "1920x1080",
      "Codec": "H.264/H.265 (MP4)",
      "Bitrate": 2500000,
      "Size": 257698816,
      "HasAudio": true
    }
  ]
}
```

## Supported Formats

| Format | Extensions |
|--------|------------|
| MP4 | .mp4, .m4v |
| AVI | .avi |
| Matroska | .mkv |
| QuickTime | .mov |
| Windows Media | .wmv, .asf |
| Flash Video | .flv |
| WebM | .webm |
| MPEG | .mpeg, .mpg |

## Sort Options

| Option | Description |
|--------|-------------|
| `name` (default) | Sort alphabetically by filename |
| `duration` | Sort by duration (shortest first) |
| `duration_desc` | Sort by duration (longest first) |
| `size` | Sort by file size (smallest first) |
| `size_desc` | Sort by file size (largest first) |
| `date` | Sort by creation date (oldest first) |

## Concepts Demonstrated

- Video file metadata extraction (TagLibSharp)
- Duration parsing and formatting
- File size formatting
- Duration-based filtering
- Recursive directory traversal
- Multiple output formats (table, JSON)
- Sorting by different criteria
- Batch file processing
- Error handling for unsupported/corrupt files
- Command-line argument parsing
