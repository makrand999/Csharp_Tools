using MessagePack;
using static MessagePack.MessagePackSerializer;

namespace MessagePackSerializerDemo;

/// <summary>
/// MessagePack serializer - demonstrates binary serialization with MessagePack library
/// Fast, compact binary serialization format similar to JSON but more efficient
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== MessagePack Serializer ===\n");

        // Demo 1: Basic serialization
        Console.WriteLine("1. Basic Serialization:");
        var person = new Person
        {
            Id = 42,
            Name = "John Doe",
            Email = "john@example.com",
            Age = 35,
            IsActive = true
        };

        byte[] personBytes = Serialize(person);
        Console.WriteLine($"   Original: {person.Name}, Age {person.Age}");
        Console.WriteLine($"   Serialized size: {personBytes.Length} bytes");
        Console.WriteLine($"   Hex dump: {BitConverter.ToString(personBytes.Take(20).ToArray())}...");

        // Demo 2: Deserialization
        Console.WriteLine("\n2. Deserialization:");
        Person? deserializedPerson = Deserialize<Person>(personBytes);
        Console.WriteLine($"   Deserialized: {deserializedPerson.Name}, Age {deserializedPerson.Age}");
        Console.WriteLine($"   Match: {deserializedPerson.Name == person.Name && deserializedPerson.Age == person.Age}");

        // Demo 3: Nested objects
        Console.WriteLine("\n3. Nested Objects:");
        var order = new Order
        {
            OrderId = 1001,
            Customer = person,
            Items = new List<OrderItem>
            {
                new OrderItem { ProductId = 1, ProductName = "Laptop", Quantity = 1, Price = 999.99m },
                new OrderItem { ProductId = 2, ProductName = "Mouse", Quantity = 2, Price = 29.99m }
            },
            OrderDate = DateTime.UtcNow,
            TotalAmount = 1059.97m
        };

        byte[] orderBytes = Serialize(order);
        Console.WriteLine($"   Order #{order.OrderId} with {order.Items.Count} items");
        Console.WriteLine($"   Serialized size: {orderBytes.Length} bytes");

        Order? deserializedOrder = Deserialize<Order>(orderBytes);
        Console.WriteLine($"   Deserialized: Order #{deserializedOrder.OrderId}, Customer: {deserializedOrder.Customer.Name}");

        // Demo 4: Collections
        Console.WriteLine("\n4. Collections:");
        var company = new Company
        {
            Name = "Tech Corp",
            Employees = new List<Person>
            {
                new Person { Id = 1, Name = "Alice", Email = "alice@corp.com", Age = 30, IsActive = true },
                new Person { Id = 2, Name = "Bob", Email = "bob@corp.com", Age = 25, IsActive = true },
                new Person { Id = 3, Name = "Charlie", Email = "charlie@corp.com", Age = 35, IsActive = false }
            },
            Departments = new[] { "Engineering", "Sales", "Marketing" }
        };

        byte[] companyBytes = Serialize(company);
        Console.WriteLine($"   Company: {company.Name}");
        Console.WriteLine($"   Employees: {company.Employees.Count}, Departments: {company.Departments.Length}");
        Console.WriteLine($"   Serialized size: {companyBytes.Length} bytes");

        Company? deserializedCompany = Deserialize<Company>(companyBytes);
        Console.WriteLine($"   Deserialized employees: {string.Join(", ", deserializedCompany.Employees.Select(e => e.Name))}");

        // Demo 5: Union types (polymorphic serialization)
        Console.WriteLine("\n5. Union Types (Polymorphic Serialization):");
        List<Shape> shapes = new List<Shape>
        {
            new Circle { Radius = 5, Color = "Red" },
            new Rectangle { Width = 10, Height = 20, Color = "Blue" },
            new Triangle { Base = 8, Height = 6, Color = "Green" }
        };

        byte[] shapesBytes = Serialize(shapes);
        Console.WriteLine($"   Serialized {shapes.Count} shapes: {shapesBytes.Length} bytes");

        List<Shape>? deserializedShapes = Deserialize<List<Shape>>(shapesBytes);
        Console.WriteLine($"   Deserialized shapes:");
        foreach (var shape in deserializedShapes!)
        {
            Console.WriteLine($"      - {shape.GetType().Name}: Area = {shape.CalculateArea():F2}, Color = {shape.Color}");
        }

        // Demo 6: Compression (LZ4)
        Console.WriteLine("\n6. LZ4 Compression:");
        var largeData = new LargeDataObject
        {
            Id = 1,
            Title = "Large Data Object for Compression Test",
            Description = new string('A', 10000), // 10KB of repeated data
            Values = Enumerable.Range(0, 1000).ToList(),
            Created = DateTime.UtcNow
        };

        byte[] uncompressed = Serialize(largeData);
        byte[] compressed = Serialize(largeData, MessagePackSerializerOptions.Standard.WithCompression(MessagePackCompression.Lz4Block));

        Console.WriteLine($"   Uncompressed size: {uncompressed.Length} bytes");
        Console.WriteLine($"   LZ4 compressed size: {compressed.Length} bytes");
        Console.WriteLine($"   Compression ratio: {(double)compressed.Length / uncompressed.Length:P2}");
        Console.WriteLine($"   Space savings: {100 - (double)compressed.Length / uncompressed.Length * 100:F1}%");

        // Demo 7: Comparison with JSON and Protobuf
        Console.WriteLine("\n7. Size Comparison (MessagePack vs JSON vs Protobuf):");
        var testData = new TestData
        {
            Id = 1,
            Name = "Test Object",
            Description = "This is a test object for comparing serialization formats",
            Count = 100,
            Price = 49.99m,
            Tags = new List<string> { "test", "sample", "comparison" },
            Created = DateTime.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                ["version"] = "1.0",
                ["author"] = "Test"
            }
        };

        // MessagePack size
        byte[] msgpackBytes = Serialize(testData);

        // JSON size
        string json = System.Text.Json.JsonSerializer.Serialize(testData);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

        Console.WriteLine($"   MessagePack: {msgpackBytes.Length} bytes");
        Console.WriteLine($"   JSON: {jsonBytes.Length} bytes");
        Console.WriteLine($"   MessagePack is {100 - (double)msgpackBytes.Length / jsonBytes.Length * 100:F1}% smaller than JSON");

        // Demo 8: Performance benchmark
        Console.WriteLine("\n8. Performance Benchmark (10000 iterations):");
        var benchmarkObj = new Person { Id = 1, Name = "Benchmark User", Email = "benchmark@test.com", Age = 30, IsActive = true };

        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            byte[] bytes = Serialize(benchmarkObj);
            _ = Deserialize<Person>(bytes);
        }
        sw.Stop();
        Console.WriteLine($"   MessagePack: {sw.ElapsedMilliseconds}ms ({10000.0 / sw.ElapsedMilliseconds:F0} ops/sec)");

        sw.Restart();
        for (int i = 0; i < 10000; i++)
        {
            string jsonStr = System.Text.Json.JsonSerializer.Serialize(benchmarkObj);
            _ = System.Text.Json.JsonSerializer.Deserialize<Person>(jsonStr);
        }
        sw.Stop();
        Console.WriteLine($"   JSON: {sw.ElapsedMilliseconds}ms ({10000.0 / sw.ElapsedMilliseconds:F0} ops/sec)");

        // Demo 9: Dynamic/Anonymous types
        Console.WriteLine("\n9. Dynamic Type Serialization:");
        dynamic dynamicObj = new
        {
            Id = 123,
            Name = "Dynamic Object",
            Values = new[] { 1, 2, 3, 4, 5 }
        };

        // MessagePack can serialize anonymous types
        byte[] dynamicBytes = Serialize(dynamicObj);
        Console.WriteLine($"   Dynamic object serialized: {dynamicBytes.Length} bytes");

        // Deserialize as object array
        var deserializedDynamic = Deserialize<object[]>(dynamicBytes);
        Console.WriteLine($"   Deserialized type: {deserializedDynamic.GetType()}");

        // Demo 10: MessagePack format types
        Console.WriteLine("\n10. MessagePack Format Types:");
        ShowFormatType((sbyte)42, "Integer (positive fixint)");
        ShowFormatType(-5, "Integer (negative fixint)");
        ShowFormatType(3.14, "Float");
        ShowFormatType(true, "Boolean");
        ShowFormatType("Hello", "String");
        ShowFormatType(new byte[] { 1, 2, 3 }, "Binary");
    }

    static void ShowFormatType(object value, string description)
    {
        byte[] bytes = Serialize(value);
        string formatType = bytes[0] switch
        {
            >= 0x00 and <= 0x7F => "Positive Fixint",
            >= 0xE0 and <= 0xFF => "Negative Fixint",
            0xC0 => "Nil",
            0xC2 => "False",
            0xC3 => "True",
            0xCA => "Float 32",
            0xCB => "Float 64",
            >= 0xA0 and <= 0xBF => "Fixstr",
            0xD9 => "Str8",
            0xDA => "Str16",
            0xC4 => "Bin8",
            0xC5 => "Bin16",
            0xDC => "Array16",
            0xDD => "Array32",
            0xDE => "Map16",
            0xDF => "Map32",
            _ => $"Other (0x{bytes[0]:X2})"
        };
        Console.WriteLine($"   {description}: 0x{bytes[0]:X2} ({formatType}), {bytes.Length} bytes");
    }
}

