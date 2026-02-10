# XML Formatter

Pretty-print or minify XML documents.

## Usage

```bash
dotnet run --project 137-xml-formatter.csproj -- <input.xml> [output.xml]
cat input.xml | dotnet run --project 137-xml-formatter.csproj
```

## Example

```bash
dotnet run --project 137-xml-formatter.csproj -- config.xml formatted.xml
dotnet run --project 137-xml-formatter.csproj -- data.xml --minify
dotnet run --project 137-xml-formatter.csproj -- data.xml --indent 4
```

## Options

- `--minify` - Remove all whitespace
- `--indent N` - Set indentation spaces (default: 2)

## Concepts Demonstrated

- XML parsing with System.Xml
- XmlWriter settings
- Document formatting
- Error handling for invalid XML
- File I/O
