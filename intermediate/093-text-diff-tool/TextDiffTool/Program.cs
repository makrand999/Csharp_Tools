if (args.Length < 2)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "files":
    case "f":
        if (args.Length < 3)
        {
            Console.WriteLine("Error: Two file paths required");
            ShowHelp();
            return;
        }
        DiffFiles(args[1], args[2]);
        break;
    case "text":
    case "t":
        if (args.Length < 3)
        {
            Console.WriteLine("Error: Two text strings required");
            ShowHelp();
            return;
        }
        DiffText(args[1], args[2]);
        break;
    case "distance":
    case "d":
        if (args.Length < 3)
        {
            Console.WriteLine("Error: Two strings required");
            ShowHelp();
            return;
        }
        ShowDistance(args[1], args[2]);
        break;
    default:
        // Assume file diff if two args provided
        DiffFiles(args[0], args[1]);
        break;
}

static void ShowHelp()
{
    Console.WriteLine("Text Diff Tool - Compare files and text strings");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project TextDiffTool.csproj <command> [arguments]");
    Console.WriteLine();
    Console.WriteLine("Commands:");
    Console.WriteLine("  files <file1> <file2>     Compare two files line by line");
    Console.WriteLine("  text <str1> <str2>        Compare two text strings");
    Console.WriteLine("  distance <str1> <str2>    Calculate Levenshtein distance");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project TextDiffTool.csproj files original.txt modified.txt");
    Console.WriteLine("  dotnet run --project TextDiffTool.csproj text \"hello\" \"hallo\"");
    Console.WriteLine("  dotnet run --project TextDiffTool.csproj distance \"kitten\" \"sitting\"");
}

static void DiffFiles(string file1, string file2)
{
    if (!File.Exists(file1))
    {
        Console.Error.WriteLine($"Error: File '{file1}' not found");
        return;
    }
    if (!File.Exists(file2))
    {
        Console.Error.WriteLine($"Error: File '{file2}' not found");
        return;
    }

    var lines1 = File.ReadAllLines(file1);
    var lines2 = File.ReadAllLines(file2);

    Console.WriteLine($"Comparing: {file1} vs {file2}");
    Console.WriteLine(new string('-', 60));
    
    var diff = ComputeDiff(lines1, lines2);
    
    Console.WriteLine($"\nStatistics:");
    Console.WriteLine($"  File 1: {lines1.Length} lines");
    Console.WriteLine($"  File 2: {lines2.Length} lines");
    Console.WriteLine($"  Added: {diff.Added} lines");
    Console.WriteLine($"  Removed: {diff.Removed} lines");
    Console.WriteLine($"  Unchanged: {diff.Unchanged} lines");
    
    Console.WriteLine($"\nSimilarity: {diff.Similarity:P2}");
    
    Console.WriteLine($"\nDiff Output:");
    Console.WriteLine(new string('-', 60));
    
    foreach (var change in diff.Changes)
    {
        switch (change.Type)
        {
            case ChangeType.Removed:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"- ");
                break;
            case ChangeType.Added:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"+ ");
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"  ");
                break;
        }
        Console.ResetColor();
        Console.WriteLine(change.Text);
    }
}

static void DiffText(string text1, string text2)
{
    var lines1 = text1.Split('\n');
    var lines2 = text2.Split('\n');
    
    Console.WriteLine("Text Comparison:");
    Console.WriteLine(new string('-', 60));
    Console.WriteLine($"Text 1: {text1.Length} chars, {lines1.Length} lines");
    Console.WriteLine($"Text 2: {text2.Length} chars, {lines2.Length} lines");
    
    var distance = LevenshteinDistance(text1, text2);
    var maxLen = Math.Max(text1.Length, text2.Length);
    var similarity = maxLen > 0 ? 1.0 - (double)distance / maxLen : 1.0;
    
    Console.WriteLine($"\nLevenshtein Distance: {distance}");
    Console.WriteLine($"Similarity: {similarity:P2}");
    
    Console.WriteLine($"\nLine-by-line diff:");
    Console.WriteLine(new string('-', 60));
    
    var diff = ComputeDiff(lines1, lines2);
    foreach (var change in diff.Changes)
    {
        switch (change.Type)
        {
            case ChangeType.Removed:
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"- ");
                break;
            case ChangeType.Added:
                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"+ ");
                break;
            default:
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.Write($"  ");
                break;
        }
        Console.ResetColor();
        Console.WriteLine(change.Text);
    }
}

