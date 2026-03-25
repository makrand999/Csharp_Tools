namespace NamePicker;

class Program
{
    private static readonly Random Random = new();

    static void Main(string[] args)
    {
        Console.WriteLine("🎯 Random Name Picker");
        Console.WriteLine("=====================\n");

        if (args.Length > 0)
        {
            PickFromArgs(args);
            return;
        }

        Console.WriteLine("Modes:");
        Console.WriteLine("  1. Pick winner from list");
        Console.WriteLine("  2. Random team assignment");
        Console.WriteLine("  3. Secret Santa generator");
        Console.Write("\nChoose mode (1-3): ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1": PickWinner(); break;
            case "2": TeamAssignment(); break;
            case "3": SecretSanta(); break;
            default: Console.WriteLine("Invalid choice!"); break;
        }
    }

    private static void PickFromArgs(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("Please provide at least 2 names!");
            return;
        }

        Console.WriteLine("\n🎲 Selecting winner...");
        Thread.Sleep(500);

        // Animate
        var names = args.ToList();
        for (int i = 0; i < 15; i++)
        {
            Console.Write($"\r   {names[Random.Next(names.Count)]}   ");
            Thread.Sleep(100);
        }

        string winner = names[Random.Next(names.Count)];
        Console.WriteLine($"\n\n🏆 Winner: {winner}!");
    }

    private static void PickWinner()
    {
        Console.WriteLine("\nEnter names (one per line, empty line to finish):");

        var names = new List<string>();
        string? line;

        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            names.Add(line.Trim());
        }

        if (names.Count < 2)
        {
            Console.WriteLine("Need at least 2 names!");
            return;
        }

        Console.WriteLine("\n🎲 Picking winner...");
        Thread.Sleep(1000);

        // Build suspense
        var shuffled = names.OrderBy(_ => Random.Next()).ToList();
        foreach (var name in shuffled.Take(3))
        {
            Console.WriteLine($"   {name}...");
            Thread.Sleep(500);
        }

        string winner = shuffled[0];
        Console.WriteLine($"\n🏆 WINNER: {winner}!");
        Console.WriteLine($"🎉 Congratulations!\n");
    }

    private static void TeamAssignment()
    {
        Console.WriteLine("\nEnter names (one per line, empty line to finish):");

        var names = new List<string>();
        string? line;

        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            names.Add(line.Trim());
        }

        if (names.Count < 2)
        {
            Console.WriteLine("Need at least 2 names!");
            return;
        }

        Console.Write("\nNumber of teams (default 2): ");
        string? teamInput = Console.ReadLine();
        int teamCount = int.TryParse(teamInput, out int t) && t > 0 ? t : 2;

        var teams = new List<List<string>>();
        for (int i = 0; i < teamCount; i++)
            teams.Add(new List<string>());

        // Shuffle and distribute
        var shuffled = names.OrderBy(_ => Random.Next()).ToList();
        for (int i = 0; i < shuffled.Count; i++)
        {
            teams[i % teamCount].Add(shuffled[i]);
        }

        Console.WriteLine("\n=== Team Assignments ===\n");
        for (int i = 0; i < teams.Count; i++)
        {
            Console.WriteLine($"Team {i + 1} ({teams[i].Count} members):");
            foreach (var name in teams[i])
            {
                Console.WriteLine($"  • {name}");
            }
            Console.WriteLine();
        }
    }

    private static void SecretSanta()
    {
        Console.WriteLine("\nEnter participants (one per line, empty line to finish):");

        var participants = new List<string>();
        string? line;

        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            participants.Add(line.Trim());
        }

        if (participants.Count < 3)
        {
            Console.WriteLine("Need at least 3 participants!");
            return;
        }

        List<string> receivers;
        do
        {
            receivers = participants.OrderBy(_ => Random.Next()).ToList();
        } while (IsInvalidAssignment(participants, receivers));

        Console.WriteLine("\n=== Secret Santa Assignments ===\n");
        for (int i = 0; i < participants.Count; i++)
        {
            Console.WriteLine($"{participants[i]} → {receivers[i]}");
        }

        Console.WriteLine("\n💡 Tip: Screenshot this or save to file!");
    }

    private static bool IsInvalidAssignment(List<string> givers, List<string> receivers)
    {
        for (int i = 0; i < givers.Count; i++)
        {
            if (givers[i] == receivers[i])
                return true;
        }
        return false;
    }
}
