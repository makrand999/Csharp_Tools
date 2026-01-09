using System;
using System.Security.Cryptography;
using System.Text;

namespace HashGenerator;

/// <summary>
/// A practical CLI tool for generating cryptographic hashes of text or files.
/// Supports MD5, SHA1, SHA256, SHA384, and SHA512 algorithms.
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

        string algorithm = args[0].ToLower();
        string input = args.Length > 1 ? string.Join(" ", args[1..]) : string.Empty;

        // Check if reading from stdin
        if (input == "-" || (args.Length == 1 && !Console.IsInputRedirected))
        {
            if (args.Length == 1)
            {
                // Interactive mode - read from stdin until EOF
                input = Console.ReadLine() ?? string.Empty;
            }
            else if (input == "-")
            {
                input = Console.ReadLine() ?? string.Empty;
            }
        }

        if (string.IsNullOrEmpty(input))
        {
            Console.Error.WriteLine("Error: No input provided");
            PrintUsage();
            return 1;
        }

        try
        {
            string hash = GenerateHash(input, algorithm);
            Console.WriteLine(hash);
            return 0;
        }
        catch (ArgumentException ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            PrintUsage();
            return 1;
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Hash Generator - Generate cryptographic hashes of text");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project HashGenerator.csproj <algorithm> <text>");
        Console.WriteLine();
        Console.WriteLine("Algorithms:");
        Console.WriteLine("  md5     - MD5 (128-bit, fast but not cryptographically secure)");
        Console.WriteLine("  sha1    - SHA-1 (160-bit, legacy)");
        Console.WriteLine("  sha256  - SHA-256 (256-bit, recommended)");
        Console.WriteLine("  sha384  - SHA-384 (384-bit)");
        Console.WriteLine("  sha512  - SHA-512 (512-bit)");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project HashGenerator.csproj sha256 \"Hello World\"");
        Console.WriteLine("  dotnet run --project HashGenerator.csproj md5 \"password123\"");
        Console.WriteLine("  echo \"Hello\" | dotnet run --project HashGenerator.csproj sha256 -");
    }

    static string GenerateHash(string input, string algorithm)
    {
        byte[] inputBytes = Encoding.UTF8.GetBytes(input);
        byte[] hashBytes = algorithm.ToLower() switch
        {
            "md5" => MD5.HashData(inputBytes),
            "sha1" => SHA1.HashData(inputBytes),
            "sha256" => SHA256.HashData(inputBytes),
            "sha384" => SHA384.HashData(inputBytes),
            "sha512" => SHA512.HashData(inputBytes),
            _ => throw new ArgumentException($"Unsupported algorithm: {algorithm}")
        };

        // Convert to hex string
        return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
    }
}
