namespace CommitGenerator;

class Program
{
    private static readonly string[] Types = { "feat", "fix", "docs", "style", "refactor", "test", "chore", "perf", "ci", "build" };
    private static readonly string[] Scopes = { "api", "ui", "core", "auth", "db", "config", "deps", "docs", "tests", "utils" };

    static void Main(string[] args)
    {
        Console.WriteLine("📝 Git Commit Message Generator");
        Console.WriteLine("================================\n");

        Console.WriteLine("Select commit type:");
        for (int i = 0; i < Types.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {Types[i]}");
        }
        Console.Write("\nChoice (1-{0}): ", Types.Length);

        string? typeInput = Console.ReadLine();
        if (!int.TryParse(typeInput, out int typeIndex) || typeIndex < 1 || typeIndex > Types.Length)
        {
            Console.WriteLine("Invalid selection. Using 'feat' as default.");
            typeIndex = 1;
        }

        string commitType = Types[typeIndex - 1];

        Console.Write("\nEnter scope (optional, press Enter to skip): ");
        string? scopeInput = Console.ReadLine();
        string scope = string.IsNullOrWhiteSpace(scopeInput) ? "" : scopeInput!.Trim();

        Console.Write("\nEnter short description (imperative mood): ");
        string? description = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(description))
        {
            Console.WriteLine("Description required!");
            return;
        }

        Console.Write("\nEnter body (optional, press Enter to skip): ");
        string? bodyInput = Console.ReadLine();
        string body = bodyInput?.Trim() ?? "";

        Console.Write("\nEnter footer (optional, e.g., 'Closes #123', press Enter to skip): ");
        string? footerInput = Console.ReadLine();
        string footer = footerInput?.Trim() ?? "";

        string commitMessage = BuildCommitMessage(commitType, scope, description!, body, footer);

        Console.WriteLine("\n--- Generated Commit Message ---\n");
        Console.WriteLine(commitMessage);

        // Save to clipboard-style file
        string outputFile = "commit_message.txt";
        File.WriteAllText(outputFile, commitMessage);
        Console.WriteLine($"\n✅ Commit message saved to: {outputFile}");

        // Copy to clipboard command
        Console.WriteLine("\nTo copy to clipboard:");
        Console.WriteLine($"  cat {outputFile} | clip");
    }

    private static string BuildCommitMessage(string type, string scope, string description, string body, string footer)
    {
        var commit = new System.Text.StringBuilder();

        // Header line
        if (!string.IsNullOrEmpty(scope))
        {
            commit.AppendLine($"{type}({scope}): {description}");
        }
        else
        {
            commit.AppendLine($"{type}: {description}");
        }

        // Body (blank line + body)
        if (!string.IsNullOrEmpty(body))
        {
            commit.AppendLine();
            commit.AppendLine(body);
        }

        // Footer (blank line + footer)
        if (!string.IsNullOrEmpty(footer))
        {
            commit.AppendLine();
            commit.AppendLine(footer);
        }

        return commit.ToString().TrimEnd();
    }
}
