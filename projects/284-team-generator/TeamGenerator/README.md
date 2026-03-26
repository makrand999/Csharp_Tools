# Team Generator

Create balanced teams from a list of participants with multiple assignment methods.

## Usage

```bash
dotnet run --project TeamGenerator.csproj
```

## Team Creation Methods

### 1. Random Assignment
Automatically determines optimal number of teams based on participant count.

### 2. Specify Team Size
Creates teams with a specific number of members per team.

### 3. Specify Number of Teams
Creates exactly the specified number of teams.

## Example

```
👥 Team Generator
=================

Enter participant names (one per line, empty line to finish):
> Alice
> Bob
> Charlie
> Diana
> Eve
> Frank
>
6 participants registered.

Team creation method:
  1. Random assignment
  2. Specify team size
  3. Specify number of teams

Choose (1-3): 3

Number of teams: 2

========================================

📋 Team Assignments

========================================

🔵 Team 1 (3 members)
------------------------------
   • Charlie
   • Alice
   • Frank

🔵 Team 2 (3 members)
------------------------------
   • Diana
   • Bob
   • Eve

========================================
Total: 2 teams, 6 participants

💾 Save to file? (y/n): y
✅ Saved to: teams_20240115_143022.txt
```

## Features

- **Random shuffling** - Fair team assignment
- **Multiple methods** - Choose by size or count
- **Balanced teams** - Even distribution
- **File export** - Save results for later

## Use Cases

- Sports teams
- Project groups
- Game teams
- Study groups
- Competition brackets

## Concepts Demonstrated

- List manipulation
- Random shuffling
- LINQ operations (Skip, Take)
- File I/O
- User input handling
