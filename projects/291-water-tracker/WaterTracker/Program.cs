using System.Text.Json;

namespace WaterTracker;

class Program
{
    private const string DataFile = "water.json";
    private static int _dailyGoal = 2000; // ml
    private static List<WaterEntry> _entries = new();

    static void Main()
    {
        Console.WriteLine("💧 Water Intake Tracker");
        Console.WriteLine("======================\n");

        LoadData();

        int todayIntake = GetTodayIntake();
        int remaining = _dailyGoal - todayIntake;

        Console.WriteLine($"Today's intake: {todayIntake}ml / {_dailyGoal}ml");
        Console.WriteLine($"Progress: {GetProgressBar(todayIntake, _dailyGoal)}");
        Console.WriteLine($"Remaining: {Math.Max(0, remaining)}ml\n");

        Console.WriteLine("Options:");
        Console.WriteLine("  1. Log water (250ml)");
        Console.WriteLine("  2. Log custom amount");
        Console.WriteLine("  3. Set daily goal");
        Console.WriteLine("  4. View history");
        Console.WriteLine("  5. Exit");
        Console.Write("\nChoice: ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                LogWater(250);
                break;
            case "2":
                Console.Write("Amount (ml): ");
                if (int.TryParse(Console.ReadLine(), out int amount))
                    LogWater(amount);
                break;
            case "3":
                Console.Write("Daily goal (ml): ");
                if (int.TryParse(Console.ReadLine(), out int goal))
                {
                    _dailyGoal = goal;
                    SaveData();
                    Console.WriteLine($"Goal set to {_dailyGoal}ml");
                }
                break;
            case "4":
                ShowHistory();
                break;
        }
    }

    private static void LogWater(int amount)
    {
        _entries.Add(new WaterEntry { Date = DateTime.Today, Amount = amount, LoggedAt = DateTime.Now });
        SaveData();
        Console.WriteLine($"\n✅ Logged {amount}ml of water!");
        
        int todayIntake = GetTodayIntake();
        if (todayIntake >= _dailyGoal)
            Console.WriteLine("🎉 Daily goal reached! Great job staying hydrated!");
    }

    private static int GetTodayIntake()
    {
        return _entries.Where(e => e.Date == DateTime.Today).Sum(e => e.Amount);
    }

    private static void ShowHistory()
    {
        var last7Days = Enumerable.Range(0, 7)
            .Select(i => DateTime.Today.AddDays(-i))
            .Select(date => new { Date = date, Total = _entries.Where(e => e.Date == date).Sum(e => e.Amount) });

        Console.WriteLine("\n📊 Last 7 Days:");
        foreach (var day in last7Days)
        {
            string bar = GetProgressBar(day.Total, _dailyGoal, 20);
            Console.WriteLine($"{day.Date:ddd}: {day.Total,4}ml {bar}");
        }
    }

    private static string GetProgressBar(int current, int max, int length = 30)
    {
        int filled = Math.Min(length, (current * length) / max);
        return "[" + new string('█', filled) + new string('░', length - filled) + "]";
    }

    private static void LoadData()
    {
        if (File.Exists(DataFile))
        {
            var data = JsonSerializer.Deserialize<Dictionary<string, object>>(File.ReadAllText(DataFile)) ?? new();
            if (data.ContainsKey("goal")) _dailyGoal = int.Parse(data["goal"]!.ToString()!);
            if (data.ContainsKey("entries"))
                _entries = JsonSerializer.Deserialize<List<WaterEntry>>(data["entries"]!.ToString()!) ?? new();
        }
    }

    private static void SaveData()
    {
        var data = new { goal = _dailyGoal, entries = _entries };
        File.WriteAllText(DataFile, JsonSerializer.Serialize(data, new JsonSerializerOptions { WriteIndented = true }));
    }
}

class WaterEntry
{
    public DateTime Date { get; set; }
    public int Amount { get; set; }
    public DateTime LoggedAt { get; set; }
}
