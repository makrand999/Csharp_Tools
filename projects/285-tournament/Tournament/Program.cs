namespace Tournament;

class Program
{
    private static readonly Random Random = new();

    static void Main()
    {
        Console.WriteLine("🏆 Tournament Bracket Generator");
        Console.WriteLine("================================\n");

        Console.WriteLine("Enter participants (one per line, empty line to finish):");
        Console.WriteLine("(Best if participant count is power of 2: 2, 4, 8, 16...)\n");

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

        // Pad to power of 2 with BYEs
        int size = 2;
        while (size < participants.Count) size *= 2;

        while (participants.Count < size)
        {
            participants.Add("BYE");
        }

        // Shuffle for random seeding
        var seeded = participants.OrderBy(_ => Random.Next()).ToList();

        Console.WriteLine($"\n📊 Tournament Bracket ({seeded.Count} participants)\n");
        Console.WriteLine(new string('=', 60));

        var bracket = new List<Match>();
        int round = 1;
        var currentRound = seeded.Select((p, i) => new Match
        {
            Player1 = p,
            Player2 = seeded[seeded.Count - 1 - i],
            Round = round
        }).Take(seeded.Count / 2).ToList();

        while (currentRound.Count > 0)
        {
            Console.WriteLine($"\n📍 Round {round}\n");

            var nextRound = new List<Match>();
            int matchNum = 1;

            foreach (var match in currentRound)
            {
                if (match.Player1 == "BYE" && match.Player2 == "BYE")
                {
                    Console.WriteLine($"  Match {matchNum}: BYE vs BYE (TBD)");
                }
                else if (match.Player2 == "BYE")
                {
                    Console.WriteLine($"  Match {matchNum}: {match.Player1} → BYE (Auto-advance)");
                    nextRound.Add(new Match { Player1 = match.Player1, Player2 = "", Round = round + 1 });
                }
                else if (match.Player1 == "BYE")
                {
                    Console.WriteLine($"  Match {matchNum}: BYE vs {match.Player2} (Auto-advance)");
                    nextRound.Add(new Match { Player1 = match.Player2, Player2 = "", Round = round + 1 });
                }
                else
                {
                    Console.WriteLine($"  Match {matchNum}: {match.Player1} vs {match.Player2}");
                }
                matchNum++;
            }

            bracket.AddRange(currentRound);

            if (nextRound.Count <= 1)
            {
                if (nextRound.Count == 1)
                {
                    Console.WriteLine($"\n🏅 FINAL: {nextRound[0].Player1} vs {nextRound[0].Player2}");
                    Console.WriteLine($"\n🏆 Champion: {SimulateWinner(nextRound[0])}");
                }
                break;
            }

            // Simulate winners for next round
            var simulatedNext = new List<Match>();
            for (int i = 0; i < nextRound.Count; i += 2)
            {
                var m = nextRound[i];
                if (i + 1 < nextRound.Count)
                {
                    var m2 = nextRound[i + 1];
                    string winner1 = string.IsNullOrEmpty(m.Player2) ? m.Player1 : SimulateWinner(m);
                    string winner2 = string.IsNullOrEmpty(m2.Player2) ? m2.Player1 : SimulateWinner(m2);
                    simulatedNext.Add(new Match { Player1 = winner1, Player2 = winner2, Round = round + 1 });
                }
                else
                {
                    simulatedNext.Add(m);
                }
            }

            currentRound = simulatedNext;
            round++;
            Console.WriteLine(new string('-', 60));
        }

        Console.WriteLine($"\n{new string('=', 60)}");
        Console.WriteLine($"Total rounds: {round - 1}");
        Console.WriteLine($"Total matches: {size - 1}");
    }

    private static string SimulateWinner(Match match)
    {
        // 50/50 chance, but favor non-BYE players
        if (match.Player1 == "BYE") return match.Player2;
        if (match.Player2 == "BYE") return match.Player1;
        return Random.Next(2) == 0 ? match.Player1 : match.Player2;
    }
}

class Match
{
    public string Player1 { get; set; } = "";
    public string Player2 { get; set; } = "";
    public int Round { get; set; }
}
