# Loops

Comprehensive demonstration of loop structures in C#: for, while, do-while, foreach, nested loops, and loop control statements.

## Usage

```bash
dotnet run --project Loops.csproj
```

## Example

```
=== For Loop ===
Counting from 1 to 5:
1 2 3 4 5 

Countdown from 5 to 1:
5 4 3 2 1 

Even numbers from 2 to 10:
2 4 6 8 10 

Array elements:
numbers[0] = 10  numbers[1] = 20  numbers[2] = 30  numbers[3] = 40  numbers[4] = 50  

=== While Loop ===
While loop counting 1 to 5:
1 2 3 4 5 

While loop with condition:
1 2 3 4 5 
Sum reached: 15 (limit was 15)

=== Do-While Loop ===
Do-while loop (starts at 5, condition < 5):
Value: 5
Note: Executed once even though condition was false!

Do-while for input validation simulation:
Attempt 1 of 3
Attempt 2 of 3
Success!

=== Foreach Loop ===
Fruits in array:
Apple Banana Cherry Date 

Squares in list:
1 4 9 16 25 

Fruits with index:
  [0] Apple
  [1] Banana
  [2] Cherry
  [3] Date

=== Nested Loops ===
Multiplication Table (1-5):
   1   2   3   4   5
   2   4   6   8  10
   3   6   9  12  15
   4   8  12  16  20
   5  10  15  20  25

Pyramid Pattern:
    *
   ***
  *****
 *******
*********

=== Loop Control: Break and Continue ===
Break at 3:
1 2 Break!

Continue (skip 3):
1 2 skip 4 5 

Nested loop with break:
Found: 2 * 3 = 6

=== Infinite Loop (with break) ===
Counting to 3 with infinite loop:
1 2 3 
Exited infinite loop with break!
```

## Concepts Demonstrated

- For loop with increment, decrement, and custom step
- For loop for array iteration
- While loop basics
- While loop with complex conditions
- Do-while loop (executes at least once)
- Foreach loop for arrays and collections
- Nested loops for patterns and tables
- Break statement to exit loops
- Continue statement to skip iterations
- Loop control with boolean flags
- Infinite loops with break conditions
