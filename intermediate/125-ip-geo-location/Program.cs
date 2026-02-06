using System.Text.Json;

namespace IpGeoLocation;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("IP Geolocation Lookup Tool");
            Console.WriteLine("Usage: dotnet run --project 125-ip-geo-location.csproj -- [ip-address]");
            Console.WriteLine("Example: dotnet run --project 125-ip-geo-location.csproj -- 8.8.8.8");
            Console.WriteLine("         dotnet run --project 125-ip-geo-location.csproj (lookup your IP)");
            return;
        }

        string ip = args[0];
        var httpClient = new HttpClient();
        var url = $"https://ipapi.co/{ip}/json/";

        try
        {
            Console.WriteLine($"Looking up IP: {ip}");
            Console.WriteLine();

            var response = await httpClient.GetAsync(url);
            
            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"Error: Could not lookup IP. Status: {response.StatusCode}");
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Check for error
            if (root.TryGetProperty("error", out var error) && error.GetBoolean())
            {
                var reason = GetString(root, "reason");
                Console.WriteLine($"Error: {reason}");
                return;
            }

            Console.WriteLine("=== IP Geolocation ===");
            Console.WriteLine($"IP Address: {GetString(root, "ip")}");
            Console.WriteLine($"City: {GetString(root, "city")}");
            Console.WriteLine($"Region: {GetString(root, "region")}");
            Console.WriteLine($"Country: {GetString(root, "country_name")} ({GetString(root, "country_code")})");
            Console.WriteLine($"Postal Code: {GetString(root, "postal")}");
            Console.WriteLine($"Latitude: {GetString(root, "latitude")}");
            Console.WriteLine($"Longitude: {GetString(root, "longitude")}");
            Console.WriteLine($"Timezone: {GetString(root, "timezone")}");
            Console.WriteLine($"UTC Offset: {GetString(root, "utc_offset")}");
            Console.WriteLine($"Country Calling Code: {GetString(root, "country_calling_code")}");
            Console.WriteLine($"Currency: {GetString(root, "currency")}");
            Console.WriteLine($"Languages: {GetString(root, "languages")}");
            Console.WriteLine($"ASN: {GetString(root, "asn")}");
            Console.WriteLine($"Organization: {GetString(root, "org")}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static string GetString(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.String
            ? prop.GetString() ?? "N/A"
            : "N/A";
    }
}
