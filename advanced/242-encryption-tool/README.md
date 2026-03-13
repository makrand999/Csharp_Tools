# Encryption Tool

Encrypts and decrypts files using AES-256 symmetric encryption with PBKDF2 key derivation.

## Usage

```bash
dotnet run --project EncryptionTool.csproj
```

## Example

```
=== Encryption Tool ===

Using AES-256 encryption with PBKDF2 key derivation

Creating test file...
Original size: 10047 bytes

Original SHA-256: a1b2c3d4e5f6...

Encrypting with AES-256...
Encrypted size: 10064 bytes
Time: 25ms

Decrypting...
Decrypted size: 10047 bytes
Time: 20ms

Verification: ✓ PASSED
```

## Concepts Demonstrated

- Aes encryption class
- Rfc2898DeriveBytes (PBKDF2)
- CryptoStream for encrypted I/O
- SHA-256 hash computation
- Salt and IV handling
