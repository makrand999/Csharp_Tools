# Repository Pattern

Repository pattern with Unit of Work for clean data access abstraction. Decouples business logic from data access concerns.

## Usage

```bash
dotnet run --project 180-repository-pattern/RepositoryPattern.csproj
```

## Example

```
Repository Pattern with Unit of Work
====================================

--- Adding Users ---

--- All Users ---

  [1] Alice <alice@example.com>
  [2] Bob <bob@example.com>
  [3] Charlie <charlie@example.com>

--- Find by Email ---

  Found: Alice <alice@example.com>

--- Adding Orders ---

--- Orders with Specification ---

Pending Orders:
  Order #1 - User 1 - $99.99
  Order #3 - User 2 - $29.99

High Value Orders (>$100):
  Order #2 - User 1 - $149.50

--- Transactional Update ---

  Transaction started
  Transaction committed

--- Final State ---

  Alice: 2 orders
  Bob: 1 orders
  Charlie: 0 orders

--- Repository Pattern Benefits ---
✓ Abstracts data access logic
✓ Enables unit testing with mocks
✓ Centralizes data access code
✓ Unit of Work ensures consistency
```

## Components

| Component | Purpose |
|-----------|---------|
| **Repository<T>** | Generic data access abstraction |
| **UnitOfWork** | Coordinates multiple repositories |
| **Specification** | Encapsulates query logic |
| **Transaction** | Ensures atomic operations |

## Concepts Demonstrated

- Generic repository pattern
- Unit of Work pattern
- Specification pattern
- Transaction management
- Dependency inversion
- Testable data access layer
