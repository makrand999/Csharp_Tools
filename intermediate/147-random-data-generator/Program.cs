using System.Security.Cryptography;
using System.Text;

namespace RandomDataGenerator;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Random Data Generator");
            Console.WriteLine("Usage: dotnet run --project 147-random-data-generator.csproj -- <type> [count] [options]");
            Console.WriteLine("\nTypes:");
            Console.WriteLine("  uuid        Generate UUIDs");
            Console.WriteLine("  password    Generate passwords");
            Console.WriteLine("  string      Generate random strings");
            Console.WriteLine("  number      Generate random numbers");
            Console.WriteLine("  bytes       Generate random bytes (hex)");
            Console.WriteLine("  date        Generate random dates");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --count N     Number of items (default: 1)");
            Console.WriteLine("  --length N    Length of string/password (default: 16)");
            Console.WriteLine("  --min N       Minimum value for numbers");
            Console.WriteLine("  --max N       Maximum value for numbers");
            return;
        }

        string type = args[0];
        int count = 1;
        int length = 16;
        int? min = null;
        int? max = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--count" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out count);
            }
            else if (args[i] == "--length" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out length);
            }
            else if (args[i] == "--min" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out var v);
                min = v;
            }
            else if (args[i] == "--max" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out var v);
                max = v;
            }
        }

        var random = RandomNumberGenerator.Create();

        for (int i = 0; i < count; i++)
        {
            string result = type.ToLower() switch
            {
                "uuid" => Guid.NewGuid().ToString(),
                "password" => GeneratePassword(length, random),
                "string" => GenerateRandomString(length, random),
                "number" => GenerateRandomNumber(random, min, max),
                "bytes" => GenerateRandomBytes(length, random),
                "date" => GenerateRandomDate(random, min, max),
                _ => "Unknown type"
            };
            Console.WriteLine(result);
        }
    }

    static string GeneratePassword(int length, RandomNumberGenerator rng)
    {
        const string upper = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lower = "abcdefghijklmnopqrstuvwxyz";
        const string digits = "0123456789";
        const string special = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        var all = upper + lower + digits + special;
        var password = new StringBuilder();

        // Ensure at least one of each type
        password.Append(upper[GetRandomInt(rng, upper.Length)]);
        password.Append(lower[GetRandomInt(rng, lower.Length)]);
        password.Append(digits[GetRandomInt(rng, digits.Length)]);
        password.Append(special[GetRandomInt(rng, special.Length)]);

        // Fill rest randomly
        for (int i = password.Length; i < length; i++)
        {
            password.Append(all[GetRandomInt(rng, all.Length)]);
        }

        // Shuffle
        var chars = password.ToString().ToCharArray();
        for (int i = chars.Length - 1; i > 0; i--)
        {
            int j = GetRandomInt(rng, i + 1);
            (chars[i], chars[j]) = (chars[j], chars[i]);
        }

        return new string(chars);
    }

    static string GenerateRandomString(int length, RandomNumberGenerator rng)
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
        var result = new StringBuilder();
        for (int i = 0; i < length; i++)
        {
            result.Append(chars[GetRandomInt(rng, chars.Length)]);
        }
        return result.ToString();
    }

    static string GenerateRandomNumber(RandomNumberGenerator rng, int? min, int? max)
    {
        int minimum = min ?? 0;
        int maximum = max ?? int.MaxValue;
        return (GetRandomInt(rng, maximum - minimum) + minimum).ToString();
    }

    static string GenerateRandomBytes(int count, RandomNumberGenerator rng)
    {
        var bytes = new byte[count];
        rng.GetBytes(bytes);
        return BitConverter.ToString(bytes).Replace("-", "").ToLower();
    }

    static string GenerateRandomDate(RandomNumberGenerator rng, int? minYear, int? maxYear)
    {
        int min = minYear ?? 2000;
        int max = maxYear ?? DateTime.Now.Year;
        
        var start = new DateTime(min, 1, 1);
        var range = (max - min) * 365 + 100;
        var days = GetRandomInt(rng, range);
        var date = start.AddDays(days);
        
        return date.ToString("yyyy-MM-dd");
    }

    static int GetRandomInt(RandomNumberGenerator rng, int maxValue)
    {
        var buffer = new byte[4];
        rng.GetBytes(buffer);
        return BitConverter.ToInt32(buffer) % maxValue;
    }
}
