using System.Text.Json;

namespace StepCounter;

class Program
{
    private const string DataFile = "steps.json";
    private static int _dailyGoal = 10000;
    private static List<StepEntry> _entries = new();

    static void Main()
    {
        Console.WriteLine("👟 Step Counter");
        Console.WriteLine("===============\n");

        LoadData();

        int todaySteps = GetTodaySteps();
        Console.WriteLine($"Today's steps: {todaySteps:N0} / {_dailyGoal:N0}");
        Console.WriteLine($"Progress: {GetProgressBar(todaySteps, _dailyGoal)}\n");

        Console.WriteLine("Options:");
        Console.WriteLine("  1. Add steps");
        Console.WriteLine("  2. Set daily goal");
        Console.WriteLine("  3. View history");
        Console.WriteLine("  4. Show statistics");
        Console.Write("\nChoice: ");

        switch (Console.ReadLine())
        {
            case "1":
                Console.Write("Steps to add: ");
                if (int.TryParse(Console.ReadLine(), out int steps))
                {
                    _entries.Add(new StepEntry { Date = DateTime.Today, Steps = steps, LoggedAt = DateTime.Now });
                    SaveData();
                    Console.WriteLine($"\n✅ Added {steps:N0} steps!");
                }
                break;
            case "2":
                Console.Write("Daily step goal: ");
                if (int.TryParse(Console.ReadLine(), out int goal))
                {
                    _dailyGoal = goal;
                    SaveData();
                    Console.WriteLine($"Goal set to {_dailyGoal:N0} steps");
                }
                break;
            case "3": ShowHistory(); break;
            case "4": ShowStats(); break;
        }
    }

    private static int GetTodaySteps()
    {
        return _entries.Where(e => e.Date == DateTime.Today).Sum(e => e.Steps);
    }

    private static void ShowHistory()
    {
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.Today.AddDays(-i))
            .Select(date => new { Date = date, Steps = _entries.Where(e => e.Date == date).Sum(e => e.Steps) });

        Console.WriteLine("\n📊 Last 7 Days:\n");
        Console.WriteLine($"{"Date",-12} {"Steps",-12} {"Goal",-10}");
        Console.WriteLine(new string('-', 35));

        foreach (var day in last7Days)
        {
            string met = day.Steps >= _dailyGoal ? "✅" : "⏳";
            Console.WriteLine($"{day.Date:yyyy-MM-dd,-12} {day.Steps,-12:N0} {met}");
        }
    }

    private static void ShowStats()
    {
        var byDay = _entries.GroupBy(e => e.Date.Date)
            .Select(g => new { Date = g.Key, Total = g.Sum(e => e.Steps) })
            .ToList();

        if (byDay.Count == 0)
        {
            Console.WriteLine("No step data yet.");
            return;
        }

        int avgSteps = (int)byDay.Average(d => d.Total);
        var bestDay = byDay.MaxBy(d => d.Total);
        int daysMet = byDay.Count(d => d.Total >= _dailyGoal);

        Console.WriteLine("\n📈 Step Statistics");
        Console.WriteLine(new string('-', 40));
        Console.WriteLine($"Days tracked: {byDay.Count}");
        Console.WriteLine($"Average steps: {avgSteps:N0}");
        Console.WriteLine($"Best day: {bestDay?.Date:yyyy-MM-dd} ({bestDay?.Total:N0} steps)");
        Console.WriteLine($"Days goal met: {daysMet}/{byDay.Count}");
    }

    private static string GetProgressBar(int current, int max, int length = 30)
    {
        int filled = Math.Min(length, (current * length) / max);
        return "[" + new string('█', filled) + new string('░', length - filled) + $"] {current / (double)max * 100:F0}%";
    }

    private static void LoadData()
    {
        if (File.Exists(DataFile))
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(DataFile)) ?? new();
            if (data.ContainsKey("goal")) _dailyGoal = int.Parse(data["goal"]!.ToString()!);
            if (data.ContainsKey("entries")) _entries = JsonSerializer.Deserialize<List<StepEntry>>(data["entries"]!.ToString()!) ?? new();
        }
    }

    private static void SaveData()
    {
        var data = new { goal = _dailyGoal, entries = _entries };
        File.WriteAllText(DataFile, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }
}

class StepEntry
{
    public DateTime Date { get; set; }
    public int Steps { get; set; }
    public DateTime LoggedAt { get; set; }
}
