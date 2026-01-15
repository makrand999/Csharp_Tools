using System;
using System.Collections.Generic;

namespace Generics;

/// <summary>
/// A generic repository utility demonstrating type-safe collections and operations.
/// Provides CRUD-like operations for any entity type with an ID.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Generic Repository Utility\n");

        // Demo with Employee entities
        var employees = new Repository<Employee>();
        employees.Add(new Employee(1, "Alice", "Engineering", 85000));
        employees.Add(new Employee(2, "Bob", "Marketing", 65000));
        employees.Add(new Employee(3, "Charlie", "Engineering", 92000));
        employees.Add(new Employee(4, "Diana", "Sales", 71000));
        employees.Add(new Employee(5, "Eve", "Marketing", 58000));

        Console.WriteLine("=== Employee Repository ===");
        Console.WriteLine($"Total employees: {employees.Count}");
        Console.WriteLine("\nAll employees:");
        foreach (var emp in employees.GetAll())
            Console.WriteLine($"  {emp.Id}: {emp.Name} ({emp.Department}) - ${emp.Salary:N0}");

        Console.WriteLine("\nFind employee with ID 3:");
        var found = employees.GetById(3);
        if (found != null)
            Console.WriteLine($"  Found: {found.Name} - {found.Department}");

        Console.WriteLine("\nFilter by department (Engineering):");
        var engineers = employees.FindAll(e => e.Department == "Engineering");
        foreach (var e in engineers)
            Console.WriteLine($"  {e.Name} - ${e.Salary:N0}");

        Console.WriteLine("\nHigh earners (salary > 70000):");
        var highEarners = employees.FindAll(e => e.Salary > 70000);
        foreach (var e in highEarners)
            Console.WriteLine($"  {e.Name} - ${e.Salary:N0}");

        // Demo with Product entities
        Console.WriteLine("\n=== Product Repository ===");
        var products = new Repository<Product>();
        products.Add(new Product(101, "Laptop", 999.99m, 50));
        products.Add(new Product(102, "Mouse", 29.99m, 200));
        products.Add(new Product(103, "Keyboard", 79.99m, 150));
        products.Add(new Product(104, "Monitor", 349.99m, 75));

        Console.WriteLine($"Total products: {products.Count}");
        Console.WriteLine("\nAll products:");
        foreach (var p in products.GetAll())
            Console.WriteLine($"  {p.Id}: {p.Name} - ${p.Price:F2} (Stock: {p.Stock})");

        Console.WriteLine("\nExpensive products (> $100):");
        var expensive = products.FindAll(p => p.Price > 100);
        foreach (var p in expensive)
            Console.WriteLine($"  {p.Name} - ${p.Price:F2}");

        Console.WriteLine("\nLow stock items (< 100):");
        var lowStock = products.FindAll(p => p.Stock < 100);
        foreach (var p in lowStock)
            Console.WriteLine($"  {p.Name} - Stock: {p.Stock}");

        // Update and delete demo
        Console.WriteLine("\n=== Update & Delete Demo ===");
        var employee = employees.GetById(1);
        if (employee != null)
        {
            Console.WriteLine($"Before update: {employee.Name} - ${employee.Salary:N0}");
            employee.Salary = 90000;
            employees.Update(employee);
            Console.WriteLine($"After update: {employee.Name} - ${employee.Salary:N0}");
        }

        Console.WriteLine("\nDeleting employee with ID 5...");
        employees.Delete(5);
        Console.WriteLine($"Remaining employees: {employees.Count}");
    }
}

/// <summary>
/// Generic repository providing type-safe CRUD operations.
/// </summary>
/// <typeparam name="T">Entity type that must have an Id property</typeparam>
class Repository<T> where T : class, IHasId
{
    private readonly Dictionary<int, T> _items = new();
    private int _nextId = 1;

    public int Count => _items.Count;

    public void Add(T item)
    {
        if (item.Id == 0)
        {
            // Use reflection to set auto-increment ID
            var idProp = item.GetType().GetProperty(nameof(IHasId.Id));
            idProp?.SetValue(item, _nextId++);
        }
        _items[item.Id] = item;
    }

    public T? GetById(int id) => _items.TryGetValue(id, out var item) ? item : null;

    public IEnumerable<T> GetAll() => _items.Values;

    public IEnumerable<T> FindAll(Func<T, bool> predicate) => _items.Values.Where(predicate);

    public void Update(T item)
    {
        if (item.Id > 0 && _items.ContainsKey(item.Id))
            _items[item.Id] = item;
    }

    public void Delete(int id) => _items.Remove(id);
}

/// <summary>
/// Interface requiring an Id property for repository entities.
/// </summary>
interface IHasId
{
    int Id { get; }
}

/// <summary>
/// Employee entity with department and salary information.
/// </summary>
class Employee : IHasId
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Department { get; set; } = string.Empty;
    public decimal Salary { get; set; }

    public Employee(int id, string name, string department, decimal salary)
    {
        Id = id;
        Name = name;
        Department = department;
        Salary = salary;
    }
}

/// <summary>
/// Product entity with price and stock tracking.
/// </summary>
class Product : IHasId
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Stock { get; set; }

    public Product(int id, string name, decimal price, int stock)
    {
        Id = id;
        Name = name;
        Price = price;
        Stock = stock;
    }
}
