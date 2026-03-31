using System.Text.Json;

namespace WellnessDashboard;

class Program
{
    private const string DataFile = "wellness.json";
    private static WellnessData _data = new();

    static void Main()
    {
        Console.WriteLine("🏥 Wellness Dashboard");
        Console.WriteLine("====================\n");

        LoadData();

        Console.WriteLine("Options:");
        Console.WriteLine("  1. Quick log (all metrics)");
        Console.WriteLine("  2. View today's summary");
        Console.WriteLine("  3. Weekly overview");
        Console.WriteLine("  4. Clear data");
        Console.Write("\nChoice: ");

        switch (Console.ReadLine())
        {
            case "1": QuickLog(); break;
            case "2": TodaySummary(); break;
            case "3": WeeklyOverview(); break;
            case "4": ClearData(); break;
        }
    }

    private static void QuickLog()
    {
        Console.WriteLine("\n📝 Quick Wellness Check-in\n");

        Console.Write("💧 Water (glasses, ~250ml each): ");
        int.TryParse(Console.ReadLine(), out int water);

        Console.Write("😴 Sleep (hours): ");
        double.TryParse(Console.ReadLine(), out double sleep);

        Console.Write("👟 Steps (in thousands, e.g., 8 for 8000): ");
        int.TryParse(Console.ReadLine(), out int steps);

        Console.Write("😊 Mood (1-5): ");
        int.TryParse(Console.ReadLine(), out int mood);

        Console.Write("🧘 Exercise (minutes): ");
        int.TryParse(Console.ReadLine(), out int exercise);

        Console.Write("🥗 Healthy meals (count): ");
        int.TryParse(Console.ReadLine(), out int meals);

        _data.DailyLogs.Add(new DailyLog
        {
            Date = DateTime.Today,
            WaterGlasses = water,
            SleepHours = sleep,
            Steps = steps * 1000,
            Mood = mood,
            ExerciseMinutes = exercise,
            HealthyMeals = meals,
            LoggedAt = DateTime.Now
        });

        SaveData();

        Console.WriteLine("\n✅ Wellness data logged!");
        ShowTodaySummary();
    }

    private static void TodaySummary()
    {
        var today = _data.DailyLogs.FirstOrDefault(l => l.Date == DateTime.Today);
        if (today == null)
        {
            Console.WriteLine("\nNo data logged for today yet.");
            return;
        }

        ShowTodaySummary(today);
    }

    private static void ShowTodaySummary(DailyLog? log = null)
    {
        log ??= _data.DailyLogs.FirstOrDefault(l => l.Date == DateTime.Today);
        if (log == null) return;

        Console.WriteLine("\n📊 Today's Wellness Summary");
        Console.WriteLine(new string('=', 50));

        Console.WriteLine($"\n💧 Water: {log.WaterGlasses} glasses ({log.WaterGlasses * 250}ml)");
        Console.WriteLine($"   {GetBar(log.WaterGlasses, 8)}");

        Console.WriteLine($"\n😴 Sleep: {log.SleepHours:F1} hours");
        Console.WriteLine($"   {GetBar(log.SleepHours, 8)}");

        Console.WriteLine($"\n👟 Steps: {log.Steps:N0}");
        Console.WriteLine($"   {GetBar(log.Steps, 10000)}");

        Console.WriteLine($"\n😊 Mood: {log.Mood}/5 {new string('⭐', log.Mood)}");

        Console.WriteLine($"\n🧘 Exercise: {log.ExerciseMinutes} min");
        Console.WriteLine($"   {GetBar(log.ExerciseMinutes, 30)}");

        Console.WriteLine($"\n🥗 Healthy Meals: {log.HealthyMeals}");

        // Overall wellness score
        double score = CalculateScore(log);
        Console.WriteLine($"\n📈 Wellness Score: {score:F0}/100");
        Console.WriteLine($"   {GetBar(score, 100, 25)}");
    }

    private static void WeeklyOverview()
    {
        var last7Days = _data.DailyLogs
            .Where(l => l.Date >= DateTime.Today.AddDays(-6))
            .OrderBy(l => l.Date)
            .ToList();

        if (last7Days.Count == 0)
        {
            Console.WriteLine("\nNo data for the past week.");
            return;
        }

        Console.WriteLine("\n📊 Weekly Wellness Overview");
        Console.WriteLine(new string('=', 60));

        Console.WriteLine($"\n{"Day",-10} {"Water",-8} {"Sleep",-8} {"Steps",-10} {"Mood",-8} {"Exercise",-10}");
        Console.WriteLine(new string('-', 60));

        foreach (var log in last7Days)
        {
            Console.WriteLine($"{log.Date:ddd,-10} {log.WaterGlasses,-8} {log.SleepHours,-8:F1} {log.Steps,-10:N0} {log.Mood,-8} {log.ExerciseMinutes,-10}");
        }

        // Averages
        Console.WriteLine(new string('-', 60));
        Console.WriteLine($"{"Avg",-10} {last7Days.Average(l => l.WaterGlasses):F0,-8} {last7Days.Average(l => l.SleepHours),-8:F1} {last7Days.Average(l => l.Steps),-10:N0} {last7Days.Average(l => l.Mood):F1,-8} {last7Days.Average(l => l.ExerciseMinutes),-10:F0}");

        // Weekly score
        double avgScore = last7Days.Average(CalculateScore);
        Console.WriteLine($"\n📈 Weekly Wellness Score: {avgScore:F0}/100");
    }

    private static double CalculateScore(DailyLog log)
    {
        double waterScore = Math.Min(100, (log.WaterGlasses / 8.0) * 100);
        double sleepScore = Math.Min(100, (log.SleepHours / 8.0) * 100);
        double stepsScore = Math.Min(100, (log.Steps / 10000.0) * 100);
        double moodScore = (log.Mood / 5.0) * 100;
        double exerciseScore = Math.Min(100, (log.ExerciseMinutes / 30.0) * 100);

        return (waterScore + sleepScore + stepsScore + moodScore + exerciseScore) / 5;
    }

    private static void ClearData()
    {
        Console.Write("\n⚠️  Are you sure you want to clear all data? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _data.DailyLogs.Clear();
            SaveData();
            Console.WriteLine("✅ All data cleared.");
        }
    }

    private static string GetBar(double current, double max, int length = 20)
    {
        int filled = Math.Min(length, (int)(current / max * length));
        return "[" + new string('█', filled) + new string('░', length - filled) + "]";
    }

    private static void LoadData()
    {
        if (File.Exists(DataFile))
            _data = JsonSerializer.Deserialize<WellnessData>(File.ReadAllText(DataFile)) ?? new WellnessData();
    }

    private static void SaveData()
    {
        File.WriteAllText(DataFile, JsonSerializer.Serialize(_data, new JsonSerializerOptions { WriteIndented = true }));
    }
}

class WellnessData
{
    public List<DailyLog> DailyLogs { get; set; } = new();
}

class DailyLog
{
    public DateTime Date { get; set; }
    public int WaterGlasses { get; set; }
    public double SleepHours { get; set; }
    public int Steps { get; set; }
    public int Mood { get; set; }
    public int ExerciseMinutes { get; set; }
    public int HealthyMeals { get; set; }
    public DateTime LoggedAt { get; set; }
}
