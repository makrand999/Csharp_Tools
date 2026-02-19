# Protocol Buffers Serializer

Protocol Buffers encoder/decoder demonstrating binary serialization with protobuf-net. Efficient binary serialization format with schema-based contracts.

## Usage

```bash
dotnet run --project ProtobufSerializer.csproj
```

## Example

```
=== Protocol Buffers Serializer ===

1. Basic Serialization:
   Original: John Doe, Age 35
   Serialized size: 42 bytes
   Hex dump: 08-2A-12-08-4A-6F-68-6E-20-44-6F-65-1A-10-6A-6F-68-6E-40-65...

2. Deserialization:
   Deserialized: John Doe, Age 35
   Match: True

3. Nested Objects:
   Order #1001 with 2 items
   Serialized size: 156 bytes
   Deserialized: Order #1001, Customer: John Doe

4. Collections:
   Company: Tech Corp
   Employees: 3, Departments: 3
   Serialized size: 178 bytes
   Deserialized employees: Alice, Bob, Charlie

5. Optional/Nullable Fields:
   Partial person (missing Email, Age, IsActive): 18 bytes
   Deserialized: Name='Anonymous', Email='', Age=0

6. Size Comparison (Protobuf vs JSON):
   Protobuf size: 198 bytes
   JSON size: 312 bytes
   Size ratio: 63.46%
   Savings: 36.5% smaller

7. Runtime Model (No Attributes):
   Registered dynamic type: DynamicRecord
   Serialized dynamic object: 45 bytes
   Deserialized: Id=123, Value='Dynamic Value'

8. Performance Benchmark (10000 iterations):
   Protobuf: 245ms (40816 ops/sec)
   JSON: 512ms (19531 ops/sec)
```

## Concepts Demonstrated

- Protocol Buffers binary serialization
- `[ProtoContract]` and `[ProtoMember]` attributes
- Field numbering and schema evolution
- Nested object serialization
- Collection serialization (List<T>, arrays)
- Nullable/optional fields
- Dictionary serialization
- DateTime serialization
- Size comparison with JSON
- Performance comparison with JSON
- Runtime model configuration
- Stream-based serialization/deserialization
- Binary format efficiency
- Backward/forward compatibility

## Key Benefits of Protocol Buffers

1. **Smaller size**: Binary format is more compact than JSON/XML
2. **Faster serialization**: Optimized binary encoding
3. **Strong typing**: Schema-based contracts
4. **Version tolerance**: Field numbers enable evolution
5. **Cross-platform**: Supported in many languages
