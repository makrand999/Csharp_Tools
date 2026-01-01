# QrGenerator

Generate QR codes as PNG images or ASCII art.

## Usage

```bash
# Create PNG QR code
dotnet run --project QrGenerator.csproj -- -t "https://example.com"

# Save PNG to specific file
dotnet run --project QrGenerator.csproj -- -t "Hello" -o myqr.png

# Generate ASCII QR code (print to terminal)
dotnet run --project QrGenerator.csproj -- -t "Hello" --ascii

# Save ASCII QR code to file
dotnet run --project QrGenerator.csproj -- -t "Hello" --ascii -o qr.txt -s 5
```

## Options

| Option | Description |
|--------|-------------|
| `-t, --text <text>` | Text to encode in QR code |
| `-o, --output <file>` | Output file |
| `-s, --size <n>` | Size multiplier for ASCII art (default: 10, range: 1-50) |
| `--ascii` | Generate ASCII art instead of PNG |

## Build

```bash
dotnet build -c Release
```
