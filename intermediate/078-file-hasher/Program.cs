using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FileHasher;

/// <summary>
/// Generates and verifies file checksums using multiple hash algorithms.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return 1;
        }

        var algorithm = "SHA256";
        var verify = false;
        var outputJson = false;
        string? hashFile = null;

        var filePath = "";
        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--algo" when i + 1 < args.Length:
                    algorithm = args[++i].ToUpper();
                    break;
                case "--verify" when i + 1 < args.Length:
                    verify = true;
                    hashFile = args[++i];
                    break;
                case "--json":
                    outputJson = true;
                    break;
                case "--md5":
                    algorithm = "MD5";
                    break;
                case "--sha1":
                    algorithm = "SHA1";
                    break;
                case "--sha256":
                    algorithm = "SHA256";
                    break;
                case "--sha512":
                    algorithm = "SHA512";
                    break;
                case "-h" or "--help":
                    PrintUsage();
                    return 0;
                default:
                    if (!args[i].StartsWith("-"))
                        filePath = args[i];
                    break;
            }
        }

        if (string.IsNullOrEmpty(filePath))
        {
            Console.WriteLine("Error: No file specified.");
            PrintUsage();
            return 1;
        }

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Error: File '{filePath}' does not exist.");
            return 1;
        }

        try
        {
            if (verify && !string.IsNullOrEmpty(hashFile))
            {
                return VerifyHash(filePath, hashFile, algorithm) ? 0 : 1;
            }
            else
            {
                return GenerateHash(filePath, algorithm, outputJson) ? 0 : 1;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("""
            FileHasher - Generate and verify file checksums

            Usage:
              dotnet run --project FileHasher.csproj <file> [options]
              dotnet run --project FileHasher.csproj <file> --verify <hashfile>

            Options:
              --md5         Use MD5 algorithm
              --sha1        Use SHA1 algorithm
              --sha256      Use SHA256 algorithm (default)
              --sha512      Use SHA512 algorithm
              --algo <name> Specify algorithm (MD5, SHA1, SHA256, SHA512)
              --verify <f>  Verify file against stored hash
              --json        Output as JSON
              -h, --help    Show this help

            Examples:
              dotnet run --project FileHasher.csproj myfile.iso
              dotnet run --project FileHasher.csproj myfile.iso --sha256
              dotnet run --project FileHasher.csproj myfile.iso --verify checksums.txt
              dotnet run --project FileHasher.csproj myfile.iso --json
            """);
    }

    static bool GenerateHash(string filePath, string algorithm, bool outputJson)
    {
        var hash = ComputeHash(filePath, algorithm);
        var fileName = Path.GetFileName(filePath);
        var fileSize = new FileInfo(filePath).Length;

        if (outputJson)
        {
            Console.WriteLine("{");
            Console.WriteLine($"  \"file\": \"{EscapeJson(filePath)}\",");
            Console.WriteLine($"  \"fileName\": \"{EscapeJson(fileName)}\",");
            Console.WriteLine($"  \"size\": {fileSize},");
            Console.WriteLine($"  \"algorithm\": \"{algorithm}\",");
            Console.WriteLine($"  \"hash\": \"{hash}\"");
            Console.WriteLine("}");
        }
        else
        {
            Console.WriteLine($"{hash.ToLower()}  {fileName}");
        }

        return true;
    }

    static bool VerifyHash(string filePath, string hashFile, string algorithm)
    {
        if (!File.Exists(hashFile))
        {
            Console.WriteLine($"Error: Hash file '{hashFile}' does not exist.");
            return false;
        }

        var computedHash = ComputeHash(filePath, algorithm);
        var fileName = Path.GetFileName(filePath);
        
        // Read hash file and look for our file
        var storedHash = "";
        foreach (var line in File.ReadAllLines(hashFile))
        {
            var parts = line.Split(new[] { ' ', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length >= 2)
            {
                var hashInFile = parts[0];
                var fileInHash = parts.Length > 1 ? parts[1] : "";
                
                if (fileInHash == fileName || fileInHash.EndsWith("/" + fileName) || fileInHash.EndsWith("\\" + fileName))
                {
                    storedHash = hashInFile;
                    break;
                }
            }
        }

        if (string.IsNullOrEmpty(storedHash))
        {
            Console.WriteLine($"Warning: No hash found for '{fileName}' in {hashFile}");
            Console.WriteLine($"Computed hash ({algorithm}): {computedHash.ToLower()}");
            return false;
        }

        var isValid = computedHash.Equals(storedHash, StringComparison.OrdinalIgnoreCase);
        
        Console.WriteLine($"File: {fileName}");
        Console.WriteLine($"Stored hash:  {storedHash.ToLower()}");
        Console.WriteLine($"Computed hash: {computedHash.ToLower()}");
        Console.WriteLine($"Algorithm: {algorithm}");
        Console.WriteLine();
        
        if (isValid)
        {
            Console.WriteLine("✓ Hash verification PASSED");
        }
        else
        {
            Console.WriteLine("✗ Hash verification FAILED - file may be corrupted!");
        }

        return isValid;
    }

    static string ComputeHash(string filePath, string algorithm)
    {
        using var stream = File.OpenRead(filePath);
        HashAlgorithm? hasher = algorithm switch
        {
            "MD5" => MD5.Create(),
            "SHA1" => SHA1.Create(),
            "SHA256" => SHA256.Create(),
            "SHA512" => SHA512.Create(),
            _ => throw new ArgumentException($"Unknown algorithm: {algorithm}")
        };

        if (hasher == null)
            throw new ArgumentException($"Unknown algorithm: {algorithm}");

        var hashBytes = hasher.ComputeHash(stream);
        return BitConverter.ToString(hashBytes).Replace("-", "");
    }

    static string EscapeJson(string s) => s.Replace("\\", "\\\\")
                                            .Replace("\"", "\\\"")
                                            .Replace("\n", "\\n")
                                            .Replace("\r", "\\r")
                                            .Replace("\t", "\\t");
}
