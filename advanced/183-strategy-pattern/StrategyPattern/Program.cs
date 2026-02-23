using System;
using System.Collections.Generic;

namespace StrategyPattern;

public interface IPaymentStrategy
{
    string Name { get; }
    void ProcessPayment(decimal amount);
    bool SupportsCurrency(string currency);
}

public class CreditCardPayment : IPaymentStrategy
{
    public string Name => "Credit Card";
    public void ProcessPayment(decimal amount) =>
        Console.WriteLine($"  Processing ${amount} via Credit Card (2.9% fee = ${amount * 0.029m})");
    public bool SupportsCurrency(string currency) =>
        new[] { "USD", "EUR", "GBP", "CAD" }.Contains(currency);
}

public class PayPalPayment : IPaymentStrategy
{
    public string Name => "PayPal";
    public void ProcessPayment(decimal amount) =>
        Console.WriteLine($"  Processing ${amount} via PayPal (3.4% fee = ${amount * 0.034m})");
    public bool SupportsCurrency(string currency) =>
        new[] { "USD", "EUR", "GBP", "AUD", "JPY" }.Contains(currency);
}

public class CryptoPayment : IPaymentStrategy
{
    public string Name => "Cryptocurrency";
    public void ProcessPayment(decimal amount) =>
        Console.WriteLine($"  Processing ${amount} via Cryptocurrency (1% fee = ${amount * 0.01m})");
    public bool SupportsCurrency(string currency) =>
        new[] { "BTC", "ETH", "USDT" }.Contains(currency);
}

public class BankTransferPayment : IPaymentStrategy
{
    public string Name => "Bank Transfer";
    public void ProcessPayment(decimal amount) =>
        Console.WriteLine($"  Processing ${amount} via Bank Transfer (flat $5 fee)");
    public bool SupportsCurrency(string currency) =>
        new[] { "USD", "EUR", "GBP", "CHF" }.Contains(currency);
}

public class PaymentContext
{
    private IPaymentStrategy _strategy;

    public PaymentContext(IPaymentStrategy strategy) => _strategy = strategy;

    public void SetStrategy(IPaymentStrategy strategy) => _strategy = strategy;

    public void ExecutePayment(decimal amount)
    {
        Console.WriteLine($"\nPayment Method: {_strategy.Name}");
        _strategy.ProcessPayment(amount);
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Strategy Pattern Demo - Payment Processing ===\n");

        var strategies = new List<IPaymentStrategy>
        {
            new CreditCardPayment(),
            new PayPalPayment(),
            new CryptoPayment(),
            new BankTransferPayment()
        };

        Console.WriteLine("Available payment methods:");
        foreach (var s in strategies)
            Console.WriteLine($"  - {s.Name}");

        var context = new PaymentContext(new CreditCardPayment());

        Console.WriteLine("\n--- Scenario 1: Customer pays $100 with Credit Card ---");
        context.ExecutePayment(100);

        Console.WriteLine("\n--- Scenario 2: Customer switches to PayPal ---");
        context.SetStrategy(new PayPalPayment());
        context.ExecutePayment(250);

        Console.WriteLine("\n--- Scenario 3: International customer uses Crypto ---");
        context.SetStrategy(new CryptoPayment());
        context.ExecutePayment(500);

        Console.WriteLine("\n--- Scenario 4: Large purchase via Bank Transfer ---");
        context.SetStrategy(new BankTransferPayment());
        context.ExecutePayment(5000);

        Console.WriteLine("\n--- Currency Support Check ---");
        foreach (var s in strategies)
        {
            Console.WriteLine($"{s.Name,-20} USD: {s.SupportsCurrency("USD"),-5} | EUR: {s.SupportsCurrency("EUR"),-5} | BTC: {s.SupportsCurrency("BTC"),-5}");
        }
    }
}
