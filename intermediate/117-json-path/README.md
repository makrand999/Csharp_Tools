# JsonPathQuery

Query JSON files using path expressions. Extract specific values, navigate nested structures, and transform JSON data from the command line.

## Usage

```bash
dotnet run --project JsonPath.csproj -- query <file.json> <path>
dotnet run --project JsonPath.csproj -- query '<json>' <path>
dotnet run --project JsonPath.csproj -- get <file.json> <json-path>
dotnet run --project JsonPath.csproj -- keys <file.json>
dotnet run --project JsonPath.csproj -- flatten <file.json>
```

## Path Syntax

| Syntax | Description |
|--------|-------------|
| `$` | Root node |
| `.property` | Child property access |
| `['property']` | Bracket notation for properties |
| `[0]` | Array index access |
| `[*]` | All array items |
| `..property` | Recursive descent (find property anywhere) |

## Examples

### Sample JSON (data.json)
```json
{
  "store": {
    "book": [
      { "title": "Book 1", "author": "Author A", "price": 10 },
      { "title": "Book 2", "author": "Author B", "price": 20 }
    ],
    "name": "My Store"
  }
}
```

### Query Commands

```bash
# Get entire document
dotnet run --project JsonPath.csproj -- query data.json $

# Access nested property
dotnet run --project JsonPath.csproj -- query data.json $.store

# Access array element
dotnet run --project JsonPath.csproj -- query data.json '$.store.book[0]'

# Get all titles
dotnet run --project JsonPath.csproj -- query data.json '$.store.book[*].title'

# Find all authors recursively
dotnet run --project JsonPath.csproj -- query data.json '$..author'

# List all keys
dotnet run --project JsonPath.csproj -- keys data.json

# Flatten to key-value pairs
dotnet run --project JsonPath.csproj -- flatten data.json
```

## Sample Output

```
# Query result
{
  "title": "Book 1",
  "author": "Author A",
  "price": 10
}

# Keys output
Keys:
  store (object)

# Flattened output
Flattened JSON:
  store.name = My Store
  store.book[0].title = Book 1
  store.book[0].author = Author A
  store.book[0].price = 10
  store.book[1].title = Book 2
```

## Commands

| Command | Description |
|---------|-------------|
| `query` | Query JSON with path expression |
| `get` | Get value at specific path |
| `keys` | List all keys in JSON object |
| `flatten` | Flatten nested JSON to dot notation |

## Concepts Demonstrated

- System.Text.Json.Nodes for JSON parsing
- Path expression parsing and evaluation
- Recursive descent algorithms
- StringBuilder for path construction
- Dictionary manipulation
- JSON serialization options
