# CQRS Pattern

Command Query Responsibility Segregation pattern separating read and write operations for better scalability and maintainability.

## Usage

```bash
dotnet run --project 179-cqrs-pattern/CqrsPattern.csproj
```

## Example

```
CQRS Pattern Implementation
===========================

--- Creating Products (Commands) ---

  Created product: Laptop
  Created product: Mouse
  Created product: Keyboard
  Updated price for product 1 to $899.99
  Updated stock for product 2 by -5

--- Querying Products (Queries) ---

All Products:
  [1] Laptop - $899.99 (Stock: 10)
  [2] Mouse - $29.99 (Stock: 45)
  [3] Keyboard - $79.99 (Stock: 30)

Product by ID:
  [1] Laptop - $899.99 (Stock: 10)

Products under $50:
  [2] Mouse - $29.99 (Stock: 45)

--- CQRS Benefits ---
✓ Commands (writes) can be optimized for consistency
✓ Queries (reads) can be optimized for performance
✓ Read and write models can scale independently
✓ Different data stores can be used for reads/writes
```

## Components

| Component | Purpose |
|-----------|---------|
| **Commands** | Write operations (Create, Update, Delete) |
| **Queries** | Read operations (Get, List, Search) |
| **CommandBus** | Routes commands to handlers |
| **QueryBus** | Routes queries to handlers |

## Concepts Demonstrated

- CQRS pattern fundamentals
- Command/Query separation
- Mediator pattern (Command/Query Bus)
- Read/Write model separation
- DTOs for read models
- Domain entities for write models
