namespace DiceRoller;

class Program
{
    private static readonly Random Random = new();

    static void Main(string[] args)
    {
        Console.WriteLine("🎲 Dice Roller");
        Console.WriteLine("==============\n");

        if (args.Length >= 1 && args[0] == "--roll")
        {
            string notation = args.ElementAtOrDefault(1) ?? "1d6";
            Roll(notation);
            return;
        }

        Console.WriteLine("Dice notation: NdS (N = number of dice, S = sides)");
        Console.WriteLine("Examples: 1d6, 2d6, 3d8, 4d10, 5d20, 2d6+3\n");

        while (true)
        {
            Console.Write("Enter dice notation (or 'q' to quit): ");
            string? input = Console.ReadLine();

            if (input?.ToLower() == "q") break;
            if (string.IsNullOrWhiteSpace(input)) continue;

            Roll(input);
            Console.WriteLine();
        }
    }

    private static void Roll(string notation)
    {
        try
        {
            notation = notation.Replace(" ", "");

            int modifier = 0;
            var modMatch = System.Text.RegularExpressions.Regex.Match(notation, "([+-]\\d+)$");
            if (modMatch.Success)
            {
                modifier = int.Parse(modMatch.Groups[1].Value);
                notation = notation[..modMatch.Index];
            }

            var parts = notation.Split('d');
            if (parts.Length != 2 ||
                !int.TryParse(parts[0], out int numDice) ||
                !int.TryParse(parts[1], out int sides) ||
                numDice < 1 || sides < 2)
            {
                Console.WriteLine("❌ Invalid notation! Use format: NdS (e.g., 2d6, 3d8)");
                return;
            }

            Console.WriteLine($"\nRolling {numDice}d{sides}{(modifier != 0 ? (modifier > 0 ? $"+{modifier}" : modifier.ToString()) : "")}...");

            var rolls = new List<int>();
            int total = 0;

            for (int i = 0; i < numDice; i++)
            {
                int roll = Random.Next(1, sides + 1);
                rolls.Add(roll);
                total += roll;
            }

            total += modifier;

            Console.WriteLine($"Rolls: [{string.Join(", ", rolls)}]{(modifier != 0 ? $" + {modifier}" : "")}");
            Console.WriteLine($"Total: {total}");

            // Show crits for d20
            if (sides == 20 && numDice == 1)
            {
                if (rolls[0] == 20) Console.WriteLine("🎉 NATURAL 20! Critical hit!");
                else if (rolls[0] == 1) Console.WriteLine("💀 Natural 1! Critical miss!");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }
}
