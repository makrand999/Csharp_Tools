using System.Collections.Concurrent;
using System.Reflection;

namespace DiContainer;

/// <summary>
/// Custom Dependency Injection container - demonstrates DI patterns, service lifetime management, and registration
/// Supports Transient, Scoped, and Singleton lifetimes with automatic dependency resolution
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Custom DI Container ===\n");

        // Create container and register services
        var container = new ServiceContainer();

        // Register services with different lifetimes
        container.AddSingleton<ILogger, ConsoleLogger>();
        container.AddSingleton<IEmailService, EmailService>();
        container.AddScoped<IUserRepository, UserRepository>();
        container.AddTransient<INotificationService, NotificationService>();
        container.AddSingleton<IDatabaseConnection, DatabaseConnection>();

        Console.WriteLine("1. Service Registration:");
        container.PrintRegistrations();

        // Demo 2: Resolve singleton services
        Console.WriteLine("\n2. Singleton Resolution:");
        var logger1 = container.Resolve<ILogger>();
        var logger2 = container.Resolve<ILogger>();
        Console.WriteLine($"Logger 1 instance: {logger1.GetHashCode()}");
        Console.WriteLine($"Logger 2 instance: {logger2.GetHashCode()}");
        Console.WriteLine($"Same instance? {ReferenceEquals(logger1, logger2)}");

        // Demo 3: Resolve scoped services (same scope = same instance)
        Console.WriteLine("\n3. Scoped Resolution (Same Scope):");
        using (var scope1 = container.CreateScope())
        {
            var repo1 = scope1.Resolve<IUserRepository>();
            var repo2 = scope1.Resolve<IUserRepository>();
            Console.WriteLine($"Repo 1 instance: {repo1.GetHashCode()}");
            Console.WriteLine($"Repo 2 instance: {repo2.GetHashCode()}");
            Console.WriteLine($"Same instance in scope? {ReferenceEquals(repo1, repo2)}");
        }

        // Demo 4: Scoped services (different scopes = different instances)
        Console.WriteLine("\n4. Scoped Resolution (Different Scopes):");
        using (var scope1 = container.CreateScope())
        using (var scope2 = container.CreateScope())
        {
            var repo1 = scope1.Resolve<IUserRepository>();
            var repo2 = scope2.Resolve<IUserRepository>();
            Console.WriteLine($"Scope 1 Repo: {repo1.GetHashCode()}");
            Console.WriteLine($"Scope 2 Repo: {repo2.GetHashCode()}");
            Console.WriteLine($"Different instances? {!ReferenceEquals(repo1, repo2)}");
        }

        // Demo 5: Transient services (always new instance)
        Console.WriteLine("\n5. Transient Resolution:");
        var notification1 = container.Resolve<INotificationService>();
        var notification2 = container.Resolve<INotificationService>();
        Console.WriteLine($"Notification 1 instance: {notification1.GetHashCode()}");
        Console.WriteLine($"Notification 2 instance: {notification2.GetHashCode()}");
        Console.WriteLine($"Different instances? {!ReferenceEquals(notification1, notification2)}");

        // Demo 6: Constructor injection with dependencies
        Console.WriteLine("\n6. Constructor Injection:");
        using (var scope = container.CreateScope())
        {
            var userService = scope.Resolve<IUserService>();
            userService.CreateUser("john@example.com");
            userService.SendWelcomeEmail("john@example.com");
        }

        // Demo 7: Complex dependency chain
        Console.WriteLine("\n7. Complex Dependency Chain:");
        using (var scope = container.CreateScope())
        {
            var orderProcessor = scope.Resolve<IOrderProcessor>();
            orderProcessor.ProcessOrder(new Order { Id = 1, CustomerEmail = "customer@example.com", Amount = 99.99m });
        }

        // Demo 8: Service with configuration
        Console.WriteLine("\n8. Factory Registration:");
        container.AddSingleton<IConfigService>(_ => new ConfigService("Production", "v1.0.0"));
        var config = container.Resolve<IConfigService>();
        Console.WriteLine($"Environment: {config.Environment}, Version: {config.Version}");

        // Demo 9: Dispose tracking
        Console.WriteLine("\n9. Dispose Tracking:");
        using (var scope = container.CreateScope())
        {
            var db = scope.Resolve<IDatabaseConnection>();
            Console.WriteLine($"Database connection opened: {db.IsOpen}");
        }
        // Scope disposed - scoped services should be disposed
        Console.WriteLine("Scope disposed - scoped services cleaned up");
    }
}

