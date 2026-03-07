# Channel Pipeline

Channel-based pipeline for producer-consumer scenarios using `System.Threading.Channels`. Demonstrates bounded channels, backpressure, and async stream processing.

## Usage

```bash
dotnet run --project ChannelPipeline/ChannelPipeline.csproj
```

## Example

```
=== Channel-Based Pipeline ===

Produced: Item-001
Consumer 0: Processing Item-001
Produced: Item-002
Consumer 1: Processing Item-002
...

=== Pipeline Complete ===
```

## Concepts Demonstrated

- `System.Threading.Channels` namespace
- Bounded vs unbounded channels
- `BoundedChannelFullMode` options (Wait, DropNewest, DropOldest)
- Async streams with `IAsyncEnumerable`
- Backpressure handling
