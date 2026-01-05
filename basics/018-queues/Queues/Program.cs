// Program 18: Queues - Demonstrates Queue<T> FIFO (First-In-First-Out) operations
// Topics: Enqueue, Dequeue, Peek, counting, clearing

Console.WriteLine("=== Queue Basics (FIFO) ===\n");

// Create a queue of strings (representing a print queue)
var printQueue = new Queue<string>();

// Enqueue items
Console.WriteLine("--- Enqueuing Print Jobs ---");
printQueue.Enqueue("document1.pdf");
printQueue.Enqueue("report.docx");
printQueue.Enqueue("image.png");
printQueue.Enqueue("spreadsheet.xlsx");
Console.WriteLine($"Enqueued 4 print jobs");
Console.WriteLine($"Current queue count: {printQueue.Count}\n");

// Display queue contents
Console.WriteLine("Current queue (front to back):");
foreach (var job in printQueue)
{
    Console.WriteLine($"  → {job}");
}

// Peek - view front item without removing
Console.WriteLine("\n--- Peek Operation ---");
Console.WriteLine($"Next job to print (Peek): {printQueue.Peek()}");
Console.WriteLine($"Queue count after Peek: {printQueue.Count}");

// Dequeue - remove and return front item
Console.WriteLine("\n--- Dequeue Operations (Processing Jobs) ---");
while (printQueue.Count > 0)
{
    var job = printQueue.Dequeue();
    Console.WriteLine($"  Printing: {job} (Remaining: {printQueue.Count})");
}

// Queue with integers (representing customers in line)
Console.WriteLine("\n=== Queue of Customers ===\n");

var customerQueue = new Queue<int>();

Console.WriteLine("--- Customers Arriving ---");
for (int i = 1; i <= 5; i++)
{
    customerQueue.Enqueue(i);
    Console.WriteLine($"  Customer #{i} joined the queue");
}
Console.WriteLine($"\nTotal customers waiting: {customerQueue.Count}");

Console.WriteLine("\n--- Serving Customers ---");
while (customerQueue.Count > 0)
{
    var customer = customerQueue.Dequeue();
    Console.WriteLine($"  Serving customer #{customer}");
}

// Queue with TryDequeue (safe dequeue)
Console.WriteLine("\n=== Safe Dequeue with TryDequeue ===\n");

var orderQueue = new Queue<string>();
orderQueue.Enqueue("Order #101");
orderQueue.Enqueue("Order #102");
orderQueue.Enqueue("Order #103");

Console.WriteLine("Order queue:");
foreach (var order in orderQueue)
{
    Console.WriteLine($"  □ {order}");
}

Console.WriteLine("\nProcessing orders:");
while (orderQueue.TryDequeue(out string? order))
{
    Console.WriteLine($"  ✓ Fulfilled: {order}");
}

// TryDequeue on empty queue
Console.WriteLine("\nTrying to dequeue from empty queue:");
if (!orderQueue.TryDequeue(out string? emptyResult))
{
    Console.WriteLine("  Queue is empty - no more orders!");
}

// Clear operation
Console.WriteLine("\n=== Clear Operation ===\n");

var numbers = new Queue<int>();
numbers.Enqueue(1);
numbers.Enqueue(2);
numbers.Enqueue(3);
Console.WriteLine($"Queue count before Clear: {numbers.Count}");

numbers.Clear();
Console.WriteLine($"Queue count after Clear: {numbers.Count}");
Console.WriteLine($"Queue is empty: {numbers.Count == 0}");

// Contains operation
Console.WriteLine("\n=== Contains Operation ===\n");

var colors = new Queue<string>();
colors.Enqueue("Red");
colors.Enqueue("Green");
colors.Enqueue("Blue");

Console.WriteLine("Colors in queue: Red, Green, Blue");
Console.WriteLine($"Contains 'Green': {colors.Contains("Green")}");
Console.WriteLine($"Contains 'Yellow': {colors.Contains("Yellow")}");

// Convert to array
Console.WriteLine("\n=== Convert Queue to Array ===\n");

var queueArray = colors.ToArray();
Console.WriteLine("Queue converted to array:");
for (int i = 0; i < queueArray.Length; i++)
{
    Console.WriteLine($"  [{i}]: {queueArray[i]}");
}

// Real-world example: Message queue
Console.WriteLine("\n=== Real-World: Message Queue ===\n");

var messageQueue = new Queue<(string sender, string message, DateTime timestamp)>();

messageQueue.Enqueue(("Alice", "Hello!", DateTime.Now));
messageQueue.Enqueue(("Bob", "Hi there!", DateTime.Now.AddSeconds(5)));
messageQueue.Enqueue(("Charlie", "How are you?", DateTime.Now.AddSeconds(10)));

Console.WriteLine("Processing messages in order received:\n");
while (messageQueue.TryDequeue(out var msg))
{
    Console.WriteLine($"  [{msg.timestamp:HH:mm:ss}] {msg.sender}: {msg.message}");
}

Console.WriteLine("\n=== Program Complete ===");
