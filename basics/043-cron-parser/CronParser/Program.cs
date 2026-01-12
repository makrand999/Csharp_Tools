using System;
using System.Collections.Generic;
using System.Linq;

namespace CronParser;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Cron Expression Parser");
            Console.WriteLine("Usage: dotnet run --project CronParser.csproj \"<cron-expression>\"");
            Console.WriteLine("Example: dotnet run --project CronParser.csproj \"*/15 * * * *\"");
            Console.WriteLine();
            Console.WriteLine("Or provide 5 fields: minute hour day month weekday");
            Console.WriteLine("Example: dotnet run --project CronParser.csproj 30 2 * * 1-5");
            return 1;
        }

        string cronExpression = string.Join(" ", args);
        
        try
        {
            var parsed = ParseCronExpression(cronExpression);
            DisplayParsedExpression(parsed, cronExpression);
            DisplayNextOccurrences(parsed);
            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static CronField ParseCronExpression(string expression)
    {
        var parts = expression.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        
        if (parts.Length != 5)
            throw new ArgumentException($"Invalid cron expression: expected 5 fields, got {parts.Length}");

        return new CronField
        {
            Minutes = ParseField(parts[0], 0, 59),
            Hours = ParseField(parts[1], 0, 23),
            Days = ParseField(parts[2], 1, 31),
            Months = ParseField(parts[3], 1, 12),
            Weekdays = ParseField(parts[4], 0, 7) // 0 and 7 both represent Sunday
        };
    }

    static HashSet<int> ParseField(string field, int min, int max)
    {
        var result = new HashSet<int>();

        if (field == "*")
        {
            for (int i = min; i <= max; i++)
                result.Add(i);
            return result;
        }

        foreach (var part in field.Split(','))
        {
            if (part.Contains('/'))
            {
                var segments = part.Split('/');
                int start = segments[0] == "*" ? min : ParseValue(segments[0], min, max);
                int step = int.Parse(segments[1]);
                for (int i = start; i <= max; i += step)
                    result.Add(i);
            }
            else if (part.Contains('-'))
            {
                var segments = part.Split('-');
                int start = ParseValue(segments[0], min, max);
                int end = ParseValue(segments[1], min, max);
                for (int i = start; i <= end; i++)
                    result.Add(i);
            }
            else
            {
                result.Add(ParseValue(part, min, max));
            }
        }

        return result;
    }

    static int ParseValue(string value, int min, int max)
    {
        if (int.TryParse(value, out int result))
            return result;

        // Handle weekday/month names (simplified)
        var dayNames = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase)
        {
            { "sun", 0 }, { "mon", 1 }, { "tue", 2 }, { "wed", 3 },
            { "thu", 4 }, { "fri", 5 }, { "sat", 6 }
        };

        if (dayNames.TryGetValue(value, out int dayValue))
            return dayValue;

        throw new ArgumentException($"Invalid value: {value}");
    }

    static void DisplayParsedExpression(CronField parsed, string expression)
    {
        Console.WriteLine($"Cron Expression: {expression}");
        Console.WriteLine();
        Console.WriteLine("Parsed Fields:");
        Console.WriteLine($"  Minutes:  {FormatField(parsed.Minutes)}");
        Console.WriteLine($"  Hours:    {FormatField(parsed.Hours)}");
        Console.WriteLine($"  Days:     {FormatField(parsed.Days)}");
        Console.WriteLine($"  Months:   {FormatField(parsed.Months)}");
        Console.WriteLine($"  Weekdays: {FormatWeekdays(parsed.Weekdays)}");
        Console.WriteLine();
    }

    static string FormatField(HashSet<int> values)
    {
        if (values.Count == 0) return "none";
        
        var sorted = values.OrderBy(x => x).ToList();
        
        // Check if it's all values (wildcard)
        int expectedCount = sorted.Max() - sorted.Min() + 1;
        if (values.Count == expectedCount && sorted.Min() == 0 && sorted.Max() == 59)
            return "* (every minute)";
        if (values.Count == expectedCount && sorted.Min() == 0 && sorted.Max() == 23)
            return "* (every hour)";
        if (values.Count == expectedCount && sorted.Min() == 1 && sorted.Max() == 31)
            return "* (every day)";
        if (values.Count == expectedCount && sorted.Min() == 1 && sorted.Max() == 12)
            return "* (every month)";

        return string.Join(", ", sorted);
    }

    static string FormatWeekdays(HashSet<int> weekdays)
    {
        var names = new[] { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
        var sorted = weekdays.Where(x => x < 7).OrderBy(x => x).ToList();
        return string.Join(", ", sorted.Select(x => names[x]));
    }

    static void DisplayNextOccurrences(CronField parsed, int count = 5)
    {
        Console.WriteLine($"Next {count} occurrences:");
        
        var now = DateTime.Now;
        var occurrences = new List<DateTime>();
        var current = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0).AddMinutes(1);
        
        while (occurrences.Count < count && current.Year < now.Year + 2)
        {
            if (IsMatch(current, parsed))
            {
                occurrences.Add(current);
            }
            current = current.AddMinutes(1);
        }

        foreach (var occurrence in occurrences)
        {
            Console.WriteLine($"  {occurrence:yyyy-MM-dd HH:mm} ({GetDayDescription(occurrence)})");
        }
    }

    static bool IsMatch(DateTime dt, CronField parsed)
    {
        if (!parsed.Minutes.Contains(dt.Minute)) return false;
        if (!parsed.Hours.Contains(dt.Hour)) return false;
        if (!parsed.Months.Contains(dt.Month)) return false;
        if (!parsed.Days.Contains(dt.Day)) return false;
        if (!parsed.Weekdays.Contains((int)dt.DayOfWeek)) return false;
        return true;
    }

    static string GetDayDescription(DateTime dt)
    {
        var today = DateTime.Today;
        if (dt.Date == today) return "today";
        if (dt.Date == today.AddDays(1)) return "tomorrow";
        return dt.ToString("dddd");
    }
}

class CronField
{
    public HashSet<int> Minutes { get; set; } = new();
    public HashSet<int> Hours { get; set; } = new();
    public HashSet<int> Days { get; set; } = new();
    public HashSet<int> Months { get; set; } = new();
    public HashSet<int> Weekdays { get; set; } = new();
}
