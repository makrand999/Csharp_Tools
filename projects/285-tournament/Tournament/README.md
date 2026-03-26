# Tournament Bracket Generator

Generate single-elimination tournament brackets for any number of participants.

## Usage

```bash
dotnet run --project Tournament.csproj
```

## Example

```
🏆 Tournament Bracket Generator
================================

Enter participants (one per line, empty line to finish):
> Team Alpha
> Team Beta
> Team Gamma
> Team Delta
> Team Epsilon
> Team Zeta
> Team Eta
> Team Theta
>
📊 Tournament Bracket (8 participants)

============================================================

📍 Round 1

  Match 1: Team Gamma vs Team Theta
  Match 2: Team Epsilon vs Team Eta
  Match 3: Team Beta vs Team Zeta
  Match 4: Team Alpha vs Team Delta
------------------------------------------------------------

📍 Round 2

  Match 1: Team Gamma vs Team Epsilon
  Match 2: Team Beta vs Team Alpha
------------------------------------------------------------

🏅 FINAL: Team Gamma vs Team Beta

🏆 Champion: Team Gamma

============================================================
Total rounds: 3
Total matches: 7
```

## Features

- **Auto-seeding** - Random bracket placement
- **BYE handling** - Auto-advances when odd numbers
- **Power of 2** - Pads to 2, 4, 8, 16, 32 participants
- **Simulation** - Simulates match outcomes
- **Visual bracket** - Clear round-by-round display

## Supported Sizes

| Participants | Rounds | Matches |
|--------------|--------|---------|
| 2 | 1 | 1 |
| 4 | 2 | 3 |
| 8 | 3 | 7 |
| 16 | 4 | 15 |
| 32 | 5 | 31 |

## Use Cases

- Sports tournaments
- Gaming competitions
- Debate brackets
- Coding challenges
- Office competitions

## Concepts Demonstrated

- Bracket algorithm
- Power of 2 padding
- Round-robin progression
- Match simulation
- List manipulation