// === Data Contracts ===

[MessagePackObject]
public class Person
{
    [Key(0)]
    public int Id { get; set; }

    [Key(1)]
    public string Name { get; set; } = "";

    [Key(2)]
    public string? Email { get; set; }

    [Key(3)]
    public int Age { get; set; }

    [Key(4)]
    public bool IsActive { get; set; }
}

[MessagePackObject]
public class OrderItem
{
    [Key(0)]
    public int ProductId { get; set; }

    [Key(1)]
    public string ProductName { get; set; } = "";

    [Key(2)]
    public int Quantity { get; set; }

    [Key(3)]
    public decimal Price { get; set; }
}

[MessagePackObject]
public class Order
{
    [Key(0)]
    public int OrderId { get; set; }

    [Key(1)]
    public Person Customer { get; set; } = new();

    [Key(2)]
    public List<OrderItem> Items { get; set; } = new();

    [Key(3)]
    public DateTime OrderDate { get; set; }

    [Key(4)]
    public decimal TotalAmount { get; set; }
}

[MessagePackObject]
public class Company
{
    [Key(0)]
    public string Name { get; set; } = "";

    [Key(1)]
    public List<Person> Employees { get; set; } = new();

    [Key(2)]
    public string[] Departments { get; set; } = Array.Empty<string>();
}

