using System.Text.Json;

var dataFile = "habits.json";
var habits = LoadHabits(dataFile);

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "add":
        AddHabit(habits, args.Skip(1).ToArray());
        break;
    case "list":
        ListHabits(habits);
        break;
    case "check":
        CheckHabit(habits, args.Skip(1).ToArray());
        break;
    case "uncheck":
        UncheckHabit(habits, args.Skip(1).ToArray());
        break;
    case "history":
        ShowHistory(habits, args.Skip(1).ToArray());
        break;
    case "delete":
        DeleteHabit(habits, args.Skip(1).ToArray());
        break;
    case "stats":
        ShowStats(habits);
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

SaveHabits(dataFile, habits);

void ShowHelp()
{
    Console.WriteLine("""
        Habit Tracker - Build and track daily habits with streaks
        
        Usage:
          dotnet run --project HabitTracker.csproj <command> [arguments]
        
        Commands:
          add <name> [frequency]          Add a new habit (daily, weekly, weekdays)
          list                            List all habits with current streaks
          check <id> [date]               Mark a habit as complete
          uncheck <id> [date]             Remove completion for a date
          history <id> [days]             Show completion history (default: 7 days)
          delete <id>                     Remove a habit
          stats                           Show overall statistics
        
        Examples:
          dotnet run -- add "Drink 8 glasses of water" daily
          dotnet run -- add "Exercise" weekdays
          dotnet run -- list
          dotnet run -- check 1
          dotnet run -- check 2 2026-03-30
          dotnet run -- history 1 14
          dotnet run -- stats
        """);
}

void AddHabit(List<Habit> habits, string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: add <name> [frequency]");
        Console.WriteLine("Frequencies: daily (default), weekly, weekdays");
        return;
    }

    var name = args[0];
    var frequency = args.Length > 1 ? args[1].ToLower() : "daily";
    var validFrequencies = new[] { "daily", "weekly", "weekdays" };
    
    if (!validFrequencies.Contains(frequency))
    {
        Console.WriteLine($"Invalid frequency. Valid options: {string.Join(", ", validFrequencies)}");
        return;
    }

    var habit = new Habit
    {
        Id = habits.Count > 0 ? habits.Max(h => h.Id) + 1 : 1,
        Name = name,
        Frequency = frequency,
        CreatedAt = DateTime.Now,
        Completions = new List<string>()
    };

    habits.Add(habit);
    Console.WriteLine($"✓ Added habit: \"{name}\" ({frequency})");
    Console.WriteLine($"  ID: {habit.Id} | Created: {habit.CreatedAt:yyyy-MM-dd}");
}

void ListHabits(List<Habit> habits)
{
    if (habits.Count == 0)
    {
        Console.WriteLine("No habits tracked yet. Add one with 'add <name>'");
        return;
    }

    var today = DateTime.Now.Date;
    Console.WriteLine($"\n📊 Habits for {today:dddd, MMMM d, yyyy}");
    Console.WriteLine(new string('═', 60));

    foreach (var habit in habits.OrderBy(h => h.Name))
    {
        var streak = CalculateStreak(habit);
        var completedToday = habit.Completions.Contains(today.ToString("yyyy-MM-dd"));
        var status = completedToday ? "✅" : "⬜";
        
        Console.WriteLine($"\n{status} [{habit.Id}] {habit.Name}");
        Console.WriteLine($"    Frequency: {habit.Frequency} | 🔥 Streak: {streak} days");
        
        // Show last 7 days
        Console.Write("    ");
        for (int i = 6; i >= 0; i--)
        {
            var date = today.AddDays(-i);
            var dateStr = date.ToString("yyyy-MM-dd");
            var dayName = i == 0 ? "T" : i == 1 ? "Y" : date.ToString("ddd")[0].ToString();
            var done = habit.Completions.Contains(dateStr);
            Console.Write($"{(done ? "🟢" : "⚪")}{dayName} ");
        }
        Console.WriteLine();
    }
    Console.WriteLine();
}

