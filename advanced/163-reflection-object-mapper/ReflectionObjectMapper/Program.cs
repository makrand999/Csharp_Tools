using System.Reflection;

namespace ReflectionObjectMapper;

/// <summary>
/// Reflection-based object mapper - maps properties between objects of different types
/// Demonstrates reflection, property mapping, type conversion, and attribute-based configuration
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Reflection Object Mapper ===\n");

        // Demo 1: Basic property mapping (same names)
        Console.WriteLine("1. Basic Property Mapping:");
        var userDto = new UserDto
        {
            Id = 1,
            Name = "John Doe",
            Email = "john@example.com",
            CreatedAt = DateTime.UtcNow
        };
        var userEntity = ObjectMapper.Map<UserDto, UserEntity>(userDto);
        Console.WriteLine($"Mapped: {userEntity.Id} - {userEntity.Name} ({userEntity.Email})");

        // Demo 2: Mapping with different property names (using [MapFrom] attribute)
        Console.WriteLine("\n2. Mapping with Property Name Differences:");
        var apiResponse = new ApiResponse
        {
            UserId = 42,
            UserName = "Jane Smith",
            UserEmail = "jane@api.com",
            AccountStatus = "active"
        };
        var customer = ObjectMapper.Map<ApiResponse, Customer>(apiResponse);
        Console.WriteLine($"Mapped: {customer.Id} - {customer.FullName} ({customer.EmailAddress}), Status: {customer.Status}");

        // Demo 3: Type conversion during mapping
        Console.WriteLine("\n3. Type Conversion:");
        var dataModel = new DataModel
        {
            Id = "12345",
            Score = "98.5",
            IsActive = "true",
            BirthDate = "1990-05-15"
        };
        var businessObject = ObjectMapper.Map<DataModel, BusinessObject>(dataModel);
        Console.WriteLine($"Id: {businessObject.Id} (int), Score: {businessObject.Score} (decimal), Active: {businessObject.IsActive} (bool), BirthDate: {businessObject.BirthDate} (DateTime)");

        // Demo 4: Nested object mapping
        Console.WriteLine("\n4. Nested Object Mapping:");
        var orderDto = new OrderDto
        {
            OrderId = 100,
            CustomerName = "Alice Johnson",
            CustomerEmail = "alice@order.com",
            Items = new List<OrderItemDto>
            {
                new OrderItemDto { ProductName = "Laptop", Quantity = 1, UnitPrice = 999.99m },
                new OrderItemDto { ProductName = "Mouse", Quantity = 2, UnitPrice = 29.99m }
            },
            TotalAmount = 1059.97m
        };
        var orderEntity = ObjectMapper.Map<OrderDto, OrderEntity>(orderDto);
        Console.WriteLine($"Order #{orderEntity.OrderId} for {orderEntity.Customer.FullName}");
        Console.WriteLine($"Items: {orderEntity.Items.Count} products, Total: ${orderEntity.TotalAmount}");

        // Demo 5: Collection mapping
        Console.WriteLine("\n5. Collection Mapping:");
        var productDtos = new List<ProductDto>
        {
            new ProductDto { ProductId = 1, ProductName = "Widget", ProductPrice = 19.99m },
            new ProductDto { ProductId = 2, ProductName = "Gadget", ProductPrice = 49.99m },
            new ProductDto { ProductId = 3, ProductName = "Gizmo", ProductPrice = 99.99m }
        };
        var productEntities = ObjectMapper.MapCollection<ProductDto, ProductEntity>(productDtos);
        Console.WriteLine($"Mapped {productEntities.Count} products:");
        foreach (var p in productEntities)
        {
            Console.WriteLine($"  - {p.Name}: ${p.Price}");
        }

        // Demo 6: Selective mapping (using [Ignore] attribute)
        Console.WriteLine("\n6. Selective Mapping (Ignored Properties):");
        var sourceWithIgnored = new SourceWithIgnored
        {
            PublicData = "Visible",
            InternalData = "Also Visible",
            SensitiveData = "SECRET_KEY_12345",
            ComputedData = "Should Not Copy"
        };
        var targetWithIgnored = ObjectMapper.Map<SourceWithIgnored, TargetWithIgnored>(sourceWithIgnored);
        Console.WriteLine($"PublicData: {targetWithIgnored.PublicData}");
        Console.WriteLine($"InternalData: {targetWithIgnored.InternalData}");
        Console.WriteLine($"SensitiveData: {(targetWithIgnored.SensitiveData == null ? "(ignored)" : targetWithIgnored.SensitiveData)}");
        Console.WriteLine($"ComputedData: {(targetWithIgnored.ComputedData == null ? "(ignored)" : targetWithIgnored.ComputedData)}");

        // Demo 7: Bidirectional mapping
        Console.WriteLine("\n7. Bidirectional Mapping:");
        var entity = new UserEntity { Id = 99, Name = "Test User", Email = "test@example.com", CreatedAt = DateTime.Now };
        var dto = ObjectMapper.Map<UserEntity, UserDto>(entity);
        Console.WriteLine($"Entity -> DTO: {dto.Id} - {dto.Name}");
        var backToEntity = ObjectMapper.Map<UserDto, UserEntity>(dto);
        Console.WriteLine($"DTO -> Entity: {backToEntity.Id} - {backToEntity.Name}");
    }
}

