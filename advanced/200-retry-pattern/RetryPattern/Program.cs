namespace RetryPattern;

using System.Diagnostics;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Retry Pattern Implementation - Resilient operations with retry policies");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project RetryPattern.csproj -- demo");
            Console.WriteLine("  dotnet run --project RetryPattern.csproj -- interactive");
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
        Console.WriteLine("=== Retry Pattern Implementation Demo ===\n");

        // Demo 1: Basic Retry with Fixed Delay
        Console.WriteLine("1. Basic Retry (Fixed Delay)");
        Console.WriteLine("----------------------------");
        
        var fixedRetry = RetryPolicy.Create()
            .WithMaxRetries(3)
            .WithFixedDelay(TimeSpan.FromMilliseconds(500));
        
        var operation1 = new FlakyOperation(2); // Fails twice, succeeds on 3rd
        
        try
        {
            var result = fixedRetry.Execute(() => operation1.Execute());
            Console.WriteLine($"Success after {operation1.AttemptCount} attempts: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
        Console.WriteLine();

        // Demo 2: Exponential Backoff
        Console.WriteLine("2. Exponential Backoff Retry");
        Console.WriteLine("----------------------------");
        
        var exponentialRetry = RetryPolicy.Create()
            .WithMaxRetries(5)
            .WithExponentialBackoff(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(2));
        
        var operation2 = new FlakyOperation(3);
        
        try
        {
            var result = exponentialRetry.Execute(() => operation2.Execute());
            Console.WriteLine($"Success after {operation2.AttemptCount} attempts: {result}");
            Console.WriteLine($"Total time: {operation2.TotalTime.ElapsedMilliseconds}ms");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
        Console.WriteLine();

        // Demo 3: Retry with Jitter
        Console.WriteLine("3. Exponential Backoff with Jitter");
        Console.WriteLine("----------------------------------");
        
        var jitterRetry = RetryPolicy.Create()
            .WithMaxRetries(4)
            .WithExponentialBackoffAndJitter(
                TimeSpan.FromMilliseconds(100),
                TimeSpan.FromSeconds(1),
                jitterFactor: 0.5);
        
        var operation3 = new FlakyOperation(2);
        
        try
        {
            var result = jitterRetry.Execute(() => operation3.Execute());
            Console.WriteLine($"Success after {operation3.AttemptCount} attempts: {result}");
            Console.WriteLine($"Delays: {string.Join(", ", operation3.Delays.Select(d => $"{d.TotalMilliseconds:F0}ms"))}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
        Console.WriteLine();

        // Demo 4: Retry with Specific Exception Handling
        Console.WriteLine("4. Selective Exception Handling");
        Console.WriteLine("-------------------------------");
        
        var selectiveRetry = RetryPolicy.Create()
            .WithMaxRetries(3)
            .WithFixedDelay(TimeSpan.FromMilliseconds(200))
            .Handle<TimeoutException>()
            .Handle<InvalidOperationException>();
        
        var operation4 = new FlakyOperationWithExceptionTypes();
        
        try
        {
            var result = selectiveRetry.Execute(() => operation4.Execute());
            Console.WriteLine($"Success: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed with non-retryable exception: {ex.GetType().Name}");
        }
        Console.WriteLine();

        // Demo 5: Retry with Callback/Notification
        Console.WriteLine("5. Retry with OnRetry Callback");
        Console.WriteLine("------------------------------");
        
        var callbackRetry = RetryPolicy.Create()
            .WithMaxRetries(3)
            .WithFixedDelay(TimeSpan.FromMilliseconds(300))
            .OnRetry((attempt, exception, delay) =>
            {
                Console.WriteLine($"  Retry {attempt}: {exception.GetType().Name}, waiting {delay.TotalMilliseconds:F0}ms");
            });
        
        var operation5 = new FlakyOperation(2);
        
        try
        {
            var result = callbackRetry.Execute(() => operation5.Execute());
            Console.WriteLine($"Final success: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
        Console.WriteLine();

        // Demo 6: Async Retry
        Console.WriteLine("6. Async Retry Operations");
        Console.WriteLine("-------------------------");
        
        var asyncRetry = RetryPolicy.Create()
            .WithMaxRetries(3)
            .WithExponentialBackoff(TimeSpan.FromMilliseconds(100), TimeSpan.FromMilliseconds(500))
            .OnRetry((attempt, exception, delay) =>
            {
                Console.WriteLine($"  Attempt {attempt + 1} failed: {exception.Message}");
            });
        
        var asyncOperation = new AsyncFlakyOperation(2);
        
        try
        {
            var result = asyncRetry.ExecuteAsync(async () => await asyncOperation.ExecuteAsync()).Result;
            Console.WriteLine($"Async success: {result}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
        Console.WriteLine();

        // Demo 7: Retry with Timeout
        Console.WriteLine("7. Retry with Timeout");
        Console.WriteLine("---------------------");
        
        var timeoutRetry = RetryPolicy.Create()
            .WithMaxRetries(3)
            .WithFixedDelay(TimeSpan.FromMilliseconds(200))
            .WithTimeout(TimeSpan.FromSeconds(1));
        
        var slowOperation = new SlowFlakyOperation(3, TimeSpan.FromMilliseconds(600));
        
        try
        {
            var result = timeoutRetry.Execute(() => slowOperation.Execute());
            Console.WriteLine($"Success: {result}");
        }
        catch (TimeoutException ex)
        {
            Console.WriteLine($"Timeout: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed: {ex.Message}");
        }
        Console.WriteLine();

        // Demo 8: Circuit Breaker Integration
        Console.WriteLine("8. Retry with Circuit Breaker");
        Console.WriteLine("-----------------------------");
        
        var circuitBreaker = new CircuitBreaker(3, TimeSpan.FromSeconds(5));
        var circuitRetry = RetryPolicy.Create()
            .WithMaxRetries(2)
            .WithFixedDelay(TimeSpan.FromMilliseconds(100))
            .WithCircuitBreaker(circuitBreaker);
        
        var failingOperation = new AlwaysFailingOperation();
        
        Console.WriteLine("Attempting operations (circuit will open after 3 failures):");
        for (int i = 0; i < 5; i++)
        {
            try
            {
                circuitRetry.Execute(() => failingOperation.Execute());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"  Attempt {i + 1}: {ex.GetType().Name} - {ex.Message}");
            }
        }
        Console.WriteLine($"Circuit state: {circuitBreaker.State}");
        Console.WriteLine();

        // Demo 9: Statistics
        Console.WriteLine("9. Retry Statistics");
        Console.WriteLine("-------------------");
        
        var statsRetry = RetryPolicy.Create()
            .WithMaxRetries(3)
            .WithFixedDelay(TimeSpan.FromMilliseconds(100));
        
        var statsOp = new FlakyOperation(1);
        statsRetry.Execute(() => statsOp.Execute());
        
        var stats = statsRetry.GetStatistics();
        Console.WriteLine($"Total executions: {stats.TotalExecutions}");
        Console.WriteLine($"Successful: {stats.SuccessfulExecutions}");
        Console.WriteLine($"Failed: {stats.FailedExecutions}");
        Console.WriteLine($"Total retries: {stats.TotalRetries}");
        Console.WriteLine($"Success rate: {stats.SuccessRate:P1}");
        Console.WriteLine();

        Console.WriteLine("Demo completed!");
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("Retry Pattern (Interactive Mode)");
        Console.WriteLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine();

        var policy = RetryPolicy.Create()
            .WithMaxRetries(3)
            .WithExponentialBackoff(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(2))
            .OnRetry((attempt, ex, delay) =>
            {
                Console.WriteLine($"  Retry {attempt}: {ex.Message}");
            });

        while (true)
        {
            Console.Write("rp> ");
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

                    case "test":
                        if (parts.Length < 3)
                        {
                            Console.WriteLine("Usage: test <fail-count> <operation-type>");
                            Console.WriteLine("  operation-type: flaky, slow, async");
                            break;
                        }
                        var failCount = int.Parse(parts[1]);
                        var opType = parts[2];
                        
                        switch (opType)
                        {
                            case "flaky":
                                var flaky = new FlakyOperation(failCount);
                                var result = policy.Execute(() => flaky.Execute());
                                Console.WriteLine($"Success: {result} after {flaky.AttemptCount} attempts");
                                break;
                            case "slow":
                                var slow = new SlowFlakyOperation(failCount, TimeSpan.FromMilliseconds(200));
                                var slowResult = policy.Execute(() => slow.Execute());
                                Console.WriteLine($"Success: {slowResult}");
                                break;
                            default:
                                Console.WriteLine($"Unknown operation type: {opType}");
                                break;
                        }
                        break;

                    case "config":
                        if (parts.Length < 2)
                        {
                            Console.WriteLine("Usage: config <max-retries>");
                            break;
                        }
                        var maxRetries = int.Parse(parts[1]);
                        policy = RetryPolicy.Create()
                            .WithMaxRetries(maxRetries)
                            .WithExponentialBackoff(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(2));
                        Console.WriteLine($"Max retries set to {maxRetries}");
                        break;

                    case "stats":
                        var stats = policy.GetStatistics();
                        Console.WriteLine($"Executions: {stats.TotalExecutions}");
                        Console.WriteLine($"Success: {stats.SuccessfulExecutions}");
                        Console.WriteLine($"Failed: {stats.FailedExecutions}");
                        Console.WriteLine($"Retries: {stats.TotalRetries}");
                        break;

                    case "reset":
                        policy = RetryPolicy.Create()
                            .WithMaxRetries(3)
                            .WithExponentialBackoff(TimeSpan.FromMilliseconds(100), TimeSpan.FromSeconds(2));
                        Console.WriteLine("Policy reset");
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

    private static void ShowHelp()
    {
        Console.WriteLine();
        Console.WriteLine("Commands:");
        Console.WriteLine("  test <fails> <type>  - Test retry with flaky operation");
        Console.WriteLine("  config <retries>     - Configure max retries");
        Console.WriteLine("  stats                - Show retry statistics");
        Console.WriteLine("  reset                - Reset policy");
        Console.WriteLine("  quit                 - Exit");
        Console.WriteLine();
    }
}

// Flaky operation that fails N times then succeeds
public class FlakyOperation
{
    private readonly int _failuresBeforeSuccess;
    public int AttemptCount { get; private set; }
    public Stopwatch TotalTime { get; } = new();
    public List<TimeSpan> Delays { get; } = new();
    private DateTime _lastAttempt;

    public FlakyOperation(int failuresBeforeSuccess)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
        _lastAttempt = DateTime.UtcNow;
        TotalTime.Start();
    }

    public string Execute()
    {
        AttemptCount++;
        var now = DateTime.UtcNow;
        
        if (AttemptCount > 1)
        {
            Delays.Add(now - _lastAttempt);
        }
        _lastAttempt = now;

        if (AttemptCount <= _failuresBeforeSuccess)
        {
            throw new InvalidOperationException($"Simulated failure (attempt {AttemptCount})");
        }

        TotalTime.Stop();
        return $"Success on attempt {AttemptCount}";
    }
}

// Flaky operation with different exception types
public class FlakyOperationWithExceptionTypes
{
    private int _attemptCount;

    public string Execute()
    {
        _attemptCount++;
        
        return _attemptCount switch
        {
            1 => throw new TimeoutException("Operation timed out"),
            2 => throw new InvalidOperationException("Invalid state"),
            3 => throw new ArgumentException("Invalid argument"), // Non-retryable
            _ => "Success"
        };
    }
}

// Async flaky operation
public class AsyncFlakyOperation
{
    private readonly int _failuresBeforeSuccess;
    private int _attemptCount;

    public AsyncFlakyOperation(int failuresBeforeSuccess)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public async Task<string> ExecuteAsync()
    {
        _attemptCount++;
        await Task.Delay(50);

        if (_attemptCount <= _failuresBeforeSuccess)
        {
            throw new InvalidOperationException($"Async failure (attempt {_attemptCount})");
        }

        return $"Async success on attempt {_attemptCount}";
    }
}

// Slow flaky operation
public class SlowFlakyOperation
{
    private readonly int _failuresBeforeSuccess;
    private readonly TimeSpan _executionTime;
    private int _attemptCount;

    public SlowFlakyOperation(int failuresBeforeSuccess, TimeSpan executionTime)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
        _executionTime = executionTime;
    }

    public string Execute()
    {
        _attemptCount++;
        Thread.Sleep(_executionTime);

        if (_attemptCount <= _failuresBeforeSuccess)
        {
            throw new TimeoutException($"Slow operation failed (attempt {_attemptCount})");
        }

        return $"Slow success on attempt {_attemptCount}";
    }
}

// Always failing operation (for circuit breaker testing)
public class AlwaysFailingOperation
{
    public string Execute()
    {
        throw new InvalidOperationException("Always fails");
    }
}

// Retry statistics
public record RetryStatistics(
    int TotalExecutions,
    int SuccessfulExecutions,
    int FailedExecutions,
    int TotalRetries
)
{
    public double SuccessRate => TotalExecutions > 0 ? (double)SuccessfulExecutions / TotalExecutions : 0;
}

// Retry policy builder
public class RetryPolicy
{
    private int _maxRetries = 3;
    private Func<int, TimeSpan> _delayFactory = _ => TimeSpan.Zero;
    private Action<int, Exception, TimeSpan>? _onRetry;
    private HashSet<Type> _handledExceptions = new();
    private TimeSpan _timeout = TimeSpan.MaxValue;
    private CircuitBreaker? _circuitBreaker;
    
    private int _totalExecutions;
    private int _successfulExecutions;
    private int _failedExecutions;
    private int _totalRetries;

    public static RetryPolicy Create() => new();

    public RetryPolicy WithMaxRetries(int maxRetries)
    {
        _maxRetries = maxRetries;
        return this;
    }

    public RetryPolicy WithFixedDelay(TimeSpan delay)
    {
        _delayFactory = _ => delay;
        return this;
    }

    public RetryPolicy WithExponentialBackoff(TimeSpan initialDelay, TimeSpan maxDelay)
    {
        _delayFactory = attempt =>
        {
            var delay = initialDelay.TotalMilliseconds * Math.Pow(2, attempt);
            return TimeSpan.FromMilliseconds(Math.Min(delay, maxDelay.TotalMilliseconds));
        };
        return this;
    }

    public RetryPolicy WithExponentialBackoffAndJitter(TimeSpan initialDelay, TimeSpan maxDelay, double jitterFactor)
    {
        var random = new Random();
        _delayFactory = attempt =>
        {
            var delay = initialDelay.TotalMilliseconds * Math.Pow(2, attempt);
            var jitter = delay * jitterFactor * (random.NextDouble() * 2 - 1);
            return TimeSpan.FromMilliseconds(Math.Min(delay + jitter, maxDelay.TotalMilliseconds));
        };
        return this;
    }

    public RetryPolicy OnRetry(Action<int, Exception, TimeSpan> onRetry)
    {
        _onRetry = onRetry;
        return this;
    }

    public RetryPolicy Handle<TException>() where TException : Exception
    {
        _handledExceptions.Add(typeof(TException));
        return this;
    }

    public RetryPolicy WithTimeout(TimeSpan timeout)
    {
        _timeout = timeout;
        return this;
    }

    public RetryPolicy WithCircuitBreaker(CircuitBreaker circuitBreaker)
    {
        _circuitBreaker = circuitBreaker;
        return this;
    }

    public T Execute<T>(Func<T> action)
    {
        var lastException = default(Exception);
        
        for (int attempt = 0; attempt <= _maxRetries; attempt++)
        {
            Interlocked.Increment(ref _totalExecutions);

            // Check circuit breaker
            if (_circuitBreaker?.State == CircuitState.Open)
            {
                throw new CircuitOpenException("Circuit breaker is open");
            }

            try
            {
                // Execute with timeout
                var result = ExecuteWithTimeout(action);
                
                Interlocked.Increment(ref _successfulExecutions);
                _circuitBreaker?.RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                lastException = ex;
                
                // Check if exception is handled
                if (!IsHandledException(ex))
                {
                    Interlocked.Increment(ref _failedExecutions);
                    throw;
                }

                // Record failure with circuit breaker
                _circuitBreaker?.RecordFailure();

                if (attempt < _maxRetries)
                {
                    Interlocked.Increment(ref _totalRetries);
                    var delay = _delayFactory(attempt);
                    _onRetry?.Invoke(attempt + 1, ex, delay);
                    Thread.Sleep(delay);
                }
            }
        }

        Interlocked.Increment(ref _failedExecutions);
        throw new RetryExhaustedException($"Retry exhausted after {_maxRetries + 1} attempts", lastException);
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        var lastException = default(Exception);
        
        for (int attempt = 0; attempt <= _maxRetries; attempt++)
        {
            Interlocked.Increment(ref _totalExecutions);

            if (_circuitBreaker?.State == CircuitState.Open)
            {
                throw new CircuitOpenException("Circuit breaker is open");
            }

            try
            {
                var result = await ExecuteWithTimeoutAsync(action);
                
                Interlocked.Increment(ref _successfulExecutions);
                _circuitBreaker?.RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                lastException = ex;
                
                if (!IsHandledException(ex))
                {
                    Interlocked.Increment(ref _failedExecutions);
                    throw;
                }

                _circuitBreaker?.RecordFailure();

                if (attempt < _maxRetries)
                {
                    Interlocked.Increment(ref _totalRetries);
                    var delay = _delayFactory(attempt);
                    _onRetry?.Invoke(attempt + 1, ex, delay);
                    await Task.Delay(delay);
                }
            }
        }

        Interlocked.Increment(ref _failedExecutions);
        throw new RetryExhaustedException($"Retry exhausted after {_maxRetries + 1} attempts", lastException);
    }

    public RetryStatistics GetStatistics() => new(
        Volatile.Read(ref _totalExecutions),
        Volatile.Read(ref _successfulExecutions),
        Volatile.Read(ref _failedExecutions),
        Volatile.Read(ref _totalRetries)
    );

    private T ExecuteWithTimeout<T>(Func<T> action)
    {
        var cts = new CancellationTokenSource(_timeout);
        var task = Task.Run(action, cts.Token);
        
        if (!task.Wait(_timeout, cts.Token))
        {
            throw new TimeoutException("Operation timed out");
        }
        
        return task.Result;
    }

    private async Task<T> ExecuteWithTimeoutAsync<T>(Func<Task<T>> action)
    {
        using var cts = new CancellationTokenSource(_timeout);
        var task = action();
        var delayTask = Task.Delay(_timeout, cts.Token);
        
        if (await Task.WhenAny(task, delayTask) == delayTask)
        {
            throw new TimeoutException("Operation timed out");
        }
        
        return await task;
    }

    private bool IsHandledException(Exception ex)
    {
        return _handledExceptions.Count == 0 || _handledExceptions.Contains(ex.GetType());
    }
}

// Circuit breaker state
public enum CircuitState { Closed, Open, HalfOpen }

// Circuit breaker implementation
public class CircuitBreaker
{
    private readonly int _failureThreshold;
    private readonly TimeSpan _recoveryTimeout;
    private int _failureCount;
    private DateTime _lastFailureTime;
    private CircuitState _state = CircuitState.Closed;
    private readonly object _lock = new();

    public CircuitBreaker(int failureThreshold, TimeSpan recoveryTimeout)
    {
        _failureThreshold = failureThreshold;
        _recoveryTimeout = recoveryTimeout;
    }

    public CircuitState State
    {
        get
        {
            lock (_lock)
            {
                if (_state == CircuitState.Open && DateTime.UtcNow - _lastFailureTime >= _recoveryTimeout)
                {
                    _state = CircuitState.HalfOpen;
                }
                return _state;
            }
        }
    }

    public void RecordSuccess()
    {
        lock (_lock)
        {
            _failureCount = 0;
            _state = CircuitState.Closed;
        }
    }

    public void RecordFailure()
    {
        lock (_lock)
        {
            _failureCount++;
            _lastFailureTime = DateTime.UtcNow;
            
            if (_failureCount >= _failureThreshold)
            {
                _state = CircuitState.Open;
            }
        }
    }
}

// Custom exceptions
public class RetryExhaustedException : Exception
{
    public RetryExhaustedException(string message, Exception? innerException) 
        : base(message, innerException) { }
}

public class CircuitOpenException : Exception
{
    public CircuitOpenException(string message) : base(message) { }
}
