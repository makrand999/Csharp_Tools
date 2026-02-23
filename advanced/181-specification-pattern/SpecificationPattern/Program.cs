using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace SpecificationPattern;

public record Product(string Name, decimal Price, int Stock, string Category, bool IsActive);

public interface ISpecification<T>
{
    bool IsSatisfiedBy(T item);
    Expression<Func<T, bool>> ToExpression();
    ISpecification<T> And(ISpecification<T> other);
    ISpecification<T> Or(ISpecification<T> other);
    ISpecification<T> Not();
}

public class Specification<T> : ISpecification<T>
{
    private readonly Expression<Func<T, bool>> _expression;

    public Specification(Expression<Func<T, bool>> expression) => _expression = expression;

    public bool IsSatisfiedBy(T item) => _expression.Compile().Invoke(item);

    public Expression<Func<T, bool>> ToExpression() => _expression;

    public ISpecification<T> And(ISpecification<T> other)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var newExpr = Expression.Lambda<Func<T, bool>>(
            Expression.AndAlso(
                Expression.Invoke(_expression, param),
                Expression.Invoke(other.ToExpression(), param)
            ),
            param
        );
        return new Specification<T>(newExpr);
    }

    public ISpecification<T> Or(ISpecification<T> other)
    {
        var param = Expression.Parameter(typeof(T), "x");
        var newExpr = Expression.Lambda<Func<T, bool>>(
            Expression.OrElse(
                Expression.Invoke(_expression, param),
                Expression.Invoke(other.ToExpression(), param)
            ),
            param
        );
        return new Specification<T>(newExpr);
    }

    public ISpecification<T> Not()
    {
        var param = Expression.Parameter(typeof(T), "x");
        var newExpr = Expression.Lambda<Func<T, bool>>(
            Expression.Not(
                Expression.Invoke(_expression, param)
            ),
            param
        );
        return new Specification<T>(newExpr);
    }
}

public static class ProductSpecs
{
    public static ISpecification<Product> InStock() =>
        new Specification<Product>(p => p.Stock > 0);

    public static ISpecification<Product> Active() =>
        new Specification<Product>(p => p.IsActive);

    public static ISpecification<Product> PriceRange(decimal min, decimal max) =>
        new Specification<Product>(p => p.Price >= min && p.Price <= max);

    public static ISpecification<Product> Category(string category) =>
        new Specification<Product>(p => p.Category.Equals(category, StringComparison.OrdinalIgnoreCase));
}

public class Program
{
    private static readonly List<Product> Products = new()
    {
        new("Laptop", 999.99m, 10, "Electronics", true),
        new("Mouse", 29.99m, 50, "Electronics", true),
        new("Desk Chair", 199.99m, 0, "Furniture", true),
        new("Coffee Maker", 79.99m, 25, "Appliances", false),
        new("Keyboard", 89.99m, 30, "Electronics", true),
        new("Monitor", 349.99m, 5, "Electronics", true),
        new("Desk Lamp", 45.99m, 15, "Furniture", true),
        new("Blender", 59.99m, 0, "Appliances", true),
    };

    public static void Main()
    {
        Console.WriteLine("=== Specification Pattern Demo ===\n");

        var inStockAndActive = ProductSpecs.InStock().And(ProductSpecs.Active());
        var electronics = ProductSpecs.Category("Electronics");
        var affordable = ProductSpecs.PriceRange(0, 100);

        Console.WriteLine("Products in stock AND active:");
        var results = Products.Where(p => inStockAndActive.IsSatisfiedBy(p)).ToList();
        PrintProducts(results);

        Console.WriteLine("\nElectronics OR affordable items:");
        var electronicsOrAffordable = electronics.Or(affordable);
        results = Products.Where(p => electronicsOrAffordable.IsSatisfiedBy(p)).ToList();
        PrintProducts(results);

        Console.WriteLine("\nNOT in Electronics category:");
        var notElectronics = electronics.Not();
        results = Products.Where(p => notElectronics.IsSatisfiedBy(p)).ToList();
        PrintProducts(results);

        Console.WriteLine("\nComplex query: Electronics AND (in stock OR active):");
        var complex = electronics.And(ProductSpecs.InStock().Or(ProductSpecs.Active()));
        results = Products.Where(p => complex.IsSatisfiedBy(p)).ToList();
        PrintProducts(results);
    }

    private static void PrintProducts(IEnumerable<Product> products)
    {
        foreach (var p in products)
            Console.WriteLine($"  {p.Name,-15} ${p.Price,8} | Stock: {p.Stock,3} | {p.Category,-12} | {(p.IsActive ? "Active" : "Inactive")}");
    }
}
