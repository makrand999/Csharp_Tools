# PasswordGenerator

Generates cryptographically secure random passwords with customizable options.

## Usage

```bash
# Generate default 16-character password
dotnet run --project PasswordGenerator.csproj

# Generate 32-character password
dotnet run --project PasswordGenerator.csproj -- -l 32

# Generate without special characters
dotnet run --project PasswordGenerator.csproj -- --no-special

# Show help
dotnet run --project PasswordGenerator.csproj -- --help
```

## Options

| Option | Description |
|--------|-------------|
| `-l, --length <n>` | Password length (default: 16, range: 4-128) |
| `--no-upper` | Exclude uppercase letters |
| `--no-lower` | Exclude lowercase letters |
| `--no-digits` | Exclude digits |
| `--no-special` | Exclude special characters |

## Build

```bash
dotnet build -c Release
```
