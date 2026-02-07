namespace TimezoneConverter;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Timezone Converter");
            Console.WriteLine("Usage: dotnet run --project 128-timezone-converter.csproj -- <datetime> <from-tz> <to-tz>");
            Console.WriteLine("Example: dotnet run --project 128-timezone-converter.csproj -- \"2026-04-01 12:00\" \"America/New_York\" \"Asia/Tokyo\"");
            Console.WriteLine("         dotnet run --project 128-timezone-converter.csproj -- now \"UTC\" \"Europe/London\"");
            Console.WriteLine("\nCommon timezones:");
            Console.WriteLine("  UTC, America/New_York, America/Los_Angeles, Europe/London, Europe/Paris");
            Console.WriteLine("  Asia/Tokyo, Asia/Shanghai, Asia/Dubai, Australia/Sydney, Pacific/Auckland");
            return;
        }

        string dateTimeStr = args[0];
        string fromTz = args.Length > 1 ? args[1] : "UTC";
        string toTz = args.Length > 2 ? args[2] : "UTC";

        try
        {
            // Parse the input datetime
            DateTime inputDate;
            if (dateTimeStr.ToLower() == "now")
            {
                inputDate = DateTime.Now;
            }
            else if (!DateTime.TryParse(dateTimeStr, null, System.Globalization.DateTimeStyles.AssumeLocal, out inputDate))
            {
                Console.WriteLine($"Error: Could not parse datetime '{dateTimeStr}'");
                Console.WriteLine("Try formats: yyyy-MM-dd HH:mm, MM/dd/yyyy, etc.");
                return;
            }

            // Get timezones
            TimeZoneInfo? fromTimeZone = GetTimeZone(fromTz);
            TimeZoneInfo? toTimeZone = GetTimeZone(toTz);

            if (fromTimeZone == null)
            {
                Console.WriteLine($"Error: Unknown timezone '{fromTz}'");
                ListAvailableTimezones(fromTz);
                return;
            }

            if (toTimeZone == null)
            {
                Console.WriteLine($"Error: Unknown timezone '{toTz}'");
                ListAvailableTimezones(toTz);
                return;
            }

            // Convert
            DateTime fromTime = TimeZoneInfo.ConvertTime(inputDate, fromTimeZone, toTimeZone);

            Console.WriteLine("=== Timezone Conversion ===");
            Console.WriteLine();
            Console.WriteLine($"Input:  {inputDate:yyyy-MM-dd HH:mm:ss} ({fromTz})");
            Console.WriteLine($"Output: {fromTime:yyyy-MM-dd HH:mm:ss} ({toTz})");
            Console.WriteLine();

            // Show current time in both timezones
            var nowUtc = DateTime.UtcNow;
            var nowFrom = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, fromTimeZone);
            var nowTo = TimeZoneInfo.ConvertTimeFromUtc(nowUtc, toTimeZone);

            Console.WriteLine("=== Current Time ===");
            Console.WriteLine($"{fromTz}: {nowFrom:yyyy-MM-dd HH:mm:ss} ({fromTimeZone.GetUtcOffset(nowFrom):hh\\:mm})");
            Console.WriteLine($"{toTz}: {nowTo:yyyy-MM-dd HH:mm:ss} ({toTimeZone.GetUtcOffset(nowTo):hh\\:mm})");
            Console.WriteLine();

            // Time difference
            var offsetDiff = toTimeZone.GetUtcOffset(nowTo) - fromTimeZone.GetUtcOffset(nowFrom);
            var hoursDiff = offsetDiff.TotalHours;
            Console.WriteLine($"Time Difference: {hoursDiff:+##;-##;0} hours");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static TimeZoneInfo? GetTimeZone(string tzId)
    {
        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(tzId);
        }
        catch
        {
            // Try common aliases
            var aliases = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
            {
                { "UTC", "UTC" },
                { "GMT", "GMT Standard Time" },
                { "EST", "Eastern Standard Time" },
                { "CST", "Central Standard Time" },
                { "MST", "Mountain Standard Time" },
                { "PST", "Pacific Standard Time" },
                { "JST", "Tokyo" },
                { "CET", "Romance Standard Time" },
                { "EET", "FLE Standard Time" },
                { "AEST", "AUS Eastern Standard Time" }
            };

            if (aliases.TryGetValue(tzId, out var realTz))
            {
                try
                {
                    return TimeZoneInfo.FindSystemTimeZoneById(realTz);
                }
                catch
                {
                    return null;
                }
            }

            return null;
        }
    }

    static void ListAvailableTimezones(string search)
    {
        var allTzs = TimeZoneInfo.GetSystemTimeZones();
        var matches = allTzs.Where(t => t.Id.Contains(search, StringComparison.OrdinalIgnoreCase) 
                                        || t.DisplayName.Contains(search, StringComparison.OrdinalIgnoreCase))
                            .Take(10)
                            .ToList();

        if (matches.Any())
        {
            Console.WriteLine("\nDid you mean one of these?");
            foreach (var tz in matches)
            {
                Console.WriteLine($"  {tz.Id} - {tz.DisplayName}");
            }
        }
    }
}
