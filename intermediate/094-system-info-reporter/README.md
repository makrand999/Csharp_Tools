# System Info Reporter

Display comprehensive system information including OS details, hardware specs, runtime stats, network configuration, and disk space.

## Usage

```bash
# Full report
dotnet run --project SystemInfoReporter.csproj

# Brief summary
dotnet run --project SystemInfoReporter.csproj --brief

# JSON output
dotnet run --project SystemInfoReporter.csproj --json
```

## Examples

### Full Report

```
$ dotnet run --project SystemInfoReporter.csproj

═══════════════════════════════════════════════════════════
                    SYSTEM INFORMATION                      
═══════════════════════════════════════════════════════════

📌 OPERATING SYSTEM
  OS:              Linux 6.5.0-28-generic
  Architecture:    X64
  Process Arch:    X64
  .NET Version:    .NET 8.0.5
  Runtime:         8.0.5

💻 HARDWARE
  Processor Count: 8 cores
  Memory:          16 GB
  Available Mem:   8.5 GB

👤 USER & MACHINE
  Machine Name:    mycomputer
  User Name:       max
  User Domain:     mycomputer
  Is Admin:        Yes

📁 ENVIRONMENT
  Current Dir:     /home/max/project
  System Dir:      /lib
  Temp Path:       /tmp/
  Tick Count:      1234567890 ms
  Uptime:          2d 5h 30m

🌐 NETWORK
  Hostname:        mycomputer
  IP Addresses:    127.0.0.1, 192.168.1.100

📊 RUNTIME STATS
  GC Count:        15 (Gen 0)
                 5 (Gen 1)
                 2 (Gen 2)
  Total Memory:    1,234,567 bytes
  Working Set:     45,678,901 bytes

📀 DISK SPACE
  /      50.2 GB free / 256 GB total (ext4)
  /home  100.5 GB free / 512 GB total (ext4)

═══════════════════════════════════════════════════════════
```

### Brief Output

```
$ dotnet run --project SystemInfoReporter.csproj --brief

Linux 6.5.0-28-generic | mycomputer | 8 cores | 16 GB RAM
.NET 8.0.5 | Uptime: 2d 5h 30m
```

### JSON Output

```json
{
  "operatingSystem": {
    "description": "Linux 6.5.0-28-generic",
    "architecture": "X64",
    "framework": ".NET 8.0.5"
  },
  "hardware": {
    "processorCount": 8,
    "totalMemory": "16 GB",
    "availableMemory": "8.5 GB"
  },
  "user": {
    "machineName": "mycomputer",
    "isAdmin": true
  },
  ...
}
```

## Concepts Demonstrated

- Cross-platform detection (Windows, Linux, macOS)
- RuntimeInformation API for system details
- Process and environment information
- DNS and network information retrieval
- File system and disk space analysis
- GC (Garbage Collector) statistics
- JSON serialization with System.Text.Json
- Linux /proc filesystem parsing
- Console formatting with colors and Unicode

## Cross-Platform Support

| Feature | Windows | Linux | macOS |
|---------|---------|-------|-------|
| OS Info | ✅ | ✅ | ✅ |
| Memory | ✅ | ✅ | ⚠️ Est. |
| Uptime | ✅ | ✅ | ⚠️ |
| Admin Check | ✅ | ✅ | ✅ |
| Disk Info | ✅ | ✅ | ✅ |
