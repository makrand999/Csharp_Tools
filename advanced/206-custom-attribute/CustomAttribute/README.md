# Custom Attribute System

Demonstrates custom attribute creation and reflection-based processing. Shows how to build metadata-driven systems for validation, serialization, and command registration.

## Usage

```bash
dotnet run --project CustomAttribute.csproj
```

## Example

```
=== Custom Attribute System ===

--- Validation Attributes ---

Validating user: Alice
✓ Validation passed!

--- Serialization Attributes ---

Original object:
  Id: 1
  Name: Widget
  Price: 29.99
  InternalCode: SECRET123
  Description: A useful widget

Serialized JSON:
{
  "product_id": 1,
  "product_name": "Widget",
  "price_usd": 29.99,
  "Description": "A useful widget"
}

--- Command Registration ---

  [add] Result: 8
  [subtract] Result: 6
  [multiply] Result: 42

Registered commands:
  - add: Add two numbers
  - subtract: Subtract two numbers
  - multiply: Multiply two numbers
```

## Concepts Demonstrated

- Custom attribute definition
- AttributeUsage configuration
- Reflection for attribute reading
- Metadata-driven validation
- Custom serialization based on attributes
- Command registration via attributes
- Dynamic method invocation
