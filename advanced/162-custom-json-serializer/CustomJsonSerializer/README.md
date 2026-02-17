# Custom JSON Serializer

Custom JSON serializer implementation using reflection - demonstrates serialization/deserialization without relying on System.Text.Json.

## Usage

```bash
dotnet run --project CustomJsonSerializer.csproj
```

## Example

```
=== Custom JSON Serializer ===

1. Simple Object Serialization:
{"Name":"John Doe","Age":35,"Email":"john@example.com","IsActive":true}

2. Deserialization:
Name: Jane Smith, Age: 28

3. Nested Object Serialization:
{"OrderId":12345,"Customer":{"Name":"John Doe","Age":35,"Email":"john@example.com","IsActive":true},"Items":["Laptop","Mouse","Keyboard"],"Total":1299.99,"OrderDate":"2025-03-31T14:30:45.1234567Z"}

4. Array/List Serialization:
{"Name":"Tech Corp","Employees":[{"Name":"Alice","Age":30,"Email":"alice@corp.com","IsActive":true},{"Name":"Bob","Age":25,"Email":"bob@corp.com","IsActive":true},{"Name":"Charlie","Age":35,"Email":"charlie@corp.com","IsActive":false}],"Departments":["Engineering","Sales","Marketing"]}

5. Dictionary Serialization:
{"Settings":{"ConnectionString":"Server=localhost;Database=MyDb","MaxConnections":100,"EnableCache":true,"Timeout":30.5}}

6. Null Value Handling:
{"Name":"Unknown","Age":0,"Email":null,"IsActive":false}

7. Special Characters:
{"Message":"Hello\nWorld\t!","Path":"C:\\Users\\Test","Quote":"He said \"Hi\""}

8. Pretty Printed JSON:
{
  "OrderId": 12345,
  "Customer": {
    "Name": "John Doe",
    "Age": 35,
    "Email": "john@example.com",
    "IsActive": true
  },
  "Items": [
    "Laptop",
    "Mouse",
    "Keyboard"
  ],
  "Total": 1299.99,
  "OrderDate": "2025-03-31T14:30:45.1234567Z"
}
```

## Concepts Demonstrated

- Reflection for property inspection (`GetProperties`, `GetValue`, `SetValue`)
- Custom serialization logic
- Type handling (primitives, strings, DateTime, enums)
- Collection serialization (arrays, lists)
- Dictionary serialization
- Nested object handling
- String escaping (quotes, newlines, tabs, backslashes)
- JSON parsing and deserialization
- Nullable type handling
- Pretty printing with indentation
- Generic type constraints
