namespace Voting;

class Program
{
    private static readonly Dictionary<string, int> Votes = new();
    private static readonly HashSet<string> Voters = new();

    static void Main()
    {
        Console.WriteLine("🗳️  Voting System");
        Console.WriteLine("=================\n");

        Console.WriteLine("Enter poll options (one per line, empty line to finish):\n");

        var options = new List<string>();
        string? line;

        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            options.Add(line.Trim());
            Votes[line.Trim()] = 0;
        }

        if (options.Count < 2)
        {
            Console.WriteLine("\n⚠️  Need at least 2 options!");
            return;
        }

        Console.WriteLine($"\n✅ Poll created with {options.Count} options\n");
        Console.WriteLine("Voting modes:");
        Console.WriteLine("  1. Open voting (anyone can vote)");
        Console.WriteLine("  2. One vote per person (named)");
        Console.Write("\nChoose mode (1-2): ");

        bool requireNames = Console.ReadLine() == "2";

        Console.WriteLine("\n--- Voting Started ---");
        Console.WriteLine("Enter votes (format: option or 'name:option' for mode 2)");
        Console.WriteLine("Type 'end' to finish voting\n");

        while (true)
        {
            Console.Write("Vote: ");
            string? input = Console.ReadLine();

            if (input?.ToLower() == "end") break;
            if (string.IsNullOrWhiteSpace(input)) continue;

            string voterName = "";
            string voteOption = input;

            if (requireNames && input.Contains(':'))
            {
                var parts = input.Split(':', 2);
                voterName = parts[0].Trim();
                voteOption = parts[1].Trim();

                if (Voters.Contains(voterName))
                {
                    Console.WriteLine($"⚠️  {voterName} has already voted!");
                    continue;
                }
            }

            var match = options.FirstOrDefault(o => o.Equals(voteOption, StringComparison.OrdinalIgnoreCase));
            if (match != null)
            {
                Votes[match]++;
                if (requireNames && !string.IsNullOrEmpty(voterName))
                {
                    Voters.Add(voterName);
                    Console.WriteLine($"✅ {voterName} voted for {match}");
                }
                else
                {
                    Console.WriteLine($"✅ Vote recorded for {match}");
                }
            }
            else
            {
                Console.WriteLine($"❌ Invalid option! Choose from: {string.Join(", ", options)}");
            }
        }

        DisplayResults(options);
    }

    private static void DisplayResults(List<string> options)
    {
        int totalVotes = Votes.Values.Sum();

        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine("\n📊 POLL RESULTS\n");
        Console.WriteLine(new string('=', 50));

        var sorted = Votes.OrderByDescending(v => v.Value).ToList();
        int maxVotes = sorted.FirstOrDefault().Value;

        foreach (var (option, count) in sorted)
        {
            int barLength = totalVotes > 0 ? (count * 30) / totalVotes : 0;
            string bar = new string('█', barLength) + new string('░', 30 - barLength);
            string percentage = totalVotes > 0 ? $"{(count * 100.0 / totalVotes):F1}%" : "0%";
            string winner = count == maxVotes && count > 0 ? " 🏆" : "";

            Console.WriteLine($"{option,-20} [{bar}] {count,3} ({percentage,6}){winner}");
        }

        Console.WriteLine(new string('=', 50));
        Console.WriteLine($"\nTotal votes: {totalVotes}");
        Console.WriteLine($"Unique voters: {Voters.Count}");

        // Check for tie
        var winners = sorted.Where(v => v.Value == maxVotes).ToList();
        if (winners.Count > 1)
        {
            Console.WriteLine($"\n🤝 TIE between: {string.Join(", ", winners.Select(w => w.Key))}");
        }
        else if (winners.Count > 0)
        {
            Console.WriteLine($"\n🏆 WINNER: {winners[0].Key}!");
        }
    }
}
