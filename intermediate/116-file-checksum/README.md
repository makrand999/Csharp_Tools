# FileChecksumVerifier

Verify file integrity with cryptographic checksums. Compute, verify, and compare file hashes using multiple algorithms (MD5, SHA1, SHA256, SHA384, SHA512).

## Usage

```bash
dotnet run --project FileChecksum.csproj -- hash <file> [algorithm]
dotnet run --project FileChecksum.csproj -- hash <text> --text
dotnet run --project FileChecksum.csproj -- verify <file> <expected-hash>
dotnet run --project FileChecksum.csproj -- compare <file1> <file2>
```

## Supported Algorithms

| Algorithm | Bit Length | Hex Length | Use Case |
|-----------|------------|------------|----------|
| MD5 | 128-bit | 32 chars | Fast checksums (not secure) |
| SHA1 | 160-bit | 40 chars | Legacy systems |
| SHA256 | 256-bit | 64 chars | **Recommended** (default) |
| SHA384 | 384-bit | 96 chars | High security |
| SHA512 | 512-bit | 128 chars | Maximum security |

## Examples

```bash
# Compute hash (default: SHA256)
dotnet run --project FileChecksum.csproj -- hash myfile.zip
dotnet run --project FileChecksum.csproj -- hash myfile.zip SHA512

# Hash text content
dotnet run --project FileChecksum.csproj -- hash "Hello World" --text

# Verify file against known hash
dotnet run --project FileChecksum.csproj -- verify myfile.zip a591a6d40bf420404a011733cfb7b190d2905da0e9c30e2c4d1a3f8e9b7c6d5e

# Compare two files
dotnet run --project FileChecksum.csproj -- compare file1.txt file2.txt
```

## Sample Output

```
# Computing hash
SHA256 Hash:
  myfile.zip
  a591a6d40bf420404a011733cfb7b190d2905da0e9c30e2c4d1a3f8e9b7c6d5e

For verification: dotnet run -- verify myfile.zip a591a6d40bf420404a011733cfb7b190...

# Verifying hash
Verifying myfile.zip...
Algorithm: SHA256
Expected: a591a6d40bf420404a011733cfb7b190...
Actual:   a591a6d40bf420404a011733cfb7b190...

✓ Hash verified - File is intact!

# Comparing files
File Comparison:
  file1.txt (1,024 bytes)
  SHA256: abc123...

  file2.txt (1,024 bytes)
  SHA256: abc123...

✓ Files are identical
```

## Use Cases

- **Download verification**: Verify downloaded files match publisher's checksums
- **Backup integrity**: Confirm backup files haven't been corrupted
- **File comparison**: Check if two files are identical without opening them
- **Tamper detection**: Verify files haven't been modified

## Concepts Demonstrated

- Cryptographic hash functions (System.Security.Cryptography)
- Hash algorithm selection and abstraction
- File I/O and binary data handling
- Hash format detection by length
- File comparison algorithms
- BitConverter for byte array conversion
