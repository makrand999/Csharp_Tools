// Program #8: Operators - Arithmetic and comparison
// Demonstrates: Arithmetic, comparison, logical, and assignment operators

Console.WriteLine("=== Arithmetic Operators ===");
int a = 15, b = 4;

Console.WriteLine($"a = {a}, b = {b}");
Console.WriteLine($"a + b = {a + b}");      // Addition
Console.WriteLine($"a - b = {a - b}");      // Subtraction
Console.WriteLine($"a * b = {a * b}");      // Multiplication
Console.WriteLine($"a / b = {a / b}");      // Division (integer)
Console.WriteLine($"a % b = {a % b}");      // Modulus (remainder)
Console.WriteLine($"a++ = {a++}, then a = {a}");  // Post-increment
Console.WriteLine($"++a = {++a}, now a = {a}");   // Pre-increment

Console.WriteLine("\n=== Comparison Operators ===");
int x = 10, y = 20;
Console.WriteLine($"x = {x}, y = {y}");
Console.WriteLine($"x == y: {x == y}");     // Equal to
Console.WriteLine($"x != y: {x != y}");     // Not equal to
Console.WriteLine($"x > y:  {x > y}");      // Greater than
Console.WriteLine($"x < y:  {x < y}");      // Less than
Console.WriteLine($"x >= 10: {x >= 10}");   // Greater than or equal
Console.WriteLine($"y <= 20: {y <= 20}");   // Less than or equal

Console.WriteLine("\n=== Logical Operators ===");
bool isSunny = true;
bool isWarm = false;

Console.WriteLine($"isSunny = {isSunny}, isWarm = {isWarm}");
Console.WriteLine($"isSunny && isWarm: {isSunny && isWarm}");  // AND
Console.WriteLine($"isSunny || isWarm: {isSunny || isWarm}");  // OR
Console.WriteLine($"!isSunny: {!isSunny}");                     // NOT

// Complex logical expression
bool isNiceDay = isSunny && (isWarm || !isSunny);
Console.WriteLine($"isNiceDay (complex): {isNiceDay}");

Console.WriteLine("\n=== Assignment Operators ===");
int num = 10;
Console.WriteLine($"Initial: num = {num}");
num += 5;
Console.WriteLine($"num += 5  → {num}");   // Add and assign
num -= 3;
Console.WriteLine($"num -= 3  → {num}");   // Subtract and assign
num *= 2;
Console.WriteLine($"num *= 2  → {num}");   // Multiply and assign
num /= 4;
Console.WriteLine($"num /= 4  → {num}");   // Divide and assign
num %= 3;
Console.WriteLine($"num %= 3  → {num}");   // Modulus and assign

Console.WriteLine("\n=== Bitwise Operators ===");
int p = 5;  // Binary: 0101
int q = 3;  // Binary: 0011

Console.WriteLine($"p = {p} (binary: {Convert.ToString(p, 2).PadLeft(4, '0')})");
Console.WriteLine($"q = {q} (binary: {Convert.ToString(q, 2).PadLeft(4, '0')})");
Console.WriteLine($"p & q = {p & q} (AND)");
Console.WriteLine($"p | q = {p | q} (OR)");
Console.WriteLine($"p ^ q = {p ^ q} (XOR)");
Console.WriteLine($"~p = {~p} (NOT)");
Console.WriteLine($"p << 1 = {p << 1} (Left shift)");
Console.WriteLine($"p >> 1 = {p >> 1} (Right shift)");

Console.WriteLine("\n=== Ternary Operator ===");
int score = 85;
string result = score >= 50 ? "Pass" : "Fail";
Console.WriteLine($"Score: {score} → Result: {result}");

int grade = score >= 90 ? 1 : score >= 75 ? 2 : score >= 50 ? 3 : 4;
Console.WriteLine($"Score: {score} → Grade: {grade}");

Console.WriteLine("\n=== Null-coalescing Operators ===");
string? userInput = null;
string defaultName = "Guest";
string name = userInput ?? defaultName;
Console.WriteLine($"userInput ?? defaultName = \"{name}\"");

int? nullableNumber = null;
int number = nullableNumber ?? 0;
Console.WriteLine($"nullableNumber ?? 0 = {number}");

// Null-coalescing assignment
userInput ??= "Default User";
Console.WriteLine($"userInput ??= \"Default User\" → \"{userInput}\"");
