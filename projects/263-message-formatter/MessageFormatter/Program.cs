namespace MessageFormatter;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("💬 Slack/Discord Message Formatter");
        Console.WriteLine("==================================\n");

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run --project MessageFormatter.csproj [message]");
            Console.WriteLine("\nFormatting options:");
            Console.WriteLine("  --bold      Make text bold");
            Console.WriteLine("  --italic    Make text italic");
            Console.WriteLine("  --code      Inline code");
            Console.WriteLine("  --block     Code block");
            Console.WriteLine("  --quote     Quote block");
            Console.WriteLine("  --link      Create link");
            Console.WriteLine("  --strike    Strikethrough");
            Console.WriteLine("  --all       Show all formats demo");
            return;
        }

        if (args.Contains("--all"))
        {
            ShowAllFormats();
            return;
        }

        string message = string.Join(" ", args.Where(a => !a.StartsWith("--")));
        string format = args.FirstOrDefault(a => a.StartsWith("--"))?.TrimStart('-') ?? "plain";

        string formatted = format switch
        {
            "bold" => FormatBold(message),
            "italic" => FormatItalic(message),
            "code" => FormatCode(message),
            "block" => FormatBlock(message),
            "quote" => FormatQuote(message),
            "link" => FormatLink(message),
            "strike" => FormatStrike(message),
            _ => message
        };

        Console.WriteLine("\n--- Formatted Message ---\n");
        Console.WriteLine(formatted);
        Console.WriteLine("\n(Copy and paste into Slack/Discord)");
    }

    private static string FormatBold(string text) => $"*{text}*";
    private static string FormatItalic(string text) => $"_{text}_";
    private static string FormatCode(string text) => $"`{text}`";
    private static string FormatStrike(string text) => $"~{text}~";

    private static string FormatBlock(string text)
    {
        return $"```\n{text}\n```";
    }

    private static string FormatQuote(string text)
    {
        var lines = text.Split('\n');
        return string.Join("\n", lines.Select(l => $"> {l}"));
    }

    private static string FormatLink(string text)
    {
        Console.Write("Enter URL: ");
        string? url = Console.ReadLine();
        return string.IsNullOrEmpty(url) ? text : $"<{url}|{text}>";
    }

    private static void ShowAllFormats()
    {
        Console.WriteLine("\n=== Slack/Discord Formatting Guide ===\n");

        var examples = new (string name, string slack, string discord)[]
        {
            ("Bold", "*bold*", "**bold**"),
            ("Italic", "_italic_", "*italic*"),
            ("Strikethrough", "~strike~", "~~strike~~"),
            ("Inline Code", "`code`", "`code`"),
            ("Code Block", "```\\ncode\\n```", "```\\ncode\\n```"),
            ("Quote", "> quote", "> quote"),
            ("Link", "<url|text>", "[text](url)"),
        };

        Console.WriteLine($"{"Format",-15} {"Slack",-25} {"Discord",-25}");
        Console.WriteLine(new string('-', 65));

        foreach (var (name, slack, discord) in examples)
        {
            Console.WriteLine($"{name,-15} {slack,-25} {discord,-25}");
        }

        Console.WriteLine("\n=== Live Preview ===\n");
        Console.WriteLine($"Bold:         {FormatBold("This is bold")}");
        Console.WriteLine($"Italic:       {FormatItalic("This is italic")}");
        Console.WriteLine($"Code:         {FormatCode("Console.WriteLine()")}");
        Console.WriteLine($"Strike:       {FormatStrike("This is wrong")}");
        Console.WriteLine($"Quote:        {FormatQuote("To be or not to be")}");
        Console.WriteLine($"Code Block:\n{FormatBlock("public class Program {}")}");
    }
}
