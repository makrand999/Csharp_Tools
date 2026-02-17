# Reflection Object Mapper

Reflection-based object mapper that maps properties between objects of different types. Demonstrates reflection, property mapping, type conversion, and attribute-based configuration.

## Usage

```bash
dotnet run --project ReflectionObjectMapper.csproj
```

## Example

```
=== Reflection Object Mapper ===

1. Basic Property Mapping:
Mapped: 1 - John Doe (john@example.com)

2. Mapping with Property Name Differences:
Mapped: 42 - Jane Smith (jane@api.com), Status: active

3. Type Conversion:
Id: 12345 (int), Score: 98.5 (decimal), Active: True (bool), BirthDate: 05/15/1990 00:00:00 (DateTime)

4. Nested Object Mapping:
Order #100 for Alice Johnson
Items: 2 products, Total: $1059.97

5. Collection Mapping:
Mapped 3 products:
  - Widget: $19.99
  - Gadget: $49.99
  - Gizmo: $99.99

6. Selective Mapping (Ignored Properties):
PublicData: Visible
InternalData: Also Visible
SensitiveData: (ignored)
ComputedData: (ignored)

7. Bidirectional Mapping:
Entity -> DTO: 99 - Test User
DTO -> Entity: 99 - Test User
```

## Concepts Demonstrated

- Reflection for property inspection (`GetProperties`, `GetValue`, `SetValue`)
- Custom attribute creation and usage (`[MapFrom]`, `[Ignore]`)
- Property name matching (case-insensitive)
- Type conversion (string to int/decimal/bool/DateTime)
- Nullable type handling
- Nested object mapping
- Collection mapping (List<T>)
- Property caching for performance
- Generic methods with type constraints
- Bidirectional mapping between types
