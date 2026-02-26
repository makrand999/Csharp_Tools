namespace SqlQueryBuilder;

public class Program
{
    public static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("SQL Query Builder - Build SQL queries programmatically");
            Console.WriteLine();
            Console.WriteLine("Usage:");
            Console.WriteLine("  dotnet run --project SqlQueryBuilder.csproj -- demo");
            Console.WriteLine("  dotnet run --project SqlQueryBuilder.csproj -- interactive");
            return 0;
        }

        if (args[0].Equals("demo", StringComparison.OrdinalIgnoreCase))
        {
            RunDemo();
            return 0;
        }

        if (args[0].Equals("interactive", StringComparison.OrdinalIgnoreCase))
        {
            RunInteractiveMode();
            return 0;
        }

        Console.WriteLine($"Unknown command: {args[0]}");
        Console.WriteLine("Use 'demo' or 'interactive'");
        return 1;
    }

    private static void RunDemo()
    {
        Console.WriteLine("=== SQL Query Builder Demo ===\n");

        // SELECT query
        var selectQuery = SqlQuery.Select("id", "name", "email")
            .From("users")
            .Where("age", ">", 18)
            .AndWhere("status", "=", "active")
            .OrderBy("name", OrderDirection.Asc)
            .Limit(10);

        Console.WriteLine("SELECT Query:");
        Console.WriteLine(selectQuery.Build());
        Console.WriteLine("Parameters: " + string.Join(", ", selectQuery.GetParameters()));
        Console.WriteLine();

        // INSERT query
        var insertQuery = SqlQuery.InsertInto("users")
            .Values(new Dictionary<string, object>
            {
                ["name"] = "John Doe",
                ["email"] = "john@example.com",
                ["age"] = 25
            });

        Console.WriteLine("INSERT Query:");
        Console.WriteLine(insertQuery.Build());
        Console.WriteLine("Parameters: " + string.Join(", ", insertQuery.GetParameters()));
        Console.WriteLine();

        // UPDATE query
        var updateQuery = SqlQuery.Update("users")
            .Set("status", "active")
            .Set("last_login", DateTime.Now)
            .Where("id", "=", 42);

        Console.WriteLine("UPDATE Query:");
        Console.WriteLine(updateQuery.Build());
        Console.WriteLine("Parameters: " + string.Join(", ", updateQuery.GetParameters()));
        Console.WriteLine();

        // DELETE query
        var deleteQuery = SqlQuery.DeleteFrom("logs")
            .Where("created_at", "<", DateTime.Now.AddDays(-30));

        Console.WriteLine("DELETE Query:");
        Console.WriteLine(deleteQuery.Build());
        Console.WriteLine("Parameters: " + string.Join(", ", deleteQuery.GetParameters()));
        Console.WriteLine();

        // Complex query with JOIN
        var complexQuery = SqlQuery.Select("u.id", "u.name", "o.total")
            .From("users", "u")
            .Join("orders", "o").On("u.id", "o.user_id")
            .Where("u.status", "=", "active")
            .AndWhere("o.total", ">", 100)
            .GroupBy("u.id")
            .Having("COUNT(o.id)", ">", 5)
            .OrderBy("o.total", OrderDirection.Desc);

        Console.WriteLine("Complex JOIN Query:");
        Console.WriteLine(complexQuery.Build());
        Console.WriteLine("Parameters: " + string.Join(", ", complexQuery.GetParameters()));
    }

    private static void RunInteractiveMode()
    {
        Console.WriteLine("SQL Query Builder (Interactive Mode)");
        Console.WriteLine("Type 'quit' to exit.");
        Console.WriteLine("Commands: select, insert, update, delete");
        Console.WriteLine();

        while (true)
        {
            Console.Write("> ");
            var input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input))
                continue;

            if (input.Equals("quit", StringComparison.OrdinalIgnoreCase))
                break;

            try
            {
                var query = BuildFromInput(input);
                Console.WriteLine(query.Build());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }
    }

    private static SqlQuery BuildFromInput(string input)
    {
        // Simple parser for interactive mode
        if (input.StartsWith("SELECT ", StringComparison.OrdinalIgnoreCase))
        {
            return SqlQuery.Select("*").From("table_name").Build().Length > 0 
                ? SqlQuery.Select("*").From("table_name") 
                : SqlQuery.Select("*").From("table_name");
        }
        throw new NotImplementedException("Use demo mode for full examples");
    }
}

