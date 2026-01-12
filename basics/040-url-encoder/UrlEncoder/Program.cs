using System;
using System.Text;
using System.Web;

namespace UrlEncoder;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        string command = args[0].ToLowerInvariant();

        switch (command)
        {
            case "encode":
            case "-e":
            case "--encode":
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Error: Missing text to encode");
                    return;
                }
                string encodeText = string.Join(" ", args[1..]);
                Console.WriteLine(EncodeUrl(encodeText));
                break;

            case "decode":
            case "-d":
            case "--decode":
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Error: Missing text to decode");
                    return;
                }
                string decodeText = string.Join(" ", args[1..]);
                Console.WriteLine(DecodeUrl(decodeText));
                break;

            case "encode-form":
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Error: Missing text to encode");
                    return;
                }
                string formText = string.Join(" ", args[1..]);
                Console.WriteLine(EncodeFormData(formText));
                break;

            case "decode-form":
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Error: Missing text to decode");
                    return;
                }
                string formDecodeText = string.Join(" ", args[1..]);
                Console.WriteLine(DecodeFormData(formDecodeText));
                break;

            default:
                // If no command, encode the input
                Console.WriteLine(EncodeUrl(string.Join(" ", args)));
                break;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("URL Encoder/Decoder - Encode and decode URL-safe strings");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project UrlEncoder.csproj [command] <text>");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  encode, -e, --encode     URL encode text (default)");
        Console.WriteLine("  decode, -d, --decode     URL decode text");
        Console.WriteLine("  encode-form              Encode form data (spaces as +)");
        Console.WriteLine("  decode-form              Decode form data");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project UrlEncoder.csproj encode \"Hello World!\"");
        Console.WriteLine("  dotnet run --project UrlEncoder.csproj decode \"Hello%20World%21\"");
        Console.WriteLine("  dotnet run --project UrlEncoder.csproj \"Hello World\"  # defaults to encode");
    }

    static string EncodeUrl(string text)
    {
        return HttpUtility.UrlEncode(text, Encoding.UTF8)
            .Replace("+", "%20"); // Use %20 instead of + for spaces
    }

    static string DecodeUrl(string text)
    {
        return HttpUtility.UrlDecode(text, Encoding.UTF8) ?? text;
    }

    static string EncodeFormData(string text)
    {
        return HttpUtility.UrlEncode(text, Encoding.UTF8) ?? text;
    }

    static string DecodeFormData(string text)
    {
        return HttpUtility.UrlDecode(text, Encoding.UTF8) ?? text;
    }
}
