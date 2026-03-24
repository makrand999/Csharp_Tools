# Sudoku

Classic Sudoku puzzle with hint system and validation.

## Usage

```bash
dotnet run --project Sudoku.csproj
```

## Controls

| Input | Action |
|-------|--------|
| `row col value` | Place a number |
| `H` | Get a hint |
| `Q` | Quit game |

## Example

```
🔢 Sudoku
=========

Enter: row col value (or H for hint, Q to quit)

    1   2   3   4   5   6   7   8   9 
   ┌───┬───┬───┬───┬───┬───┬───┬───┬───┐
 1│ 5 · 2 │ · · · │ · 8 · │
 2│ · · · │ 4 · 6 │ · · · │
 3│ · 8 · │ · · · │ · 3 · │
   ├───┼───┼───┼───┼───┼───┼───┼───┼───┤
 4│ · · [·] │ · · · │ · · · │
 ...

Your move: 4 3 7
✅ Correct!
```

## Rules

- Fill the grid so each row, column, and 3x3 box contains 1-9
- Numbers in circles are fixed
- Numbers in brackets are your answers
- Use hints if stuck

## Features

- **Random puzzles** - New puzzle every game
- **Hint system** - Get help when stuck
- **Validation** - Checks if moves are correct
- **Visual grid** - Clear 3x3 box separation

## Concepts Demonstrated

- Backtracking algorithm
- 2D array manipulation
- Puzzle generation
- Input validation
- Console drawing with Unicode borders
