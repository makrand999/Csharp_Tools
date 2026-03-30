using System.Text.Json;

namespace IntermittentFasting;

class Program
{
    private const string DataFile = "fasting.json";
    private static DateTime? _fastStart;
    private static DateTime? _fastEnd;
    private static List<FastSession> _history = new();
    private static int _targetFastHours = 16;

    static void Main()
    {
        Console.WriteLine("⏱️  Intermittent Fasting Tracker");
        Console.WriteLine("================================\n");

        LoadData();

        if (_fastStart.HasValue && !_fastEnd.HasValue)
        {
            ShowActiveFast();
        }
        else
        {
            ShowMenu();
        }
    }

    private static void ShowActiveFast()
    {
        var elapsed = DateTime.Now - _fastStart!.Value;
        var remaining = TimeSpan.FromHours(_targetFastHours) - elapsed;

        Console.WriteLine($"🍽️  Currently Fasting");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Started: {_fastStart:yyyy-MM-dd HH:mm}");
        Console.WriteLine($"Elapsed: {elapsed.Hours}h {elapsed.Minutes}m");
        Console.WriteLine($"Remaining: {Math.Max(0, remaining.Hours)}h {Math.Max(0, remaining.Minutes)}m");
        Console.WriteLine($"Progress: {GetProgressBar(elapsed.TotalHours, _targetFastHours)}");

        Console.WriteLine("\nOptions:");
        Console.WriteLine("  1. End fast");
        Console.WriteLine("  2. Cancel fast");
        Console.Write("\nChoice: ");

        switch (Console.ReadLine())
        {
            case "1":
                _fastEnd = DateTime.Now;
                var duration = _fastEnd.Value - _fastStart.Value;
                _history.Add(new FastSession { Start = _fastStart.Value, End = _fastEnd.Value, Duration = duration.TotalHours });
                SaveData();
                Console.WriteLine($"\n✅ Fast completed! Duration: {duration.Hours}h {duration.Minutes}m");
                break;
            case "2":
                _fastStart = null;
                SaveData();
                Console.WriteLine("\nFast cancelled.");
                break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Options:");
        Console.WriteLine("  1. Start fast");
        Console.WriteLine("  2. View history");
        Console.WriteLine("  3. Set target hours");
        Console.WriteLine("  4. Show statistics");
        Console.Write("\nChoice: ");

        switch (Console.ReadLine())
        {
            case "1":
                _fastStart = DateTime.Now;
                _fastEnd = null;
                SaveData();
                Console.WriteLine($"\n✅ Fast started! Target: {_targetFastHours} hours");
                break;
            case "2":
                ShowHistory();
                break;
            case "3":
                Console.Write("Target fasting hours: ");
                if (int.TryParse(Console.ReadLine(), out int hours))
                {
                    _targetFastHours = hours;
                    SaveData();
                    Console.WriteLine($"Target set to {_targetFastHours} hours");
                }
                break;
            case "4":
                ShowStats();
                break;
        }
    }

    private static void ShowHistory()
    {
        Console.WriteLine("\n📊 Fasting History (Last 5 sessions):\n");
        Console.WriteLine($"{"Date",-12} {"Start",-8} {"End",-8} {"Duration",-10}");
        Console.WriteLine(new string('-', 45));

        foreach (var session in _history.OrderByDescending(s => s.Start).Take(5))
        {
            Console.WriteLine($"{session.Start:yyyy-MM-dd,-12} {session.Start:HH:mm,-8} {session.End:HH:mm,-8} {session.Duration:F1}h");
        }
    }

    private static void ShowStats()
    {
        if (_history.Count == 0)
        {
            Console.WriteLine("No fasting history yet.");
            return;
        }

        double avgDuration = _history.Average(h => h.Duration);
        var longest = _history.MaxBy(h => h.Duration);
        int completed = _history.Count(h => h.Duration >= _targetFastHours);

        Console.WriteLine("\n📈 Fasting Statistics");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Total fasts: {_history.Count}");
        Console.WriteLine($"Average duration: {avgDuration:F1}h");
        Console.WriteLine($"Longest fast: {longest?.Duration:F1}h ({longest?.Start:yyyy-MM-dd})");
        Console.WriteLine($"Completed ({_targetFastHours}h+): {completed}/{_history.Count}");
    }

    private static string GetProgressBar(double current, double max, int length = 25)
    {
        int filled = Math.Min(length, (int)(current / max * length));
        return "[" + new string('█', filled) + new string('░', length - filled) + $"] {current / max * 100:F0}%";
    }

    private static void LoadData()
    {
        if (File.Exists(DataFile))
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(DataFile)) ?? new();
            if (data.ContainsKey("fastStart")) _fastStart = DateTime.Parse(data["fastStart"]!.ToString()!);
            if (data.ContainsKey("fastEnd")) _fastEnd = DateTime.Parse(data["fastEnd"]!.ToString()!);
            if (data.ContainsKey("targetHours")) _targetFastHours = int.Parse(data["targetHours"]!.ToString()!);
            if (data.ContainsKey("history")) _history = JsonSerializer.Deserialize<List<FastSession>>(data["history"]!.ToString()!) ?? new();
        }
    }

    private static void SaveData()
    {
        var data = new { fastStart = _fastStart, fastEnd = _fastEnd, targetHours = _targetFastHours, history = _history };
        File.WriteAllText(DataFile, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }
}

class FastSession
{
    public DateTime Start { get; set; }
    public DateTime End { get; set; }
    public double Duration { get; set; }
}
