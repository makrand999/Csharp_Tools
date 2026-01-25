using System.Net;
using System.Net.Sockets;
using System.Text.Json;
using System.Text.RegularExpressions;

if (args.Length == 0 || args.Contains("--help") || args.Contains("-h"))
{
    ShowUsage();
    Environment.Exit(args.Contains("--help") || args.Contains("-h") ? 0 : 1);
}

var domains = args.Where(a => !a.StartsWith("-")).ToList();
var showJson = args.Contains("--json") || args.Contains("-j");
var showWhois = args.Contains("--whois") || args.Contains("-w");
var showDns = args.Contains("--dns") || args.Contains("-d");
var showAll = args.Contains("--all") || args.Contains("-a");

if (showAll)
{
    showWhois = true;
    showDns = true;
}

if (domains.Count == 0)
{
    Console.Error.WriteLine("Error: No domains specified.");
    ShowUsage();
    Environment.Exit(1);
}

var results = new List<object>();

foreach (var domain in domains)
{
    var cleanDomain = CleanDomain(domain);
    
    if (!IsValidDomain(cleanDomain))
    {
        Console.Error.WriteLine($"Error: Invalid domain format: {domain}");
        continue;
    }
    
    var domainInfo = new
    {
        Domain = cleanDomain,
        Dns = showDns || showAll ? await LookupDns(cleanDomain) : null,
        Whois = showWhois || showAll ? await LookupWhois(cleanDomain) : null,
        IpAddresses = await ResolveIpAddresses(cleanDomain),
        IsValid = true
    };
    
    results.Add(domainInfo);
}

if (showJson)
{
    var options = new JsonSerializerOptions
    {
        WriteIndented = true,
        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };
    
    Console.WriteLine(JsonSerializer.Serialize(results.Count == 1 ? results[0] : results, options));
}
else
{
    foreach (var result in results)
    {
        PrintResult(result);
    }
}

static string CleanDomain(string domain)
{
    domain = domain.Trim();
    
    if (domain.StartsWith("http://", StringComparison.OrdinalIgnoreCase))
        domain = domain[7..];
    if (domain.StartsWith("https://", StringComparison.OrdinalIgnoreCase))
        domain = domain[8..];
    if (domain.StartsWith("www.", StringComparison.OrdinalIgnoreCase))
        domain = domain[4..];
    
    var pathIndex = domain.IndexOf('/');
    if (pathIndex > 0)
        domain = domain[..pathIndex];
    
    return domain.ToLowerInvariant();
}

static bool IsValidDomain(string domain)
{
    if (string.IsNullOrEmpty(domain)) return false;
    
    var pattern = @"^(?:[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?\.)+[a-zA-Z]{2,}$";
    return Regex.IsMatch(domain, pattern);
}

static async Task<Dictionary<string, object>> LookupDns(string domain)
{
    var dnsInfo = new Dictionary<string, object>();
    
    try
    {
        var aRecords = await Dns.GetHostAddressesAsync(domain);
        dnsInfo["A"] = aRecords
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .Select(ip => ip.ToString())
            .ToArray();
    }
    catch
    {
        dnsInfo["A"] = Array.Empty<string>();
    }
    
    return dnsInfo;
}

static async Task<string?> LookupWhois(string domain)
{
    try
    {
        using var client = new TcpClient();
        client.ReceiveTimeout = 5000;
        client.SendTimeout = 5000;
        
        await client.ConnectAsync("whois.iana.org", 43);
        
        using var stream = client.GetStream();
        var request = System.Text.Encoding.ASCII.GetBytes($"{domain}\r\n");
        await stream.WriteAsync(request);
        
        var response = new byte[4096];
        var bytesRead = await stream.ReadAsync(response);
        
        var whoisText = System.Text.Encoding.ASCII.GetString(response, 0, bytesRead);
        
        // Extract referral server if present
        var referralMatch = Regex.Match(whoisText, @"refer:\s*(whois\.[^\s]+)");
        if (referralMatch.Success)
        {
            var referralServer = referralMatch.Groups[1].Value.Trim();
            return await QueryReferralWhois(domain, referralServer);
        }
        
        return whoisText;
    }
    catch (Exception ex)
    {
        return $"Error: {ex.Message}";
    }
}