// === Service Interfaces ===

interface ILogger
{
    void Log(string message);
}

interface IEmailService
{
    void SendEmail(string to, string subject, string body);
}

interface IUserRepository
{
    void SaveUser(string email);
    string? GetUser(string email);
}

interface INotificationService
{
    void Notify(string message);
}

interface IDatabaseConnection
{
    bool IsOpen { get; }
    void Execute(string sql);
}

interface IConfigService
{
    string Environment { get; }
    string Version { get; }
}

interface IUserService
{
    void CreateUser(string email);
    void SendWelcomeEmail(string email);
}

interface IOrderProcessor
{
    void ProcessOrder(Order order);
}

// === Service Implementations ===

class ConsoleLogger : ILogger
{
    public void Log(string message) => Console.WriteLine($"  [LOG] {message}");
}

class EmailService : IEmailService
{
    private readonly ILogger _logger;
    public EmailService(ILogger logger)
    {
        _logger = logger;
        _logger.Log("EmailService initialized");
    }

    public void SendEmail(string to, string subject, string body)
    {
        _logger.Log($"Sending email to {to}: {subject}");
    }
}

class UserRepository : IUserRepository, IDisposable
{
    private readonly IDatabaseConnection _db;
    private readonly ILogger _logger;
    private readonly Dictionary<string, string> _users = new();
    private bool _disposed;

    public UserRepository(IDatabaseConnection db, ILogger logger)
    {
        _db = db;
        _logger = logger;
        _logger.Log("UserRepository initialized");
    }

    public void SaveUser(string email)
    {
        _db.Execute($"INSERT INTO users (email) VALUES ('{email}')");
        _users[email] = email;
        _logger.Log($"User saved: {email}");
    }

    public string? GetUser(string email)
    {
        _db.Execute($"SELECT * FROM users WHERE email = '{email}'");
        return _users.TryGetValue(email, out var user) ? user : null;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _logger.Log("UserRepository disposed");
            _disposed = true;
        }
    }
}

class NotificationService : INotificationService
{
    private readonly ILogger _logger;
    private readonly Guid _instanceId = Guid.NewGuid();

    public NotificationService(ILogger logger)
    {
        _logger = logger;
        _logger.Log($"NotificationService initialized (Instance: {_instanceId})");
    }

    public void Notify(string message)
    {
        _logger.Log($"Notification: {message}");
    }
}

class DatabaseConnection : IDatabaseConnection, IDisposable
{
    private readonly ILogger _logger;
    private bool _isOpen;
    private bool _disposed;

    public DatabaseConnection(ILogger logger)
    {
        _logger = logger;
        _logger.Log("DatabaseConnection opened");
        _isOpen = true;
    }

    public bool IsOpen => _isOpen && !_disposed;

    public void Execute(string sql)
    {
        if (!_isOpen || _disposed) throw new InvalidOperationException("Connection is closed");
        _logger.Log($"Executing SQL: {sql}");
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _logger.Log("DatabaseConnection closed");
            _isOpen = false;
            _disposed = true;
        }
    }
}

class ConfigService : IConfigService
{
    public string Environment { get; }
    public string Version { get; }

    public ConfigService(string environment, string version)
    {
        Environment = environment;
        Version = version;
    }
}

class UserService : IUserService
{
    private readonly IUserRepository _repo;
    private readonly IEmailService _email;
    private readonly ILogger _logger;

