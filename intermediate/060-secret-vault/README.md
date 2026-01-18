# Secret Vault - CLI Credential Manager

A secure CLI tool for storing and managing secrets, API keys, and credentials.

## Usage

```bash
dotnet run --project SecretVault.csproj
```

## Commands

| Command | Description |
|---------|-------------|
| `add`   | Add a new secret (name, value, category) |
| `list`  | List all secret names with categories |
| `get`   | Retrieve and display a specific secret |
| `delete`| Delete a secret from the vault |
| `export`| Export vault to a file |
| `quit`  | Exit the vault |

## Example

```
Secret Vault - Credential Manager
==================================

Commands:
  add     - Add a new secret
  list    - List all secret names
  get     - Retrieve a secret
  delete  - Delete a secret
  export  - Export vault (encrypted)
  quit    - Exit the vault

vault> add
Secret name: github_api_key
Secret value: ***************
Category (optional): api
✅ Secret 'github_api_key' added.

vault> list

📋 Secrets in vault:
--------------------------------------------------
   [api] github_api_key
      Created: 2026-03-31 10:30:00
--------------------------------------------------
Total: 1 secrets

vault> get github_api_key

🔑 github_api_key:
   Value: ghp_xxxxxxxxxxxx
   Category: api
   Created: 2026-03-31 10:30:00
   Updated: 2026-03-31 10:30:00

vault> quit
🔒 Vault locked. Goodbye!
```

## Concepts Demonstrated

- Secure password input (masked characters)
- JSON serialization for data persistence
- File-based storage in user profile directory
- Interactive REPL-style command loop
- Dictionary-based data management
- DateTime tracking for audit purposes
- Category-based organization
- Cryptographic random key generation
