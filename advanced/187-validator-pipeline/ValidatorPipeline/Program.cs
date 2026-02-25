using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace ValidatorPipeline;

public record ValidationResult(string PropertyName, string ErrorMessage, bool IsValid);

public interface IValidator<T>
{
    IEnumerable<ValidationResult> Validate(T item);
}

public class User
{
    public string Username { get; set; } = "";
    public string Email { get; set; } = "";
    public string Password { get; set; } = "";
    public int Age { get; set; }
}

public class UserValidatorPipeline : IValidator<User>
{
    private readonly List<IValidator<User>> _validators = new();

    public UserValidatorPipeline AddValidator(IValidator<User> validator)
    {
        _validators.Add(validator);
        return this;
    }

    public IEnumerable<ValidationResult> Validate(User user)
    {
        var results = new List<ValidationResult>();
        foreach (var validator in _validators)
            results.AddRange(validator.Validate(user));
        return results;
    }
}

public class UsernameValidator : IValidator<User>
{
    public IEnumerable<ValidationResult> Validate(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Username))
            yield return new ValidationResult("Username", "Username is required", false);
        else if (user.Username.Length < 3)
            yield return new ValidationResult("Username", "Username must be at least 3 characters", false);
        else if (user.Username.Length > 20)
            yield return new ValidationResult("Username", "Username cannot exceed 20 characters", false);
        else if (!Regex.IsMatch(user.Username, @"^[a-zA-Z0-9_]+$"))
            yield return new ValidationResult("Username", "Username can only contain letters, numbers, and underscores", false);
    }
}

public class EmailValidator : IValidator<User>
{
    public IEnumerable<ValidationResult> Validate(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Email))
            yield return new ValidationResult("Email", "Email is required", false);
        else if (!Regex.IsMatch(user.Email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            yield return new ValidationResult("Email", "Invalid email format", false);
    }
}

public class PasswordValidator : IValidator<User>
{
    public IEnumerable<ValidationResult> Validate(User user)
    {
        if (string.IsNullOrWhiteSpace(user.Password))
            yield return new ValidationResult("Password", "Password is required", false);
        else if (user.Password.Length < 8)
            yield return new ValidationResult("Password", "Password must be at least 8 characters", false);
        else if (!Regex.IsMatch(user.Password, @"[A-Z]"))
            yield return new ValidationResult("Password", "Password must contain at least one uppercase letter", false);
        else if (!Regex.IsMatch(user.Password, @"[a-z]"))
            yield return new ValidationResult("Password", "Password must contain at least one lowercase letter", false);
        else if (!Regex.IsMatch(user.Password, @"[0-9]"))
            yield return new ValidationResult("Password", "Password must contain at least one number", false);
        else if (!Regex.IsMatch(user.Password, @"[!@#$%^&*]"))
            yield return new ValidationResult("Password", "Password must contain at least one special character (!@#$%^&*)", false);
    }
}

public class AgeValidator : IValidator<User>
{
    private readonly int _minAge;
    private readonly int _maxAge;

    public AgeValidator(int minAge = 13, int maxAge = 120)
    {
        _minAge = minAge;
        _maxAge = maxAge;
    }

    public IEnumerable<ValidationResult> Validate(User user)
    {
        if (user.Age < _minAge)
            yield return new ValidationResult("Age", $"You must be at least {_minAge} years old", false);
        else if (user.Age > _maxAge)
            yield return new ValidationResult("Age", $"Age cannot exceed {_maxAge} years", false);
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Validator Pipeline Demo - User Registration ===\n");

        var validatorPipeline = new UserValidatorPipeline()
            .AddValidator(new UsernameValidator())
            .AddValidator(new EmailValidator())
            .AddValidator(new PasswordValidator())
            .AddValidator(new AgeValidator());

        var testUsers = new List<User>
        {
            new() { Username = "ab", Email = "invalid", Password = "weak", Age = 10 },
            new() { Username = "john_doe", Email = "john@example.com", Password = "StrongP@ss1", Age = 25 },
            new() { Username = "user@name", Email = "test@test.com", Password = "NoSpecial1", Age = 30 },
            new() { Username = "", Email = "", Password = "", Age = 0 },
            new() { Username = "valid_user", Email = "valid@email.com", Password = "ValidP@ss123", Age = 150 },
        };

        foreach (var user in testUsers)
        {
            Console.WriteLine($"\n--- Validating User: '{user.Username}' ({user.Email}) ---");
            var results = validatorPipeline.Validate(user);
            var invalidResults = new List<ValidationResult>(results);

            if (invalidResults.Count == 0)
            {
                Console.WriteLine("  ✅ All validations passed!");
            }
            else
            {
                Console.WriteLine($"  ❌ {invalidResults.Count} validation(s) failed:");
                foreach (var result in invalidResults)
                    Console.WriteLine($"    - [{result.PropertyName}] {result.ErrorMessage}");
            }
        }

        Console.WriteLine("\n--- Individual Validator Tests ---");
        Console.WriteLine("Testing password validator with 'MyP@ssw0rd':");
        var passwordOnlyValidator = new PasswordValidator();
        var testUser = new User { Password = "MyP@ssw0rd" };
        var passwordResults = passwordOnlyValidator.Validate(testUser);
        Console.WriteLine(passwordResults.Any() ? "  ❌ Failed" : "  ✅ Passed");
    }
}
