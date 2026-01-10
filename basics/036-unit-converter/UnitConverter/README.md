# Unit Converter

A CLI tool for converting between different units of measurement including length, weight, and temperature.

## Usage

```bash
dotnet run --project UnitConverter.csproj <value> <from_unit> <to_unit>
```

## Supported Units

**Length:** m, km, cm, mm, mi, yd, ft, in

**Weight:** kg, g, mg, lb, oz

**Temperature:** c, f, k

## Examples

```bash
# Convert 100 meters to kilometers
dotnet run --project UnitConverter.csproj 100 m km
# Output: 100 m = 0.1 km

# Convert 32 Celsius to Fahrenheit
dotnet run --project UnitConverter.csproj 32 c f
# Output: 32 c = 89.6 f

# Convert 5 kilograms to pounds
dotnet run --project UnitConverter.csproj 5 kg lb
# Output: 5 kg = 11.023113109243878 lb
```

## Concepts Demonstrated

- Pattern matching with switch expressions
- Dictionary-like unit conversion using intermediate base units
- Command-line argument parsing
- Culture-invariant number parsing
- Error handling with ArgumentException
