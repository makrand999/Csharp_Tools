using System.Text.Json;

var sessionsFile = "pomodoro-sessions.json";
var sessions = LoadSessions(sessionsFile);

if (args.Length == 0)
{
    StartInteractiveMode(sessions, sessionsFile);
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "start":
        await StartTimer(sessions, sessionsFile, args.Skip(1).ToArray());
        break;
    case "log":
        LogSession(sessions, args.Skip(1).ToArray());
        break;
    case "list":
        ListSessions(sessions, args.Skip(1).ToArray());
        break;
    case "stats":
        ShowStats(sessions);
        break;
    case "clear":
        ClearSessions(sessions);
        break;
    default:
        Console.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

SaveSessions(sessionsFile, sessions);

void ShowHelp()
{
    Console.WriteLine("""
        Pomodoro Timer - Focus timer with session tracking and statistics
        
        Usage:
          dotnet run --project PomodoroTimer.csproj [command] [arguments]
        
        Commands:
          start [minutes]               Start a focus timer (default: 25 min)
          log <minutes> [task]          Log a completed session manually
          list [days]                   Show recent sessions (default: 7 days)
          stats                         Show productivity statistics
          clear                         Clear all session history
          (no command)                  Interactive mode
        
        Examples:
          dotnet run -- start 25
          dotnet run -- start 45
          dotnet run -- log 30 "Read documentation"
          dotnet run -- list 14
          dotnet run -- stats
        """);
}

void StartInteractiveMode(List<Session> sessions, string sessionsFile)
{
    Console.WriteLine("""
        
        ╔═══════════════════════════════════════════╗
        ║     🍅 Pomodoro Timer - Interactive Mode  ║
        ╚═══════════════════════════════════════════╝
        
        Commands:
          start [min]  - Start a timer (default 25 min)
          log <min>    - Log a completed session
          list         - Show recent sessions
          stats        - Show statistics
          quit         - Exit the program
        
        """);

    while (true)
    {
        Console.Write("🍅 > ");
        var input = Console.ReadLine()?.Trim();
        
        if (string.IsNullOrEmpty(input) || input == "quit" || input == "exit")
            break;

        var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var cmd = parts[0].ToLower();
        var args = parts.Skip(1).ToArray();

        switch (cmd)
        {
            case "start":
                StartTimer(sessions, sessionsFile, args).Wait();
                break;
            case "log":
                LogSession(sessions, args);
                SaveSessions(sessionsFile, sessions);
                break;
            case "list":
                ListSessions(sessions, args);
                break;
            case "stats":
                ShowStats(sessions);
                break;
            default:
                Console.WriteLine($"Unknown command: {cmd}");
                break;
        }
    }
}

async Task StartTimer(List<Session> sessions, string sessionsFile, string[] args)
{
    var minutes = 25;
    var task = "";

    if (args.Length > 0 && int.TryParse(args[0], out var m))
    {
        minutes = m;
        task = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "";
    }
    else if (args.Length > 0)
    {
        task = string.Join(" ", args);
    }

    if (!string.IsNullOrEmpty(task))
        Console.WriteLine($"\n📝 Task: {task}");
    
    Console.WriteLine($"⏱️  Starting {minutes}-minute focus session...\n");
    Console.WriteLine("Press Ctrl+C to cancel.\n");

    var startTime = DateTime.Now;
    var endTime = startTime.AddMinutes(minutes);

    try
    {
        while (DateTime.Now < endTime)
        {
            var remaining = endTime - DateTime.Now;
            var minutesLeft = (int)remaining.TotalMinutes;
            var secondsLeft = remaining.Seconds;

            Console.Write($"\r🍅 {minutesLeft:D2}:{secondsLeft:D2} remaining");
            
            await Task.Delay(1000);
        }

        Console.WriteLine("\n\n🔔 Time's up! Session complete.\n");

        var session = new Session
        {
            Id = sessions.Count > 0 ? sessions.Max(s => s.Id) + 1 : 1,
            Duration = minutes,
            Task = task,
            CompletedAt = DateTime.Now
        };

        sessions.Add(session);
        SaveSessions(sessionsFile, sessions);

        Console.WriteLine($"✓ Logged {minutes}-minute session");
        if (!string.IsNullOrEmpty(task))
            Console.WriteLine($"  Task: {task}");
        
        // Show daily progress
        var today = DateTime.Now.Date;
        var todaySessions = sessions.Where(s => s.CompletedAt.Date == today).ToList();
        var todayMinutes = todaySessions.Sum(s => s.Duration);
        Console.WriteLine($"📊 Today: {todaySessions.Count} sessions, {todayMinutes} minutes\n");
    }
    catch (TaskCanceledException)
    {
        Console.WriteLine("\n\n⏸️  Timer cancelled.");
    }
}

void LogSession(List<Session> sessions, string[] args)
{
    if (args.Length == 0 || !int.TryParse(args[0], out var minutes))
    {
        Console.WriteLine("Usage: log <minutes> [task]");
        return;
    }

    var task = args.Length > 1 ? string.Join(" ", args.Skip(1)) : "";
    
    var session = new Session
    {
        Id = sessions.Count > 0 ? sessions.Max(s => s.Id) + 1 : 1,
        Duration = minutes,
        Task = task,
        CompletedAt = DateTime.Now
    };

    sessions.Add(session);
    Console.WriteLine($"✓ Logged {minutes}-minute session");
    if (!string.IsNullOrEmpty(task))
        Console.WriteLine($"  Task: {task}");
}

void ListSessions(List<Session> sessions, string[] args)
{
    var days = 7;
    if (args.Length > 0 && int.TryParse(args[0], out var d))
        days = d;

    var cutoff = DateTime.Now.Date.AddDays(-days + 1);
    var filtered = sessions.Where(s => s.CompletedAt >= cutoff)
                           .OrderByDescending(s => s.CompletedAt)
                           .ToList();

    if (filtered.Count == 0)
    {
        Console.WriteLine($"No sessions in the last {days} days.");
        return;
    }

    Console.WriteLine($"\n📅 Pomodoro Sessions (last {days} days)");
    Console.WriteLine(new string('─', 60));

    DateTime? currentDate = null;
    foreach (var session in filtered)
    {
        if (session.CompletedAt.Date != currentDate)
        {
            currentDate = session.CompletedAt.Date;
            var dayLabel = currentDate == DateTime.Now.Date ? "Today" :
                          currentDate == DateTime.Now.Date.AddDays(-1) ? "Yesterday" :
                          currentDate.Value.ToString("dddd, MMM d");
            Console.WriteLine($"\n{dayLabel}");
            Console.WriteLine(new string('─', 40));
        }

        var timeLabel = session.CompletedAt.ToString("HH:mm");
        var taskLabel = string.IsNullOrEmpty(session.Task) ? "(no task)" : session.Task;
        Console.WriteLine($"  [{session.Id}] {timeLabel} | {session.Duration} min | {taskLabel}");
    }

    var totalSessions = filtered.Count;
    var totalMinutes = filtered.Sum(s => s.Duration);
    Console.WriteLine($"\n{new string('─', 60)}");
    Console.WriteLine($"Total: {totalSessions} sessions, {totalMinutes} minutes ({totalMinutes / 60.0:F1} hours)");
    Console.WriteLine();
}

void ShowStats(List<Session> sessions)
{
    if (sessions.Count == 0)
    {
        Console.WriteLine("No sessions recorded yet.");
        return;
    }

    var today = DateTime.Now.Date;
    var thisWeekStart = today.AddDays(-(int)today.DayOfWeek);
    
    var totalSessions = sessions.Count;
    var totalMinutes = sessions.Sum(s => s.Duration);
    var firstSession = sessions.Min(s => s.CompletedAt);
    var daysActive = (today - firstSession.Date).Days + 1;

    var todaySessions = sessions.Where(s => s.CompletedAt.Date == today).ToList();
    var weekSessions = sessions.Where(s => s.CompletedAt >= thisWeekStart).ToList();

    var todayMinutes = todaySessions.Sum(s => s.Duration);
    var weekMinutes = weekSessions.Sum(s => s.Duration);

    var avgSessionLength = totalMinutes / (double)totalSessions;
    var avgPerDay = totalMinutes / (double)daysActive;

    Console.WriteLine("""
        
        ═══════════════════════════════════════════
         🍅 Pomodoro Statistics
        ═══════════════════════════════════════════
        """);

    Console.WriteLine($"  All Time:");
    Console.WriteLine($"    Total sessions:    {totalSessions}");
    Console.WriteLine($"    Total time:        {totalMinutes} minutes ({totalMinutes / 60.0:F1} hours)");
    Console.WriteLine($"    Avg session:       {avgSessionLength:F0} minutes");
    Console.WriteLine($"    Daily average:     {avgPerDay:F0} minutes");
    Console.WriteLine();

    Console.WriteLine($"  This Week:");
    Console.WriteLine($"    Sessions:          {weekSessions.Count}");
    Console.WriteLine($"    Time:              {weekMinutes} minutes ({weekMinutes / 60.0:F1} hours)");
    Console.WriteLine();

    Console.WriteLine($"  Today:");
    Console.WriteLine($"    Sessions:          {todaySessions.Count}");
    Console.WriteLine($"    Time:              {todayMinutes} minutes");
    Console.WriteLine();

    // Top tasks
    var taskGroups = sessions.Where(s => !string.IsNullOrEmpty(s.Task))
                             .GroupBy(s => s.Task)
                             .OrderByDescending(g => g.Sum(s => s.Duration))
                             .Take(5)
                             .ToList();

    if (taskGroups.Count > 0)
    {
        Console.WriteLine("  Top Tasks:");
        Console.WriteLine("  ─────────────────────────────────────");
        foreach (var task in taskGroups)
        {
            var total = task.Sum(s => s.Duration);
            var count = task.Count();
            Console.WriteLine($"  {task.Key,-25} {total,4} min ({count}x)");
        }
        Console.WriteLine();
    }

    // Weekly chart
    Console.WriteLine("  This Week:");
    Console.WriteLine("  ─────────────────────────────────────");
    var maxDayMinutes = 0;
    var dailyMinutes = new Dictionary<string, int>();
    
    for (int i = 0; i < 7; i++)
    {
        var date = thisWeekStart.AddDays(i);
        var dayMinutes = sessions.Where(s => s.CompletedAt.Date == date).Sum(s => s.Duration);
        dailyMinutes[date.ToString("ddd")] = dayMinutes;
        if (dayMinutes > maxDayMinutes) maxDayMinutes = dayMinutes;
    }

    foreach (var entry in dailyMinutes)
    {
        var barLength = maxDayMinutes > 0 ? entry.Value * 30 / maxDayMinutes : 0;
        var bar = new string('█', barLength);
        Console.WriteLine($"  {entry.Key}: {bar,-30} {entry.Value} min");
    }
    Console.WriteLine();
}

void ClearSessions(List<Session> sessions)
{
    Console.Write("Are you sure you want to clear all session history? (y/N): ");
    var response = Console.ReadLine()?.Trim().ToLower();
    
    if (response == "y" || response == "yes")
    {
        sessions.Clear();
        Console.WriteLine("✓ All session history cleared.");
    }
    else
    {
        Console.WriteLine("Cancelled.");
    }
}

List<Session> LoadSessions(string path)
{
    if (!File.Exists(path))
        return new List<Session>();
    
    try
    {
        var json = File.ReadAllText(path);
        var sessions = JsonSerializer.Deserialize<List<Session>>(json);
        return sessions ?? new List<Session>();
    }
    catch (JsonException)
    {
        Console.WriteLine("Warning: Could not parse pomodoro-sessions.json, starting fresh.");
        return new List<Session>();
    }
}

void SaveSessions(string path, List<Session> sessions)
{
    var options = new JsonSerializerOptions { WriteIndented = true };
    var json = JsonSerializer.Serialize(sessions, options);
    File.WriteAllText(path, json);
}

class Session
{
    public int Id { get; set; }
    public int Duration { get; set; }
    public string Task { get; set; } = "";
    public DateTime CompletedAt { get; set; }
}
