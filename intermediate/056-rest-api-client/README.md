# GitHub REST API Client

A CLI tool that fetches GitHub user information and repositories using the GitHub REST API.

## Usage

```bash
dotnet run --project RestClient.csproj
```

## Example

```
GitHub REST API Client
======================

Enter GitHub username or organization: microsoft

Fetching user information...

👤 Microsoft
   Username: microsoft
   Bio: Open source projects and contributions from Microsoft
   Location: Redmond, WA
   Public Repos: 342
   Followers: 125000 | Following: 230
   Profile: https://github.com/microsoft

Fetching repositories for microsoft...

📦 Recent Repositories:

   vscode
      The Visual Studio Code editor
      Language: TypeScript | ⭐ 45000 | 🍴 8500
      URL: https://github.com/microsoft/vscode

✅ Done!
```

## Concepts Demonstrated

- HTTP client for REST API calls
- JSON deserialization with System.Text.Json
- Async/await for network operations
- Source generators for JSON serialization
- User-agent header for API compliance
- Error handling for API responses
