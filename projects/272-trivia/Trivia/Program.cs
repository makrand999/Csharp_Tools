using System.Net.Http.Json;
using System.Text.Json;

namespace Trivia;

class Program
{
    private static readonly HttpClient HttpClient = new();
    private static int _score = 0;
    private static int _streak = 0;

    static async Task Main()
    {
        Console.WriteLine("🎯 Trivia Challenge");
        Console.WriteLine("===================\n");

        Console.WriteLine("Select difficulty:");
        Console.WriteLine("  1. Easy");
        Console.WriteLine("  2. Medium");
        Console.WriteLine("  3. Hard");
        Console.Write("\nChoice (1-3): ");

        string? diffInput = Console.ReadLine();
        string difficulty = diffInput switch
        {
            "1" => "easy",
            "2" => "medium",
            _ => "hard"
        };

        Console.Write("\nHow many questions? (5-20, default=10): ");
        string? countInput = Console.ReadLine();
        int count = int.TryParse(countInput, out int c) && c >= 5 && c <= 20 ? c : 10;

        Console.WriteLine($"\nFetching {count} {difficulty} questions...\n");

        try
        {
            var questions = await FetchQuestions(count, difficulty);
            
            if (questions == null || questions.Count == 0)
            {
                Console.WriteLine("❌ Failed to fetch questions. Check your internet connection.");
                return;
            }

            foreach (var q in questions)
            {
                await AskQuestion(q);
            }

            ShowResults(questions.Count);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    private static async Task<List<TriviaQuestion>?> FetchQuestions(int count, string difficulty)
    {
        string url = $"https://opentdb.com/api.php?amount={count}&difficulty={difficulty}&type=multiple";
        var response = await HttpClient.GetAsync(url);
        response.EnsureSuccessStatusCode();
        
        var result = await response.Content.ReadFromJsonAsync<TriviaResponse>();
        return result?.Results;
    }

    private static async Task AskQuestion(TriviaQuestion q)
    {
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"Category: {q.Category}");
        Console.WriteLine($"Question: {System.Net.WebUtility.HtmlDecode(q.Question)}\n");

        var allAnswers = new List<string> { q.CorrectAnswer };
        allAnswers.AddRange(q.IncorrectAnswers);
        allAnswers = allAnswers.OrderBy(_ => Guid.NewGuid()).ToList();

        for (int i = 0; i < allAnswers.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {System.Net.WebUtility.HtmlDecode(allAnswers[i])}");
        }

        Console.Write("\nYour answer (1-4, or 'q' to quit): ");
        string? input = Console.ReadLine();

        if (input?.ToLower() == "q")
        {
            Console.WriteLine("\nThanks for playing!");
            Environment.Exit(0);
        }

        if (int.TryParse(input, out int answer) && answer >= 1 && answer <= 4)
        {
            string selected = allAnswers[answer - 1];
            if (selected == q.CorrectAnswer)
            {
                Console.WriteLine("✅ Correct!");
                _score++;
                _streak++;
                if (_streak >= 3)
                    Console.WriteLine($"🔥 Streak: {_streak}!");
            }
            else
            {
                Console.WriteLine($"❌ Wrong! Correct: {System.Net.WebUtility.HtmlDecode(q.CorrectAnswer)}");
                _streak = 0;
            }
        }
        else
        {
            Console.WriteLine($"⏭️  Skipped! Correct: {System.Net.WebUtility.HtmlDecode(q.CorrectAnswer)}");
            _streak = 0;
        }

        Console.WriteLine($"Score: {_score} | Streak: {_streak}\n");
        await Task.Delay(500);
    }

    private static void ShowResults(int total)
    {
        Console.WriteLine("\n" + new string('=', 50));
        Console.WriteLine($"\n📊 Final Results");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"Score: {_score}/{total} ({_score * 100 / total}%)");
        Console.WriteLine($"Best streak: {_streak}");

        if (_score == total)
            Console.WriteLine("\n🏆 Perfect! You're a trivia master!");
        else if (_score >= total * 0.7)
            Console.WriteLine("\n🌟 Excellent work!");
        else if (_score >= total * 0.5)
            Console.WriteLine("\n👍 Good job!");
        else
            Console.WriteLine("\n📚 Keep practicing!");
    }
}

class TriviaResponse
{
    public List<TriviaQuestion> Results { get; set; } = new();
}

class TriviaQuestion
{
    public string Category { get; set; } = "";
    public string Question { get; set; } = "";
    public string CorrectAnswer { get; set; } = "";
    public List<string> IncorrectAnswers { get; set; } = new();
}
