# HTML Table Extractor

Extracts tables from HTML documents and converts them to JSON, CSV, Markdown, or plain text format. Supports local files, URLs, and stdin input.

## Usage

```bash
# From stdin (text output)
cat page.html | dotnet run --project HtmlTableExtractor.csproj

# From file (JSON output)
dotnet run --project HtmlTableExtractor.csproj --json input.html

# From URL (CSV output)
dotnet run --project HtmlTableExtractor.csproj --csv https://example.com/data.html

# Extract specific table (0-indexed)
dotnet run --project HtmlTableExtractor.csproj -t 0 input.html

# Write to output file
dotnet run --project HtmlTableExtractor.csproj --csv -o output.csv input.html
```

## Example

**Input HTML:**
```html
<table>
  <thead>
    <tr><th>Name</th><th>Age</th><th>City</th></tr>
  </thead>
  <tbody>
    <tr><td>Alice</td><td>30</td><td>New York</td></tr>
    <tr><td>Bob</td><td>25</td><td>London</td></tr>
  </tbody>
</table>
```

**Output (JSON):**
```json
{
  "headers": ["Name", "Age", "City"],
  "rows": [
    ["Alice", "30", "New York"],
    ["Bob", "25", "London"]
  ]
}
```

**Output (Markdown):**
```markdown
| Name | Age | City |
|---|---|---|
| Alice | 30 | New York |
| Bob | 25 | London |
```

**Output (CSV):**
```csv
Name,Age,City
Alice,30,New York
Bob,25,London
```

## Concepts Demonstrated

- HTML parsing with AngleSharp
- DOM traversal and element selection
- Table structure extraction (thead, tbody, tr, th, td)
- Multiple output format generation (JSON, CSV, Markdown)
- HTTP client for URL fetching
- File I/O and stdin reading
- Command-line argument parsing
- Text formatting and alignment
