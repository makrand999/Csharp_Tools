using System.Collections.Concurrent;
using System.Diagnostics;
using System.Net.Http;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();
var monitor = new WebsiteMonitor();

switch (command)
{
    case "check":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: URL required");
            return;
        }
        await monitor.CheckSingle(args[1]);
        break;
        
    case "batch":
        if (args.Length < 2)
        {
            Console.Error.WriteLine("Error: File path required");
            return;
        }
        await monitor.CheckBatch(args[1]);
        break;
        
    case "watch":
        if (args.Length < 3)
        {
            Console.Error.WriteLine("Error: URL and interval required");
            return;
        }
        if (!int.TryParse(args[2], out var interval) || interval < 1)
        {
            Console.Error.WriteLine("Error: Interval must be a positive number (seconds)");
            return;
        }
        await monitor.WatchContinuous(args[1], interval);
        break;
        
    default:
        Console.Error.WriteLine($"Unknown command: {command}");
        ShowHelp();
        break;
}

void ShowHelp()
{
    Console.WriteLine("""
        Website Uptime Monitor
        
        Usage:
          dotnet run --project UptimeMonitor/UptimeMonitor.csproj <command> [arguments]
        
        Commands:
          check <url>              Check single website status
          batch <file>             Check multiple URLs from file (one per line)
          watch <url> <interval>   Continuously monitor with interval (seconds)
        
        Examples:
          dotnet run --project UptimeMonitor/UptimeMonitor.csproj check https://google.com
          dotnet run --project UptimeMonitor/UptimeMonitor.csproj batch websites.txt
          dotnet run --project UptimeMonitor/UptimeMonitor.csproj watch https://api.example.com 30
        """);
}

class WebsiteMonitor
{
    private static readonly HttpClient HttpClient = new()
    {
        Timeout = TimeSpan.FromSeconds(10)
    };

    public async Task CheckSingle(string url)
    {
        var result = await CheckUrl(url);
        PrintResult(result);
    }
    
    public async Task CheckBatch(string inputFile)
    {
        if (!File.Exists(inputFile))
        {
            Console.Error.WriteLine($"File not found: {inputFile}");
            return;
        }
        
        var urls = File.ReadAllLines(inputFile)
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l) && !l.StartsWith('#'))
            .ToList();
        
        Console.WriteLine($"Checking {urls.Count} websites...\n");
        
        var results = new ConcurrentBag<CheckResult>();
        var tasks = urls.Select(url => CheckAndReport(url, results));
        await Task.WhenAll(tasks);
        
        PrintSummary(results.ToList());
    }
    
    public async Task WatchContinuous(string url, int intervalSeconds)
    {
        Console.WriteLine($"Monitoring {url} every {intervalSeconds} seconds...");
        Console.WriteLine("Press Ctrl+C to stop\n");
        
        var checkCount = 0;
        var upCount = 0;
        
        while (true)
        {
            checkCount++;
            var result = await CheckUrl(url);
            
            if (result.IsUp) upCount++;
            
            var uptime = (upCount / (double)checkCount) * 100;
            var statusIcon = result.IsUp ? "✓ UP" : "✗ DOWN";
            var timestamp = DateTime.Now.ToString("HH:mm:ss");
            
            Console.WriteLine($"[{timestamp}] {statusIcon} - Response: {result.ResponseTimeMs}ms - Status: {result.StatusCode}");
            Console.WriteLine($"  Uptime: {uptime:F1}% ({upCount}/{checkCount} checks)");
            
            await Task.Delay(TimeSpan.FromSeconds(intervalSeconds));
        }
    }
    
    private async Task CheckAndReport(string url, ConcurrentBag<CheckResult> results)
    {
        var result = await CheckUrl(url);
        results.Add(result);
        PrintResult(result);
    }
    
    private async Task<CheckResult> CheckUrl(string url)
    {
        var result = new CheckResult { Url = url };
        var stopwatch = Stopwatch.StartNew();
        
        try
        {
            // Ensure URL has protocol
            if (!url.StartsWith("http://") && !url.StartsWith("https://"))
            {
                url = "https://" + url;
                result.Url = url;
            }
            
            var response = await HttpClient.GetAsync(url);
            stopwatch.Stop();
            
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.StatusCode = response.StatusCode;
            result.IsUp = response.IsSuccessStatusCode;
        }
        catch (TaskCanceledException)
        {
            stopwatch.Stop();
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.Error = "Timeout (10s)";
        }
        catch (HttpRequestException ex)
        {
            stopwatch.Stop();
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.Error = ex.Message;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            result.ResponseTimeMs = stopwatch.ElapsedMilliseconds;
            result.Error = ex.Message;
        }
        
        return result;
    }
    
    private void PrintResult(CheckResult result)
    {
        if (result.IsUp)
        {
            Console.WriteLine($"✓ UP     {result.Url}");
            Console.WriteLine($"         Status: {result.StatusCode} | Response: {result.ResponseTimeMs}ms");
        }
        else if (!string.IsNullOrEmpty(result.Error))
        {
            Console.WriteLine($"✗ DOWN   {result.Url}");
            Console.WriteLine($"         Error: {result.Error}");
        }
        else
        {
            Console.WriteLine($"✗ DOWN   {result.Url}");
            Console.WriteLine($"         Status: {result.StatusCode} | Response: {result.ResponseTimeMs}ms");
        }
    }
    
    private void PrintSummary(List<CheckResult> results)
    {
        var up = results.Count(r => r.IsUp);
        var down = results.Count - up;
        var avgResponse = results.Where(r => r.IsUp).Average(r => r.ResponseTimeMs);
        
        Console.WriteLine($"\n--- Summary ---");
        Console.WriteLine($"Total: {results.Count}");
        Console.WriteLine($"Up: {up} ({(up / (double)results.Count) * 100:F1}%)");
        Console.WriteLine($"Down: {down} ({(down / (double)results.Count) * 100:F1}%)");
        Console.WriteLine($"Avg Response Time: {avgResponse:F0}ms");
    }
}

class CheckResult
{
    public string Url { get; set; } = "";
    public bool IsUp { get; set; }
    public System.Net.HttpStatusCode StatusCode { get; set; }
    public long ResponseTimeMs { get; set; }
    public string? Error { get; set; }
}
