# MessagePack Serializer

MessagePack serializer demonstrating binary serialization with the MessagePack library. Fast, compact binary serialization format similar to JSON but more efficient.

## Usage

```bash
dotnet run --project MessagePackSerializer.csproj
```

## Example

```
=== MessagePack Serializer ===

1. Basic Serialization:
   Original: John Doe, Age 35
   Serialized size: 52 bytes
   Hex dump: 85-00-2A-01-A8-4A-6F-68-6E-20-44-6F-65-02-B0-6A-6F-68-6E-40-65...

2. Deserialization:
   Deserialized: John Doe, Age 35
   Match: True

3. Nested Objects:
   Order #1001 with 2 items
   Serialized size: 168 bytes
   Deserialized: Order #1001, Customer: John Doe

4. Collections:
   Company: Tech Corp
   Employees: 3, Departments: 3
   Serialized size: 186 bytes
   Deserialized employees: Alice, Bob, Charlie

5. Union Types (Polymorphic Serialization):
   Serialized 3 shapes: 98 bytes
   Deserialized shapes:
      - Circle: Area = 78.54, Color = Red
      - Rectangle: Area = 200.00, Color = Blue
      - Triangle: Area = 24.00, Color = Green

6. LZ4 Compression:
   Uncompressed size: 10234 bytes
   LZ4 compressed size: 156 bytes
   Compression ratio: 1.52%
   Space savings: 98.5%

7. Size Comparison (MessagePack vs JSON vs Protobuf):
   MessagePack: 156 bytes
   JSON: 245 bytes
   MessagePack is 36.3% smaller than JSON

8. Performance Benchmark (10000 iterations):
   MessagePack: 198ms (50505 ops/sec)
   JSON: 512ms (19531 ops/sec)

9. Dynamic Type Serialization:
   Dynamic object serialized: 42 bytes
   Deserialized type: System.Object[]

10. MessagePack Format Types:
   Integer (positive fixint): 0x2A (Positive Fixint), 2 bytes
   Integer (negative fixint): 0xFB (Negative Fixint), 2 bytes
   Float: 0xCB (Float 64), 9 bytes
   Boolean: 0xC3 (True), 1 bytes
   String: 0xA5 (Fixstr), 6 bytes
   Binary: 0xC4 (Bin8), 5 bytes
```

## Concepts Demonstrated

- MessagePack binary serialization
- `[MessagePackObject]` and `[Key]` attributes
- Property key numbering
- Nested object serialization
- Collection serialization (List<T>, arrays)
- Dictionary serialization
- Union types for polymorphic serialization (`[Union]` attribute)
- LZ4 compression for large data
- Size comparison with JSON
- Performance comparison with JSON
- MessagePack format types (fixint, fixstr, bin, array, map)
- Dynamic/anonymous type serialization
- DateTime serialization
- Nullable property handling

## Key Benefits of MessagePack

1. **Compact**: Binary format smaller than JSON
2. **Fast**: Efficient serialization/deserialization
3. **Schema-less**: No pre-compilation required (unlike Protobuf)
4. **Cross-platform**: Supported in many languages
5. **LZ4 Compression**: Built-in compression support
6. **Type safety**: Attribute-based contracts
