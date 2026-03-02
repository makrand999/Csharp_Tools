# IL Generator

Demonstrates dynamic code generation using `System.Reflection.Emit`. Creates types, methods, and IL code at runtime for scenarios requiring high-performance dynamic behavior.

## Usage

```bash
dotnet run --project IlGenerator.csproj
```

## Example

```
=== IL Generator Demo ===

--- Dynamic Method (Add Two Numbers) ---

Result: 7 + 3 = 10

--- Dynamic Type Creation ---

Hello, my name is Alice
Person created dynamically: Name=Alice, Age=30

--- Dynamic Calculator ---

10 + 5 = 15
10 - 5 = 5
10 * 5 = 50
10 / 5 = 2
```

## Concepts Demonstrated

- DynamicMethod for runtime method creation
- AssemblyBuilder for dynamic assemblies
- TypeBuilder for runtime type generation
- ILGenerator for emitting IL opcodes
- Property and field generation
- Stack-based IL programming
