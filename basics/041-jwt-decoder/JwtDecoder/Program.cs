using System;
using System.Text;
using System.Text.Json;

namespace JwtDecoder;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            PrintUsage();
            return;
        }

        string token = string.Join(" ", args);
        DecodeJwt(token);
    }

    static void PrintUsage()
    {
        Console.WriteLine("JWT Decoder - Decode JSON Web Tokens and display their payload");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project JwtDecoder.csproj <jwt_token>");
        Console.WriteLine();
        Console.WriteLine("Example:");
        Console.WriteLine("  dotnet run --project JwtDecoder.csproj eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...");
    }

    static void DecodeJwt(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
        {
            Console.Error.WriteLine("Error: No token provided");
            return;
        }

        string[] parts = token.Split('.');
        if (parts.Length != 3)
        {
            Console.Error.WriteLine("Error: Invalid JWT format. Expected 3 parts separated by dots.");
            return;
        }

        try
        {
            string headerJson = DecodeBase64Url(parts[0]);
            string payloadJson = DecodeBase64Url(parts[1]);
            string signature = parts[2];

            Console.WriteLine("=== JWT Decoded ===");
            Console.WriteLine();

            Console.WriteLine("Header:");
            PrintJson(headerJson);
            Console.WriteLine();

            Console.WriteLine("Payload:");
            PrintJson(payloadJson);
            Console.WriteLine();

            Console.WriteLine("Signature:");
            Console.WriteLine($"{signature[..Math.Min(20, signature.Length)]}...");
            Console.WriteLine();

            // Check for expiration
            if (TryGetClaim(payloadJson, "exp", out long exp))
            {
                DateTime expDate = DateTimeOffset.FromUnixTimeSeconds(exp).DateTime;
                bool isExpired = expDate < DateTime.UtcNow;
                Console.WriteLine($"Expiration: {expDate:yyyy-MM-dd HH:mm:ss} ({(isExpired ? "EXPIRED" : "Valid")})");
            }

            if (TryGetClaim(payloadJson, "iat", out long iat))
            {
                DateTime iatDate = DateTimeOffset.FromUnixTimeSeconds(iat).DateTime;
                Console.WriteLine($"Issued At:  {iatDate:yyyy-MM-dd HH:mm:ss}");
            }
        }
        catch (JsonException ex)
        {
            Console.Error.WriteLine($"Error parsing JWT JSON: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.Error.WriteLine($"Error decoding JWT: {ex.Message}");
        }
    }

    static string DecodeBase64Url(string base64Url)
    {
        // Add padding if necessary
        string base64 = base64Url.Replace('-', '+').Replace('_', '/');
        switch (base64.Length % 4)
        {
            case 2: base64 += "=="; break;
            case 3: base64 += "="; break;
        }

        byte[] bytes = Convert.FromBase64String(base64);
        return Encoding.UTF8.GetString(bytes);
    }

    static void PrintJson(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            string formatted = JsonSerializer.Serialize(doc.RootElement, options);
            Console.WriteLine(formatted);
        }
        catch
        {
            Console.WriteLine(json);
        }
    }

    static bool TryGetClaim(string json, string claimName, out long value)
    {
        value = 0;
        try
        {
            using var doc = JsonDocument.Parse(json);
            if (doc.RootElement.TryGetProperty(claimName, out var element))
            {
                value = element.GetInt64();
                return true;
            }
        }
        catch { }
        return false;
    }
}
