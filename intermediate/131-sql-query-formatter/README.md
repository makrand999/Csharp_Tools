# SQL Query Formatter

Format and beautify SQL queries with proper indentation and keyword capitalization.

## Usage

```bash
dotnet run --project 131-sql-query-formatter.csproj -- "<sql-query>"
```

## Example

```bash
dotnet run --project 131-sql-query-formatter.csproj -- "select * from users where id = 1 order by name"
```

### Sample Output

```sql
SELECT *
FROM users
WHERE id = 1
ORDER BY name
```

## Options

- `--uppercase` - Convert keywords to UPPERCASE (default)
- `--lowercase` - Convert keywords to lowercase

## Concepts Demonstrated

- Regular expressions
- String manipulation
- SQL keyword recognition
- Text formatting
- Command-line argument parsing
