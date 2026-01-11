using System;
using System.Collections.Generic;
using System.Linq;

namespace PasswordStrengthChecker;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Enter a password to check (or 'quit' to exit):");
            InteractiveMode();
        }
        else
        {
            string password = string.Join(" ", args);
            CheckPassword(password);
        }
    }

    static void InteractiveMode()
    {
        while (true)
        {
            Console.Write("> ");
            string? input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input) || input.ToLowerInvariant() == "quit")
                break;
            
            CheckPassword(input);
            Console.WriteLine();
        }
    }

    static void CheckPassword(string password)
    {
        var result = AnalyzePassword(password);
        
        Console.WriteLine($"Password: {MaskPassword(password)}");
        Console.WriteLine($"Length: {password.Length} characters");
        Console.WriteLine($"Strength: {result.Strength} ({result.Score}/100)");
        Console.WriteLine();
        
        Console.WriteLine("Criteria:");
        Console.WriteLine($"  {(result.HasMinLength ? "✓" : "✗")} Minimum length (8+ characters)");
        Console.WriteLine($"  {(result.HasUppercase ? "✓" : "✗")} Uppercase letter");
        Console.WriteLine($"  {(result.HasLowercase ? "✓" : "✗")} Lowercase letter");
        Console.WriteLine($"  {(result.HasDigit ? "✓" : "✗")} Number");
        Console.WriteLine($"  {(result.HasSpecial ? "✓" : "✗")} Special character");
        
        if (result.Issues.Count > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Recommendations:");
            foreach (string issue in result.Issues)
            {
                Console.WriteLine($"  • {issue}");
            }
        }
    }

    static string MaskPassword(string password)
    {
        if (password.Length <= 4)
            return new string('*', password.Length);
        return password[..2] + new string('*', password.Length - 4) + password[^2..];
    }

    static PasswordAnalysis AnalyzePassword(string password)
    {
        var issues = new List<string>();
        int score = 0;

        bool hasMinLength = password.Length >= 8;
        bool hasUppercase = password.Any(char.IsUpper);
        bool hasLowercase = password.Any(char.IsLower);
        bool hasDigit = password.Any(char.IsDigit);
        bool hasSpecial = password.Any(c => !char.IsLetterOrDigit(c));

        // Length scoring
        if (hasMinLength)
            score += 20;
        else
            issues.Add("Password should be at least 8 characters long");
        
        if (password.Length >= 12)
            score += 10;
        if (password.Length >= 16)
            score += 10;

        // Character variety scoring
        if (hasUppercase) score += 15;
        else issues.Add("Add uppercase letters (A-Z)");
        
        if (hasLowercase) score += 15;
        else issues.Add("Add lowercase letters (a-z)");
        
        if (hasDigit) score += 15;
        else issues.Add("Add numbers (0-9)");
        
        if (hasSpecial) score += 15;
        else issues.Add("Add special characters (!@#$%^&*...)");

        // Bonus for using all character types
        int typesUsed = new[] { hasUppercase, hasLowercase, hasDigit, hasSpecial }.Count(b => b);
        if (typesUsed == 4)
            score += 10;

        // Cap score at 100
        score = Math.Min(score, 100);

        string strength = score switch
        {
            >= 80 => "Strong",
            >= 60 => "Good",
            >= 40 => "Fair",
            _ => "Weak"
        };

        return new PasswordAnalysis
        {
            Score = score,
            Strength = strength,
            HasMinLength = hasMinLength,
            HasUppercase = hasUppercase,
            HasLowercase = hasLowercase,
            HasDigit = hasDigit,
            HasSpecial = hasSpecial,
            Issues = issues
        };
    }
}

record PasswordAnalysis
{
    public int Score { get; init; }
    public string Strength { get; init; } = "";
    public bool HasMinLength { get; init; }
    public bool HasUppercase { get; init; }
    public bool HasLowercase { get; init; }
    public bool HasDigit { get; init; }
    public bool HasSpecial { get; init; }
    public List<string> Issues { get; init; } = new();
}
