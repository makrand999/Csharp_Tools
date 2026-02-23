namespace RepositoryPattern;

/// <summary>
/// Repository Pattern with Unit of Work - Data access abstraction
/// Decouples business logic from data access concerns
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Repository Pattern with Unit of Work");
        Console.WriteLine("====================================\n");

        var context = new AppDbContext();
        var unitOfWork = new UnitOfWork(context);

        // Add users
        Console.WriteLine("--- Adding Users ---\n");
        
        var user1 = new User { Name = "Alice", Email = "alice@example.com" };
        var user2 = new User { Name = "Bob", Email = "bob@example.com" };
        var user3 = new User { Name = "Charlie", Email = "charlie@example.com" };

        await unitOfWork.Users.AddAsync(user1);
        await unitOfWork.Users.AddAsync(user2);
        await unitOfWork.Users.AddAsync(user3);
        await unitOfWork.SaveAsync();

        // Query users
        Console.WriteLine("--- All Users ---\n");
        var users = await unitOfWork.Users.GetAllAsync();
        foreach (var user in users)
        {
            Console.WriteLine($"  [{user.Id}] {user.Name} <{user.Email}>");
        }

        // Find specific user
        Console.WriteLine("\n--- Find by Email ---\n");
        var found = await unitOfWork.Users.FirstOrDefaultAsync(u => u.Email.Contains("alice"));
        Console.WriteLine($"  Found: {found?.Name} <{found?.Email}>");

        // Add orders for users (demonstrating multiple repositories)
        Console.WriteLine("\n--- Adding Orders ---\n");
        
        var order1 = new Order { UserId = user1.Id, Total = 99.99m, Status = OrderStatus.Pending };
        var order2 = new Order { UserId = user1.Id, Total = 149.50m, Status = OrderStatus.Completed };
        var order3 = new Order { UserId = user2.Id, Total = 29.99m, Status = OrderStatus.Pending };

        await unitOfWork.Orders.AddAsync(order1);
        await unitOfWork.Orders.AddAsync(order2);
        await unitOfWork.Orders.AddAsync(order3);
        await unitOfWork.SaveAsync();

        // Query with specification
        Console.WriteLine("--- Orders with Specification ---\n");
        
        var pendingOrders = await unitOfWork.Orders.FindAsync(new PendingOrdersSpecification());
        Console.WriteLine("Pending Orders:");
        foreach (var order in pendingOrders)
        {
            Console.WriteLine($"  Order #{order.Id} - User {order.UserId} - ${order.Total:F2}");
        }

        var highValueOrders = await unitOfWork.Orders.FindAsync(new HighValueOrdersSpecification(100));
        Console.WriteLine("\nHigh Value Orders (>$100):");
        foreach (var order in highValueOrders)
        {
            Console.WriteLine($"  Order #{order.Id} - User {order.UserId} - ${order.Total:F2}");
        }

        // Update within transaction
        Console.WriteLine("\n--- Transactional Update ---\n");
        
        await unitOfWork.BeginTransactionAsync();
        try
        {
            var alice = await unitOfWork.Users.FirstOrDefaultAsync(u => u.Name == "Alice");
            if (alice != null)
            {
                alice.Email = "alice.new@example.com";
                unitOfWork.Users.Update(alice);
                
                var newOrder = new Order { UserId = alice.Id, Total = 199.99m, Status = OrderStatus.Pending };
                await unitOfWork.Orders.AddAsync(newOrder);
                
                await unitOfWork.SaveAsync();
                await unitOfWork.CommitTransactionAsync();
                
                Console.WriteLine("  Transaction committed successfully");
            }
        }
        catch
        {
            await unitOfWork.RollbackTransactionAsync();
            Console.WriteLine("  Transaction rolled back");
            throw;
        }

        // Final state
        Console.WriteLine("\n--- Final State ---\n");
        users = await unitOfWork.Users.GetAllAsync();
        foreach (var user in users)
        {
            var orderCount = (await unitOfWork.Orders.FindAsync(o => o.UserId == user.Id)).Count();
            Console.WriteLine($"  {user.Name}: {orderCount} orders");
        }

        Console.WriteLine("\n--- Repository Pattern Benefits ---");
        Console.WriteLine("✓ Abstracts data access logic");
        Console.WriteLine("✓ Enables unit testing with mocks");
        Console.WriteLine("✓ Centralizes data access code");
        Console.WriteLine("✓ Unit of Work ensures consistency");
    }
}

