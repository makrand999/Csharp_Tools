namespace FiberScheduler;

/// <summary>
/// Lightweight fiber/green thread scheduler for cooperative multitasking.
/// Demonstrates user-mode scheduling, yield points, and coroutine-like execution.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Fiber Scheduler ===\n");
        
        var scheduler = new FiberScheduler();
        
        // Create multiple fibers (lightweight tasks)
        scheduler.Start(async () => await CreateFiber("Fiber-A", 5, 100));
        scheduler.Start(async () => await CreateFiber("Fiber-B", 8, 50));
        scheduler.Start(async () => await CreateFiber("Fiber-C", 3, 200));
        
        // Run until all fibers complete
        await scheduler.RunUntilCompleteAsync();
        
        Console.WriteLine($"\nAll fibers completed!");
        Console.WriteLine($"Total yield points: {scheduler.TotalYields}");
        
        // Demo fiber operations
        DemonstrateFibers();
    }
    
    static async Task CreateFiber(string name, int iterations, int delay)
    {
        for (int i = 0; i < iterations; i++)
        {
            Console.WriteLine($"{name}: Iteration {i + 1}/{iterations}");
            await Fiber.Yield();
            await Task.Delay(delay);
        }
        Console.WriteLine($"{name}: Completed");
    }
    
    static void DemonstrateFibers()
    {
        Console.WriteLine("\n=== Fiber Demo ===\n");
        
        var scheduler = new FiberScheduler();
        
        // Fibonacci generator fiber
        scheduler.Start(async () =>
        {
            Console.WriteLine("Fibonacci sequence:");
            int a = 0, b = 1;
            for (int i = 0; i < 10; i++)
            {
                Console.Write($"{a} ");
                (a, b) = (b, a + b);
                await Fiber.Yield();
            }
            Console.WriteLine();
        });
        
        // Counter fiber
        scheduler.Start(async () =>
        {
            Console.WriteLine("\nCounter:");
            for (int i = 1; i <= 5; i++)
            {
                Console.Write($"{i} ");
                await Fiber.Yield();
            }
            Console.WriteLine();
        });
        
        scheduler.RunUntilCompleteAsync().Wait();
    }
}

/// <summary>
/// Fiber - a lightweight cooperative task that can yield control.
/// </summary>
static class Fiber
{
    private static readonly Task _yieldTask = Task.CompletedTask;
    
    public static Task Yield() => _yieldTask;
}

/// <summary>
/// Cooperative fiber scheduler using async/await.
/// </summary>
class FiberScheduler
{
    private readonly Queue<Func<Task>> _fibers = new();
    private readonly Queue<Func<Task>> _readyQueue = new();
    private int _totalYields;
    private bool _running;
    
    public int TotalYields => Volatile.Read(ref _totalYields);
    
    public void Start(Func<Task> fiber)
    {
        _fibers.Enqueue(fiber);
    }
    
    public async Task RunUntilCompleteAsync()
    {
        _running = true;
        
        while (_fibers.Count > 0 || _readyQueue.Count > 0)
        {
            // Move all fibers to ready queue
            while (_fibers.Count > 0)
            {
                _readyQueue.Enqueue(_fibers.Dequeue());
            }
            
            if (_readyQueue.Count == 0)
                break;
            
            var fiber = _readyQueue.Dequeue();
            
            try
            {
                // Create a continuation that re-queues the fiber if it yields
                var task = fiber();
                
                if (!task.IsCompleted)
                {
                    Interlocked.Increment(ref _totalYields);
                    
                    // Schedule continuation to re-queue
                    _ = task.ContinueWith(_ => _readyQueue.Enqueue(fiber));
                    
                    // Give other fibers a chance
                    await Task.Yield();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fiber error: {ex.Message}");
            }
        }
        
        _running = false;
    }
}

/// <summary>
/// Alternative implementation using Task-based fibers with explicit scheduling.
/// </summary>
class AdvancedFiberScheduler
{
    private class FiberTask
    {
        public string Name { get; }
        public Func<Task> Action { get; }
        public int Priority { get; }
        public bool IsCompleted { get; set; }
        
        public FiberTask(string name, Func<Task> action, int priority = 0)
        {
            Name = name;
            Action = action;
            Priority = priority;
        }
    }
    
    private readonly List<FiberTask> _fibers = new();
    private readonly object _lock = new();
    
    public void Spawn(string name, Func<Task> action, int priority = 0)
    {
        lock (_lock)
        {
            _fibers.Add(new FiberTask(name, action, priority));
        }
    }
    
    public async Task RunAllAsync()
    {
        var tasks = new List<Task>();
        
        lock (_lock)
        {
            foreach (var fiber in _fibers.OrderBy(f => -f.Priority))
            {
                tasks.Add(RunFiberAsync(fiber));
            }
        }
        
        await Task.WhenAll(tasks);
    }
    
    private async Task RunFiberAsync(FiberTask fiber)
    {
        try
        {
            Console.WriteLine($"Starting fiber: {fiber.Name}");
            await fiber.Action();
            Console.WriteLine($"Fiber completed: {fiber.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Fiber failed: {fiber.Name} - {ex.Message}");
        }
        finally
        {
            fiber.IsCompleted = true;
        }
    }
}