// === Source and Target Types ===

// Basic DTO and Entity with matching property names
class UserDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

class UserEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

// Different property names - uses [MapFrom] attribute
class ApiResponse
{
    public int UserId { get; set; }
    public string UserName { get; set; } = "";
    public string UserEmail { get; set; } = "";
    public string AccountStatus { get; set; } = "";
}

class Customer
{
    [MapFrom("UserId")]
    public int Id { get; set; }
    
    [MapFrom("UserName")]
    public string FullName { get; set; } = "";
    
    [MapFrom("UserEmail")]
    public string EmailAddress { get; set; } = "";
    
    [MapFrom("AccountStatus")]
    public string Status { get; set; } = "";
}

// Type conversion example - strings to typed values
class DataModel
{
    public string Id { get; set; } = "";
    public string Score { get; set; } = "";
    public string IsActive { get; set; } = "";
    public string BirthDate { get; set; } = "";
}

class BusinessObject
{
    public int Id { get; set; }
    public decimal Score { get; set; }
    public bool IsActive { get; set; }
    public DateTime BirthDate { get; set; }
}

// Nested object mapping
class OrderItemDto
{
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

class OrderItemEntity
{
    public string ProductName { get; set; } = "";
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
}

class OrderDto
{
    public int OrderId { get; set; }
    public string CustomerName { get; set; } = "";
    public string CustomerEmail { get; set; } = "";
    public List<OrderItemDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

class OrderEntity
{
    public int OrderId { get; set; }
    public Customer Customer { get; set; } = new();
    public List<OrderItemEntity> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

// Collection mapping
class ProductDto
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = "";
    public decimal ProductPrice { get; set; }
}

class ProductEntity
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
}

// Selective mapping with ignored properties
class SourceWithIgnored
{
    public string PublicData { get; set; } = "";
    public string InternalData { get; set; } = "";
    
    [Ignore]
    public string SensitiveData { get; set; } = "";
    
    [Ignore]
    public string ComputedData { get; set; } = "";
}

class TargetWithIgnored
{
    public string PublicData { get; set; } = "";
    public string InternalData { get; set; } = "";
    public string? SensitiveData { get; set; }
    public string? ComputedData { get; set; }
}

// === Attributes ===

[AttributeUsage(AttributeTargets.Property)]
class MapFromAttribute : Attribute
{
    public string SourcePropertyName { get; }
    public MapFromAttribute(string sourcePropertyName)
    {
        SourcePropertyName = sourcePropertyName;
    }
}

[AttributeUsage(AttributeTargets.Property)]
class IgnoreAttribute : Attribute { }

// === Object Mapper Implementation ===

class ObjectMapper
{
    static readonly Dictionary<(Type, Type), PropertyInfo[]> _propertyCache = new();

