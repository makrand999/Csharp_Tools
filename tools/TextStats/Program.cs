// Text Statistics - Analyze text files for various statistics
class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Usage: TextStats <file1> [file2] [file3] ...");
            Console.WriteLine("       TextStats --stdin");
            return;
        }

        if (args[0] == "--stdin")
        {
            AnalyzeText(Console.ReadToEnd(), "stdin");
            return;
        }

        foreach (var file in args)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"File not found: {file}");
                continue;
            }

            string content = File.ReadAllText(file);
            AnalyzeText(content, file);
            Console.WriteLine();
        }
    }

    static void AnalyzeText(string text, string label)
    {
        var lines = text.Split('\n');
        var words = text.Split(new[] { ' ', '\t', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
        var sentences = text.Split(new[] { '.', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

        int charCount = text.Length;
        int charCountNoSpace = text.Replace(" ", "").Replace("\t", "").Replace("\n", "").Replace("\r", "").Length;
        int lineCount = lines.Length;
        int wordCount = words.Length;
        int sentenceCount = sentences.Length;

        double avgWordLength = wordCount > 0 ? words.Average(w => w.Length) : 0;
        double avgWordsPerLine = lineCount > 0 ? (double)wordCount / lineCount : 0;

        // Find most common words (top 5)
        var wordFreq = words
            .GroupBy(w => w.ToLower().Trim('.', ',', '!', '?', ';', ':', '"', '\''))
            .Where(g => g.Key.Length > 0)
            .OrderByDescending(g => g.Count())
            .Take(5);

        Console.WriteLine($"=== Statistics for: {label} ===");
        Console.WriteLine($"Characters (with spaces):    {charCount,8}");
        Console.WriteLine($"Characters (no spaces):      {charCountNoSpace,8}");
        Console.WriteLine($"Lines:                       {lineCount,8}");
        Console.WriteLine($"Words:                       {wordCount,8}");
        Console.WriteLine($"Sentences:                   {sentenceCount,8}");
        Console.WriteLine($"Average word length:         {avgWordLength,8:F2}");
        Console.WriteLine($"Average words per line:      {avgWordsPerLine,8:F2}");

        Console.WriteLine("\nTop 5 most common words:");
        foreach (var wf in wordFreq)
        {
            Console.WriteLine($"  {wf.Key,-20} {wf.Count()}");
        }
    }
}
