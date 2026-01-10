using System;
using System.Collections.Generic;
using System.Globalization;

namespace UnitConverter;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 3)
        {
            PrintUsage();
            return;
        }

        if (!double.TryParse(args[0], NumberStyles.Any, CultureInfo.InvariantCulture, out double value))
        {
            Console.Error.WriteLine("Error: Invalid number format.");
            return;
        }

        string fromUnit = args[1].ToLowerInvariant();
        string toUnit = args[2].ToLowerInvariant();

        try
        {
            double result = ConvertUnits(value, fromUnit, toUnit);
            Console.WriteLine($"{value} {fromUnit} = {result} {toUnit}");
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Unit Converter - Convert between length, weight, and temperature units");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project UnitConverter.csproj <value> <from_unit> <to_unit>");
        Console.WriteLine();
        Console.WriteLine("Length units: m, km, cm, mm, mi, yd, ft, in");
        Console.WriteLine("Weight units: kg, g, mg, lb, oz");
        Console.WriteLine("Temperature units: c, f, k");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project UnitConverter.csproj 100 m km");
        Console.WriteLine("  dotnet run --project UnitConverter.csproj 32 c f");
        Console.WriteLine("  dotnet run --project UnitConverter.csproj 5 kg lb");
    }

    static double ConvertUnits(double value, string fromUnit, string toUnit)
    {
        if (fromUnit == toUnit)
            return value;

        // Length conversions
        var lengthUnits = new[] { "m", "km", "cm", "mm", "mi", "yd", "ft", "in" };
        if (Array.IndexOf(lengthUnits, fromUnit) >= 0 && Array.IndexOf(lengthUnits, toUnit) >= 0)
        {
            return ConvertLength(value, fromUnit, toUnit);
        }

        // Weight conversions
        var weightUnits = new[] { "kg", "g", "mg", "lb", "oz" };
        if (Array.IndexOf(weightUnits, fromUnit) >= 0 && Array.IndexOf(weightUnits, toUnit) >= 0)
        {
            return ConvertWeight(value, fromUnit, toUnit);
        }

        // Temperature conversions
        var tempUnits = new[] { "c", "f", "k" };
        if (Array.IndexOf(tempUnits, fromUnit) >= 0 && Array.IndexOf(tempUnits, toUnit) >= 0)
        {
            return ConvertTemperature(value, fromUnit, toUnit);
        }

        throw new ArgumentException($"Cannot convert between {fromUnit} and {toUnit}. Check units are valid and in the same category.");
    }

    static double ConvertLength(double value, string fromUnit, string toUnit)
    {
        // Convert to meters first
        double meters = fromUnit switch
        {
            "m" => value,
            "km" => value * 1000,
            "cm" => value / 100,
            "mm" => value / 1000,
            "mi" => value * 1609.344,
            "yd" => value * 0.9144,
            "ft" => value * 0.3048,
            "in" => value * 0.0254,
            _ => throw new ArgumentException($"Unknown length unit: {fromUnit}")
        };

        // Convert from meters to target unit
        return toUnit switch
        {
            "m" => meters,
            "km" => meters / 1000,
            "cm" => meters * 100,
            "mm" => meters * 1000,
            "mi" => meters / 1609.344,
            "yd" => meters / 0.9144,
            "ft" => meters / 0.3048,
            "in" => meters / 0.0254,
            _ => throw new ArgumentException($"Unknown length unit: {toUnit}")
        };
    }

    static double ConvertWeight(double value, string fromUnit, string toUnit)
    {
        // Convert to grams first
        double grams = fromUnit switch
        {
            "kg" => value * 1000,
            "g" => value,
            "mg" => value / 1000,
            "lb" => value * 453.59237,
            "oz" => value * 28.34952,
            _ => throw new ArgumentException($"Unknown weight unit: {fromUnit}")
        };

        // Convert from grams to target unit
        return toUnit switch
        {
            "kg" => grams / 1000,
            "g" => grams,
            "mg" => grams * 1000,
            "lb" => grams / 453.59237,
            "oz" => grams / 28.34952,
            _ => throw new ArgumentException($"Unknown weight unit: {toUnit}")
        };
    }

    static double ConvertTemperature(double value, string fromUnit, string toUnit)
    {
        // Convert to Celsius first
        double celsius = fromUnit switch
        {
            "c" => value,
            "f" => (value - 32) * 5 / 9,
            "k" => value - 273.15,
            _ => throw new ArgumentException($"Unknown temperature unit: {fromUnit}")
        };

        // Convert from Celsius to target unit
        return toUnit switch
        {
            "c" => celsius,
            "f" => celsius * 9 / 5 + 32,
            "k" => celsius + 273.15,
            _ => throw new ArgumentException($"Unknown temperature unit: {toUnit}")
        };
    }
}
