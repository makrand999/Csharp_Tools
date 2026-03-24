namespace Blackjack;

class Program
{
    private static List<Card> deck = new();
    private static List<Card> playerHand = new();
    private static List<Card> dealerHand = new();
    private static int playerMoney = 100;
    private static int currentBet;
    private static readonly Random Random = new();

    static void Main()
    {
        Console.WriteLine("🃏 Blackjack");
        Console.WriteLine("============\n");

        while (playerMoney > 0)
        {
            Console.WriteLine($"💰 Your money: ${playerMoney}");
            Console.Write("Place your bet (0 to quit): $");
            
            if (!int.TryParse(Console.ReadLine(), out currentBet) || currentBet <= 0)
            {
                Console.WriteLine($"Thanks for playing! You're leaving with ${playerMoney}");
                return;
            }

            if (currentBet > playerMoney)
            {
                Console.WriteLine("Not enough money!");
                continue;
            }

            PlayRound();
        }

        Console.WriteLine("\n💸 You're bankrupt! Game over.");
    }

    private static void PlayRound()
    {
        deck = CreateDeck();
        Shuffle(deck);
        playerHand.Clear();
        dealerHand.Clear();

        // Deal initial cards
        playerHand.Add(deck[0]);
        dealerHand.Add(deck[1]);
        playerHand.Add(deck[2]);
        dealerHand.Add(deck[3]);
        deck.RemoveRange(0, 4);

        bool playerBusted = false;

        // Player's turn
        while (true)
        {
            DisplayTable(false);

            int playerTotal = CalculateHand(playerHand);
            Console.WriteLine($"\nYour total: {playerTotal}");

            if (playerTotal == 21)
            {
                Console.WriteLine("🎰 Blackjack!");
                break;
            }

            if (playerTotal > 21)
            {
                Console.WriteLine("💥 Bust!");
                playerBusted = true;
                break;
            }

            Console.Write("\n(H)it or (S)tand? ");
            string? choice = Console.ReadLine()?.ToUpper();

            if (choice == "S") break;
            if (choice != "H") continue;

            playerHand.Add(deck[0]);
            deck.RemoveAt(0);
        }

        // Dealer's turn
        if (!playerBusted)
        {
            while (CalculateHand(dealerHand) < 17)
            {
                dealerHand.Add(deck[0]);
                deck.RemoveAt(0);
            }

            DisplayTable(true);

            int playerTotal = CalculateHand(playerHand);
            int dealerTotal = CalculateHand(dealerHand);

            Console.WriteLine($"\nDealer total: {dealerTotal}");

            if (dealerTotal > 21)
            {
                Console.WriteLine("🎉 Dealer busts! You win!");
                playerMoney += currentBet;
            }
            else if (playerTotal > dealerTotal)
            {
                Console.WriteLine("🎉 You win!");
                playerMoney += currentBet;
            }
            else if (playerTotal < dealerTotal)
            {
                Console.WriteLine("😞 Dealer wins!");
                playerMoney -= currentBet;
            }
            else
            {
                Console.WriteLine("🤝 Push (tie)!");
            }

            // Blackjack pays 3:2
            if (playerTotal == 21 && playerHand.Count == 2)
            {
                Console.WriteLine("🎰 Blackjack bonus!");
                playerMoney += currentBet / 2;
            }
        }
        else
        {
            playerMoney -= currentBet;
        }

        Console.WriteLine($"\n💰 Your money: ${playerMoney}");
        Console.WriteLine(new string('-', 40));
    }

    private static void DisplayTable(bool showDealer)
    {
        Console.Clear();
        Console.WriteLine("🃏 Blackjack\n");

        Console.WriteLine("Dealer's hand:");
        if (showDealer)
        {
            Console.WriteLine($"  {string.Join(" ", dealerHand.Select(c => c.ToString()))}");
            Console.WriteLine($"  Total: {CalculateHand(dealerHand)}");
        }
        else
        {
            Console.WriteLine($"  {dealerHand[0]} [?]");
        }

        Console.WriteLine("\nYour hand:");
        Console.WriteLine($"  {string.Join(" ", playerHand.Select(c => c.ToString()))}");
    }

    private static List<Card> CreateDeck()
    {
        var cards = new List<Card>();
        string[] suits = { "♠", "♥", "♦", "♣" };
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "J", "Q", "K", "A" };

        foreach (var suit in suits)
        {
            foreach (var rank in ranks)
            {
                cards.Add(new Card { Suit = suit, Rank = rank });
            }
        }

        return cards;
    }

    private static void Shuffle(List<Card> cards)
    {
        for (int i = cards.Count - 1; i > 0; i--)
        {
            int j = Random.Next(i + 1);
            (cards[i], cards[j]) = (cards[j], cards[i]);
        }
    }

    private static int CalculateHand(List<Card> hand)
    {
        int total = 0;
        int aces = 0;

        foreach (var card in hand)
        {
            if (card.Rank == "A")
            {
                aces++;
                total += 11;
            }
            else if (card.Rank is "K" or "Q" or "J")
            {
                total += 10;
            }
            else
            {
                total += int.Parse(card.Rank);
            }
        }

        while (total > 21 && aces > 0)
        {
            total -= 10;
            aces--;
        }

        return total;
    }
}

class Card
{
    public string Suit { get; set; } = "";
    public string Rank { get; set; } = "";

    public override string ToString() => $"{Rank}{Suit}";
}
