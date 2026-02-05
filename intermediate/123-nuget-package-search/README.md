# NuGet Package Search Tool

Search for .NET packages on NuGet.org and display package information.

## Usage

```bash
dotnet run --project 123-nuget-package-search.csproj -- <search-term> [--take N]
```

## Example

```bash
dotnet run --project 123-nuget-package-search.csproj -- json --take 10
dotnet run --project 123-nuget-package-search.csproj -- serilog
```

### Sample Output

```
Searching NuGet for: 'json'
Fetching top 10 packages...

1. Newtonsoft.Json v13.0.3
   Downloads: 2,500,000,000+
   Json.NET is a popular high-performance JSON framework for .NET
   Project: https://www.newtonsoft.com/json

2. System.Text.Json v8.0.0
   Downloads: 500,000,000+
   Provides high-performance, low-allocating, standards-compliant JSON support
```

## Concepts Demonstrated

- NuGet Search API
- JSON parsing
- Number formatting
- Text truncation for display
- API result pagination
