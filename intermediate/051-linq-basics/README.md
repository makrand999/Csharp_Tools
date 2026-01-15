# LINQ Data Utility

A practical command-line tool for filtering, transforming, and analyzing product data using LINQ operations.

## Usage

```bash
dotnet run --project LinqBasics.csproj [command] [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `filter <category>` | Filter products by category (Electronics, Furniture, Office) |
| `expensive [minPrice]` | Show products above price threshold (default: $100) |
| `lowstock [threshold]` | Show products with stock below threshold (default: 50) |
| `group` | Group products by category |
| `stats` | Show statistics about the product catalog |
| `search <term>` | Search products by name |

## Examples

```bash
# Filter by category
dotnet run --project LinqBasics.csproj filter Electronics

# Show expensive products
dotnet run --project LinqBasics.csproj expensive 200

# Show low stock items
dotnet run --project LinqBasics.csproj lowstock 100

# Group by category
dotnet run --project LinqBasics.csproj group

# Show statistics
dotnet run --project LinqBasics.csproj stats

# Search products
dotnet run --project LinqBasics.csproj search desk
```

## Sample Output

```
$ dotnet run --project LinqBasics.csproj stats
Product Statistics:
  Total Products:     10
  Total Value:        $188,244.25
  Average Price:      $256.79
  Most Expensive:     Laptop ($999.99)
  Cheapest:           Notebook ($4.99)
  Total Stock:        1510 units
  Categories:         3
```

## Concepts Demonstrated

- LINQ filtering with `Where()`
- Sorting with `OrderBy()` and `OrderByDescending()`
- Grouping with `GroupBy()`
- Aggregation: `Sum()`, `Average()`, `Min()`, `Max()`
- Finding extremes with `MinBy()` and `MaxBy()`
- Projection and transformation
- Pattern matching with switch expressions
- Record types for data modeling
