using System.Collections.Concurrent;
using System.Net.Http;
using System.Threading;

namespace ThrottledHttpClient;

/// <summary>
/// Throttled HTTP Client - Controls concurrent HTTP requests with configurable limits.
/// Prevents overwhelming servers and handles rate limiting gracefully.
/// </summary>
public class ThrottledHttpClient : IDisposable
{
    private readonly HttpClient _httpClient;
    private readonly SemaphoreSlim _semaphore;
    private readonly int _maxConcurrency;
    private readonly TimeSpan _delayBetweenRequests;
    private int _totalRequests;
    private int _successfulRequests;
    private int _failedRequests;

    public ThrottledHttpClient(int maxConcurrency = 5, TimeSpan? delayBetweenRequests = null)
    {
        _maxConcurrency = maxConcurrency;
        _delayBetweenRequests = delayBetweenRequests ?? TimeSpan.FromMilliseconds(100);
        _httpClient = new HttpClient();
        _semaphore = new SemaphoreSlim(maxConcurrency, maxConcurrency);
        _totalRequests = 0;
        _successfulRequests = 0;
        _failedRequests = 0;
    }

    public async Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        Interlocked.Increment(ref _totalRequests);

        try
        {
            await Task.Delay(_delayBetweenRequests, cancellationToken);
            var response = await _httpClient.GetAsync(url, cancellationToken);
            
            if (response.IsSuccessStatusCode)
                Interlocked.Increment(ref _successfulRequests);
            else
                Interlocked.Increment(ref _failedRequests);
            
            return response;
        }
        catch (Exception)
        {
            Interlocked.Increment(ref _failedRequests);
            throw;
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<IEnumerable<(string Url, string? Content, bool Success)>> DownloadBatchAsync(
        IEnumerable<string> urls, 
        CancellationToken cancellationToken = default)
    {
        var tasks = urls.Select(url => DownloadSingleAsync(url, cancellationToken));
        var results = await Task.WhenAll(tasks);
        return results;
    }

    private async Task<(string Url, string? Content, bool Success)> DownloadSingleAsync(
        string url, CancellationToken cancellationToken)
    {
        try
        {
            var response = await GetAsync(url, cancellationToken);
            var content = await response.Content.ReadAsStringAsync(cancellationToken);
            return (url, content, response.IsSuccessStatusCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to download {url}: {ex.Message}");
            return (url, null, false);
        }
    }

    public void PrintStatistics()
    {
        Console.WriteLine($"\n=== HTTP Client Statistics ===");
        Console.WriteLine($"Max Concurrency: {_maxConcurrency}");
        Console.WriteLine($"Total Requests: {_totalRequests}");
        Console.WriteLine($"Successful: {_successfulRequests}");
        Console.WriteLine($"Failed: {_failedRequests}");
        if (_totalRequests > 0)
        {
            var successRate = (double)_successfulRequests / _totalRequests * 100;
            Console.WriteLine($"Success Rate: {successRate:F1}%");
        }
    }

    public void Dispose()
    {
        _semaphore.Dispose();
        _httpClient.Dispose();
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("=== Throttled HTTP Client ===\n");

        // Demo URLs (using httpbin.org for testing)
        var urls = new[]
        {
            "https://httpbin.org/delay/1",
            "https://httpbin.org/delay/1",
            "https://httpbin.org/delay/1",
            "https://httpbin.org/delay/1",
            "https://httpbin.org/delay/1",
            "https://httpbin.org/delay/1",
            "https://httpbin.org/delay/1",
            "https://httpbin.org/delay/1"
        };

        using var client = new ThrottledHttpClient(maxConcurrency: 3, TimeSpan.FromMilliseconds(200));

        Console.WriteLine($"Downloading {urls.Length} URLs with max 3 concurrent requests...\n");

        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        var results = await client.DownloadBatchAsync(urls);

        stopwatch.Stop();

        foreach (var (url, content, success) in results)
        {
            var status = success ? "✓" : "✗";
            var size = content?.Length ?? 0;
            Console.WriteLine($"{status} {url} - {size} bytes");
        }

        Console.WriteLine($"\nTotal time: {stopwatch.ElapsedMilliseconds}ms");
        client.PrintStatistics();

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