void CheckHabit(List<Habit> habits, string[] args)
{
    if (args.Length == 0 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: check <id> [date]");
        return;
    }

    var habit = habits.FirstOrDefault(h => h.Id == id);
    if (habit == null)
    {
        Console.WriteLine($"Habit #{id} not found.");
        return;
    }

    var date = args.Length > 1 && DateTime.TryParse(args[1], out var parsedDate) 
        ? parsedDate.Date 
        : DateTime.Now.Date;

    var dateStr = date.ToString("yyyy-MM-dd");
    if (habit.Completions.Contains(dateStr))
    {
        Console.WriteLine($"✓ Habit \"{habit.Name}\" already checked for {date:yyyy-MM-dd}");
        return;
    }

    habit.Completions.Add(dateStr);
    var streak = CalculateStreak(habit);
    Console.WriteLine($"✓ Checked: \"{habit.Name}\" for {date:yyyy-MM-dd}");
    Console.WriteLine($"  🔥 Current streak: {streak} days");
}

void UncheckHabit(List<Habit> habits, string[] args)
{
    if (args.Length == 0 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: uncheck <id> [date]");
        return;
    }

    var habit = habits.FirstOrDefault(h => h.Id == id);
    if (habit == null)
    {
        Console.WriteLine($"Habit #{id} not found.");
        return;
    }

    var date = args.Length > 1 && DateTime.TryParse(args[1], out var parsedDate) 
        ? parsedDate.Date 
        : DateTime.Now.Date;

    var dateStr = date.ToString("yyyy-MM-dd");
    if (!habit.Completions.Contains(dateStr))
    {
        Console.WriteLine($"Habit \"{habit.Name}\" not checked for {date:yyyy-MM-dd}");
        return;
    }

    habit.Completions.Remove(dateStr);
    Console.WriteLine($"✓ Unchecked: \"{habit.Name}\" for {date:yyyy-MM-dd}");
}

void ShowHistory(List<Habit> habits, string[] args)
{
    if (args.Length == 0 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: history <id> [days]");
        return;
    }

    var habit = habits.FirstOrDefault(h => h.Id == id);
    if (habit == null)
    {
        Console.WriteLine($"Habit #{id} not found.");
        return;
    }

    var days = args.Length > 1 && int.TryParse(args[1], out var d) ? d : 7;
    var today = DateTime.Now.Date;

    Console.WriteLine($"\n📅 History: {habit.Name} (last {days} days)");
    Console.WriteLine(new string('─', 50));

    var completed = 0;
    for (int i = days - 1; i >= 0; i--)
    {
        var date = today.AddDays(-i);
        var dateStr = date.ToString("yyyy-MM-dd");
        var isDone = habit.Completions.Contains(dateStr);
        
        if (isDone) completed++;
        
        var symbol = isDone ? "✅" : "❌";
        var dayName = date.ToString("ddd");
        Console.WriteLine($"{symbol} {date:yyyy-MM-dd} ({dayName})");
    }

    var rate = (double)completed / days * 100;
    Console.WriteLine(new string('─', 50));
    Console.WriteLine($"Completed: {completed}/{days} ({rate:F1}%)");
    Console.WriteLine();
}

void DeleteHabit(List<Habit> habits, string[] args)
{
    if (args.Length == 0 || !int.TryParse(args[0], out var id))
    {
        Console.WriteLine("Usage: delete <id>");
        return;
    }

    var habit = habits.FirstOrDefault(h => h.Id == id);
    if (habit == null)
    {
        Console.WriteLine($"Habit #{id} not found.");
        return;
    }

    habits.Remove(habit);
    Console.WriteLine($"✓ Deleted habit: \"{habit.Name}\"");
}

