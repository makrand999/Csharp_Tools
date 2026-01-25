# PDF Metadata Reader

Extracts and displays metadata from PDF files including title, author, subject, keywords, creator, producer, creation/modification dates, and document statistics.

## Usage

```bash
# Single file (human-readable output)
dotnet run --project PdfMetadataReader.csproj document.pdf

# JSON output
dotnet run --project PdfMetadataReader.csproj --json report.pdf

# Show all fields (including empty)
dotnet run --project PdfMetadataReader.csproj --all document.pdf

# Multiple files
dotnet run --project PdfMetadataReader.csproj file1.pdf file2.pdf file3.pdf
```

## Example

**Output:**
```
============================================================
File: report.pdf
Path: /home/user/documents/report.pdf
============================================================
Title               : Annual Report 2024
Author              : John Smith
Subject             : Financial Summary
Keywords            : finance, annual, report
Creator             : Microsoft Word
Producer            : Adobe PDF Library 15.0
Creation Date       : 20240115103000Z
Modified Date       : 20240115143000Z
Pages               : 42
PDF Version         : 1.7
Encrypted           : False
```

**JSON Output:**
```json
{
  "File": "report.pdf",
  "Path": "/home/user/documents/report.pdf",
  "Metadata": {
    "Title": "Annual Report 2024",
    "Author": "John Smith",
    "Subject": "Financial Summary",
    "Keywords": "finance, annual, report",
    "Creator": "Microsoft Word",
    "Producer": "Adobe PDF Library 15.0",
    "Creation Date": "20240115103000Z",
    "Modified Date": "20240115143000Z",
    "Pages": 42,
    "PDF Version": "1.7",
    "Encrypted": false
  }
}
```

## Concepts Demonstrated

- PDF parsing with PdfSharpCore
- PDF document info extraction
- Dictionary-based data structures
- JSON serialization
- File I/O and path handling
- Command-line argument parsing
- Batch file processing
- Error handling for invalid files
