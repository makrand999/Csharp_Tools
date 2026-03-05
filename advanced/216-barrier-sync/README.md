# Barrier Sync

Demonstrates `Barrier` for multi-phase synchronization where multiple threads must complete each phase before proceeding to the next.

## Usage

```bash
dotnet run --project BarrierSync.csproj
```

## Example

```
=== Barrier Synchronization ===

Simulating multi-phase processing with Barrier...

Starting 4 workers through 3 phases...

[Worker 0] Phase 0: Starting work...
[Worker 1] Phase 0: Starting work...
[Worker 2] Phase 0: Starting work...
[Worker 3] Phase 0: Starting work...
[Worker 0] Phase 0: Work done, waiting at barrier...
[Worker 1] Phase 0: Work done, waiting at barrier...
[Worker 2] Phase 0: Work done, waiting at barrier...
[Worker 3] Phase 0: Work done, waiting at barrier...
[Worker 0] Phase 1: Starting work...
...

✓ All phases completed successfully!
```

## Concepts Demonstrated

- `Barrier` for multi-phase synchronization
- `SignalAndWait` for barrier coordination
- Post-phase actions with barrier callback
- Participant count management
- Multi-phase algorithm patterns
- Parallel sorting with barrier sync

## Common Use Cases

- Parallel algorithms with distinct phases
- Iterative computations requiring synchronization
- Simulation steps that must complete together
- Parallel sorting algorithms (odd-even sort)
