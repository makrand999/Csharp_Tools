namespace CqrsPattern;

/// <summary>
/// CQRS Pattern - Command Query Responsibility Segregation
/// Separate read and write operations for better scalability
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("CQRS Pattern Implementation");
        Console.WriteLine("===========================\n");

        var commandBus = new CommandBus();
        var queryBus = new QueryBus();
        
        // Register handlers
        var productStore = new ProductStore();
        ProductCommandHandlers.Register(commandBus, productStore);
        ProductQueryHandlers.Register(queryBus, productStore);

        // Execute commands (writes)
        Console.WriteLine("--- Creating Products (Commands) ---\n");
        
        await commandBus.SendAsync(new CreateProductCommand("Laptop", 999.99m, 10));
        await commandBus.SendAsync(new CreateProductCommand("Mouse", 29.99m, 50));
        await commandBus.SendAsync(new CreateProductCommand("Keyboard", 79.99m, 30));
        
        await commandBus.SendAsync(new UpdatePriceCommand(1, 899.99m));
        await commandBus.SendAsync(new UpdateStockCommand(2, -5)); // Sell 5 mice

        // Execute queries (reads)
        Console.WriteLine("--- Querying Products (Queries) ---\n");

        var allProducts = await queryBus.SendAsync<GetAllProductsQuery, IEnumerable<ProductDto>>(new GetAllProductsQuery());
        PrintProducts("All Products", allProducts);

        var product = await queryBus.SendAsync<GetProductByIdQuery, ProductDto?>(new GetProductByIdQuery(1));
        PrintProducts("Product by ID", product != null ? [product] : []);

        var cheapProducts = await queryBus.SendAsync<GetProductsByMaxPriceQuery, IEnumerable<ProductDto>>(new GetProductsByMaxPriceQuery(50));
        PrintProducts($"Products under $50", cheapProducts);

        // Show separation
        Console.WriteLine("\n--- CQRS Benefits ---");
        Console.WriteLine("✓ Commands (writes) can be optimized for consistency");
        Console.WriteLine("✓ Queries (reads) can be optimized for performance");
        Console.WriteLine("✓ Read and write models can scale independently");
        Console.WriteLine("✓ Different data stores can be used for reads/writes");
    }

    static void PrintProducts(string title, IEnumerable<ProductDto> products)
    {
        Console.WriteLine($"{title}:");
        foreach (var p in products)
        {
            Console.WriteLine($"  [{p.Id}] {p.Name} - ${p.Price:F2} (Stock: {p.Stock})");
        }
        Console.WriteLine();
    }
}

// Commands (Write operations)
interface ICommand { }

record CreateProductCommand(string Name, decimal Price, int Stock) : ICommand;
record UpdatePriceCommand(int Id, decimal NewPrice) : ICommand;
record UpdateStockCommand(int Id, int Delta) : ICommand;
record DeleteProductCommand(int Id) : ICommand;

// Queries (Read operations)
interface IQuery<TResult> { }

record GetAllProductsQuery : IQuery<IEnumerable<ProductDto>>;
record GetProductByIdQuery(int Id) : IQuery<ProductDto?>;
record GetProductsByMaxPriceQuery(decimal MaxPrice) : IQuery<IEnumerable<ProductDto>>;

// DTOs for read model
record ProductDto(int Id, string Name, decimal Price, int Stock);

// Write model (domain entity)
class Product
{
    public int Id { get; init; }
    public string Name { get; set; } = "";
    public decimal Price { get; set; }
    public int Stock { get; set; }
}

// Simple in-memory store
class ProductStore
{
    private readonly Dictionary<int, Product> _products = new();
    private int _nextId = 1;

    public Product Create(string name, decimal price, int stock)
    {
        var product = new Product { Id = _nextId++, Name = name, Price = price, Stock = stock };
        _products[product.Id] = product;
        return product;
    }

    public Product? GetById(int id) => _products.GetValueOrDefault(id);
    public IEnumerable<Product> GetAll() => _products.Values;
    public void Delete(int id) => _products.Remove(id);
}

// Command Bus
class CommandBus
{
    private readonly Dictionary<Type, Func<ICommand, Task>> _handlers = new();

    public void Register<T>(Func<T, Task> handler) where T : ICommand
        => _handlers[typeof(T)] = cmd => handler((T)cmd);

    public Task SendAsync(ICommand command)
    {
        if (_handlers.TryGetValue(command.GetType(), out var handler))
        {
            return handler(command);
        }
        throw new InvalidOperationException($"No handler for {command.GetType().Name}");
    }
}

// Query Bus
class QueryBus
{
    private readonly Dictionary<Type, Delegate> _handlers = new();

    public void Register<TQuery, TResult>(Func<TQuery, Task<TResult>> handler) where TQuery : IQuery<TResult>
        => _handlers[typeof(TQuery)] = handler;

    public async Task<TResult> SendAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery<TResult>
    {
        if (_handlers.TryGetValue(typeof(TQuery), out var handlerObj) && handlerObj is Func<TQuery, Task<TResult>> handler)
        {
            return await handler(query);
        }
        throw new InvalidOperationException($"No handler for {typeof(TQuery).Name}");
    }
}

// Command Handlers
static class ProductCommandHandlers
{
    public static void Register(CommandBus bus, ProductStore store)
    {
        bus.Register<CreateProductCommand>(async cmd =>
        {
            store.Create(cmd.Name, cmd.Price, cmd.Stock);
            Console.WriteLine($"  Created product: {cmd.Name}");
            await Task.CompletedTask;
        });

        bus.Register<UpdatePriceCommand>(async cmd =>
        {
            var product = store.GetById(cmd.Id);
            if (product != null)
            {
                product.Price = cmd.NewPrice;
                Console.WriteLine($"  Updated price for product {cmd.Id} to ${cmd.NewPrice:F2}");
            }
            await Task.CompletedTask;
        });

        bus.Register<UpdateStockCommand>(async cmd =>
        {
            var product = store.GetById(cmd.Id);
            if (product != null)
            {
                product.Stock = Math.Max(0, product.Stock + cmd.Delta);
                Console.WriteLine($"  Updated stock for product {cmd.Id} by {cmd.Delta:+#;-#;0}");
            }
            await Task.CompletedTask;
        });
    }
}

// Query Handlers
static class ProductQueryHandlers
{
    public static void Register(QueryBus bus, ProductStore store)
    {
        bus.Register<GetAllProductsQuery, IEnumerable<ProductDto>>(_ =>
        {
            var products = store.GetAll().Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock));
            return Task.FromResult(products);
        });

        bus.Register<GetProductByIdQuery, ProductDto?>(q =>
        {
            var product = store.GetById(q.Id);
            var dto = product != null ? new ProductDto(product.Id, product.Name, product.Price, product.Stock) : null;
            return Task.FromResult(dto);
        });

        bus.Register<GetProductsByMaxPriceQuery, IEnumerable<ProductDto>>(q =>
        {
            var products = store.GetAll()
                .Where(p => p.Price <= q.MaxPrice)
                .Select(p => new ProductDto(p.Id, p.Name, p.Price, p.Stock));
            return Task.FromResult(products);
        });
    }
}
