namespace SqlQueryFormatter;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("SQL Query Formatter");
            Console.WriteLine("Usage: dotnet run --project 131-sql-query-formatter.csproj -- \"<sql-query>\"");
            Console.WriteLine("       echo \"SELECT * FROM users\" | dotnet run --project 131-sql-query-formatter.csproj");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --uppercase    Convert keywords to UPPERCASE");
            Console.WriteLine("  --lowercase    Convert keywords to lowercase");
            return;
        }

        bool uppercase = args.Contains("--uppercase");
        bool lowercase = args.Contains("--lowercase");
        string sql = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "";

        if (string.IsNullOrEmpty(sql))
        {
            sql = Console.In.ReadLine() ?? "";
        }

        var formatted = FormatSql(sql, uppercase || !lowercase);
        Console.WriteLine(formatted);
    }

    static string FormatSql(string sql, bool uppercaseKeywords)
    {
        var keywords = new[]
        {
            "SELECT", "FROM", "WHERE", "AND", "OR", "NOT", "IN", "BETWEEN", "LIKE",
            "ORDER", "BY", "GROUP", "HAVING", "LIMIT", "OFFSET", "JOIN", "LEFT",
            "RIGHT", "INNER", "OUTER", "ON", "AS", "DISTINCT", "COUNT", "SUM",
            "AVG", "MIN", "MAX", "INSERT", "INTO", "VALUES", "UPDATE", "SET",
            "DELETE", "CREATE", "TABLE", "ALTER", "DROP", "INDEX", "PRIMARY",
            "KEY", "FOREIGN", "REFERENCES", "CONSTRAINT", "DEFAULT", "NULL",
            "CASE", "WHEN", "THEN", "ELSE", "END", "UNION", "ALL", "EXISTS",
            "ASC", "DESC", "NULLS", "FIRST", "LAST", "CROSS", "NATURAL", "USING"
        };

        var result = sql;
        
        // Simple formatting: add newlines after certain keywords
        var breakKeywords = new[] { "SELECT", "FROM", "WHERE", "ORDER BY", "GROUP BY", "HAVING", "LIMIT", "INSERT INTO", "VALUES", "UPDATE", "SET", "DELETE FROM" };
        
        foreach (var keyword in breakKeywords)
        {
            result = System.Text.RegularExpressions.Regex.Replace(
                result, 
                $@"\b{keyword}\b", 
                $"\n{keyword}", 
                System.Text.RegularExpressions.RegexOptions.IgnoreCase
            );
        }

        // Format keywords
        if (uppercaseKeywords)
        {
            foreach (var keyword in keywords)
            {
                result = System.Text.RegularExpressions.Regex.Replace(
                    result,
                    $@"\b{keyword}\b",
                    keyword,
                    System.Text.RegularExpressions.RegexOptions.IgnoreCase
                );
            }
        }

        // Clean up multiple newlines and trim
        var lines = result.Split('\n')
            .Select(l => l.Trim())
            .Where(l => !string.IsNullOrEmpty(l))
            .ToList();

        return string.Join('\n', lines).Trim();
    }
}
