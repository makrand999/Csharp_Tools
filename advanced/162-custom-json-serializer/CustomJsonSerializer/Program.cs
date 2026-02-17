using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;

namespace CustomJsonSerializer;

/// <summary>
/// Custom JSON serializer using reflection - demonstrates serialization without System.Text.Json
/// Supports objects, arrays, nested objects, and common types
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Custom JSON Serializer ===\n");

        // Demo 1: Serialize simple object
        Console.WriteLine("1. Simple Object Serialization:");
        var person = new Person
        {
            Name = "John Doe",
            Age = 35,
            Email = "john@example.com",
            IsActive = true
        };
        string personJson = JsonSerializer.Serialize(person);
        Console.WriteLine(personJson);

        // Demo 2: Deserialize simple object
        Console.WriteLine("\n2. Deserialization:");
        string inputJson = """{"Name":"Jane Smith","Age":28,"Email":"jane@example.com","IsActive":true}""";
        var deserializedPerson = JsonSerializer.Deserialize<Person>(inputJson);
        Console.WriteLine($"Name: {deserializedPerson.Name}, Age: {deserializedPerson.Age}");

        // Demo 3: Nested objects
        Console.WriteLine("\n3. Nested Object Serialization:");
        var order = new Order
        {
            OrderId = 12345,
            Customer = person,
            Items = new List<string> { "Laptop", "Mouse", "Keyboard" },
            Total = 1299.99m,
            OrderDate = DateTime.UtcNow
        };
        string orderJson = JsonSerializer.Serialize(order);
        Console.WriteLine(orderJson);

        // Demo 4: Arrays and Lists
        Console.WriteLine("\n4. Array/List Serialization:");
        var company = new Company
        {
            Name = "Tech Corp",
            Employees = new List<Person>
            {
                new Person { Name = "Alice", Age = 30, Email = "alice@corp.com", IsActive = true },
                new Person { Name = "Bob", Age = 25, Email = "bob@corp.com", IsActive = true },
                new Person { Name = "Charlie", Age = 35, Email = "charlie@corp.com", IsActive = false }
            },
            Departments = new[] { "Engineering", "Sales", "Marketing" }
        };
        string companyJson = JsonSerializer.Serialize(company);
        Console.WriteLine(companyJson);

        // Demo 5: Dictionary serialization
        Console.WriteLine("\n5. Dictionary Serialization:");
        var config = new Configuration
        {
            Settings = new Dictionary<string, object>
            {
                ["ConnectionString"] = "Server=localhost;Database=MyDb",
                ["MaxConnections"] = 100,
                ["EnableCache"] = true,
                ["Timeout"] = 30.5
            }
        };
        string configJson = JsonSerializer.Serialize(config);
        Console.WriteLine(configJson);

        // Demo 6: Null handling
        Console.WriteLine("\n6. Null Value Handling:");
        var partialPerson = new Person { Name = "Unknown" };
        string partialJson = JsonSerializer.Serialize(partialPerson);
        Console.WriteLine(partialJson);

        // Demo 7: Special characters and escaping
        Console.WriteLine("\n7. Special Characters:");
        var specialObj = new { Message = "Hello\nWorld\t!", Path = "C:\\Users\\Test", Quote = "He said \"Hi\"" };
        string specialJson = JsonSerializer.Serialize(specialObj);
        Console.WriteLine(specialJson);

        // Demo 8: Pretty print
        Console.WriteLine("\n8. Pretty Printed JSON:");
        string prettyJson = JsonSerializer.Serialize(order, indent: 2);
        Console.WriteLine(prettyJson);
    }
}

// Sample classes for serialization
class Person
{
    public string Name { get; set; } = "";
    public int Age { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; }
}

class Order
{
    public int OrderId { get; set; }
    public Person Customer { get; set; } = new();
    public List<string> Items { get; set; } = new();
    public decimal Total { get; set; }
    public DateTime OrderDate { get; set; }
}

class Company
{
    public string Name { get; set; } = "";
    public List<Person> Employees { get; set; } = new();
    public string[] Departments { get; set; } = Array.Empty<string>();
}

class Configuration
{
    public Dictionary<string, object> Settings { get; set; } = new();
}

/// <summary>
/// Custom JSON serializer implementation using reflection
/// </summary>
class JsonSerializer
{
    /// <summary>
    /// Serialize object to JSON string
    /// </summary>
    public static string Serialize(object? obj, int indent = 0)
    {
        if (obj == null) return "null";
        
        var sb = new StringBuilder();
        SerializeValue(obj, sb, indent, 0);
        return sb.ToString();
    }

    /// <summary>
    /// Deserialize JSON string to object of type T
    /// </summary>
    public static T Deserialize<T>(string json) where T : new()
    {
        var obj = new T();
        var properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        
        // Simple parsing - remove braces and split by comma (outside strings)
        var content = json.Trim('{', '}').Trim();
        if (string.IsNullOrEmpty(content)) return obj;
        
        var pairs = ParseJsonPairs(content);
        
        foreach (var prop in properties)
        {
            if (pairs.TryGetValue(prop.Name, out var value))
            {
                var convertedValue = ConvertValue(value, prop.PropertyType);
                if (convertedValue != null)
                {
                    prop.SetValue(obj, convertedValue);
                }
            }
        }
        
        return obj;
    }

