# Producer-Consumer Queue

Thread-safe queue implementation with async support. Demonstrates three approaches: BlockingCollection, Channel<T>, and custom ConcurrentQueue + Semaphore implementation.

## Usage

```bash
dotnet run --project ProducerConsumerQueue.csproj
```

## Example

```
=== Producer-Consumer Queue ===
Demonstrating thread-safe queue implementations.

Choose implementation:
1. BlockingCollection (Thread-safe blocking queue)
2. Channel<T> (Async producer-consumer)
3. ConcurrentQueue + Semaphore (Custom implementation)
4. Run All Demos

Selection (1-4): 1

--- BlockingCollection Demo ---

Produced: 1
Produced: 2
Consumed: 1
Produced: 3
Consumed: 2
Producer finished.
Consumer finished.
```

## Concepts Demonstrated

- **BlockingCollection<T>** - Thread-safe blocking/bounded collections
- **Channel<T>** - Modern async producer-consumer pattern
- **ConcurrentQueue<T>** - Lock-free thread-safe queue
- **SemaphoreSlim** - Counting semaphore for coordination
- **Bounded Capacity** - Backpressure in producer-consumer scenarios
- **GetConsumingEnumerable** - Blocking consumption pattern
- **CompleteAdding** - Signaling end of production
