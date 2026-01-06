// Program 23: Nullable Types - Demonstrates null-coalescing, null-conditional operators
// Topics: Nullable value types, ??, ?., ?[], !, null checks

Console.WriteLine("=== Nullable Value Types ===\n");

// Nullable int
int? nullableInt = null;
Console.WriteLine($"Nullable int (null): {nullableInt}");
Console.WriteLine($"HasValue: {nullableInt.HasValue}");

nullableInt = 42;
Console.WriteLine($"\nNullable int (42): {nullableInt}");
Console.WriteLine($"HasValue: {nullableInt.HasValue}");
Console.WriteLine($"Value: {nullableInt.Value}");

// Nullable double
double? price = 19.99;
double? discount = null;

Console.WriteLine($"\nPrice: ${price}");
Console.WriteLine($"Discount: {(discount.HasValue ? $"{discount}%" : "No discount")}");

// Nullable DateTime
DateTime? lastLogin = null;
Console.WriteLine($"\nLast login: {(lastLogin?.ToString() ?? "Never logged in")}");

lastLogin = DateTime.Now;
Console.WriteLine($"Last login: {lastLogin}");

// Null-coalescing operator (??)
Console.WriteLine("\n=== Null-Coalescing Operator (??) ===\n");

string? userName = null;
string displayName = userName ?? "Guest";
Console.WriteLine($"UserName is null, DisplayName: {displayName}");

userName = "Alice";
displayName = userName ?? "Guest";
Console.WriteLine($"UserName is '{userName}', DisplayName: {displayName}");

// Chaining null-coalescing
Console.WriteLine("\n--- Chaining Null-Coalescing ---");
string? primary = null;
string? secondary = null;
string? tertiary = "Default User";
string result = primary ?? secondary ?? tertiary ?? "Anonymous";
Console.WriteLine($"Result: {result}");

// Null-coalescing assignment (??=)
Console.WriteLine("\n--- Null-Coalescing Assignment (??=) ---");
int? count = null;
Console.WriteLine($"Count before: {count}");
count ??= 10; // Assign if null
Console.WriteLine($"Count after ??= 10: {count}");
count ??= 20; // Won't assign (already has value)
Console.WriteLine($"Count after ??= 20: {count}");

// Null-conditional operator (?.)
Console.WriteLine("\n=== Null-Conditional Operator (?.) ===\n");

string? text = null;
int? length = text?.Length;
Console.WriteLine($"Null string length: {length}"); // null

text = "Hello, World!";
length = text?.Length;
Console.WriteLine($"'Hello, World!' length: {length}"); // 13

// Null-conditional with methods
Console.WriteLine("\n--- Null-Conditional with Methods ---");
string? input = null;
string? upper = input?.ToUpper();
Console.WriteLine($"ToUpper on null: {upper}");

input = "hello";
upper = input?.ToUpper();
Console.WriteLine($"ToUpper on 'hello': {upper}");

// Null-conditional with arrays (?[])
Console.WriteLine("\n--- Null-Conditional with Arrays ---");
int[]? numbers = null;
int? first = numbers?[0];
Console.WriteLine($"First element of null array: {first}");

numbers = new[] { 1, 2, 3, 4, 5 };
first = numbers?[0];
Console.WriteLine($"First element of [1,2,3,4,5]: {first}");

// Chaining null-conditional operators
Console.WriteLine("\n--- Chaining Null-Conditional ---");
Person? person = null;
string? city = person?.Address?.City;
Console.WriteLine($"City from null person: {city}");

person = new Person { Name = "Bob", Address = new Address { City = "Seattle" } };
city = person?.Address?.City;
Console.WriteLine($"City from person: {city}");

// Nullable arithmetic
Console.WriteLine("\n=== Nullable Arithmetic ===\n");

int? a = 10;
int? b = 5;
int? c = null;

Console.WriteLine($"{a} + {b} = {a + b}");
Console.WriteLine($"{a} - {b} = {a - b}");
Console.WriteLine($"{a} * {b} = {a * b}");
Console.WriteLine($"{a} / {b} = {a / b}");

Console.WriteLine($"\n{a} + {c} = {a + c}"); // null
Console.WriteLine($"{c} * {b} = {c * b}"); // null

// Nullable comparisons
Console.WriteLine("\n=== Nullable Comparisons ===\n");

int? x = 10;
int? y = 10;
int? z = null;

Console.WriteLine($"{x} == {y}: {x == y}");
Console.WriteLine($"{x} == {z}: {x == z}");
Console.WriteLine($"{z} == null: {z == null}");
Console.WriteLine($"{x} > {y}: {x > y}");
Console.WriteLine($"{x} < {y}: {x < y}");

// Nullable boolean and ?? operator
Console.WriteLine("\n=== Nullable Boolean ===\n");

bool? isVerified = null;
string status = isVerified == true ? "Verified" : "Not Verified";
Console.WriteLine($"Status (null verified): {status}");

isVerified = true;
status = isVerified == true ? "Verified" : "Not Verified";
Console.WriteLine($"Status (true verified): {status}");

// Using GetValueOrDefault()
Console.WriteLine("\n=== GetValueOrDefault() ===\n");

int? value = null;
Console.WriteLine($"Null value.GetValueOrDefault(): {value.GetValueOrDefault()}");
Console.WriteLine($"Null value.GetValueOrDefault(100): {value.GetValueOrDefault(100)}");

value = 50;
Console.WriteLine($"50 value.GetValueOrDefault(): {value.GetValueOrDefault()}");
Console.WriteLine($"50 value.GetValueOrDefault(100): {value.GetValueOrDefault(100)}");

// Real-world example: Database record simulation
Console.WriteLine("\n=== Real-World: Database Record Simulation ===\n");

var user = new Dictionary<string, object?>
{
    ["Id"] = 1,
    ["Name"] = "Charlie",
    ["Age"] = null,
    ["Email"] = "charlie@example.com",
    ["PhoneNumber"] = null,
    ["LastLogin"] = DateTime.Now.AddDays(-5)
};

Console.WriteLine("User Profile:");
Console.WriteLine($"  ID: {user["Id"]}");
Console.WriteLine($"  Name: {user["Name"]}");
Console.WriteLine($"  Age: {(int?)user["Age"] ?? 0} (default if null)");
Console.WriteLine($"  Email: {user["Email"] ?? "Not provided"}");
Console.WriteLine($"  Phone: {(string?)user["PhoneNumber"] ?? "Not provided"}");
Console.WriteLine($"  Last Login: {((DateTime?)user["LastLogin"])?.ToString("yyyy-MM-dd") ?? "Never"}");

Console.WriteLine("\n=== Program Complete ===");

// Supporting classes
class Address
{
    public string? Street { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}

class Person
{
    public string? Name { get; set; }
    public int? Age { get; set; }
    public Address? Address { get; set; }
}
