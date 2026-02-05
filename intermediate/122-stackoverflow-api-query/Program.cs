using System.Text.Json;
using System.Web;

namespace StackOverflowApiQuery;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("Stack Overflow API Query Tool");
            Console.WriteLine("Usage: dotnet run --project 122-stackoverflow-api-query.csproj -- [search] [--top N]");
            Console.WriteLine("Example: dotnet run --project 122-stackoverflow-api-query.csproj -- c# async --top 5");
            return;
        }

        var searchQuery = string.Join(" ", args.Where(a => !a.StartsWith("--")));
        int topN = 10;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--top" && i + 1 < args.Length)
            {
                int.TryParse(args[i + 1], out topN);
            }
        }

        var httpClient = new HttpClient();
        var baseUrl = "https://api.stackexchange.com/2.3/search";
        var queryParams = new Dictionary<string, string>
        {
            { "site", "stackoverflow" },
            { "order", "desc" },
            { "sort", "relevance" },
            { "intitle", searchQuery },
            { "pagesize", topN.ToString() }
        };

        var queryString = string.Join("&", queryParams.Select(kvp => $"{HttpUtility.UrlEncode(kvp.Key)}={HttpUtility.UrlEncode(kvp.Value)}"));
        var url = $"{baseUrl}?{queryString}";

        try
        {
            Console.WriteLine($"Searching Stack Overflow for: '{searchQuery}'");
            Console.WriteLine($"Fetching top {topN} results...\n");

            var response = await httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            if (!root.TryGetProperty("items", out var items))
            {
                Console.WriteLine("No results found.");
                return;
            }

            int rank = 1;
            foreach (var item in items.EnumerateArray())
            {
                var title = GetString(item, "title");
                var link = GetString(item, "link");
                var score = GetInt(item, "score");
                var answerCount = GetInt(item, "answer_count");
                var viewCount = GetInt(item, "view_count");
                var tags = GetTags(item);

                Console.WriteLine($"{rank}. {title}");
                Console.WriteLine($"   Score: {score} | Answers: {answerCount} | Views: {viewCount}");
                Console.WriteLine($"   Tags: {string.Join(", ", tags)}");
                Console.WriteLine($"   Link: {link}");
                Console.WriteLine();
                rank++;
            }

            Console.WriteLine($"Total results found: {rank - 1}");
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

    static List<string> GetTags(JsonElement element)
    {
        var tags = new List<string>();
        if (element.TryGetProperty("tags", out var tagsProp) && tagsProp.ValueKind == JsonValueKind.Array)
        {
            foreach (var tag in tagsProp.EnumerateArray())
            {
                if (tag.ValueKind == JsonValueKind.String)
                {
                    tags.Add(tag.GetString() ?? "");
                }
            }
        }
        return tags;
    }
}
