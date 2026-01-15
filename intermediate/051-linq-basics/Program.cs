using System;
using System.Linq;

namespace LinqBasics;

/// <summary>
/// A practical LINQ utility for filtering, transforming, and analyzing data collections.
/// Demonstrates real-world LINQ operations on sample datasets.
/// </summary>
class Program
{
    record Product(string Name, decimal Price, string Category, int Stock);

    static void Main(string[] args)
    {
        var products = new List<Product>
        {
            new("Laptop", 999.99m, "Electronics", 50),
            new("Mouse", 29.99m, "Electronics", 200),
            new("Keyboard", 79.99m, "Electronics", 150),
            new("Desk Chair", 249.99m, "Furniture", 30),
            new("Standing Desk", 599.99m, "Furniture", 25),
            new("Monitor", 349.99m, "Electronics", 75),
            new("Notebook", 4.99m, "Office", 500),
            new("Pen Set", 12.99m, "Office", 300),
            new("Headphones", 149.99m, "Electronics", 100),
            new("Webcam", 89.99m, "Electronics", 80)
        };

        if (args.Length == 0)
        {
            ShowMenu();
            return;
        }

        string command = args[0].ToLower();

        switch (command)
        {
            case "filter":
                FilterByCategory(products, args.ElementAtOrDefault(1));
                break;
            case "expensive":
                ShowExpensive(products, decimal.TryParse(args.ElementAtOrDefault(1), out var min) ? min : 100m);
                break;
            case "lowstock":
                ShowLowStock(products, int.TryParse(args.ElementAtOrDefault(1), out var threshold) ? threshold : 50);
                break;
            case "group":
                GroupByCategory(products);
                break;
            case "stats":
                ShowStatistics(products);
                break;
            case "search":
                SearchProducts(products, string.Join(" ", args.Skip(1)));
                break;
            default:
                Console.WriteLine($"Unknown command: {command}");
                ShowMenu();
                break;
        }
    }

    static void ShowMenu()
    {
        Console.WriteLine("LINQ Data Utility - Available Commands:");
        Console.WriteLine("  filter <category>     - Filter products by category");
        Console.WriteLine("  expensive [minPrice]  - Show products above price threshold");
        Console.WriteLine("  lowstock [threshold]  - Show products with low stock");
        Console.WriteLine("  group                 - Group products by category");
        Console.WriteLine("  stats                 - Show statistics");
        Console.WriteLine("  search <term>         - Search products by name");
        Console.WriteLine();
        Console.WriteLine("Example: dotnet run -- filter Electronics");
    }

    static void FilterByCategory(List<Product> products, string? category)
    {
        if (string.IsNullOrEmpty(category))
        {
            Console.WriteLine("Please specify a category (Electronics, Furniture, Office)");
            return;
        }

        var filtered = products
            .Where(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Name)
            .ToList();

        Console.WriteLine($"Products in '{category}':");
        foreach (var p in filtered)
            Console.WriteLine($"  {p.Name,-20} ${p.Price,8:F2}  Stock: {p.Stock}");
    }

    static void ShowExpensive(List<Product> products, decimal minPrice)
    {
        var expensive = products
            .Where(p => p.Price >= minPrice)
            .OrderByDescending(p => p.Price)
            .ToList();

        Console.WriteLine($"Products >= ${minPrice:F2}:");
        foreach (var p in expensive)
            Console.WriteLine($"  {p.Name,-20} ${p.Price,8:F2}  [{p.Category}]");
    }

    static void ShowLowStock(List<Product> products, int threshold)
    {
        var lowStock = products
            .Where(p => p.Stock < threshold)
            .OrderBy(p => p.Stock)
            .ToList();

        Console.WriteLine($"Products with stock < {threshold}:");
        foreach (var p in lowStock)
            Console.WriteLine($"  {p.Name,-20} Stock: {p.Stock}  ${p.Price:F2}");
    }

    static void GroupByCategory(List<Product> products)
    {
        var grouped = products.GroupBy(p => p.Category);

        foreach (var group in grouped)
        {
            Console.WriteLine($"\n{group.Key}:");
            foreach (var p in group.OrderBy(p => p.Name))
                Console.WriteLine($"  {p.Name,-20} ${p.Price,8:F2}  Stock: {p.Stock}");
        }
    }

    static void ShowStatistics(List<Product> products)
    {
        Console.WriteLine("Product Statistics:");
        Console.WriteLine($"  Total Products:     {products.Count}");
        Console.WriteLine($"  Total Value:        ${products.Sum(p => p.Price * p.Stock):F2}");
        Console.WriteLine($"  Average Price:      ${products.Average(p => p.Price):F2}");
        Console.WriteLine($"  Most Expensive:     {products.MaxBy(p => p.Price)?.Name} (${products.Max(p => p.Price):F2})");
        Console.WriteLine($"  Cheapest:           {products.MinBy(p => p.Price)?.Name} (${products.Min(p => p.Price):F2})");
        Console.WriteLine($"  Total Stock:        {products.Sum(p => p.Stock)} units");
        Console.WriteLine($"  Categories:         {products.Select(p => p.Category).Distinct().Count()}");
    }

    static void SearchProducts(List<Product> products, string term)
    {
        if (string.IsNullOrEmpty(term))
        {
            Console.WriteLine("Please specify a search term");
            return;
        }

        var results = products
            .Where(p => p.Name.Contains(term, StringComparison.OrdinalIgnoreCase))
            .OrderBy(p => p.Name)
            .ToList();

        Console.WriteLine($"Search results for '{term}':");
        if (results.Count == 0)
            Console.WriteLine("  No products found");
        else
            foreach (var p in results)
                Console.WriteLine($"  {p.Name,-20} ${p.Price,8:F2}  [{p.Category}]");
    }
}
