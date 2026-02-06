using System.Text.Json;

namespace WeatherApiClient;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Weather API Client (Open-Meteo - Free, no API key required)");
            Console.WriteLine("Usage: dotnet run --project 124-weather-api-client.csproj -- <city> <lat> <lon>");
            Console.WriteLine("Example: dotnet run --project 124-weather-api-client.csproj -- London 51.5074 -0.1278");
            Console.WriteLine("\nCommon coordinates:");
            Console.WriteLine("  London: 51.5074 -0.1278");
            Console.WriteLine("  New York: 40.7128 -74.0060");
            Console.WriteLine("  Tokyo: 35.6762 139.6503");
            Console.WriteLine("  Paris: 48.8566 2.3522");
            Console.WriteLine("  Berlin: 52.5200 13.4050");
            return;
        }

        string city = args[0];
        if (args.Length < 3 || !double.TryParse(args[1], out double latitude) || !double.TryParse(args[2], out double longitude))
        {
            Console.WriteLine("Error: Please provide valid coordinates (latitude longitude)");
            return;
        }

        var httpClient = new HttpClient();
        var url = $"https://api.open-meteo.com/v1/forecast?latitude={latitude}&longitude={longitude}&current_weather=true&hourly=temperature_2m,relativehumidity_2m,windspeed_10m&daily=temperature_2m_max,temperature_2m_min,precipitation_probability_max&timezone=auto";

        try
        {
            Console.WriteLine($"Fetching weather for: {city} ({latitude}, {longitude})");
            Console.WriteLine();

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            // Current weather
            if (root.TryGetProperty("current_weather", out var current))
            {
                Console.WriteLine("=== Current Weather ===");
                Console.WriteLine($"Temperature: {GetDouble(current, "temperature")}°C");
                Console.WriteLine($"Wind Speed: {GetDouble(current, "windspeed")} km/h");
                Console.WriteLine($"Wind Direction: {GetDouble(current, "winddirection")}°");
                Console.WriteLine($"Time: {GetString(current, "time")}");
                Console.WriteLine();
            }

            // Daily forecast
            if (root.TryGetProperty("daily", out var daily))
            {
                Console.WriteLine("=== 7-Day Forecast ===");
                var times = daily.GetProperty("time").EnumerateArray().ToList();
                var tempMax = daily.GetProperty("temperature_2m_max").EnumerateArray().ToList();
                var tempMin = daily.GetProperty("temperature_2m_min").EnumerateArray().ToList();
                var precipProb = daily.GetProperty("precipitation_probability_max").EnumerateArray().ToList();

                for (int i = 0; i < Math.Min(7, times.Count); i++)
                {
                    var date = times[i].GetString()?.Substring(0, 10) ?? "N/A";
                    var max = tempMax[i].GetSingle();
                    var min = tempMin[i].GetSingle();
                    var precip = precipProb[i].GetInt32();
                    Console.WriteLine($"{date}: {max}°C / {min}°C | Precip: {precip}%");
                }
                Console.WriteLine();
            }

            // Hourly forecast (next 24 hours)
            if (root.TryGetProperty("hourly", out var hourly))
            {
                Console.WriteLine("=== Next 24 Hours ===");
                var times = hourly.GetProperty("time").EnumerateArray().Take(24).ToList();
                var temps = hourly.GetProperty("temperature_2m").EnumerateArray().Take(24).ToList();
                var humidity = hourly.GetProperty("relativehumidity_2m").EnumerateArray().Take(24).ToList();
                var windSpeed = hourly.GetProperty("windspeed_10m").EnumerateArray().Take(24).ToList();

                Console.WriteLine($"{"Time",-12} {"Temp",-8} {"Humidity",-10} {"Wind",-10}");
                Console.WriteLine(new string('-', 42));

                for (int i = 0; i < times.Count; i++)
                {
                    var time = times[i].GetString()?.Substring(11, 5) ?? "N/A";
                    var temp = temps[i].GetSingle();
                    var hum = humidity[i].GetInt32();
                    var wind = windSpeed[i].GetSingle();
                    Console.WriteLine($"{time,-12} {temp,-8} {hum,-10} {wind,-10}");
                }
            }
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

    static double GetDouble(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number
            ? prop.GetDouble()
            : 0;
    }
}
