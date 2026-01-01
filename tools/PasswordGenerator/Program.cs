// Password Generator - Generates secure random passwords
class Program
{
    static void Main(string[] args)
    {
        int length = 16;
        bool useUppercase = true;
        bool useLowercase = true;
        bool useDigits = true;
        bool useSpecial = true;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i].ToLower())
            {
                case "-l":
                case "--length":
                    if (i + 1 < args.Length && int.TryParse(args[i + 1], out int l))
                        length = l;
                    break;
                case "--no-upper":
                    useUppercase = false;
                    break;
                case "--no-lower":
                    useLowercase = false;
                    break;
                case "--no-digits":
                    useDigits = false;
                    break;
                case "--no-special":
                    useSpecial = false;
                    break;
                case "-h":
                case "--help":
                    PrintHelp();
                    return;
            }
        }

        if (length < 4) length = 4;
        if (length > 128) length = 128;

        string password = GeneratePassword(length, useUppercase, useLowercase, useDigits, useSpecial);
        Console.WriteLine($"Generated Password: {password}");
    }

    static string GeneratePassword(int length, bool upper, bool lower, bool digits, bool special)
    {
        const string uppercase = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string lowercase = "abcdefghijklmnopqrstuvwxyz";
        const string numbers = "0123456789";
        const string symbols = "!@#$%^&*()_+-=[]{}|;:,.<>?";

        string chars = "";
        if (upper) chars += uppercase;
        if (lower) chars += lowercase;
        if (digits) chars += numbers;
        if (special) chars += symbols;

        if (string.IsNullOrEmpty(chars))
        {
            Console.WriteLine("Error: At least one character type must be selected.");
            return "";
        }

        var random = new Random();
        var password = new char[length];

        // Ensure at least one of each selected type
        int idx = 0;
        if (upper) password[idx++] = uppercase[random.Next(uppercase.Length)];
        if (lower) password[idx++] = lowercase[random.Next(lowercase.Length)];
        if (digits) password[idx++] = numbers[random.Next(numbers.Length)];
        if (special) password[idx++] = symbols[random.Next(symbols.Length)];

        // Fill the rest randomly
        for (int i = idx; i < length; i++)
        {
            password[i] = chars[random.Next(chars.Length)];
        }

        // Shuffle the password
        return new string(password.OrderBy(c => random.Next()).ToArray());
    }

    static void PrintHelp()
    {
        Console.WriteLine(@"
Password Generator - Generate secure random passwords

Usage: PasswordGenerator [options]

Options:
  -l, --length <n>     Password length (default: 16, range: 4-128)
  --no-upper           Exclude uppercase letters
  --no-lower           Exclude lowercase letters
  --no-digits          Exclude digits
  --no-special         Exclude special characters
  -h, --help           Show this help message

Examples:
  PasswordGenerator                      # Generate 16-char password
  PasswordGenerator -l 32                # Generate 32-char password
  PasswordGenerator --no-special -l 12   # Generate 12-char, no symbols
");
    }
}
