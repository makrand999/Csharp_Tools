using System.Security.Cryptography;
using System.Text;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "hash":
        HashCommand(args.Skip(1).ToArray());
        break;
    case "verify":
        VerifyHash(args.Skip(1).ToArray());
        break;
    case "compare":
        CompareFiles(args.Skip(1).ToArray());
        break;
    default:
        HashCommand(args);
        break;
}

void HashCommand(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- hash <file> [algorithm]");
        Console.WriteLine("   or: dotnet run -- hash <text> --text");
        Console.WriteLine("Algorithms: MD5, SHA1, SHA256, SHA384, SHA512 (default: SHA256)");
        return;
    }

    var algorithm = "SHA256";
    var isText = false;
    var target = args[0];

    for (var i = 1; i < args.Length; i++)
    {
        if (args[i] == "--text")
            isText = true;
        else if (IsAlgorithm(args[i]))
            algorithm = args[i].ToUpper();
    }

    byte[] data;

    try
    {
        if (isText)
        {
            data = Encoding.UTF8.GetBytes(target);
        }
        else
        {
            if (!File.Exists(target))
            {
                Console.WriteLine($"File not found: {target}");
                return;
            }
            data = File.ReadAllBytes(target);
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error reading data: {ex.Message}");
        return;
    }

    var hash = CalculateHash(data, algorithm);
    var label = isText ? "Text" : Path.GetFileName(target);

    Console.WriteLine($"{algorithm} Hash:");
    Console.WriteLine($"  {label}");
    Console.WriteLine($"  {hash}");

    if (!isText)
        Console.WriteLine($"\nFor verification: dotnet run -- verify {target} {hash}");
}

void VerifyHash(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- verify <file> <expected-hash>");
        return;
    }

    var filePath = args[0];
    var expectedHash = args[1].Replace("-", "").Replace(":", "").ToLower();

    if (!File.Exists(filePath))
    {
        Console.WriteLine($"File not found: {filePath}");
        return;
    }

    var data = File.ReadAllBytes(filePath);
    var algorithm = DetectAlgorithm(expectedHash);

    if (algorithm == null)
    {
        Console.WriteLine("Unable to detect hash algorithm from hash length");
        Console.WriteLine("Expected lengths: MD5=32, SHA1=40, SHA256=64, SHA384=96, SHA512=128");
        return;
    }

    var actualHash = CalculateHash(data, algorithm);

    Console.WriteLine($"Verifying {Path.GetFileName(filePath)}...");
    Console.WriteLine($"Algorithm: {algorithm}");
    Console.WriteLine($"Expected: {expectedHash}");
    Console.WriteLine($"Actual:   {actualHash}");

    if (actualHash == expectedHash)
    {
        Console.WriteLine("\n✓ Hash verified - File is intact!");
    }
    else
    {
        Console.WriteLine("\n✗ Hash mismatch - File may be corrupted or tampered!");
    }
}

void CompareFiles(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- compare <file1> <file2>");
        return;
    }

    var file1 = args[0];
    var file2 = args[1];

    if (!File.Exists(file1))
    {
        Console.WriteLine($"File not found: {file1}");
        return;
    }

    if (!File.Exists(file2))
    {
        Console.WriteLine($"File not found: {file2}");
        return;
    }

    var data1 = File.ReadAllBytes(file1);
    var data2 = File.ReadAllBytes(file2);

    var hash1 = CalculateHash(data1, "SHA256");
    var hash2 = CalculateHash(data2, "SHA256");

    Console.WriteLine("File Comparison:");
    Console.WriteLine($"  {Path.GetFileName(file1)} ({data1.Length:N0} bytes)");
    Console.WriteLine($"  SHA256: {hash1}");
    Console.WriteLine();
    Console.WriteLine($"  {Path.GetFileName(file2)} ({data2.Length:N0} bytes)");
    Console.WriteLine($"  SHA256: {hash2}");
    Console.WriteLine();

    if (hash1 == hash2)
    {
        Console.WriteLine("✓ Files are identical");
    }
    else
    {
        Console.WriteLine("✗ Files are different");
        if (data1.Length == data2.Length)
            Console.WriteLine("  (Same size, different content)");
        else
            Console.WriteLine($"  (Size difference: {Math.Abs(data1.Length - data2.Length):N0} bytes)");
    }
}

void ShowHelp()
{
    Console.WriteLine("""
        FileChecksumVerifier - Verify file integrity with checksums

        Usage:
          dotnet run -- hash <file> [algorithm]
          dotnet run -- hash <text> --text
          dotnet run -- verify <file> <expected-hash>
          dotnet run -- compare <file1> <file2>

        Algorithms:
          MD5    - 128-bit (32 hex chars) - Fast, not cryptographically secure
          SHA1   - 160-bit (40 hex chars) - Legacy
          SHA256 - 256-bit (64 hex chars) - Recommended (default)
          SHA384 - 384-bit (96 hex chars)
          SHA512 - 512-bit (128 hex chars)

        Examples:
          dotnet run -- hash myfile.zip
          dotnet run -- hash myfile.zip SHA512
          dotnet run -- hash "Hello World" --text
          dotnet run -- verify myfile.zip a591a6d40bf420404a011733cfb7b190...
          dotnet run -- compare file1.txt file2.txt
        """);
}

string CalculateHash(byte[] data, string algorithm)
{
    using var hasher = GetHashAlgorithm(algorithm);
    var hash = hasher.ComputeHash(data);
    return BitConverter.ToString(hash).Replace("-", "").ToLower();
}

HashAlgorithm GetHashAlgorithm(string algorithm) => algorithm switch
{
    "MD5" => MD5.Create(),
    "SHA1" => SHA1.Create(),
    "SHA256" => SHA256.Create(),
    "SHA384" => SHA384.Create(),
    "SHA512" => SHA512.Create(),
    _ => throw new ArgumentException($"Unknown algorithm: {algorithm}")
};

string? DetectAlgorithm(string hash) => hash.Length switch
{
    32 => "MD5",
    40 => "SHA1",
    64 => "SHA256",
    96 => "SHA384",
    128 => "SHA512",
    _ => null
};

bool IsAlgorithm(string s) => s.ToUpper() switch
{
    "MD5" or "SHA1" or "SHA256" or "SHA384" or "SHA512" => true,
    _ => false
};
