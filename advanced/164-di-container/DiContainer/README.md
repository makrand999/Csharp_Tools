# Custom DI Container

Dependency Injection container implementation demonstrating DI patterns, service lifetime management, and automatic dependency resolution.

## Usage

```bash
dotnet run --project DiContainer.csproj
```

## Example

```
=== Custom DI Container ===

1. Service Registration:
  Singleton  ILogger                   -> ConsoleLogger
  Singleton  IEmailService             -> EmailService
  Scoped     IUserRepository           -> UserRepository
  Transient  INotificationService      -> NotificationService
  Singleton  IDatabaseConnection       -> DatabaseConnection

2. Singleton Resolution:
Logger 1 instance: 45032156
Logger 2 instance: 45032156
Same instance? True

3. Scoped Resolution (Same Scope):
Repo 1 instance: 58234891
Repo 2 instance: 58234891
Same instance in scope? True

4. Scoped Resolution (Different Scopes):
Scope 1 Repo: 32145678
Scope 2 Repo: 87654321
Different instances? True

5. Transient Resolution:
Notification 1 instance: 12345678
Notification 2 instance: 23456789
Different instances? True

6. Constructor Injection:
  [LOG] UserService initialized
  [LOG] Creating user: john@example.com
  [LOG] User saved: john@example.com
  [LOG] Sending welcome email to: john@example.com
  [LOG] Sending email to john@example.com: Welcome!

7. Complex Dependency Chain:
  [LOG] OrderProcessor initialized
  [LOG] Processing order #1 for $99.99
  [LOG] New customer, creating account: customer@example.com
  [LOG] User saved: customer@example.com
  [LOG] Sending email to customer@example.com: Order Confirmation
  [LOG] Order #1 processed successfully

8. Factory Registration:
Environment: Production, Version: v1.0.0

9. Dispose Tracking:
  [LOG] DatabaseConnection opened
Database connection opened: True
Scope disposed - scoped services cleaned up
```

## Concepts Demonstrated

- Service registration (AddSingleton, AddScoped, AddTransient)
- Service resolution with dependency injection
- Constructor injection with automatic dependency resolution
- Service lifetime management (Singleton, Scoped, Transient)
- Scope creation and disposal
- Factory-based registration
- Circular dependency detection
- IDisposable tracking and cleanup
- Reflection for constructor resolution
- Thread-safe service registration (ConcurrentDictionary)
- Custom IServiceProvider implementation
