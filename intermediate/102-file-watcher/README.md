# File Watcher

A practical utility to monitor directories for file system changes in real-time. Useful for watching build outputs, sync folders, download directories, or detecting new files.

## Usage

```bash
dotnet run --project FileWatcher.csproj -- <paths...> [options]
```

## Arguments

| Argument | Description |
|----------|-------------|
| `paths` | One or more directories to watch |

## Options

| Option | Description |
|--------|-------------|
| `--filter <f>` | File filters, comma-separated (e.g., `*.cs,*.json`) |
| `--no-subdirs` | Don't watch subdirectories |
| `--verbose` | Show detailed event information |
| `--help` | Show help |

## Examples

**Watch current directory:**
```bash
dotnet run --project FileWatcher.csproj -- .
```

**Watch multiple directories with filter:**
```bash
dotnet run --project FileWatcher.csproj -- ./src ./test --filter '*.cs'
```

**Watch Downloads folder for specific file types:**
```bash
dotnet run --project FileWatcher.csproj -- ~/Downloads --filter '*.pdf,*.zip' --no-subdirs
```

**Watch with all options:**
```bash
dotnet run --project FileWatcher.csproj -- /var/log --filter '*.log' --verbose
```

## Example Output

```
File Watcher - Monitoring for changes
Press Ctrl+C to stop

Watching: /home/user/Downloads

Monitoring 1 watcher(s)...
Events will be logged below:

[14:32:15] Created    /home/user/Downloads/document.pdf
[14:32:18] Changed    /home/user/Downloads/document.pdf
[14:32:45] Renamed    /home/user/Downloads/document.pdf -> /home/user/Downloads/report.pdf
[14:33:00] Deleted    /home/user/Downloads/old-file.zip
```

## Concepts Demonstrated

- `FileSystemWatcher` class for file system monitoring
- Event handling (Created, Changed, Deleted, Renamed)
- Multi-directory watching with multiple watchers
- Thread-safe logging with locking
- Console color output for different event types
- Graceful shutdown with Ctrl+C handling
- IDisposable pattern for cleanup
