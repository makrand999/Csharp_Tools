// Program #13: Strings - Demonstrates string manipulation, interpolation, and common methods

Console.WriteLine("=== String Operations in C# ===\n");

// String declaration
string greeting = "Hello, World!";
string name = "C# Developer";

// String interpolation
Console.WriteLine($"=== String Interpolation ===");
Console.WriteLine($"{greeting} Welcome, {name}!");

// Multi-line string (verbatim string)
string multiLine = """
    This is a multi-line string.
    It preserves formatting and indentation.
    Very useful for templates and messages.
    """;
Console.WriteLine($"\n{multiLine}");

// String concatenation
Console.WriteLine($"\n=== Concatenation ===");
string firstName = "John";
string lastName = "Doe";
string fullName = firstName + " " + lastName;
Console.WriteLine($"Full name: {fullName}");

// String methods
Console.WriteLine($"\n=== String Methods ===");
string text = "  Hello, C# World!  ";
Console.WriteLine($"Original: '{text}'");
Console.WriteLine($"Length: {text.Length}");
Console.WriteLine($"Trimmed: '{text.Trim()}'");
Console.WriteLine($"ToLower: '{text.ToLower()}'");
Console.WriteLine($"ToUpper: '{text.ToUpper()}'");

// Substring
Console.WriteLine($"\n=== Substring ===");
string sentence = "The quick brown fox jumps over the lazy dog";
Console.WriteLine($"Original: {sentence}");
Console.WriteLine($"First 9 chars: {sentence.Substring(0, 9)}");
Console.WriteLine($"From index 16: {sentence.Substring(16)}");

// IndexOf and LastIndexOf
Console.WriteLine($"\n=== Searching ===");
Console.WriteLine($"Index of 'fox': {sentence.IndexOf("fox")}");
Console.WriteLine($"Last index of 'o': {sentence.LastIndexOf('o')}");
Console.WriteLine($"Contains 'quick': {sentence.Contains("quick")}");
Console.WriteLine($"Starts with 'The': {sentence.StartsWith("The")}");
Console.WriteLine($"Ends with 'dog': {sentence.EndsWith("dog")}");

// Replace
Console.WriteLine($"\n=== Replace ===");
string replaced = sentence.Replace("quick", "slow").Replace("dog", "cat");
Console.WriteLine(replaced);

// Split and Join
Console.WriteLine($"\n=== Split and Join ===");
string csv = "apple,banana,cherry,date";
string[] fruits = csv.Split(',');
Console.WriteLine($"CSV: {csv}");
Console.WriteLine($"Split into array: [{string.Join(", ", fruits)}]");

// Join array back to string
string joined = string.Join(" | ", fruits);
Console.WriteLine($"Joined with ' | ': {joined}");

// String formatting
Console.WriteLine($"\n=== String Formatting ===");
double price = 49.99;
int quantity = 3;
DateTime today = DateTime.Now;

Console.WriteLine($"Price: {price:C}");           // Currency format
Console.WriteLine($"Quantity: {quantity:D3}");     // Decimal with leading zeros
Console.WriteLine($"Date: {today:d}");             // Short date
Console.WriteLine($"Date: {today:yyyy-MM-dd}");    // Custom format
Console.WriteLine($"Total: {price * quantity:C}");

// PadLeft and PadRight
Console.WriteLine($"\n=== Padding ===");
Console.WriteLine($"|{"Right",-10}|");  // Left-aligned in 10 chars
Console.WriteLine($"|{"Left",10}|");    // Right-aligned in 10 chars

// String comparison
Console.WriteLine($"\n=== Comparison ===");
string str1 = "Hello";
string str2 = "hello";
Console.WriteLine($"\"{str1}\" == \"{str2}\": {str1 == str2}");
Console.WriteLine($"Equals (ignore case): {str1.Equals(str2, StringComparison.OrdinalIgnoreCase)}");

// Character operations
Console.WriteLine($"\n=== Character Operations ===");
string word = "CSharp";
foreach (char c in word)
{
    Console.WriteLine($"'{c}': {(char.IsLetter(c) ? "Letter" : "Other")}");
}

// String builder for performance (brief demo)
Console.WriteLine($"\n=== StringBuilder ===");
var sb = new System.Text.StringBuilder();
for (int i = 1; i <= 5; i++)
{
    sb.Append($"Step {i}, ");
}
Console.WriteLine($"Built string: {sb.ToString().TrimEnd(',', ' ')}");