void ShowStats(List<Habit> habits)
{
    if (habits.Count == 0)
    {
        Console.WriteLine("No habits tracked yet.");
        return;
    }

    var today = DateTime.Now.Date;
    var totalHabits = habits.Count;
    var completedToday = habits.Count(h => h.Completions.Contains(today.ToString("yyyy-MM-dd")));
    var completionRate = (double)completedToday / totalHabits * 100;

    var bestStreakHabit = habits.MaxBy(CalculateStreak);
    var bestStreak = bestStreakHabit != null ? CalculateStreak(bestStreakHabit) : 0;

    var totalCompletions = habits.Sum(h => h.Completions.Count);
    var avgCompletionsPerHabit = (double)totalCompletions / totalHabits;

    Console.WriteLine("""
        
        ═══════════════════════════════════════════
         Habit Statistics
        ═══════════════════════════════════════════
        """);

    Console.WriteLine($"  Total habits:        {totalHabits}");
    Console.WriteLine($"  Completed today:     {completedToday}/{totalHabits} ({completionRate:F1}%)");
    Console.WriteLine($"  Best streak:         {bestStreak} days ({bestStreakHabit?.Name ?? "N/A"})");
    Console.WriteLine($"  Total completions:   {totalCompletions}");
    Console.WriteLine($"  Avg per habit:       {avgCompletionsPerHabit:F1}");

    // Weekly summary
    Console.WriteLine("\n  This Week:");
    Console.WriteLine("  ─────────────────────────────────────");
    
    var weekStart = today.AddDays(-(int)today.DayOfWeek);
    for (int i = 0; i < 7; i++)
    {
        var date = weekStart.AddDays(i);
        var dateStr = date.ToString("yyyy-MM-dd");
        var done = habits.Count(h => h.Completions.Contains(dateStr));
        var bar = new string('█', done);
        var empty = new string('░', totalHabits - done);
        Console.WriteLine($"  {date:ddd}: {bar}{empty} {done}/{totalHabits}");
    }
    
    Console.WriteLine();
}

int CalculateStreak(Habit habit)
{
    if (habit.Completions.Count == 0)
        return 0;

    var today = DateTime.Now.Date;
    var streak = 0;
    
    // Check if completed today or yesterday (streak still active)
    var todayStr = today.ToString("yyyy-MM-dd");
    var yesterdayStr = today.AddDays(-1).ToString("yyyy-MM-dd");
    
    if (!habit.Completions.Contains(todayStr) && !habit.Completions.Contains(yesterdayStr))
        return 0;

    // Count consecutive days
    for (int i = 0; i < 365; i++)
    {
        var date = today.AddDays(-i);
        var dateStr = date.ToString("yyyy-MM-dd");
        
        // Skip weekends for weekday habits
        if (habit.Frequency == "weekdays" && (date.DayOfWeek == DayOfWeek.Saturday || date.DayOfWeek == DayOfWeek.Sunday))
            continue;
        
        // Skip 6 days for weekly habits (only need 1 per week)
        if (habit.Frequency == "weekly")
        {
            var weekStart = date.AddDays(-(int)date.DayOfWeek);
            var weekCompletions = habit.Completions
                .Where(d => DateTime.TryParse(d, out var cd) && 
                           cd >= weekStart && cd < weekStart.AddDays(7))
                .Count();
            if (weekCompletions > 0)
                streak++;
            else if (i > 0)
                break;
            continue;
        }
        
        if (habit.Completions.Contains(dateStr))
            streak++;
        else if (i > 0 && habit.Frequency == "daily")
            break;
    }
    
    return streak;
}

List<Habit> LoadHabits(string path)
{
    if (!File.Exists(path))
        return new List<Habit>();
    
    try
    {
        var json = File.ReadAllText(path);
        var habits = JsonSerializer.Deserialize<List<Habit>>(json);
        return habits ?? new List<Habit>();
    }
    catch (JsonException)
    {
        Console.WriteLine("Warning: Could not parse habits.json, starting fresh.");
        return new List<Habit>();
    }
}

void SaveHabits(string path, List<Habit> habits)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(habits, options);
    File.WriteAllText(path, json);
}

class Habit
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Frequency { get; set; } = "daily";
    public DateTime CreatedAt { get; set; }
    public List<string> Completions { get; set; } = new();
}
