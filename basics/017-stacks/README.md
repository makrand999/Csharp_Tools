# Stacks

Demonstrates C# `Stack<T>` operations including Push, Pop, Peek, and other LIFO (Last-In-First-Out) collection methods.

## Usage

```bash
dotnet run --project Stacks.csproj
```

## Example

```
=== Stack Basics (LIFO) ===

--- Pushing Pages onto Stack ---
Pushed 4 pages onto the stack
Current stack count: 4

Current stack (top to bottom):
  → https://dotnet.microsoft.com
  → https://stackoverflow.com
  → https://github.com
  → https://google.com

--- Peek Operation ---
Current page (Peek): https://dotnet.microsoft.com
Stack count after Peek: 4

--- Pop Operations (Back Button) ---
  Popped: https://dotnet.microsoft.com (Count: 3)
  Popped: https://stackoverflow.com (Count: 2)
  Popped: https://github.com (Count: 1)
  Popped: https://google.com (Count: 0)

=== Stack of Integers ===

--- Adding Plates to Stack ---
  Added plate #1
  Added plate #2
  Added plate #3
  Added plate #4
  Added plate #5

Total plates: 5

--- Removing Plates from Stack ---
  Removed plate #5
  Removed plate #4
  Removed plate #3
  Removed plate #2
  Removed plate #1

=== Safe Pop with TryPop ===

Task stack:
  □ Deploy application
  □ Write tests
  □ Review code

Completing tasks:
  ✓ Completed: Deploy application
  ✓ Completed: Write tests
  ✓ Completed: Review code

Trying to pop from empty stack:
  Stack is empty - no more tasks!

=== Clear Operation ===

Stack count before Clear: 3
Stack count after Clear: 0
Stack is empty: True

=== Contains Operation ===

Colors in stack: Red, Green, Blue
Contains 'Green': True
Contains 'Yellow': False

=== Convert Stack to Array ===

Stack converted to array:
  [0]: Blue
  [1]: Green
  [2]: Red

=== Program Complete ===
```

## Concepts Demonstrated

- Stack<T> creation and initialization
- Push - adding items to top of stack
- Pop - removing and returning top item
- Peek - viewing top item without removing
- TryPop - safe pop operation (returns false if empty)
- Count - getting number of items
- Clear - removing all items
- Contains - checking if item exists
- ToArray - converting stack to array
- LIFO (Last-In-First-Out) principle
- Real-world examples: browser history, task completion, call stack
