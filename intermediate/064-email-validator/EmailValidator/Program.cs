using System.Net;
using System.Text.RegularExpressions;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var validator = new EmailValidatorService();
var command = args[0].ToLower();

switch (command)
{
    case "check":
    case "validate":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: Email address required");
            return;
        }
        validator.ValidateEmail(args[1]);
        break;
        
    case "batch":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: File path required");
            return;
        }
        validator.ValidateBatch(args[1], args.ElementAtOrDefault(2));
        break;
        
    case "extract":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: File path required");
            return;
        }
        validator.ExtractEmails(args[1]);
        break;
        
    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

void ShowHelp()
{
    Console.WriteLine("""
        Email Validator
        
        Usage:
          dotnet run --project EmailValidator/EmailValidator.csproj <command> [arguments]
        
        Commands:
          check <email>           Validate a single email address
          batch <file> [output]   Validate emails from file (one per line)
          extract <file>          Extract email addresses from text file
        
        Examples:
          dotnet run --project EmailValidator/EmailValidator.csproj check user@example.com
          dotnet run --project EmailValidator/EmailValidator.csproj batch emails.txt
          dotnet run --project EmailValidator/EmailValidator.csproj extract document.txt
        """);
}

class EmailValidatorService
{
    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase
    );
    
    private static readonly HashSet<string> DisposableDomains = new(StringComparer.OrdinalIgnoreCase)
    {
        "tempmail.com", "throwaway.com", "guerrillamail.com", "mailinator.com",
        "10minutemail.com", "fakeinbox.com", "trashmail.com", "temp-mail.org"
    };

    public void ValidateEmail(string email)
    {
        var result = AnalyzeEmail(email);
        PrintResult(result);
    }
    
    public void ValidateBatch(string inputFile, string? outputFile)
    {
        if (!File.Exists(inputFile))
        {
            Console.Error.WriteLine($"File not found: {inputFile}");
            return;
        }
        
        var emails = File.ReadAllLines(inputFile)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith('#'))
            .ToList();
        
        Console.WriteLine($"Validating {emails.Count} email addresses...\n");
        
        var results = emails.Select(AnalyzeEmail).ToList();
        var validCount = results.Count(r => r.IsValid);
        var invalidCount = results.Count(r => !r.IsValid);
        
        foreach (var result in results)
        {
            PrintResult(result, verbose: false);
        }
        
        Console.WriteLine($"\n--- Summary ---");
        Console.WriteLine($"Total: {emails.Count}");
        Console.WriteLine($"Valid: {validCount}");
        Console.WriteLine($"Invalid: {invalidCount}");
        Console.WriteLine($"Disposable: {results.Count(r => r.IsDisposable)}");
        
        if (!string.IsNullOrEmpty(outputFile))
        {
            var validEmails = results.Where(r => r.IsValid && !r.IsDisposable)
                .Select(r => r.Email);
            File.WriteAllLines(outputFile, validEmails);
            Console.WriteLine($"\nValid emails saved to: {outputFile}");
        }
    }
    
    public void ExtractEmails(string inputFile)
    {
        if (!File.Exists(inputFile))
        {
            Console.Error.WriteLine($"File not found: {inputFile}");
            return;
        }
        
        var content = File.ReadAllText(inputFile);
        var extractRegex = new Regex(
            @"[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}",
            RegexOptions.Compiled | RegexOptions.IgnoreCase
        );
        
        var matches = extractRegex.Matches(content);
        var uniqueEmails = matches.Select(m => m.Value).Distinct().ToList();
        
        Console.WriteLine($"Found {uniqueEmails.Count} unique email addresses:\n");
        foreach (var email in uniqueEmails)
        {
            var result = AnalyzeEmail(email);
            var status = result.IsValid ? "✓" : "✗";
            Console.WriteLine($"{status} {email}");
        }
    }
    
    private EmailResult AnalyzeEmail(string email)
    {
        var result = new EmailResult { Email = email };
        
        // Syntax check
        result.HasValidSyntax = EmailRegex.IsMatch(email);
        
        if (!result.HasValidSyntax)
        {
            result.Errors.Add("Invalid syntax");
            return result;
        }
        
        var parts = email.Split('@');
        result.LocalPart = parts[0];
        result.Domain = parts[1];
        
        // Domain checks
        if (!result.Domain.Contains('.'))
        {
            result.Errors.Add("Domain must have a TLD");
        }
        
        if (result.Domain.StartsWith(".") || result.Domain.EndsWith("."))
        {
            result.Errors.Add("Domain cannot start or end with a dot");
        }
        
        // Local part checks
        if (result.LocalPart.Length > 64)
        {
            result.Errors.Add("Local part exceeds 64 characters");
        }
        
        if (result.LocalPart.StartsWith(".") || result.LocalPart.EndsWith("."))
        {
            result.Errors.Add("Local part cannot start or end with a dot");
        }
        
        // Disposable check
        result.IsDisposable = DisposableDomains.Contains(result.Domain);
        if (result.IsDisposable)
        {
            result.Warnings.Add("Disposable email provider");
        }
        
        // Common typo detection
        var commonDomains = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { "gmial.com", "gmail.com" },
            { "gmai.com", "gmail.com" },
            { "yahooo.com", "yahoo.com" },
            { "yaho.com", "yahoo.com" },
            { "hotmial.com", "hotmail.com" },
            { "outlok.com", "outlook.com" }
        };
        
        if (commonDomains.TryGetValue(result.Domain, out var suggestion))
        {
            result.Warnings.Add($"Possible typo, did you mean {suggestion}?");
        }
        
        result.IsValid = result.HasValidSyntax && !result.Errors.Any();
        return result;
    }
    
    private void PrintResult(EmailResult result, bool verbose = true)
    {
        var status = result.IsValid ? "✓ Valid" : "✗ Invalid";
        Console.WriteLine($"{status}: {result.Email}");
        
        if (verbose)
        {
            if (result.IsDisposable)
                Console.WriteLine($"  ⚠ Warning: Disposable email provider");
            
            foreach (var warning in result.Warnings)
                Console.WriteLine($"  ⚠ Warning: {warning}");
            
            foreach (var error in result.Errors)
                Console.WriteLine($"  ✗ Error: {error}");
            
            if (result.IsValid && !result.Warnings.Any())
                Console.WriteLine("  Email appears valid");
        }
    }
}

class EmailResult
{
    public string Email { get; set; } = "";
    public string LocalPart { get; set; } = "";
    public string Domain { get; set; } = "";
    public bool IsValid { get; set; }
    public bool HasValidSyntax { get; set; }
    public bool IsDisposable { get; set; }
    public List<string> Errors { get; set; } = new();
    public List<string> Warnings { get; set; } = new();
}
