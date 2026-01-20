using System.Xml.Linq;

if (args.Length == 0)
{
    Console.WriteLine("Podcast Downloader - Download episodes from podcast RSS feeds");
    Console.WriteLine("\nUsage: dotnet run --project PodcastDownloader.csproj <rss-url> [output-dir] [max-episodes]");
    Console.WriteLine("\nExamples:");
    Console.WriteLine("  dotnet run --project PodcastDownloader.csproj https://feeds.example.com/podcast.xml");
    Console.WriteLine("  dotnet run --project PodcastDownloader.csproj https://feeds.example.com/podcast.xml ./podcasts 5");
    return;
}

var url = args[0];
var outputDir = args.Length > 1 ? args[1] : "./downloads";
var maxEpisodes = args.Length > 2 ? int.Parse(args[2]) : 5;

if (!Directory.Exists(outputDir))
{
    Directory.CreateDirectory(outputDir);
}

Console.WriteLine($"Fetching podcast feed: {url}");
Console.WriteLine($"Output directory: {Path.GetFullPath(outputDir)}");
Console.WriteLine($"Max episodes: {maxEpisodes}");
Console.WriteLine(new string('-', 60));

try
{
    using var httpClient = new HttpClient();
    httpClient.Timeout = TimeSpan.FromSeconds(30);
    var xmlContent = await httpClient.GetStringAsync(url);
    
    var doc = XDocument.Parse(xmlContent);
    var channel = doc.Element("rss")?.Element("channel");
    
    if (channel == null)
    {
        Console.WriteLine("Error: Invalid RSS feed format");
        return;
    }
    
    var podcastTitle = channel.Element("title")?.Value ?? "Podcast";
    Console.WriteLine($"\n🎙️ {podcastTitle}\n");
    
    var items = channel.Elements("item").Take(maxEpisodes);
    var downloaded = 0;
    
    foreach (var item in items)
    {
        var title = item.Element("title")?.Value ?? "Untitled";
        var enclosure = item.Element("enclosure")?.Attribute("url")?.Value;
        
        if (string.IsNullOrEmpty(enclosure))
        {
            Console.WriteLine($"⊘ {title} (no audio URL)");
            continue;
        }
        
        // Extract filename from URL
        var filename = Path.GetFileName(new Uri(enclosure).AbsolutePath);
        if (string.IsNullOrEmpty(filename))
        {
            filename = $"episode_{downloaded + 1}.mp3";
        }
        
        var filepath = Path.Combine(outputDir, SanitizeFilename(filename));
        
        if (File.Exists(filepath))
        {
            Console.WriteLine($"✓ {title} (already exists)");
            continue;
        }
        
        Console.WriteLine($"↓ Downloading: {title}...");
        
        try
        {
            var episodeData = await httpClient.GetByteArrayAsync(enclosure);
            await File.WriteAllBytesAsync(filepath, episodeData);
            
            var sizeKb = episodeData.Length / 1024.0;
            Console.WriteLine($"  Saved: {filepath} ({sizeKb:F0} KB)");
            downloaded++;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"  Error: {ex.Message}");
        }
    }
    
    Console.WriteLine($"\nDownloaded {downloaded} episode(s) to {outputDir}");
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
}

static string SanitizeFilename(string filename)
{
    foreach (var c in Path.GetInvalidFileNameChars())
    {
        filename = filename.Replace(c, '_');
    }
    return filename;
}
