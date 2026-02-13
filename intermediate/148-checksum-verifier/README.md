# Checksum Verifier

Compute and verify file checksums using various hash algorithms.

## Usage

```bash
dotnet run --project 148-checksum-verifier.csproj -- <file> [expected-hash]
```

## Example

```bash
dotnet run --project 148-checksum-verifier.csproj -- file.zip
dotnet run --project 148-checksum-verifier.csproj -- file.zip --md5
dotnet run --project 148-checksum-verifier.csproj -- file.zip abc123... --verify --sha256
```

## Options

- `--md5` - Use MD5 (128-bit)
- `--sha1` - Use SHA1 (160-bit)
- `--sha256` - Use SHA256 (256-bit, default)
- `--sha512` - Use SHA512 (512-bit)
- `--verify` - Verify against expected hash

## Concepts Demonstrated

- Cryptographic hashing
- MD5, SHA1, SHA256, SHA512
- File stream processing
- Hash verification
- Byte array conversion
