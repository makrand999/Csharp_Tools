namespace PubSubSystem;

/// <summary>
/// Pub/Sub System - Publish-Subscribe messaging pattern
/// Decouples message producers from consumers using topics
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Pub/Sub Messaging System");
        Console.WriteLine("========================\n");

        var broker = new MessageBroker();

        // Create subscribers
        var subscriber1 = new Subscriber("Email Service");
        var subscriber2 = new Subscriber("SMS Service");
        var subscriber3 = new Subscriber("Analytics Service");

        // Subscribe to topics
        broker.Subscribe("orders", subscriber1);
        broker.Subscribe("orders", subscriber2);
        broker.Subscribe("payments", subscriber1);
        broker.Subscribe("analytics", subscriber3);
        broker.Subscribe("orders", subscriber3);

        Console.WriteLine("--- Subscribers Registered ---\n");

        // Publish messages
        Console.WriteLine("--- Publishing Messages ---\n");

        await broker.PublishAsync("orders", new OrderCreatedEvent { OrderId = 1001, Amount = 99.99m });
        await Task.Delay(100);

        await broker.PublishAsync("payments", new PaymentProcessedEvent { OrderId = 1001, Status = "Success" });
        await Task.Delay(100);

        await broker.PublishAsync("orders", new OrderCreatedEvent { OrderId = 1002, Amount = 249.50m });
        await Task.Delay(100);

        await broker.PublishAsync("analytics", new PageViewEvent { Page = "/products", UserId = 42 });
        await Task.Delay(100);

        // Unsubscribe
        Console.WriteLine("\n--- Unsubscribing SMS Service from orders ---\n");
        broker.Unsubscribe("orders", subscriber2);

        await broker.PublishAsync("orders", new OrderCreatedEvent { OrderId = 1003, Amount = 15.00m });
        await Task.Delay(100);

        // Show statistics
        Console.WriteLine("\n--- Subscriber Statistics ---");
        Console.WriteLine($"Email Service received: {subscriber1.MessageCount} messages");
        Console.WriteLine($"SMS Service received: {subscriber2.MessageCount} messages");
        Console.WriteLine($"Analytics Service received: {subscriber3.MessageCount} messages");
    }
}

interface IEvent { }

record OrderCreatedEvent : IEvent
{
    public int OrderId { get; init; }
    public decimal Amount { get; init; }
}

record PaymentProcessedEvent : IEvent
{
    public int OrderId { get; init; }
    public string Status { get; init; } = "";
}

record PageViewEvent : IEvent
{
    public string Page { get; init; } = "";
    public int UserId { get; init; }
}

interface ISubscriber
{
    string Name { get; }
    Task HandleMessageAsync(string topic, IEvent message);
}

class Subscriber : ISubscriber
{
    public string Name { get; }
    public int MessageCount { get; private set; }

    public Subscriber(string name) => Name = name;

    public Task HandleMessageAsync(string topic, IEvent message)
    {
        MessageCount++;
        Console.WriteLine($"[{Name}] Received on '{topic}': {message}");
        return Task.CompletedTask;
    }
}

class MessageBroker
{
    private readonly Dictionary<string, List<ISubscriber>> _subscriptions = new();
    private readonly object _lock = new();

    public void Subscribe(string topic, ISubscriber subscriber)
    {
        lock (_lock)
        {
            if (!_subscriptions.ContainsKey(topic))
            {
                _subscriptions[topic] = new List<ISubscriber>();
            }
            _subscriptions[topic].Add(subscriber);
            Console.WriteLine($"  {subscriber.Name} subscribed to '{topic}'");
        }
    }

    public void Unsubscribe(string topic, ISubscriber subscriber)
    {
        lock (_lock)
        {
            if (_subscriptions.ContainsKey(topic))
            {
                _subscriptions[topic].Remove(subscriber);
                Console.WriteLine($"  {subscriber.Name} unsubscribed from '{topic}'");
            }
        }
    }

    public async Task PublishAsync(string topic, IEvent message)
    {
        List<ISubscriber> subscribers;
        
        lock (_lock)
        {
            if (!_subscriptions.TryGetValue(topic, out subscribers))
            {
                Console.WriteLine($"  No subscribers for topic '{topic}'");
                return;
            }
            subscribers = new List<ISubscriber>(subscribers);
        }

        Console.WriteLine($"  Publishing to '{topic}': {message}");
        
        var tasks = subscribers.Select(s => s.HandleMessageAsync(topic, message));
        await Task.WhenAll(tasks);
    }
}
