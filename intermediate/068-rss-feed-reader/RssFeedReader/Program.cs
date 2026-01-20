using System.Xml.Linq;

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run --project RssFeedReader.csproj <rss-url>");
    Console.WriteLine("\nExample:");
    Console.WriteLine("  dotnet run --project RssFeedReader.csproj https://rss.nytimes.com/services/xml/rss/nyt/HomePage.xml");
    return;
}

var url = args[0];
Console.WriteLine($"Fetching RSS feed: {url}");
Console.WriteLine(new string('-', 60));

try
{
    using var httpClient = new HttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(10);
    var xmlContent = await httpClient.GetStringAsync(url);
    
    var doc = XDocument.Parse(xmlContent);
    var channel = doc.Element("rss")?.Element("channel") 
               ?? doc.Element("feed"); // Atom support
    
    if (channel == null)
    {
        Console.WriteLine("Error: Invalid RSS/Atom feed format");
        return;
    }
    
    // Get feed title
    var feedTitle = channel.Element("title")?.Value ?? "Unknown Feed";
    Console.WriteLine($"\n📰 {feedTitle}\n");
    
    // Get description for RSS or subtitle for Atom
    var description = channel.Element("description")?.Value 
                   ?? channel.Element("subtitle")?.Value;
    if (!string.IsNullOrEmpty(description))
    {
        Console.WriteLine($"{description}\n");
    }
    
    // Get items (item for RSS, entry for Atom)
    var items = channel.Elements("item").Concat(channel.Elements("{http://www.w3.org/2005/Atom}entry")).Take(10);
    
    var index = 1;
    foreach (var item in items)
    {
        var title = item.Element("title")?.Value ?? "No Title";
        var link = item.Element("link")?.Value ?? item.Element("link")?.Attribute("href")?.Value ?? "No Link";
        var pubDate = item.Element("pubDate")?.Value ?? item.Element("published")?.Value ?? "No Date";
        var descriptionText = item.Element("description")?.Value 
                           ?? item.Element("summary")?.Value 
                           ?? item.Element("{http://www.w3.org/2005/Atom}summary")?.Value;
        
        Console.WriteLine($"{index}. {title}");
        Console.WriteLine($"   Link: {link}");
        Console.WriteLine($"   Date: {pubDate}");
        if (!string.IsNullOrEmpty(descriptionText))
        {
            var truncated = descriptionText.Length > 150 
                ? descriptionText[..147] + "..." 
                : descriptionText;
            Console.WriteLine($"   Summary: {truncated}");
        }
        Console.WriteLine();
        index++;
    }
    
    Console.WriteLine($"Showing {index - 1} items from feed");
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Error fetching feed: {ex.Message}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error parsing feed: {ex.Message}");
}
