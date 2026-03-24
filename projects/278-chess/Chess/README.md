# Chess

Two-player chess game with Unicode pieces and move tracking.

## Usage

```bash
dotnet run --project Chess.csproj
```

## Controls

| Input | Action |
|-------|--------|
| `row col to_row to_col` | Move piece |
| `Q` | Quit game |

## Example

```
♟️  Chess (2 Player)
===================

White's turn (♙)
Format: from_row from_col to_row to_col (e.g., 6 0 4 0)

    a   b   c   d   e   f   g   h 
  8 ┌───┬───┬───┬───┬───┬───┬───┬───┐
  8 │ ♜ │ ♞ │ ♝ │ ♛ │ ♚ │ ♝ │ ♞ │ ♜ │
  ...
  2 │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │ ♟ │
  1 └───┴───┴───┴───┴───┴───┴───┴───┘
    a   b   c   d   e   f   g   h 

Move: 6 0 4 0
```

## Pieces

| Symbol | Piece | Symbol | Piece |
|--------|-------|--------|-------|
| ♔/♚ | King | ♕/♛ | Queen |
| ♖/♜ | Rook | ♗/♝ | Bishop |
| ♘/♞ | Knight | ♙/♟ | Pawn |

## Rules (Simplified)

- White moves first
- Capture the opponent's king to win
- Note: Move validation is simplified for this implementation

## Concepts Demonstrated

- 2D array for game board
- Unicode characters for pieces
- Turn-based gameplay
- Move tracking
- Win condition checking
