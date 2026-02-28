# Message Queue Simulator

In-memory message queue system with pub/sub support, priority queues, dead letter handling, and scheduled message processing.

## Usage

```bash
# Run demo
dotnet run --project MessageQueue.csproj -- demo

# Run interactive mode
dotnet run --project MessageQueue.csproj -- interactive
```

## Example

```
=== Message Queue Simulator Demo ===

1. Basic Queue Operations
-------------------------
Queue depth: 3
Total messages processed: 3

2. Topic-based Publishing
-------------------------
Publishing messages:
  [Subscriber 1] Received on order-created: {"orderId": 1003, "customer": "Charlie"}
  [Subscriber 2] Order created: {"orderId": 1003, "customer": "Charlie"}
  [Subscriber 1] Received on order-shipped: {"orderId": 1003, "tracking": "XYZ789"}

3. Priority Queue
-----------------
Dequeuing in priority order:
  [Critical] DATABASE DOWN!
  [High] URGENT: Server down!
  [Normal] Normal task 1
  [Normal] Normal task 2
  [Low] Cleanup task

4. Dead Letter Queue (Failed Messages)
--------------------------------------
Processing messages (some will fail):
  Processed: Data to process
  Processed: More data
  Failed to process: Simulated failure

Dead letter queue size: 1
```

## Concepts Demonstrated

- ConcurrentQueue for thread-safe messaging
- Publish-Subscribe pattern with wildcard topics
- Priority queue implementation
- Dead letter queue for failed messages
- Retry logic with failure counting
- Scheduled/delayed message processing
- Message statistics and throughput tracking
