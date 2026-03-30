namespace HydrationReminder;

class Program
{
    static void Main()
    {
        Console.WriteLine("💧 Hydration Reminder");
        Console.WriteLine("====================\n");

        Console.WriteLine("Set reminder interval:");
        Console.WriteLine("  1. Every 30 minutes");
        Console.WriteLine("  2. Every hour");
        Console.WriteLine("  3. Every 2 hours");
        Console.WriteLine("  4. Custom interval");
        Console.Write("\nChoice: ");

        int minutes = Console.ReadLine() switch
        {
            "1" => 30,
            "2" => 60,
            "3" => 120,
            "4" => int.TryParse(Console.ReadLine(), out var m) ? m : 60,
            _ => 60
        };

        Console.WriteLine($"\n💧 Hydration reminders set for every {minutes} minutes.");
        Console.WriteLine("Stay hydrated! 💧\n");
        Console.WriteLine("Press Ctrl+C to stop.\n");

        int reminderCount = 0;
        var wakeHours = TimeSpan.FromHours(16); // Assume 16 waking hours
        var maxReminders = (int)(wakeHours.TotalMinutes / minutes);

        while (reminderCount < maxReminders)
        {
            reminderCount++;
            
            Console.WriteLine($"\n💧 Reminder #{reminderCount}: Time to drink water!");
            Console.WriteLine($"   [{DateTime.Now:HH:mm:ss}]");
            Console.WriteLine($"   Remaining reminders today: {maxReminders - reminderCount}");
            
            // Countdown to next reminder
            for (int i = minutes * 60; i > 0; i -= 60)
            {
                var remaining = TimeSpan.FromSeconds(i);
                if (i % 300 == 0 || i <= 60) // Show every 5 min or last minute
                {
                    Console.Write($"\r   Next reminder in: {remaining.Hours:00}:{remaining.Minutes:00}   ");
                }
                Thread.Sleep(60000); // Check every minute
            }
            Console.WriteLine();
        }

        Console.WriteLine("\n✅ Daily hydration goal completed!");
        Console.WriteLine("Great job staying hydrated today!");
    }
}
