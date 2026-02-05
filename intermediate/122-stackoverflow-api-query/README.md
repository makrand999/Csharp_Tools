# Stack Overflow API Query Tool

Search Stack Overflow for questions matching a query using the official API.

## Usage

```bash
dotnet run --project 122-stackoverflow-api-query.csproj -- [search terms] [--top N]
```

## Example

```bash
dotnet run --project 122-stackoverflow-api-query.csproj -- c# async --top 5
dotnet run --project 122-stackoverflow-api-query.csproj -- docker compose
```

### Sample Output

```
Searching Stack Overflow for: 'c# async'
Fetching top 5 results...

1. Understanding async/await in C#
   Score: 150 | Answers: 12 | Views: 50000
   Tags: c#, async-await, task
   Link: https://stackoverflow.com/questions/...
```

## Concepts Demonstrated

- REST API consumption
- URL query string building
- JSON deserialization
- LINQ for filtering and transformation
- Command-line argument parsing
