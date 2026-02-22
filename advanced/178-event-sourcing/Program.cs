namespace EventSourcing;

/// <summary>
/// Event Sourcing - Store state changes as immutable events
/// Rebuild state by replaying events from the beginning
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Event Sourcing Pattern");
        Console.WriteLine("======================\n");

        var store = new EventStore();
        var projector = new BankAccountProjector(store);

        // Create account and perform operations
        var accountId = Guid.NewGuid();
        
        Console.WriteLine("--- Creating Account ---");
        await projector.CreateAccountAsync(accountId, "John Doe", 1000m);
        PrintAccount(projector.GetAccount(accountId));

        Console.WriteLine("\n--- Making Deposits ---");
        await projector.DepositAsync(accountId, 500m);
        await projector.DepositAsync(accountId, 250m);
        PrintAccount(projector.GetAccount(accountId));

        Console.WriteLine("\n--- Making Withdrawals ---");
        await projector.WithdrawAsync(accountId, 200m);
        await projector.WithdrawAsync(accountId, 100m);
        PrintAccount(projector.GetAccount(accountId));

        Console.WriteLine("\n--- Attempting Overdraft ---");
        try
        {
            await projector.WithdrawAsync(accountId, 2000m);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Rejected: {ex.Message}");
        }
        PrintAccount(projector.GetAccount(accountId));

        // Show event history
        Console.WriteLine("\n--- Event History ---");
        var events = store.GetEvents(accountId);
        foreach (var evt in events)
        {
            Console.WriteLine($"  [{evt.EventType}] {evt.Data}");
        }

        // Rebuild from events
        Console.WriteLine("\n--- Rebuilding State from Events ---");
        var rebuiltProjector = new BankAccountProjector(store);
        rebuiltProjector.Rebuild();
        
        var rebuilt = rebuiltProjector.GetAccount(accountId);
        Console.WriteLine($"Rebuilt account matches: {rebuilt?.ToString() == projector.GetAccount(accountId)?.ToString()}");
        PrintAccount(rebuilt);

        // Show all streams
        Console.WriteLine("\n--- Event Store Statistics ---");
        Console.WriteLine($"Total streams: {store.GetStreamCount()}");
        Console.WriteLine($"Total events: {store.GetTotalEventCount()}");
    }

    static void PrintAccount(BankAccount? account)
    {
        if (account == null)
        {
            Console.WriteLine("  Account not found");
            return;
        }
        Console.WriteLine($"  Account: {account.Id}[..8] | Owner: {account.Owner} | Balance: ${account.Balance:F2}");
    }
}

interface IEvent
{
    Guid AggregateId { get; }
    DateTime Timestamp { get; }
}

record AccountCreatedEvent : IEvent
{
    public Guid AggregateId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public string Owner { get; init; } = "";
    public decimal InitialBalance { get; init; }
}

record MoneyDepositedEvent : IEvent
{
    public Guid AggregateId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public decimal Amount { get; init; }
}

record MoneyWithdrawnEvent : IEvent
{
    public Guid AggregateId { get; init; }
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;
    public decimal Amount { get; init; }
}

record EventEnvelope
{
    public Guid Id { get; init; }
    public Guid AggregateId { get; init; }
    public string EventType { get; init; } = "";
    public string Data { get; init; } = "";
    public long Version { get; init; }
}

class EventStore
{
    private readonly Dictionary<Guid, List<EventEnvelope>> _streams = new();
    private readonly object _lock = new();

    public void Append(Guid streamId, IEvent evt)
    {
        lock (_lock)
        {
            if (!_streams.ContainsKey(streamId))
            {
                _streams[streamId] = new List<EventEnvelope>();
            }

            var envelope = new EventEnvelope
            {
                Id = Guid.NewGuid(),
                AggregateId = evt.AggregateId,
                EventType = evt.GetType().Name,
                Data = System.Text.Json.JsonSerializer.Serialize(evt),
                Version = _streams[streamId].Count + 1
            };

            _streams[streamId].Add(envelope);
        }
    }

