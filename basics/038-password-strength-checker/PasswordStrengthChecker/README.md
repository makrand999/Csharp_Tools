# Password Strength Checker

A CLI tool that analyzes password strength and provides recommendations for improvement.

## Usage

```bash
# Interactive mode
dotnet run --project PasswordStrengthChecker.csproj

# Single password check
dotnet run --project PasswordStrengthChecker.csproj "YourPassword123!"
```

## Examples

```bash
# Check a password
dotnet run --project PasswordStrengthChecker.csproj "MyPass123"
# Output:
# Password: My****123
# Length: 9 characters
# Strength: Fair (55/100)
#
# Criteria:
#   ✓ Minimum length (8+ characters)
#   ✓ Uppercase letter
#   ✓ Lowercase letter
#   ✓ Number
#   ✗ Special character
#
# Recommendations:
#   • Add special characters (!@#$%^&*...)
```

## Features

- Interactive mode for testing multiple passwords
- Command-line argument mode for single checks
- Password masking for security
- Detailed criteria breakdown
- Actionable recommendations

## Concepts Demonstrated

- LINQ queries for character analysis
- Pattern matching with switch expressions
- Record types for immutable data
- Interactive console I/O
- String manipulation and masking
