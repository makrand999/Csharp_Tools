# Random Name Picker

Pick random winners, create teams, and generate Secret Santa assignments.

## Usage

```bash
# Interactive mode
dotnet run --project NamePicker.csproj

# Quick pick from arguments
dotnet run --project NamePicker.csproj Alice Bob Charlie David
```

## Modes

### 1. Pick Winner
Enter a list of names and randomly select a winner with suspense.

### 2. Team Assignment
Automatically assign people to balanced teams.

### 3. Secret Santa
Generate random gift assignments (no one gets themselves).

## Example

```
🎯 Random Name Picker
=====================

Modes:
  1. Pick winner from list
  2. Random team assignment
  3. Secret Santa generator

Choose mode (1-3): 1

Enter names (one per line, empty line to finish):
> Alice
> Bob
> Charlie
> Diana
>
🎲 Picking winner...
   Diana...
   Bob...
   Charlie...

🏆 WINNER: Charlie!
🎉 Congratulations!
```

```
$ dotnet run --project NamePicker.csproj TeamA TeamB TeamC

🎲 Selecting winner...

🏆 Winner: TeamB!
```

## Use Cases

- Raffle drawings
- Classroom participation
- Team sports
- Gift exchanges
- Random selection

## Concepts Demonstrated

- List shuffling
- Random selection
- Input validation
- Team balancing algorithm
- Derangement generation (Secret Santa)
