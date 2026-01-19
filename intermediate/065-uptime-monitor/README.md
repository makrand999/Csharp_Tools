# Website Uptime Monitor

CLI tool for monitoring website availability and response times.

## Usage

```bash
# Check single website
dotnet run --project UptimeMonitor/UptimeMonitor.csproj check https://google.com

# Check multiple websites from file
dotnet run --project UptimeMonitor/UptimeMonitor.csproj batch websites.txt

# Continuously monitor with 30-second intervals
dotnet run --project UptimeMonitor/UptimeMonitor.csproj watch https://api.example.com 30
```

## Example

```
$ dotnet run --project UptimeMonitor/UptimeMonitor.csproj batch websites.txt
Checking 5 websites...

✓ UP     https://google.com
         Status: OK | Response: 45ms
✓ UP     https://github.com
         Status: OK | Response: 120ms
✗ DOWN   https://nonexistent-domain-12345.com
         Error: Name or service not known
✓ UP     https://api.example.com
         Status: OK | Response: 230ms
✗ DOWN   https://down-server.com
         Status: ServiceUnavailable | Response: 150ms

--- Summary ---
Total: 5
Up: 3 (60.0%)
Down: 2 (40.0%)
Avg Response Time: 132ms
```

## Features

- **Single URL check** - Quick status check for one website
- **Batch checking** - Monitor multiple URLs concurrently
- **Continuous monitoring** - Watch mode with configurable intervals
- **Response time tracking** - Measures latency in milliseconds
- **Uptime calculation** - Tracks uptime percentage over time

## Concepts Demonstrated

- Async/await for concurrent HTTP requests
- HttpClient for web requests
- Concurrent collections (ConcurrentBag)
- Stopwatch for performance measurement
- Continuous monitoring loop
- Error handling for network operations
