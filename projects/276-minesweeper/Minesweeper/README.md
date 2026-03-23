# Minesweeper

Classic minesweeper puzzle game with flagging and auto-reveal.

## Usage

```bash
dotnet run --project Minesweeper.csproj
```

## Controls

| Input | Action |
|-------|--------|
| `x,y` | Reveal cell at position |
| `Fx,y` | Toggle flag on cell |

## Example

```
💣 Minesweeper
==============

Mines: 12 | Flags: 0 | Use: [x,y] or F[x,y] to flag

    1   2   3   4   5   6   7   8   9   10
   ┌───────────────────────────────────────┐
 1│ · · · · · · · · · · │
 2│ · · · · · · · · · · │
 3│ · · 1 · · · · · · · │
 4│ · 2 · 1 · · · · · · │
 5│ · · · · · · · · · · │
   └───────────────────────────────────────┘

Move (format: x,y or Fx,y to flag): 3,3
```

## Rules

- Numbers show how many mines are adjacent
- Flag all mines to win
- Don't reveal a mine!
- Empty cells auto-reveal neighbors

## Concepts Demonstrated

- 2D arrays
- Recursive flood-fill algorithm
- Adjacency calculation
- Game state management
- Console drawing with borders
