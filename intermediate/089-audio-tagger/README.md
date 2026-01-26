# Audio Tagger

Read and write metadata tags (ID3, Vorbis, etc.) for audio files. Supports MP3, OGG, FLAC, M4A, WAV, WMA, AAC, and Opus formats.

## Usage

```bash
# Read tags
dotnet run --project AudioTagger.csproj song.mp3

# Read tags as JSON
dotnet run --project AudioTagger.csproj --json song.mp3

# Write tags
dotnet run --project AudioTagger.csproj --write --title "My Song" --artist "Artist Name" song.mp3

# Write multiple tags
dotnet run --project AudioTagger.csproj -w -t "Song Title" -a "Artist" -l "Album" -y 2024 -g "Rock" song.mp3

# Add cover art
dotnet run --project AudioTagger.csproj -w --cover cover.jpg song.mp3

# Remove cover art
dotnet run --project AudioTagger.csproj -w --remove-cover song.mp3

# Process multiple files
dotnet run --project AudioTagger.csproj *.mp3
```

## Example

**Read Output:**
```
============================================================
song.mp3
============================================================

🎵 Audio Properties:
  Duration:   03:45
  Bitrate:    320 kbps
  SampleRate: 44100 Hz
  Channels:   2

🏷️  Tags:
  Title       : My Favorite Song
  Artist      : The Band Name
  Album       : Greatest Hits
  AlbumArtist : The Band Name
  Year        : 2024
  Genre       : Rock
  Track       : 5
  Disc        : 1
  Comment     : Recorded at Studio X
  Cover Art:  Yes (1 image(s))
```

**JSON Output:**
```json
{
  "File": "song.mp3",
  "Path": "/home/user/music/song.mp3",
  "Type": "Mpeg3",
  "Duration": "03:45",
  "Bitrate": "320 kbps",
  "SampleRate": "44100 Hz",
  "Channels": 2,
  "Title": "My Favorite Song",
  "Artist": "The Band Name",
  "Album": "Greatest Hits",
  "Year": "2024",
  "Genre": "Rock",
  "Track": "5",
  "Disc": "1",
  "Comment": "Recorded at Studio X",
  "HasCoverArt": true,
  "CoverArtCount": 1
}
```

## Supported Tag Fields

| Field | Description |
|-------|-------------|
| Title | Track title |
| Artist | Performer/artist name |
| Album | Album name |
| AlbumArtist | Album artist (for compilations) |
| Year | Release year |
| Genre | Music genre |
| Track | Track number |
| Disc | Disc number |
| Composer | Composer name |
| Comment | Free-form comment |
| Cover Art | Embedded album art |

## Supported Formats

- **MP3** - ID3v1, ID3v2.3, ID3v2.4
- **OGG** - Vorbis comments
- **FLAC** - Vorbis comments + pictures
- **M4A/AAC** - MP4 atoms
- **WAV** - INFO tags
- **WMA** - ASF tags
- **Opus** - Vorbis comments

## Concepts Demonstrated

- Audio file metadata reading/writing (TagLibSharp)
- ID3 tag manipulation
- Vorbis comments handling
- Cover art extraction and embedding
- Audio property extraction (duration, bitrate, sample rate)
- Multiple audio format support
- JSON serialization
- Command-line argument parsing
- Batch file processing
- Error handling for corrupted files