// Domain Entities
class User
{
    public int Id { get; init; }
    public string Name { get; set; } = "";
    public string Email { get; set; } = "";
}

class Order
{
    public int Id { get; init; }
    public int UserId { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; }
}

enum OrderStatus { Pending, Completed, Cancelled }

// Specification Pattern for complex queries
interface ISpecification<T>
{
    bool IsSatisfiedBy(T entity);
}

class PendingOrdersSpecification : ISpecification<Order>
{
    public bool IsSatisfiedBy(Order entity) => entity.Status == OrderStatus.Pending;
}

class HighValueOrdersSpecification : ISpecification<Order>
{
    private readonly decimal _minAmount;
    public HighValueOrdersSpecification(decimal minAmount) => _minAmount = minAmount;
    public bool IsSatisfiedBy(Order entity) => entity.Total >= _minAmount;
}

// Generic Repository
interface IRepository<T> where T : class
{
    Task<IEnumerable<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<IEnumerable<T>> FindAsync(ISpecification<T> specification);
    Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate);
    Task<T?> FirstOrDefaultAsync(Func<T, bool> predicate);
    Task AddAsync(T entity);
    void Update(T entity);
    void Remove(T entity);
}

class Repository<T> : IRepository<T> where T : class
{
    protected readonly AppDbContext _context;
    protected readonly List<T> _entities = new();

    public Repository(AppDbContext context) => _context = context;

    public virtual Task<IEnumerable<T>> GetAllAsync() => 
        Task.FromResult(_entities.AsEnumerable());

    public Task<T?> GetByIdAsync(int id) => 
        Task.FromResult(_entities.FirstOrDefault(e => GetId(e) == id));

    public Task<IEnumerable<T>> FindAsync(ISpecification<T> specification) => 
        Task.FromResult(_entities.Where(specification.IsSatisfiedBy));

    public Task<IEnumerable<T>> FindAsync(Func<T, bool> predicate) => 
        Task.FromResult(_entities.Where(predicate));

    public Task<T?> FirstOrDefaultAsync(Func<T, bool> predicate) => 
        Task.FromResult(_entities.FirstOrDefault(predicate));

    public Task AddAsync(T entity)
    {
        _entities.Add(entity);
        return Task.CompletedTask;
    }

    public void Update(T entity) { /* Mark as modified */ }
    public void Remove(T entity) => _entities.Remove(entity);

    protected virtual int GetId(T entity)
    {
        var prop = entity.GetType().GetProperty("Id");
        return prop != null ? (int)prop.GetValue(entity)! : 0;
    }
}

// Specific Repositories
class UserRepository : Repository<User>
{
    public UserRepository(AppDbContext context) : base(context) { }
}

class OrderRepository : Repository<Order>
{
    public OrderRepository(AppDbContext context) : base(context) { }
}

// Unit of Work
class UnitOfWork : IDisposable
{
    private readonly AppDbContext _context;
    private bool _disposed;

    public UserRepository Users { get; }
    public OrderRepository Orders { get; }

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
        Users = new UserRepository(context);
        Orders = new OrderRepository(context);
    }

    public Task SaveAsync() => Task.CompletedTask; // In real app, persist to DB

    public Task BeginTransactionAsync()
    {
        Console.WriteLine("  Transaction started");
        return Task.CompletedTask;
    }

    public Task CommitTransactionAsync()
    {
        Console.WriteLine("  Transaction committed");
        return Task.CompletedTask;
    }

    public Task RollbackTransactionAsync()
    {
        Console.WriteLine("  Transaction rolled back");
        return Task.CompletedTask;
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

// Fake database context
class AppDbContext { }
