# Reactive Stream

Reactive stream processing with Rx-like patterns. Demonstrates observable sequences, stream operators, and backpressure handling for event-driven architectures.

## Usage

```bash
dotnet run --project ReactiveStream/ReactiveStream.csproj
```

## Example

```
=== Reactive Stream Processor ===

Publishing numbers 1-20...

[Pipeline] Received: 20 (total: 1)
[Pipeline] Received: 40 (total: 2)
[Pipeline] Received: 60 (total: 3)
...

[Pipeline] Completed. Total items: 10
```

## Concepts Demonstrated

- Observer/Observable patterns
- Stream operators (Filter, Map, Take, Distinct)
- Buffer and flatten operations
- Stream merging
- Backpressure handling
- Functional reactive programming