    public static TTarget Map<TSource, TTarget>(TSource source) where TTarget : new()
    {
        if (source == null) return default!;
        
        var target = new TTarget();
        Map(source, target);
        return target;
    }

    public static void Map<TSource, TTarget>(TSource source, TTarget target)
    {
        if (source == null || target == null) return;

        var sourceType = typeof(TSource);
        var targetType = typeof(TTarget);
        var cacheKey = (sourceType, targetType);

        if (!_propertyCache.TryGetValue(cacheKey, out var sourceProperties))
        {
            sourceProperties = sourceType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            _propertyCache[cacheKey] = sourceProperties;
        }

        var targetProperties = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);

        foreach (var sourceProp in sourceProperties)
        {
            // Check if source property should be ignored
            if (sourceProp.GetCustomAttribute<IgnoreAttribute>() != null)
                continue;

            // Find target property - check for [MapFrom] attribute first
            var targetProp = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .FirstOrDefault(p =>
                {
                    var mapFromAttr = p.GetCustomAttribute<MapFromAttribute>();
                    if (mapFromAttr != null)
                        return mapFromAttr.SourcePropertyName.Equals(sourceProp.Name, StringComparison.OrdinalIgnoreCase);
                    return false;
                });

            // If no [MapFrom] found, try direct name match
            if (targetProp == null)
            {
                targetProperties.TryGetValue(sourceProp.Name, out targetProp);
            }

            if (targetProp != null && targetProp.CanWrite)
            {
                var value = sourceProp.GetValue(source);
                if (value != null)
                {
                    var convertedValue = ConvertValue(value, targetProp.PropertyType);
                    targetProp.SetValue(target, convertedValue);
                }
            }
        }
    }

    public static List<TTarget> MapCollection<TSource, TTarget>(IEnumerable<TSource> source) where TTarget : new()
    {
        return source.Select(Map<TSource, TTarget>).ToList();
    }

    static object? ConvertValue(object? value, Type targetType)
    {
        if (value == null) return null;
        
        var valueType = value.GetType();
        
        // Direct type match
        if (valueType == targetType)
            return value;
        
        // Nullable handling
        var underlyingType = Nullable.GetUnderlyingType(targetType);
        if (underlyingType != null)
            return ConvertValue(value, underlyingType);
        
        // String conversion
        if (value is string str)
        {
            if (targetType == typeof(int)) return int.Parse(str);
            if (targetType == typeof(long)) return long.Parse(str);
            if (targetType == typeof(double)) return double.Parse(str);
            if (targetType == typeof(decimal)) return decimal.Parse(str);
            if (targetType == typeof(bool)) return bool.Parse(str);
            if (targetType == typeof(DateTime)) return DateTime.Parse(str);
            if (targetType == typeof(Guid)) return Guid.Parse(str);
        }
        
        // Numeric conversions
        if (valueType.IsPrimitive && targetType.IsPrimitive)
        {
            return Convert.ChangeType(value, targetType);
        }
        
        // Enum conversion
        if (targetType.IsEnum)
        {
            return Enum.Parse(targetType, value.ToString()!);
        }
        
        // Collection conversion
        if (value is System.Collections.IEnumerable enumerable && !string.IsNullOrEmpty(valueType.Name))
        {
            if (targetType.IsGenericType)
            {
                var genericType = targetType.GetGenericTypeDefinition();
                if (genericType == typeof(List<>) || genericType == typeof(System.Collections.Generic.IEnumerable<>) || genericType == typeof(System.Collections.Generic.ICollection<>))
                {
                    var elementType = targetType.GetGenericArguments()[0];
                    var list = Activator.CreateInstance(targetType)!;
                    var addMethod = list.GetType().GetMethod("Add")!;

                    foreach (var item in enumerable)
                    {
                        addMethod.Invoke(list, new[] { ConvertValue(item, elementType) });
                    }
                    return list;
                }
            }
        }
        
        return value;
    }
}
