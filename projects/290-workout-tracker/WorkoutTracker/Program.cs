using System.Text.Json;

namespace WorkoutTracker;

class Program
{
    private const string DataFile = "workouts.json";
    private static List<Workout> _workouts = new();

    static void Main(string[] args)
    {
        Console.WriteLine("💪 Workout Tracker");
        Console.WriteLine("==================\n");

        LoadWorkouts();

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();
        switch (command)
        {
            case "log": LogWorkout(); break;
            case "list": ListWorkouts(); break;
            case "stats": ShowStats(); break;
            case "delete": DeleteWorkout(args.ElementAtOrDefault(1)); break;
            default: ShowMenu(); break;
        }
    }

    private static void ShowMenu()
    {
        Console.WriteLine("Commands:");
        Console.WriteLine("  log    - Log a workout");
        Console.WriteLine("  list   - List all workouts");
        Console.WriteLine("  stats  - Show statistics");
        Console.WriteLine("  delete - Delete workout");
        Console.WriteLine();

        Console.Write("Enter command: ");
        string? input = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(input)) return;

        var parts = input.Trim().Split(' ', 2);
        string cmd = parts[0].ToLower();
        string? arg = parts.Length > 1 ? parts[1] : null;

        switch (cmd)
        {
            case "log": LogWorkout(); break;
            case "list": ListWorkouts(); break;
            case "stats": ShowStats(); break;
            case "delete": DeleteWorkout(arg); break;
        }
    }

    private static void LoadWorkouts()
    {
        if (File.Exists(DataFile))
        {
            string json = File.ReadAllText(DataFile);
            _workouts = JsonSerializer.Deserialize<List<Workout>>(json) ?? new List<Workout>();
        }
    }

    private static void SaveWorkouts()
    {
        string json = JsonSerializer.Serialize(_workouts, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(DataFile, json);
    }

    private static void LogWorkout()
    {
        Console.Write("\nDate (YYYY-MM-DD, Enter for today): ");
        string? dateStr = Console.ReadLine();
        DateTime date = string.IsNullOrWhiteSpace(dateStr) ? DateTime.Today : DateTime.Parse(dateStr);

        Console.Write("Workout type (e.g., Running, Weightlifting, Yoga): ");
        string? type = Console.ReadLine();

        Console.Write("Duration (minutes): ");
        string? durationStr = Console.ReadLine();
        int.TryParse(durationStr, out int duration);

        Console.Write("Calories burned (optional): ");
        string? caloriesStr = Console.ReadLine();
        int.TryParse(caloriesStr, out int calories);

        Console.Write("Intensity (1-5, 5 being hardest): ");
        string? intensityStr = Console.ReadLine();
        int.TryParse(intensityStr, out int intensity);

        Console.Write("Notes (optional): ");
        string? notes = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(type) || duration <= 0)
        {
            Console.WriteLine("Workout type and duration are required!");
            return;
        }

        var workout = new Workout
        {
            Id = Guid.NewGuid().ToString("N")[..8],
            Date = date,
            Type = type!,
            Duration = duration,
            Calories = calories > 0 ? calories : null,
            Intensity = intensity > 0 ? intensity : 3,
            Notes = notes ?? "",
            LoggedAt = DateTime.Now
        };

        _workouts.Add(workout);
        SaveWorkouts();

        Console.WriteLine($"\n✅ Workout logged: {workout.Type}");
        Console.WriteLine($"   Date: {workout.Date:yyyy-MM-dd}");
        Console.WriteLine($"   Duration: {workout.Duration} min | Intensity: {GetIntensityDisplay(workout.Intensity)}");
    }

    private static void ListWorkouts()
    {
        if (_workouts.Count == 0)
        {
            Console.WriteLine("No workouts logged yet.");
            return;
        }

        Console.Write("\nFilter by type (or Enter for all): ");
        string? typeFilter = Console.ReadLine();

        var filtered = string.IsNullOrWhiteSpace(typeFilter)
            ? _workouts
            : _workouts.Where(w => w.Type.Equals(typeFilter!.Trim(), StringComparison.OrdinalIgnoreCase)).ToList();

        if (filtered.Count == 0)
        {
            Console.WriteLine("No workouts found of that type.");
            return;
        }

        Console.WriteLine($"\n{"ID",-10} {"Date",-12} {"Type",-20} {"Duration",-10} {"Intensity",-10}");
        Console.WriteLine(new string('-', 65));

        foreach (var workout in filtered.OrderByDescending(w => w.Date))
        {
            string intensity = GetIntensityDisplay(workout.Intensity);
            Console.WriteLine($"{workout.Id,-10} {workout.Date:yyyy-MM-dd,-12} {Truncate(workout.Type, 20),-20} {workout.Duration,-10} {intensity,-10}");
        }

        Console.WriteLine($"\nTotal: {filtered.Count} workout(s)");
    }

    private static void ShowStats()
    {
        if (_workouts.Count == 0)
        {
            Console.WriteLine("No workouts logged yet. Start tracking!");
            return;
        }

        int totalWorkouts = _workouts.Count;
        int totalDuration = _workouts.Sum(w => w.Duration);
        int? totalCalories = _workouts.Where(w => w.Calories.HasValue).Sum(w => w.Calories);
        double avgIntensity = _workouts.Average(w => w.Intensity);

        var thisWeek = _workouts.Where(w => w.Date >= DateTime.Today.AddDays(-7)).ToList();
        var thisMonth = _workouts.Where(w => w.Date >= DateTime.Today.AddDays(-30)).ToList();

        var byType = _workouts.GroupBy(w => w.Type)
            .Select(g => new { Type = g.Key, Count = g.Count(), Duration = g.Sum(w => w.Duration) })
            .OrderByDescending(t => t.Count)
            .ToList();

        int currentStreak = CalculateStreak();
        int longestStreak = CalculateLongestStreak();

        Console.WriteLine("\n📊 Workout Statistics");
        Console.WriteLine(new string('=', 50));

        Console.WriteLine($"\n📈 Overall:");
        Console.WriteLine($"   Total workouts: {totalWorkouts}");
        Console.WriteLine($"   Total duration: {totalDuration} min ({totalDuration / 60.0:F1} hours)");
        if (totalCalories.HasValue && totalCalories > 0)
            Console.WriteLine($"   Total calories: {totalCalories:N0}");
        Console.WriteLine($"   Avg intensity: {avgIntensity:F1}/5 {GetIntensityDisplay((int)Math.Round(avgIntensity))}");

        Console.WriteLine($"\n📅 Recent Activity:");
        Console.WriteLine($"   This week: {thisWeek.Count} workouts");
        Console.WriteLine($"   This month: {thisMonth.Count} workouts");

        Console.WriteLine($"\n🔥 Streaks:");
        Console.WriteLine($"   Current streak: {currentStreak} days");
        Console.WriteLine($"   Longest streak: {longestStreak} days");

        Console.WriteLine($"\n💪 By Type:");
        foreach (var t in byType.Take(5))
        {
            Console.WriteLine($"   {t.Type}: {t.Count} workouts, {t.Duration} min");
        }
    }

    private static int CalculateStreak()
    {
        var dates = _workouts.Select(w => w.Date.Date).Distinct().OrderByDescending(d => d).ToList();
        if (dates.Count == 0) return 0;

        int streak = 1;
        for (int i = 1; i < dates.Count; i++)
        {
            if ((dates[i - 1] - dates[i]).Days == 1)
                streak++;
            else
                break;
        }
        return streak;
    }

    private static int CalculateLongestStreak()
    {
        var dates = _workouts.Select(w => w.Date.Date).Distinct().OrderBy(d => d).ToList();
        if (dates.Count == 0) return 0;

        int longest = 1, current = 1;
        for (int i = 1; i < dates.Count; i++)
        {
            if ((dates[i] - dates[i - 1]).Days == 1)
                current++;
            else
            {
                longest = Math.Max(longest, current);
                current = 1;
            }
        }
        return Math.Max(longest, current);
    }

    private static void DeleteWorkout(string? idArg)
    {
        string? id = idArg;
        if (string.IsNullOrWhiteSpace(id))
        {
            Console.Write("Workout ID to delete: ");
            id = Console.ReadLine();
        }

        var workout = _workouts.FirstOrDefault(w => w.Id == id);
        if (workout == null)
        {
            Console.WriteLine($"Workout '{id}' not found.");
            return;
        }

        Console.Write($"Delete {workout.Type} on {workout.Date:yyyy-MM-dd}? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _workouts.Remove(workout);
            SaveWorkouts();
            Console.WriteLine("✅ Workout deleted.");
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }

    private static string GetIntensityDisplay(int intensity)
    {
        return string.Concat(Enumerable.Repeat("🔥", intensity));
    }
}

class Workout
{
    public string Id { get; set; } = "";
    public DateTime Date { get; set; }
    public string Type { get; set; } = "";
    public int Duration { get; set; }
    public int? Calories { get; set; }
    public int Intensity { get; set; }
    public string Notes { get; set; } = "";
    public DateTime LoggedAt { get; set; }
}
