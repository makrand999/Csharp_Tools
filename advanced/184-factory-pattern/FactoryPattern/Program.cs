using System;
using System.Collections.Generic;

namespace FactoryPattern;

public interface INotification
{
    string Channel { get; }
    void Send(string recipient, string message);
}

public class EmailNotification : INotification
{
    public string Channel => "Email";
    public void Send(string recipient, string message) =>
        Console.WriteLine($"  📧 Email sent to {recipient}: {message}");
}

public class SmsNotification : INotification
{
    public string Channel => "SMS";
    public void Send(string recipient, string message) =>
        Console.WriteLine($"  📱 SMS sent to {recipient}: {message}");
}

public class PushNotification : INotification
{
    public string Channel => "Push";
    public void Send(string recipient, string message) =>
        Console.WriteLine($"  🔔 Push notification to {recipient}: {message}");
}

public class SlackNotification : INotification
{
    public string Channel => "Slack";
    public void Send(string recipient, string message) =>
        Console.WriteLine($"  💬 Slack message to {recipient}: {message}");
}

public interface INotificationFactory
{
    INotification Create();
}

public class EmailFactory : INotificationFactory
{
    public INotification Create() => new EmailNotification();
}

public class SmsFactory : INotificationFactory
{
    public INotification Create() => new SmsNotification();
}

public class PushFactory : INotificationFactory
{
    public INotification Create() => new PushNotification();
}

public class SlackFactory : INotificationFactory
{
    public INotification Create() => new SlackNotification();
}

public class NotificationService
{
    private readonly Dictionary<string, INotificationFactory> _factories = new();

    public void RegisterFactory(string channel, INotificationFactory factory) =>
        _factories[channel.ToLower()] = factory;

    public INotification CreateNotification(string channel)
    {
        if (_factories.TryGetValue(channel.ToLower(), out var factory))
            return factory.Create();
        throw new ArgumentException($"Unknown notification channel: {channel}");
    }

    public void SendNotification(string channel, string recipient, string message)
    {
        var notification = CreateNotification(channel);
        notification.Send(recipient, message);
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Factory Pattern Demo - Notification System ===\n");

        var service = new NotificationService();
        service.RegisterFactory("email", new EmailFactory());
        service.RegisterFactory("sms", new SmsFactory());
        service.RegisterFactory("push", new PushFactory());
        service.RegisterFactory("slack", new SlackFactory());

        Console.WriteLine("Registered channels: " + string.Join(", ", new[] { "email", "sms", "push", "slack" }));

        Console.WriteLine("\n--- Sending notifications via different channels ---");
        service.SendNotification("email", "user@example.com", "Welcome to our service!");
        service.SendNotification("sms", "+1234567890", "Your verification code is 123456");
        service.SendNotification("push", "device-token-abc", "New message received");
        service.SendNotification("slack", "#general", "Deployment completed successfully");

        Console.WriteLine("\n--- Creating notifications directly from factories ---");
        var factories = new Dictionary<string, INotificationFactory>
        {
            ["Email"] = new EmailFactory(),
            ["SMS"] = new SmsFactory(),
            ["Push"] = new PushFactory(),
            ["Slack"] = new SlackFactory()
        };

        foreach (var kvp in factories)
        {
            var notification = kvp.Value.Create();
            Console.WriteLine($"  {kvp.Key} channel created: {notification.Channel}");
        }
    }
}
