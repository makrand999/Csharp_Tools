// Program #9: Conditionals - If/else and switch statements
// Demonstrates: if/else, nested if, switch, switch expressions, pattern matching

Console.WriteLine("=== If/Else Statements ===");

// Simple if statement
int age = 18;
Console.WriteLine($"Age: {age}");

if (age >= 18)
{
    Console.WriteLine("You are an adult.");
}

// if/else statement
int temperature = 25;
Console.WriteLine($"\nTemperature: {temperature}°C");

if (temperature > 30)
{
    Console.WriteLine("It's hot outside!");
}
else if (temperature > 20)
{
    Console.WriteLine("It's warm and pleasant.");
}
else if (temperature > 10)
{
    Console.WriteLine("It's a bit chilly.");
}
else
{
    Console.WriteLine("It's cold! Wear a jacket.");
}

// Nested if statements
int score = 85;
bool isMember = true;

Console.WriteLine($"\nScore: {score}, Member: {isMember}");

if (score >= 50)
{
    Console.WriteLine("You passed!");
    
    if (isMember)
    {
        Console.WriteLine("Member bonus applied! +10 points");
        score += 10;
        Console.WriteLine($"Final score: {score}");
    }
    
    if (score >= 90)
    {
        Console.WriteLine("Grade: A - Excellent!");
    }
    else if (score >= 75)
    {
        Console.WriteLine("Grade: B - Very Good!");
    }
    else
    {
        Console.WriteLine("Grade: C - Good!");
    }
}
else
{
    Console.WriteLine("You failed. Better luck next time!");
}

Console.WriteLine("\n=== Switch Statement ===");

// Classic switch statement
int dayNumber = 3;
string dayName;

switch (dayNumber)
{
    case 1:
        dayName = "Monday";
        break;
    case 2:
        dayName = "Tuesday";
        break;
    case 3:
        dayName = "Wednesday";
        break;
    case 4:
        dayName = "Thursday";
        break;
    case 5:
        dayName = "Friday";
        break;
    case 6:
        dayName = "Saturday";
        break;
    case 7:
        dayName = "Sunday";
        break;
    default:
        dayName = "Invalid day";
        break;
}

Console.WriteLine($"Day {dayNumber}: {dayName}");

// Switch with multiple cases
char grade = 'B';
string feedback;

switch (grade)
{
    case 'A':
    case 'a':
        feedback = "Outstanding!";
        break;
    case 'B':
    case 'b':
        feedback = "Great job!";
        break;
    case 'C':
    case 'c':
        feedback = "Good work!";
        break;
    case 'D':
    case 'd':
        feedback = "Needs improvement.";
        break;
    case 'F':
    case 'f':
        feedback = "Failed. Please retry.";
        break;
    default:
        feedback = "Invalid grade.";
        break;
}

Console.WriteLine($"\nGrade {grade}: {feedback}");

Console.WriteLine("\n=== Switch Expression (C# 8+) ===");

// Switch expression - more concise
string result = dayNumber switch
{
    1 => "Monday",
    2 => "Tuesday",
    3 => "Wednesday",
    4 => "Thursday",
    5 => "Friday",
    6 => "Saturday",
    7 => "Sunday",
    _ => "Invalid day"
};

Console.WriteLine($"Day {dayNumber} (switch expression): {result}");

// Switch expression with pattern matching
string message = grade switch
{
    'A' or 'a' => "Outstanding!",
    'B' or 'b' => "Great job!",
    'C' or 'c' => "Good work!",
    'D' or 'd' => "Needs improvement.",
    'F' or 'f' => "Failed. Please retry.",
    _ => "Invalid grade."
};

Console.WriteLine($"Grade {grade} (switch expression): {message}");

Console.WriteLine("\n=== Pattern Matching with if ===");

// Type pattern matching
object data = "Hello, World!";

if (data is string text)
{
    Console.WriteLine($"\nData is a string: \"{text}\" (Length: {text.Length})");
}
else if (data is int number)
{
    Console.WriteLine($"\nData is an integer: {number}");
}
else if (data is double decimalNum)
{
    Console.WriteLine($"\nData is a double: {decimalNum}");
}

// Property pattern matching
var person = new { Name = "Alice", Age = 25, City = "London" };

if (person is { Age: >= 18, City: "London" })
{
    Console.WriteLine($"\n{person.Name} is an adult living in London.");
}

// Null check with pattern matching
string? userInput = null;

if (userInput is null)
{
    Console.WriteLine("\nUser input is null.");
}
else if (userInput is not null && userInput.Length > 0)
{
    Console.WriteLine($"\nUser input: \"{userInput}\"");
}
else
{
    Console.WriteLine("\nUser input is empty.");
}

Console.WriteLine("\n=== Conditional (Ternary) Operator ===");

int number1 = 10, number2 = 20;
int max = number1 > number2 ? number1 : number2;
Console.WriteLine($"Max of {number1} and {number2}: {max}");

string status = (number1 + number2) > 25 ? "High" : "Low";
Console.WriteLine($"Sum status: {status}");
