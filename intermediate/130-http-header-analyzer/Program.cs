namespace HttpHeaderAnalyzer;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("HTTP Header Analyzer");
            Console.WriteLine("Usage: dotnet run --project 130-http-header-analyzer.csproj -- <url>");
            Console.WriteLine("Example: dotnet run --project 130-http-header-analyzer.csproj -- https://example.com");
            Console.WriteLine("         dotnet run --project 130-http-header-analyzer.csproj -- https://google.com --head");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --head    Use HEAD request instead of GET");
            Console.WriteLine("  --no-redirect   Don't follow redirects");
            return;
        }

        string url = args[0];
        bool useHead = args.Contains("--head");
        bool followRedirects = !args.Contains("--no-redirect");

        if (!url.StartsWith("http://") && !url.StartsWith("https://"))
        {
            url = "https://" + url;
        }

        var handler = new HttpClientHandler
        {
            AllowAutoRedirect = followRedirects,
            UseCookies = false
        };

        var httpClient = new HttpClient(handler);
        httpClient.DefaultRequestHeaders.Add("User-Agent", "HTTPHeaderAnalyzer/1.0");
        httpClient.Timeout = TimeSpan.FromSeconds(30);

        try
        {
            Console.WriteLine($"Analyzing headers for: {url}");
            Console.WriteLine($"Request type: {(useHead ? "HEAD" : "GET")}");
            Console.WriteLine($"Following redirects: {followRedirects}");
            Console.WriteLine();

            HttpResponseMessage response;
            if (useHead)
            {
                response = await httpClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, url));
            }
            else
            {
                response = await httpClient.GetAsync(url);
            }

            Console.WriteLine("=== Response Information ===");
            Console.WriteLine($"Status Code: {(int)response.StatusCode} {response.StatusCode}");
            Console.WriteLine($"Protocol Version: {response.Version}");
            Console.WriteLine($"Content Length: {response.Content.Headers.ContentLength?.ToString() ?? "Unknown"} bytes");
            Console.WriteLine($"Content Type: {response.Content.Headers.ContentType?.ToString() ?? "Unknown"}");
            Console.WriteLine();

            Console.WriteLine("=== Response Headers ===");
            var allHeaders = response.Headers.Union(response.Content.Headers);
            
            foreach (var header in allHeaders.OrderBy(h => h.Key))
            {
                var values = string.Join(", ", header.Value);
                Console.WriteLine($"{header.Key,-30} : {values}");
            }
            Console.WriteLine();

            // Security headers analysis
            Console.WriteLine("=== Security Headers Analysis ===");
            var securityHeaders = new Dictionary<string, string>
            {
                { "Strict-Transport-Security", "HSTS - Forces HTTPS connections" },
                { "Content-Security-Policy", "CSP - Prevents XSS attacks" },
                { "X-Content-Type-Options", "Prevents MIME type sniffing" },
                { "X-Frame-Options", "Prevents clickjacking attacks" },
                { "X-XSS-Protection", "Legacy XSS protection" },
                { "Referrer-Policy", "Controls referrer information" },
                { "Permissions-Policy", "Controls browser features" },
                { "Cross-Origin-Embedder-Policy", "COEP - Cross-origin isolation" },
                { "Cross-Origin-Opener-Policy", "COOP - Cross-origin isolation" },
                { "Cross-Origin-Resource-Policy", "CORP - Resource loading" }
            };

            var presentHeaders = new List<string>();
            var missingHeaders = new List<string>();

            foreach (var secHeader in securityHeaders)
            {
                if (allHeaders.Any(h => h.Key.Equals(secHeader.Key, StringComparison.OrdinalIgnoreCase)))
                {
                    presentHeaders.Add(secHeader.Key);
                }
                else
                {
                    missingHeaders.Add(secHeader.Key);
                }
            }

            Console.WriteLine($"Present ({presentHeaders.Count}):");
            foreach (var h in presentHeaders)
            {
                Console.WriteLine($"  ✓ {h} - {securityHeaders[h]}");
            }

            Console.WriteLine($"\nMissing ({missingHeaders.Count}):");
            foreach (var h in missingHeaders)
            {
                Console.WriteLine($"  ✗ {h} - {securityHeaders[h]}");
            }

            Console.WriteLine();
            Console.WriteLine($"Security Score: {presentHeaders.Count}/{securityHeaders.Count} headers present");

            // Server technology detection
            Console.WriteLine();
            Console.WriteLine("=== Technology Detection ===");

            var headersDict = allHeaders.ToDictionary(h => h.Key, h => h.Value, StringComparer.OrdinalIgnoreCase);

            if (headersDict.TryGetValue("Server", out var serverValues))
            {
                Console.WriteLine($"Server: {string.Join(", ", serverValues)}");
            }

            if (headersDict.TryGetValue("X-Powered-By", out var poweredByValues))
            {
                Console.WriteLine($"Powered By: {string.Join(", ", poweredByValues)}");
            }

            if (headersDict.ContainsKey("ASP.NET"))
            {
                Console.WriteLine("Framework: ASP.NET detected");
            }
            
            if (allHeaders.Any(h => h.Key.Equals("X-AspNet-Version", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("Framework: ASP.NET (via X-AspNet-Version)");
            }

            if (allHeaders.Any(h => h.Key.Equals("X-Drupal-Cache", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("CMS: Drupal detected");
            }

            if (allHeaders.Any(h => h.Key.Equals("X-WP-Engine", StringComparison.OrdinalIgnoreCase) || 
                                    h.Key.Equals("X-Pass-Why", StringComparison.OrdinalIgnoreCase)))
            {
                Console.WriteLine("CMS: WordPress (WP Engine) detected");
            }
        }
        catch (TaskCanceledException)
        {
            Console.WriteLine("Error: Request timed out");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}
