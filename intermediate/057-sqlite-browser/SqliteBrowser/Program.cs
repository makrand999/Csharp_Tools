using Microsoft.Data.Sqlite;

Console.WriteLine("SQLite Database Browser");
Console.WriteLine("=======================\n");

Console.Write("Enter path to SQLite database file: ");
var dbPath = Console.ReadLine()?.Trim() ?? "";

if (string.IsNullOrEmpty(dbPath) || !File.Exists(dbPath))
{
    Console.WriteLine("Error: Invalid database file path.");
    return;
}

var connectionString = new SqliteConnectionStringBuilder
{
    DataSource = dbPath
}.ToString();

using var connection = new SqliteConnection(connectionString);
connection.Open();

Console.WriteLine($"\n✅ Connected to: {Path.GetFileName(dbPath)}");

// Get all tables
Console.WriteLine("\n📊 Tables in database:");
var tablesQuery = "SELECT name FROM sqlite_master WHERE type='table' ORDER BY name;";
using var tablesCommand = new SqliteCommand(tablesQuery, connection);
using var tablesReader = tablesCommand.ExecuteReader();

var tables = new List<string>();
while (tablesReader.Read())
{
    var tableName = tablesReader.GetString(0);
    if (!tableName.StartsWith("sqlite_"))
    {
        tables.Add(tableName);
        Console.WriteLine($"   - {tableName}");
    }
}

if (tables.Count == 0)
{
    Console.WriteLine("   (No tables found)");
    return;
}

Console.WriteLine("\n---");

// Show details for each table
foreach (var table in tables)
{
    Console.WriteLine($"\n📋 Table: {table}");
    
    // Get column info
    var columnsQuery = $"PRAGMA table_info({table});";
    using var columnsCommand = new SqliteCommand(columnsQuery, connection);
    using var columnsReader = columnsCommand.ExecuteReader();
    
    Console.WriteLine("   Columns:");
    var columnNames = new List<string>();
    while (columnsReader.Read())
    {
        var colName = columnsReader.GetString(1);
        var colType = columnsReader.GetString(2);
        var notNull = columnsReader.GetInt32(3) == 1 ? "NOT NULL" : "";
        var primaryKey = columnsReader.GetInt32(5) == 1 ? "PK" : "";
        columnNames.Add(colName);
        Console.WriteLine($"      {primaryKey} {colName} ({colType}) {notNull}");
    }
    
    // Get row count
    var countQuery = $"SELECT COUNT(*) FROM {table};";
    using var countCommand = new SqliteCommand(countQuery, connection);
    var rowCount = countCommand.ExecuteScalar();
    Console.WriteLine($"   Rows: {rowCount}");
    
    // Show sample data (first 3 rows)
    Console.WriteLine("   Sample data (first 3 rows):");
    var sampleQuery = $"SELECT * FROM {table} LIMIT 3;";
    using var sampleCommand = new SqliteCommand(sampleQuery, connection);
    using var sampleReader = sampleCommand.ExecuteReader();
    
    while (sampleReader.Read())
    {
        var values = new List<string>();
        for (int i = 0; i < sampleReader.FieldCount; i++)
        {
            var value = sampleReader.GetValue(i);
            var displayValue = value == DBNull.Value ? "NULL" : value.ToString();
            if (displayValue != null && displayValue.Length > 30)
                displayValue = displayValue[..30] + "...";
            values.Add(displayValue ?? "NULL");
        }
        Console.WriteLine($"      [{string.Join(" | ", values)}]");
    }
}

Console.WriteLine("\n✅ Done!");
