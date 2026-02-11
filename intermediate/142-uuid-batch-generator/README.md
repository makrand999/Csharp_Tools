# UUID Batch Generator

Generate multiple UUIDs/GUIDs in various formats.

## Usage

```bash
dotnet run --project 142-uuid-batch-generator.csproj -- [count] [--v4] [--json]
```

## Example

```bash
dotnet run --project 142-uuid-batch-generator.csproj -- 10
dotnet run --project 142-uuid-batch-generator.csproj -- 5 --v4 --json
```

### Sample Output

```
550e8400-e29b-41d4-a716-446655440000
6ba7b810-9dad-11d1-80b4-00c04fd430c8
...
```

## Options

- `count` - Number of UUIDs to generate (default: 1)
- `--v4` - Generate version 4 UUIDs (random)
- `--json` - Output as JSON array

## Concepts Demonstrated

- Guid class
- Random UUID generation
- Batch processing
- JSON output formatting
