using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace ConcurrentHttpClient;

/// <summary>
/// Concurrent HTTP client with rate limiting for parallel API requests.
/// Demonstrates semaphore-based throttling and parallel processing.
/// </summary>
class Program
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(30)
    };

    private static SemaphoreSlim _rateLimiter = new(5, 5); // Max 5 concurrent requests
    private static readonly ConcurrentBag<RequestResult> Results = new();
    private static int _requestsPerSecond = 10;
    private static int _maxRetries = 3;

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Concurrent HTTP Client with Rate Limiting");
            Console.WriteLine("==========================================");
            Console.WriteLine("Usage: dotnet run --project ConcurrentHttpClient.csproj [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --urls <file>       File with URLs (one per line)");
            Console.WriteLine("  --concurrency <n>   Max concurrent requests (default: 5)");
            Console.WriteLine("  --rps <n>           Requests per second limit (default: 10)");
            Console.WriteLine("  --timeout <ms>      Request timeout in ms (default: 30000)");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project ConcurrentHttpClient.csproj --urls urls.txt");
            Console.WriteLine("  dotnet run --project ConcurrentHttpClient.csproj --concurrency 10 --rps 20");
            return 0;
        }

        var urlsFile = string.Empty;

        for (int i = 0; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--urls" when i + 1 < args.Length:
                    urlsFile = args[++i];
                    break;
                case "--concurrency" when i + 1 < args.Length:
                    _rateLimiter.Dispose();
                    _rateLimiter = new SemaphoreSlim(int.Parse(args[++i]));
                    break;
                case "--rps" when i + 1 < args.Length:
                    _requestsPerSecond = int.Parse(args[++i]);
                    break;
                case "--timeout" when i + 1 < args.Length:
                    HttpClient.Timeout = TimeSpan.FromMilliseconds(int.Parse(args[++i]));
                    break;
            }
        }

        var urls = new List<string>();

        if (!string.IsNullOrEmpty(urlsFile) && File.Exists(urlsFile))
        {
            urls = (await File.ReadAllLinesAsync(urlsFile)).ToList();
            Console.WriteLine($"Loaded {urls.Count} URLs from {urlsFile}");
        }
        else
        {
            // Default demo URLs
            urls = new List<string>
            {
                "https://jsonplaceholder.typicode.com/posts/1",
                "https://jsonplaceholder.typicode.com/posts/2",
                "https://jsonplaceholder.typicode.com/posts/3",
                "https://jsonplaceholder.typicode.com/posts/4",
                "https://jsonplaceholder.typicode.com/posts/5",
                "https://jsonplaceholder.typicode.com/users/1",
                "https://jsonplaceholder.typicode.com/users/2",
                "https://jsonplaceholder.typicode.com/users/3",
                "https://jsonplaceholder.typicode.com/todos/1",
                "https://jsonplaceholder.typicode.com/todos/2",
                "https://httpbin.org/delay/1",
                "https://httpbin.org/delay/2",
                "https://httpbin.org/status/200",
                "https://httpbin.org/status/404",
                "https://httpbin.org/status/500"
            };
            Console.WriteLine($"Using {urls.Count} demo URLs");
        }

        Console.WriteLine($"\nConfiguration:");
        Console.WriteLine($"  Max concurrency: {_rateLimiter.CurrentCount}");
        Console.WriteLine($"  Rate limit: {_requestsPerSecond} req/sec");
        Console.WriteLine($"  Timeout: {HttpClient.Timeout.TotalSeconds}s");
        Console.WriteLine();

        var stopwatch = Stopwatch.StartNew();

        try
        {
            await ProcessUrlsAsync(urls);

            stopwatch.Stop();

            Console.WriteLine($"\n{'=',-50}");
            Console.WriteLine($"Completed in {stopwatch.ElapsedMilliseconds}ms");
            Console.WriteLine($"Total requests: {Results.Count}");
            Console.WriteLine($"Successful: {Results.Count(r => r.Success)}");
            Console.WriteLine($"Failed: {Results.Count(r => !r.Success)}");
            Console.WriteLine($"Average response time: {Results.Average(r => r.ResponseTimeMs):F2}ms");
            Console.WriteLine($"Requests/second: {Results.Count / (stopwatch.ElapsedMilliseconds / 1000.0):F2}");

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static async Task ProcessUrlsAsync(List<string> urls)
    {
        var rateLimiter = new RateLimiter(_requestsPerSecond);

        var tasks = urls.Select(async url =>
        {
            await rateLimiter.WaitAsync();
            await MakeRequestAsync(url);
        });

        await Task.WhenAll(tasks);
    }

    static async Task MakeRequestAsync(string url)
    {
        await _rateLimiter.WaitAsync();

        try
        {
            var stopwatch = Stopwatch.StartNew();
            HttpResponseMessage? response = null;
            int retries = 0;

            while (retries < _maxRetries)
            {
                try
                {
                    response = await HttpClient.GetAsync(url);
                    break;
                }
                catch (HttpRequestException ex) when (retries < _maxRetries - 1)
                {
                    retries++;
                    Console.WriteLine($"[{retries}/{_maxRetries}] Retrying {url}: {ex.Message}");
                    await Task.Delay(1000 * retries);
                }
            }

            stopwatch.Stop();

            if (response != null)
            {
                var result = new RequestResult
                {
                    Url = url,
                    Success = response.IsSuccessStatusCode,
                    StatusCode = (int)response.StatusCode,
                    ResponseTimeMs = stopwatch.ElapsedMilliseconds,
                    ContentLength = response.Content.Headers.ContentLength ?? 0
                };

                Results.Add(result);

                var status = result.Success ? "✓" : "✗";
                Console.WriteLine($"{status} {url} - {(int)result.StatusCode} ({result.ResponseTimeMs}ms)");
            }
        }
        catch (Exception ex)
        {
            Results.Add(new RequestResult
            {
                Url = url,
                Success = false,
                ErrorMessage = ex.Message,
                ResponseTimeMs = 0
            });
            Console.WriteLine($"✗ {url} - Error: {ex.Message}");
        }
        finally
        {
            _rateLimiter.Release();
        }
    }
}

class RequestResult
{
    public string Url { get; set; } = string.Empty;
    public bool Success { get; set; }
    public int StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public long ContentLength { get; set; }
    public string? ErrorMessage { get; set; }
}

/// <summary>
/// Rate limiter that controls requests per second using a sliding window.
/// </summary>
class RateLimiter
{
    private readonly int _maxRequestsPerSecond;
    private readonly Queue<DateTime> _requestTimes = new();
    private readonly SemaphoreSlim _semaphore;

    public RateLimiter(int maxRequestsPerSecond)
    {
        _maxRequestsPerSecond = maxRequestsPerSecond;
        _semaphore = new SemaphoreSlim(maxRequestsPerSecond, maxRequestsPerSecond);
    }

    public async Task WaitAsync()
    {
        await _semaphore.WaitAsync();

        lock (_requestTimes)
        {
            var now = DateTime.UtcNow;

            // Remove requests older than 1 second
            while (_requestTimes.Count > 0 && _requestTimes.Peek() < now - TimeSpan.FromSeconds(1))
            {
                _requestTimes.Dequeue();
            }

            // If at rate limit, wait
            if (_requestTimes.Count >= _maxRequestsPerSecond)
            {
                var oldestRequest = _requestTimes.Peek();
                var waitTime = oldestRequest + TimeSpan.FromSeconds(1) - now;
                if (waitTime > TimeSpan.Zero)
                {
                    Task.Delay(waitTime).Wait();
                }
            }

            _requestTimes.Enqueue(DateTime.UtcNow);
        }
    }
}
