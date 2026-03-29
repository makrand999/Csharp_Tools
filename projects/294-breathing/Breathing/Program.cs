namespace Breathing;

class Program
{
    static void Main()
    {
        Console.WriteLine("🌬️ Breathing Exercise");
        Console.WriteLine("====================\n");

        Console.WriteLine("Select breathing pattern:");
        Console.WriteLine("  1. Box Breathing (4-4-4-4)");
        Console.WriteLine("  2. 4-7-8 Relaxation");
        Console.WriteLine("  3. Coherent Breathing (5-5)");
        Console.WriteLine("  4. Custom pattern");
        Console.Write("\nChoice: ");

        int inhale = 4, hold1 = 4, exhale = 4, hold2 = 4;

        switch (Console.ReadLine())
        {
            case "2": inhale = 4; hold1 = 7; exhale = 8; hold2 = 0; break;
            case "3": inhale = 5; hold1 = 0; exhale = 5; hold2 = 0; break;
            case "4":
                Console.Write("Inhale seconds: "); int.TryParse(Console.ReadLine(), out inhale);
                Console.Write("Hold after inhale: "); int.TryParse(Console.ReadLine(), out hold1);
                Console.Write("Exhale seconds: "); int.TryParse(Console.ReadLine(), out exhale);
                Console.Write("Hold after exhale: "); int.TryParse(Console.ReadLine(), out hold2);
                break;
        }

        Console.WriteLine($"\n🌬️ Starting breathing exercise...");
        Console.WriteLine("Follow the prompts on screen.\n");
        Console.WriteLine("Press Ctrl+C to end.\n");

        RunBreathing(inhale, hold1, exhale, hold2);
    }

    private static void RunBreathing(int inhale, int hold1, int exhale, int hold2)
    {
        int cycle = 1;
        
        while (true)
        {
            Console.WriteLine($"\n--- Cycle {cycle} ---\n");

            // Inhale
            AnimatePhase("📥 Inhale...", inhale);
            
            // Hold after inhale
            if (hold1 > 0)
                AnimatePhase("⏸️  Hold...", hold1);
            
            // Exhale
            AnimatePhase("📤 Exhale...", exhale);
            
            // Hold after exhale
            if (hold2 > 0)
                AnimatePhase("⏸️  Hold...", hold2);

            cycle++;
        }
    }

    private static void AnimatePhase(string text, int seconds)
    {
        for (int i = seconds; i > 0; i--)
        {
            Console.Write($"\r{text} {i}   ");
            Thread.Sleep(1000);
        }
        Console.WriteLine();
    }
}
