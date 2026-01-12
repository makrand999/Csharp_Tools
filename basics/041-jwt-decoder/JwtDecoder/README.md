# JWT Decoder

A CLI tool for decoding JSON Web Tokens (JWT) and displaying their header and payload.

## Usage

```bash
dotnet run --project JwtDecoder.csproj <jwt_token>
```

## Examples

```bash
# Decode a JWT token
dotnet run --project JwtDecoder.csproj eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiIxMjM0NTY3ODkwIiwibmFtZSI6IkpvaG4gRG9lIiwiaWF0IjoxNTE2MjM5MDIyfQ.SflKxwRJSMeKKF2QT4fwpMeJf36POk6yJV_adQssw5c

# Output:
# === JWT Decoded ===
#
# Header:
# {
#   "alg": "HS256",
#   "typ": "JWT"
# }
#
# Payload:
# {
#   "sub": "1234567890",
#   "name": "John Doe",
#   "iat": 1516239022
# }
#
# Signature:
# SflKxwRJSMeKKF2QT4fwp...
#
# Issued At:  2018-01-18 01:30:22
```

## Features

- Decodes JWT header and payload
- Pretty-prints JSON output
- Shows expiration status (if `exp` claim present)
- Shows issued-at date (if `iat` claim present)
- Base64Url decoding with padding handling

## Concepts Demonstrated

- Base64Url decoding (JWT uses URL-safe base64)
- System.Text.Json for JSON parsing and formatting
- Unix timestamp conversion
- String manipulation and splitting
- Error handling for malformed tokens