// Union types for polymorphic serialization
[MessagePackObject]
[Union(0, typeof(Circle))]
[Union(1, typeof(Rectangle))]
[Union(2, typeof(Triangle))]
public abstract class Shape
{
    [Key(0)]
    public string Color { get; set; } = "";

    public abstract double CalculateArea();
}

[MessagePackObject]
public class Circle : Shape
{
    [Key(1)]
    public double Radius { get; set; }

    public override double CalculateArea() => Math.PI * Radius * Radius;
}

[MessagePackObject]
public class Rectangle : Shape
{
    [Key(1)]
    public double Width { get; set; }

    [Key(2)]
    public double Height { get; set; }

    public override double CalculateArea() => Width * Height;
}

[MessagePackObject]
public class Triangle : Shape
{
    [Key(1)]
    public double Base { get; set; }

    [Key(2)]
    public double Height { get; set; }

    public override double CalculateArea() => 0.5 * Base * Height;
}

[MessagePackObject]
public class LargeDataObject
{
    [Key(0)]
    public int Id { get; set; }

    [Key(1)]
    public string Title { get; set; } = "";

    [Key(2)]
    public string Description { get; set; } = "";

    [Key(3)]
    public List<int> Values { get; set; } = new();

    [Key(4)]
    public DateTime Created { get; set; }
}

[MessagePackObject]
public class TestData
{
    [Key(0)]
    public int Id { get; set; }

    [Key(1)]
    public string Name { get; set; } = "";

    [Key(2)]
    public string Description { get; set; } = "";

    [Key(3)]
    public int Count { get; set; }

    [Key(4)]
    public decimal Price { get; set; }

    [Key(5)]
    public List<string> Tags { get; set; } = new();

    [Key(6)]
    public DateTime Created { get; set; }

    [Key(7)]
    public Dictionary<string, string> Metadata { get; set; } = new();
}
