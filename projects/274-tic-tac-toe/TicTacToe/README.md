# Tic-Tac-Toe

Classic two-player game with an optional computer opponent.

## Usage

```bash
dotnet run --project TicTacToe.csproj
```

## Example

```
⭕ Tic-Tac-Toe
==============

Game modes:
  1. Player vs Player
  2. Player vs Computer

Choose mode (1-2): 2

  X | O | X 
 ---+---+---
  O | X | O 
 ---+---+---
  X | O | X 

Positions:
  1 | 2 | 3 
 ---+---+---
  4 | 5 | 6 
 ---+---+---
  7 | 8 | 9 

Player X, enter position (1-9): 5
```

## Features

- **Two game modes** - PvP or vs Computer
- **Smart AI** - Computer tries to win and blocks your moves
- **Visual board** - Clear display with position guide
- **Win detection** - Checks all winning combinations

## Rules

- Get 3 in a row (horizontal, vertical, or diagonal) to win
- X goes first
- Enter position 1-9 to place your mark

## Concepts Demonstrated

- 2D array patterns for win detection
- Game state management
- Basic AI algorithm
- Console drawing
- Input validation
