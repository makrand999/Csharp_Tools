namespace SleepTimer;

class Program
{
    static void Main()
    {
        Console.WriteLine("🌙 Sleep Timer");
        Console.WriteLine("==============\n");

        Console.WriteLine("Set sleep timer:");
        Console.WriteLine("  1. 15 minutes");
        Console.WriteLine("  2. 30 minutes");
        Console.WriteLine("  3. 45 minutes");
        Console.WriteLine("  4. 1 hour");
        Console.WriteLine("  5. Custom duration");
        Console.Write("\nChoice: ");

        int minutes = Console.ReadLine() switch
        {
            "1" => 15,
            "2" => 30,
            "3" => 45,
            "4" => 60,
            "5" => int.TryParse(Console.ReadLine(), out var m) ? m : 30,
            _ => 30
        };

        Console.WriteLine($"\n🌙 Sleep timer set for {minutes} minutes.");
        Console.WriteLine("The program will beep when time is up.\n");
        Console.WriteLine("Sweet dreams! 💤\n");
        Console.WriteLine("Press Ctrl+C to cancel.\n");

        int totalSeconds = minutes * 60;
        for (int i = totalSeconds; i > 0; i--)
        {
            var remaining = TimeSpan.FromSeconds(i);
            
            // Show time every 10 seconds
            if (i % 10 == 0 || i <= 10)
            {
                Console.Write($"\r⏰ {remaining.Minutes:00}:{remaining.Seconds:00} remaining   ");
            }

            Thread.Sleep(1000);
        }

        // Alarm
        Console.WriteLine("\n\n🔔 Time's up! Good morning!");
        for (int i = 0; i < 5; i++)
        {
            Console.Beep(880, 200);
            Thread.Sleep(300);
        }
    }
}