    static void SerializeValue(object? value, StringBuilder sb, int indent, int depth)
    {
        if (value == null)
        {
            sb.Append("null");
            return;
        }

        var type = value.GetType();
        var indentStr = new string(' ', depth * indent);
        var innerIndentStr = new string(' ', (depth + 1) * indent);

        // Primitive types
        if (type == typeof(string))
        {
            sb.Append('\"').Append(EscapeString(value.ToString()!)).Append('\"');
        }
        else if (type == typeof(bool))
        {
            sb.Append(value.ToString()!.ToLower());
        }
        else if (type == typeof(DateTime))
        {
            sb.Append('\"').Append(((DateTime)value).ToString("o")).Append('\"');
        }
        else if (type.IsPrimitive || type == typeof(decimal) || type == typeof(Guid))
        {
            sb.Append(value.ToString());
        }
        else if (type.IsEnum)
        {
            sb.Append('\"').Append(value.ToString()).Append('\"');
        }
        // Dictionary
        else if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Dictionary<,>))
        {
            sb.Append('{');
            var dict = (System.Collections.IDictionary)value;
            bool first = true;
            foreach (var key in dict.Keys)
            {
                if (!first) sb.Append(',');
                if (indent > 0) sb.Append('\n').Append(innerIndentStr);
                first = false;
                sb.Append('\"').Append(key).Append("\":");
                SerializeValue(dict[key], sb, indent, depth + 1);
            }
            if (indent > 0 && dict.Count > 0) sb.Append('\n').Append(indentStr);
            sb.Append('}');
        }
        // Collection
        else if (typeof(System.Collections.IEnumerable).IsAssignableFrom(type) && type != typeof(string))
        {
            sb.Append('[');
            var list = (System.Collections.IEnumerable)value;
            bool first = true;
            foreach (var item in list)
            {
                if (!first) sb.Append(',');
                if (indent > 0) sb.Append('\n').Append(innerIndentStr);
                first = false;
                SerializeValue(item, sb, indent, depth + 1);
            }
            if (indent > 0 && list.Cast<object>().Any()) sb.Append('\n').Append(indentStr);
            sb.Append(']');
        }
        // Object
        else
        {
            sb.Append('{');
            var props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            bool first = true;
            foreach (var prop in props)
            {
                var propValue = prop.GetValue(value);
                if (!first) sb.Append(',');
                if (indent > 0) sb.Append('\n').Append(innerIndentStr);
                first = false;
                sb.Append('\"').Append(prop.Name).Append("\":");
                SerializeValue(propValue, sb, indent, depth + 1);
            }
            if (indent > 0 && props.Length > 0) sb.Append('\n').Append(indentStr);
            sb.Append('}');
        }
    }

    static string EscapeString(string input)
    {
        return input
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\n", "\\n")
            .Replace("\r", "\\r")
            .Replace("\t", "\\t");
    }

    static Dictionary<string, string> ParseJsonPairs(string content)
    {
        var pairs = new Dictionary<string, string>();
        int i = 0;
        
        while (i < content.Length)
        {
            // Skip whitespace and commas
            while (i < content.Length && (char.IsWhiteSpace(content[i]) || content[i] == ','))
                i++;
            
            if (i >= content.Length) break;
            
            // Parse key
            if (content[i] != '"') break;
            i++; // skip opening quote
            int keyStart = i;
            while (i < content.Length && content[i] != '"') i++;
            var key = content.Substring(keyStart, i - keyStart);
            i++; // skip closing quote
            
            // Skip colon
            while (i < content.Length && (char.IsWhiteSpace(content[i]) || content[i] == ':'))
                i++;
            
            // Parse value
            string value;
            if (content[i] == '"')
            {
                i++; // skip opening quote
                var valueStart = i;
                var valueSb = new StringBuilder();
                while (i < content.Length)
                {
                    if (content[i] == '\\' && i + 1 < content.Length)
                    {
                        valueSb.Append(content.Substring(valueStart, i - valueStart));
                        i++;
                        valueSb.Append(content[i] switch
                        {
                            'n' => '\n',
                            'r' => '\r',
                            't' => '\t',
                            _ => content[i]
                        });
                        i++;
                        valueStart = i;
                    }
                    else if (content[i] == '"')
                    {
                        valueSb.Append(content.Substring(valueStart, i - valueStart));
                        i++;
                        break;
                    }
                    else
                    {
                        i++;
                    }
                }
                value = valueSb.ToString();
            }
            else
            {
                int valueStart = i;
                while (i < content.Length && content[i] != ',' && content[i] != '}')
                    i++;
                value = content.Substring(valueStart, i - valueStart).Trim();
            }
            
            pairs[key] = value;
        }
        
        return pairs;
    }

    static object? ConvertValue(string value, Type targetType)
    {
        if (string.IsNullOrEmpty(value)) return null;
        
        // Handle null
        if (value.Equals("null", StringComparison.OrdinalIgnoreCase))
            return null;
        
        // Handle string
        if (targetType == typeof(string))
            return value;
        
        // Handle bool
        if (targetType == typeof(bool))
            return bool.Parse(value);
        
        // Handle numeric types
        if (targetType == typeof(int))
            return int.Parse(value);
        if (targetType == typeof(long))
            return long.Parse(value);
        if (targetType == typeof(double))
            return double.Parse(value);
        if (targetType == typeof(decimal))
            return decimal.Parse(value);
        
        // Handle nullable types
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
            return ConvertValue(value, underlyingType);
        
        return value;
    }
}
