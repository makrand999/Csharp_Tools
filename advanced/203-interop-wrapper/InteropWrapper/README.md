# Interop Wrapper

Demonstrates P/Invoke and interop wrappers for calling native code safely from managed C#. Shows memory marshaling, unmanaged memory operations, and safe wrapper patterns.

## Usage

```bash
dotnet run --project InteropWrapper.csproj
```

## Example

```
=== Interop Wrapper Demo ===

--- Platform Information ---

OS: Unix
OS Version: 5.15.0.0
Process Architecture: X64
OS Architecture: X64
Framework Description: .NET 8.0.0
Is 64-bit: True
Native library: libc.so.6

--- Memory Operations ---

Unmanaged memory test:
  Int32 at offset 0: 42
  Int64 at offset 4: 123456789

String marshaling:
  Original: Hello, Native World!
  Round-trip: Hello, Native World!

--- Safe Native Wrapper ---

Buffer content: Hello from C#!
Buffer size: 256 bytes
```

## Concepts Demonstrated

- P/Invoke and native interop
- Marshal class for memory operations
- Unmanaged memory allocation/deallocation
- String marshaling between managed and native
- Safe wrapper pattern with IDisposable
- Platform-specific code with RuntimeInformation
