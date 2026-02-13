using System.Security.Cryptography;

namespace ChecksumVerifier;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Checksum Verifier");
            Console.WriteLine("Usage: dotnet run --project 148-checksum-verifier.csproj -- <file> [expected-hash]");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --md5         Use MD5 (default: SHA256)");
            Console.WriteLine("  --sha1        Use SHA1");
            Console.WriteLine("  --sha256      Use SHA256");
            Console.WriteLine("  --sha512      Use SHA512");
            Console.WriteLine("  --verify      Verify against expected hash");
            return;
        }

        string algorithm = "SHA256";
        string? expectedHash = null;
        bool verifyMode = false;
        string inputFile = "";

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--md5") algorithm = "MD5";
            else if (args[i] == "--sha1") algorithm = "SHA1";
            else if (args[i] == "--sha256") algorithm = "SHA256";
            else if (args[i] == "--sha512") algorithm = "SHA512";
            else if (args[i] == "--verify") verifyMode = true;
            else if (string.IsNullOrEmpty(inputFile)) inputFile = args[i];
            else if (verifyMode) expectedHash = args[i];
        }

        if (string.IsNullOrEmpty(inputFile))
        {
            Console.WriteLine("Error: No input file specified");
            return;
        }

        if (!File.Exists(inputFile))
        {
            Console.WriteLine($"Error: File not found: {inputFile}");
            return;
        }

        try
        {
            var hash = ComputeHash(inputFile, algorithm);
            var hashString = BitConverter.ToString(hash).Replace("-", "").ToLower();

            Console.WriteLine($"File: {inputFile}");
            Console.WriteLine($"Algorithm: {algorithm}");
            Console.WriteLine($"Hash: {hashString}");

            if (verifyMode && !string.IsNullOrEmpty(expectedHash))
            {
                var expected = expectedHash.ToLower().Replace(" ", "");
                if (hashString == expected)
                {
                    Console.WriteLine($"\n✓ Checksum VERIFIED - File is intact!");
                }
                else
                {
                    Console.WriteLine($"\n✗ Checksum MISMATCH!");
                    Console.WriteLine($"Expected: {expected}");
                    Console.WriteLine($"Got:      {hashString}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static byte[] ComputeHash(string file, string algorithm)
    {
        using var stream = File.OpenRead(file);
        HashAlgorithm hasher = algorithm switch
        {
            "MD5" => MD5.Create(),
            "SHA1" => SHA1.Create(),
            "SHA256" => SHA256.Create(),
            "SHA512" => SHA512.Create(),
            _ => SHA256.Create()
        };

        using (hasher)
        {
            return hasher.ComputeHash(stream);
        }
    }
}
