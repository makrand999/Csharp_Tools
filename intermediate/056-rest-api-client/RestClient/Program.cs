using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Serialization;

var client = new HttpClient();
client.DefaultRequestHeaders.UserAgent.ParseAdd("CSharp-REST-Client");

Console.WriteLine("GitHub REST API Client");
Console.WriteLine("======================\n");

Console.Write("Enter GitHub username or organization: ");
var username = Console.ReadLine()?.Trim() ?? "";

if (string.IsNullOrEmpty(username))
{
    Console.WriteLine("Error: No username provided.");
    return;
}

Console.WriteLine("\nFetching user information...");
var userResponse = await client.GetAsync($"https://api.github.com/users/{username}");

if (!userResponse.IsSuccessStatusCode)
{
    Console.WriteLine($"Error: User '{username}' not found or API error.");
    return;
}

var userJson = await userResponse.Content.ReadAsStringAsync();
var user = JsonSerializer.Deserialize<GitHubUser>(userJson, new JsonSerializerOptions
{
    PropertyNameCaseInsensitive = true
});

if (user == null)
{
    Console.WriteLine("Error: Failed to parse user data.");
    return;
}

Console.WriteLine($"\n👤 {user.Name ?? user.Login}");
Console.WriteLine($"   Username: {user.Login}");
Console.WriteLine($"   Bio: {user.Bio ?? "N/A"}");
Console.WriteLine($"   Location: {user.Location ?? "N/A"}");
Console.WriteLine($"   Public Repos: {user.PublicRepos}");
Console.WriteLine($"   Followers: {user.Followers} | Following: {user.Following}");
Console.WriteLine($"   Profile: {user.HtmlUrl}");

Console.WriteLine($"\nFetching repositories for {username}...");
var reposResponse = await client.GetAsync($"{user.ReposUrl}?sort=updated&per_page=5");

if (reposResponse.IsSuccessStatusCode)
{
    var reposJson = await reposResponse.Content.ReadAsStringAsync();
    var repos = JsonSerializer.Deserialize<List<Repository>>(reposJson, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = true
    });

    if (repos != null && repos.Count > 0)
    {
        Console.WriteLine("\n📦 Recent Repositories:");
        foreach (var repo in repos)
        {
            var language = repo.Language ?? "N/A";
            var description = repo.Description ?? "No description";
            Console.WriteLine($"\n   {repo.Name}");
            Console.WriteLine($"      {description}");
            Console.WriteLine($"      Language: {language} | ⭐ {repo.StargazersCount} | 🍴 {repo.ForksCount}");
            Console.WriteLine($"      URL: {repo.HtmlUrl}");
        }
    }
}

Console.WriteLine("\n✅ Done!");

[JsonSerializable(typeof(GitHubUser))]
[JsonSerializable(typeof(Repository))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
partial class SourceGenerationContext : JsonSerializerContext { }

class GitHubUser
{
    public string Login { get; set; } = string.Empty;
    public string? Name { get; set; }
    public string? Bio { get; set; }
    public string? Location { get; set; }
    public int PublicRepos { get; set; }
    public int Followers { get; set; }
    public int Following { get; set; }
    public string HtmlUrl { get; set; } = string.Empty;
    public string ReposUrl { get; set; } = string.Empty;
}

class Repository
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Language { get; set; }
    public int StargazersCount { get; set; }
    public int ForksCount { get; set; }
    public string HtmlUrl { get; set; } = string.Empty;
}
