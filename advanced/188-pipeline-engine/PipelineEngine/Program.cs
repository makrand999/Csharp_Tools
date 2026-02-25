using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PipelineEngine;

public interface IPipelineStage<T>
{
    string Name { get; }
    T Process(T input);
}

public class Pipeline<T>
{
    private readonly List<IPipelineStage<T>> _stages = new();
    private readonly Stopwatch _stopwatch = new();

    public Pipeline<T> AddStage(IPipelineStage<T> stage)
    {
        _stages.Add(stage);
        return this;
    }

    public (T Result, PipelineStats Stats) Execute(T input)
    {
        _stopwatch.Restart();
        var stageTimes = new List<StageTime>();

        var current = input;
        foreach (var stage in _stages)
        {
            var stageWatch = Stopwatch.StartNew();
            current = stage.Process(current);
            stageWatch.Stop();
            stageTimes.Add(new StageTime(stage.Name, stageWatch.ElapsedMilliseconds));
        }

        _stopwatch.Stop();
        return (current, new PipelineStats(_stopwatch.ElapsedMilliseconds, stageTimes));
    }
}

public record StageTime(string StageName, long ElapsedMs);
public record PipelineStats(long TotalMs, List<StageTime> StageTimes);

public record Order(string Id, string Customer, decimal Subtotal, decimal Tax, decimal Shipping, decimal Total);

public class CalculateTaxStage : IPipelineStage<Order>
{
    public string Name => "Calculate Tax";
    public Order Process(Order input)
    {
        var taxRate = 0.08m;
        var tax = input.Subtotal * taxRate;
        return input with { Tax = tax };
    }
}

public class CalculateShippingStage : IPipelineStage<Order>
{
    public string Name => "Calculate Shipping";
    public Order Process(Order input)
    {
        var shipping = input.Subtotal > 100 ? 0m : 9.99m;
        return input with { Shipping = shipping };
    }
}

public class CalculateTotalStage : IPipelineStage<Order>
{
    public string Name => "Calculate Total";
    public Order Process(Order input)
    {
        var total = input.Subtotal + input.Tax + input.Shipping;
        return input with { Total = total };
    }
}

public class ValidateOrderStage : IPipelineStage<Order>
{
    public string Name => "Validate Order";
    public Order Process(Order input)
    {
        if (string.IsNullOrWhiteSpace(input.Id))
            throw new ArgumentException("Order ID is required");
        if (string.IsNullOrWhiteSpace(input.Customer))
            throw new ArgumentException("Customer is required");
        if (input.Subtotal < 0)
            throw new ArgumentException("Subtotal cannot be negative");
        return input;
    }
}

public class ApplyDiscountStage : IPipelineStage<Order>
{
    private readonly decimal _discountPercent;

    public ApplyDiscountStage(decimal discountPercent = 10) => _discountPercent = discountPercent;

    public string Name => $"Apply {_discountPercent}% Discount";
    public Order Process(Order input)
    {
        var discountedSubtotal = input.Subtotal * (1 - _discountPercent / 100);
        return input with { Subtotal = discountedSubtotal };
    }
}

public class LogOrderStage : IPipelineStage<Order>
{
    public string Name => "Log Order";
    public Order Process(Order input)
    {
        Console.WriteLine($"  📝 Logged order {input.Id} for customer {input.Customer}");
        return input;
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Pipeline Processing Engine Demo - Order Processing ===\n");

        var orderPipeline = new Pipeline<Order>()
            .AddStage(new ValidateOrderStage())
            .AddStage(new LogOrderStage())
            .AddStage(new ApplyDiscountStage(15))
            .AddStage(new CalculateTaxStage())
            .AddStage(new CalculateShippingStage())
            .AddStage(new CalculateTotalStage());

        var orders = new List<Order>
        {
            new("ORD-001", "Alice", 50m, 0, 0, 0),
            new("ORD-002", "Bob", 150m, 0, 0, 0),
            new("ORD-003", "Charlie", 200m, 0, 0, 0),
        };

        foreach (var order in orders)
        {
            Console.WriteLine($"\n--- Processing Order: {order.Id} ---");
            Console.WriteLine($"  Initial subtotal: ${order.Subtotal}");

            var (result, stats) = orderPipeline.Execute(order);

            Console.WriteLine($"\n  Final Order Details:");
            Console.WriteLine($"    Subtotal: ${result.Subtotal:F2}");
            Console.WriteLine($"    Tax:      ${result.Tax:F2}");
            Console.WriteLine($"    Shipping: ${result.Shipping:F2}");
            Console.WriteLine($"    Total:    ${result.Total:F2}");

            Console.WriteLine($"\n  Pipeline Performance:");
            Console.WriteLine($"    Total time: {stats.TotalMs}ms");
            foreach (var stage in stats.StageTimes)
                Console.WriteLine($"    - {stage.StageName,-25} {stage.ElapsedMs}ms");
        }

        Console.WriteLine("\n--- Building Custom Pipeline ---");
        var simplePipeline = new Pipeline<Order>()
            .AddStage(new ValidateOrderStage())
            .AddStage(new CalculateTotalStage());

        var simpleOrder = new Order("SIMPLE-001", "Test", 100m, 8m, 5m, 0);
        var (simpleResult, _) = simplePipeline.Execute(simpleOrder);
        Console.WriteLine($"  Simple pipeline result: ${simpleResult.Total}");
    }
}
