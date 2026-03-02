# Unsafe Code

Demonstrates unsafe code operations in C# including pointer manipulation, stack allocation, and direct memory access. Useful for performance-critical code and interop scenarios.

## Usage

```bash
dotnet run --project UnsafeCode.csproj
```

**Note:** This project requires unsafe code compilation. The `.csproj` file includes `<AllowUnsafeBlocks>true</AllowUnsafeBlocks>`.

## Example

```
=== Unsafe Code Demo ===

--- Pointer Arithmetic ---

Value: 42
Address: 7FFC8A2B4D50
Dereferenced: 42
ptr + 1 address: 7FFC8A2B4D54
Address difference: 4 bytes

Array via pointer:
  array[0] = 10
  array[1] = 20
  array[2] = 30
  array[3] = 40
  array[4] = 50

--- Stack Allocation (stackalloc) ---

Stack-allocated buffer:
  buffer[0] = 0
  buffer[1] = 1
  buffer[2] = 4
  ...

--- Fixed Statement (Pinning) ---

Pinned string: Hello, Unsafe World!
First char address: 7FFC8A1B5C40
```

## Concepts Demonstrated

- Pointer types and dereferencing
- Pointer arithmetic
- stackalloc for stack memory
- fixed statement for pinning
- Unsafe context
- Direct memory manipulation
- Performance-critical patterns
