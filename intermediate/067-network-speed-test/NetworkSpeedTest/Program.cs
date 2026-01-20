using System.Diagnostics;
using System.Net.Http;

const int TestFileSizeBytes = 1024 * 1024; // 1MB test
var TestUrl = "https://speed.cloudflare.com/__down?bytes=" + TestFileSizeBytes;

Console.WriteLine("Network Speed Test");
Console.WriteLine(new string('-', 50));
Console.WriteLine($"Testing download speed from Cloudflare...");
Console.WriteLine(new string('-', 50));

try
{
    using var httpClient = new HttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(30);

    var sw = Stopwatch.StartNew();
    var response = await httpClient.GetAsync(TestUrl);
    response.EnsureSuccessStatusCode();
    
    await using var stream = await response.Content.ReadAsStreamAsync();
    var totalBytes = 0L;
    var buffer = new byte[8192];
    
    while (true)
    {
        var bytesRead = await stream.ReadAsync(buffer);
        if (bytesRead == 0) break;
        totalBytes += bytesRead;
    }
    
    sw.Stop();
    
    var elapsedSeconds = sw.Elapsed.TotalSeconds;
    var bitsPerSecond = (totalBytes * 8) / elapsedSeconds;
    var mbps = bitsPerSecond / (1024 * 1024);
    
    Console.WriteLine($"\nResults:");
    Console.WriteLine($"  Downloaded: {totalBytes:N0} bytes ({totalBytes / 1024.0 / 1024.0:F2} MB)");
    Console.WriteLine($"  Time: {elapsedSeconds:F2} seconds");
    Console.WriteLine($"  Speed: {mbps:F2} Mbps");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Error: Failed to connect - {ex.Message}");
    Console.WriteLine("Make sure you have an active internet connection.");
}
catch (TaskCanceledException)
{
    Console.WriteLine("Error: Request timed out after 30 seconds");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}
