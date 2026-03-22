namespace QuizGame;

class Program
{
    private static readonly List<Question> Questions = new()
    {
        new("What does CPU stand for?", new[] { "Central Processing Unit", "Central Power Unit", "Computer Personal Unit", "Central Process Utility" }, 0),
        new("Which language runs in a web browser?", new[] { "Java", "C", "Python", "JavaScript" }, 3),
        new("What does CSS stand for?", new[] { "Creative Style Sheets", "Cascading Style Sheets", "Computer Style Sheets", "Colorful Style Sheets" }, 1),
        new("What year was C# released?", new[] { "1999", "2000", "2001", "2002" }, 2),
        new("Which company created C#?", new[] { "Google", "Apple", "Microsoft", "IBM" }, 2),
        new("What is the largest planet in our solar system?", new[] { "Earth", "Mars", "Jupiter", "Saturn" }, 2),
        new("What is H2O commonly known as?", new[] { "Hydrogen Peroxide", "Water", "Helium", "Hydrazine" }, 1),
        new("How many continents are there?", new[] { "5", "6", "7", "8" }, 2),
        new("What is the capital of France?", new[] { "Berlin", "Madrid", "Paris", "Rome" }, 2),
        new("Which planet is known as the Red Planet?", new[] { "Venus", "Mars", "Jupiter", "Saturn" }, 1),
    };

    static void Main()
    {
        Console.WriteLine("🧠 Quiz Game");
        Console.WriteLine("============\n");

        int score = 0;
        int current = 0;

        foreach (var question in Questions)
        {
            current++;
            Console.WriteLine($"\nQuestion {current}/{Questions.Count}:");
            Console.WriteLine(question.Text);
            Console.WriteLine();

            for (int i = 0; i < question.Options.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {question.Options[i]}");
            }

            Console.Write("\nYour answer (1-4): ");
            string? input = Console.ReadLine();

            if (int.TryParse(input, out int answer) && answer >= 1 && answer <= 4)
            {
                if (answer - 1 == question.CorrectIndex)
                {
                    Console.WriteLine("✅ Correct!");
                    score++;
                }
                else
                {
                    Console.WriteLine($"❌ Wrong! The correct answer was: {question.Options[question.CorrectIndex]}");
                }
            }
            else
            {
                Console.WriteLine($"⏭️  Skipped! The correct answer was: {question.Options[question.CorrectIndex]}");
            }
        }

        Console.WriteLine("\n" + new string('=', 40));
        Console.WriteLine($"\n📊 Final Score: {score}/{Questions.Count} ({score * 100 / Questions.Count}%)");

        if (score == Questions.Count)
            Console.WriteLine("🏆 Perfect score! You're a genius!");
        else if (score >= Questions.Count * 0.8)
            Console.WriteLine("🌟 Great job!");
        else if (score >= Questions.Count * 0.5)
            Console.WriteLine("👍 Good effort!");
        else
            Console.WriteLine("📚 Keep learning!");
    }
}

class Question
{
    public string Text { get; }
    public string[] Options { get; }
    public int CorrectIndex { get; }

    public Question(string text, string[] options, int correctIndex)
    {
        Text = text;
        Options = options;
        CorrectIndex = correctIndex;
    }
}
