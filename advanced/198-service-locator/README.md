# Service Locator Pattern

Centralized service registry and resolution with support for singleton, transient, and factory-based service lifetimes, automatic dependency injection, and service statistics tracking.

## Usage

```bash
# Run demo
dotnet run --project ServiceLocator.csproj -- demo

# Run interactive mode
dotnet run --project ServiceLocator.csproj -- interactive
```

## Example

```
=== Service Locator Pattern Demo ===

1. Service Registration
-----------------------
Registered services:
  ILogger -> ConsoleLogger (Singleton)
  IEmailService -> SmtpEmailService (Singleton)
  ICacheService -> MemoryCacheService (Singleton)
  IUserService -> UserService (Transient)
  IDatabaseService -> DatabaseService (Factory)

2. Service Resolution
---------------------
[LOG] Application started
[EMAIL] To: user@example.com, Subject: Welcome!
Cache contains 2 items
key1 = value1

3. Service Lifetime Demonstration
----------------------------------
ILogger is singleton: True (should be True)
IUserService is transient: True (should be True)

4. Service with Dependencies
----------------------------
[LOG] Creating user: john.doe
[LOG] Creating user: jane.smith
Created 2 users:
  - john.doe (john@example.com)
  - jane.smith (jane@example.com)
```

## Concepts Demonstrated

- Service Locator pattern implementation
- Service lifetime management (Singleton, Transient, Factory)
- Automatic dependency injection via reflection
- Constructor injection with service resolution
- Service registration and resolution APIs
- TryResolve and fallback patterns
- Service statistics and monitoring
