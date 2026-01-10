using System;
using System.Security.Cryptography;
using System.Text;

namespace UuidGenerator;

/// <summary>
/// A practical CLI tool for generating UUIDs/GUIDs in various formats and versions.
/// Supports UUID v4 (random), v7 (timestamp-based), and custom namespace-based v5.
/// </summary>
class Program
{
    static int Main(string[] args)
    {
        string version = "v4";
        int count = 1;
        string format = "canonical";
        string? namespaceId = null;
        string? name = null;

        // Parse arguments
        for (int i = 0; i < args.Length; i++)
        {
            string arg = args[i].ToLower();
            switch (arg)
            {
                case "-v":
                case "--version":
                    if (i + 1 < args.Length)
                        version = args[++i].ToLower();
                    break;
                case "-n":
                case "--count":
                    if (i + 1 < args.Length && int.TryParse(args[++i], out int c))
                        count = Math.Max(1, Math.Min(c, 1000));
                    break;
                case "-f":
                case "--format":
                    if (i + 1 < args.Length)
                        format = args[++i].ToLower();
                    break;
                case "--ns":
                    if (i + 1 < args.Length)
                        namespaceId = args[++i];
                    break;
                case "--name":
                    if (i + 1 < args.Length)
                        name = args[++i];
                    break;
                case "-h":
                case "--help":
                    PrintUsage();
                    return 0;
            }
        }

        try
        {
            for (int i = 0; i < count; i++)
            {
                Guid uuid = version.ToLower() switch
                {
                    "v4" or "4" or "random" => GenerateV4(),
                    "v7" or "7" or "timestamp" => GenerateV7(),
                    "v5" or "5" or "sha1" => GenerateV5(namespaceId ?? Guid.Empty.ToString(), name ?? string.Empty),
                    "v1" or "1" => GenerateV1(),
                    _ => throw new ArgumentException($"Unsupported UUID version: {version}")
                };

                string output = FormatUuid(uuid, format);
                Console.WriteLine(output);
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
        Console.WriteLine("UUID/GUID Generator - Generate universally unique identifiers");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project UuidGenerator.csproj [options]");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  -v, --version <v>    UUID version: v1, v4 (default), v5, v7");
        Console.WriteLine("  -n, --count <n>      Number of UUIDs to generate (max 1000)");
        Console.WriteLine("  -f, --format <f>     Output format: canonical (default), braced, urn, hex");
        Console.WriteLine("  --ns <namespace>     Namespace GUID for v5 (required for v5)");
        Console.WriteLine("  --name <name>        Name for v5 generation (required for v5)");
        Console.WriteLine("  -h, --help           Show this help message");
        Console.WriteLine();
        Console.WriteLine("Output Formats:");
        Console.WriteLine("  canonical  - 123e4567-e89b-12d3-a456-426614174000");
        Console.WriteLine("  braced     - {123e4567-e89b-12d3-a456-426614174000}");
        Console.WriteLine("  urn        - urn:uuid:123e4567-e89b-12d3-a456-426614174000");
        Console.WriteLine("  hex        - 123e4567e89b12d3a456426614174000");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project UuidGenerator.csproj");
        Console.WriteLine("  dotnet run --project UuidGenerator.csproj -v v4 -n 5");
        Console.WriteLine("  dotnet run --project UuidGenerator.csproj -v v7 -f hex");
        Console.WriteLine("  dotnet run --project UuidGenerator.csproj -v v5 --ns 6ba7b810-9dad-11d1-80b4-00c04fd430c8 --name \"example.com\"");
    }

    /// <summary>
    /// Generate UUID v4 (random)
    /// </summary>
    static Guid GenerateV4()
    {
        byte[] bytes = new byte[16];
        RandomNumberGenerator.Fill(bytes);

        // Set version to 4 (random)
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x40);

        // Set variant to RFC 4122
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        return new Guid(bytes);
    }

    /// <summary>
    /// Generate UUID v7 (Unix timestamp - recommended for new applications)
    /// </summary>
    static Guid GenerateV7()
    {
        byte[] bytes = new byte[16];
        long timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

        // First 6 bytes are timestamp
        byte[] timestampBytes = BitConverter.GetBytes(timestamp);
        if (BitConverter.IsLittleEndian)
            Array.Reverse(timestampBytes);

        Array.Copy(timestampBytes, 0, bytes, 0, 6);

        // Fill remaining with random
        RandomNumberGenerator.Fill(bytes.AsSpan(6));

        // Set version to 7
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x70);

        // Set variant to RFC 4122
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        return new Guid(bytes);
    }

    /// <summary>
    /// Generate UUID v1 (time-based with MAC address)
    /// </summary>
    static Guid GenerateV1()
    {
        // Simplified v1 - uses timestamp + random node ID
        // Full v1 would require MAC address
        byte[] bytes = new byte[16];
        long timestamp = DateTimeOffset.UtcNow.Ticks;

        byte[] timestampBytes = BitConverter.GetBytes(timestamp);
        Array.Copy(timestampBytes, 0, bytes, 0, 8);

        // Fill node ID with random
        RandomNumberGenerator.Fill(bytes.AsSpan(8));

        // Set version to 1
        bytes[6] = (byte)((bytes[6] & 0x0F) | 0x10);

        // Set variant to RFC 4122
        bytes[8] = (byte)((bytes[8] & 0x3F) | 0x80);

        return new Guid(bytes);
    }

    /// <summary>
    /// Generate UUID v5 (name-based with SHA-1)
    /// </summary>
    static Guid GenerateV5(string namespaceId, string name)
    {
        Guid namespaceGuid = Guid.Parse(namespaceId);
        byte[] namespaceBytes = namespaceGuid.ToByteArray();

        // Convert to big-endian for consistent hashing
        SwapEndian(namespaceBytes);

        // Combine namespace and name
        byte[] data = new byte[namespaceBytes.Length + Encoding.UTF8.GetByteCount(name)];
        Array.Copy(namespaceBytes, data, namespaceBytes.Length);
        Array.Copy(Encoding.UTF8.GetBytes(name), 0, data, namespaceBytes.Length, data.Length - namespaceBytes.Length);

        // Hash with SHA-1
        byte[] hash = SHA1.HashData(data);

        // Use first 16 bytes
        byte[] uuidBytes = new byte[16];
        Array.Copy(hash, uuidBytes, 16);

        // Set version to 5
        uuidBytes[6] = (byte)((uuidBytes[6] & 0x0F) | 0x50);

        // Set variant to RFC 4122
        uuidBytes[8] = (byte)((uuidBytes[8] & 0x3F) | 0x80);

        return new Guid(uuidBytes);
    }

    static string FormatUuid(Guid uuid, string format)
    {
        return format.ToLower() switch
        {
            "canonical" or "default" or "std" => uuid.ToString("D"),
            "braced" or "braces" => uuid.ToString("B"),
            "urn" or "uri" => uuid.ToString("P"),
            "hex" or "nohyphens" or "plain" => uuid.ToString("N"),
            _ => uuid.ToString("D")
        };
    }

    static void SwapEndian(byte[] bytes)
    {
        // Swap first 4 bytes
        (bytes[3], bytes[0]) = (bytes[0], bytes[3]);
        (bytes[2], bytes[1]) = (bytes[1], bytes[2]);
        // Swap next 2 bytes
        (bytes[5], bytes[4]) = (bytes[4], bytes[5]);
        // Swap next 2 bytes
        (bytes[7], bytes[6]) = (bytes[6], bytes[7]);
    }
}
