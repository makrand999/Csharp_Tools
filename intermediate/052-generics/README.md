# Generic Repository Utility

A type-safe generic repository demonstrating C# generics with practical CRUD operations for any entity type.

## Usage

```bash
dotnet run --project Generics.csproj
```

## Features

- **Type-safe storage**: Generic repository works with any entity implementing `IHasId`
- **CRUD operations**: Add, Get, Update, Delete entities
- **Filtering**: Find entities using predicate functions
- **Auto-increment IDs**: Automatic ID assignment for new entities

## Example Output

```
=== Employee Repository ===
Total employees: 5

All employees:
  1: Alice (Engineering) - $85,000
  2: Bob (Marketing) - $65,000
  3: Charlie (Engineering) - $92,000
  4: Diana (Sales) - $71,000
  5: Eve (Marketing) - $58,000

Find employee with ID 3:
  Found: Charlie - Engineering

Filter by department (Engineering):
  Charlie - $92,000
  Alice - $85,000

=== Product Repository ===
Total products: 4

Expensive products (> $100):
  Laptop - $999.99
  Monitor - $349.99
```

## Concepts Demonstrated

- Generic classes with type constraints (`where T : class, IHasId`)
- Generic methods with `Func<T, bool>` predicates
- Interface implementation for constraints
- Record types for immutable data
- Dictionary-based storage
- Reflection for auto-increment IDs
- Type-safe collections
