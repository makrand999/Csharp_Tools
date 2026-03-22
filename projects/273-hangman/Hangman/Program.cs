namespace Hangman;

class Program
{
    private static readonly string[] Words = {
        "PROGRAMMING", "COMPUTER", "ALGORITHM", "DATABASE", "NETWORK",
        "INTERFACE", "VARIABLE", "FUNCTION", "ASSEMBLY", "COMPILER",
        "DEBUGGING", "ENCRYPTION", "FIREWALL", "GIGABYTE", "HARDWARE",
        "KEYBOARD", "MONITOR", "PROCESSOR", "SOFTWARE", "TERMINAL"
    };

    private static readonly string[] HangmanArt = {
        @"
  +---+
      |
      |
      |
     ===",
        @"
  +---+
  O   |
      |
      |
     ===",
        @"
  +---+
  O   |
  |   |
      |
     ===",
        @"
  +---+
  O   |
 /|   |
      |
     ===",
        @"
  +---+
  O   |
 /|\  |
      |
     ===",
        @"
  +---+
  O   |
 /|\  |
 /    |
     ===",
        @"
  +---+
  O   |
 /|\  |
 / \  |
     ==="
    };

    static void Main()
    {
        Console.WriteLine("🎮 Hangman");
        Console.WriteLine("=========\n");

        var random = new Random();
        string word = Words[random.Next(Words.Length)];
        var guessed = new HashSet<char>();
        int wrongAttempts = 0;
        const int maxAttempts = 6;

        while (wrongAttempts < maxAttempts)
        {
            Console.Clear();
            Console.WriteLine("🎮 Hangman\n");
            Console.WriteLine(HangmanArt[wrongAttempts]);
            Console.WriteLine();

            // Display word with blanks
            string display = string.Join(" ", word.Select(c => guessed.Contains(c) ? c.ToString() : "_"));
            Console.WriteLine($"Word: {display}");
            Console.WriteLine($"\nWrong attempts: {wrongAttempts}/{maxAttempts}");
            
            if (guessed.Count > 0)
            {
                Console.WriteLine($"Guessed letters: {string.Join(", ", guessed.OrderBy(c => c))}");
            }

            // Check win condition
            if (word.All(c => guessed.Contains(c)))
            {
                Console.WriteLine($"\n🎉 Congratulations! You guessed: {word}");
                return;
            }

            Console.Write("\nGuess a letter: ");
            string? input = Console.ReadLine()?.ToUpper();

            if (string.IsNullOrEmpty(input) || input.Length != 1 || !char.IsLetter(input[0]))
            {
                Console.WriteLine("⚠️  Please enter a single letter.");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                continue;
            }

            char letter = input[0];

            if (guessed.Contains(letter))
            {
                Console.WriteLine($"⚠️  You already guessed '{letter}'!");
                Console.WriteLine("Press any key to continue...");
                Console.ReadKey();
                continue;
            }

            guessed.Add(letter);

            if (!word.Contains(letter))
            {
                Console.WriteLine($"❌ '{letter}' is not in the word!");
                wrongAttempts++;
            }
            else
            {
                Console.WriteLine($"✅ '{letter}' is in the word!");
            }
        }

        Console.Clear();
        Console.WriteLine("🎮 Hangman\n");
        Console.WriteLine(HangmanArt[wrongAttempts]);
        Console.WriteLine($"\n💀 Game Over! The word was: {word}");
    }
}
