# YAML Converter

Bidirectional converter between YAML and JSON formats. Supports both file input and stdin for flexible pipeline usage.

## Usage

```bash
# YAML to JSON (default)
echo 'name: John\nage: 30' | dotnet run --project YamlConverter.csproj

# JSON to YAML
dotnet run --project YamlConverter.csproj --json-to-yaml data.json

# From file (auto-detects format)
dotnet run --project YamlConverter.csproj config.yaml

# With explicit options
dotnet run --project YamlConverter.csproj -y2j -i input.yaml
dotnet run --project YamlConverter.csproj -j2y -i input.json
```

## Example

**YAML to JSON:**

Input:
```yaml
database:
  host: localhost
  port: 5432
  credentials:
    username: admin
    password: secret
```

Output:
```json
{
  "database": {
    "host": "localhost",
    "port": 5432,
    "credentials": {
      "username": "admin",
      "password": "secret"
    }
  }
}
```

**JSON to YAML:**

Input:
```json
{
  "server": {
    "name": "production",
    "ports": [80, 443, 8080]
  }
}
```

Output:
```yaml
server:
  name: production
  ports:
  - 80
  - 443
  - 8080
```

## Concepts Demonstrated

- YAML parsing and serialization (YamlDotNet library)
- JSON serialization (System.Text.Json)
- Command-line argument parsing
- File I/O and stdin reading
- Object deserialization to dynamic types
- Format detection and conversion
- Error handling for invalid formats
