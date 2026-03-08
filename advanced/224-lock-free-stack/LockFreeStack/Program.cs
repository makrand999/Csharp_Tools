namespace LockFreeStack;

/// <summary>
/// Lock-free stack implementation using Interlocked operations.
/// Demonstrates atomic compare-and-swap (CAS) for thread-safe data structures.
/// </summary>
class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("=== Lock-Free Stack ===\n");
        
        var stack = new LockFreeStack<int>();
        
        // Multiple producers pushing concurrently
        var pushTasks = Enumerable.Range(0, 4)
            .Select(id => Task.Run(() => PushRange(stack, id * 100, 100, id)))
            .ToList();
        
        // Multiple consumers popping concurrently
        var popTasks = Enumerable.Range(0, 4)
            .Select(id => Task.Run(() => PopRange(stack, id, 100)))
            .ToList();
        
        await Task.WhenAll(pushTasks);
        await Task.WhenAll(popTasks);
        
        Console.WriteLine($"\nFinal stack count: {stack.Count}");
        Console.WriteLine($"Total pushed: {stack.TotalPushed}");
        Console.WriteLine($"Total popped: {stack.TotalPopped}");
        
        // Demo basic operations
        DemonstrateOperations();
    }
    
    static async Task PushRange(LockFreeStack<int> stack, int start, int count, int taskId)
    {
        for (int i = 0; i < count; i++)
        {
            stack.Push(start + i);
            if (i % 20 == 0)
                Console.WriteLine($"Task {taskId}: Pushed {start + i}");
            await Task.Delay(1);
        }
    }
    
    static async Task PopRange(LockFreeStack<int> stack, int taskId, int maxAttempts)
    {
        int attempts = 0;
        while (attempts < maxAttempts)
        {
            if (stack.TryPop(out int value))
            {
                if (attempts % 20 == 0)
                    Console.WriteLine($"Popper {taskId}: Popped {value}");
            }
            attempts++;
            await Task.Delay(2);
        }
    }
    
    static void DemonstrateOperations()
    {
        Console.WriteLine("\n=== Lock-Free Stack Operations Demo ===\n");
        
        var stack = new LockFreeStack<string>();
        
        stack.Push("First");
        stack.Push("Second");
        stack.Push("Third");
        
        Console.WriteLine($"Count: {stack.Count}");
        Console.WriteLine($"IsEmpty: {stack.IsEmpty}");
        
        while (stack.TryPop(out var item))
        {
            Console.WriteLine($"Popped: {item}");
        }
        
        Console.WriteLine($"Final IsEmpty: {stack.IsEmpty}");
    }
}

/// <summary>
/// Thread-safe lock-free stack using atomic operations.
/// </summary>
class LockFreeStack<T>
{
    private class Node
    {
        public T Value { get; }
        public Node? Next { get; set; }
        
        public Node(T value) => Value = value;
    }
    
    private Node? _head;
    private int _count;
    private int _totalPushed;
    private int _totalPopped;
    
    public int Count => Volatile.Read(ref _count);
    public bool IsEmpty => _head == null;
    public int TotalPushed => Volatile.Read(ref _totalPushed);
    public int TotalPopped => Volatile.Read(ref _totalPopped);
    
    public void Push(T value)
    {
        var newNode = new Node(value);
        Node? oldHead;
        
        do
        {
            oldHead = _head;
            newNode.Next = oldHead;
        }
        while (Interlocked.CompareExchange(ref _head, newNode, oldHead) != oldHead);
        
        Interlocked.Increment(ref _count);
        Interlocked.Increment(ref _totalPushed);
    }
    
    public bool TryPop(out T value)
    {
        Node? oldHead;
        Node? newHead;
        
        do
        {
            oldHead = _head;
            if (oldHead == null)
            {
                value = default!;
                return false;
            }
            newHead = oldHead.Next;
        }
        while (Interlocked.CompareExchange(ref _head, newHead, oldHead) != oldHead);
        
        value = oldHead.Value;
        Interlocked.Decrement(ref _count);
        Interlocked.Increment(ref _totalPopped);
        return true;
    }
}
