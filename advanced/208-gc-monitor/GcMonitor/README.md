# GC Monitor

Monitors garbage collection events and memory statistics. Useful for diagnosing memory issues, understanding GC behavior, and optimizing memory usage.

## Usage

```bash
dotnet run --project GcMonitor.csproj
```

## Example

```
=== GC Monitor Demo ===

Memory Statistics:
  GC Memory (bytes): 50,000
  Total Memory (bytes): 100,000
  Max Working Set: 2
  Generation 0 Count: 1
  Generation 1 Count: 0
  Generation 2 Count: 0
  Working Set (MB): 25.50
  Private Memory (MB): 30.25

--- GC Generations Demo ---

Allocated 100,000 bytes in Gen 0
Gen 0 count before: 1
Gen 0 count after: 2

--- Memory Pressure Demo ---

Simulating memory pressure...
  Allocated 1500KB, GC Memory: 2000KB
  Allocated 3000KB, GC Memory: 3500KB

--- Finalization Demo ---

Creating objects with finalizers...
  Created Object_0
  Created Object_1
  ...
  Finalized Object_0
  Finalized Object_1
```

## Concepts Demonstrated

- GC.GetTotalMemory() for memory tracking
- GC.CollectionCount() for generation stats
- GC.Collect() for manual collection
- GC.WaitForPendingFinalizers()
- GC generations (0, 1, 2)
- Finalizers and object lifecycle
- Process memory statistics
- Memory pressure simulation
