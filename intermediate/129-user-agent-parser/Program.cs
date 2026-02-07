using System.Text.RegularExpressions;

namespace UserAgentParser;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("User Agent Parser");
            Console.WriteLine("Usage: dotnet run --project 129-user-agent-parser.csproj -- \"<user-agent-string>\"");
            Console.WriteLine("Example: dotnet run --project 129-user-agent-parser.csproj -- \"Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36\"");
            Console.WriteLine("\nOr pipe a user agent string:");
            Console.WriteLine("  echo \"Mozilla/5.0...\" | dotnet run --project 129-user-agent-parser.csproj");
            return;
        }

        string userAgent = string.Join(" ", args);

        // Try to read from stdin if args suggest piping
        if (args.Length == 1 && args[0] == "-")
        {
            userAgent = Console.In.ReadLine() ?? "";
        }

        var result = ParseUserAgent(userAgent);

        Console.WriteLine("=== User Agent Analysis ===");
        Console.WriteLine();
        Console.WriteLine($"Original: {userAgent}");
        Console.WriteLine();
        Console.WriteLine("=== Parsed Information ===");
        Console.WriteLine($"Browser:    {result.Browser} {result.BrowserVersion}");
        Console.WriteLine($"OS:         {result.OS} {result.OSVersion}");
        Console.WriteLine($"Device:     {result.DeviceType}");
        Console.WriteLine($"Platform:   {result.Platform}");
        Console.WriteLine($"Is Mobile:  {result.IsMobile}");
        Console.WriteLine($"Is Bot:     {result.IsBot}");
        Console.WriteLine();

        if (result.IsBot)
        {
            Console.WriteLine("=== Bot Information ===");
            Console.WriteLine($"Bot Name: {result.BotName}");
            Console.WriteLine();
        }
    }

    static UserAgentResult ParseUserAgent(string ua)
    {
        var result = new UserAgentResult();
        result.Original = ua;

        // Detect bots
        var botPatterns = new Dictionary<string, string>
        {
            { "Googlebot", "googlebot" },
            { "Bingbot", "bingbot" },
            { "Slurp", "yahoo" },
            { "DuckDuckBot", "duckduckbot" },
            { "Baiduspider", "baidu" },
            { "YandexBot", "yandex" },
            { "facebot", "facebook" },
            { "ia_archiver", "alexa" },
            { "Twitterbot", "twitter" },
            { "LinkedInBot", "linkedin" },
            { "AhrefsBot", "ahrefs" },
            { "MJ12bot", "majestic" },
            { "SemrushBot", "semrush" },
            { "DotBot", "dotbot" }
        };

        foreach (var bot in botPatterns)
        {
            if (ua.Contains(bot.Value, StringComparison.OrdinalIgnoreCase))
            {
                result.IsBot = true;
                result.BotName = bot.Key;
                break;
            }
        }

        // Detect device type
        if (ua.Contains("Mobile", StringComparison.OrdinalIgnoreCase) || 
            ua.Contains("Android", StringComparison.OrdinalIgnoreCase) ||
            ua.Contains("iPhone", StringComparison.OrdinalIgnoreCase) ||
            ua.Contains("iPad", StringComparison.OrdinalIgnoreCase))
        {
            result.IsMobile = true;
            result.DeviceType = "Mobile";
            
            if (ua.Contains("iPad", StringComparison.OrdinalIgnoreCase) || 
                ua.Contains("Tablet", StringComparison.OrdinalIgnoreCase))
            {
                result.DeviceType = "Tablet";
            }
        }
        else if (ua.Contains("Smart-TV", StringComparison.OrdinalIgnoreCase) ||
                 ua.Contains("CrKey", StringComparison.OrdinalIgnoreCase))
        {
            result.DeviceType = "TV";
        }
        else if (ua.Contains("Watch", StringComparison.OrdinalIgnoreCase) ||
                 ua.Contains("Wearable", StringComparison.OrdinalIgnoreCase))
        {
            result.DeviceType = "Wearable";
        }
        else
        {
            result.DeviceType = "Desktop";
        }

        // Detect OS
        if (ua.Contains("Windows NT 10", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "Windows";
            result.OSVersion = "10";
            result.Platform = "Windows";
        }
        else if (ua.Contains("Windows NT 6.3", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "Windows";
            result.OSVersion = "8.1";
            result.Platform = "Windows";
        }
        else if (ua.Contains("Windows NT 6.2", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "Windows";
            result.OSVersion = "8";
            result.Platform = "Windows";
        }
        else if (ua.Contains("Windows NT 6.1", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "Windows";
            result.OSVersion = "7";
            result.Platform = "Windows";
        }
        else if (ua.Contains("Mac OS X", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "macOS";
            var match = Regex.Match(ua, @"Mac OS X (\d+[._]\d+[._]?\d*)");
            if (match.Success)
            {
                result.OSVersion = match.Groups[1].Value.Replace('_', '.');
            }
            result.Platform = "Mac";
        }
        else if (ua.Contains("Android", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "Android";
            var match = Regex.Match(ua, @"Android (\d+[._]?\d*)");
            if (match.Success)
            {
                result.OSVersion = match.Groups[1].Value.Replace('_', '.');
            }
            result.Platform = "Android";
        }
        else if (ua.Contains("iPhone OS", StringComparison.OrdinalIgnoreCase) || ua.Contains("iOS", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "iOS";
            var match = Regex.Match(ua, @"(?:iPhone OS|iOS) (\d+[._]?\d*)");
            if (match.Success)
            {
                result.OSVersion = match.Groups[1].Value.Replace('_', '.');
            }
            result.Platform = "iOS";
        }
        else if (ua.Contains("Linux", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "Linux";
            result.Platform = "Linux";
            if (ua.Contains("Ubuntu", StringComparison.OrdinalIgnoreCase))
            {
                result.OS = "Ubuntu";
            }
            else if (ua.Contains("Fedora", StringComparison.OrdinalIgnoreCase))
            {
                result.OS = "Fedora";
            }
            else if (ua.Contains("Debian", StringComparison.OrdinalIgnoreCase))
            {
                result.OS = "Debian";
            }
        }
        else if (ua.Contains("CrOS", StringComparison.OrdinalIgnoreCase))
        {
            result.OS = "Chrome OS";
            result.Platform = "Chrome OS";
        }

        // Detect browser
        // Edge (new Chromium-based)
        var edgeMatch = Regex.Match(ua, @"Edg/(\d+\.\d+\.\d+)");
        if (edgeMatch.Success)
        {
            result.Browser = "Edge";
            result.BrowserVersion = edgeMatch.Groups[1].Value;
        }
        // Edge (old)
        else if (ua.Contains("Edge/", StringComparison.OrdinalIgnoreCase))
        {
            result.Browser = "Edge (Legacy)";
            var match = Regex.Match(ua, @"Edge/(\d+\.\d+)");
            result.BrowserVersion = match.Success ? match.Groups[1].Value : "";
        }
        // Chrome
        else if (ua.Contains("Chrome/", StringComparison.OrdinalIgnoreCase) && !ua.Contains("Edg/", StringComparison.OrdinalIgnoreCase))
        {
            result.Browser = "Chrome";
            var match = Regex.Match(ua, @"Chrome/(\d+\.\d+\.\d+)");
            result.BrowserVersion = match.Success ? match.Groups[1].Value : "";
        }
        // Firefox
        else if (ua.Contains("Firefox/", StringComparison.OrdinalIgnoreCase))
        {
            result.Browser = "Firefox";
            var match = Regex.Match(ua, @"Firefox/(\d+\.\d+)");
            result.BrowserVersion = match.Success ? match.Groups[1].Value : "";
        }
        // Safari
        else if (ua.Contains("Safari/", StringComparison.OrdinalIgnoreCase) && !ua.Contains("Chrome/", StringComparison.OrdinalIgnoreCase))
        {
            result.Browser = "Safari";
            var match = Regex.Match(ua, @"Version/(\d+\.\d+)");
            result.BrowserVersion = match.Success ? match.Groups[1].Value : "";
        }
        // Opera
        else if (ua.Contains("OPR/", StringComparison.OrdinalIgnoreCase))
        {
            result.Browser = "Opera";
            var match = Regex.Match(ua, @"OPR/(\d+\.\d+)");
            result.BrowserVersion = match.Success ? match.Groups[1].Value : "";
        }
        // IE
        else if (ua.Contains("MSIE", StringComparison.OrdinalIgnoreCase) || ua.Contains("Trident/", StringComparison.OrdinalIgnoreCase))
        {
            result.Browser = "Internet Explorer";
            var match = Regex.Match(ua, @"(?:MSIE |rv:)(\d+\.\d+)");
            result.BrowserVersion = match.Success ? match.Groups[1].Value : "";
        }

        return result;
    }

    class UserAgentResult
    {
        public string Original { get; set; } = "";
        public string Browser { get; set; } = "Unknown";
        public string BrowserVersion { get; set; } = "";
        public string OS { get; set; } = "Unknown";
        public string OSVersion { get; set; } = "";
        public string Platform { get; set; } = "Unknown";
        public string DeviceType { get; set; } = "Desktop";
        public bool IsMobile { get; set; }
        public bool IsBot { get; set; }
        public string BotName { get; set; } = "";
    }
}
