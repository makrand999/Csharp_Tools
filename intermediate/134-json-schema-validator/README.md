# JSON Schema Validator

Validate JSON data against a JSON Schema (basic implementation).

## Usage

```bash
dotnet run --project 134-json-schema-validator.csproj -- <data.json> <schema.json>
```

## Example

```bash
dotnet run --project 134-json-schema-validator.csproj -- user.json user-schema.json
```

## Supported Schema Keywords

- `type` - Type validation (string, number, boolean, array, object)
- `required` - Required properties
- `properties` - Property definitions
- `minimum` / `maximum` - Numeric range
- `minLength` / `maxLength` - String length
- `pattern` - Regex pattern matching
- `minItems` / `maxItems` - Array size

## Concepts Demonstrated

- JSON parsing with System.Text.Json
- Schema validation
- Recursive validation
- Error collection
- Type checking
