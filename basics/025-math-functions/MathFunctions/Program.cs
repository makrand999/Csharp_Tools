// Program 25: Math Functions - Demonstrates Math class, random numbers, calculations
// Topics: Math class methods, Random class, rounding, trigonometry, statistics

Console.WriteLine("=== Math Class Basics ===\n");

// Absolute value
Console.WriteLine("--- Absolute Value ---");
Console.WriteLine($"Math.Abs(-42) = {Math.Abs(-42)}");
Console.WriteLine($"Math.Abs(42) = {Math.Abs(42)}");
Console.WriteLine($"Math.Abs(-3.14) = {Math.Abs(-3.14)}");

// Min and Max
Console.WriteLine("\n--- Min and Max ---");
Console.WriteLine($"Math.Min(10, 20) = {Math.Min(10, 20)}");
Console.WriteLine($"Math.Max(10, 20) = {Math.Max(10, 20)}");
Console.WriteLine($"Math.Min(-5, 5) = {Math.Min(-5, 5)}");

// Power and Square Root
Console.WriteLine("\n--- Power and Square Root ---");
Console.WriteLine($"Math.Pow(2, 10) = {Math.Pow(2, 10)}");
Console.WriteLine($"Math.Pow(3, 4) = {Math.Pow(3, 4)}");
Console.WriteLine($"Math.Sqrt(144) = {Math.Sqrt(144)}");
Console.WriteLine($"Math.Sqrt(2) = {Math.Sqrt(2):F6}");

// Rounding
Console.WriteLine("\n--- Rounding ---");
Console.WriteLine($"Math.Round(3.14159, 2) = {Math.Round(3.14159, 2)}");
Console.WriteLine($"Math.Round(3.5) = {Math.Round(3.5)}");
Console.WriteLine($"Math.Round(4.5) = {Math.Round(4.5)}"); // Banker's rounding
Console.WriteLine($"Math.Floor(3.7) = {Math.Floor(3.7)}");
Console.WriteLine($"Math.Floor(-3.7) = {Math.Floor(-3.7)}");
Console.WriteLine($"Math.Ceiling(3.2) = {Math.Ceiling(3.2)}");
Console.WriteLine($"Math.Ceiling(-3.2) = {Math.Ceiling(-3.2)}");

// Trigonometry
Console.WriteLine("\n--- Trigonometry ---");
double angle = Math.PI / 4; // 45 degrees
Console.WriteLine($"Angle: {angle:F4} radians ({angle * 180 / Math.PI:F0} degrees)");
Console.WriteLine($"Math.Sin(π/4) = {Math.Sin(angle):F4}");
Console.WriteLine($"Math.Cos(π/4) = {Math.Cos(angle):F4}");
Console.WriteLine($"Math.Tan(π/4) = {Math.Tan(angle):F4}");

// Converting degrees to radians
double degrees = 60;
double radians = degrees * Math.PI / 180;
Console.WriteLine($"\n{degrees} degrees = {radians:F4} radians");
Console.WriteLine($"Math.Sin({degrees}°) = {Math.Sin(radians):F4}");

// Logarithms
Console.WriteLine("\n--- Logarithms ---");
Console.WriteLine($"Math.Log(Math.E) = {Math.Log(Math.E)}"); // Natural log
Console.WriteLine($"Math.Log10(100) = {Math.Log10(100)}");
Console.WriteLine($"Math.Log10(1000) = {Math.Log10(1000)}");
Console.WriteLine($"Math.Log2(256) = {Math.Log2(256)}");

// Constants
Console.WriteLine("\n--- Math Constants ---");
Console.WriteLine($"Math.PI = {Math.PI:F10}");
Console.WriteLine($"Math.E = {Math.E:F10}");
Console.WriteLine($"Math.Tau = {Math.Tau:F10}");

// Random Number Generation
Console.WriteLine("\n=== Random Number Generation ===\n");

var random = new Random();

// Random integers
Console.WriteLine("--- Random Integers ---");
Console.WriteLine($"Random int (0 to MaxValue): {random.Next()}");
Console.WriteLine($"Random int (0 to 100): {random.Next(100)}");
Console.WriteLine($"Random int (50 to 100): {random.Next(50, 101)}");

