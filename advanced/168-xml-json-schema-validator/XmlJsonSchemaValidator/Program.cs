using System.Text.Json;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Schema;

namespace XmlJsonSchemaValidator;

/// <summary>
/// XML and JSON Schema Validator - demonstrates schema validation, XSD, and JSON Schema
/// Validates XML against XSD and JSON against JSON Schema
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== XML/JSON Schema Validator ===\n");

        // ===== XML/XSD Validation =====
        Console.WriteLine("=== XML Schema (XSD) Validation ===\n");

        // Define XSD schema
        string xsdSchema = """
            <?xml version="1.0" encoding="UTF-8"?>
            <xs:schema xmlns:xs="http://www.w3.org/2001/XMLSchema">
                <xs:element name="Library">
                    <xs:complexType>
                        <xs:sequence>
                            <xs:element name="Book" maxOccurs="unbounded">
                                <xs:complexType>
                                    <xs:sequence>
                                        <xs:element name="Title" type="xs:string"/>
                                        <xs:element name="Author" type="xs:string"/>
                                        <xs:element name="Year" type="xs:int"/>
                                        <xs:element name="Price" type="xs:decimal"/>
                                        <xs:element name="ISBN" type="xs:string" minOccurs="0"/>
                                    </xs:sequence>
                                    <xs:attribute name="id" type="xs:int" use="required"/>
                                    <xs:attribute name="category" use="required">
                                        <xs:simpleType>
                                            <xs:restriction base="xs:string">
                                                <xs:enumeration value="Fiction"/>
                                                <xs:enumeration value="NonFiction"/>
                                                <xs:enumeration value="Technical"/>
                                            </xs:restriction>
                                        </xs:simpleType>
                                    </xs:attribute>
                                </xs:complexType>
                            </xs:element>
                        </xs:sequence>
                    </xs:complexType>
                </xs:element>
            </xs:schema>
            """;

        // Valid XML
        string validXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <Library>
                <Book id="1" category="Technical">
                    <Title>C# in Depth</Title>
                    <Author>Jon Skeet</Author>
                    <Year>2019</Year>
                    <Price>49.99</Price>
                    <ISBN>978-1617294532</ISBN>
                </Book>
                <Book id="2" category="Fiction">
                    <Title>The Hobbit</Title>
                    <Author>J.R.R. Tolkien</Author>
                    <Year>1937</Year>
                    <Price>15.99</Price>
                </Book>
            </Library>
            """;

        // Invalid XML (wrong category, missing required field)
        string invalidXml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <Library>
                <Book id="1" category="InvalidCategory">
                    <Title>Bad Book</Title>
                    <Author>Unknown</Author>
                    <Year>not_a_number</Year>
                    <Price>19.99</Price>
                </Book>
                <Book id="2">
                    <Title>Missing Category</Title>
                    <Author>Someone</Author>
                    <Year>2020</Year>
                    <Price>9.99</Price>
                </Book>
            </Library>
            """;

        Console.WriteLine("1. Validating Valid XML:");
        ValidateXml(validXml, xsdSchema);

        Console.WriteLine("\n2. Validating Invalid XML:");
        ValidateXml(invalidXml, xsdSchema);

        // ===== JSON Schema Validation =====
        Console.WriteLine("\n=== JSON Schema Validation ===\n");

        // Define JSON Schema
        string jsonSchema = """
            {
                "$schema": "http://json-schema.org/draft-07/schema#",
                "title": "User",
                "type": "object",
                "required": ["id", "username", "email"],
                "properties": {
                    "id": {
                        "type": "integer",
                        "minimum": 1
                    },
                    "username": {
                        "type": "string",
                        "minLength": 3,
                        "maxLength": 20,
                        "pattern": "^[a-zA-Z0-9_]+$"
                    },
                    "email": {
                        "type": "string",
                        "format": "email"
                    },
                    "age": {
                        "type": "integer",
                        "minimum": 0,
                        "maximum": 150
                    },
                    "role": {
                        "type": "string",
                        "enum": ["user", "admin", "moderator"]
                    },
                    "tags": {
                        "type": "array",
                        "items": {
                            "type": "string"
                        },
                        "minItems": 1,
                        "uniqueItems": true
                    },
                    "profile": {
                        "type": "object",
                        "properties": {
                            "bio": { "type": "string", "maxLength": 500 },
                            "website": { "type": "string", "format": "uri" }
                        }
                    }
                },
                "additionalProperties": false
            }
            """;

        // Valid JSON
        string validJson = """
            {
                "id": 42,
                "username": "john_doe",
                "email": "john@example.com",
                "age": 30,
                "role": "admin",
                "tags": ["developer", "csharp"],
                "profile": {
                    "bio": "A passionate developer",
                    "website": "https://example.com"
                }
            }
            """;

        // Invalid JSON (multiple violations)
        string invalidJson = """
            {
                "id": -5,
                "username": "ab",
                "email": "not-an-email",
                "age": 200,
                "role": "superuser",
                "tags": [],
                "extraField": "not allowed"
            }
            """;

        Console.WriteLine("3. Validating Valid JSON:");
        ValidateJson(validJson, jsonSchema);

        Console.WriteLine("\n4. Validating Invalid JSON:");
        ValidateJson(invalidJson, jsonSchema);

        // ===== Schema Analysis =====
        Console.WriteLine("\n=== Schema Analysis ===\n");
        AnalyzeJsonSchema(jsonSchema);
    }

    static void ValidateXml(string xml, string xsd)
    {
        var settings = new XmlReaderSettings();
        settings.ValidationType = ValidationType.Schema;
        
        using (var schemaReader = new StringReader(xsd))
        using (var schemaXmlReader = XmlReader.Create(schemaReader))
        {
            settings.Schemas.Add(null, schemaXmlReader);
        }

        var validationErrors = new List<ValidationEventArgs>();
        settings.ValidationEventHandler += (sender, e) =>
        {
            validationErrors.Add(e);
            Console.WriteLine($"   [{e.Severity}] {e.Message}");
        };

        try
        {
            using (var stringReader = new StringReader(xml))
            using (var reader = XmlReader.Create(stringReader, settings))
            {
                while (reader.Read()) { }
            }

            if (validationErrors.Count == 0)
            {
                Console.WriteLine("   ✓ XML is valid against schema");
            }
            else
            {
                Console.WriteLine($"   ✗ Found {validationErrors.Count} validation error(s)");
            }
        }
        catch (XmlException ex)
        {
            Console.WriteLine($"   ✗ XML Parse Error: {ex.Message}");
        }
    }

    static void ValidateJson(string json, string schemaJson)
    {
        using var jsonDoc = JsonDocument.Parse(json);
        using var schemaDoc = JsonDocument.Parse(schemaJson);
        
        var schema = schemaDoc.RootElement;
        var data = jsonDoc.RootElement;
        
        var errors = ValidateJsonElement(data, schema, "$");
        
        if (errors.Count == 0)
        {
            Console.WriteLine("   ✓ JSON is valid against schema");
        }
        else
        {
            Console.WriteLine($"   ✗ Found {errors.Count} validation error(s):");
            foreach (var error in errors)
            {
                Console.WriteLine($"      - {error.Path}: {error.Message}");
            }
        }
    }

    static List<ValidationError> ValidateJsonElement(JsonElement element, JsonElement schema, string path)
    {
        var errors = new List<ValidationError>();

        // Check type
        if (schema.TryGetProperty("type", out var typeProp))
        {
            var expectedType = typeProp.GetString();
            var actualType = GetJsonType(element);
            
            if (expectedType != actualType)
            {
                errors.Add(new ValidationError(path, $"Expected type '{expectedType}' but got '{actualType}'"));
                return errors; // Type mismatch, skip further validation
            }
        }

        // Validate based on type
        switch (element.ValueKind)
        {
            case JsonValueKind.String:
                ValidateString(element, schema, path, errors);
                break;
            case JsonValueKind.Number:
                ValidateNumber(element, schema, path, errors);
                break;
            case JsonValueKind.Array:
                ValidateArray(element, schema, path, errors);
                break;
            case JsonValueKind.Object:
                ValidateObject(element, schema, path, errors);
                break;
        }

        return errors;
    }

    static void ValidateString(JsonElement element, JsonElement schema, string path, List<ValidationError> errors)
    {
        var value = element.GetString()!;

        if (schema.TryGetProperty("minLength", out var minLength))
        {
            if (value.Length < minLength.GetInt32())
            {
                errors.Add(new ValidationError(path, $"String length {value.Length} is less than minimum {minLength.GetInt32()}"));
            }
        }

        if (schema.TryGetProperty("maxLength", out var maxLength))
        {
            if (value.Length > maxLength.GetInt32())
            {
                errors.Add(new ValidationError(path, $"String length {value.Length} exceeds maximum {maxLength.GetInt32()}"));
            }
        }

        if (schema.TryGetProperty("pattern", out var pattern))
        {
            var regex = new Regex(pattern.GetString()!);
            if (!regex.IsMatch(value))
            {
                errors.Add(new ValidationError(path, $"String does not match pattern '{pattern.GetString()}'"));
            }
        }

        if (schema.TryGetProperty("format", out var format))
        {
            var formatValue = format.GetString();
            if (formatValue == "email" && !IsValidEmail(value))
            {
                errors.Add(new ValidationError(path, "Invalid email format"));
            }
            if (formatValue == "uri" && !IsValidUri(value))
            {
                errors.Add(new ValidationError(path, "Invalid URI format"));
            }
        }
    }

    static void ValidateNumber(JsonElement element, JsonElement schema, string path, List<ValidationError> errors)
    {
        var value = element.GetDouble();

        if (schema.TryGetProperty("minimum", out var minimum))
        {
            if (value < minimum.GetDouble())
            {
                errors.Add(new ValidationError(path, $"Value {value} is less than minimum {minimum.GetDouble()}"));
            }
        }

        if (schema.TryGetProperty("maximum", out var maximum))
        {
            if (value > maximum.GetDouble())
            {
                errors.Add(new ValidationError(path, $"Value {value} exceeds maximum {maximum.GetDouble()}"));
            }
        }
    }

    static void ValidateArray(JsonElement element, JsonElement schema, string path, List<ValidationError> errors)
    {
        var count = element.GetArrayLength();

        if (schema.TryGetProperty("minItems", out var minItems))
        {
            if (count < minItems.GetInt32())
            {
                errors.Add(new ValidationError(path, $"Array has {count} items, minimum is {minItems.GetInt32()}"));
            }
        }

        if (schema.TryGetProperty("maxItems", out var maxItems))
        {
            if (count > maxItems.GetInt32())
            {
                errors.Add(new ValidationError(path, $"Array has {count} items, maximum is {maxItems.GetInt32()}"));
            }
        }

        if (schema.TryGetProperty("uniqueItems", out var uniqueItems) && uniqueItems.GetBoolean())
        {
            var items = new HashSet<string>();
            foreach (var item in element.EnumerateArray())
            {
                var itemStr = item.ToString();
                if (!items.Add(itemStr))
                {
                    errors.Add(new ValidationError(path, "Array contains duplicate items"));
                    break;
                }
            }
        }

        if (schema.TryGetProperty("items", out var itemsSchema))
        {
            int index = 0;
            foreach (var item in element.EnumerateArray())
            {
                var itemErrors = ValidateJsonElement(item, itemsSchema, $"{path}[{index}]");
                errors.AddRange(itemErrors);
                index++;
            }
        }
    }

    static void ValidateObject(JsonElement element, JsonElement schema, string path, List<ValidationError> errors)
    {
        // Check required properties
        if (schema.TryGetProperty("required", out var requiredProp))
        {
            foreach (var requiredField in requiredProp.EnumerateArray())
            {
                var fieldName = requiredField.GetString()!;
                if (!element.TryGetProperty(fieldName, out _))
                {
                    errors.Add(new ValidationError(path, $"Missing required property '{fieldName}'"));
                }
            }
        }

        // Validate properties
        if (schema.TryGetProperty("properties", out var propertiesSchema))
        {
            foreach (var prop in element.EnumerateObject())
            {
                if (propertiesSchema.TryGetProperty(prop.Name, out var propSchema))
                {
                    var propErrors = ValidateJsonElement(prop.Value, propSchema, $"{path}.{prop.Name}");
                    errors.AddRange(propErrors);
                }
            }
        }

        // Check additionalProperties
        if (schema.TryGetProperty("additionalProperties", out var additionalProp))
        {
            if (additionalProp.ValueKind == JsonValueKind.False)
            {
                var allowedProps = new HashSet<string>();
                if (schema.TryGetProperty("properties", out var props))
                {
                    foreach (var prop in props.EnumerateObject())
                    {
                        allowedProps.Add(prop.Name);
                    }
                }

                foreach (var prop in element.EnumerateObject())
                {
                    if (!allowedProps.Contains(prop.Name))
                    {
                        errors.Add(new ValidationError(path, $"Additional property '{prop.Name}' is not allowed"));
                    }
                }
            }
        }

        // Check enum
        if (schema.TryGetProperty("enum", out var enumProp))
        {
            var valueStr = element.ToString();
            var found = false;
            foreach (var enumValue in enumProp.EnumerateArray())
            {
                if (enumValue.ToString() == valueStr)
                {
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                errors.Add(new ValidationError(path, $"Value '{valueStr}' is not one of the allowed enum values"));
            }
        }
    }

    static string GetJsonType(JsonElement element)
    {
        return element.ValueKind switch
        {
            JsonValueKind.String => "string",
            JsonValueKind.Number => "number",
            JsonValueKind.True or JsonValueKind.False => "boolean",
            JsonValueKind.Array => "array",
            JsonValueKind.Object => "object",
            JsonValueKind.Null => "null",
            _ => "unknown"
        };
    }

    static bool IsValidEmail(string email)
    {
        return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
    }

    static bool IsValidUri(string uri)
    {
        return Uri.TryCreate(uri, UriKind.Absolute, out _);
    }

    static void AnalyzeJsonSchema(string schemaJson)
    {
        using var schemaDoc = JsonDocument.Parse(schemaJson);
        var schema = schemaDoc.RootElement;

        Console.WriteLine("Schema Details:");
        
        if (schema.TryGetProperty("title", out var title))
        {
            Console.WriteLine($"   Title: {title.GetString()}");
        }

        if (schema.TryGetProperty("type", out var type))
        {
            Console.WriteLine($"   Root Type: {type.GetString()}");
        }

        if (schema.TryGetProperty("required", out var required))
        {
            Console.Write("   Required Fields: ");
            var fields = required.EnumerateArray().Select(p => p.GetString()).ToList();
            Console.WriteLine(string.Join(", ", fields));
        }

        if (schema.TryGetProperty("properties", out var properties))
        {
            Console.WriteLine("   Properties:");
            foreach (var prop in properties.EnumerateObject())
            {
                var propType = prop.Value.TryGetProperty("type", out var t) ? t.GetString() : "any";
                Console.WriteLine($"      - {prop.Name}: {propType}");
            }
        }
    }
}

class ValidationError
{
    public string Path { get; }
    public string Message { get; }

    public ValidationError(string path, string message)
    {
        Path = path;
        Message = message;
    }
}
