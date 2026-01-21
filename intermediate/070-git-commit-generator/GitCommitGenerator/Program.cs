var types = new[]
{
    new { Type = "feat", Description = "A new feature" },
    new { Type = "fix", Description = "A bug fix" },
    new { Type = "docs", Description = "Documentation only changes" },
    new { Type = "style", Description = "Code style changes (formatting, etc)" },
    new { Type = "refactor", Description = "Code refactoring without behavior change" },
    new { Type = "perf", Description = "Performance improvements" },
    new { Type = "test", Description = "Adding or updating tests" },
    new { Type = "chore", Description = "Maintenance tasks, build config" },
    new { Type = "ci", Description = "CI/CD configuration changes" },
    new { Type = "build", Description = "Build system or external dependencies" },
};

Console.WriteLine("Git Commit Message Generator");
Console.WriteLine("Conventional Commits Format: https://www.conventionalcommits.org");
Console.WriteLine(new string('-', 60));

Console.WriteLine("\nSelect commit type:");
for (var i = 0; i < types.Length; i++)
{
    Console.WriteLine($"  {i + 1}. {types[i].Type,10} - {types[i].Description}");
}

Console.Write("\nEnter choice (1-10): ");
var input = Console.ReadLine();

if (!int.TryParse(input, out var choice) || choice < 1 || choice > types.Length)
{
    Console.WriteLine("Invalid choice. Using 'feat' as default.");
    choice = 1;
}

var selectedType = types[choice - 1].Type;

Console.Write("Enter scope (optional, press Enter to skip): ");
var scope = Console.ReadLine()?.Trim();

Console.Write("Enter commit message (imperative mood, e.g., 'add feature'): ");
var message = Console.ReadLine()?.Trim() ?? "update code";

if (string.IsNullOrEmpty(message))
{
    Console.WriteLine("Error: Commit message cannot be empty");
    return;
}

Console.Write("Enter breaking changes details (optional, press Enter to skip): ");
var breakingChanges = Console.ReadLine()?.Trim();

Console.Write("Enter issue reference (optional, e.g., #123): ");
var issueRef = Console.ReadLine()?.Trim();

// Build the commit message
var commitMessage = new System.Text.StringBuilder();
commitMessage.Append($"{selectedType}");

if (!string.IsNullOrEmpty(scope))
{
    commitMessage.Append($"({scope})");
}

commitMessage.Append($": {message}");

if (!string.IsNullOrEmpty(breakingChanges))
{
    commitMessage.Append("\n\n");
    commitMessage.Append($"BREAKING CHANGE: {breakingChanges}");
}

if (!string.IsNullOrEmpty(issueRef))
{
    if (!string.IsNullOrEmpty(breakingChanges))
    {
        commitMessage.Append($"\n\n{issueRef}");
    }
    else
    {
        commitMessage.Append($"\n\n{issueRef}");
    }
}

Console.WriteLine("\n" + new string('-', 60));
Console.WriteLine("Generated Commit Message:");
Console.WriteLine(new string('-', 60));
Console.WriteLine(commitMessage.ToString());
Console.WriteLine(new string('-', 60));

// Copy to clipboard command suggestion
Console.WriteLine("\nTip: Copy the commit message above and use with git:");
Console.WriteLine($"  git commit -m \"{selectedType}: {message}\"");
