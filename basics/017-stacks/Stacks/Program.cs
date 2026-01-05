// Program 17: Stacks - Demonstrates Stack<T> LIFO (Last-In-First-Out) operations
// Topics: Push, Pop, Peek, counting, clearing

Console.WriteLine("=== Stack Basics (LIFO) ===\n");

// Create a stack of strings (representing browser history)
var browserHistory = new Stack<string>();

// Push items onto the stack
Console.WriteLine("--- Pushing Pages onto Stack ---");
browserHistory.Push("https://google.com");
browserHistory.Push("https://github.com");
browserHistory.Push("https://stackoverflow.com");
browserHistory.Push("https://dotnet.microsoft.com");
Console.WriteLine($"Pushed 4 pages onto the stack");
Console.WriteLine($"Current stack count: {browserHistory.Count}\n");

// Display stack contents
Console.WriteLine("Current stack (top to bottom):");
foreach (var page in browserHistory)
{
    Console.WriteLine($"  → {page}");
}

// Peek - view top item without removing
Console.WriteLine("\n--- Peek Operation ---");
Console.WriteLine($"Current page (Peek): {browserHistory.Peek()}");
Console.WriteLine($"Stack count after Peek: {browserHistory.Count}");

// Pop - remove and return top item
Console.WriteLine("\n--- Pop Operations (Back Button) ---");
while (browserHistory.Count > 0)
{
    var page = browserHistory.Pop();
    Console.WriteLine($"  Popped: {page} (Count: {browserHistory.Count})");
}

// Stack with integers (representing plates)
Console.WriteLine("\n=== Stack of Integers ===\n");

var plates = new Stack<int>();

Console.WriteLine("--- Adding Plates to Stack ---");
for (int i = 1; i <= 5; i++)
{
    plates.Push(i);
    Console.WriteLine($"  Added plate #{i}");
}
Console.WriteLine($"\nTotal plates: {plates.Count}");

Console.WriteLine("\n--- Removing Plates from Stack ---");
while (plates.Count > 0)
{
    var topPlate = plates.Pop();
    Console.WriteLine($"  Removed plate #{topPlate}");
}

// Stack with TryPop (safe pop)
Console.WriteLine("\n=== Safe Pop with TryPop ===\n");

var tasks = new Stack<string>();
tasks.Push("Review code");
tasks.Push("Write tests");
tasks.Push("Deploy application");

Console.WriteLine("Task stack:");
foreach (var task in tasks)
{
    Console.WriteLine($"  □ {task}");
}

Console.WriteLine("\nCompleting tasks:");
while (tasks.TryPop(out string? task))
{
    Console.WriteLine($"  ✓ Completed: {task}");
}

// TryPop on empty stack
Console.WriteLine("\nTrying to pop from empty stack:");
if (!tasks.TryPop(out string? emptyResult))
{
    Console.WriteLine("  Stack is empty - no more tasks!");
}

// Clear operation
Console.WriteLine("\n=== Clear Operation ===\n");

var numbers = new Stack<int>();
numbers.Push(1);
numbers.Push(2);
numbers.Push(3);
Console.WriteLine($"Stack count before Clear: {numbers.Count}");

numbers.Clear();
Console.WriteLine($"Stack count after Clear: {numbers.Count}");
Console.WriteLine($"Stack is empty: {numbers.Count == 0}");

// Contains operation
Console.WriteLine("\n=== Contains Operation ===\n");

var colors = new Stack<string>();
colors.Push("Red");
colors.Push("Green");
colors.Push("Blue");

Console.WriteLine("Colors in stack: Red, Green, Blue");
Console.WriteLine($"Contains 'Green': {colors.Contains("Green")}");
Console.WriteLine($"Contains 'Yellow': {colors.Contains("Yellow")}");

// Convert to array
Console.WriteLine("\n=== Convert Stack to Array ===\n");

var stackArray = colors.ToArray();
Console.WriteLine("Stack converted to array:");
for (int i = 0; i < stackArray.Length; i++)
{
    Console.WriteLine($"  [{i}]: {stackArray[i]}");
}

Console.WriteLine("\n=== Program Complete ===");
