# XML/JSON Schema Validator

XML and JSON Schema validator demonstrating schema validation, XSD, and JSON Schema. Validates XML against XSD and JSON against JSON Schema.

## Usage

```bash
dotnet run --project XmlJsonSchemaValidator.csproj
```

## Example

```
=== XML/JSON Schema Validator ===

=== XML Schema (XSD) Validation ===

1. Validating Valid XML:
   ✓ XML is valid against schema

2. Validating Invalid XML:
   [Error] The 'category' attribute is invalid - 'InvalidCategory' is not a valid value
   [Error] Invalid data type for 'Year' - 'not_a_number' is not a valid int
   [Error] The 'category' attribute is required but missing
   ✗ Found 3 validation error(s)

=== JSON Schema Validation ===

3. Validating Valid JSON:
   ✓ JSON is valid against schema

4. Validating Invalid JSON:
   ✗ Found 7 validation error(s):
      - $.id: Value -5 is less than minimum 1
      - $.username: String length 2 is less than minimum 3
      - $.email: Invalid email format
      - $.age: Value 200 exceeds maximum 150
      - $.role: Value 'superuser' is not one of the allowed enum values
      - $.tags: Array has 0 items, minimum is 1
      - $: Additional property 'extraField' is not allowed

=== Schema Analysis ===

Schema Details:
   Title: User
   Root Type: object
   Required Fields: id, username, email
   Properties:
      - id: integer
      - username: string
      - email: string
      - age: integer
      - role: string
      - tags: array
      - profile: object
```

## Concepts Demonstrated

- XML Schema (XSD) validation with `XmlReaderSettings`
- XSD schema definition with complex types
- XSD enumerations and restrictions
- JSON Schema validation (draft-07)
- Type validation (string, number, boolean, array, object)
- String constraints (minLength, maxLength, pattern)
- Number constraints (minimum, maximum)
- Array constraints (minItems, maxItems, uniqueItems)
- Object constraints (required properties, additionalProperties)
- Format validation (email, uri)
- Enum validation
- Nested object validation
- Schema analysis and introspection
- Error collection and reporting
