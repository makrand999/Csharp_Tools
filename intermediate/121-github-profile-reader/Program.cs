using System.Text.Json;

namespace GitHubProfileReader;

class Program
{
    static async Task Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("GitHub Profile Reader");
            Console.WriteLine("Usage: dotnet run --project 121-github-profile-reader.csproj -- <username>");
            Console.WriteLine("Example: dotnet run --project 121-github-profile-reader.csproj -- octocat");
            return;
        }

        string username = args[0];
        var httpClient = new HttpClient();
        httpClient.DefaultRequestHeaders.Add("User-Agent", "GitHubProfileReader");
        httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github.v3+json");

        try
        {
            Console.WriteLine($"Fetching profile for: {username}");
            var response = await httpClient.GetAsync($"https://api.github.com/users/{username}");
            
            if (!response.IsSuccessStatusCode)
            {
                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    Console.WriteLine($"Error: User '{username}' not found.");
                }
                else
                {
                    Console.WriteLine($"Error: {response.StatusCode}");
                }
                return;
            }

            var json = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(json);
            var root = doc.RootElement;

            Console.WriteLine("\n=== GitHub Profile ===");
            Console.WriteLine($"Username: {GetString(root, "login")}");
            Console.WriteLine($"Name: {GetString(root, "name")}");
            Console.WriteLine($"Bio: {GetString(root, "bio")}");
            Console.WriteLine($"Location: {GetString(root, "location")}");
            Console.WriteLine($"Blog: {GetString(root, "blog")}");
            Console.WriteLine($"Twitter: {GetString(root, "twitter_username")}");
            Console.WriteLine($"Company: {GetString(root, "company")}");
            Console.WriteLine($"Public Repos: {GetInt(root, "public_repos")}");
            Console.WriteLine($"Followers: {GetInt(root, "followers")}");
            Console.WriteLine($"Following: {GetInt(root, "following")}");
            Console.WriteLine($"Public Gists: {GetInt(root, "public_gists")}");
            Console.WriteLine($"Created: {GetString(root, "created_at")}");
            Console.WriteLine($"Updated: {GetString(root, "updated_at")}");
            Console.WriteLine($"Profile URL: {GetString(root, "html_url")}");
            Console.WriteLine($"Avatar: {GetString(root, "avatar_url")}");
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
}
