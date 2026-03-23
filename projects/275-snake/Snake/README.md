# Snake Game

Classic snake game with real-time controls and score tracking.

## Usage

```bash
dotnet run --project Snake.csproj
```

## Controls

| Key | Action |
|-----|--------|
| W / ↑ | Move Up |
| S / ↓ | Move Down |
| A / ← | Move Left |
| D / → | Move Right |
| C | Quit Game |

## Example

```
🐍 Snake Game
=============

Controls: W/A/S/D or Arrow Keys
Press any key to start...

┌──────────────────────────────┐
│                              │
│         ●                    │
│      ▓▓▓█                    │
│                              │
│                              │
└──────────────────────────────┘
Score: 30 | Length: 6
Controls: W/A/S/D | Press C to quit
```

## Rules

- Eat food (●) to grow and score points
- Avoid hitting walls
- Avoid hitting yourself
- Each food = 10 points

## Concepts Demonstrated

- Real-time game loop
- Console cursor positioning
- Multi-threaded input handling
- Collision detection
- List manipulation for snake body
