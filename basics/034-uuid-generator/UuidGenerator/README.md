# UUID/GUID Generator

A practical CLI tool for generating universally unique identifiers (UUIDs/GUIDs) in various versions and output formats.

## Usage

```bash
dotnet run --project UuidGenerator.csproj [options]
```

### Options

| Option | Description |
|--------|-------------|
| `-v, --version <v>` | UUID version: `v1`, `v4` (default), `v5`, `v7` |
| `-n, --count <n>` | Number of UUIDs to generate (max 1000) |
| `-f, --format <f>` | Output format: `canonical`, `braced`, `urn`, `hex` |
| `--ns <namespace>` | Namespace GUID for v5 generation |
| `--name <name>` | Name for v5 generation |
| `-h, --help` | Show help message |

### UUID Versions

| Version | Type | Use Case |
|---------|------|----------|
| **v1** | Time-based (MAC address) | Legacy, temporal ordering |
| **v4** | Random | **Default**, general purpose |
| **v5** | Name-based (SHA-1) | Deterministic, reproducible IDs |
| **v7** | Timestamp-based | **Recommended** - sortable, modern |

### Output Formats

| Format | Example |
|--------|---------|
| `canonical` | `123e4567-e89b-12d3-a456-426614174000` |
| `braced` | `{123e4567-e89b-12d3-a456-426614174000}` |
| `urn` | `urn:uuid:123e4567-e89b-12d3-a456-426614174000` |
| `hex` | `123e4567e89b12d3a456426614174000` |

## Examples

```bash
# Generate a random UUID v4 (default)
dotnet run --project UuidGenerator.csproj
# Output: 550e8400-e29b-41d4-a716-446655440000

# Generate 5 UUIDs
dotnet run --project UuidGenerator.csproj -n 5

# Generate timestamp-based UUID v7 (sortable)
dotnet run --project UuidGenerator.csproj -v v7

# Generate in hex format (no hyphens)
dotnet run --project UuidGenerator.csproj -f hex

# Generate UUID v5 (name-based, deterministic)
dotnet run --project UuidGenerator.csproj -v v5 \
  --ns 6ba7b810-9dad-11d1-80b4-00c04fd430c8 \
  --name "example.com"

# Generate UUID v7 in URN format
dotnet run --project UuidGenerator.csproj -v v7 -f urn
```

## Use Cases

- **Database Primary Keys**: Use v4 or v7 for unique IDs
- **Distributed Systems**: Generate unique IDs without coordination
- **Session Tokens**: Create unique session identifiers
- **File Naming**: Generate unique filenames for uploads
- **Testing**: Create reproducible test data with v5
- **Logging**: Add correlation IDs to log entries

## Common Namespaces for v5

| Namespace | UUID |
|-----------|------|
| DNS | `6ba7b810-9dad-11d1-80b4-00c04fd430c8` |
| URL | `6ba7b811-9dad-11d1-80b4-00c04fd430c8` |
| OID | `6ba7b812-9dad-11d1-80b4-00c04fd430c8` |
| X.500 DN | `6ba7b814-9dad-11d1-80b4-00c04fd430c8` |

## Concepts Demonstrated

- `Guid` struct and parsing
- Cryptographic random number generation (`RandomNumberGenerator`)
- SHA-1 hashing for UUID v5
- Unix timestamp conversion
- Byte array manipulation and endianness
- Command-line argument parsing
- Pattern matching with switch expressions
