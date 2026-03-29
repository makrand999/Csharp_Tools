using System.Text.Json;

namespace MoodTracker;

class Program
{
    private const string DataFile = "mood.json";
    private static List<MoodEntry> _entries = new();

    static void Main()
    {
        Console.WriteLine("😊 Mood Tracker");
        Console.WriteLine("===============\n");

        LoadData();

        Console.WriteLine("Options:");
        Console.WriteLine("  1. Log today's mood");
        Console.WriteLine("  2. View mood history");
        Console.WriteLine("  3. Show patterns");
        Console.Write("\nChoice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1": LogMood(); break;
            case "2": ShowHistory(); break;
            case "3": ShowPatterns(); break;
        }
    }

    private static void LogMood()
    {
        Console.WriteLine("\nHow are you feeling today?");
        Console.WriteLine("  5. 😄 Excellent");
        Console.WriteLine("  4. 🙂 Good");
        Console.WriteLine("  3. 😐 Neutral");
        Console.WriteLine("  2. 🙁 Bad");
        Console.WriteLine("  1. 😞 Terrible");
        Console.Write("\nYour mood (1-5): ");

        if (!int.TryParse(Console.ReadLine(), out int mood) || mood < 1 || mood > 5)
        {
            Console.WriteLine("Invalid rating.");
            return;
        }

        Console.Write("Notes (optional): ");
        string? notes = Console.ReadLine();

        _entries.Add(new MoodEntry
        {
            Date = DateTime.Today,
            Mood = mood,
            Notes = notes ?? ""
        });

        SaveData();
        Console.WriteLine("\n✅ Mood logged!");
    }

    private static void ShowHistory()
    {
        Console.WriteLine("\n📊 Mood History (Last 7 entries):\n");
        
        foreach (var entry in _entries.OrderByDescending(e => e.Date).Take(7))
        {
            string emoji = GetMoodEmoji(entry.Mood);
            Console.WriteLine($"{entry.Date:yyyy-MM-dd}: {emoji} ({entry.Mood}/5)");
            if (!string.IsNullOrEmpty(entry.Notes))
                Console.WriteLine($"   \"{entry.Notes}\"");
        }
    }

    private static void ShowPatterns()
    {
        if (_entries.Count == 0)
        {
            Console.WriteLine("No mood data yet.");
            return;
        }

        double avgMood = _entries.Average(e => e.Mood);
        var bestDay = _entries.MaxBy(e => e.Mood);
        var worstDay = _entries.MinBy(e => e.Mood);

        // Weekly pattern
        var byDayOfWeek = _entries.GroupBy(e => e.Date.DayOfWeek)
            .Select(g => new { Day = g.Key, Avg = g.Average(e => e.Mood) })
            .OrderBy(x => (int)x.Day);

        Console.WriteLine("\n📈 Mood Patterns");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Average mood: {avgMood:F1}/5 {GetMoodEmoji((int)Math.Round(avgMood))}");
        Console.WriteLine($"Best day: {bestDay?.Date:yyyy-MM-dd} ({bestDay?.Mood}/5)");
        Console.WriteLine($"Worst day: {worstDay?.Date:yyyy-MM-dd} ({worstDay?.Mood}/5)");

        Console.WriteLine("\nBy Day of Week:");
        foreach (var d in byDayOfWeek)
        {
            Console.WriteLine($"  {d.Day,-10}: {d.Avg:F1}/5");
        }
    }

    private static string GetMoodEmoji(int mood) => mood switch
    {
        5 => "😄",
        4 => "🙂",
        3 => "😐",
        2 => "🙁",
        1 => "😞",
        _ => "?"
    };

    private static void LoadData()
    {
        if (File.Exists(DataFile))
            _entries = JsonSerializer.Deserialize<List<MoodEntry>>(File.ReadAllText(DataFile)) ?? new();
    }

    private static void SaveData()
    {
        File.WriteAllText(DataFile, JsonSerializer.Serialize(_entries, new JsonSerializerOptions { WriteIndented = true }));
    }
}

class MoodEntry
{
    public DateTime Date { get; set; }
    public int Mood { get; set; }
    public string Notes { get; set; } = "";
}
