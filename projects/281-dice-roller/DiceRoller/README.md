# Dice Roller

Roll dice using standard RPG notation (e.g., 2d6, 3d8, 4d10+3).

## Usage

```bash
# Interactive mode
dotnet run --project DiceRoller.csproj

# Command line
dotnet run --project DiceRoller.csproj --roll 2d6
dotnet run --project DiceRoller.csproj --roll 3d8+2
```

## Example

```
🎲 Dice Roller
==============

Dice notation: NdS (N = number of dice, S = sides)
Examples: 1d6, 2d6, 3d8, 4d10, 5d20, 2d6+3

Enter dice notation (or 'q' to quit): 2d20

Rolling 2d20...
Rolls: [15, 7]
Total: 22

Enter dice notation (or 'q' to quit): 1d20

Rolling 1d20...
Rolls: [20]
Total: 20
🎉 NATURAL 20! Critical hit!
```

## Supported Notation

| Notation | Description |
|----------|-------------|
| `1d6` | One 6-sided die |
| `2d6` | Two 6-sided dice |
| `4d10` | Four 10-sided dice |
| `2d6+3` | Two 6-sided dice + 3 modifier |
| `3d8-1` | Three 8-sided dice - 1 modifier |

## Common Dice Types

- d4 - Four-sided (pyramid)
- d6 - Six-sided (cube)
- d8 - Eight-sided (octahedron)
- d10 - Ten-sided (pentagonal trapezohedron)
- d12 - Twelve-sided (dodecahedron)
- d20 - Twenty-sided (icosahedron)

## Concepts Demonstrated

- Regular expressions for parsing
- Random number generation
- Command-line argument handling
- String manipulation
- Interactive loop
