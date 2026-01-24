# XML to JSON Converter

Converts XML documents to JSON format with proper structure preservation including attributes and nested elements.

## Usage

```bash
# From stdin
echo '<root><item id="1">Value</item></root>' | dotnet run --project XmlToJson.csproj

# From file
dotnet run --project XmlToJson.csproj input.xml
```

## Example

**Input XML:**
```xml
<book id="123">
  <title>Learn C#</title>
  <author>John Doe</author>
  <price currency="USD">29.99</price>
</book>
```

**Output JSON:**
```json
{
  "book": {
    "@id": "123",
    "title": "Learn C#",
    "author": "John Doe",
    "price": {
      "@currency": "USD",
      "#text": "29.99"
    }
  }
}
```

## Concepts Demonstrated

- XML parsing with LINQ to XML (XDocument, XElement)
- JSON serialization with System.Text.Json
- Recursive tree traversal
- Dictionary-based data structures
- Attribute handling (@prefix for XML attributes)
- Text content extraction (#text for mixed content)
- File I/O and stdin reading
