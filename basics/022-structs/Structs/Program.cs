// Program 22: Structs - Demonstrates struct definition, constructors, and value type behavior
// Topics: Value types, constructors, properties, methods, comparison with classes

Console.WriteLine("=== Struct Basics ===\n");

// Creating struct instances
Point p1 = new Point(3, 4);
Console.WriteLine("Point p1:");
p1.Display();
Console.WriteLine($"Distance from origin: {p1.DistanceFromOrigin():F2}");

// Object initializer syntax
Point p2 = new Point { X = 10, Y = 20 };
Console.WriteLine($"\nPoint p2: ({p2.X}, {p2.Y})");

// Default struct values
Point p3 = new Point(); // All fields set to default (0)
Console.WriteLine($"\nDefault point p3: ({p3.X}, {p3.Y})");

// Struct value semantics (copy on assignment)
Console.WriteLine("\n=== Value Semantics (Copy on Assignment) ===\n");

Point original = new Point(5, 5);
Point copy = original; // Creates a copy

Console.WriteLine($"Original: ({original.X}, {original.Y})");
Console.WriteLine($"Copy: ({copy.X}, {copy.Y})");

// Modify the copy
copy.X = 100;
copy.Y = 200;

Console.WriteLine("\nAfter modifying copy:");
Console.WriteLine($"Original: ({original.X}, {original.Y}) - unchanged");
Console.WriteLine($"Copy: ({copy.X}, {copy.Y}) - modified");

// Readonly struct
Console.WriteLine("\n=== Readonly Struct ===\n");

Color red = new Color(255, 0, 0);
Color green = new Color(0, 255, 0);
Color blue = new Color(0, 0, 255);

Console.WriteLine($"Red: {red.ToRgbString()} = {red.ToHex()}");
Console.WriteLine($"Green: {green.ToRgbString()} = {green.ToHex()}");
Console.WriteLine($"Blue: {blue.ToRgbString()} = {blue.ToHex()}");

// Complex struct with nested structs
Console.WriteLine("\n=== Complex Struct with Nested Structs ===\n");

Rectangle rect = new Rectangle(
    new Point(0, 0),
    new Point(100, 50),
    "Main Rectangle"
);
rect.DisplayInfo();

// Array of structs
Console.WriteLine("\n=== Array of Structs ===\n");

Point[] points = new Point[5];
for (int i = 0; i < points.Length; i++)
{
    points[i] = new Point(i * 10, i * 5);
}

Console.WriteLine("Points array:");
foreach (var point in points)
{
    point.Display();
}

// List of structs
Console.WriteLine("\n=== List of Structs ===\n");

var colors = new List<Color>
{
    new Color(255, 128, 0),   // Orange
    new Color(128, 0, 255),   // Purple
    new Color(0, 255, 255),   // Cyan
    new Color(255, 255, 0),   // Yellow
};

Console.WriteLine("Custom colors:");
foreach (var color in colors)
{
    Console.WriteLine($"  {color.ToRgbString()} = {color.ToHex()}");
}

// Struct vs Class - Value type behavior
Console.WriteLine("\n=== Struct vs Class (Value vs Reference) ===\n");

// Struct (value type)
StructData struct1 = new StructData { Value = 10 };
StructData struct2 = struct1; // Copy
struct2.Value = 20;
Console.WriteLine($"Struct: struct1.Value = {struct1.Value}, struct2.Value = {struct2.Value}");
Console.WriteLine("  (Structs are copied on assignment - independent values)");

// Class (reference type)
ClassData class1 = new ClassData { Value = 10 };
ClassData class2 = class1; // Reference copy
class2.Value = 20;
Console.WriteLine($"\nClass: class1.Value = {class1.Value}, class2.Value = {class2.Value}");
Console.WriteLine("  (Classes are references - same object)");

// Nullable structs
Console.WriteLine("\n=== Nullable Structs ===\n");

Point? nullablePoint = null;
Console.WriteLine($"Nullable point (null): {nullablePoint.HasValue}");

nullablePoint = new Point(50, 50);
Console.WriteLine($"Nullable point (assigned): {nullablePoint.HasValue}");
Console.WriteLine($"Value: ({nullablePoint.Value.X}, {nullablePoint.Value.Y})");

// Using null-conditional operator
Console.WriteLine($"X value: {nullablePoint?.X ?? 0}");

Console.WriteLine("\n=== Program Complete ===");

// Basic struct definition
struct Point
{
    public int X { get; set; }
    public int Y { get; set; }

    // Constructor
    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    // Method
    public void Display()
    {
        Console.WriteLine($"Point({X}, {Y})");
    }

    // Method with return value
    public double DistanceFromOrigin()
    {
        return Math.Sqrt(X * X + Y * Y);
    }
}

// Struct with readonly members
readonly struct Color
{
    public byte R { get; }
    public byte G { get; }
    public byte B { get; }

    public Color(byte r, byte g, byte b)
    {
        R = r;
        G = g;
        B = b;
    }

    // Readonly method (explicit for readonly structs)
    public readonly string ToRgbString()
    {
        return $"rgb({R}, {G}, {B})";
    }

    public readonly string ToHex()
    {
        return $"#{R:X2}{G:X2}{B:X2}";
    }
}

// Struct representing a rectangle
struct Rectangle
{
    public Point TopLeft { get; set; }
    public Point BottomRight { get; set; }
    public string Label { get; set; }

    public Rectangle(Point topLeft, Point bottomRight, string label = "Rectangle")
    {
        TopLeft = topLeft;
        BottomRight = bottomRight;
        Label = label;
    }

    public int Width => BottomRight.X - TopLeft.X;
    public int Height => BottomRight.Y - TopLeft.Y;
    public int Area => Width * Height;

    public void DisplayInfo()
    {
        Console.WriteLine($"{Label}:");
        Console.WriteLine($"  Top-Left: ({TopLeft.X}, {TopLeft.Y})");
        Console.WriteLine($"  Bottom-Right: ({BottomRight.X}, {BottomRight.Y})");
        Console.WriteLine($"  Width: {Width}, Height: {Height}");
        Console.WriteLine($"  Area: {Area}");
    }
}

// Supporting types for comparison demo
struct StructData
{
    public int Value { get; set; }
}

class ClassData
{
    public int Value { get; set; }
}
