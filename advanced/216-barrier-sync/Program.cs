// Barrier Sync - Multi-phase synchronization with Barrier
// Demonstrates coordinating threads through multiple phases

Console.WriteLine("=== Barrier Synchronization ===\n");
Console.WriteLine("Simulating multi-phase processing with Barrier...\n");

const int participantCount = 4;
const int phaseCount = 3;

var barrier = new Barrier(participantCount);
var workers = new List<Task>();

Console.WriteLine($"Starting {participantCount} workers through {phaseCount} phases...\n");

// Create worker tasks
for (int i = 0; i < participantCount; i++)
{
    int workerId = i;
    workers.Add(Task.Run(() => WorkerAsync(workerId, barrier, phaseCount)));
}

// Wait for all workers to complete
await Task.WhenAll(workers);

Console.WriteLine("\n✓ All phases completed successfully!");

// Demonstrate Barrier with post-phase action
Console.WriteLine("\n=== Barrier with Post-Phase Action ===\n");

var barrierWithAction = new Barrier(participantCount, b =>
{
    Console.WriteLine($"\n[Barrier] Phase {b.CurrentPhaseNumber - 1} completed. " +
                      $"{b.ParticipantsRemaining} participants remaining.");
    Console.WriteLine($"[Barrier] Starting phase {b.CurrentPhaseNumber}...\n");
});

var workersWithAction = new List<Task>();

for (int i = 0; i < participantCount; i++)
{
    int workerId = i;
    workersWithAction.Add(Task.Run(() => WorkerWithActionAsync(workerId, barrierWithAction, 3)));
}

await Task.WhenAll(workersWithAction);

Console.WriteLine("\n✓ Barrier with post-phase action completed!");

static async Task WorkerAsync(int workerId, Barrier barrier, int phases)
{
    for (int phase = 0; phase < phases; phase++)
    {
        // Do work for this phase
        Console.WriteLine($"[Worker {workerId}] Phase {phase}: Starting work...");
        await Task.Delay(Random.Shared.Next(100, 300));
        Console.WriteLine($"[Worker {workerId}] Phase {phase}: Work done, waiting at barrier...");

        // Signal and wait for other participants
        barrier.SignalAndWait();
    }
}

static async Task WorkerWithActionAsync(int workerId, Barrier barrier, int phases)
{
    for (int phase = 0; phase < phases; phase++)
    {
        Console.WriteLine($"[Worker {workerId}] Phase {phase}: Processing...");
        await Task.Delay(Random.Shared.Next(50, 150));
        Console.WriteLine($"[Worker {workerId}] Phase {phase}: Complete, signaling barrier...");

        barrier.SignalAndWait();
    }
}

/// <summary>
/// Demonstrates Barrier for multi-phase algorithms
/// </summary>
public class MultiPhaseProcessor
{
    private readonly Barrier _barrier;
    private readonly int _phases;
    private readonly long[] _phaseResults;

    public MultiPhaseProcessor(int participantCount, int phases)
    {
        _phases = phases;
        _phaseResults = new long[phases];
        _barrier = new Barrier(participantCount, b =>
        {
            // Post-phase action: aggregate results
            var phase = (int)(b.CurrentPhaseNumber - 1);
            if (phase >= 0 && phase < phases)
            {
                Console.WriteLine($"[Aggregator] Phase {phase} results aggregated");
            }
        });
    }

    /// <summary>
    /// Process data through multiple phases
    /// Each phase must complete before the next begins
    /// </summary>
    public async Task<long> ProcessAsync(int workerId, int[] inputData)
    {
        long result = 0;

        for (int phase = 0; phase < _phases; phase++)
        {
            // Process data for this phase
            var phaseResult = await ProcessPhaseAsync(workerId, phase, inputData);

            // Add to phase results
            Interlocked.Add(ref _phaseResults[phase], phaseResult);

            // Wait for all workers to complete this phase
            _barrier.SignalAndWait();
        }

        return result;
    }

    private Task<long> ProcessPhaseAsync(int workerId, int phase, int[] data)
    {
        // Simulate phase-specific processing
        return Task.FromResult((long)data.Sum() * (phase + 1));
    }

    /// <summary>
    /// Get aggregated results for a specific phase
    /// </summary>
    public long GetPhaseResult(int phase) => _phaseResults[phase];
}

/// <summary>
/// Example: Parallel sorting algorithm using barrier synchronization
/// </summary>
public class ParallelSorter
{
    private readonly Barrier _barrier;
    private readonly int[][] _partitions;
    private readonly int _phases;

    public ParallelSorter(int[][] partitions)
    {
        _partitions = partitions;
        _phases = partitions.Length;
        _barrier = new Barrier(partitions.Length);
    }

    /// <summary>
    /// Sort using odd-even transposition sort (parallel bubble sort)
    /// </summary>
    public async Task SortAsync()
    {
        var tasks = _partitions.Select((partition, index) => 
            Task.Run(() => SortPartitionAsync(index)));

        await Task.WhenAll(tasks);
    }

    private async Task SortPartitionAsync(int partitionIndex)
    {
        for (int phase = 0; phase < _phases; phase++)
        {
            // Odd phase: compare with right neighbor
            if (phase % 2 == 1 && partitionIndex < _partitions.Length - 1)
            {
                await CompareExchangeAsync(partitionIndex, partitionIndex + 1);
            }
            // Even phase: compare with left neighbor
            else if (phase % 2 == 0 && partitionIndex > 0)
            {
                await CompareExchangeAsync(partitionIndex, partitionIndex - 1);
            }

            _barrier.SignalAndWait();
        }
    }

    private Task CompareExchangeAsync(int left, int right)
    {
        // Simulate compare-exchange operation
        Console.WriteLine($"Comparing partitions {left} and {right}");
        return Task.CompletedTask;
    }
}
