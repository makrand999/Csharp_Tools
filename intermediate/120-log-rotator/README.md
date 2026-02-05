# LogRotator

Automatic log file rotation and compression. Manage log files with size-based rotation, gzip compression, and cleanup of old archives.

## Usage

```bash
dotnet run --project LogRotator.csproj -- rotate <log-file> [options]
dotnet run --project LogRotator.csproj -- compress <log-file> [output-file]
dotnet run --project LogRotator.csproj -- compress <directory>
dotnet run --project LogRotator.csproj -- cleanup <directory> [options]
dotnet run --project LogRotator.csproj -- status <directory>
```

## Commands

| Command | Description |
|---------|-------------|
| `rotate` | Rotate log file when size exceeded |
| `compress` | Compress log files with gzip |
| `cleanup` | Remove old rotated logs |
| `status` | Show log directory status |

## Rotate Options

| Option | Description |
|--------|-------------|
| `--max-size <bytes>` | Rotate when file exceeds this size (default: 10MB) |
| `--max-files <count>` | Keep this many rotated files (default: 5) |
| `--compress` | Compress rotated files |
| `--pattern <pattern>` | Rotated file pattern (default: `{file}.{n}.gz`) |

## Cleanup Options

| Option | Description |
|--------|-------------|
| `--older-than <days>` | Remove files older than this (default: 30) |
| `--pattern <pattern>` | File pattern (default: `*.gz`) |
| `--dry-run` | Show what would be deleted |

## Examples

### Rotate Log Files

```bash
# Rotate when exceeding 5MB, keep 5 backups
dotnet run --project LogRotator.csproj -- rotate app.log --max-size 5MB --compress

# Custom naming pattern
dotnet run --project LogRotator.csproj -- rotate server.log --pattern "{file}.backup.{n}"
```

### Compress Logs

```bash
# Compress single file
dotnet run --project LogRotator.csproj -- compress app.log

# Compress all logs in directory
dotnet run --project LogRotator.csproj -- compress ./logs
```

**Sample Output:**
```
✓ Compressed: app.log
  Original: 1,048,576 bytes
  Compressed: 52,428 bytes (95.0% reduction)
```

### Cleanup Old Logs

```bash
# Preview cleanup
dotnet run --project LogRotator.csproj -- cleanup ./logs --older-than 7 --dry-run

# Actually delete old files
dotnet run --project LogRotator.csproj -- cleanup ./logs --older-than 30
```

**Sample Output:**
```
Dry run: 5 file(s) would be deleted (10,485,760 bytes)

✓ Cleaned up 5 old log file(s) (10,485,760 bytes freed)
```

### Check Log Status

```bash
dotnet run --project LogRotator.csproj -- status ./logs
```

**Sample Output:**
```
Log Status: ./logs
--------------------------------------------------
Active log files: 3 (1,048,576 bytes)
Compressed archives: 12 (524,288 bytes)

Active logs:
  app.log                           524,288 bytes
  error.log                         262,144 bytes
  access.log                        262,144 bytes

Compressed archives:
  app.log.1.gz                       52,428 bytes  2026-03-31
  app.log.2.gz                       48,576 bytes  2026-03-30
```

## Rotation Pattern

Files are rotated with numeric suffixes:
```
app.log          (current)
app.log.1.gz     (most recent backup)
app.log.2.gz
app.log.3.gz
...
app.log.5.gz     (oldest backup)
```

## Use Cases

- **Web server logs**: Rotate access/error logs daily
- **Application logging**: Prevent disk space exhaustion
- **Archive management**: Compress old logs for storage
- **Compliance**: Maintain log history for auditing

## Concepts Demonstrated

- GZipStream for compression
- File I/O and stream copying
- Directory traversal and file filtering
- Date/time calculations for cleanup
- Pattern-based file naming
- Dry-run preview functionality