static void ShowDistance(string str1, string str2)
{
    var distance = LevenshteinDistance(str1, str2);
    var maxLen = Math.Max(str1.Length, str2.Length);
    var similarity = maxLen > 0 ? 1.0 - (double)distance / maxLen : 1.0;
    
    Console.WriteLine($"String 1: \"{str1}\" ({str1.Length} chars)");
    Console.WriteLine($"String 2: \"{str2}\" ({str2.Length} chars)");
    Console.WriteLine();
    Console.WriteLine($"Levenshtein Distance: {distance}");
    Console.WriteLine($"Similarity: {similarity:P2}");
    
    // Show character-level changes
    Console.WriteLine($"\nCharacter-level analysis:");
    var charDiff = ComputeCharDiff(str1, str2);
    Console.WriteLine($"  Matching: {charDiff.Matching}");
    Console.WriteLine($"  Different: {charDiff.Different}");
    Console.WriteLine($"  Extra: {charDiff.Extra}");
}

static (List<Change> Changes, int Added, int Removed, int Unchanged, double Similarity) 
    ComputeDiff(string[] lines1, string[] lines2)
{
    var changes = new List<Change>();
    int added = 0, removed = 0, unchanged = 0;
    
    // Use LCS-based diff algorithm
    var lcs = FindLCS(lines1, lines2);
    
    int i = 0, j = 0, k = 0;
    while (i < lines1.Length || j < lines2.Length)
    {
        if (k < lcs.Count && i < lines1.Length && lines1[i] == lcs[k])
        {
            if (j < lines2.Length && lines2[j] == lcs[k])
            {
                changes.Add(new Change(ChangeType.Unchanged, lines1[i]));
                unchanged++;
                i++; j++; k++;
            }
            else
            {
                changes.Add(new Change(ChangeType.Removed, lines1[i]));
                removed++;
                i++;
            }
        }
        else if (j < lines2.Length)
        {
            if (k < lcs.Count && lines2[j] == lcs[k])
            {
                continue; // Will be handled above
            }
            changes.Add(new Change(ChangeType.Added, lines2[j]));
            added++;
            j++;
        }
        else if (i < lines1.Length)
        {
            changes.Add(new Change(ChangeType.Removed, lines1[i]));
            removed++;
            i++;
        }
    }
    
    var maxLines = Math.Max(lines1.Length, lines2.Length);
    var similarity = maxLines > 0 ? (double)unchanged / maxLines : 1.0;
    
    return (changes, added, removed, unchanged, similarity);
}

static List<string> FindLCS(string[] a, string[] b)
{
    int m = a.Length, n = b.Length;
    var dp = new int[m + 1, n + 1];
    
    for (int i = 1; i <= m; i++)
    {
        for (int j = 1; j <= n; j++)
        {
            if (a[i - 1] == b[j - 1])
                dp[i, j] = dp[i - 1, j - 1] + 1;
            else
                dp[i, j] = Math.Max(dp[i - 1, j], dp[i, j - 1]);
        }
    }
    
    // Backtrack to find LCS
    var lcs = new List<string>();
    int x = m, y = n;
    while (x > 0 && y > 0)
    {
        if (a[x - 1] == b[y - 1])
        {
            lcs.Insert(0, a[x - 1]);
            x--; y--;
        }
        else if (dp[x - 1, y] > dp[x, y - 1])
        {
            x--;
        }
        else
        {
            y--;
        }
    }
    
    return lcs;
}

static int LevenshteinDistance(string s1, string s2)
{
    int m = s1.Length, n = s2.Length;
    var dp = new int[m + 1, n + 1];
    
    for (int i = 0; i <= m; i++) dp[i, 0] = i;
    for (int j = 0; j <= n; j++) dp[0, j] = j;
    
    for (int i = 1; i <= m; i++)
    {
        for (int j = 1; j <= n; j++)
        {
            int cost = s1[i - 1] == s2[j - 1] ? 0 : 1;
            dp[i, j] = Math.Min(
                Math.Min(dp[i - 1, j] + 1, dp[i, j - 1] + 1),
                dp[i - 1, j - 1] + cost
            );
        }
    }
    
    return dp[m, n];
}

static (int Matching, int Different, int Extra) ComputeCharDiff(string s1, string s2)
{
    var chars1 = s1.ToLower().GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
    var chars2 = s2.ToLower().GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());
    
    int matching = 0, different = 0;
    
    foreach (var kvp in chars1)
    {
        if (chars2.TryGetValue(kvp.Key, out var count2))
        {
            matching += Math.Min(kvp.Value, count2);
            different += Math.Abs(kvp.Value - count2);
        }
        else
        {
            different += kvp.Value;
        }
    }
    
    int extra = chars2.Sum(kvp => kvp.Value) - matching;
    
    return (matching, different, Math.Max(0, extra));
}

record Change(ChangeType Type, string Text);
enum ChangeType { Unchanged, Added, Removed }
