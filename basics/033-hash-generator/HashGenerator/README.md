# Hash Generator

A practical CLI tool for generating cryptographic hashes of text. Supports MD5, SHA-1, SHA-256, SHA-384, and SHA-512 algorithms.

## Usage

```bash
dotnet run --project HashGenerator.csproj <algorithm> <text>
```

### Supported Algorithms

| Algorithm | Bits | Security Level | Use Case |
|-----------|------|----------------|----------|
| `md5` | 128 | ❌ Broken | Checksums, non-security |
| `sha1` | 160 | ⚠️ Weak | Legacy systems |
| `sha256` | 256 | ✅ Strong | **Recommended** for security |
| `sha384` | 384 | ✅ Very Strong | High-security applications |
| `sha512` | 512 | ✅ Very Strong | Maximum security |

## Examples

```bash
# Generate SHA-256 hash
dotnet run --project HashGenerator.csproj sha256 "Hello World"
# Output: a591a6d40bf420404a011733cfb7b190d62c65bf0bcda32b57b277d9ad9f146e

# Generate MD5 hash (for checksums)
dotnet run --project HashGenerator.csproj md5 "password123"
# Output: 482c811da5d5b4bc6d497ffa98491138

# Generate SHA-512 hash
dotnet run --project HashGenerator.csproj sha512 "SecurePassword!"
# Output: (128-character hex string)

# Pipe input from another command
echo "Hello" | dotnet run --project HashGenerator.csproj sha256 -
```

## Use Cases

- **Password Verification**: Generate hashes for password comparison (use SHA-256+)
- **File Integrity**: Create checksums for file verification
- **Data Deduplication**: Identify duplicate content by hash
- **Digital Signatures**: Prepare data for signing
- **Blockchain/Crypto**: Generate hashes for cryptographic operations

## Security Notes

⚠️ **Important**: 
- MD5 and SHA-1 are cryptographically broken and should NOT be used for security purposes
- For password hashing, use dedicated algorithms like bcrypt, Argon2, or PBKDF2
- SHA-256 or higher is recommended for cryptographic applications

## Concepts Demonstrated

- Cryptographic hash functions (`System.Security.Cryptography`)
- `HashData` static methods (.NET 8+)
- Byte array manipulation
- Hex string conversion
- Pattern matching with switch expressions
- UTF-8 encoding
