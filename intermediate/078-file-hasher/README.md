# FileHasher

Generate and verify file checksums using multiple hash algorithms (MD5, SHA1, SHA256, SHA512).

## Usage

```bash
# Generate hash
dotnet run --project FileHasher.csproj <file> [options]

# Verify hash
dotnet run --project FileHasher.csproj <file> --verify <hashfile>
```

## Options

| Option | Description |
|--------|-------------|
| `--md5` | Use MD5 algorithm |
| `--sha1` | Use SHA1 algorithm |
| `--sha256` | Use SHA256 algorithm (default) |
| `--sha512` | Use SHA512 algorithm |
| `--algo <name>` | Specify algorithm explicitly |
| `--verify <file>` | Verify against stored hash file |
| `--json` | Output as JSON |

## Examples

```bash
# Generate SHA256 hash (default)
dotnet run --project FileHasher.csproj ubuntu.iso

# Generate MD5 hash
dotnet run --project FileHasher.csproj archive.zip --md5

# Generate SHA512 hash
dotnet run --project FileHasher.csproj backup.tar --sha512

# Output as JSON
dotnet run --project FileHasher.csproj data.db --json

# Verify against checksum file
dotnet run --project FileHasher.csproj ubuntu.iso --verify SHA256SUMS
```

## Example Output

```
# Standard output (compatible with sha256sum format)
a1b2c3d4e5f6...  ubuntu.iso

# JSON output
{
  "file": "/home/user/downloads/ubuntu.iso",
  "fileName": "ubuntu.iso",
  "size": 5368709120,
  "algorithm": "SHA256",
  "hash": "A1B2C3D4E5F6..."
}

# Verification output
File: ubuntu.iso
Stored hash:  a1b2c3d4e5f6...
Computed hash: a1b2c3d4e5f6...
Algorithm: SHA256

✓ Hash verification PASSED
```

## Concepts Demonstrated

- Cryptographic hash functions (MD5, SHA1, SHA256, SHA512)
- File stream processing
- Hash verification and comparison
- Multiple algorithm support via pattern matching
- JSON output formatting
- Checksum file parsing (sha256sum format)
- Binary-to-hex conversion
