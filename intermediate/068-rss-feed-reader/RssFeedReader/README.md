# RSS Feed Reader

A command-line RSS/Atom feed reader that fetches and displays feed items from any RSS URL.

## Usage

```bash
dotnet run --project RssFeedReader.csproj <rss-url>
```

## Examples

```bash
# Read New York Times feed
dotnet run --project RssFeedReader.csproj https://rss.nytimes.com/services/xml/rss/nyt/HomePage.xml

# Read BBC News
dotnet run --project RssFeedReader.csproj http://feeds.bbci.co.uk/news/rss.xml

# Read Hacker News
dotnet run --project RssFeedReader.csproj https://hnrss.org/frontpage
```

## Example Output

```
Fetching RSS feed: https://rss.nytimes.com/services/xml/rss/nyt/HomePage.xml
------------------------------------------------------------

📰 The New York Times

Breaking news, investigations, politics, sports, and more.

1. Breaking: Major Policy Announcement Expected
   Link: https://www.nytimes.com/2024/01/15/example.html
   Date: Mon, 15 Jan 2024 10:30:00 GMT
   Summary: Government officials are expected to announce...

2. Tech Giants Report Quarterly Earnings
   Link: https://www.nytimes.com/2024/01/15/business/example.html
   Date: Mon, 15 Jan 2024 09:15:00 GMT
   Summary: Major technology companies exceeded...

Showing 10 items from feed
```

## Concepts Demonstrated

- XML parsing with XDocument
- RSS 2.0 and Atom feed formats
- HTTP client for fetching remote content
- LINQ to XML queries
- String manipulation and truncation
- Error handling for network operations
