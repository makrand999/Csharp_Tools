# XmlTransformer

Transform XML documents using XSLT stylesheets. Validate, pretty-print, and analyze XML files from the command line.

## Usage

```bash
dotnet run --project XmlTransform.csproj -- transform <xml-file> <xslt-file> [output-file]
dotnet run --project XmlTransform.csproj -- transform <xml-file> --inline '<xslt>'
dotnet run --project XmlTransform.csproj -- validate <xml-file>
dotnet run --project XmlTransform.csproj -- pretty <xml-file> [output-file]
dotnet run --project XmlTransform.csproj -- pretty --stdin
dotnet run --project XmlTransform.csproj -- info <xml-file>
```

## Commands

| Command | Description |
|---------|-------------|
| `transform` | Apply XSLT stylesheet to XML |
| `validate` | Check if XML is well-formed |
| `pretty` | Format/indent XML for readability |
| `info` | Show XML structure information |

## Options

### Transform Options
- `--output, -o <file>` - Save output to file
- `--inline '<xslt>'` - Provide XSLT inline

## Examples

### Transform XML to HTML

**data.xml:**
```xml
<?xml version="1.0"?>
<catalog>
  <book>
    <title>Book 1</title>
    <author>Author A</author>
  </book>
  <book>
    <title>Book 2</title>
    <author>Author B</author>
  </book>
</catalog>
```

**stylesheet.xsl:**
```xml
<?xml version="1.0"?>
<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform">
  <xsl:template match="/">
    <html>
      <body>
        <h1>Book Catalog</h1>
        <ul>
          <xsl:for-each select="catalog/book">
            <li><xsl:value-of select="title"/> by <xsl:value-of select="author"/></li>
          </xsl:for-each>
        </ul>
      </body>
    </html>
  </xsl:template>
</xsl:stylesheet>
```

```bash
# Transform file
dotnet run --project XmlTransform.csproj -- transform data.xml stylesheet.xsl output.html

# Inline XSLT
dotnet run --project XmlTransform.csproj -- transform data.xml --inline '<xsl:stylesheet version="1.0" xmlns:xsl="http://www.w3.org/1999/XSL/Transform"><xsl:template match="/"><xsl:copy-of select="."/></xsl:template></xsl:stylesheet>'
```

### Validate XML
```bash
dotnet run --project XmlTransform.csproj -- validate data.xml
```

### Pretty Print XML
```bash
# Format file
dotnet run --project XmlTransform.csproj -- pretty data.xml formatted.xml

# Format from stdin
cat data.xml | dotnet run --project XmlTransform.csproj -- pretty --stdin
```

### Show XML Info
```bash
dotnet run --project XmlTransform.csproj -- info data.xml
```

**Sample Output:**
```
XML Information: data.xml
--------------------------------------------------
File size: 1,234 bytes
Root element: <catalog>
Total elements: 7
Total attributes: 0
Max depth: 3
Namespaces: 0

Root element children:
  <book> (0 attrs)
```

## Concepts Demonstrated

- System.Xml for XML parsing
- XslCompiledTransform for XSLT processing
- XmlReader/XmlWriter for streaming XML
- XmlDocument for DOM-based processing
- XML validation and error handling
- Tree traversal algorithms
