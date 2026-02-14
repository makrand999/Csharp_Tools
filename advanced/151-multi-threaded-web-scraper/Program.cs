using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MultiThreadedWebScraper;

/// <summary>
/// Multi-threaded web scraper that crawls URLs and extracts links in parallel.
/// Uses ConcurrentBag for thread-safe collections and async/await for I/O operations.
/// </summary>
class Program
{
    private static readonly HttpClient HttpClient = new();
    private static readonly ConcurrentBag<string> VisitedUrls = new();
    private static readonly ConcurrentBag<string> FoundUrls = new();
    private static readonly ConcurrentBag<(string Url, string Title)> PageTitles = new();
    private static int _maxDepth = 2;
    private static int _maxPages = 10;
    private static int _threads = 4;
    private static string _baseUrl = string.Empty;

    static async Task<int> Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Multi-threaded Web Scraper");
            Console.WriteLine("==========================");
            Console.WriteLine("Usage: dotnet run --project MultiThreadedWebScraper.csproj <url> [options]");
            Console.WriteLine();
            Console.WriteLine("Options:");
            Console.WriteLine("  --depth <n>       Max crawl depth (default: 2)");
            Console.WriteLine("  --max-pages <n>   Max pages to scrape (default: 10)");
            Console.WriteLine("  --threads <n>     Number of parallel threads (default: 4)");
            Console.WriteLine("  --extract-links   Extract and display all links");
            Console.WriteLine("  --extract-text    Extract visible text content");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project MultiThreadedWebScraper.csproj https://example.com");
            Console.WriteLine("  dotnet run --project MultiThreadedWebScraper.csproj https://example.com --depth 3 --threads 8");
            return 0;
        }

        _baseUrl = args[0].TrimEnd('/');
        
        for (int i = 1; i < args.Length; i++)
        {
            switch (args[i])
            {
                case "--depth" when i + 1 < args.Length:
                    _maxDepth = int.Parse(args[++i]);
                    break;
                case "--max-pages" when i + 1 < args.Length:
                    _maxPages = int.Parse(args[++i]);
                    break;
                case "--threads" when i + 1 < args.Length:
                    _threads = int.Parse(args[++i]);
                    break;
            }
        }

        var extractLinks = args.Contains("--extract-links");
        var extractText = args.Contains("--extract-text");

        Console.WriteLine($"Starting multi-threaded web scraper");
        Console.WriteLine($"Base URL: {_baseUrl}");
        Console.WriteLine($"Max depth: {_maxDepth}, Max pages: {_maxPages}, Threads: {_threads}");
        Console.WriteLine();

        try
        {
            await CrawlAsync(_baseUrl, 0);

            Console.WriteLine($"\nCrawl complete!");
            Console.WriteLine($"Pages visited: {VisitedUrls.Count}");
            Console.WriteLine($"Total links found: {FoundUrls.Count}");

            if (PageTitles.Count > 0)
            {
                Console.WriteLine("\nPage Titles:");
                Console.WriteLine("------------");
                foreach (var title in PageTitles.Take(20))
                {
                    Console.WriteLine($"  {title.Title}");
                    Console.WriteLine($"    -> {title.Url}");
                }
            }

            if (extractLinks && FoundUrls.Count > 0)
            {
                Console.WriteLine($"\nAll Extracted Links ({FoundUrls.Count}):");
                Console.WriteLine("----------------------------------------");
                foreach (var url in FoundUrls.OrderBy(u => u).Take(50))
                {
                    Console.WriteLine($"  {url}");
                }
                if (FoundUrls.Count > 50)
                    Console.WriteLine($"  ... and {FoundUrls.Count - 50} more");
            }

            return 0;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static async Task CrawlAsync(string url, int depth)
    {
        if (depth > _maxDepth || VisitedUrls.Count >= _maxPages)
            return;

        if (VisitedUrls.Contains(url))
            return;

        VisitedUrls.Add(url);
        Console.WriteLine($"[{depth}] Crawling: {url}");

        try
        {
            var html = await HttpClient.GetStringAsync(url);
            
            // Extract title
            var titleMatch = Regex.Match(html, "<title[^>]*>(.*?)</title>", RegexOptions.IgnoreCase);
            var title = titleMatch.Success ? Regex.Replace(titleMatch.Groups[1].Value, "<.*?>", string.Empty).Trim() : "No title";
            PageTitles.Add((url, title));

            // Extract links
            var links = ExtractLinks(html, url);
            foreach (var link in links)
            {
                FoundUrls.Add(link);
            }

            // Crawl found links in parallel
            var linksToCrawl = links
                .Where(l => l.StartsWith(_baseUrl) && !VisitedUrls.Contains(l))
                .Take(_threads)
                .ToList();

            if (linksToCrawl.Count > 0 && depth < _maxDepth)
            {
                var tasks = linksToCrawl.Select(link => CrawlAsync(link, depth + 1));
                await Task.WhenAll(tasks);
            }
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"  HTTP Error: {ex.StatusCode} for {url}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }
    }

    static List<string> ExtractLinks(string html, string baseUrl)
    {
        var links = new List<string>();
        var pattern = @"href=[""'](?<url>http[^""']+)[\""']";
        
        var matches = Regex.Matches(html, pattern, RegexOptions.IgnoreCase);
        foreach (Match match in matches)
        {
            var url = match.Groups["url"].Value.Split('#')[0].Split('?')[0];
            if (!links.Contains(url) && IsValidUrl(url))
            {
                links.Add(url);
            }
        }

        return links;
    }

    static bool IsValidUrl(string url)
    {
        try
        {
            var uri = new Uri(url);
            return uri.Scheme == Uri.UriSchemeHttp || uri.Scheme == Uri.UriSchemeHttps;
        }
        catch
        {
            return false;
        }
    }
}
