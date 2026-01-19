# Port Scanner

A network utility that scans a host for open TCP ports within a specified range.

## Usage

```bash
dotnet run --project PortScanner.csproj [host] [port-range]
```

### Arguments

- `host` - Target hostname or IP address (default: localhost)
- `port-range` - Port range to scan (default: 1-1024). Can be single port or range like "80-443"

## Examples

```bash
# Scan localhost ports 1-1024
dotnet run --project PortScanner.csproj

# Scan specific host
dotnet run --project PortScanner.csproj 192.168.1.1

# Scan specific port range
dotnet run --project PortScanner.csproj google.com 80-443

# Scan single port
dotnet run --project PortScanner.csproj localhost 443
```

## Example Output

```
Scanning localhost for open ports...
Port range: 1-1024
--------------------------------------------------
Port    22: OPEN
Port    80: OPEN
Port   443: OPEN
Port  3306: OPEN
--------------------------------------------------
Scan completed in 245ms
Found 4 open port(s): 22, 80, 443, 3306
```

## Concepts Demonstrated

- TCP socket connections
- Async/await for network operations
- Timeout handling with Task.WhenAny
- Port range parsing
- Network diagnostics
