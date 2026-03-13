using System.Security.Cryptography;
using System.Text;

namespace EncryptionTool;

/// <summary>
/// Encryption Tool - Encrypts and decrypts files using AES-256 symmetric encryption.
/// Demonstrates cryptographic stream operations with PBKDF2 key derivation.
/// </summary>
public class EncryptionService
{
    private const int SaltSize = 16;
    private const int KeySize = 32; // 256 bits
    private const int IvSize = 16;
    private const int Iterations = 100000;

    public async Task<EncryptionResult> EncryptFileAsync(
        string inputPath, string outputPath, string password, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var inputFile = new FileInfo(inputPath);

        using var aes = Aes.Create();
        var salt = RandomNumberGenerator.GetBytes(SaltSize);

        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        aes.Key = deriveBytes.GetBytes(KeySize);
        aes.IV = RandomNumberGenerator.GetBytes(IvSize);

        await using var input = File.OpenRead(inputPath);
        await using var output = File.Create(outputPath);

        // Write salt and IV to output file
        await output.WriteAsync(salt.AsMemory(0, SaltSize), cancellationToken);
        await output.WriteAsync(aes.IV.AsMemory(0, IvSize), cancellationToken);

        await using var cryptoStream = new CryptoStream(output, aes.CreateEncryptor(), CryptoStreamMode.Write);
        await input.CopyToAsync(cryptoStream, 81920, cancellationToken);

        await cryptoStream.FlushFinalBlockAsync(cancellationToken);

        stopwatch.Stop();
        var encryptedSize = new FileInfo(outputPath).Length;

        return new EncryptionResult
        {
            OriginalSize = inputFile.Length,
            EncryptedSize = encryptedSize,
            ElapsedTime = stopwatch.Elapsed,
            IsEncrypted = true
        };
    }

    public async Task<EncryptionResult> DecryptFileAsync(
        string inputPath, string outputPath, string password, CancellationToken cancellationToken = default)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        await using var input = File.OpenRead(inputPath);
        
        // Read salt and IV
        var salt = new byte[SaltSize];
        var iv = new byte[IvSize];
        
        await input.ReadExactlyAsync(salt.AsMemory(0, SaltSize), cancellationToken);
        await input.ReadExactlyAsync(iv.AsMemory(0, IvSize), cancellationToken);

        using var aes = Aes.Create();
        using var deriveBytes = new Rfc2898DeriveBytes(password, salt, Iterations, HashAlgorithmName.SHA256);
        aes.Key = deriveBytes.GetBytes(KeySize);
        aes.IV = iv;

        await using var output = File.Create(outputPath);
        await using var cryptoStream = new CryptoStream(input, aes.CreateDecryptor(), CryptoStreamMode.Read);
        await cryptoStream.CopyToAsync(output, 81920, cancellationToken);

        stopwatch.Stop();
        var decryptedSize = new FileInfo(outputPath).Length;

        return new EncryptionResult
        {
            OriginalSize = new FileInfo(inputPath).Length,
            EncryptedSize = decryptedSize,
            ElapsedTime = stopwatch.Elapsed,
            IsEncrypted = false
        };
    }

    public string ComputeHash(string filePath, HashAlgorithmName algorithm)
    {
        using var hashAlgorithm = HashAlgorithm.Create(algorithm.Name!)!;
        using var stream = File.OpenRead(filePath);
        var hash = hashAlgorithm.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}

public record EncryptionResult
{
    public long OriginalSize { get; init; }
    public long EncryptedSize { get; init; }
    public TimeSpan ElapsedTime { get; init; }
    public bool IsEncrypted { get; init; }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Encryption Tool ===\n");
        Console.WriteLine("Using AES-256 encryption with PBKDF2 key derivation\n");

        var service = new EncryptionService();
        var password = "MySecurePassword123!";

        // Create test file
        var testFile = Path.Combine(Path.GetTempPath(), $"secret_data_{Guid.NewGuid()}.txt");
        var encryptedFile = testFile + ".enc";
        var decryptedFile = testFile + "_decrypted.txt";

        Console.WriteLine("Creating test file...");
        var secretContent = "This is a secret message that needs encryption!\n" + 
                           new string('X', 10000); // Add some data
        await File.WriteAllTextAsync(testFile, secretContent);

        var originalSize = new FileInfo(testFile).Length;
        Console.WriteLine($"Original size: {originalSize} bytes\n");

        // Compute hash
        var hash = service.ComputeHash(testFile, HashAlgorithmName.SHA256);
        Console.WriteLine($"Original SHA-256: {hash}\n");

        // Encrypt
        Console.WriteLine("Encrypting with AES-256...");
        var encryptResult = await service.EncryptFileAsync(testFile, encryptedFile, password);

        Console.WriteLine($"Encrypted size: {encryptResult.EncryptedSize} bytes");
        Console.WriteLine($"Time: {encryptResult.ElapsedTime.TotalMilliseconds:F0}ms\n");

        // Decrypt
        Console.WriteLine("Decrypting...");
        var decryptResult = await service.DecryptFileAsync(encryptedFile, decryptedFile, password);

        Console.WriteLine($"Decrypted size: {decryptResult.EncryptedSize} bytes");
        Console.WriteLine($"Time: {decryptResult.ElapsedTime.TotalMilliseconds:F0}ms\n");

        // Verify
        var originalContent = await File.ReadAllTextAsync(testFile);
        var decryptedContent = await File.ReadAllTextAsync(decryptedFile);
        var verified = originalContent == decryptedContent;

        Console.WriteLine($"Verification: {(verified ? "✓ PASSED" : "✗ FAILED")}");

        // Show encrypted content (first 50 bytes as hex)
        var encryptedBytes = await File.ReadAllBytesAsync(encryptedFile);
        Console.WriteLine($"\nEncrypted data (first 50 bytes):");
        Console.WriteLine(BitConverter.ToString(encryptedBytes.Take(50).ToArray()));

        // Cleanup
        File.Delete(testFile);
        File.Delete(encryptedFile);
        File.Delete(decryptedFile);

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
