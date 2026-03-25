namespace DecisionMaker;

class Program
{
    private static readonly Random Random = new();

    static void Main(string[] args)
    {
        Console.WriteLine("🔮 Decision Maker");
        Console.WriteLine("=================\n");

        if (args.Length > 0)
        {
            var options = args.ToList();
            MakeDecision(options);
            return;
        }

        Console.WriteLine("Modes:");
        Console.WriteLine("  1. Yes/No question");
        Console.WriteLine("  2. Choose from options");
        Console.WriteLine("  3. Coin flip");
        Console.WriteLine("  4. Random number");
        Console.Write("\nChoose mode (1-4): ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1": YesNoQuestion(); break;
            case "2": ChooseFromOptions(); break;
            case "3": CoinFlip(); break;
            case "4": RandomNumber(); break;
            default: Console.WriteLine("Invalid choice!"); break;
        }
    }

    private static void YesNoQuestion()
    {
        Console.WriteLine("\nAsk a yes/no question:");
        Console.Write("> ");
        string? question = Console.ReadLine();

        if (string.IsNullOrWhiteSpace(question))
        {
            Console.WriteLine("Please ask a question!");
            return;
        }

        Console.WriteLine("\n🔮 Consulting the oracle...");
        Thread.Sleep(1000);

        var answers = new[]
        {
            ("✅ Yes, definitely!", "positive"),
            ("✅ Yes, absolutely!", "positive"),
            ("✅ Most likely!", "positive"),
            ("✅ Yes, go for it!", "positive"),
            ("🤔 Maybe, try again later", "neutral"),
            ("🤔 Cannot predict now", "neutral"),
            ("🤔 Ask again when ready", "neutral"),
            ("❌ No, don't do it", "negative"),
            ("❌ Definitely not", "negative"),
            ("❌ Very doubtful", "negative"),
            ("❌ My sources say no", "negative"),
        };

        var answer = answers[Random.Next(answers.Length)];
        Console.WriteLine($"\n{answer.Item1}");

        // Add fun element based on question length
        if (question.Contains('?'))
            Console.WriteLine("(Good question format!)");
    }

    private static void ChooseFromOptions()
    {
        Console.WriteLine("\nEnter options (one per line, empty line to finish):");

        var options = new List<string>();
        string? line;
        int num = 1;

        while ((line = Console.ReadLine()) is not null && line.Trim() != "")
        {
            options.Add(line.Trim());
            num++;
        }

        if (options.Count < 2)
        {
            Console.WriteLine("Need at least 2 options!");
            return;
        }

        Console.WriteLine("\n🎲 Making selection...");
        Thread.Sleep(500);

        // Animate selection
        for (int i = 0; i < 10; i++)
        {
            Console.Write($"\r   Choosing: {options[Random.Next(options.Count)]}   ");
            Thread.Sleep(100);
        }

        var choice = options[Random.Next(options.Count)];
        Console.WriteLine($"\n\n🎯 Selected: {choice}");
    }

    private static void CoinFlip()
    {
        Console.WriteLine("\n🪙 Flipping coin...");
        Thread.Sleep(500);

        Console.WriteLine("\n   ┌───────┐");
        
        for (int i = 0; i < 5; i++)
        {
            string side = Random.Next(2) == 0 ? "H" : "T";
            Console.WriteLine($"   │   {side}   │");
            Thread.Sleep(150);
            Console.Write("\r");
        }

        string result = Random.Next(2) == 0 ? "HEADS" : "TAILS";
        Console.WriteLine($"\n   ┌───────┐");
        Console.WriteLine($"   │ {result,-5} │");
        Console.WriteLine($"   └───────┘");
        Console.WriteLine($"\nResult: {result}!");
    }

    private static void RandomNumber()
    {
        Console.Write("\nMinimum value: ");
        string? minInput = Console.ReadLine();
        Console.Write("Maximum value: ");
        string? maxInput = Console.ReadLine();

        if (!int.TryParse(minInput, out int min) || !int.TryParse(maxInput, out int max))
        {
            min = 1;
            max = 100;
            Console.WriteLine($"Using default: {min}-{max}");
        }

        if (min > max) (min, max) = (max, min);

        int result = Random.Next(min, max + 1);
        Console.WriteLine($"\n🎲 Random number: {result}");
    }

    private static void MakeDecision(List<string> options)
    {
        if (options.Count < 2)
        {
            Console.WriteLine("Please provide at least 2 options!");
            return;
        }

        var choice = options[Random.Next(options.Count)];
        Console.WriteLine($"\n🎯 Decision: {choice}");
    }
}