// Random doubles
Console.WriteLine("\n--- Random Doubles ---");
Console.WriteLine($"Random double [0.0 to 1.0): {random.NextDouble():F4}");
Console.WriteLine($"Random double [0.0 to 1.0): {random.NextDouble():F4}");
Console.WriteLine($"Random double [0.0 to 1.0): {random.NextDouble():F4}");

// Random in range (double)
double min = 1.5;
double max = 5.5;
double randomInRange = min + random.NextDouble() * (max - min);
Console.WriteLine($"Random double [{min} to {max}): {randomInRange:F4}");

// Simulating dice roll
Console.WriteLine("\n--- Simulating Dice Roll ---");
Console.WriteLine("Rolling 2 dice 5 times:");
for (int i = 0; i < 5; i++)
{
    int die1 = random.Next(1, 7);
    int die2 = random.Next(1, 7);
    Console.WriteLine($"  Roll {i + 1}: [{die1}] + [{die2}] = {die1 + die2}");
}

// Coin flip simulation
Console.WriteLine("\n--- Coin Flip Simulation ---");
Console.WriteLine("Flipping coin 10 times:");
int heads = 0;
int tails = 0;
for (int i = 0; i < 10; i++)
{
    bool isHeads = random.Next(2) == 0;
    if (isHeads) heads++; else tails++;
    Console.Write($"{(isHeads ? "H" : "T")} ");
}
Console.WriteLine($"\nResult: {heads} Heads, {tails} Tails");

// Statistics with Math
Console.WriteLine("\n=== Statistics Calculations ===\n");

double[] values = { 12.5, 15.3, 18.7, 22.1, 25.9, 30.2, 35.8, 40.1 };

Console.WriteLine($"Data: [{string.Join(", ", values)}]");
Console.WriteLine($"\nCount: {values.Length}");
Console.WriteLine($"Min: {values.Min()}");
Console.WriteLine($"Max: {values.Max()}");
Console.WriteLine($"Sum: {values.Sum():F2}");
Console.WriteLine($"Average: {values.Average():F2}");

// Standard deviation
double mean = values.Average();
double variance = values.Average(v => Math.Pow(v - mean, 2));
double stdDev = Math.Sqrt(variance);
Console.WriteLine($"Standard Deviation: {stdDev:F2}");

// Median
var sorted = values.OrderBy(v => v).ToArray();
int mid = sorted.Length / 2;
double median = sorted.Length % 2 == 0
    ? (sorted[mid - 1] + sorted[mid]) / 2
    : sorted[mid];
Console.WriteLine($"Median: {median:F2}");

// Geometry calculations
Console.WriteLine("\n=== Geometry Calculations ===\n");

// Circle
double radius = 5;
Console.WriteLine($"Circle with radius {radius}:");
Console.WriteLine($"  Circumference: {2 * Math.PI * radius:F2}");
Console.WriteLine($"  Area: {Math.PI * radius * radius:F2}");

// Sphere
Console.WriteLine($"\nSphere with radius {radius}:");
Console.WriteLine($"  Surface Area: {4 * Math.PI * radius * radius:F2}");
Console.WriteLine($"  Volume: {4.0 / 3.0 * Math.PI * Math.Pow(radius, 3):F2}");

// Distance between two points
Console.WriteLine("\n--- Distance Between Points ---");
double x1 = 0, y1 = 0;
double x2 = 3, y2 = 4;
double distance = Math.Sqrt(Math.Pow(x2 - x1, 2) + Math.Pow(y2 - y1, 2));
Console.WriteLine($"Distance from ({x1}, {y1}) to ({x2}, {y2}): {distance}");

// Hypotenuse
double hypotenuse = Math.Sqrt(3 * 3 + 4 * 4);
Console.WriteLine($"Hypotenuse of right triangle (3, 4): {hypotenuse}");

// Clamp value (constrain to range)
Console.WriteLine("\n--- Clamp Value to Range ---");
double valueToClamp = 150;
double clamped = Math.Max(0, Math.Min(100, valueToClamp));
Console.WriteLine($"Clamp {valueToClamp} to [0, 100]: {clamped}");

valueToClamp = 50;
clamped = Math.Max(0, Math.Min(100, valueToClamp));
Console.WriteLine($"Clamp {valueToClamp} to [0, 100]: {clamped}");

Console.WriteLine("\n=== Program Complete ===");
