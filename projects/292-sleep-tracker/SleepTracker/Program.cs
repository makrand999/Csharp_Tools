using System.Text.Json;

namespace SleepTracker;

class Program
{
    private const string DataFile = "sleep.json";
    private static List<SleepEntry> _entries = new();

    static void Main()
    {
        Console.WriteLine("😴 Sleep Tracker");
        Console.WriteLine("================\n");

        LoadData();

        Console.WriteLine("Options:");
        Console.WriteLine("  1. Log last night's sleep");
        Console.WriteLine("  2. View sleep history");
        Console.WriteLine("  3. Show statistics");
        Console.WriteLine("  4. Exit");
        Console.Write("\nChoice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1": LogSleep(); break;
            case "2": ShowHistory(); break;
            case "3": ShowStats(); break;
        }
    }

    private static void LogSleep()
    {
        Console.Write("\nDate (YYYY-MM-DD, Enter for yesterday): ");
        string? dateStr = Console.ReadLine();
        DateTime date = string.IsNullOrWhiteSpace(dateStr) ? DateTime.Today.AddDays(-1) : DateTime.Parse(dateStr);

        Console.Write("Bedtime (HH:MM): ");
        TimeSpan? bedtime = TimeSpan.TryParse(Console.ReadLine(), out var bt) ? bt : null;

        Console.Write("Wake time (HH:MM): ");
        TimeSpan? wakeTime = TimeSpan.TryParse(Console.ReadLine(), out var wt) ? wt : null;

        Console.Write("Sleep quality (1-5): ");
        int.TryParse(Console.ReadLine(), out int quality);

        Console.Write("Hours slept (e.g., 7.5): ");
        double.TryParse(Console.ReadLine(), out double hours);

        _entries.Add(new SleepEntry
        {
            Date = date,
            Bedtime = bedtime,
            WakeTime = wakeTime,
            Quality = quality,
            Hours = hours
        });

        SaveData();
        Console.WriteLine("\n✅ Sleep logged!");
    }

    private static void ShowHistory()
    {
        Console.WriteLine("\n📊 Sleep History (Last 7 entries):\n");
        Console.WriteLine($"{"Date",-12} {"Hours",-8} {"Quality",-10} {"Bedtime",-10} {"Wake",-10}");
        Console.WriteLine(new string('-', 55));

        foreach (var entry in _entries.OrderByDescending(e => e.Date).Take(7))
        {
            string stars = new string('⭐', entry.Quality);
            Console.WriteLine($"{entry.Date:yyyy-MM-dd,-12} {entry.Hours,-8} {stars,-10} {entry.Bedtime?.ToString(@"hh\:mm") ?? "--:--",-10} {entry.WakeTime?.ToString(@"hh\:mm") ?? "--:--",-10}");
        }
    }

    private static void ShowStats()
    {
        if (_entries.Count == 0)
        {
            Console.WriteLine("No sleep data yet.");
            return;
        }

        double avgHours = _entries.Average(e => e.Hours);
        double avgQuality = _entries.Average(e => e.Quality);
        var bestNight = _entries.MaxBy(e => e.Quality);

        Console.WriteLine("\n📈 Sleep Statistics");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Total entries: {_entries.Count}");
        Console.WriteLine($"Average sleep: {avgHours:F1} hours");
        Console.WriteLine($"Average quality: {avgQuality:F1}/5 {new string('⭐', (int)Math.Round(avgQuality))}");
        
        if (bestNight != null)
            Console.WriteLine($"Best night: {bestNight.Date:yyyy-MM-dd} ({bestNight.Quality}/5)");
    }

    private static void LoadData()
    {
        if (File.Exists(DataFile))
            _entries = JsonSerializer.Deserialize<List<SleepEntry>>(File.ReadAllText(DataFile)) ?? new();
    }

    private static void SaveData()
    {
        File.WriteAllText(DataFile, JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true }));
    }
}

class SleepEntry
{
    public DateTime Date { get; set; }
    public TimeSpan? Bedtime { get; set; }
    public TimeSpan? WakeTime { get; set; }
    public int Quality { get; set; }
    public double Hours { get; set; }
}