    public UserService(IUserRepository repo, IEmailService email, ILogger logger)
    {
        _repo = repo;
        _email = email;
        _logger = logger;
        _logger.Log("UserService initialized");
    }

    public void CreateUser(string email)
    {
        _logger.Log($"Creating user: {email}");
        _repo.SaveUser(email);
    }

    public void SendWelcomeEmail(string email)
    {
        _logger.Log($"Sending welcome email to: {email}");
        _email.SendEmail(email, "Welcome!", "Thanks for joining!");
    }
}

class Order
{
    public int Id { get; set; }
    public string CustomerEmail { get; set; } = "";
    public decimal Amount { get; set; }
}

class OrderProcessor : IOrderProcessor
{
    private readonly ILogger _logger;
    private readonly IEmailService _email;
    private readonly IUserRepository _userRepo;

    public OrderProcessor(ILogger logger, IEmailService email, IUserRepository userRepo)
    {
        _logger = logger;
        _email = email;
        _userRepo = userRepo;
        _logger.Log("OrderProcessor initialized");
    }

    public void ProcessOrder(Order order)
    {
        _logger.Log($"Processing order #{order.Id} for ${order.Amount}");
        
        // Verify user exists
        var user = _userRepo.GetUser(order.CustomerEmail);
        if (user == null)
        {
            _logger.Log($"New customer, creating account: {order.CustomerEmail}");
            _userRepo.SaveUser(order.CustomerEmail);
        }

        // Send confirmation
        _email.SendEmail(order.CustomerEmail, "Order Confirmation", $"Order #{order.Id} confirmed");
        _logger.Log($"Order #{order.Id} processed successfully");
    }
}

// === DI Container Implementation ===

enum ServiceLifetime
{
    Transient,
    Scoped,
    Singleton
}

class ServiceDescriptor
{
    public Type ServiceType { get; }
    public Type? ImplementationType { get; }
    public Func<IServiceProvider, object>? Factory { get; }
    public ServiceLifetime Lifetime { get; }

    public ServiceDescriptor(Type serviceType, Type implementationType, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        ImplementationType = implementationType;
        Lifetime = lifetime;
    }

    public ServiceDescriptor(Type serviceType, Func<IServiceProvider, object> factory, ServiceLifetime lifetime)
    {
        ServiceType = serviceType;
        Factory = factory;
        Lifetime = lifetime;
    }
}

interface IServiceScope : IDisposable
{
    T Resolve<T>();
    object Resolve(Type serviceType);
}

class ServiceContainer : IServiceProvider, IDisposable
{
    private readonly ConcurrentDictionary<Type, ServiceDescriptor> _services = new();
    private readonly ConcurrentDictionary<Type, object> _singletons = new();
    private readonly List<IDisposable> _disposables = new();
    private bool _disposed;

