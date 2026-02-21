namespace CircuitBreaker;

/// <summary>
/// Circuit Breaker Pattern - Resilience pattern for handling failures
/// Prevents cascading failures by failing fast when service is unhealthy
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Circuit Breaker Pattern");
        Console.WriteLine("=======================\n");

        var breaker = new CircuitBreaker(
            failureThreshold: 3,
            recoveryTimeout: TimeSpan.FromSeconds(5)
        );

        var simulator = new FaultyService(failureRate: 0.7);

        Console.WriteLine("Simulating service calls with 70% failure rate...");
        Console.WriteLine("Circuit will open after 3 consecutive failures\n");

        for (int i = 1; i <= 15; i++)
        {
            Console.Write($"Call #{i}: ");
            
            try
            {
                var result = await breaker.ExecuteAsync(async () => await simulator.CallAsync());
                Console.WriteLine($"✓ Success (Response: {result})");
            }
            catch (CircuitOpenException)
            {
                Console.WriteLine("⚡ Circuit OPEN - Failing fast");
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"✗ Service Error: {ex.Message}");
            }

            Console.WriteLine($"   State: {breaker.State} (Failures: {breaker.ConsecutiveFailures})");
            await Task.Delay(500);
        }

        Console.WriteLine("\nWaiting for circuit to recover...");
        await Task.Delay(6000);

        Console.WriteLine("\nAttempting recovery calls:");
        for (int i = 16; i <= 20; i++)
        {
            Console.Write($"Call #{i}: ");
            try
            {
                var result = await breaker.ExecuteAsync(async () => await simulator.CallAsync());
                Console.WriteLine($"✓ Success (Response: {result})");
            }
            catch (CircuitOpenException)
            {
                Console.WriteLine("⚡ Circuit OPEN - Failing fast");
            }
            catch (ServiceException ex)
            {
                Console.WriteLine($"✗ Service Error: {ex.Message}");
            }
            Console.WriteLine($"   State: {breaker.State}");
            await Task.Delay(500);
        }
    }
}

enum CircuitState { Closed, Open, HalfOpen }

class CircuitBreaker
{
    private readonly int _failureThreshold;
    private readonly TimeSpan _recoveryTimeout;
    private CircuitState _state = CircuitState.Closed;
    private int _consecutiveFailures;
    private DateTime _nextRetryTime;

    public CircuitState State => _state;
    public int ConsecutiveFailures => _consecutiveFailures;

    public CircuitBreaker(int failureThreshold, TimeSpan recoveryTimeout)
    {
        _failureThreshold = failureThreshold;
        _recoveryTimeout = recoveryTimeout;
    }

    public async Task<T> ExecuteAsync<T>(Func<Task<T>> action)
    {
        if (_state == CircuitState.Open)
        {
            if (DateTime.Now >= _nextRetryTime)
            {
                _state = CircuitState.HalfOpen;
                Console.WriteLine("   [Circuit] Transitioning to HALF-OPEN for test");
            }
            else
            {
                throw new CircuitOpenException();
            }
        }

        try
        {
            var result = await action();
            
            if (_state == CircuitState.HalfOpen)
            {
                _state = CircuitState.Closed;
                _consecutiveFailures = 0;
                Console.WriteLine("   [Circuit] Recovery successful - Circuit CLOSED");
            }
            else if (_state == CircuitState.Closed)
            {
                _consecutiveFailures = 0;
            }
            
            return result;
        }
        catch
        {
            _consecutiveFailures++;
            
            if (_state == CircuitState.HalfOpen || _consecutiveFailures >= _failureThreshold)
            {
                _state = CircuitState.Open;
                _nextRetryTime = DateTime.Now + _recoveryTimeout;
                Console.WriteLine($"   [Circuit] Opening circuit for {_recoveryTimeout.TotalSeconds}s");
            }
            
            throw;
        }
    }
}

class CircuitOpenException : Exception { }
class ServiceException : Exception { public ServiceException(string message) : base(message) { } }

class FaultyService
{
    private readonly double _failureRate;
    private readonly Random _random = new();

    public FaultyService(double failureRate) => _failureRate = failureRate;

    public async Task<string> CallAsync()
    {
        await Task.Delay(100);
        
        if (_random.NextDouble() < _failureRate)
        {
            throw new ServiceException("Service unavailable");
        }
        
        return $"OK-{Guid.NewGuid().ToString()[..8]}";
    }
}
