using System.Net.Http.Json;
using System.Text.Json;

namespace RedditBrowser;

class Program
{
    private static readonly HttpClient HttpClient = new();
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    static async Task Main(string[] args)
    {
        Console.WriteLine("🔴 Reddit CLI Browser");
        Console.WriteLine("=====================\n");

        if (args.Length == 0)
        {
            Console.WriteLine("Usage: dotnet run --project RedditBrowser.csproj [subreddit]");
            Console.WriteLine("Example: dotnet run --project RedditBrowser.csproj dotnet");
            return;
        }

        string subreddit = args[0];
        string url = $"https://www.reddit.com/r/{subreddit}/hot.json?limit=10";

        HttpClient.DefaultRequestHeaders.UserAgent.ParseAdd("RedditBrowser/1.0");

        try
        {
            Console.WriteLine($"Fetching hot posts from r/{subreddit}...\n");

            var response = await HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            var jsonData = await response.Content.ReadFromJsonAsync<JsonElement>();
            var posts = jsonData.GetProperty("data").GetProperty("children");

            int postNum = 1;
            foreach (var postElement in posts.EnumerateArray())
            {
                var data = postElement.GetProperty("data");
                string title = data.GetProperty("title").GetString() ?? "No title";
                string author = data.GetProperty("author").GetString() ?? "Unknown";
                int score = data.GetProperty("score").GetInt32();
                int numComments = data.GetProperty("num_comments").GetInt32();
                string permalink = data.GetProperty("permalink").GetString() ?? "";
                bool isSelf = data.GetProperty("is_self").GetBoolean();
                string postType = isSelf ? "📝" : "🔗";

                Console.WriteLine($"{postNum}. {postType} {Truncate(title, 60)}");
                Console.WriteLine($"   by u/{author} | ⬆ {score:N0} | 💬 {numComments}");
                Console.WriteLine($"   https://reddit.com{permalink}");
                Console.WriteLine();

                postNum++;
            }

            Console.WriteLine($"Showing top {Math.Min(postNum - 1, 10)} posts from r/{subreddit}");
        }
        catch (HttpRequestException ex)
        {
            Console.WriteLine($"❌ Error fetching posts: {ex.Message}");
            Console.WriteLine("Make sure the subreddit exists and you have internet connection.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Error: {ex.Message}");
        }
    }

    private static string Truncate(string value, int maxLength)
    {
        if (string.IsNullOrEmpty(value)) return value;
        return value.Length <= maxLength ? value : value[..(maxLength - 3)] + "...";
    }
}
