using System;
using System.Reflection;

namespace CustomAttribute;

/// <summary>
/// Demonstrates custom attribute creation and reflection-based processing.
/// Shows how to create metadata-driven systems for validation, serialization, and more.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Custom Attribute System ===\n");
        Console.WriteLine("Demonstrates metadata-driven programming with custom attributes.\n");

        // Demo 1: Validation attributes
        Console.WriteLine("--- Validation Attributes ---\n");
        var user = new User
        {
            Name = "Alice",
            Email = "alice@example.com",
            Age = 30,
            Role = "Admin"
        };

        Console.WriteLine($"Validating user: {user.Name}");
        var validationErrors = Validator.Validate(user);
        
        if (validationErrors.Count == 0)
        {
            Console.WriteLine("✓ Validation passed!\n");
        }
        else
        {
            Console.WriteLine("Validation errors:");
            foreach (var error in validationErrors)
            {
                Console.WriteLine($"  - {error}");
            }
        }

        // Demo 2: Serialization attributes
        Console.WriteLine("\n--- Serialization Attributes ---\n");
        var product = new Product
        {
            Id = 1,
            Name = "Widget",
            Price = 29.99m,
            InternalCode = "SECRET123",
            Description = "A useful widget"
        };

        Console.WriteLine("Original object:");
        Console.WriteLine($"  Id: {product.Id}");
        Console.WriteLine($"  Name: {product.Name}");
        Console.WriteLine($"  Price: {product.Price}");
        Console.WriteLine($"  InternalCode: {product.InternalCode}");
        Console.WriteLine($"  Description: {product.Description}");

        var json = Serializer.Serialize(product);
        Console.WriteLine($"\nSerialized JSON:\n{json}");

        // Demo 3: Command registration
        Console.WriteLine("\n--- Command Registration ---\n");
        CommandRegistry.Register<CalculatorCommands>();
        CommandRegistry.Execute("add", ["5", "3"]);
        CommandRegistry.Execute("subtract", ["10", "4"]);
        CommandRegistry.Execute("multiply", ["6", "7"]);
        CommandRegistry.ListCommands();
    }
}

// === Custom Attributes ===

[AttributeUsage(AttributeTargets.Property)]
public class ValidateRequiredAttribute : Attribute
{
    public string ErrorMessage { get; set; } = "This field is required";
}

[AttributeUsage(AttributeTargets.Property)]
public class ValidateRangeAttribute : Attribute
{
    public int Min { get; set; }
    public int Max { get; set; }

    public ValidateRangeAttribute(int min, int max)
    {
        Min = min;
        Max = max;
    }
}

[AttributeUsage(AttributeTargets.Property)]
public class ValidateEmailAttribute : Attribute
{
    public string ErrorMessage { get; set; } = "Invalid email format";
}

[AttributeUsage(AttributeTargets.Property)]
public class SerializeAttribute : Attribute
{
    public string? Name { get; set; }
    public bool Include { get; set; } = true;
}

[AttributeUsage(AttributeTargets.Method)]
public class CommandAttribute : Attribute
{
    public string Name { get; set; }
    public string Description { get; set; } = "";

    public CommandAttribute(string name)
    {
        Name = name;
    }
}

// === Sample Models ===

public class User
{
    [ValidateRequired(ErrorMessage = "Name is required")]
    public string Name { get; set; } = "";

    [ValidateRequired]
    [ValidateEmail]
    public string Email { get; set; } = "";

    [ValidateRange(0, 150)]
    public int Age { get; set; }

    [ValidateRequired]
    public string Role { get; set; } = "";
}

public class Product
{
    [Serialize(Name = "product_id")]
    public int Id { get; set; }

    [Serialize(Name = "product_name")]
    public string Name { get; set; } = "";

    [Serialize(Name = "price_usd")]
    public decimal Price { get; set; }

    [Serialize(Include = false)]
    public string InternalCode { get; set; } = "";

    [Serialize]
    public string Description { get; set; } = "";
}

// === Validator Engine ===

public static class Validator
{
    public static List<string> Validate(object obj)
    {
        var errors = new List<string>();
        var properties = obj.GetType().GetProperties();

        foreach (var prop in properties)
        {
            var value = prop.GetValue(obj);

            // Check Required
            var requiredAttr = prop.GetCustomAttribute<ValidateRequiredAttribute>();
            if (requiredAttr != null)
            {
                if (value == null || (value is string s && string.IsNullOrWhiteSpace(s)))
                {
                    errors.Add($"{prop.Name}: {requiredAttr.ErrorMessage}");
                    continue;
                }
            }

            // Check Email
            var emailAttr = prop.GetCustomAttribute<ValidateEmailAttribute>();
            if (emailAttr != null && value is string email)
            {
                if (!email.Contains("@") || !email.Contains("."))
                {
                    errors.Add($"{prop.Name}: {emailAttr.ErrorMessage}");
                }
            }

            // Check Range
            var rangeAttr = prop.GetCustomAttribute<ValidateRangeAttribute>();
            if (rangeAttr != null && value is int intVal)
            {
                if (intVal < rangeAttr.Min || intVal > rangeAttr.Max)
                {
                    errors.Add($"{prop.Name}: Value must be between {rangeAttr.Min} and {rangeAttr.Max}");
                }
            }
        }

        return errors;
    }
}

// === Serializer Engine ===

public static class Serializer
{
    public static string Serialize(object obj)
    {
        var properties = obj.GetType().GetProperties();
        var pairs = new List<string>();

        foreach (var prop in properties)
        {
            var serializeAttr = prop.GetCustomAttribute<SerializeAttribute>();
            if (serializeAttr == null || !serializeAttr.Include)
                continue;

            var name = serializeAttr.Name ?? prop.Name;
            var value = prop.GetValue(obj);
            var stringValue = value switch
            {
                string s => $"\"{s}\"",
                null => "null",
                _ => value.ToString()
            };

            pairs.Add($"  \"{name}\": {stringValue}");
        }

        return "{\n" + string.Join(",\n", pairs) + "\n}";
    }
}

// === Command Registry ===

public static class CommandRegistry
{
    private static readonly Dictionary<string, (MethodInfo Method, object Instance, string Description)> _commands = new();

    public static void Register<T>() where T : new()
    {
        var instance = new T();
        var type = typeof(T);
        var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var method in methods)
        {
            var attr = method.GetCustomAttribute<CommandAttribute>();
            if (attr != null)
            {
                _commands[attr.Name] = (method, instance, attr.Description);
            }
        }
    }

    public static void Execute(string name, string[] args)
    {
        if (_commands.TryGetValue(name, out var cmd))
        {
            var parameters = cmd.Method.GetParameters();
            var convertedArgs = new object?[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
            {
                convertedArgs[i] = Convert.ChangeType(args[i], parameters[i].ParameterType);
            }

            var result = cmd.Method.Invoke(cmd.Instance, convertedArgs);
            Console.WriteLine($"  [{name}] Result: {result}");
        }
        else
        {
            Console.WriteLine($"  Unknown command: {name}");
        }
    }

    public static void ListCommands()
    {
        Console.WriteLine("\nRegistered commands:");
        foreach (var kvp in _commands)
        {
            Console.WriteLine($"  - {kvp.Key}: {kvp.Value.Description}");
        }
    }
}

public class CalculatorCommands
{
    [Command("add", Description = "Add two numbers")]
    public int Add(int a, int b) => a + b;

    [Command("subtract", Description = "Subtract two numbers")]
    public int Subtract(int a, int b) => a - b;

    [Command("multiply", Description = "Multiply two numbers")]
    public int Multiply(int a, int b) => a * b;
}
