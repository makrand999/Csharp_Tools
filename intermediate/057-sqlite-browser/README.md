# SQLite Database Browser

A CLI tool that browses SQLite database files, showing tables, columns, and sample data.

## Usage

```bash
dotnet run --project SqliteBrowser.csproj
```

## Example

```
SQLite Database Browser
=======================

Enter path to SQLite database file: /path/to/chinook.db

✅ Connected to: chinook.db

📊 Tables in database:
   - Album
   - Artist
   - Customer
   - Employee
   - Genre
   - Invoice
   - Track

---

📋 Table: Album
   Columns:
      PK AlbumId (INTEGER) NOT NULL
         Title (NVARCHAR) NOT NULL
         ArtistId (INTEGER) NOT NULL
   Rows: 204
   Sample data (first 3 rows):
      [1 | For Those About To Rock We Salute You | 1]
      [2 | Balls to the Wall | 2]
      [3 | Restless and Wild | 2]

✅ Done!
```

## Concepts Demonstrated

- SQLite database connection with Microsoft.Data.Sqlite
- Executing SQL queries and reading results
- PRAGMA commands for metadata
- Dynamic query building
- Handling NULL values in database results
- File system validation