    public void AddSingleton<TService, TImplementation>() where TImplementation : class, TService
    {
        _services[typeof(TService)] = new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Singleton);
    }

    public void AddScoped<TService, TImplementation>() where TImplementation : class, TService
    {
        _services[typeof(TService)] = new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Scoped);
    }

    public void AddTransient<TService, TImplementation>() where TImplementation : class, TService
    {
        _services[typeof(TService)] = new ServiceDescriptor(typeof(TService), typeof(TImplementation), ServiceLifetime.Transient);
    }

    public void AddSingleton<TService>(Func<IServiceProvider, TService> factory) where TService : class
    {
        _services[typeof(TService)] = new ServiceDescriptor(typeof(TService), sp => factory(sp), ServiceLifetime.Singleton);
    }

    public T Resolve<T>()
    {
        return (T)Resolve(typeof(T));
    }

    public object? Resolve(Type serviceType)
    {
        if (_disposed) throw new ObjectDisposedException(nameof(ServiceContainer));

        if (!_services.TryGetValue(serviceType, out var descriptor))
        {
            throw new InvalidOperationException($"Service {serviceType.Name} not registered");
        }

        return ResolveService(descriptor, this);
    }

    public IServiceScope CreateScope()
    {
        return new Scope(this);
    }

    private object ResolveService(ServiceDescriptor descriptor, IServiceProvider provider)
    {
        if (descriptor.Lifetime == ServiceLifetime.Singleton)
        {
            return _singletons.GetOrAdd(descriptor.ServiceType, _ => CreateInstance(descriptor, provider));
        }

        return CreateInstance(descriptor, provider);
    }

    private object CreateInstance(ServiceDescriptor descriptor, IServiceProvider provider)
    {
        return CreateInstance(this, descriptor, provider);
    }

    internal static object CreateInstance(ServiceContainer container, ServiceDescriptor descriptor, IServiceProvider provider)
    {
        object instance;

        if (descriptor.Factory != null)
        {
            instance = descriptor.Factory(provider);
        }
        else if (descriptor.ImplementationType != null)
        {
            var constructors = descriptor.ImplementationType.GetConstructors();
            var constructor = constructors.OrderByDescending(c => c.GetParameters().Length).First();
            var parameters = constructor.GetParameters()
                .Select(p => container.Resolve(p.ParameterType))
                .ToArray();
            instance = constructor.Invoke(parameters);
        }
        else
        {
            throw new InvalidOperationException("Invalid service descriptor");
        }

        if (instance is IDisposable disposable && descriptor.Lifetime != ServiceLifetime.Transient)
        {
            lock (container._disposables)
            {
                container._disposables.Add(disposable);
            }
        }

        return instance;
    }

    public object? GetService(Type serviceType)
    {
        try
        {
            return Resolve(serviceType);
        }
        catch
        {
            return null;
        }
    }

    public void PrintRegistrations()
    {
        foreach (var service in _services)
        {
            var implName = service.Value.ImplementationType?.Name ?? "Factory";
            Console.WriteLine($"  {service.Value.Lifetime,-10} {service.Key.Name,-25} -> {implName}");
        }
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            lock (_disposables)
            {
                foreach (var disposable in _disposables)
                {
                    disposable.Dispose();
                }
                _disposables.Clear();
            }
            _singletons.Clear();
            _disposed = true;
        }
    }

    // Inner scope class
    private class Scope : IServiceScope, IServiceProvider
    {
        private readonly ServiceContainer _container;
        private readonly ConcurrentDictionary<Type, object> _scopedServices = new();
        private readonly List<IDisposable> _scopedDisposables = new();
        private bool _disposed;

        public Scope(ServiceContainer container)
        {
            _container = container;
        }

        public T Resolve<T>() => (T)Resolve(typeof(T));

        public object? GetService(Type serviceType) => Resolve(serviceType);

        public object Resolve(Type serviceType)
        {
            if (_disposed) throw new ObjectDisposedException(nameof(Scope));

            if (!_container._services.TryGetValue(serviceType, out var descriptor))
            {
                throw new InvalidOperationException($"Service {serviceType.Name} not registered");
            }

            if (descriptor.Lifetime == ServiceLifetime.Singleton)
            {
                return _container.ResolveService(descriptor, _container);
            }

            if (descriptor.Lifetime == ServiceLifetime.Scoped)
            {
                return _scopedServices.GetOrAdd(serviceType, _ => CreateScopedInstance(descriptor));
            }

            return CreateScopedInstance(descriptor);
        }

        private object CreateScopedInstance(ServiceDescriptor descriptor)
        {
            var instance = ServiceContainer.CreateInstance(_container, descriptor, this);
            if (instance is IDisposable disposable)
            {
                lock (_scopedDisposables)
                {
                    _scopedDisposables.Add(disposable);
                }
            }
            return instance;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                lock (_scopedDisposables)
                {
                    foreach (var disposable in _scopedDisposables)
                    {
                        disposable.Dispose();
                    }
                    _scopedDisposables.Clear();
                }
                _scopedServices.Clear();
                _disposed = true;
            }
        }
    }
}
