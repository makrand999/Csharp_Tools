# Reddit CLI Browser

A command-line Reddit browser that displays hot posts from any subreddit.

## Usage

```bash
dotnet run --project RedditBrowser.csproj [subreddit]
```

## Example

```
$ dotnet run --project RedditBrowser.csproj dotnet

🔴 Reddit CLI Browser
=====================

Fetching hot posts from r/dotnet...

1. 📝 .NET 9 Release Notes
   by u/dotnet_dev | ⬆ 1,234 | 💬 89
   https://reddit.com/r/dotnet/abc123

2. 🔗 Best C# libraries you should know about
   by u/csharp_fan | ⬆ 567 | 💬 45
   https://reddit.com/r/dotnet/def456
```

## Concepts Demonstrated

- HTTP client with async/await
- JSON deserialization with System.Text.Json
- Reddit API consumption
- Console output formatting
- Error handling for network requests