static async Task<string?> QueryReferralWhois(string domain, string whoisServer)
{
    try
    {
        using var client = new TcpClient();
        client.ReceiveTimeout = 5000;
        client.SendTimeout = 5000;
        
        await client.ConnectAsync(whoisServer, 43);
        
        using var stream = client.GetStream();
        var request = System.Text.Encoding.ASCII.GetBytes($"{domain}\r\n");
        await stream.WriteAsync(request);
        
        var response = new byte[8192];
        var bytesRead = await stream.ReadAsync(response);
        
        return System.Text.Encoding.ASCII.GetString(response, 0, bytesRead);
    }
    catch (Exception ex)
    {
        return $"Error querying {whoisServer}: {ex.Message}";
    }
}

static async Task<string[]> ResolveIpAddresses(string domain)
{
    try
    {
        var addresses = await Dns.GetHostAddressesAsync(domain);
        return addresses
            .Where(ip => ip.AddressFamily == AddressFamily.InterNetwork)
            .Select(ip => ip.ToString())
            .ToArray();
    }
    catch
    {
        return Array.Empty<string>();
    }
}

static void PrintResult(object result)
{
    var json = JsonSerializer.Serialize(result);
    using var doc = JsonDocument.Parse(json);
    var root = doc.RootElement;
    
    Console.WriteLine($"\n{'='*60}");
    Console.WriteLine($"Domain: {root.GetProperty("Domain").GetString()}");
    Console.WriteLine($"{'='*60}");
    
    if (root.TryGetProperty("IpAddresses", out var ips) && ips.GetArrayLength() > 0)
    {
        Console.WriteLine($"\nIP Addresses:");
        foreach (var ip in ips.EnumerateArray())
        {
            Console.WriteLine($"  - {ip.GetString()}");
        }
    }
    else
    {
        Console.WriteLine("\nIP Addresses: Not resolved");
    }
    
    if (root.TryGetProperty("Dns", out var dns) && dns.ValueKind != JsonValueKind.Null)
    {
        Console.WriteLine($"\nDNS Information:");
        if (dns.TryGetProperty("A", out var aRecords))
        {
            foreach (var record in aRecords.EnumerateArray())
            {
                Console.WriteLine($"  A: {record.GetString()}");
            }
        }
    }
    
    if (root.TryGetProperty("Whois", out var whois) && whois.ValueKind != JsonValueKind.Null)
    {
        Console.WriteLine($"\nWHOIS Information:");
        var whoisText = whois.GetString();
        if (!string.IsNullOrEmpty(whoisText) && !whoisText.StartsWith("Error:"))
        {
            // Parse and display key WHOIS fields
            var lines = whoisText.Split('\n');
            foreach (var line in lines.Take(20))
            {
                if (!string.IsNullOrWhiteSpace(line))
                {
                    Console.WriteLine($"  {line.Trim()}");
                }
            }
            if (lines.Length > 20)
            {
                Console.WriteLine($"  ... ({lines.Length - 20} more lines)");
            }
        }
        else
        {
            Console.WriteLine($"  {whoisText}");
        }
    }
}

static void ShowUsage()
{
    Console.WriteLine(@"Domain Lookup - DNS and WHOIS lookup tool

Usage:
  dotnet run --project DomainLookup.csproj [options] <domain1> [domain2] ...

Options:
  -j, --json      Output as JSON
  -w, --whois     Include WHOIS information
  -d, --dns       Include DNS records
  -a, --all       Include all information (DNS + WHOIS)
  -h, --help      Show this help message

Examples:
  dotnet run --project DomainLookup.csproj example.com
  dotnet run --project DomainLookup.csproj -j example.com
  dotnet run --project DomainLookup.csproj --dns example.com
  dotnet run --project DomainLookup.csproj --all example.com google.com
  dotnet run --project DomainLookup.csproj https://www.example.com/path");
}
