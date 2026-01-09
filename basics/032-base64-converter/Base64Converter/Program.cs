using System;
using System.Text;

namespace Base64Converter;

/// <summary>
/// A practical CLI tool for encoding and decoding Base64 strings.
/// Useful for working with encoded data, tokens, and binary-to-text conversions.
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

        string command = args[0].ToLower();

        try
        {
            switch (command)
            {
                case "encode":
                case "enc":
                case "e":
                    if (args.Length < 2)
                    {
                        Console.Error.WriteLine("Error: Missing text to encode");
                        return 1;
                    }
                    string textToEncode = string.Join(" ", args[1..]);
                    string encoded = Encode(textToEncode);
                    Console.WriteLine(encoded);
                    break;

                case "decode":
                case "dec":
                case "d":
                    if (args.Length < 2)
                    {
                        Console.Error.WriteLine("Error: Missing Base64 string to decode");
                        return 1;
                    }
                    string textToDecode = string.Join(" ", args[1..]);
                    string decoded = Decode(textToDecode);
                    Console.WriteLine(decoded);
                    break;

                case "help":
                case "-h":
                case "--help":
                    PrintUsage();
                    break;

                default:
                    // Try to auto-detect: if it looks like Base64, decode it; otherwise encode
                    string input = string.Join(" ", args);
                    if (IsValidBase64(input))
                    {
                        Console.WriteLine(Decode(input));
                    }
                    else
                    {
                        Console.WriteLine(Encode(input));
                    }
                    break;
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Base64 Converter - Encode and decode Base64 strings");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project Base64Converter.csproj <command> [text]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  encode, enc, e  - Encode text to Base64");
        Console.WriteLine("  decode, dec, d  - Decode Base64 to text");
        Console.WriteLine("  help, -h        - Show this help");
        Console.WriteLine();
        Console.WriteLine("Auto-detect mode:");
        Console.WriteLine("  If no command is given, the tool auto-detects the input type.");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project Base64Converter.csproj encode \"Hello World\"");
        Console.WriteLine("  dotnet run --project Base64Converter.csproj decode SGVsbG8gV29ybGQ=");
        Console.WriteLine("  dotnet run --project Base64Converter.csproj SGVsbG8gV29ybGQ=");
        Console.WriteLine("  dotnet run --project Base64Converter.csproj \"Hello World\"");
    }

    static string Encode(string text)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(text);
        return Convert.ToBase64String(bytes);
    }

    static string Decode(string base64)
    {
        // Remove any whitespace and URL-safe character replacements
        string cleanBase64 = base64.Trim()
            .Replace('-', '+')
            .Replace('_', '/');

        // Add padding if necessary
        int padding = 4 - (cleanBase64.Length % 4);
        if (padding < 4)
        {
            cleanBase64 += new string('=', padding);
        }

        byte[] bytes = Convert.FromBase64String(cleanBase64);
        return Encoding.UTF8.GetString(bytes);
    }

    static bool IsValidBase64(string input)
    {
        // Quick heuristic: Base64 only contains A-Z, a-z, 0-9, +, /, and =
        string cleanInput = input.Trim().Replace('-', '+').Replace('_', '/');
        
        if (cleanInput.Length == 0)
            return false;

        // Check length is multiple of 4 (with padding)
        if (cleanInput.Length % 4 != 0)
            return false;

        // Check valid characters
        foreach (char c in cleanInput)
        {
            if (!char.IsLetterOrDigit(c) && c != '+' && c != '/' && c != '=')
                return false;
        }

        // Try to decode as a final check
        try
        {
            Convert.FromBase64String(cleanInput);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