    public IReadOnlyList<EventEnvelope> GetEvents(Guid streamId)
    {
        lock (_lock)
        {
            return _streams.TryGetValue(streamId, out var events) 
                ? events.AsReadOnly() 
                : Array.Empty<EventEnvelope>();
        }
    }

    public int GetStreamCount() => _streams.Count;
    public long GetTotalEventCount() => _streams.Values.Sum(e => e.Count);
}

class BankAccount
{
    public Guid Id { get; init; }
    public string Owner { get; set; } = "";
    public decimal Balance { get; set; }
    public long Version { get; set; }

    public override string ToString() => $"{Id.ToString()[..8]} | {Owner} | ${Balance:F2} | v{Version}";
}

class BankAccountProjector
{
    private readonly EventStore _store;
    private readonly Dictionary<Guid, BankAccount> _accounts = new();

    public BankAccountProjector(EventStore store) => _store = store;

    public BankAccount? GetAccount(Guid id) => _accounts.GetValueOrDefault(id);

    public async Task CreateAccountAsync(Guid id, string owner, decimal initialBalance)
    {
        var evt = new AccountCreatedEvent 
        { 
            AggregateId = id, 
            Owner = owner, 
            InitialBalance = initialBalance 
        };
        _store.Append(id, evt);
        Apply(evt);
        await Task.CompletedTask;
    }

    public async Task DepositAsync(Guid id, decimal amount)
    {
        var evt = new MoneyDepositedEvent { AggregateId = id, Amount = amount };
        _store.Append(id, evt);
        Apply(evt);
        await Task.CompletedTask;
    }

    public async Task WithdrawAsync(Guid id, decimal amount)
    {
        var account = GetAccount(id);
        if (account == null || account.Balance < amount)
        {
            throw new InvalidOperationException("Insufficient funds");
        }

        var evt = new MoneyWithdrawnEvent { AggregateId = id, Amount = amount };
        _store.Append(id, evt);
        Apply(evt);
        await Task.CompletedTask;
    }

    public void Rebuild()
    {
        // In a real system, we'd iterate all streams
        // For demo, we rebuild known accounts
        foreach (var kvp in _accounts.ToList())
        {
            var events = _store.GetEvents(kvp.Key);
            var account = new BankAccount 
            { 
                Id = kvp.Key, 
                Owner = kvp.Value.Owner, 
                Balance = 0,
                Version = 0
            };

            foreach (var evt in events)
            {
                ApplyEvent(account, evt.EventType, evt.Data);
            }
            _accounts[kvp.Key] = account;
        }
    }

    void Apply(IEvent evt)
    {
        switch (evt)
        {
            case AccountCreatedEvent e:
                _accounts[e.AggregateId] = new BankAccount
                {
                    Id = e.AggregateId,
                    Owner = e.Owner,
                    Balance = e.InitialBalance,
                    Version = 1
                };
                break;
            case MoneyDepositedEvent e:
                var depositAccount = _accounts[e.AggregateId];
                depositAccount.Balance += e.Amount;
                depositAccount.Version++;
                break;
            case MoneyWithdrawnEvent e:
                var withdrawAccount = _accounts[e.AggregateId];
                withdrawAccount.Balance -= e.Amount;
                withdrawAccount.Version++;
                break;
        }
    }

    void ApplyEvent(BankAccount account, string eventType, string data)
    {
        switch (eventType)
        {
            case nameof(AccountCreatedEvent):
                var created = System.Text.Json.JsonSerializer.Deserialize<AccountCreatedEvent>(data)!;
                account.Owner = created.Owner;
                account.Balance = created.InitialBalance;
                account.Version++;
                break;
            case nameof(MoneyDepositedEvent):
                var deposited = System.Text.Json.JsonSerializer.Deserialize<MoneyDepositedEvent>(data)!;
                account.Balance += deposited.Amount;
                account.Version++;
                break;
            case nameof(MoneyWithdrawnEvent):
                var withdrawn = System.Text.Json.JsonSerializer.Deserialize<MoneyWithdrawnEvent>(data)!;
                account.Balance -= withdrawn.Amount;
                account.Version++;
                break;
        }
    }
}
