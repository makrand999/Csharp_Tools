using System.Collections;
using ProtoBuf;
using ProtoBuf.Meta;

namespace ProtobufSerializer;

/// <summary>
/// Protocol Buffers encoder/decoder - demonstrates binary serialization with protobuf
/// Efficient binary serialization format with schema-based contracts
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Protocol Buffers Serializer ===\n");

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

        byte[] personBytes;
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, person);
            personBytes = ms.ToArray();
        }

        Console.WriteLine($"   Original: {person.Name}, Age {person.Age}");
        Console.WriteLine($"   Serialized size: {personBytes.Length} bytes");
        Console.WriteLine($"   Hex dump: {BitConverter.ToString(personBytes.Take(20).ToArray())}...");

        // Demo 2: Deserialization
        Console.WriteLine("\n2. Deserialization:");
        Person? deserializedPerson;
        using (var ms = new MemoryStream(personBytes))
        {
            deserializedPerson = Serializer.Deserialize<Person>(ms);
        }
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

        byte[] orderBytes;
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, order);
            orderBytes = ms.ToArray();
        }

        Console.WriteLine($"   Order #{order.OrderId} with {order.Items.Count} items");
        Console.WriteLine($"   Serialized size: {orderBytes.Length} bytes");

        Order? deserializedOrder;
        using (var ms = new MemoryStream(orderBytes))
        {
            deserializedOrder = Serializer.Deserialize<Order>(ms);
        }
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

        byte[] companyBytes;
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, company);
            companyBytes = ms.ToArray();
        }

        Console.WriteLine($"   Company: {company.Name}");
        Console.WriteLine($"   Employees: {company.Employees.Count}, Departments: {company.Departments.Length}");
        Console.WriteLine($"   Serialized size: {companyBytes.Length} bytes");

        Company? deserializedCompany;
        using (var ms = new MemoryStream(companyBytes))
        {
            deserializedCompany = Serializer.Deserialize<Company>(ms);
        }
        Console.WriteLine($"   Deserialized employees: {string.Join(", ", deserializedCompany.Employees.Select(e => e.Name))}");

        // Demo 5: Optional fields (nullable)
        Console.WriteLine("\n5. Optional/Nullable Fields:");
        var partialPerson = new Person { Id = 99, Name = "Anonymous" };
        byte[] partialBytes;
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, partialPerson);
            partialBytes = ms.ToArray();
        }
        Console.WriteLine($"   Partial person (missing Email, Age, IsActive): {partialBytes.Length} bytes");
        
        Person? deserializedPartial;
        using (var ms = new MemoryStream(partialBytes))
        {
            deserializedPartial = Serializer.Deserialize<Person>(ms);
        }
        Console.WriteLine($"   Deserialized: Name='{deserializedPartial.Name}', Email='{deserializedPartial.Email}', Age={deserializedPartial.Age}");

        // Demo 6: Comparison with JSON
        Console.WriteLine("\n6. Size Comparison (Protobuf vs JSON):");
        var testData = new LargeDataObject
        {
            Id = 1,
            Title = "Sample Data Object for Size Comparison",
            Description = "This is a sample object used to compare the serialized size between Protocol Buffers and JSON formats.",
            Count = 100,
            Price = 49.99m,
            Tags = new List<string> { "test", "sample", "comparison", "protobuf", "json" },
            Created = DateTime.UtcNow,
            Metadata = new Dictionary<string, string>
            {
                ["version"] = "1.0",
                ["author"] = "Test User",
                ["category"] = "Benchmark"
            }
        };

        // Protobuf size
        byte[] protobufBytes;
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, testData);
            protobufBytes = ms.ToArray();
        }

        // JSON size
        string json = System.Text.Json.JsonSerializer.Serialize(testData);
        byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);

        Console.WriteLine($"   Protobuf size: {protobufBytes.Length} bytes");
        Console.WriteLine($"   JSON size: {jsonBytes.Length} bytes");
        Console.WriteLine($"   Size ratio: {(double)protobufBytes.Length / jsonBytes.Length:P2}");
        Console.WriteLine($"   Savings: {100 - (double)protobufBytes.Length / jsonBytes.Length * 100:F1}% smaller");

        // Demo 7: Runtime model (no attributes)
        Console.WriteLine("\n7. Runtime Model (No Attributes):");
        var runtimeModel = RuntimeTypeModel.Default;
        
        // Add dynamic type
        var metaType = runtimeModel.Add(typeof(DynamicRecord), true);
        Console.WriteLine($"   Registered dynamic type: {nameof(DynamicRecord)}");
        
        var dynamicObj = new DynamicRecord
        {
            Id = 123,
            Value = "Dynamic Value",
            Timestamp = DateTime.UtcNow
        };

        byte[] dynamicBytes;
        using (var ms = new MemoryStream())
        {
            Serializer.Serialize(ms, dynamicObj);
            dynamicBytes = ms.ToArray();
        }
        Console.WriteLine($"   Serialized dynamic object: {dynamicBytes.Length} bytes");

        DynamicRecord? deserializedDynamic;
        using (var ms = new MemoryStream(dynamicBytes))
        {
            deserializedDynamic = Serializer.Deserialize<DynamicRecord>(ms);
        }
        Console.WriteLine($"   Deserialized: Id={deserializedDynamic.Id}, Value='{deserializedDynamic.Value}'");

        // Demo 8: Performance benchmark
        Console.WriteLine("\n8. Performance Benchmark (10000 iterations):");
        var benchmarkObj = new Person { Id = 1, Name = "Benchmark User", Email = "benchmark@test.com", Age = 30, IsActive = true };
        
        var sw = System.Diagnostics.Stopwatch.StartNew();
        for (int i = 0; i < 10000; i++)
        {
            using var ms = new MemoryStream();
            Serializer.Serialize(ms, benchmarkObj);
            ms.Position = 0;
            _ = Serializer.Deserialize<Person>(ms);
        }
        sw.Stop();
        Console.WriteLine($"   Protobuf: {sw.ElapsedMilliseconds}ms ({10000.0 / sw.ElapsedMilliseconds:F0} ops/sec)");

        sw.Restart();
        for (int i = 0; i < 10000; i++)
        {
            string jsonStr = System.Text.Json.JsonSerializer.Serialize(benchmarkObj);
            _ = System.Text.Json.JsonSerializer.Deserialize<Person>(jsonStr);
        }
        sw.Stop();
        Console.WriteLine($"   JSON: {sw.ElapsedMilliseconds}ms ({10000.0 / sw.ElapsedMilliseconds:F0} ops/sec)");
    }
}

