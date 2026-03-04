// Dataflow Pipeline - Demonstrates TPL Dataflow for parallel processing
// Uses blocks, propagation, and parallel execution

using System.Threading.Tasks.Dataflow;

Console.WriteLine("=== TPL Dataflow Pipeline ===\n");
Console.WriteLine("Creating parallel processing pipeline with dataflow blocks...\n");

// Create the dataflow pipeline
var pipeline = new DataProcessingPipeline();

// Generate sample data
var items = Enumerable.Range(1, 15).Select(i => new DataItem
{
    Id = i,
    Value = i * 10,
    Timestamp = DateTime.UtcNow
}).ToList();

Console.WriteLine($"Processing {items.Count} items through pipeline...\n");

// Process through the pipeline
await pipeline.ProcessBatchAsync(items);

Console.WriteLine("\n✓ Pipeline processing completed!");

/// <summary>
/// Represents a data item flowing through the pipeline
/// </summary>
public class DataItem
{
    public int Id { get; set; }
    public int Value { get; set; }
    public DateTime Timestamp { get; set; }
    public int ProcessedValue { get; set; }
    public string? Status { get; set; }
}

/// <summary>
/// Demonstrates TPL Dataflow for building parallel processing pipelines
/// </summary>
public class DataProcessingPipeline
{
    private readonly TransformBlock<DataItem, DataItem> _validateBlock;
    private readonly TransformBlock<DataItem, DataItem> _transformBlock;
    private readonly TransformBlock<DataItem, DataItem> _enrichBlock;
    private readonly ActionBlock<DataItem> _outputBlock;

    public DataProcessingPipeline()
    {
        // Configure execution options for parallelism
        var executionOptions = new ExecutionDataflowBlockOptions
        {
            MaxDegreeOfParallelism = 4,
            BoundedCapacity = 10
        };

        // Block 1: Validation
        _validateBlock = new TransformBlock<DataItem, DataItem>(item =>
        {
            Thread.Sleep(Random.Shared.Next(10, 50)); // Simulate work
            
            if (item.Value < 0)
            {
                item.Status = "Invalid";
            }
            else
            {
                item.Status = "Valid";
            }
            
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId:D2}] Validated: {item.Id}");
            return item;
        }, executionOptions);

        // Block 2: Transformation
        _transformBlock = new TransformBlock<DataItem, DataItem>(item =>
        {
            Thread.Sleep(Random.Shared.Next(20, 80)); // Simulate work
            
            item.ProcessedValue = item.Value * 2 + 100;
            item.Status += " → Transformed";
            
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId:D2}] Transformed: {item.Id} (Value: {item.Value} → {item.ProcessedValue})");
            return item;
        }, executionOptions);

        // Block 3: Enrichment
        _enrichBlock = new TransformBlock<DataItem, DataItem>(item =>
        {
            Thread.Sleep(Random.Shared.Next(10, 30)); // Simulate work
            
            item.Status += " → Enriched";
            
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId:D2}] Enriched: {item.Id}");
            return item;
        }, executionOptions);

        // Block 4: Output/Final processing
        _outputBlock = new ActionBlock<DataItem>(item =>
        {
            Console.WriteLine($"[{Thread.CurrentThread.ManagedThreadId:D2}] Output: {item.Id} - Final Value: {item.ProcessedValue}");
        }, executionOptions);

        // Link the blocks with propagation
        _validateBlock.LinkTo(_transformBlock, new DataflowLinkOptions { PropagateCompletion = true });
        _transformBlock.LinkTo(_enrichBlock, new DataflowLinkOptions { PropagateCompletion = true });
        _enrichBlock.LinkTo(_outputBlock, new DataflowLinkOptions { PropagateCompletion = true });
    }

    /// <summary>
    /// Process a batch of items through the pipeline
    /// </summary>
    public async Task ProcessBatchAsync(IEnumerable<DataItem> items)
    {
        // Post all items to the pipeline
        foreach (var item in items)
        {
            await _validateBlock.SendAsync(item);
        }

        // Mark the pipeline as complete
        _validateBlock.Complete();

        // Wait for all blocks to finish processing
        await _outputBlock.Completion;
    }

    /// <summary>
    /// Create a fan-out pipeline for parallel branching
    /// </summary>
    public static async Task DemonstrateFanOutAsync(IEnumerable<int> numbers)
    {
        var sourceBlock = new BufferBlock<int>();
        
        var squareBlock = new TransformBlock<int, int>(n => n * n);
        var cubeBlock = new TransformBlock<int, int>(n => n * n * n);
        var doubleBlock = new TransformBlock<int, int>(n => n * 2);

        var mergeBlock = new ActionBlock<int>(result =>
        {
            Console.WriteLine($"Result: {result}");
        });

        // Fan-out: one source to multiple processors
        sourceBlock.LinkTo(squareBlock, new DataflowLinkOptions { PropagateCompletion = true });
        sourceBlock.LinkTo(cubeBlock, new DataflowLinkOptions { PropagateCompletion = true });
        sourceBlock.LinkTo(doubleBlock, new DataflowLinkOptions { PropagateCompletion = true });

        // All processors merge to output
        squareBlock.LinkTo(mergeBlock);
        cubeBlock.LinkTo(mergeBlock);
        doubleBlock.LinkTo(mergeBlock);

        // Send data through
        foreach (var n in numbers)
        {
            await sourceBlock.SendAsync(n);
        }

        sourceBlock.Complete();
        await mergeBlock.Completion;
    }
}
