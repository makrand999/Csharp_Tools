# Dynamic Proxy

Demonstrates runtime proxy creation using `DispatchProxy` for AOP-style method interception. Useful for implementing cross-cutting concerns like logging, timing, caching, and validation without modifying the original code.

## Usage

```bash
dotnet run --project DynamicProxy.csproj
```

## Example

```
=== Dynamic Proxy Demo ===

--- Testing UserService with Logging Proxy ---

  [LOG] >> Calling: GetUser
  [LOG]    Args: [1]
  [UserService] Getting user with ID 1
  [LOG] << Returned: User_1
  [LOG] >> Calling: CreateUser
  [LOG]    Args: [Alice, alice@example.com]
  [UserService] Creating user Alice (alice@example.com)
  [LOG] << Returned: Created: Alice
```

## Concepts Demonstrated

- DispatchProxy for runtime proxy creation
- Method interception using reflection
- AOP (Aspect-Oriented Programming) patterns
- Cross-cutting concerns (logging, timing)
- Generic type constraints
- Exception handling in proxies
