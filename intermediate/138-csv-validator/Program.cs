namespace CsvValidator;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("CSV Validator");
            Console.WriteLine("Usage: dotnet run --project 138-csv-validator.csproj -- <file.csv>");
            Console.WriteLine("       cat data.csv | dotnet run --project 138-csv-validator.csproj");
            Console.WriteLine("\nOptions:");
            Console.WriteLine("  --delimiter CHAR  Set delimiter (default: comma)");
            Console.WriteLine("  --has-header      First row is header (default: auto-detect)");
            return;
        }

        char delimiter = ',';
        bool? hasHeader = null;

        for (int i = 0; i < args.Length; i++)
        {
            if (args[i] == "--delimiter" && i + 1 < args.Length)
            {
                delimiter = args[i + 1][0];
            }
            else if (args[i] == "--has-header")
            {
                hasHeader = true;
            }
        }

        string inputFile = args.FirstOrDefault(a => !a.StartsWith("--")) ?? "";
        string csv;

        if (File.Exists(inputFile))
        {
            csv = File.ReadAllText(inputFile);
            Console.WriteLine($"Validating: {inputFile}");
        }
        else if (string.IsNullOrEmpty(inputFile))
        {
            csv = Console.In.ReadToEnd();
            Console.WriteLine("Validating CSV from stdin...");
        }
        else
        {
            csv = inputFile;
        }

        var result = ValidateCsv(csv, delimiter, hasHeader);

        Console.WriteLine($"\n=== Validation Results ===");
        if (result.Errors.Count == 0)
        {
            Console.WriteLine("✓ CSV appears valid!");
        }
        else
        {
            Console.WriteLine($"✗ Found {result.Errors.Count} issue(s):\n");
            foreach (var error in result.Errors)
            {
                Console.WriteLine($"  Line {error.Line}: {error.Message}");
            }
        }

        Console.WriteLine($"\n=== Statistics ===");
        Console.WriteLine($"Total rows: {result.TotalRows}");
        Console.WriteLine($"Header row: {(result.HasHeader ? "Yes" : "No")}");
        Console.WriteLine($"Columns: {result.ColumnCount}");
        Console.WriteLine($"Delimiter: '{delimiter}'");
        Console.WriteLine($"Empty rows: {result.EmptyRows}");
        Console.WriteLine($"Avg columns per row: {result.AvgColumns:F1}");
    }

    static CsvValidationResult ValidateCsv(string csv, char delimiter, bool? hasHeader)
    {
        var result = new CsvValidationResult();
        var lines = csv.Split('\n');
        var columnCounts = new List<int>();

        // Auto-detect header
        if (hasHeader == null)
        {
            hasHeader = DetectHeader(lines.FirstOrDefault(l => !string.IsNullOrWhiteSpace(l)), delimiter);
        }
        result.HasHeader = hasHeader.Value;

        int expectedColumns = -1;

        for (int i = 0; i < lines.Length; i++)
        {
            var line = lines[i].TrimEnd('\r');
            int lineNum = i + 1;

            if (string.IsNullOrWhiteSpace(line))
            {
                result.EmptyRows++;
                continue;
            }

            // Skip header for column count
            if (i == 0 && hasHeader.Value)
            {
                var columns = ParseCsvLine(line, delimiter);
                expectedColumns = columns.Count;
                columnCounts.Add(expectedColumns);

                // Check for common header issues
                foreach (var col in columns)
                {
                    if (string.IsNullOrWhiteSpace(col))
                    {
                        result.Errors.Add(new CsvError
                        {
                            Line = lineNum,
                            Message = "Empty column name in header"
                        });
                    }
                }
                continue;
            }

            var rowColumns = ParseCsvLine(line, delimiter);
            columnCounts.Add(rowColumns.Count);

            // Check column count consistency
            if (expectedColumns > 0 && rowColumns.Count != expectedColumns)
            {
                result.Errors.Add(new CsvError
                {
                    Line = lineNum,
                    Message = $"Column count mismatch: expected {expectedColumns}, got {rowColumns.Count}"
                });
            }

            // Check for unclosed quotes
            int quoteCount = line.Count(c => c == '"');
            if (quoteCount % 2 != 0)
            {
                result.Errors.Add(new CsvError
                {
                    Line = lineNum,
                    Message = "Unclosed quoted field"
                });
            }

            // Check for empty fields
            for (int j = 0; j < rowColumns.Count; j++)
            {
                if (string.IsNullOrWhiteSpace(rowColumns[j]))
                {
                    // Just tracking, not necessarily an error
                }
            }
        }

        result.TotalRows = lines.Length - result.EmptyRows;
        result.ColumnCount = expectedColumns > 0 ? expectedColumns : (columnCounts.Any() ? columnCounts.Average() : 0);
        result.AvgColumns = columnCounts.Any() ? columnCounts.Average() : 0;

        return result;
    }

    static List<string> ParseCsvLine(string line, char delimiter)
    {
        var columns = new List<string>();
        var current = new System.Text.StringBuilder();
        bool inQuotes = false;

        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];

            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++; // Skip escaped quote
                }
                else
                {
                    inQuotes = !inQuotes;
                }
            }
            else if (c == delimiter && !inQuotes)
            {
                columns.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        columns.Add(current.ToString());
        return columns;
    }

    static bool DetectHeader(string? firstLine, char delimiter)
    {
        if (string.IsNullOrEmpty(firstLine))
            return false;

        var columns = ParseCsvLine(firstLine!, delimiter);
        
        // Header likely if all columns are non-numeric and look like identifiers
        return columns.All(c => 
            !string.IsNullOrWhiteSpace(c) && 
            !double.TryParse(c, out _) &&
            c.Length < 50
        );
    }
}

class CsvValidationResult
{
    public List<CsvError> Errors { get; set; } = new();
    public int TotalRows { get; set; }
    public bool HasHeader { get; set; }
    public double ColumnCount { get; set; }
    public int EmptyRows { get; set; }
    public double AvgColumns { get; set; }
}

class CsvError
{
    public int Line { get; set; }
    public string Message { get; set; } = "";
}
