# Backpressure Queue

Backpressure queue with flow control for handling producer-consumer rate mismatches. Demonstrates bounded queues, watermarks, and adaptive rate limiting.

## Usage

```bash
dotnet run --project BackpressureQueue/BackpressureQueue.csproj
```

## Example

```
=== Backpressure Queue ===

Produced: Item-001 (queue: 1)
Produced: Item-002 (queue: 2)
Consumed: Item-001 (queue: 1)
Produced: Item-003 (queue: 2)
Backpressure! Waiting before producing Item-010
...

Statistics:
  Items produced: 50
  Items consumed: 50
  Backpressure events: 5
  Max queue size reached: 8
```

## Concepts Demonstrated

- Backpressure patterns
- High/low watermarks
- Bounded queues
- Flow control mechanisms
- Producer-consumer rate limiting
