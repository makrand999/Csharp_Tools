namespace Meditation;

class Program
{
    static void Main()
    {
        Console.WriteLine("🧘 Meditation Timer");
        Console.WriteLine("===================\n");

        Console.WriteLine("Select meditation type:");
        Console.WriteLine("  1. Quick Calm (3 min)");
        Console.WriteLine("  2. Mindfulness (10 min)");
        Console.WriteLine("  3. Deep Meditation (20 min)");
        Console.WriteLine("  4. Custom duration");
        Console.Write("\nChoice: ");

        int minutes = Console.ReadLine() switch
        {
            "1" => 3,
            "2" => 10,
            "3" => 20,
            "4" => int.TryParse(Console.ReadLine(), out var m) ? m : 10,
            _ => 10
        };

        Console.WriteLine($"\n🧘 Starting {minutes}-minute meditation...");
        Console.WriteLine("Find a comfortable position and close your eyes.\n");
        Console.WriteLine("Press Ctrl+C to end early.\n");

        RunMeditation(minutes);

        Console.WriteLine("\n🔔 Meditation complete!");
        Console.WriteLine("Take a moment before opening your eyes.\n");
    }

    private static void RunMeditation(int minutes)
    {
        int totalSeconds = minutes * 60;
        var startTime = DateTime.Now;

        // Opening bell
        PlayBell();

        while (true)
        {
            var elapsed = DateTime.Now - startTime;
            var remaining = TimeSpan.FromSeconds(totalSeconds) - elapsed;

            if (remaining.TotalSeconds <= 0)
                break;

            Console.Write($"\r⏱️  {remaining.Minutes:00}:{remaining.Seconds:00} remaining   ");

            // Gentle bell every 5 minutes
            if ((int)elapsed.TotalSeconds > 0 && (int)elapsed.TotalSeconds % 300 == 0)
                PlayBell();

            Thread.Sleep(1000);
        }

        // Closing bells
        Thread.Sleep(500);
        PlayBell();
        Thread.Sleep(500);
        PlayBell();
    }

    private static void PlayBell()
    {
        Console.Beep(440, 500); // A4 note
    }
}