// === Protobuf Data Contracts ===

[ProtoContract]
class Person
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Name { get; set; } = "";

    [ProtoMember(3)]
    public string? Email { get; set; }

    [ProtoMember(4)]
    public int Age { get; set; }

    [ProtoMember(5)]
    public bool IsActive { get; set; }
}

[ProtoContract]
class OrderItem
{
    [ProtoMember(1)]
    public int ProductId { get; set; }

    [ProtoMember(2)]
    public string ProductName { get; set; } = "";

    [ProtoMember(3)]
    public int Quantity { get; set; }

    [ProtoMember(4)]
    public decimal Price { get; set; }
}

[ProtoContract]
class Order
{
    [ProtoMember(1)]
    public int OrderId { get; set; }

    [ProtoMember(2)]
    public Person Customer { get; set; } = new();

    [ProtoMember(3)]
    public List<OrderItem> Items { get; set; } = new();

    [ProtoMember(4)]
    public DateTime OrderDate { get; set; }

    [ProtoMember(5)]
    public decimal TotalAmount { get; set; }
}

[ProtoContract]
class Company
{
    [ProtoMember(1)]
    public string Name { get; set; } = "";

    [ProtoMember(2)]
    public List<Person> Employees { get; set; } = new();

    [ProtoMember(3)]
    public string[] Departments { get; set; } = Array.Empty<string>();
}

[ProtoContract]
class LargeDataObject
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Title { get; set; } = "";

    [ProtoMember(3)]
    public string Description { get; set; } = "";

    [ProtoMember(4)]
    public int Count { get; set; }

    [ProtoMember(5)]
    public decimal Price { get; set; }

    [ProtoMember(6)]
    public List<string> Tags { get; set; } = new();

    [ProtoMember(7)]
    public DateTime Created { get; set; }

    [ProtoMember(8)]
    public Dictionary<string, string> Metadata { get; set; } = new();
}

[ProtoContract]
class DynamicRecord
{
    [ProtoMember(1)]
    public int Id { get; set; }

    [ProtoMember(2)]
    public string Value { get; set; } = "";

    [ProtoMember(3)]
    public DateTime Timestamp { get; set; }
}
