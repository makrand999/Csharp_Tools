using System.Text.Json;

namespace JsonSchemaValidator;

class Program
{
    static void Main(string[] args)
    {
        if (args.Length < 2)
        {
            Console.WriteLine("JSON Schema Validator");
            Console.WriteLine("Usage: dotnet run --project 134-json-schema-validator.csproj -- <data.json> <schema.json>");
            Console.WriteLine("\nThis tool performs basic JSON schema validation:");
            Console.WriteLine("- Type checking (string, number, boolean, array, object)");
            Console.WriteLine("- Required properties");
            Console.WriteLine("- Min/max values");
            Console.WriteLine("- Pattern matching");
            return;
        }

        string dataFile = args[0];
        string schemaFile = args[1];

        if (!File.Exists(dataFile))
        {
            Console.WriteLine($"Error: Data file not found: {dataFile}");
            return;
        }

        if (!File.Exists(schemaFile))
        {
            Console.WriteLine($"Error: Schema file not found: {schemaFile}");
            return;
        }

        try
        {
            var dataJson = File.ReadAllText(dataFile);
            var schemaJson = File.ReadAllText(schemaFile);

            using var dataDoc = JsonDocument.Parse(dataJson);
            using var schemaDoc = JsonDocument.Parse(schemaJson);

            var errors = ValidateJson(dataDoc.RootElement, schemaDoc.RootElement, "$");

            if (errors.Count == 0)
            {
                Console.WriteLine("✓ JSON is valid according to the schema!");
            }
            else
            {
                Console.WriteLine($"✗ Found {errors.Count} validation error(s):\n");
                foreach (var error in errors)
                {
                    Console.WriteLine($"  {error.Path}: {error.Message}");
                }
            }
        }
        catch (JsonException ex)
        {
            Console.WriteLine($"JSON Parse Error: {ex.Message}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    static List<ValidationError> ValidateJson(JsonElement data, JsonElement schema, string path)
    {
        var errors = new List<ValidationError>();

        // Check type
        if (schema.TryGetProperty("type", out var typeProp))
        {
            string expectedType = typeProp.GetString() ?? "";
            string actualType = GetJsonType(data);

            if (expectedType != actualType)
            {
                errors.Add(new ValidationError
                {
                    Path = path,
                    Message = $"Expected type '{expectedType}', got '{actualType}'"
                });
                return errors; // Can't validate further if type is wrong
            }
        }

        // Check required properties
        if (schema.TryGetProperty("required", out var requiredProp))
        {
            foreach (var req in requiredProp.EnumerateArray())
            {
                string reqProp = req.GetString() ?? "";
                if (!data.TryGetProperty(reqProp, out _))
                {
                    errors.Add(new ValidationError
                    {
                        Path = $"{path}.{reqProp}",
                        Message = $"Missing required property"
                    });
                }
            }
        }

        // Check properties
        if (data.ValueKind == JsonValueKind.Object && 
            schema.TryGetProperty("properties", out var propertiesProp))
        {
            foreach (var propSchema in propertiesProp.EnumerateObject())
            {
                if (data.TryGetProperty(propSchema.Name, out var propValue))
                {
                    var propErrors = ValidateJson(propValue, propSchema.Value, $"{path}.{propSchema.Name}");
                    errors.AddRange(propErrors);
                }
            }
        }

        // Check minimum/maximum for numbers
        if (data.ValueKind == JsonValueKind.Number)
        {
            if (schema.TryGetProperty("minimum", out var minProp))
            {
                if (data.GetDouble() < minProp.GetDouble())
                {
                    errors.Add(new ValidationError
                    {
                        Path = path,
                        Message = $"Value {data.GetDouble()} is less than minimum {minProp.GetDouble()}"
                    });
                }
            }

            if (schema.TryGetProperty("maximum", out var maxProp))
            {
                if (data.GetDouble() > maxProp.GetDouble())
                {
                    errors.Add(new ValidationError
                    {
                        Path = path,
                        Message = $"Value {data.GetDouble()} is greater than maximum {maxProp.GetDouble()}"
                    });
                }
            }
        }

        // Check minLength/maxLength for strings
        if (data.ValueKind == JsonValueKind.String)
        {
            string strValue = data.GetString() ?? "";

            if (schema.TryGetProperty("minLength", out var minLenProp))
            {
                int minLen = minLenProp.GetInt32();
                if (strValue.Length < minLen)
                {
                    errors.Add(new ValidationError
                    {
                        Path = path,
                        Message = $"String length {strValue.Length} is less than minLength {minLen}"
                    });
                }
            }

            if (schema.TryGetProperty("maxLength", out var maxLenProp))
            {
                int maxLen = maxLenProp.GetInt32();
                if (strValue.Length > maxLen)
                {
                    errors.Add(new ValidationError
                    {
                        Path = path,
                        Message = $"String length {strValue.Length} is greater than maxLength {maxLen}"
                    });
                }
            }

            // Check pattern
            if (schema.TryGetProperty("pattern", out var patternProp))
            {
                string pattern = patternProp.GetString() ?? "";
                if (!System.Text.RegularExpressions.Regex.IsMatch(strValue, pattern))
                {
                    errors.Add(new ValidationError
                    {
                        Path = path,
                        Message = $"String does not match pattern '{pattern}'"
                    });
                }
            }
        }

        // Check minItems/maxItems for arrays
        if (data.ValueKind == JsonValueKind.Array)
        {
            int itemCount = data.GetArrayLength();

            if (schema.TryGetProperty("minItems", out var minItemsProp))
            {
                int minItems = minItemsProp.GetInt32();
                if (itemCount < minItems)
                {
                    errors.Add(new ValidationError
                    {
                        Path = path,
                        Message = $"Array has {itemCount} items, minimum is {minItems}"
                    });
                }
            }

            if (schema.TryGetProperty("maxItems", out var maxItemsProp))
            {
                int maxItems = maxItemsProp.GetInt32();
                if (itemCount > maxItems)
                {
                    errors.Add(new ValidationError
                    {
                        Path = path,
                        Message = $"Array has {itemCount} items, maximum is {maxItems}"
                    });
                }
            }
        }

        return errors;
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
}

class ValidationError
{
    public string Path { get; set; } = "";
    public string Message { get; set; } = "";
}
