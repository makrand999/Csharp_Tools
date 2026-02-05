# GitHub Profile Reader

Fetch and display GitHub user profile information using the GitHub REST API.

## Usage

```bash
dotnet run --project 121-github-profile-reader.csproj -- <username>
```

## Example

```bash
dotnet run --project 121-github-profile-reader.csproj -- octocat
```

### Sample Output

```
Fetching profile for: octocat

=== GitHub Profile ===
Username: octocat
Name: The Octocat
Bio: N/A
Location: San Francisco
Blog: https://github.com
Public Repos: 85
Followers: 100000+
Following: 10
Profile URL: https://github.com/octocat
```

## Concepts Demonstrated

- HTTP client with custom headers
- JSON parsing with System.Text.Json
- Async/await patterns
- API error handling
- Console output formatting
