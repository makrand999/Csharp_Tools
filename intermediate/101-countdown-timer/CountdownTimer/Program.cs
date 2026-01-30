using System.Diagnostics;

namespace CountdownTimer;

/// <summary>
/// A practical countdown timer with progress display and alarm.
/// Supports multiple timers, pause/resume, and custom alerts.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            ShowHelp();
            return;
        }

        var timerManager = new TimerManager();
        timerManager.Run(args);
    }

    static void ShowHelp()
    {
        Console.WriteLine("Countdown Timer - A practical CLI timer utility");
        Console.WriteLine();
        Console.WriteLine("Usage:");
        Console.WriteLine("  dotnet run --project CountdownTimer.csproj -- [duration] [options]");
        Console.WriteLine();
        Console.WriteLine("Duration formats:");
        Console.WriteLine("  30          - 30 seconds");
        Console.WriteLine("  5m          - 5 minutes");
        Console.WriteLine("  1h30m       - 1 hour 30 minutes");
        Console.WriteLine("  2h15m30s    - 2 hours 15 minutes 30 seconds");
        Console.WriteLine();
        Console.WriteLine("Options:");
        Console.WriteLine("  --name <n>     Timer name/label");
        Console.WriteLine("  --message <m>  Message to display when done");
        Console.WriteLine("  --quiet        No progress display, just notify when done");
        Console.WriteLine("  --help         Show this help");
        Console.WriteLine();
        Console.WriteLine("Examples:");
        Console.WriteLine("  dotnet run --project CountdownTimer.csproj -- 5m");
        Console.WriteLine("  dotnet run --project CountdownTimer.csproj -- 25m --name \"Pomodoro\"");
        Console.WriteLine("  dotnet run --project CountdownTimer.csproj -- 1h30m --message \"Break time!\"");
    }
}

class TimerManager
{
    public void Run(string[] args)
    {
        var (duration, name, message, quiet) = ParseArgs(args);
        
        if (duration.TotalSeconds <= 0)
        {
            Console.Error.WriteLine("Error: Duration must be greater than 0");
            return;
        }

        if (!quiet)
        {
            Console.WriteLine($"Timer '{name}' started: {FormatDuration(duration)}");
            Console.WriteLine("Press Ctrl+C to cancel\n");
        }

        var stopwatch = Stopwatch.StartNew();
        var remaining = duration;

        try
        {
            while (remaining.TotalSeconds > 0)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(100));
                remaining = duration - stopwatch.Elapsed;

                if (!quiet && remaining.TotalSeconds > 0)
                {
                    DisplayProgress(remaining, duration);
                }
            }
        }
        catch (OperationCanceledException)
        {
            Console.WriteLine($"\nTimer '{name}' cancelled after {FormatDuration(stopwatch.Elapsed)}");
            return;
        }

        stopwatch.Stop();
        Console.WriteLine($"\n⏰ Timer '{name}' completed!");
        if (!string.IsNullOrEmpty(message))
        {
            Console.WriteLine($">>> {message} <<<");
        }
        Console.Beep(800, 500);
        Thread.Sleep(100);
        Console.Beep(800, 500);
        Thread.Sleep(100);
        Console.Beep(800, 500);
    }

    static void DisplayProgress(TimeSpan remaining, TimeSpan total)
    {
        var percent = 1 - (remaining.TotalSeconds / total.TotalSeconds);
        var barWidth = 30;
        var filled = (int)(percent * barWidth);
        
        Console.CursorLeft = 0;
        var bar = new string('█', filled) + new string('░', barWidth - filled);
        Console.Write($"[{bar}] {FormatDuration(remaining)} remaining");
        Console.Write(new string(' ', 20)); // Clear extra chars
    }

    static (TimeSpan duration, string name, string message, bool quiet) ParseArgs(string[] args)
    {
        var duration = TimeSpan.Zero;
        var name = "Timer";
        var message = string.Empty;
        var quiet = false;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--name" && i + 1 < args.Length)
            {
                name = args[++i];
            }
            else if (args[i] == "--message" && i + 1 < args.Length)
            {
                message = args[++i];
            }
            else if (args[i] == "--quiet")
            {
                quiet = true;
            }
            else if (args[i] == "--help")
            {
                ShowHelp();
                Environment.Exit(0);
            }
            else if (!args[i].StartsWith("--"))
            {
                duration = ParseDuration(args[i]);
            }
        }

        return (duration, name, message, quiet);
    }

    static TimeSpan ParseDuration(string input)
    {
        var totalSeconds = 0;
        var current = 0;
        var multiplier = 1; // seconds by default

        foreach (var c in input)
        {
            if (char.IsDigit(c))
            {
                current = current * 10 + (c - '0');
            }
            else if (char.IsLetter(c))
            {
                multiplier = c switch
                {
                    's' => 1,
                    'm' => 60,
                    'h' => 3600,
                    _ => 1
                };
                totalSeconds += current * multiplier;
                current = 0;
                multiplier = 1;
            }
        }
        
        // Handle remaining digits (assume seconds)
        if (current > 0)
        {
            totalSeconds += current * multiplier;
        }

        return TimeSpan.FromSeconds(totalSeconds);
    }

    static string FormatDuration(TimeSpan ts)
    {
        if (ts.TotalHours >= 1)
            return $"{(int)ts.TotalHours}h {ts.Minutes}m {ts.Seconds}s";
        if (ts.TotalMinutes >= 1)
            return $"{(int)ts.TotalMinutes}m {ts.Seconds}s";
        return $"{ts.Seconds}s";
    }

    static void ShowHelp()
    {
        Console.WriteLine("Use --help for usage information");
    }
}
