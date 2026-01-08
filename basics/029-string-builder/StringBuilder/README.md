# StringBuilder: Efficient String Operations

Demonstrates the `StringBuilder` class for efficient string manipulation, especially useful for loops and large text generation.

## Usage

```bash
dotnet run --project StringBuilder.csproj
```

## Example

```
=== StringBuilder: Efficient String Operations ===

--- String Concatenation vs StringBuilder ---
String concatenation (10000 iterations): 150ms
StringBuilder (10000 iterations): 2ms

--- Basic StringBuilder Operations ---
Content: Hello World!
C# is awesome
Length: 32
Capacity: 256

--- Pre-allocated Capacity ---
Initial capacity: 1000
After 50 appends: 200 chars, capacity: 1000

--- Modify Operations ---
Original: Hello World!
After Insert: Hello Beautiful World!
After Remove: Hello World!
After Replace: Hello Universe!

--- Building CSV Content ---
ID,Name,Email,Age
1,Alice,alice@example.com,30
2,Bob,bob@example.com,25
3,Charlie,charlie@example.com,35
```

## Concepts Demonstrated

- StringBuilder vs string concatenation performance
- Append, AppendLine, AppendFormat methods
- Capacity management and pre-allocation
- Insert, Remove, Replace operations
- Character indexing and modification
- ToString with range parameters
- Clear and reuse patterns
- Building structured content (CSV, HTML)
- Conditional content building
- Large text generation efficiency
