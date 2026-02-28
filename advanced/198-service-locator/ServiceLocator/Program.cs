namespace ServiceLocator;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Service Locator Pattern - Centralized service registry and resolution");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project ServiceLocator.csproj -- demo");
            Console.WriteLine("  dotnet run --project ServiceLocator.csproj -- interactive");
            return 0;
        }

        if (args[0].Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            RunDemo();
            return 0;
        }

        if (args[0].Equals("interactive", StringComparison.OrdinalIgnoreCase))
        {
            RunInteractiveMode();
            return 0;
        }

        Console.WriteLine($"Unknown command: {args[0]}");
        Console.WriteLine("Use 'demo' or 'interactive'");
        return 1;
    }

    private static void RunDemo()
    {
        Console.WriteLine("=== Service Locator Pattern Demo ===\n");

        // Setup: Register services
        Console.WriteLine("1. Service Registration");
        Console.WriteLine("-----------------------");
        
        var locator = new ServiceLocator();
        
        // Register singleton services
        locator.RegisterSingleton<ILogger, ConsoleLogger>();
        locator.RegisterSingleton<IEmailService, SmtpEmailService>();
        locator.RegisterSingleton<ICacheService, MemoryCacheService>();
        
        // Register transient services
        locator.RegisterTransient<IUserService, UserService>();
        locator.RegisterTransient<IReportService, ReportService>();
        
        // Register factory-based service
        locator.RegisterFactory<IDatabaseService>(_ => 
            new DatabaseService("Server=localhost;Database=Demo;"));
        
        Console.WriteLine("Registered services:");
        foreach (var service in locator.GetRegisteredServices())
        {
            Console.WriteLine($"  {service.ServiceType.Name} -> {service.ImplementationType.Name} ({service.Lifetime})");
        }
        Console.WriteLine();

        // Demo 2: Resolve services
        Console.WriteLine("2. Service Resolution");
        Console.WriteLine("---------------------");
        
        var logger = locator.Resolve<ILogger>();
        logger.Log("Application started");
        
        var emailService = locator.Resolve<IEmailService>();
        emailService.Send("user@example.com", "Welcome!", "Hello from Service Locator demo");
        
        var cacheService = locator.Resolve<ICacheService>();
        cacheService.Set("key1", "value1", TimeSpan.FromMinutes(5));
        cacheService.Set("key2", "value2", TimeSpan.FromMinutes(10));
        Console.WriteLine($"Cache contains {cacheService.Count} items");
        Console.WriteLine($"key1 = {cacheService.Get<string>("key1")}");
        Console.WriteLine();

        // Demo 3: Transient vs Singleton behavior
        Console.WriteLine("3. Service Lifetime Demonstration");
        Console.WriteLine("----------------------------------");
        
        // Singleton: Same instance
        var logger1 = locator.Resolve<ILogger>();
        var logger2 = locator.Resolve<ILogger>();
        Console.WriteLine($"ILogger is singleton: {ReferenceEquals(logger1, logger2)} (should be True)");
        
        // Transient: Different instances
        var userService1 = locator.Resolve<IUserService>();
        var userService2 = locator.Resolve<IUserService>();
        Console.WriteLine($"IUserService is transient: {!ReferenceEquals(userService1, userService2)} (should be True)");
        Console.WriteLine();

        // Demo 4: Service with dependencies
        Console.WriteLine("4. Service with Dependencies");
        Console.WriteLine("----------------------------");
        
        // UserService depends on ILogger and ICacheService
        var userService = locator.Resolve<IUserService>();
        userService.CreateUser("john.doe", "john@example.com");
        userService.CreateUser("jane.smith", "jane@example.com");
        
        var users = userService.GetAllUsers();
        Console.WriteLine($"Created {users.Count} users:");
        foreach (var user in users)
        {
            Console.WriteLine($"  - {user.Username} ({user.Email})");
        }
        Console.WriteLine();

        // Demo 5: Report generation using multiple services
        Console.WriteLine("5. Complex Service Orchestration");
        Console.WriteLine("--------------------------------");
        
        var reportService = locator.Resolve<IReportService>();
        var report = reportService.GenerateReport("Monthly Summary");
        
        Console.WriteLine($"Report: {report.Title}");
        Console.WriteLine($"Generated at: {report.GeneratedAt}");
        Console.WriteLine($"Lines: {report.Content.Length}");
        Console.WriteLine();

        // Demo 6: TryResolve and optional services
        Console.WriteLine("6. Optional Service Resolution");
        Console.WriteLine("------------------------------");
        
        if (locator.TryResolve<INotificationService>(out var notificationService))
        {
            notificationService.Send("Test notification");
        }
        else
        {
            Console.WriteLine("INotificationService not registered (expected)");
        }
        
        // Resolve with fallback
        var notification = locator.ResolveOrDefault<INotificationService>(
            new NullNotificationService()
        );
        notification.Send("Fallback notification");
        Console.WriteLine();

        // Demo 7: Service statistics
        Console.WriteLine("7. Service Locator Statistics");
        Console.WriteLine("-----------------------------");
        var stats = locator.GetStatistics();
        Console.WriteLine($"Total services registered: {stats.TotalRegistered}");
        Console.WriteLine($"Total resolutions: {stats.TotalResolutions}");
        Console.WriteLine($"Singleton instances: {stats.SingletonCount}");
        Console.WriteLine($"Transient instances: {stats.TransientCount}");
        Console.WriteLine();

        Console.WriteLine("Demo completed!");
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Service Locator (Interactive Mode)");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine();

        var locator = new ServiceLocator();
        var setupDone = false;

        while (true)
        {
            Console.Write("sl> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var cmd = parts[0].ToLowerInvariant();

            try
            {
                switch (cmd)
                {
                    case "quit":
                    case "exit":
                        return;

                    case "help":
                        ShowHelp();
                        break;

                    case "setup":
                        // Register default services
                        locator.RegisterSingleton<ILogger, ConsoleLogger>();
                        locator.RegisterSingleton<IEmailService, SmtpEmailService>();
                        locator.RegisterSingleton<ICacheService, MemoryCacheService>();
                        locator.RegisterTransient<IUserService, UserService>();
                        locator.RegisterFactory<IDatabaseService>(_ => 
                            new DatabaseService("Server=localhost;Database=Demo;"));
                        setupDone = true;
                        Console.WriteLine("Services registered successfully");
                        break;

                    case "register":
                        if (parts.Length < 4)
                        {
                            Console.WriteLine("Usage: register <lifetime> <service> <implementation>");
                            Console.WriteLine("  lifetime: singleton, transient, factory");
                            break;
                        }
                        var lifetime = parts[1].ToLowerInvariant();
                        var serviceType = parts[2];
                        var implType = parts[3];
                        Console.WriteLine($"Registration simulated: {serviceType} -> {implType} ({lifetime})");
                        break;

                    case "resolve":
                        if (!setupDone)
                        {
                            Console.WriteLine("Run 'setup' first to register services");
                            break;
                        }
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: resolve <service-type>");
                            Console.WriteLine("  Types: ILogger, IEmailService, ICacheService, IUserService, IDatabaseService");
                            break;
                        }
                        var typeToResolve = parts[1];
                        var resolved = ResolveInteractive(locator, typeToResolve);
                        Console.WriteLine(resolved ?? $"Unknown service type: {typeToResolve}");
                        break;

                    case "list":
                        var services = locator.GetRegisteredServices();
                        if (services.Count == 0)
                        {
                            Console.WriteLine("No services registered. Run 'setup' first.");
                        }
                        else
                        {
                            Console.WriteLine("Registered services:");
                            foreach (var svc in services)
                            {
                                Console.WriteLine($"  {svc.ServiceType.Name} -> {svc.ImplementationType.Name} ({svc.Lifetime})");
                            }
                        }
                        break;

                    case "stats":
                        var stats = locator.GetStatistics();
                        Console.WriteLine($"Registered: {stats.TotalRegistered}");
                        Console.WriteLine($"Resolutions: {stats.TotalResolutions}");
                        Console.WriteLine($"Singletons: {stats.SingletonCount}");
                        Console.WriteLine($"Transients: {stats.TransientCount}");
                        break;

                    case "clear":
                        locator = new ServiceLocator();
                        setupDone = false;
                        Console.WriteLine("Service locator cleared");
                        break;

                    default:
                        Console.WriteLine($"Unknown command: {cmd}. Type 'help' for commands.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static string? ResolveInteractive(ServiceLocator locator, string serviceType)
    {
        return serviceType switch
        {
            "ILogger" => $"Resolved: {locator.Resolve<ILogger>().GetType().Name}",
            "IEmailService" => $"Resolved: {locator.Resolve<IEmailService>().GetType().Name}",
            "ICacheService" => $"Resolved: {locator.Resolve<ICacheService>().GetType().Name}",
            "IUserService" => $"Resolved: {locator.Resolve<IUserService>().GetType().Name}",
            "IDatabaseService" => $"Resolved: {locator.Resolve<IDatabaseService>().GetType().Name}",
            _ => null
        };
    }

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  setup                  - Register default services");
        Console.WriteLine("  register <lt> <s> <i>  - Register service (lifetime, service, implementation)");
        Console.WriteLine("  resolve <type>         - Resolve a service");
        Console.WriteLine("  list                   - List registered services");
        Console.WriteLine("  stats                  - Show statistics");
        Console.WriteLine("  clear                  - Clear all registrations");
        Console.WriteLine("  quit                   - Exit");
        Console.WriteLine();
    }
}

// Service lifetime enum
public enum ServiceLifetime { Singleton, Transient, Factory }

// Service registration info
public record ServiceRegistration(
    Type ServiceType,
    Type ImplementationType,
    ServiceLifetime Lifetime,
    Func<ServiceLocator, object>? Factory = null
);

// Statistics record
public record LocatorStatistics(
    int TotalRegistered,
    int TotalResolutions,
    int SingletonCount,
    int TransientCount
);

// Service Locator implementation
public class ServiceLocator
{
    private readonly Dictionary<Type, ServiceRegistration> _registrations = new();
    private readonly Dictionary<Type, object> _singletonInstances = new();
    private int _totalResolutions;

    public void RegisterSingleton<TService, TImplementation>() where TImplementation : class, TService
    {
        _registrations[typeof(TService)] = new ServiceRegistration(
            typeof(TService),
            typeof(TImplementation),
            ServiceLifetime.Singleton
        );
    }

    public void RegisterTransient<TService, TImplementation>() where TImplementation : class, TService
    {
        _registrations[typeof(TService)] = new ServiceRegistration(
            typeof(TService),
            typeof(TImplementation),
            ServiceLifetime.Transient
        );
    }

    public void RegisterFactory<TService>(Func<ServiceLocator, TService> factory) where TService : class
    {
        _registrations[typeof(TService)] = new ServiceRegistration(
            typeof(TService),
            typeof(TService),
            ServiceLifetime.Factory,
            sp => factory(sp)
        );
    }

    public TService Resolve<TService>() where TService : class
    {
        if (!_registrations.TryGetValue(typeof(TService), out var registration))
        {
            throw new InvalidOperationException($"Service {typeof(TService).Name} not registered");
        }

        Interlocked.Increment(ref _totalResolutions);

        return registration.Lifetime switch
        {
            ServiceLifetime.Singleton => (TService)GetOrCreateSingleton(registration),
            ServiceLifetime.Transient => (TService)CreateInstance(registration),
            ServiceLifetime.Factory => (TService)CreateFromFactory(registration),
            _ => throw new InvalidOperationException($"Unknown lifetime: {registration.Lifetime}")
        };
    }

    public bool TryResolve<TService>(out TService service) where TService : class
    {
        try
        {
            service = Resolve<TService>();
            return true;
        }
        catch
        {
            service = null!;
            return false;
        }
    }

    public TService ResolveOrDefault<TService>(TService defaultValue) where TService : class
    {
        return TryResolve<TService>(out var service) ? service : defaultValue;
    }

    public List<ServiceRegistration> GetRegisteredServices() => _registrations.Values.ToList();

    public LocatorStatistics GetStatistics()
    {
        return new LocatorStatistics(
            _registrations.Count,
            Volatile.Read(ref _totalResolutions),
            _singletonInstances.Count,
            Volatile.Read(ref _totalResolutions) - _singletonInstances.Count
        );
    }

    private object GetOrCreateSingleton(ServiceRegistration registration)
    {
        lock (_singletonInstances)
        {
            if (!_singletonInstances.TryGetValue(registration.ServiceType, out var instance))
            {
                instance = CreateInstance(registration);
                _singletonInstances[registration.ServiceType] = instance;
            }
            return instance;
        }
    }

    private object CreateInstance(ServiceRegistration registration)
    {
        var implType = registration.ImplementationType;

        // Find constructor that takes ServiceLocator or default
        var constructors = implType.GetConstructors();
        var ctor = constructors.FirstOrDefault(c =>
            c.GetParameters().Any(p => p.ParameterType == typeof(ServiceLocator)))
            ?? constructors.First();

        var parameters = ctor.GetParameters().Select(p =>
        {
            if (p.ParameterType == typeof(ServiceLocator))
                return (object)this;
            if (_registrations.TryGetValue(p.ParameterType, out var depReg))
                return (object)GetOrCreateSingleton(depReg);
            throw new InvalidOperationException($"Dependency {p.ParameterType.Name} not registered");
        }).ToArray();

        return Activator.CreateInstance(implType, parameters)!;
    }

    private object CreateFromFactory(ServiceRegistration registration)
    {
        if (registration.Factory == null)
            throw new InvalidOperationException("Factory not provided");

        return registration.Factory(this);
    }
}

// Service interfaces and implementations
public interface ILogger
{
    void Log(string message);
}

public class ConsoleLogger : ILogger
{
    public void Log(string message)
    {
        Console.WriteLine($"[LOG] {message}");
    }
}

public interface IEmailService
{
    void Send(string to, string subject, string body);
}

public class SmtpEmailService : IEmailService
{
    public void Send(string to, string subject, string body)
    {
        Console.WriteLine($"[EMAIL] To: {to}, Subject: {subject}");
    }
}

public interface ICacheService
{
    void Set<T>(string key, T value, TimeSpan expiration);
    T? Get<T>(string key);
    int Count { get; }
}

public class MemoryCacheService : ICacheService
{
    private readonly Dictionary<string, (object Value, DateTime Expires)> _cache = new();
    
    public void Set<T>(string key, T value, TimeSpan expiration)
    {
        _cache[key] = (value!, DateTime.UtcNow + expiration);
    }
    
    public T? Get<T>(string key)
    {
        if (_cache.TryGetValue(key, out var entry) && entry.Expires > DateTime.UtcNow)
        {
            return (T)entry.Value;
        }
        return default;
    }
    
    public int Count => _cache.Count;
}

public interface IUserService
{
    void CreateUser(string username, string email);
    List<User> GetAllUsers();
}

public class User
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}

public class UserService : IUserService
{
    private readonly ILogger _logger;
    private readonly ICacheService _cache;
    private readonly List<User> _users = new();

    public UserService(ILogger logger, ICacheService cache, ServiceLocator locator)
    {
        _logger = logger;
        _cache = cache;
    }

    public void CreateUser(string username, string email)
    {
        _logger.Log($"Creating user: {username}");
        var user = new User { Username = username, Email = email };
        _users.Add(user);
        _cache.Set($"user:{username}", user, TimeSpan.FromMinutes(30));
    }

    public List<User> GetAllUsers() => _users.ToList();
}

public interface IDatabaseService
{
    string ConnectionString { get; }
    void Connect();
}

public class DatabaseService : IDatabaseService
{
    public string ConnectionString { get; }
    
    public DatabaseService(string connectionString)
    {
        ConnectionString = connectionString;
    }
    
    public void Connect()
    {
        Console.WriteLine($"[DB] Connected to {ConnectionString}");
    }
}

public interface IReportService
{
    Report GenerateReport(string title);
}

public class Report
{
    public string Title { get; set; } = string.Empty;
    public DateTime GeneratedAt { get; set; }
    public string Content { get; set; } = string.Empty;
}

public class ReportService : IReportService
{
    private readonly ILogger _logger;
    private readonly IDatabaseService _db;

    public ReportService(ILogger logger, IDatabaseService db)
    {
        _logger = logger;
        _db = db;
    }

    public Report GenerateReport(string title)
    {
        _logger.Log($"Generating report: {title}");
        _db.Connect();
        
        return new Report
        {
            Title = title,
            GeneratedAt = DateTime.Now,
            Content = $"Report content for {title}\nGenerated at {DateTime.Now:yyyy-MM-dd HH:mm:ss}"
        };
    }
}

public interface INotificationService
{
    void Send(string message);
}

public class NullNotificationService : INotificationService
{
    public void Send(string message)
    {
        // No-op implementation
    }
}
