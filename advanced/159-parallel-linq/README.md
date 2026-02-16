# Parallel LINQ (PLINQ) Demo

Performance utilities for parallel data processing. Compares sequential vs parallel query performance and demonstrates PLINQ operators, aggregation, and optimization techniques.

## Usage

```bash
dotnet run --project ParallelLinq.csproj
```

## Example

```
=== Parallel LINQ (PLINQ) Demo ===
Comparing sequential vs parallel query performance.

Choose operation:
1. Performance Comparison (Sequential vs Parallel)
2. PLINQ Operators Demo
3. Parallel Aggregation
4. Run All Demos

Selection (1-4): 1

--- Performance Comparison ---

Running sequential LINQ...
Sequential: 145 ms, Result: 166,666,666,500,000

Running parallel PLINQ...
Parallel:   42 ms, Result: 166,666,666,500,000

Speedup: 3.45x faster with PLINQ
```

## Concepts Demonstrated

- **AsParallel()** - Converting LINQ to parallel queries
- **WithDegreeOfParallelism** - Controlling parallelism level
- **WithExecutionMode** - ForceParallelism for small datasets
- **AsUnordered** - Performance optimization when order doesn't matter
- **Parallel Aggregation** - Count, Sum, Average, Min, Max
- **Parallel GroupBy** - Grouping with parallel processing
- **Performance Measurement** - Stopwatch-based benchmarking
