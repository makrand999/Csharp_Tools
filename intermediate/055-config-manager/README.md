# Config Manager

A practical JSON configuration file manager for reading, writing, validating, and manipulating `appsettings.json` and other JSON config files.

## Usage

```bash
dotnet run --project ConfigManager.csproj [command] [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `read <config>` | Display entire configuration |
| `get <config> <key>` | Get value by key (dot or colon notation) |
| `set <config> <key> <value>` | Set value by key |
| `validate <config>` | Validate JSON syntax and structure |
| `env <config>` | Substitute `${ENV_VAR}` placeholders |
| `merge <base> <overlay>` | Merge two configurations |
| `flatten <config>` | Show all flattened key paths |

## Examples

```bash
# Read entire config
dotnet run --project ConfigManager.csproj read appsettings.json

# Get a nested value
dotnet run --project ConfigManager.csproj get appsettings.json Database:ConnectionString
dotnet run --project ConfigManager.csproj get appsettings.json Logging.LogLevel.Default

# Set a value
dotnet run --project ConfigManager.csproj set appsettings.json Database:Host localhost

# Validate config
dotnet run --project ConfigManager.csproj validate appsettings.json

# Substitute environment variables
dotnet run --project ConfigManager.csproj env appsettings.json

# Merge base config with production overlay
dotnet run --project ConfigManager.csproj merge appsettings.json appsettings.production.json

# Show flattened keys
dotnet run --project ConfigManager.csproj flatten appsettings.json
```

## Sample Output

```
$ dotnet run --project ConfigManager.csproj get appsettings.json Database:Host
Database:Host = localhost

$ dotnet run --project ConfigManager.csproj validate appsettings.json
✓ Valid JSON: appsettings.json
  Total keys: 15

$ dotnet run --project ConfigManager.csproj flatten appsettings.json
Flattened keys from 'appsettings.json':

  Database:Host
  Database:Port
  Database:ConnectionString
  Logging:LogLevel:Default
  Logging:LogLevel:Microsoft
  Features:EnableCache
  Features:MaxConnections
```

## Environment Variable Substitution

The tool supports `${ENV_VAR}` syntax for environment variable substitution:

```json
{
  "Database": {
    "Host": "${DB_HOST}",
    "Password": "${DB_PASSWORD}"
  }
}
```

## Concepts Demonstrated

- JSON parsing with `System.Text.Json`
- `JsonNode` for mutable JSON manipulation
- Nested object traversal with dot/colon notation
- Environment variable substitution with regex
- Configuration merging strategies
- Recursive tree traversal
- Type-aware value setting (int, double, bool, string)
- Key path flattening for nested structures
