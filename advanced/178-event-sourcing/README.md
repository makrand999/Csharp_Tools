# Event Sourcing

Event sourcing pattern that stores state changes as immutable events. Rebuild state by replaying events from the beginning.

## Usage

```bash
dotnet run --project 178-event-sourcing/EventSourcing.csproj
```

## Example

```
Event Sourcing Pattern
======================

--- Creating Account ---
  Account: a1b2c3d4 | Owner: John Doe | Balance: $1000.00

--- Making Deposits ---
  Account: a1b2c3d4 | Owner: John Doe | Balance: $1750.00

--- Making Withdrawals ---
  Account: a1b2c3d4 | Owner: John Doe | Balance: $1450.00

--- Attempting Overdraft ---
Rejected: Insufficient funds
  Account: a1b2c3d4 | Owner: John Doe | Balance: $1450.00

--- Event History ---
  [AccountCreatedEvent] {"Owner":"John Doe","InitialBalance":1000}
  [MoneyDepositedEvent] {"Amount":500}
  [MoneyDepositedEvent] {"Amount":250}
  [MoneyWithdrawnEvent] {"Amount":200}
  [MoneyWithdrawnEvent] {"Amount":100}

--- Rebuilding State from Events ---
Rebuilt account matches: True
  Account: a1b2c3d4 | Owner: John Doe | Balance: $1450.00 | v5

--- Event Store Statistics ---
Total streams: 1
Total events: 5
```

## Key Concepts

| Concept | Description |
|---------|-------------|
| **Event Store** | Immutable event log |
| **Aggregate** | Business entity reconstructed from events |
| **Projection** | Building read models from events |
| **Replay** | Rebuilding state by reapplying events |

## Concepts Demonstrated

- Event sourcing fundamentals
- Immutable event log
- State reconstruction
- Event envelopes
- Domain events
- Event-driven architecture
