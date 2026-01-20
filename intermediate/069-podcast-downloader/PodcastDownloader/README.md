# Podcast Downloader

A command-line tool that downloads podcast episodes from RSS feeds to your local disk.

## Usage

```bash
dotnet run --project PodcastDownloader.csproj <rss-url> [output-dir] [max-episodes]
```

### Arguments

- `rss-url` - The podcast RSS feed URL
- `output-dir` - Directory to save episodes (default: `./downloads`)
- `max-episodes` - Maximum number of episodes to download (default: 5)

## Examples

```bash
# Download 5 latest episodes to ./downloads
dotnet run --project PodcastDownloader.csproj https://feeds.example.com/podcast.xml

# Download to custom directory
dotnet run --project PodcastDownloader.csproj https://feeds.example.com/podcast.xml ./podcasts

# Download specific number of episodes
dotnet run --project PodcastDownloader.csproj https://feeds.example.com/podcast.xml ./podcasts 10
```

## Example Output

```
Fetching podcast feed: https://feeds.example.com/podcast.xml
Output directory: /home/user/podcasts
Max episodes: 5
------------------------------------------------------------

🎙️ Tech Talk Daily

↓ Downloading: Episode 125 - AI Revolution...
  Saved: ./podcasts/ep125.mp3 (15234 KB)
↓ Downloading: Episode 124 - Cloud Computing...
  Saved: ./podcasts/ep124.mp3 (12456 KB)
✓ Episode 123 - DevOps Best Practices (already exists)

Downloaded 2 episode(s) to ./podcasts
```

## Concepts Demonstrated

- RSS feed parsing with XML
- Enclosure element for media files
- HTTP file downloads
- File I/O and directory management
- Filename sanitization
- Progress reporting
- Skip existing files
