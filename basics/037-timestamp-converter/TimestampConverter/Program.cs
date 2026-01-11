using System;
using System.Globalization;

namespace TimestampConverter;

class Program
{
    static readonly DateTime UnixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

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
            case "to-unix":
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Error: Missing date argument");
                    Console.WriteLine("Usage: dotnet run --project TimestampConverter.csproj to-unix <date>");
                    return;
                }
                ConvertToUnix(string.Join(" ", args[1..]));
                break;

            case "from-unix":
                if (args.Length < 2)
                {
                    Console.Error.WriteLine("Error: Missing timestamp argument");
                    Console.WriteLine("Usage: dotnet run --project TimestampConverter.csproj from-unix <timestamp>");
                    return;
                }
                ConvertFromUnix(args[1]);
                break;

            case "now":
                ShowCurrentTimestamp();
                break;

            default:
                Console.Error.WriteLine($"Error: Unknown command '{command}'");
                PrintUsage();
                break;
        }
    }

    static void PrintUsage()
    {
        Console.WriteLine("Unix Timestamp Converter - Convert between Unix timestamps and human-readable dates");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project TimestampConverter.csproj <command> [arguments]");
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  to-unix <date>     Convert a date to Unix timestamp");
        Console.WriteLine("  from-unix <ts>     Convert Unix timestamp to readable date");
        Console.WriteLine("  now                Show current Unix timestamp");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project TimestampConverter.csproj to-unix \"2024-01-15 10:30:00\"");
        Console.WriteLine("  dotnet run --project TimestampConverter.csproj from-unix 1705312200");
        Console.WriteLine("  dotnet run --project TimestampConverter.csproj now");
    }

    static void ConvertToUnix(string dateString)
    {
        if (DateTime.TryParse(dateString, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime date))
        {
            long timestamp = (long)(date.ToUniversalTime() - UnixEpoch).TotalSeconds;
            Console.WriteLine($"Input:    {date}");
            Console.WriteLine($"UTC:      {date.ToUniversalTime():yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Timestamp: {timestamp}");
        }
        else
        {
            Console.Error.WriteLine($"Error: Could not parse date '{dateString}'");
            Console.WriteLine("Try formats like: yyyy-MM-dd HH:mm:ss, MM/dd/yyyy, dd-MM-yyyy");
        }
    }

    static void ConvertFromUnix(string timestampString)
    {
        if (long.TryParse(timestampString, out long timestamp))
        {
            DateTime date = UnixEpoch.AddSeconds(timestamp);
            Console.WriteLine($"Timestamp: {timestamp}");
            Console.WriteLine($"UTC:       {date:yyyy-MM-dd HH:mm:ss}");
            Console.WriteLine($"Local:     {date.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
        }
        else
        {
            Console.Error.WriteLine($"Error: Invalid timestamp '{timestampString}'");
        }
    }

    static void ShowCurrentTimestamp()
    {
        DateTime now = DateTime.UtcNow;
        long timestamp = (long)(now - UnixEpoch).TotalSeconds;
        
        Console.WriteLine($"Current Unix Timestamp: {timestamp}");
        Console.WriteLine($"UTC Time:               {now:yyyy-MM-dd HH:mm:ss}");
        Console.WriteLine($"Local Time:             {now.ToLocalTime():yyyy-MM-dd HH:mm:ss}");
    }
}
