# Random Data Generator

Generate various types of random data for testing.

## Usage

```bash
dotnet run --project 147-random-data-generator.csproj -- <type> [options]
```

## Example

```bash
dotnet run --project 147-random-data-generator.csproj -- uuid --count 10
dotnet run --project 147-random-data-generator.csproj -- password --length 20 --count 5
dotnet run --project 147-random-data-generator.csproj -- string --length 32
dotnet run --project 147-random-data-generator.csproj -- number --min 1 --max 100 --count 10
dotnet run --project 147-random-data-generator.csproj -- bytes --length 16
dotnet run --project 147-random-data-generator.csproj -- date --min 2020 --max 2024
```

## Types

- `uuid` - Generate UUIDs
- `password` - Generate secure passwords
- `string` - Generate random strings
- `number` - Generate random numbers
- `bytes` - Generate random bytes (hex)
- `date` - Generate random dates

## Options

- `--count N` - Number of items (default: 1)
- `--length N` - Length of string/password
- `--min N` - Minimum value for numbers/dates
- `--max N` - Maximum value for numbers/dates

## Concepts Demonstrated

- RandomNumberGenerator
- Secure random generation
- Password generation
- Data type generation
