// Program 21: Enums - Demonstrates enumeration definition, usage, and flags
// Topics: Enum basics, underlying values, parsing, flags

Console.WriteLine("=== Enum Basics ===\n");

// Using enum values
DayOfWeek today = DayOfWeek.Tuesday;
Console.WriteLine($"Today is: {today}");
Console.WriteLine($"Underlying value: {(int)today}");

// Enum comparison
Console.WriteLine("\n--- Enum Comparison ---");
DayOfWeek tomorrow = DayOfWeek.Wednesday;
Console.WriteLine($"Is today Tuesday? {today == DayOfWeek.Tuesday}");
Console.WriteLine($"Is today Monday? {today == DayOfWeek.Monday}");

// Switch with enum
Console.WriteLine("\n--- Switch on Enum ---");
string message = today switch
{
    DayOfWeek.Saturday or DayOfWeek.Sunday => "Weekend!",
    DayOfWeek.Friday => "Almost weekend!",
    _ => "Weekday"
};
Console.WriteLine($"{today}: {message}");

// Getting all enum values
Console.WriteLine("\n--- All Days of Week ---");
foreach (DayOfWeek day in Enum.GetValues(typeof(DayOfWeek)))
{
    Console.WriteLine($"  {day} = {(int)day}");
}

// Getting enum names
Console.WriteLine("\n--- Enum Names ---");
foreach (string name in Enum.GetNames(typeof(DayOfWeek)))
{
    Console.WriteLine($"  {name}");
}

// Priority enum with explicit values
Console.WriteLine("\n=== Enum with Explicit Values ===\n");

Priority taskPriority = Priority.High;
Console.WriteLine($"Task priority: {taskPriority}");
Console.WriteLine($"Underlying value: {(int)taskPriority}");

// Comparing priority levels
Console.WriteLine("\n--- Priority Comparison ---");
Console.WriteLine($"High > Medium: {Priority.High > Priority.Medium}");
Console.WriteLine($"Low < High: {Priority.Low < Priority.High}");

// Parsing strings to enums
Console.WriteLine("\n=== Parsing Strings to Enums ===\n");

string dayString = "Thursday";
if (Enum.TryParse<DayOfWeek>(dayString, out DayOfWeek parsedDay))
{
    Console.WriteLine($"Parsed '{dayString}' to: {parsedDay}");
}

string invalidString = "Funday";
if (!Enum.TryParse<DayOfWeek>(invalidString, out DayOfWeek invalidDay))
{
    Console.WriteLine($"Failed to parse '{invalidString}'");
}

// Case-insensitive parsing
Console.WriteLine("\n--- Case-Insensitive Parsing ---");
if (Enum.TryParse<DayOfWeek>("monday", true, out DayOfWeek lowerDay))
{
    Console.WriteLine($"Parsed 'monday' (lowercase) to: {lowerDay}");
}

// Enum with flags (bitwise operations)
Console.WriteLine("\n=== Enum Flags (Bitwise Operations) ===\n");

// Combining flags
Permissions userPermissions = Permissions.Read | Permissions.Write;
Console.WriteLine($"User permissions: {userPermissions}");
Console.WriteLine($"Underlying value: {(int)userPermissions}");

// Checking flags
Console.WriteLine("\n--- Checking Flags ---");
Console.WriteLine($"Has Read permission: {userPermissions.HasFlag(Permissions.Read)}");
Console.WriteLine($"Has Write permission: {userPermissions.HasFlag(Permissions.Write)}");
Console.WriteLine($"Has Execute permission: {userPermissions.HasFlag(Permissions.Execute)}");

// Adding flags
Console.WriteLine("\n--- Adding Flags ---");
userPermissions |= Permissions.Execute;
Console.WriteLine($"After adding Execute: {userPermissions}");
Console.WriteLine($"Underlying value: {(int)userPermissions}");

// Removing flags
Console.WriteLine("\n--- Removing Flags ---");
userPermissions &= ~Permissions.Write;
Console.WriteLine($"After removing Write: {userPermissions}");

// All permissions combination
Console.WriteLine("\n--- All Permissions ---");
Permissions allPermissions = Permissions.Read | Permissions.Write | Permissions.Execute | Permissions.Delete;
Console.WriteLine($"All permissions: {allPermissions}");
Console.WriteLine($"Underlying value: {(int)allPermissions}");

// Real-world example: Order status
Console.WriteLine("\n=== Real-World: Order Status ===\n");

var orders = new List<(int OrderId, OrderStatus Status, string Customer)>
{
    (1001, OrderStatus.Pending, "Alice"),
    (1002, OrderStatus.Shipped, "Bob"),
    (1003, OrderStatus.Delivered, "Charlie"),
    (1004, OrderStatus.Cancelled, "Diana"),
    (1005, OrderStatus.Processing, "Eve")
};

Console.WriteLine("Order Status Report:");
Console.WriteLine("-------------------");
foreach (var order in orders)
{
    string statusIcon = order.Status switch
    {
        OrderStatus.Pending => "⏳",
        OrderStatus.Confirmed => "✓",
        OrderStatus.Processing => "🔄",
        OrderStatus.Shipped => "📦",
        OrderStatus.Delivered => "✅",
        OrderStatus.Cancelled => "❌",
        _ => "?"
    };
    Console.WriteLine($"  Order #{order.OrderId}: {statusIcon} {order.Status} - {order.Customer}");
}

// Filter by status
var pendingOrders = orders.Where(o => o.Status == OrderStatus.Pending).ToList();
Console.WriteLine($"\nPending orders: {pendingOrders.Count}");

// Real-world example: Traffic light
Console.WriteLine("\n=== Real-World: Traffic Light ===\n");

TrafficLight currentLight = TrafficLight.Red;
Console.WriteLine($"Current light: {currentLight}");

string action = currentLight switch
{
    TrafficLight.Red => "STOP",
    TrafficLight.Yellow => "CAUTION",
    TrafficLight.Green => "GO",
    _ => "UNKNOWN"
};
Console.WriteLine($"Action: {action}");

Console.WriteLine("\n=== Program Complete ===");

// Basic enum definition
enum DayOfWeek
{
    Sunday,    // 0
    Monday,    // 1
    Tuesday,   // 2
    Wednesday, // 3
    Thursday,  // 4
    Friday,    // 5
    Saturday   // 6
}

// Enum with explicit values
enum Priority
{
    Low = 1,
    Medium = 2,
    High = 3,
    Critical = 4
}

// Enum with flags (for bitwise operations)
[Flags]
enum Permissions
{
    None = 0,
    Read = 1,      // 0001
    Write = 2,     // 0010
    Execute = 4,   // 0100
    Delete = 8     // 1000
}

enum OrderStatus
{
    Pending,
    Confirmed,
    Processing,
    Shipped,
    Delivered,
    Cancelled
}

enum TrafficLight
{
    Red,
    Yellow,
    Green
}
