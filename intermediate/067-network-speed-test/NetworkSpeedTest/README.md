# Network Speed Test

A simple network utility that measures your internet download speed using Cloudflare's speed test endpoint.

## Usage

```bash
dotnet run --project NetworkSpeedTest.csproj
```

## Example

```
Network Speed Test
--------------------------------------------------
Testing download speed from Cloudflare...
--------------------------------------------------

Results:
  Downloaded: 1,048,576 bytes (1.00 MB)
  Time: 0.85 seconds
  Speed: 98.76 Mbps
```

## How It Works

1. Downloads a 1MB test file from Cloudflare's speed test server
2. Measures the time taken to complete the download
3. Calculates and displays the download speed in Mbps

## Concepts Demonstrated

- HTTP client for network requests
- Stopwatch for precise timing
- Stream reading and byte counting
- Async/await for non-blocking I/O
- Network speed calculations
- Exception handling for network errors
