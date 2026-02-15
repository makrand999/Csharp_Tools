# Async Batch Downloader

A high-performance async downloader for downloading multiple files concurrently with progress tracking and cancellation support.

## Usage

```bash
dotnet run --project AsyncBatchDownloader.csproj [options]
```

### Options

| Option | Description | Default |
|--------|-------------|---------|
| `--urls <file>` | File with URLs (one per line) | Required |
| `--output <dir>` | Output directory | ./downloads |
| `--concurrency <n>` | Max concurrent downloads | 5 |
| `--no-progress` | Disable progress display | false |
| `--timeout <seconds>` | Download timeout | 300 |

## Examples

```bash
# Download files from URL list
dotnet run --project AsyncBatchDownloader.csproj --urls urls.txt

# Download with higher concurrency
dotnet run --project AsyncBatchDownloader.csproj --urls urls.txt --concurrency 10

# Custom output directory
dotnet run --project AsyncBatchDownloader.csproj --urls urls.txt --output ./files
```

## URLs File Format

```
https://example.com/file1.zip
https://example.com/file2.pdf
https://example.com/image.png
https://releases.ubuntu.com/22.04/ubuntu-22.04.iso
```

## Example Output

```
Async Batch Downloader
=======================
URLs file: urls.txt
Output directory: /home/user/downloads
Total URLs: 10
Max concurrent: 5
Timeout: 300s

Press Ctrl+C to cancel all downloads

Starting: file1.zip
Starting: file2.pdf
Starting: image.png
  file1.zip: 45.2% (4.52 MB/10.0 MB)
  ✓ file1.zip completed (10.0 MB in 2.3s)
Starting: document.docx
  ✓ image.png completed (2.5 MB in 1.1s)
  ✓ file2.pdf completed (5.2 MB in 3.4s)
...

======================================================================
Download completed in 15.4s
Total downloads: 10
Successful: 9
Failed: 1
Total data: 125.5 MB

Average speed: 8.15 MB/s

Downloaded files:
  ✓ document.docx (3.2 MB)
  ✓ file1.zip (10.0 MB)
  ✓ file2.pdf (5.2 MB)
  ✓ image.png (2.5 MB)
```

## Concepts Demonstrated

- **Async/Await** - Non-blocking I/O operations
- **SemaphoreSlim** - Concurrency limiting
- **CancellationToken** - Cooperative cancellation
- **HttpClient** - Async HTTP operations
- **FileStream** - Async file operations
- **Progress reporting** - Real-time download progress
- **ConcurrentBag<T>** - Thread-safe result collection
- **Task.WhenAll** - Parallel task coordination
- **Stream processing** - Efficient data transfer
