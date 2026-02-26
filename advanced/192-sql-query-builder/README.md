# SQL Query Builder

A fluent SQL query builder that allows you to programmatically construct SQL queries with proper parameter binding. Supports SELECT, INSERT, UPDATE, and DELETE operations with JOINs, WHERE clauses, GROUP BY, HAVING, and ORDER BY.

## Usage

```bash
# Run demo
dotnet run --project SqlQueryBuilder.csproj -- demo

# Interactive mode
dotnet run --project SqlQueryBuilder.csproj -- interactive
```

## Examples

### SELECT Query
```csharp
var query = SqlQuery.Select("id", "name", "email")
    .From("users")
    .Where("age", ">", 18)
    .AndWhere("status", "=", "active")
    .OrderBy("name", OrderDirection.Asc)
    .Limit(10);

Console.WriteLine(query.Build());
// SELECT id, name, email FROM users WHERE age > @p0 AND status = @p1 ORDER BY name ASC LIMIT 10;
```

### INSERT Query
```csharp
var query = SqlQuery.InsertInto("users")
    .Values(new Dictionary<string, object>
    {
        ["name"] = "John Doe",
        ["email"] = "john@example.com",
        ["age"] = 25
    });

Console.WriteLine(query.Build());
// INSERT INTO users (name, email, age) VALUES (@p0, @p1, @p2);
```

### UPDATE Query
```csharp
var query = SqlQuery.Update("users")
    .Set("status", "active")
    .Set("last_login", DateTime.Now)
    .Where("id", "=", 42);

Console.WriteLine(query.Build());
// UPDATE users SET status = @p0, last_login = @p1 WHERE id = @p2;
```

### DELETE Query
```csharp
var query = SqlQuery.DeleteFrom("logs")
    .Where("created_at", "<", DateTime.Now.AddDays(-30));

Console.WriteLine(query.Build());
// DELETE FROM logs WHERE created_at < @p0;
```

### Complex JOIN Query
```csharp
var query = SqlQuery.Select("u.id", "u.name", "o.total")
    .From("users", "u")
    .Join("orders", "o").On("u.id", "o.user_id")
    .Where("u.status", "=", "active")
    .AndWhere("o.total", ">", 100)
    .GroupBy("u.id")
    .Having("COUNT(o.id)", ">", 5)
    .OrderBy("o.total", OrderDirection.Desc);

Console.WriteLine(query.Build());
// SELECT u.id, u.name, o.total FROM users u 
// INNER JOIN orders o ON u.id = o.user_id 
// WHERE u.status = @p0 AND o.total = @p1 
// GROUP BY u.id HAVING COUNT(o.id) > @p2 
// ORDER BY o.total DESC;
```

## Features

- **Fluent API** - Chainable methods for building queries
- **Parameter binding** - Automatic parameter placeholder generation
- **All CRUD operations** - SELECT, INSERT, UPDATE, DELETE
- **JOIN support** - INNER JOIN with ON clauses
- **WHERE clauses** - AND/OR conditions with operators
- **Aggregation** - GROUP BY and HAVING clauses
- **Sorting** - ORDER BY with ASC/DESC
- **Pagination** - LIMIT and OFFSET support

## Concepts Demonstrated

- Fluent interface pattern
- Builder pattern for SQL construction
- Parameter binding for SQL injection prevention
- Expression building with method chaining
- Dictionary-based parameter management
