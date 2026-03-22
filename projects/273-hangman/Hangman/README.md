# Hangman

Classic word-guessing game with ASCII art graphics.

## Usage

```bash
dotnet run --project Hangman.csproj
```

## Example

```
🎮 Hangman
=========

  +---+
  O   |
 /|\  |
      |
     ===

Word: P _ O G _ A _ _ I N G

Wrong attempts: 2/6
Guessed letters: A, G, I, N, O, P, R

Guess a letter: _
```

## Rules

- Guess the hidden word one letter at a time
- You have 6 wrong attempts before losing
- Guess all letters correctly to win
- Words are programming/tech themed

## Concepts Demonstrated

- HashSet for tracking guesses
- String manipulation
- ASCII art display
- Game state management
- Console clearing and redrawing
