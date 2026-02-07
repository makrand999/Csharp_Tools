# User Agent Parser

Parse and analyze browser User-Agent strings to detect browser, OS, and device.

## Usage

```bash
dotnet run --project 129-user-agent-parser.csproj -- "<user-agent-string>"
```

## Example

```bash
dotnet run --project 129-user-agent-parser.csproj -- "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36"
```

### Sample Output

```
=== User Agent Analysis ===

Original: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36...

=== Parsed Information ===
Browser:    Chrome 120.0.0
OS:         Windows 10
Device:     Desktop
Platform:   Windows
Is Mobile:  False
Is Bot:     False
```

## Detection Capabilities

- **Browsers:** Chrome, Firefox, Safari, Edge, Opera, IE
- **OS:** Windows, macOS, Linux, Android, iOS, Chrome OS
- **Devices:** Desktop, Mobile, Tablet, TV, Wearable
- **Bots:** Googlebot, Bingbot, and other common crawlers

## Concepts Demonstrated

- Regular expressions
- String pattern matching
- Conditional logic
- Object-oriented design
- User-Agent string parsing
