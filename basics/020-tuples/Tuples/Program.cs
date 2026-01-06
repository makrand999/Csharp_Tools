// Program 20: Tuples - Demonstrates tuple creation, access, and deconstruction
// Topics: ValueTuple, named elements, deconstruction, method returns

Console.WriteLine("=== Tuple Basics ===\n");

// Creating tuples
var person = ("Alice", 30, "Engineer");
Console.WriteLine("Simple tuple:");
Console.WriteLine($"  Item1: {person.Item1}");
Console.WriteLine($"  Item2: {person.Item2}");
Console.WriteLine($"  Item3: {person.Item3}");

// Named tuples
Console.WriteLine("\n--- Named Tuples ---");
var employee = (Name: "Bob", Age: 25, Role: "Designer");
Console.WriteLine($"Employee: {employee.Name}, {employee.Age}, {employee.Role}");

// Creating tuple with explicit syntax
Console.WriteLine("\n--- Explicit Tuple Creation ---");
var point = new ValueTuple<int, int>(10, 20);
Console.WriteLine($"Point: X={point.Item1}, Y={point.Item2}");

// Tuple deconstruction
Console.WriteLine("\n--- Tuple Deconstruction ---");
var (name, age, role) = employee;
Console.WriteLine($"Deconstructed: Name={name}, Age={age}, Role={role}");

// Deconstruction with discards
Console.WriteLine("\n--- Deconstruction with Discards ---");
var (_, onlyAge, _) = employee;
Console.WriteLine($"Only extracting age: {onlyAge}");

// Swapping values using tuples
Console.WriteLine("\n--- Swapping Values ---");
int x = 5, y = 10;
Console.WriteLine($"Before swap: x={x}, y={y}");
(x, y) = (y, x);
Console.WriteLine($"After swap: x={x}, y={y}");

// Tuples as method return values
Console.WriteLine("\n=== Tuples as Method Returns ===\n");

var divisionResult = Divide(100, 7);
Console.WriteLine($"Divide 100 by 7:");
Console.WriteLine($"  Quotient: {divisionResult.Quotient}");
Console.WriteLine($"  Remainder: {divisionResult.Remainder}");

var stats = CalculateStats(85, 90, 78, 92, 88);
Console.WriteLine($"\nTest Scores Statistics:");
Console.WriteLine($"  Average: {stats.Average:F2}");
Console.WriteLine($"  Min: {stats.Min}");
Console.WriteLine($"  Max: {stats.Max}");

// Tuple comparison
Console.WriteLine("\n=== Tuple Comparison ===\n");

var tuple1 = (1, 2, 3);
var tuple2 = (1, 2, 3);
var tuple3 = (1, 2, 4);

Console.WriteLine($"tuple1 = {tuple1}");
Console.WriteLine($"tuple2 = {tuple2}");
Console.WriteLine($"tuple3 = {tuple3}");
Console.WriteLine($"\ntuple1 == tuple2: {tuple1 == tuple2}");
Console.WriteLine($"tuple1 == tuple3: {tuple1 == tuple3}");
Console.WriteLine($"tuple1 < tuple3: {tuple1.CompareTo(tuple3)}");

// Nested tuples
Console.WriteLine("\n=== Nested Tuples ===\n");

var rectangle = (TopLeft: (x: 0, y: 0), BottomRight: (x: 100, y: 50));
Console.WriteLine($"Rectangle:");
Console.WriteLine($"  Top-Left: ({rectangle.TopLeft.x}, {rectangle.TopLeft.y})");
Console.WriteLine($"  Bottom-Right: ({rectangle.BottomRight.x}, {rectangle.BottomRight.y})");

// Tuple with arrays
Console.WriteLine("\n=== Tuple with Arrays ===\n");

var coordinates = new (int X, int Y)[]
{
    (0, 0),
    (1, 1),
    (2, 2),
    (3, 3)
};

Console.WriteLine("Coordinate points:");
foreach (var coord in coordinates)
{
    Console.WriteLine($"  Point: X={coord.X}, Y={coord.Y}");
}

// Real-world example: Multiple return values
Console.WriteLine("\n=== Real-World: Weather Data ===\n");

var weatherData = GetWeatherData("Seattle");
Console.WriteLine($"Weather in {weatherData.City}:");
Console.WriteLine($"  Temperature: {weatherData.TempHigh}°C / {weatherData.TempLow}°C");
Console.WriteLine($"  Condition: {weatherData.Condition}");
Console.WriteLine($"  Humidity: {weatherData.Humidity}%");

// Real-world example: Database record simulation
Console.WriteLine("\n=== Real-World: Database Records ===\n");

var users = new List<(int Id, string Username, string Email, bool IsActive)>
{
    (1, "alice", "alice@example.com", true),
    (2, "bob", "bob@example.com", true),
    (3, "charlie", "charlie@example.com", false),
};

Console.WriteLine("User records:");
foreach (var user in users)
{
    Console.WriteLine($"  [{user.Id}] {user.Username} - {user.Email} - Active: {user.IsActive}");
}

// Filter active users
var activeUsers = users.Where(u => u.IsActive).ToList();
Console.WriteLine($"\nActive users ({activeUsers.Count}):");
foreach (var user in activeUsers)
{
    Console.WriteLine($"  - {user.Username}");
}

Console.WriteLine("\n=== Program Complete ===");

// Method returning named tuple
static (int Quotient, int Remainder) Divide(int dividend, int divisor)
{
    return (dividend / divisor, dividend % divisor);
}

// Method returning tuple with multiple statistics
static (double Average, int Min, int Max) CalculateStats(params int[] values)
{
    return (values.Average(), values.Min(), values.Max());
}

// Method simulating database/API response
static (string City, int TempHigh, int TempLow, string Condition, int Humidity) GetWeatherData(string city)
{
    return city.ToLower() switch
    {
        "seattle" => ("Seattle", 22, 15, "Cloudy", 78),
        "miami" => ("Miami", 32, 26, "Sunny", 85),
        "denver" => ("Denver", 18, 8, "Snowy", 45),
        _ => ("Unknown", 20, 10, "Unknown", 50)
    };
}
