# Voting / Polling System

Create polls and collect votes with real-time results and visualization.

## Usage

```bash
dotnet run --project Voting.csproj
```

## Voting Modes

### 1. Open Voting
Anyone can vote without identification.

### 2. Named Voting
Each person can only vote once (tracked by name).

## Example

```
🗳️  Voting System
=================

Enter poll options (one per line, empty line to finish):

> Pizza
> Burgers
> Sushi
> Tacos
>
✅ Poll created with 4 options

Voting modes:
  1. Open voting (anyone can vote)
  2. One vote per person (named)

Choose mode (1-2): 2

--- Voting Started ---
Enter votes (format: name:option)
Type 'end' to finish voting

Vote: Alice:Pizza
✅ Alice voted for Pizza
Vote: Bob:Burgers
✅ Bob voted for Burgers
Vote: Charlie:Pizza
✅ Charlie voted for Pizza
Vote: end

==================================================

📊 POLL RESULTS

==================================================
Pizza                [██████████████░░░░░░░░░░░░]   2 ( 50.0%) 🏆
Burgers              [███████░░░░░░░░░░░░░░░░░░░]   1 ( 25.0%)
Sushi                [░░░░░░░░░░░░░░░░░░░░░░░░░░]   0 (  0.0%)
Tacos                [░░░░░░░░░░░░░░░░░░░░░░░░░░]   0 (  0.0%)
==================================================

Total votes: 3
Unique voters: 3

🏆 WINNER: Pizza!
```

## Features

- **Visual bars** - Proportional vote visualization
- **Percentage display** - Vote share calculation
- **Tie detection** - Identifies tied results
- **Vote tracking** - Prevents duplicate votes
- **Results summary** - Total votes and voters

## Use Cases

- Office decisions
- Group choices
- Team voting
- Event planning
- Preference collection

## Concepts Demonstrated

- Dictionary for vote counting
- HashSet for unique tracking
- LINQ sorting and aggregation
- String formatting
- Bar chart visualization
