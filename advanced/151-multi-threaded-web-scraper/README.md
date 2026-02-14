# Multi-threaded Web Scraper

A concurrent web crawler that scrapes multiple pages in parallel using async/await and thread-safe collections.

## Usage

```bash
dotnet run --project MultiThreadedWebScraper.csproj <url> [options]
```

### Options

| Option | Description | Default |
|--------|-------------|---------|
| `--depth <n>` | Max crawl depth | 2 |
| `--max-pages <n>` | Max pages to scrape | 10 |
| `--threads <n>` | Number of parallel threads | 4 |
| `--extract-links` | Display all extracted links | false |
| `--extract-text` | Extract visible text content | false |

## Examples

```bash
# Basic usage
dotnet run --project MultiThreadedWebScraper.csproj https://example.com

# Deep crawl with more threads
dotnet run --project MultiThreadedWebScraper.csproj https://example.com --depth 3 --threads 8

# Extract all links
dotnet run --project MultiThreadedWebScraper.csproj https://example.com --extract-links
```

## Example Output

```
Starting multi-threaded web scraper
Base URL: https://example.com
Max depth: 2, Max pages: 10, Threads: 4

[0] Crawling: https://example.com
[1] Crawling: https://example.com/about
[1] Crawling: https://example.com/contact
[2] Crawling: https://example.com/about/team

Crawl complete!
Pages visited: 10
Total links found: 45

Page Titles:
------------
  Example Domain
    -> https://example.com
  About Us
    -> https://example.com/about
```

## Concepts Demonstrated

- **Async/Await** - Non-blocking HTTP requests
- **ConcurrentBag<T>** - Thread-safe collection for parallel operations
- **Task.WhenAll** - Parallel task execution
- **HttpClient** - HTTP client for web requests
- **Regex** - HTML parsing for links and titles
- **Recursion** - Depth-limited crawling
- **URI validation** - URL parsing and validation
