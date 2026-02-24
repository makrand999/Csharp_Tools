using System;
using System.Collections.Generic;
using System.Linq;

namespace ObserverPattern;

public interface IObserver<T>
{
    void Update(T data);
}

public interface IObservable<T>
{
    void Attach(IObserver<T> observer);
    void Detach(IObserver<T> observer);
    void Notify();
}

public class Stock
{
    public string Symbol { get; }
    public decimal Price { get; private set; }

    public Stock(string symbol, decimal price)
    {
        Symbol = symbol;
        Price = price;
    }

    public void UpdatePrice(decimal newPrice)
    {
        var change = newPrice - Price;
        var percentChange = (change / Price) * 100;
        Price = newPrice;
        Console.WriteLine($"\n📈 {Symbol} updated: ${Price} ({(change >= 0 ? "+" : "")}{change:F2}, {(percentChange >= 0 ? "+" : "")}{percentChange:F2}%)");
    }
}

public class StockExchange : IObservable<Stock>
{
    private readonly List<IObserver<Stock>> _observers = new();
    private readonly Stock _stock;

    public StockExchange(Stock stock) => _stock = stock;

    public void Attach(IObserver<Stock> observer)
    {
        _observers.Add(observer);
        Console.WriteLine($"  Observer attached: {observer.GetType().Name}");
    }

    public void Detach(IObserver<Stock> observer)
    {
        _observers.Remove(observer);
        Console.WriteLine($"  Observer detached: {observer.GetType().Name}");
    }

    public void Notify()
    {
        foreach (var observer in _observers)
            observer.Update(_stock);
    }

    public void SetPrice(decimal newPrice)
    {
        _stock.UpdatePrice(newPrice);
        Notify();
    }
}

public class InvestorObserver : IObserver<Stock>
{
    private readonly string _name;
    private readonly decimal _buyThreshold;
    private readonly decimal _sellThreshold;

    public InvestorObserver(string name, decimal buyThreshold, decimal sellThreshold)
    {
        _name = name;
        _buyThreshold = buyThreshold;
        _sellThreshold = sellThreshold;
    }

    public void Update(Stock stock)
    {
        if (stock.Price <= _buyThreshold)
            Console.WriteLine($"  👤 {_name}: BUY signal for {stock.Symbol} at ${stock.Price}!");
        else if (stock.Price >= _sellThreshold)
            Console.WriteLine($"  👤 {_name}: SELL signal for {stock.Symbol} at ${stock.Price}!");
        else
            Console.WriteLine($"  👤 {_name}: Watching {stock.Symbol} at ${stock.Price}");
    }
}

public class AnalyticsObserver : IObserver<Stock>
{
    private readonly List<decimal> _priceHistory = new();

    public void Update(Stock stock)
    {
        _priceHistory.Add(stock.Price);
        var avg = _priceHistory.Average();
        var trend = _priceHistory.Count >= 2 && _priceHistory[^1] > _priceHistory[^2] ? "↑ Rising" : "↓ Falling";
        Console.WriteLine($"  📊 Analytics: {stock.Symbol} avg=${avg:F2}, trend: {trend}");
    }
}

public class AlertObserver : IObserver<Stock>
{
    private readonly decimal _volatilityThreshold;
    private decimal? _lastPrice;

    public AlertObserver(decimal volatilityThreshold) => _volatilityThreshold = volatilityThreshold;

    public void Update(Stock stock)
    {
        if (_lastPrice.HasValue)
        {
            var change = Math.Abs(stock.Price - _lastPrice.Value);
            if (change >= _volatilityThreshold)
                Console.WriteLine($"  ⚠️  ALERT: High volatility detected! {stock.Symbol} changed by ${change:F2}");
        }
        _lastPrice = stock.Price;
    }
}

public class Program
{
    public static void Main()
    {
        Console.WriteLine("=== Observer Pattern Demo - Stock Market ===\n");

        var stock = new Stock("AAPL", 150);
        var exchange = new StockExchange(stock);

        var investor1 = new InvestorObserver("Alice", 145, 160);
        var investor2 = new InvestorObserver("Bob", 140, 155);
        var analytics = new AnalyticsObserver();
        var alert = new AlertObserver(5);

        Console.WriteLine("--- Attaching observers ---");
        exchange.Attach(investor1);
        exchange.Attach(investor2);
        exchange.Attach(analytics);
        exchange.Attach(alert);

        Console.WriteLine("\n--- Simulating price changes ---");
        var prices = new[] { 148m, 152m, 147m, 143m, 158m, 155m, 162m };
        foreach (var price in prices)
            exchange.SetPrice(price);

        Console.WriteLine("\n--- Detaching investor2 ---");
        exchange.Detach(investor2);

        Console.WriteLine("\n--- More price changes ---");
        exchange.SetPrice(165);
        exchange.SetPrice(168);
    }
}
