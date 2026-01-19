# Email Validator

CLI tool for validating email addresses with syntax checking, disposable detection, and typo suggestions.

## Usage

```bash
# Validate single email
dotnet run --project EmailValidator/EmailValidator.csproj check user@example.com

# Batch validate from file
dotnet run --project EmailValidator/EmailValidator.csproj batch emails.txt valid-emails.txt

# Extract emails from text file
dotnet run --project EmailValidator/EmailValidator.csproj extract document.txt
```

## Example

```
$ dotnet run --project EmailValidator/EmailValidator.csproj check user@gmial.com
✓ Valid: user@gmial.com
  ⚠ Warning: Possible typo, did you mean gmail.com?
  Email appears valid

$ dotnet run --project EmailValidator/EmailValidator.csproj check test@tempmail.com
✓ Valid: test@tempmail.com
  ⚠ Warning: Disposable email provider
```

## Features

- **Syntax validation** - RFC-compliant email format checking
- **Disposable detection** - Identifies temporary email providers
- **Typo detection** - Suggests corrections for common domain typos
- **Batch processing** - Validate hundreds of emails from a file
- **Email extraction** - Pull email addresses from any text file

## Concepts Demonstrated

- Regular expressions for pattern matching
- HashSet for efficient lookups
- File I/O operations
- Command-line argument parsing
- Data validation patterns
- String manipulation
