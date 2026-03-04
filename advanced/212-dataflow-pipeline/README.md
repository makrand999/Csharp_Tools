# Dataflow Pipeline

Demonstrates TPL Dataflow (`System.Threading.Tasks.Dataflow`) for building parallel processing pipelines with blocks, propagation, and concurrent execution.

## Usage

```bash
dotnet run --project DataflowPipeline.csproj
```

## Example

```
=== TPL Dataflow Pipeline ===

Creating parallel processing pipeline with dataflow blocks...

Processing 15 items through pipeline...

[05] Validated: 1
[06] Validated: 2
[05] Transformed: 1 (Value: 10 → 120)
[07] Validated: 3
[06] Transformed: 2 (Value: 20 → 140)
...

✓ Pipeline processing completed!
```

## Concepts Demonstrated

- `TransformBlock<TInput, TOutput>` for data transformation
- `ActionBlock<TInput>` for final processing
- `BufferBlock<T>` for buffering data
- `ExecutionDataflowBlockOptions` for parallelism control
- `DataflowLinkOptions` with `PropagateCompletion`
- Fan-out patterns for parallel branching
- Bounded capacity for backpressure
