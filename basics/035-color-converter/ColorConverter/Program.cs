using System;
using System.Globalization;

namespace ColorConverter;

/// <summary>
/// A practical CLI tool for converting color codes between HEX, RGB, and HSL formats.
/// Useful for web developers and designers working with different color representations.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        if (args.Length < 1)
        {
            PrintUsage();
            return 1;
        }

        string input = args[0];

        try
        {
            // Auto-detect format and convert to all others
            var color = ParseColor(input);

            Console.WriteLine($"HEX: {ToHex(color)}");
            Console.WriteLine($"RGB: {ToRgb(color)}");
            Console.WriteLine($"HSL: {ToHsl(color)}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            PrintUsage();
            return 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Color Code Converter - Convert between HEX, RGB, and HSL");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project ColorConverter.csproj <color>");
        Console.WriteLine();
        Console.WriteLine("Supported formats:");
        Console.WriteLine("  HEX: #RRGGBB or #RGB (e.g., #ff5733, #f53)");
        Console.WriteLine("  RGB: rgb(R,G,B) or R,G,B (e.g., rgb(255,87,51), 255,87,51)");
        Console.WriteLine("  HSL: hsl(H,S%,L%) (e.g., hsl(11,100%,60%))");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project ColorConverter.csproj \"#ff5733\"");
        Console.WriteLine("  dotnet run --project ColorConverter.csproj \"rgb(255,87,51)\"");
        Console.WriteLine("  dotnet run --project ColorConverter.csproj \"hsl(11,100%,60%)\"");
        Console.WriteLine("  dotnet run --project ColorConverter.csproj \"#f53\"");
    }

    record Color(byte R, byte G, byte B);

    static Color ParseColor(string input)
    {
        input = input.Trim().ToLower();

        if (input.StartsWith('#'))
            return ParseHex(input);
        if (input.StartsWith("rgb"))
            return ParseRgb(input);
        if (input.StartsWith("hsl"))
            return ParseHsl(input);

        // Try comma-separated RGB
        if (input.Contains(','))
            return ParseRgbSimple(input);

        throw new ArgumentException($"Unable to parse color: {input}");
    }

    static Color ParseHex(string hex)
    {
        hex = hex.TrimStart('#');

        if (hex.Length == 3)
        {
            // Expand shorthand #RGB to #RRGGBB
            return new Color(
                (byte)(Convert.ToInt32(hex[0].ToString(), 16) * 17),
                (byte)(Convert.ToInt32(hex[1].ToString(), 16) * 17),
                (byte)(Convert.ToInt32(hex[2].ToString(), 16) * 17)
            );
        }

        if (hex.Length != 6)
            throw new ArgumentException($"Invalid HEX format: #{hex}");

        return new Color(
            Convert.ToByte(hex.Substring(0, 2), 16),
            Convert.ToByte(hex.Substring(2, 2), 16),
            Convert.ToByte(hex.Substring(4, 2), 16)
        );
    }

    static Color ParseRgb(string input)
    {
        // Remove rgb( and )
        string content = input.Replace("rgb(", "").Replace(")", "");
        return ParseRgbSimple(content);
    }

    static Color ParseRgbSimple(string content)
    {
        string[] parts = content.Split(',', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length != 3)
            throw new ArgumentException($"Invalid RGB format: {content}");

        return new Color(
            byte.Parse(parts[0].Trim()),
            byte.Parse(parts[1].Trim()),
            byte.Parse(parts[2].Trim())
        );
    }

    static Color ParseHsl(string input)
    {
        // Remove hsl( and )
        string content = input.Replace("hsl(", "").Replace(")", "");
        string[] parts = content.Split(',', StringSplitOptions.RemoveEmptyEntries);

        if (parts.Length != 3)
            throw new ArgumentException($"Invalid HSL format: {input}");

        float h = float.Parse(parts[0].Trim(), CultureInfo.InvariantCulture);
        float s = float.Parse(parts[1].Trim().Replace("%", ""), CultureInfo.InvariantCulture) / 100f;
        float l = float.Parse(parts[2].Trim().Replace("%", ""), CultureInfo.InvariantCulture) / 100f;

        return HslToRgb(h, s, l);
    }

    static string ToHex(Color color)
    {
        return $"#{color.R:X2}{color.G:X2}{color.B:X2}";
    }

    static string ToRgb(Color color)
    {
        return $"rgb({color.R},{color.G},{color.B})";
    }

    static string ToHsl(Color color)
    {
        var (h, s, l) = RgbToHsl(color.R, color.G, color.B);
        return $"hsl({h:F0},{s * 100:F0}%,{l * 100:F0}%)";
    }

    static Color HslToRgb(float h, float s, float l)
    {
        float r, g, b;

        if (s == 0)
        {
            r = g = b = l;
        }
        else
        {
            float q = l < 0.5f ? l * (1 + s) : l + s - l * s;
            float p = 2 * l - q;

            r = HueToRgb(p, q, h + 1f / 3f);
            g = HueToRgb(p, q, h);
            b = HueToRgb(p, q, h - 1f / 3f);
        }

        return new Color(
            (byte)Math.Round(r * 255),
            (byte)Math.Round(g * 255),
            (byte)Math.Round(b * 255)
        );
    }

    static float HueToRgb(float p, float q, float t)
    {
        if (t < 0) t += 1;
        if (t > 1) t -= 1;
        if (t < 1f / 6f) return p + (q - p) * 6f * t;
        if (t < 1f / 2f) return q;
        if (t < 2f / 3f) return p + (q - p) * (2f / 3f - t) * 6f;
        return p;
    }

    static (float H, float S, float L) RgbToHsl(byte r, byte g, byte b)
    {
        float rr = r / 255f;
        float gg = g / 255f;
        float bb = b / 255f;

        float max = Math.Max(rr, Math.Max(gg, bb));
        float min = Math.Min(rr, Math.Min(gg, bb));
        float delta = max - min;

        float h = 0, s, l = (max + min) / 2f;

        if (delta != 0)
        {
            s = l > 0.5f ? delta / (2f - max - min) : delta / (max + min);

            h = max switch
            {
                _ when max == rr => ((gg - bb) / delta + (gg < bb ? 6 : 0)) / 6f,
                _ when max == gg => ((bb - rr) / delta + 2) / 6f,
                _ when max == bb => ((rr - gg) / delta + 4) / 6f,
                _ => 0
            };
        }
        else
        {
            s = 0;
        }

        return (h * 360f, s, l);
    }
}
