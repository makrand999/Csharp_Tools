using System;
using System.Reflection;

namespace DynamicProxy;

/// <summary>
/// A dynamic proxy implementation using DispatchProxy for AOP-style interception.
/// Useful for logging, caching, validation, and cross-cutting concerns.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Dynamic Proxy Demo ===\n");
        Console.WriteLine("Demonstrates runtime proxy creation for method interception.\n");

        // Create a proxied service
        var userService = ProxyFactory.Create<UserService, LoggingProxy<UserService>>();
        var mathService = ProxyFactory.Create<MathService, TimingProxy<MathService>>();

        Console.WriteLine("--- Testing UserService with Logging Proxy ---\n");
        userService.GetUser(1);
        userService.CreateUser("Alice", "alice@example.com");
        userService.DeleteUser(3);

        Console.WriteLine("\n--- Testing MathService with Timing Proxy ---\n");
        Console.WriteLine($"Add: {mathService.Add(5, 3)}");
        Console.WriteLine($"Multiply: {mathService.Multiply(4, 7)}");
        mathService.SlowOperation();
    }
}

// Sample services to proxy
public class UserService
{
    public virtual string GetUser(int id)
    {
        Console.WriteLine($"  [UserService] Getting user with ID {id}");
        return $"User_{id}";
    }

    public virtual string CreateUser(string name, string email)
    {
        Console.WriteLine($"  [UserService] Creating user {name} ({email})");
        return $"Created: {name}";
    }

    public virtual void DeleteUser(int id)
    {
        Console.WriteLine($"  [UserService] Deleting user with ID {id}");
    }
}

public class MathService
{
    public virtual int Add(int a, int b)
    {
        return a + b;
    }

    public virtual int Multiply(int a, int b)
    {
        return a * b;
    }

    public virtual void SlowOperation()
    {
        Console.WriteLine("  [MathService] Performing slow operation...");
        Thread.Sleep(500);
    }
}

// Base proxy using DispatchProxy
public abstract class BaseProxy<T> : DispatchProxy
{
    protected T? Target { get; private set; }
    protected MethodInfo? TargetMethod { get; private set; }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        TargetMethod = targetMethod;
        
        // Before invocation
        OnBefore(targetMethod, args);

        try
        {
            // Invoke the actual method
            var result = targetMethod?.Invoke(this, args);
            
            // After successful invocation
            OnAfter(targetMethod, result);
            
            return result;
        }
        catch (TargetInvocationException ex)
        {
            // Handle exceptions
            OnError(targetMethod, ex.InnerException ?? ex);
            throw;
        }
    }

    protected virtual void OnBefore(MethodInfo? method, object?[]? args) { }
    protected virtual void OnAfter(MethodInfo? method, object? result) { }
    protected virtual void OnError(MethodInfo? method, Exception ex) { }
}

// Logging proxy implementation
public class LoggingProxy<T> : BaseProxy<T>
{
    protected override void OnBefore(MethodInfo? method, object?[]? args)
    {
        if (method != null)
        {
            Console.WriteLine($"  [LOG] >> Calling: {method.Name}");
            if (args != null && args.Length > 0)
            {
                Console.WriteLine($"  [LOG]    Args: [{string.Join(", ", args)}]");
            }
        }
    }

    protected override void OnAfter(MethodInfo? method, object? result)
    {
        if (method != null)
        {
            Console.WriteLine($"  [LOG] << Returned: {result ?? "void"}");
        }
    }

    protected override void OnError(MethodInfo? method, Exception ex)
    {
        Console.WriteLine($"  [LOG] XX Error in {method?.Name}: {ex.Message}");
    }
}

// Timing proxy implementation
public class TimingProxy<T> : BaseProxy<T>
{
    private DateTime _startTime;

    protected override void OnBefore(MethodInfo? method, object?[]? args)
    {
        _startTime = DateTime.UtcNow;
    }

    protected override void OnAfter(MethodInfo? method, object? result)
    {
        if (method != null)
        {
            var elapsed = DateTime.UtcNow - _startTime;
            Console.WriteLine($"  [TIMING] {method.Name} completed in {elapsed.TotalMilliseconds:F2}ms");
        }
    }
}

// Factory for creating proxied instances
public static class ProxyFactory
{
    public static TService Create<TService, TProxy>()
        where TProxy : DispatchProxy
    {
        var proxy = DispatchProxy.Create<TService, TProxy>();
        return proxy;
    }
}
