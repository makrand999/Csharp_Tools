using System.Text;
using System.Globalization;

if (args.Length == 0)
{
    ShowHelp();
    return;
}

var command = args[0].ToLower();

switch (command)
{
    case "merge":
        MergeCsv(args.Skip(1).ToArray());
        break;
    case "info":
        ShowCsvInfo(args.Skip(1).ToArray());
        break;
    case "split":
        SplitCsv(args.Skip(1).ToArray());
        break;
    case "select":
        SelectColumns(args.Skip(1).ToArray());
        break;
    default:
        MergeCsv(args);
        break;
}

void MergeCsv(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- merge <file1.csv> <file2.csv> [file3.csv...] [options]");
        Console.WriteLine("Options:");
        Console.WriteLine("  --output <file>    Output file (default: merged.csv)");
        Console.WriteLine("  --mode <mode>      vertical (append rows) or horizontal (append columns)");
        Console.WriteLine("  --delimiter <char> CSV delimiter (default: ,)");
        return;
    }

    var files = new List<string>();
    var outputFile = "merged.csv";
    var mode = "vertical";
    var delimiter = ',';

    for (var i = 0; i < args.Length; i++)
    {
        if (args[i] == "--output" || args[i] == "-o")
        {
            outputFile = args[++i];
        }
        else if (args[i] == "--mode" && i + 1 < args.Length)
        {
            mode = args[++i].ToLower();
        }
        else if (args[i] == "--delimiter" && i + 1 < args.Length)
        {
            delimiter = args[++i][0];
        }
        else if (!args[i].StartsWith("--"))
        {
            files.Add(args[i]);
        }
    }

    if (files.Count < 2)
    {
        Console.WriteLine("At least 2 CSV files required");
        return;
    }

    try
    {
        var allData = new List<List<string[]>>();
        string[]? commonHeaders = null;

        foreach (var file in files)
        {
            if (!File.Exists(file))
            {
                Console.WriteLine($"File not found: {file}");
                return;
            }

            var data = ReadCsv(file, delimiter);
            if (data.Count == 0)
            {
                Console.WriteLine($"Empty file: {file}");
                continue;
            }

            if (commonHeaders == null)
            {
                commonHeaders = data[0];
            }
            else if (mode == "vertical" && !commonHeaders.SequenceEqual(data[0]))
            {
                Console.WriteLine($"Warning: Headers mismatch in {file}");
                Console.WriteLine($"  Expected: {string.Join(delimiter, commonHeaders)}");
                Console.WriteLine($"  Got: {string.Join(delimiter, data[0])}");
            }

            allData.Add(data);
        }

        using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);
        
        if (mode == "vertical")
        {
            // Append rows - write header once
            if (commonHeaders != null)
            {
                writer.WriteLine(string.Join(delimiter, commonHeaders.Select(Escape)));
            }

            // Write all rows (skip headers from subsequent files)
            for (var i = 0; i < allData.Count; i++)
            {
                var startRow = i == 0 ? 1 : 1; // Skip header in all files
                for (var j = startRow; j < allData[i].Count; j++)
                {
                    writer.WriteLine(string.Join(delimiter, allData[i][j].Select(Escape)));
                }
            }
        }
        else
        {
            // Horizontal merge - combine columns
            var maxRows = allData.Max(d => d.Count - 1); // Exclude headers

            // Combined header
            var combinedHeader = allData.SelectMany(d => d[0]).ToArray();
            writer.WriteLine(string.Join(delimiter, combinedHeader.Select(Escape)));

            // Combined rows
            for (var row = 1; row <= maxRows; row++)
            {
                var combinedRow = new List<string>();
                foreach (var data in allData)
                {
                    if (row < data.Count)
                        combinedRow.AddRange(data[row]);
                    else
                        combinedRow.AddRange(Enumerable.Repeat("", data[0].Length));
                }
                writer.WriteLine(string.Join(delimiter, combinedRow.Select(Escape)));
            }
        }

        Console.WriteLine($"✓ Merged {files.Count} files into {outputFile}");
        Console.WriteLine($"  Mode: {mode}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ShowCsvInfo(string[] args)
{
    if (args.Length == 0)
    {
        Console.WriteLine("Usage: dotnet run -- info <file.csv>");
        return;
    }

    var file = args[0];
    if (!File.Exists(file))
    {
        Console.WriteLine($"File not found: {file}");
        return;
    }

    try
    {
        var data = ReadCsv(file, ',');
        if (data.Count == 0)
        {
            Console.WriteLine("Empty file");
            return;
        }

        var headers = data[0];
        var rows = data.Count - 1;
        var fileSize = new FileInfo(file).Length;

        Console.WriteLine($"CSV Information: {Path.GetFileName(file)}");
        Console.WriteLine(new string('-', 50));
        Console.WriteLine($"File size: {fileSize:N0} bytes");
        Console.WriteLine($"Columns: {headers.Length}");
        Console.WriteLine($"Rows: {rows:N0}");
        Console.WriteLine();
        Console.WriteLine("Columns:");

        for (var i = 0; i < headers.Length; i++)
        {
            Console.WriteLine($"  {i + 1}. {headers[i]}");
        }

        if (rows > 0)
        {
            Console.WriteLine();
            Console.WriteLine("Sample (first data row):");
            for (var i = 0; i < headers.Length; i++)
            {
                var value = data[1].Length > i ? data[1][i] : "";
                if (value.Length > 40)
                    value = value[..37] + "...";
                Console.WriteLine($"  {headers[i]}: {value}");
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void SplitCsv(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- split <file.csv> <rows-per-file>");
        Console.WriteLine("Options:");
        Console.WriteLine("  --output <pattern>  Output pattern (default: split_{n}.csv)");
        return;
    }

    var file = args[0];
    if (!File.Exists(file))
    {
        Console.WriteLine($"File not found: {file}");
        return;
    }

    if (!int.TryParse(args[1], out var rowsPerFile) || rowsPerFile < 1)
    {
        Console.WriteLine("Invalid rows-per-file value");
        return;
    }

    var outputPattern = "split_{n}.csv";
    for (var i = 2; i < args.Length; i++)
    {
        if (args[i] == "--output" && i + 1 < args.Length)
        {
            outputPattern = args[++i];
        }
    }

    try
    {
        var data = ReadCsv(file, ',');
        if (data.Count < 2)
        {
            Console.WriteLine("No data rows to split");
            return;
        }

        var headers = data[0];
        var dataRows = data.Skip(1).ToList();
        var fileCount = (int)Math.Ceiling((double)dataRows.Count / rowsPerFile);

        for (var i = 0; i < fileCount; i++)
        {
            var outputFile = outputPattern.Replace("{n}", (i + 1).ToString());
            using var writer = new StreamWriter(outputFile, false, Encoding.UTF8);

            // Write header
            writer.WriteLine(string.Join(",", headers.Select(Escape)));

            // Write rows
            var start = i * rowsPerFile;
            var end = Math.Min(start + rowsPerFile, dataRows.Count);
            for (var j = start; j < end; j++)
            {
                writer.WriteLine(string.Join(",", dataRows[j].Select(Escape)));
            }

            Console.WriteLine($"  {outputFile}: {end - start} rows");
        }

        Console.WriteLine($"✓ Split into {fileCount} files");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void SelectColumns(string[] args)
{
    if (args.Length < 2)
    {
        Console.WriteLine("Usage: dotnet run -- select <file.csv> <col1,col2,...>");
        Console.WriteLine("Options:");
        Console.WriteLine("  --output <file>  Output file (default: stdout)");
        return;
    }

    var file = args[0];
    if (!File.Exists(file))
    {
        Console.WriteLine($"File not found: {file}");
        return;
    }

    var columns = args[1].Split(',').Select(c => c.Trim()).ToList();
    var outputFile = args.Contains("--output") 
        ? args.ElementAt(Array.IndexOf(args, "--output") + 1) 
        : null;

    try
    {
        var data = ReadCsv(file, ',');
        if (data.Count == 0)
        {
            Console.WriteLine("Empty file");
            return;
        }

        var headers = data[0];
        var indices = new List<int>();

        foreach (var col in columns)
        {
            var index = Array.IndexOf(headers, col);
            if (index < 0)
            {
                Console.WriteLine($"Column not found: {col}");
                return;
            }
            indices.Add(index);
        }

        using var writer = outputFile != null 
            ? new StreamWriter(outputFile, false, Encoding.UTF8)
            : Console.Out;

        foreach (var row in data)
        {
            var selected = indices.Select(i => i < row.Length ? row[i] : "").ToList();
            writer.WriteLine(string.Join(",", selected.Select(Escape)));
        }

        if (outputFile != null)
            Console.WriteLine($"✓ Selected {columns.Count} columns to {outputFile}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error: {ex.Message}");
    }
}

void ShowHelp()
{
    Console.WriteLine("""
        CsvMerger - Merge multiple CSV files with schema handling

        Usage:
          dotnet run -- merge <file1.csv> <file2.csv> [options]
          dotnet run -- info <file.csv>
          dotnet run -- split <file.csv> <rows-per-file>
          dotnet run -- select <file.csv> <columns>

        Merge Options:
          --output <file>     Output file (default: merged.csv)
          --mode <mode>       vertical (append rows) or horizontal (append columns)
          --delimiter <char>  CSV delimiter (default: ,)

        Split Options:
          --output <pattern>  Output pattern with {n} placeholder

        Select Options:
          --output <file>     Output file (default: stdout)

        Examples:
          dotnet run -- merge a.csv b.csv -o merged.csv
          dotnet run -- merge a.csv b.csv --mode horizontal
          dotnet run -- info data.csv
          dotnet run -- split data.csv 1000 --output "part_{n}.csv"
          dotnet run -- select data.csv name,email,phone
        """);
}

List<string[]> ReadCsv(string file, char delimiter)
{
    var result = new List<string[]>();

    foreach (var line in File.ReadLines(file))
    {
        if (string.IsNullOrWhiteSpace(line))
            continue;

        result.Add(ParseCsvLine(line, delimiter));
    }

    return result;
}

string[] ParseCsvLine(string line, char delimiter)
{
    var fields = new List<string>();
    var current = new StringBuilder();
    var inQuotes = false;

    for (var i = 0; i < line.Length; i++)
    {
        var c = line[i];

        if (inQuotes)
        {
            if (c == '"')
            {
                if (i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else
                {
                    inQuotes = false;
                }
            }
            else
            {
                current.Append(c);
            }
        }
        else
        {
            if (c == '"')
            {
                inQuotes = true;
            }
            else if (c == delimiter)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }
    }

    fields.Add(current.ToString());
    return fields.ToArray();
}

string Escape(string value)
{
    if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
    {
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
    return value;
}
