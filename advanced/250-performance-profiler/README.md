# Performance Profiler

Profiles code execution time and memory allocation with benchmarking support.

## Usage

```bash
dotnet run --project PerformanceProfiler.csproj
```

## Example

```
=== Performance Profiler ===

Profiling various operations...

=== Profile Summary ===
Name                           Time (ms)       Memory (KB)    
String Concatenation (1000)    45.23           2500           
StringBuilder (1000)           0.15            50             
List.Add (10000)               2.34            400            
LINQ Where+Select (10000)      5.67           800            

=== Benchmark: Sum of 10000 numbers ===
Iterations: 5
Average: 1.25ms
Min: 1.10ms
Max: 1.45ms
StdDev: 0.12ms
```

## Concepts Demonstrated

- Stopwatch for timing
- GC memory monitoring
- Statistical analysis
- Benchmark methodology
- Performance comparison
