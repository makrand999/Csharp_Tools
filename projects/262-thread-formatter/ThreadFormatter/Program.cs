namespace ThreadFormatter;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🧵 Twitter/X Thread Formatter");
        Console.WriteLine("=============================\n");

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run --project ThreadFormatter.csproj [input_file]");
            Console.WriteLine("       dotnet run --project ThreadFormatter.csproj --stdin");
            Console.WriteLine("\nPaste your thread content (end with empty line or Ctrl+D):");
        }

        List<string> lines = new();

        if (args.Contains("--stdin") || args.Length == 0)
        {
            string? line;
            while ((line = Console.ReadLine()) is not null && line.Trim() != "")
            {
                lines.Add(line);
            }
        }
        else if (File.Exists(args[0]))
        {
            lines = File.ReadAllLines(args[0]).ToList();
        }
        else
        {
            Console.WriteLine($"❌ File not found: {args[0]}");
            return;
        }

        if (lines.Count == 0)
        {
            Console.WriteLine("No input provided.");
            return;
        }

        string threadText = FormatThread(lines);

        Console.WriteLine("\n--- Formatted Thread ---\n");
        Console.WriteLine(threadText);

        // Save to file
        string outputFile = $"thread_{DateTime.Now:yyyyMMdd_HHmmss}.txt";
        File.WriteAllText(outputFile, threadText);
        Console.WriteLine($"\n✅ Thread saved to: {outputFile}");
    }

    private static string FormatThread(List<string> lines)
    {
        var formatted = new System.Text.StringBuilder();
        var tweetBuffer = new System.Text.StringBuilder();
        int tweetCount = 1;
        int charLimit = 280;

        formatted.AppendLine($"🧵 Thread");
        formatted.AppendLine(new string('=', 50));
        formatted.AppendLine();

        foreach (string rawLine in lines)
        {
            string line = rawLine.Trim();
            if (line.Length == 0) continue;

            // Check if adding this line exceeds tweet limit
            if (tweetBuffer.Length + line.Length + 3 > charLimit)
            {
                // Output current tweet
                formatted.AppendLine($"[{tweetCount}/{int.MaxValue}]");
                formatted.AppendLine(tweetBuffer.ToString().Trim());
                formatted.AppendLine();
                formatted.AppendLine(new string('-', 40));
                formatted.AppendLine();

                // Start new tweet
                tweetBuffer.Clear();
                tweetCount++;
            }

            if (tweetBuffer.Length > 0)
                tweetBuffer.Append(" ");
            tweetBuffer.Append(line);
        }

        // Output final tweet
        if (tweetBuffer.Length > 0)
        {
            formatted.AppendLine($"[{tweetCount}/{tweetCount}]");
            formatted.AppendLine(tweetBuffer.ToString().Trim());
            formatted.AppendLine();
            formatted.AppendLine(new string('-', 40));
            formatted.AppendLine();
        }

        formatted.AppendLine();
        formatted.AppendLine($"End of thread ({tweetCount} tweets)");
        formatted.AppendLine($"Generated: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");

        return formatted.ToString();
    }
}
