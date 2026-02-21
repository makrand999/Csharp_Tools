namespace RateLimiter;

/// <summary>
/// Rate Limiter - Token bucket algorithm implementation
/// Controls the rate of operations to prevent overload
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Rate Limiter - Token Bucket Algorithm");
        Console.WriteLine("=====================================\n");

        // Create rate limiter: 5 tokens max, refill 2 tokens per second
        var limiter = new TokenBucketRateLimiter(maxTokens: 5, refillRate: 2);

        Console.WriteLine("Configuration: 5 tokens max, 2 tokens/second refill\n");
        Console.WriteLine("Attempting 15 rapid requests...\n");

        var tasks = Enumerable.Range(1, 15).Select(async i =>
        {
            var start = DateTime.Now;
            var result = await limiter.ExecuteAsync(async () =>
            {
                await Task.Delay(50);
                return $"Request-{i}";
            });

            var elapsed = DateTime.Now - start;
            
            if (result != null)
            {
                Console.WriteLine($"[{elapsed.TotalMilliseconds,6:F0}ms] ✓ {result}");
            }
            else
            {
                Console.WriteLine($"[{elapsed.TotalMilliseconds,6:F0}ms] ✗ Request-{i} - Rate limited");
            }
        });

        await Task.WhenAll(tasks);

        Console.WriteLine("\n--- Sliding Window Rate Limiter ---\n");

        var slidingLimiter = new SlidingWindowRateLimiter(maxRequests: 5, windowSize: TimeSpan.FromSeconds(2));

        Console.WriteLine("Configuration: 5 requests per 2-second window\n");
        Console.WriteLine("Attempting 10 rapid requests...\n");

        for (int i = 1; i <= 10; i++)
        {
            var start = DateTime.Now;
            var allowed = await slidingLimiter.AllowRequestAsync();
            var elapsed = DateTime.Now - start;

            if (allowed)
            {
                Console.WriteLine($"[{elapsed.TotalMilliseconds,6:F0}ms] ✓ Request #{i} - Allowed");
            }
            else
            {
                Console.WriteLine($"[{elapsed.TotalMilliseconds,6:F0}ms] ✗ Request #{i} - Rate limited");
            }

            await Task.Delay(200);
        }

        Console.WriteLine("\nWaiting for window to slide...");
        await Task.Delay(2500);

        Console.WriteLine("\nAttempting more requests after window reset:");
        for (int i = 11; i <= 13; i++)
        {
            var allowed = await slidingLimiter.AllowRequestAsync();
            Console.WriteLine($"✓ Request #{i} - {(allowed ? "Allowed" : "Rate limited")}");
        }
    }
}

class TokenBucketRateLimiter
{
    private readonly int _maxTokens;
    private readonly double _refillRate;
    private double _tokens;
    private DateTime _lastRefill;
    private readonly object _lock = new();

    public TokenBucketRateLimiter(int maxTokens, double refillRate)
    {
        _maxTokens = maxTokens;
        _refillRate = refillRate;
        _tokens = maxTokens;
        _lastRefill = DateTime.Now;
    }

    private void Refill()
    {
        var elapsed = (DateTime.Now - _lastRefill).TotalSeconds;
        _tokens = Math.Min(_maxTokens, _tokens + elapsed * _refillRate);
        _lastRefill = DateTime.Now;
    }

    public async Task<T?> ExecuteAsync<T>(Func<Task<T>> action)
    {
        lock (_lock)
        {
            Refill();
            
            if (_tokens < 1)
            {
                return default;
            }
            
            _tokens--;
        }

        return await action();
    }
}

class SlidingWindowRateLimiter
{
    private readonly int _maxRequests;
    private readonly TimeSpan _windowSize;
    private readonly Queue<DateTime> _requests = new();
    private readonly object _lock = new();

    public SlidingWindowRateLimiter(int maxRequests, TimeSpan windowSize)
    {
        _maxRequests = maxRequests;
        _windowSize = windowSize;
    }

    public Task<bool> AllowRequestAsync()
    {
        lock (_lock)
        {
            var now = DateTime.Now;
            var windowStart = now - _windowSize;

            // Remove old requests outside the window
            while (_requests.Count > 0 && _requests.Peek() < windowStart)
            {
                _requests.Dequeue();
            }

            if (_requests.Count >= _maxRequests)
            {
                return Task.FromResult(false);
            }

            _requests.Enqueue(now);
            return Task.FromResult(true);
        }
    }
}
