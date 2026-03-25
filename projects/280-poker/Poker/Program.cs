namespace Poker;

class Program
{
    private static readonly Random Random = new();

    static void Main()
    {
        Console.WriteLine("🃏 Poker Hand Evaluator");
        Console.WriteLine("======================\n");

        Console.WriteLine("Modes:");
        Console.WriteLine("  1. Evaluate a hand");
        Console.WriteLine("  2. Deal random hand");
        Console.WriteLine("  3. Compare two hands");
        Console.Write("\nChoose mode (1-3): ");

        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1": EvaluateHand(); break;
            case "2": DealRandomHand(); break;
            case "3": CompareHands(); break;
            default: Console.WriteLine("Invalid choice!"); break;
        }
    }

    private static void EvaluateHand()
    {
        Console.WriteLine("\nEnter 5 cards (format: rank suit, e.g., 'A S' for Ace of Spades)");
        Console.WriteLine("Ranks: 2-9, T, J, Q, K, A");
        Console.WriteLine("Suits: S (Spades), H (Hearts), D (Diamonds), C (Clubs)\n");

        var cards = new List<Card>();
        for (int i = 0; i < 5; i++)
        {
            Console.Write($"Card {i + 1}: ");
            string? input = Console.ReadLine();
            if (input == null || input.Length < 2)
            {
                Console.WriteLine("Invalid card!");
                return;
            }

            string rank = input.Substring(0, 1).ToUpper();
            string suit = input.Substring(1).Trim().ToUpper();

            if (!"23456789TJQKA".Contains(rank) || !"SHDC".Contains(suit))
            {
                Console.WriteLine("Invalid card!");
                return;
            }

            cards.Add(new Card { Rank = rank, Suit = suit });
        }

        var hand = Evaluate(cards);
        DisplayHand(cards, hand);
    }

    private static void DealRandomHand()
    {
        var deck = CreateDeck();
        Shuffle(deck);
        var hand = deck.Take(5).ToList();
        var evaluation = Evaluate(hand);
        DisplayHand(hand, evaluation);
    }

    private static void CompareHands()
    {
        Console.WriteLine("\n--- Hand 1 ---");
        var hand1 = InputHand();
        var eval1 = Evaluate(hand1);

        Console.WriteLine("\n--- Hand 2 ---");
        var hand2 = InputHand();
        var eval2 = Evaluate(hand2);

        Console.WriteLine("\n=== Results ===");
        DisplayHand(hand1, eval1, "Player 1");
        DisplayHand(hand2, eval2, "Player 2");

        int comparison = CompareHands(eval1, eval2);
        if (comparison > 0)
            Console.WriteLine("\n🏆 Player 1 wins!");
        else if (comparison < 0)
            Console.WriteLine("\n🏆 Player 2 wins!");
        else
            Console.WriteLine("\n🤝 It's a tie!");
    }

    private static List<Card> InputHand()
    {
        var cards = new List<Card>();
        for (int i = 0; i < 5; i++)
        {
            Console.Write($"Card {i + 1}: ");
            string? input = Console.ReadLine();
            string rank = input?.Substring(0, 1).ToUpper() ?? "";
            string suit = input?.Substring(1).Trim().ToUpper() ?? "";
            cards.Add(new Card { Rank = rank, Suit = suit });
        }
        return cards;
    }

    private static void DisplayHand(List<Card> cards, HandEvaluation eval, string? label = null)
    {
        if (label != null)
            Console.WriteLine($"\n{label}'s hand:");
        else
            Console.WriteLine("\nYour hand:");

        Console.WriteLine($"  {string.Join(" ", cards.Select(c => c.ToString()))}");
        Console.WriteLine($"\n{eval.RankName}");
        if (!string.IsNullOrEmpty(eval.Description))
            Console.WriteLine(eval.Description);
    }

    private static HandEvaluation Evaluate(List<Card> cards)
    {
        cards = cards.OrderByDescending(c => GetRankValue(c.Rank)).ToList();

        bool isFlush = cards.All(c => c.Suit == cards[0].Suit);
        
        var ranks = cards.Select(c => GetRankValue(c.Rank)).ToList();
        bool isStraight = true;
        for (int i = 0; i < 4; i++)
        {
            if (ranks[i] - ranks[i + 1] != 1)
            {
                // Check for A-2-3-4-5 straight
                if (i == 0 && ranks[0] == 14 && ranks[1] == 5 && ranks[2] == 4 && ranks[3] == 3 && ranks[4] == 2)
                {
                    ranks = new List<int> { 5, 4, 3, 2, 1 }; // A becomes 5
                }
                else
                {
                    isStraight = false;
                    break;
                }
            }
        }

        var groups = cards.GroupBy(c => c.Rank).Select(g => new { Rank = g.Key, Count = g.Count() })
            .OrderByDescending(g => g.Count).ThenByDescending(g => GetRankValue(g.Rank)).ToList();

        // Determine hand rank
        if (isStraight && isFlush)
        {
            if (ranks[0] == 14 && ranks[4] == 10)
                return new HandEvaluation { Rank = 10, RankName = "🏆 Royal Flush!", Description = "A-K-Q-J-10 of same suit" };
            return new HandEvaluation { Rank = 9, RankName = "Straight Flush", Description = $"{GetRankName(ranks[0])} high" };
        }

        if (groups[0].Count == 4)
            return new HandEvaluation { Rank = 8, RankName = "Four of a Kind", Description = $"Four {GetRankName(groups[0].Rank)}" };

        if (groups[0].Count == 3 && groups[1].Count == 2)
            return new HandEvaluation { Rank = 7, RankName = "Full House", Description = $"{GetRankName(groups[0].Rank)}s full of {GetRankName(groups[1].Rank)}s" };

        if (isFlush)
            return new HandEvaluation { Rank = 6, RankName = "Flush", Description = $"{GetRankName(cards[0].Rank)} high" };

        if (isStraight)
            return new HandEvaluation { Rank = 5, RankName = "Straight", Description = $"{GetRankName(ranks[0])} high" };

        if (groups[0].Count == 3)
            return new HandEvaluation { Rank = 4, RankName = "Three of a Kind", Description = $"Three {GetRankName(groups[0].Rank)}" };

        if (groups[0].Count == 2 && groups[1].Count == 2)
            return new HandEvaluation { Rank = 3, RankName = "Two Pair", Description = $"{GetRankName(groups[0].Rank)}s and {GetRankName(groups[1].Rank)}s" };

        if (groups[0].Count == 2)
            return new HandEvaluation { Rank = 2, RankName = "Pair", Description = $"Pair of {GetRankName(groups[0].Rank)}" };

        return new HandEvaluation { Rank = 1, RankName = "High Card", Description = GetRankName(cards[0].Rank) };
    }

    private static int CompareHands(HandEvaluation h1, HandEvaluation h2)
    {
        return h1.Rank.CompareTo(h2.Rank);
    }

    private static int GetRankValue(string rank)
    {
        return rank switch
        {
            "A" => 14, "K" => 13, "Q" => 12, "J" => 11, "T" => 10,
            _ => int.Parse(rank)
        };
    }

    private static string GetRankName(int value)
    {
        return value switch
        {
            14 => "Ace", 13 => "King", 12 => "Queen", 11 => "Jack", 10 => "Ten",
            _ => value.ToString()
        };
    }

    private static string GetRankName(string rank) => GetRankName(GetRankValue(rank));

    private static List<Card> CreateDeck()
    {
        var deck = new List<Card>();
        string[] suits = { "S", "H", "D", "C" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                deck.Add(new Card { Rank = rank, Suit = suit });
            }
        }

        return deck;
    }

    private static void Shuffle(List<Card> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }
}

class Card
{
    public string Rank { get; set; } = "";
    public string Suit { get; set; } = "";

    public override string ToString()
    {
        string suitSymbol = Suit switch { "S" => "♠", "H" => "♥", "D" => "♦", "C" => "♣", _ => Suit };
        return $"{Rank}{suitSymbol}";
    }
}

class HandEvaluation
{
    public int Rank { get; set; }
    public string RankName { get; set; } = "";
    public string Description { get; set; } = "";
}
