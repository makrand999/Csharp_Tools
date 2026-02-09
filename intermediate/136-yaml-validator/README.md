# YAML Validator

Validate YAML syntax and detect common issues.

## Usage

```bash
dotnet run --project 136-yaml-validator.csproj -- <file.yaml>
cat config.yaml | dotnet run --project 136-yaml-validator.csproj
```

## Example

```bash
dotnet run --project 136-yaml-validator.csproj -- docker-compose.yaml
```

### Sample Output

```
Validating: docker-compose.yaml
✓ YAML syntax appears valid!

=== Statistics ===
Total lines: 45
Key-value pairs: 23
List items: 8
Comments: 5
Nested depth: 4
```

## Checks Performed

- Tab character detection (YAML prefers spaces)
- List item formatting
- Indentation consistency
- Structure validation

## Concepts Demonstrated

- YAML structure parsing
- Indentation tracking
- Error detection
- Statistics calculation
- Line-by-line analysis
