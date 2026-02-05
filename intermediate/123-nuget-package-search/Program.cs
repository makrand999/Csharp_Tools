using System.Text.Json;

namespace NuGetPackageSearch;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("NuGet Package Search Tool");
            Console.WriteLine("Usage: dotnet run --project 123-nuget-package-search.csproj -- <search-term> [--take N]");
            Console.WriteLine("Example: dotnet run --project 123-nuget-package-search.csproj -- json --take 10");
            return;
        }

        var searchTerm = args.Where(a => !a.StartsWith("--")).FirstOrDefault() ?? "";
        int take = 10;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--take" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out take);
            }
        }

        var httpClient = new HttpClient();
        var url = $"https://api-v2v3search-0.nuget.org/query?q={searchTerm}&take={take}&prerelease=false";

        try
        {
            Console.WriteLine($"Searching NuGet for: '{searchTerm}'");
            Console.WriteLine($"Fetching top {take} packages...\n");

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out var data))
            {
                Console.WriteLine("No results found.");
                return;
            }

            int rank = 1;
            foreach (var package in data.EnumerateArray())
            {
                var id = GetString(package, "id");
                var version = GetString(package, "version");
                var description = GetString(package, "description");
                var totalDownloads = GetLong(package, "totalDownloads");
                var projectUrl = GetString(package, "projectUrl");

                Console.WriteLine($"{rank}. {id} v{version}");
                Console.WriteLine($"   Downloads: {totalDownloads:N0}");
                if (!string.IsNullOrEmpty(description) && description != "N/A")
                {
                    var shortDesc = description.Length > 100 ? description[..100] + "..." : description;
                    Console.WriteLine($"   {shortDesc}");
                }
                if (!string.IsNullOrEmpty(projectUrl) && projectUrl != "N/A")
                {
                    Console.WriteLine($"   Project: {projectUrl}");
                }
                Console.WriteLine();
                rank++;
            }

            Console.WriteLine($"Total packages found: {rank - 1}");
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

    static int GetInt(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number
            ? prop.GetInt32()
            : 0;
    }

    static long GetLong(JsonElement element, string propertyName)
    {
        return element.TryGetProperty(propertyName, out var prop) && prop.ValueKind == JsonValueKind.Number
            ? prop.GetInt64()
            : 0;
    }
}
