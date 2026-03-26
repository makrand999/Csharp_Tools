namespace TeamGenerator;

class Program
{
    private static readonly Random Random = new();

    static void Main()
    {
        Console.WriteLine("👥 Team Generator");
        Console.WriteLine("=================\n");

        Console.WriteLine("Enter participant names (one per line, empty line to finish):");

        var participants = new List<string>();
        string? line;

        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            participants.Add(line.Trim());
        }

        if (participants.Count < 2)
        {
            Console.WriteLine("\n⚠️  Need at least 2 participants!");
            return;
        }

        Console.WriteLine($"\n{participants.Count} participants registered.\n");

        Console.WriteLine("Team creation method:");
        Console.WriteLine("  1. Random assignment");
        Console.WriteLine("  2. Specify team size");
        Console.WriteLine("  3. Specify number of teams");
        Console.Write("\nChoose (1-3): ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "2":
                Console.Write("\nDesired team size: ");
                if (int.TryParse(Console.ReadLine(), out int size) && size > 0)
                {
                    GenerateBySize(participants, size);
                }
                else
                {
                    Console.WriteLine("Invalid size, using random.");
                    GenerateRandom(participants);
                }
                break;
            case "3":
                Console.Write("\nNumber of teams: ");
                if (int.TryParse(Console.ReadLine(), out int count) && count > 0)
                {
                    GenerateByCount(participants, count);
                }
                else
                {
                    Console.WriteLine("Invalid count, using random.");
                    GenerateRandom(participants);
                }
                break;
            default:
                GenerateRandom(participants);
                break;
        }
    }

    private static void GenerateRandom(List<string> participants)
    {
        int teamCount = Math.Max(2, (int)Math.Ceiling(Math.Sqrt(participants.Count)));
        GenerateByCount(participants, teamCount);
    }

    private static void GenerateBySize(List<string> participants, int teamSize)
    {
        var shuffled = participants.OrderBy(_ => Random.Next()).ToList();
        var teams = new List<List<string>>();

        for (int i = 0; i < shuffled.Count; i += teamSize)
        {
            var team = shuffled.Skip(i).Take(teamSize).ToList();
            teams.Add(team);
        }

        DisplayTeams(teams);
    }

    private static void GenerateByCount(List<string> participants, int teamCount)
    {
        var shuffled = participants.OrderBy(_ => Random.Next()).ToList();
        var teams = new List<List<string>>();

        for (int i = 0; i < teamCount; i++)
            teams.Add(new List<string>());

        for (int i = 0; i < shuffled.Count; i++)
        {
            teams[i % teamCount].Add(shuffled[i]);
        }

        DisplayTeams(teams);
    }

    private static void DisplayTeams(List<List<string>> teams)
    {
        Console.WriteLine("\n" + new string('=', 40));
        Console.WriteLine("\n📋 Team Assignments\n");
        Console.WriteLine(new string('=', 40));

        for (int i = 0; i < teams.Count; i++)
        {
            Console.WriteLine($"\n🔵 Team {i + 1} ({teams[i].Count} members)");
            Console.WriteLine(new string('-', 30));
            
            foreach (var member in teams[i])
            {
                Console.WriteLine($"   • {member}");
            }
        }

        Console.WriteLine($"\n{new string('=', 40)}");
        Console.WriteLine($"Total: {teams.Count} teams, {teams.Sum(t => t.Count)} participants");

        // Save to file option
        Console.WriteLine("\n💾 Save to file? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            string filename = $"teams_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
            var content = new System.Text.StringBuilder();
            content.AppendLine("Team Assignments");
            content.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
            content.AppendLine(new string('=', 40));

            for (int i = 0; i < teams.Count; i++)
            {
                content.AppendLine($"\nTeam {i + 1} ({teams[i].Count} members)");
                content.AppendLine(new string('-', 30));
                foreach (var member in teams[i])
                {
                    content.AppendLine($"   • {member}");
                }
            }

            File.WriteAllText(filename, content.ToString());
            Console.WriteLine($"✅ Saved to: {filename}");
        }
    }
}
