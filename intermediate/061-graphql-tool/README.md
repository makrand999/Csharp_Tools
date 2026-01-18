# GraphQL Query Tool

Interactive CLI tool for executing GraphQL queries against any GraphQL endpoint.

## Usage

```bash
dotnet run --project GraphQLTool/GraphQLTool.csproj
```

## Example

```
GraphQL Query Tool
==================
Default endpoint: https://countries.trevorblades.com/graphql
Enter 'q' to quit or 'e' to change endpoint

GraphQL Query> { countries { code name } }
Variables (JSON, optional): 

--- Result ---
{
  "data": {
    "countries": [
      { "code": "AD", "name": "Andorra" },
      { "code": "AE", "name": "United Arab Emirates" },
      ...
    ]
  }
}
```

## Concepts Demonstrated

- HTTP client for POST requests
- JSON serialization/deserialization
- Interactive REPL-style input
- GraphQL query execution
- Error handling for network requests
