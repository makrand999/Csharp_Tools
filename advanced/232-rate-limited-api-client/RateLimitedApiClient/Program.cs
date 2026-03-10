using System.Collections.Concurrent;
using System.Net.Http;
using System.Net.Http.Headers;

namespace RateLimitedApiClient;

/// <summary>
/// Rate Limited API Client - Implements token bucket algorithm for rate limiting.
/// Ensures API calls stay within specified rate limits to avoid throttling.
/// </summary>
public class RateLimiter
{
    private readonly int _maxTokens;
    private readonly double _refillRate; // tokens per second
    private double _currentTokens;
    private DateTime _lastRefill;
    private readonly object _lock = new();

    public RateLimiter(int requestsPerSecond)
    {
        _maxTokens = requestsPerSecond;
        _currentTokens = requestsPerSecond;
        _refillRate = requestsPerSecond;
        _lastRefill = DateTime.UtcNow;
    }

    public async Task AcquireTokenAsync(CancellationToken cancellationToken = default)
    {
        while (true)
        {
            if (TryConsumeToken())
                return;

            var waitTime = CalculateWaitTime();
            if (waitTime > 0)
                await Task.Delay(waitTime, cancellationToken);
        }
    }

    private bool TryConsumeToken()
    {
        lock (_lock)
        {
            RefillTokens();
            
            if (_currentTokens >= 1)
            {
                _currentTokens -= 1;
                return true;
            }
            
            return false;
        }
    }

    private void RefillTokens()
    {
        var now = DateTime.UtcNow;
        var elapsed = (now - _lastRefill).TotalSeconds;
        var tokensToAdd = elapsed * _refillRate;
        
        _currentTokens = Math.Min(_maxTokens, _currentTokens + tokensToAdd);
        _lastRefill = now;
    }

    private int CalculateWaitTime()
    {
        lock (_lock)
        {
            RefillTokens();
            
            if (_currentTokens >= 1)
                return 0;
            
            var tokensNeeded = 1 - _currentTokens;
            var waitTimeMs = (tokensNeeded / _refillRate) * 1000;
            return (int)Math.Ceiling(waitTimeMs);
        }
    }
}

public class RateLimitedApiClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly RateLimiter _rateLimiter;
    private readonly string _baseUrl;
    private int _totalRequests;
    private int _rateLimitedRequests;

    public RateLimitedApiClient(string baseUrl, int requestsPerSecond)
    {
        _baseUrl = baseUrl;
        _rateLimiter = new RateLimiter(requestsPerSecond);
        _httpClient = new HttpClient { BaseAddress = new Uri(baseUrl) };
        _httpClient.DefaultRequestHeaders.UserAgent.Add(
            new ProductInfoHeaderValue("RateLimitedClient", "1.0"));
    }

    public async Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        await _rateLimiter.AcquireTokenAsync(cancellationToken);
        Interlocked.Increment(ref _totalRequests);

        try
        {
            var response = await _httpClient.GetAsync(endpoint, cancellationToken);
            
            if (response.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
            {
                Interlocked.Increment(ref _rateLimitedRequests);
                
                if (response.Headers.RetryAfter?.Delta.HasValue == true)
                {
                    var retryAfter = response.Headers.RetryAfter.Delta.Value;
                    Console.WriteLine($"Rate limited! Waiting {retryAfter.TotalSeconds}s...");
                    await Task.Delay(retryAfter, cancellationToken);
                    return await GetAsync<T>(endpoint, cancellationToken);
                }
            }
            
            response.EnsureSuccessStatusCode();
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return System.Text.Json.JsonSerializer.Deserialize<T>(content);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Request failed: {ex.Message}");
            throw;
        }
    }

    public void PrintStatistics()
    {
        Console.WriteLine($"\n=== Rate Limited Client Statistics ===");
        Console.WriteLine($"Base URL: {_baseUrl}");
        Console.WriteLine($"Total Requests: {_totalRequests}");
        Console.WriteLine($"Rate Limited: {_rateLimitedRequests}");
        if (_totalRequests > 0)
        {
            var rate = (double)_rateLimitedRequests / _totalRequests * 100;
            Console.WriteLine($"Rate Limit Rate: {rate:F1}%");
        }
    }

    public void Dispose() => _httpClient.Dispose();
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Rate Limited API Client ===\n");
        Console.WriteLine("Using token bucket algorithm for rate limiting\n");

        // Using JSONPlaceholder API (free, no auth required)
        using var client = new RateLimitedApiClient(
            baseUrl: "https://jsonplaceholder.typicode.com",
            requestsPerSecond: 2); // Conservative rate

        Console.WriteLine("Fetching posts from JSONPlaceholder API...\n");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        // Fetch multiple resources with rate limiting
        var tasks = new List<Task>();
        for (int i = 1; i <= 5; i++)
        {
            tasks.Add(FetchAndDisplay<dynamic>(client, $"/posts/{i}", $"Post {i}"));
        }

        await Task.WhenAll(tasks);

        stopwatch.Stop();

        Console.WriteLine($"\nTotal time: {stopwatch.ElapsedMilliseconds}ms");
        client.PrintStatistics();

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }

    private static async Task FetchAndDisplay<T>(
        RateLimitedApiClient client, string endpoint, string label)
    {
        try
        {
            var result = await client.GetAsync<dynamic>(endpoint);
            Console.WriteLine($"✓ {label} fetched successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ {label} failed: {ex.Message}");
        }
    }
}