public enum OrderDirection { Asc, Desc }

public class SqlQuery
{
    private readonly List<string> _selectColumns = new();
    private string _table = string.Empty;
    private string _tableAlias = string.Empty;
    private readonly List<JoinClause> _joins = new();
    private readonly List<WhereClause> _whereClauses = new();
    private readonly List<string> _groupByColumns = new();
    private readonly List<HavingClause> _havingClauses = new();
    private readonly List<OrderByClause> _orderByColumns = new();
    private int? _limit;
    private int? _offset;

    private readonly Dictionary<string, object> _parameters = new();
    private int _paramCounter;

    // SELECT
    public static SqlQuery Select(params string[] columns)
    {
        var query = new SqlQuery();
        query._selectColumns.AddRange(columns);
        return query;
    }

    public SqlQuery From(string table, string? alias = null)
    {
        _table = table;
        _tableAlias = alias ?? string.Empty;
        return this;
    }

    // JOIN
    public SqlQuery Join(string table, string alias)
    {
        var join = new JoinClause { Table = table, Alias = alias };
        _joins.Add(join);
        return this;
    }

    public SqlQuery On(string leftColumn, string rightColumn)
    {
        if (_joins.Count == 0) throw new InvalidOperationException("No JOIN to add ON clause to");
        _joins[^1].OnClause = $"{leftColumn} = {rightColumn}";
        return this;
    }

    // WHERE
    public SqlQuery Where(string column, string op, object value)
    {
        var paramName = AddParameter(value);
        _whereClauses.Add(new WhereClause { Column = column, Operator = op, ParameterName = paramName, IsAnd = false });
        return this;
    }

    public SqlQuery AndWhere(string column, string op, object value)
    {
        var paramName = AddParameter(value);
        _whereClauses.Add(new WhereClause { Column = column, Operator = op, ParameterName = paramName, IsAnd = true });
        return this;
    }

    public SqlQuery OrWhere(string column, string op, object value)
    {
        var paramName = AddParameter(value);
        _whereClauses.Add(new WhereClause { Column = column, Operator = op, ParameterName = paramName, IsAnd = false, IsOr = true });
        return this;
    }

    // GROUP BY
    public SqlQuery GroupBy(params string[] columns)
    {
        _groupByColumns.AddRange(columns);
        return this;
    }

    // HAVING
    public SqlQuery Having(string expression, string op, object value)
    {
        var paramName = AddParameter(value);
        _havingClauses.Add(new HavingClause { Expression = expression, Operator = op, ParameterName = paramName });
        return this;
    }

    // ORDER BY
    public SqlQuery OrderBy(string column, OrderDirection direction = OrderDirection.Asc)
    {
        _orderByColumns.Add(new OrderByClause { Column = column, Direction = direction });
        return this;
    }

    // LIMIT / OFFSET
    public SqlQuery Limit(int limit)
    {
        _limit = limit;
        return this;
    }

    public SqlQuery Offset(int offset)
    {
        _offset = offset;
        return this;
    }

    // INSERT
    public static SqlQuery InsertInto(string table)
    {
        var query = new SqlQuery();
        query._table = table;
        return query;
    }

    private readonly Dictionary<string, object> _insertValues = new();
    
    public SqlQuery Values(Dictionary<string, object> values)
    {
        foreach (var kvp in values)
            _insertValues[kvp.Key] = kvp.Value;
        return this;
    }

    // UPDATE
    public static SqlQuery Update(string table)
    {
        var query = new SqlQuery();
        query._table = table;
        return query;
    }

    private readonly Dictionary<string, object> _updateValues = new();

    public SqlQuery Set(string column, object value)
    {
        _updateValues[column] = value;
        return this;
    }

    // DELETE
    public static SqlQuery DeleteFrom(string table)
    {
        var query = new SqlQuery();
        query._table = table;
        return query;
    }

