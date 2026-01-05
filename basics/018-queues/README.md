# Queues

Demonstrates C# `Queue<T>` operations including Enqueue, Dequeue, Peek, and other FIFO (First-In-First-Out) collection methods.

## Usage

```bash
dotnet run --project Queues.csproj
```

## Example

```
=== Queue Basics (FIFO) ===

--- Enqueuing Print Jobs ---
Enqueued 4 print jobs
Current queue count: 4

Current queue (front to back):
  → document1.pdf
  → report.docx
  → image.png
  → spreadsheet.xlsx

--- Peek Operation ---
Next job to print (Peek): document1.pdf
Queue count after Peek: 4

--- Dequeue Operations (Processing Jobs) ---
  Printing: document1.pdf (Remaining: 3)
  Printing: report.docx (Remaining: 2)
  Printing: image.png (Remaining: 1)
  Printing: spreadsheet.xlsx (Remaining: 0)

=== Queue of Customers ===

--- Customers Arriving ---
  Customer #1 joined the queue
  Customer #2 joined the queue
  Customer #3 joined the queue
  Customer #4 joined the queue
  Customer #5 joined the queue

Total customers waiting: 5

--- Serving Customers ---
  Serving customer #1
  Serving customer #2
  Serving customer #3
  Serving customer #4
  Serving customer #5

=== Safe Dequeue with TryDequeue ===

Order queue:
  □ Order #101
  □ Order #102
  □ Order #103

Processing orders:
  ✓ Fulfilled: Order #101
  ✓ Fulfilled: Order #102
  ✓ Fulfilled: Order #103

Trying to dequeue from empty queue:
  Queue is empty - no more orders!

=== Clear Operation ===

Queue count before Clear: 3
Queue count after Clear: 0
Queue is empty: True

=== Contains Operation ===

Colors in queue: Red, Green, Blue
Contains 'Green': True
Contains 'Yellow': False

=== Convert Queue to Array ===

Queue converted to array:
  [0]: Red
  [1]: Green
  [2]: Blue

=== Real-World: Message Queue ===

Processing messages in order received:

  [HH:MM:SS] Alice: Hello!
  [HH:MM:SS] Bob: Hi there!
  [HH:MM:SS] Charlie: How are you?

=== Program Complete ===
```

## Concepts Demonstrated

- Queue<T> creation and initialization
- Enqueue - adding items to back of queue
- Dequeue - removing and returning front item
- Peek - viewing front item without removing
- TryDequeue - safe dequeue operation
- Count - getting number of items
- Clear - removing all items
- Contains - checking if item exists
- ToArray - converting queue to array
- FIFO (First-In-First-Out) principle
- Real-world examples: print queue, customer line, message queue
