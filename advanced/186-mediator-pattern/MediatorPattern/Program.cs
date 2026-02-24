using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MediatorPattern;

public interface IMediator
{
    Task SendAsync<T>(T message) where T : notnull;
    void Register<T>(Action<T> handler) where T : notnull;
}

public class Mediator : IMediator
{
    private readonly Dictionary<Type, List<Delegate>> _handlers = new();

    public void Register<T>(Action<T> handler) where T : notnull
    {
        var type = typeof(T);
        if (!_handlers.ContainsKey(type))
            _handlers[type] = new List<Delegate>();
        _handlers[type].Add(handler);
    }

    public Task SendAsync<T>(T message) where T : notnull
    {
        var type = typeof(T);
        if (_handlers.TryGetValue(type, out var handlers))
        {
            foreach (var handler in handlers)
                ((Action<T>)handler)(message);
        }
        return Task.CompletedTask;
    }
}

public record UserRegistered(string UserId, string Email, DateTime RegisteredAt);
public record OrderPlaced(string OrderId, string UserId, decimal Amount);
public record PaymentProcessed(string PaymentId, string OrderId, decimal Amount);
public record InventoryUpdated(string ProductId, int QuantityChange);

public class UserService
{
    private readonly IMediator _mediator;

    public UserService(IMediator mediator)
    {
        _mediator = mediator;
        _mediator.Register<UserRegistered>(HandleUserRegistered);
    }

    private void HandleUserRegistered(UserRegistered e) =>
        Console.WriteLine($"  👤 UserService: Created user {e.UserId} ({e.Email}) at {e.RegisteredAt:HH:mm:ss}");
}

public class EmailService
{
    private readonly IMediator _mediator;

    public EmailService(IMediator mediator)
    {
        _mediator = mediator;
        _mediator.Register<UserRegistered>(HandleUserRegistered);
        _mediator.Register<OrderPlaced>(HandleOrderPlaced);
        _mediator.Register<PaymentProcessed>(HandlePaymentProcessed);
    }

    private void HandleUserRegistered(UserRegistered e) =>
        Console.WriteLine($"  📧 EmailService: Sent welcome email to {e.Email}");

    private void HandleOrderPlaced(OrderPlaced e) =>
        Console.WriteLine($"  📧 EmailService: Sent order confirmation #{e.OrderId} to user {e.UserId}");

    private void HandlePaymentProcessed(PaymentProcessed e) =>
        Console.WriteLine($"  📧 EmailService: Sent payment receipt for ${e.Amount} (Order: {e.OrderId})");
}

public class OrderService
{
    private readonly IMediator _mediator;

    public OrderService(IMediator mediator)
    {
        _mediator = mediator;
        _mediator.Register<OrderPlaced>(HandleOrderPlaced);
    }

    private void HandleOrderPlaced(OrderPlaced e) =>
        Console.WriteLine($"  📦 OrderService: Processing order {e.OrderId} for ${e.Amount}");
}

public class PaymentService
{
    private readonly IMediator _mediator;

    public PaymentService(IMediator mediator)
    {
        _mediator = mediator;
        _mediator.Register<OrderPlaced>(HandleOrderPlaced);
        _mediator.Register<PaymentProcessed>(HandlePaymentProcessed);
    }

    private void HandleOrderPlaced(OrderPlaced e)
    {
        Console.WriteLine($"  💳 PaymentService: Charging ${e.Amount} for order {e.OrderId}");
        var paymentId = $"PAY-{Guid.NewGuid().ToString()[..8]}";
        _mediator.SendAsync(new PaymentProcessed(paymentId, e.OrderId, e.Amount));
    }

    private void HandlePaymentProcessed(PaymentProcessed e) =>
        Console.WriteLine($"  💳 PaymentService: Payment {e.PaymentId} completed successfully");
}

public class InventoryService
{
    private readonly IMediator _mediator;

    public InventoryService(IMediator mediator)
    {
        _mediator = mediator;
        _mediator.Register<OrderPlaced>(HandleOrderPlaced);
        _mediator.Register<InventoryUpdated>(HandleInventoryUpdated);
    }

    private void HandleOrderPlaced(OrderPlaced e)
    {
        Console.WriteLine($"  📊 InventoryService: Reserving items for order {e.OrderId}");
        _mediator.SendAsync(new InventoryUpdated("PROD-001", -1));
    }

    private void HandleInventoryUpdated(InventoryUpdated e) =>
        Console.WriteLine($"  📊 InventoryService: Updated {e.ProductId} by {e.QuantityChange:+#;-#;0}");
}

public class AnalyticsService
{
    private readonly IMediator _mediator;
    private int _totalOrders;
    private decimal _totalRevenue;

    public AnalyticsService(IMediator mediator)
    {
        _mediator = mediator;
        _mediator.Register<UserRegistered>(HandleUserRegistered);
        _mediator.Register<OrderPlaced>(HandleOrderPlaced);
        _mediator.Register<PaymentProcessed>(HandlePaymentProcessed);
    }

    private void HandleUserRegistered(UserRegistered e) =>
        Console.WriteLine($"  📈 Analytics: New user registered (Total users tracked)");

    private void HandleOrderPlaced(OrderPlaced e)
    {
        _totalOrders++;
        Console.WriteLine($"  📈 Analytics: Order #{_totalOrders} placed");
    }

    private void HandlePaymentProcessed(PaymentProcessed e)
    {
        _totalRevenue += e.Amount;
        Console.WriteLine($"  📈 Analytics: Revenue updated. Total: ${_totalRevenue}");
    }
}

public class Program
{
    public static async Task Main()
    {
        Console.WriteLine("=== Mediator Pattern Demo - E-commerce Event Bus ===\n");

        var mediator = new Mediator();

        _ = new UserService(mediator);
        var emailService = new EmailService(mediator);
        _ = new OrderService(mediator);
        _ = new PaymentService(mediator);
        _ = new InventoryService(mediator);
        _ = new AnalyticsService(mediator);

        Console.WriteLine("--- Simulating user registration ---");
        await mediator.SendAsync(new UserRegistered("USR-001", "john@example.com", DateTime.Now));

        Console.WriteLine("\n--- Simulating order placement ---");
        await mediator.SendAsync(new OrderPlaced("ORD-001", "USR-001", 99.99m));

        Console.WriteLine("\n--- Simulating another order ---");
        await mediator.SendAsync(new OrderPlaced("ORD-002", "USR-001", 149.50m));

        Console.WriteLine("\n--- Simulating second user registration ---");
        await mediator.SendAsync(new UserRegistered("USR-002", "jane@example.com", DateTime.Now));
    }
}