    public string Build()
    {
        if (_insertValues.Count > 0)
            return BuildInsert();
        
        if (_updateValues.Count > 0)
            return BuildUpdate();
        
        if (_table.Length > 0 && _selectColumns.Count == 0)
            return BuildDelete();

        return BuildSelect();
    }

    private string BuildSelect()
    {
        var sql = "SELECT " + string.Join(", ", _selectColumns);
        sql += " FROM " + _table;
        
        if (!string.IsNullOrEmpty(_tableAlias))
            sql += " " + _tableAlias;

        foreach (var join in _joins)
            sql += $" INNER JOIN {join.Table} {join.Alias} ON {join.OnClause}";

        if (_whereClauses.Count > 0)
        {
            sql += " WHERE ";
            var parts = new List<string>();
            foreach (var w in _whereClauses)
            {
                var prefix = w.IsAnd ? "AND " : "";
                if (w.IsOr) prefix = "OR ";
                parts.Add($"{prefix}{w.Column} {w.Operator} @{w.ParameterName}");
            }
            sql += string.Join(" ", parts);
        }

        if (_groupByColumns.Count > 0)
            sql += " GROUP BY " + string.Join(", ", _groupByColumns);

        if (_havingClauses.Count > 0)
        {
            sql += " HAVING ";
            var parts = _havingClauses.Select(h => $"{h.Expression} {h.Operator} @{h.ParameterName}");
            sql += string.Join(" AND ", parts);
        }

        if (_orderByColumns.Count > 0)
        {
            sql += " ORDER BY ";
            var parts = _orderByColumns.Select(o => $"{o.Column} {(o.Direction == OrderDirection.Asc ? "ASC" : "DESC")}");
            sql += string.Join(", ", parts);
        }

        if (_limit.HasValue)
            sql += $" LIMIT {_limit}";

        if (_offset.HasValue)
            sql += $" OFFSET {_offset}";

        return sql + ";";
    }

    private string BuildInsert()
    {
        var columns = string.Join(", ", _insertValues.Keys);
        var paramNames = _insertValues.Keys.Select(k => $"@{AddParameter(_insertValues[k])}");
        var values = string.Join(", ", paramNames);
        return $"INSERT INTO {_table} ({columns}) VALUES ({values});";
    }

    private string BuildUpdate()
    {
        var sets = _updateValues.Select(kvp => $"{kvp.Key} = @{AddParameter(kvp.Value)}");
        var sql = $"UPDATE {_table} SET {string.Join(", ", sets)}";

        if (_whereClauses.Count > 0)
        {
            sql += " WHERE ";
            var parts = new List<string>();
            foreach (var w in _whereClauses)
            {
                var prefix = w.IsAnd ? "AND " : "";
                parts.Add($"{prefix}{w.Column} {w.Operator} @{w.ParameterName}");
            }
            sql += string.Join(" ", parts);
        }

        return sql + ";";
    }

    private string BuildDelete()
    {
        var sql = $"DELETE FROM {_table}";

        if (_whereClauses.Count > 0)
        {
            sql += " WHERE ";
            var parts = new List<string>();
            foreach (var w in _whereClauses)
            {
                var prefix = w.IsAnd ? "AND " : "";
                parts.Add($"{prefix}{w.Column} {w.Operator} @{w.ParameterName}");
            }
            sql += string.Join(" ", parts);
        }

        return sql + ";";
    }

    private string AddParameter(object value)
    {
        var paramName = $"p{_paramCounter++}";
        _parameters[paramName] = value;
        return paramName;
    }

    public Dictionary<string, object> GetParameters() => new(_parameters);

    // Helper classes
    private class JoinClause
    {
        public string Table { get; set; } = string.Empty;
        public string Alias { get; set; } = string.Empty;
        public string OnClause { get; set; } = string.Empty;
    }

    private class WhereClause
    {
        public string Column { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
        public bool IsAnd { get; set; }
        public bool IsOr { get; set; }
    }

    private class HavingClause
    {
        public string Expression { get; set; } = string.Empty;
        public string Operator { get; set; } = string.Empty;
        public string ParameterName { get; set; } = string.Empty;
    }

    private class OrderByClause
    {
        public string Column { get; set; } = string.Empty;
        public OrderDirection Direction { get; set; }
    }
}
