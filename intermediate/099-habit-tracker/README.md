# Habit Tracker

A CLI tool to build and track daily habits with streak counting, completion history, and weekly summaries.

## Usage

```bash
dotnet run --project HabitTracker.csproj <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `add <name> [frequency]` | Add a new habit (daily, weekly, weekdays) |
| `list` | List all habits with current streaks |
| `check <id> [date]` | Mark a habit as complete |
| `uncheck <id> [date]` | Remove completion for a date |
| `history <id> [days]` | Show completion history |
| `delete <id>` | Remove a habit |
| `stats` | Show overall statistics |

## Frequencies

- `daily` - Track every day (default)
- `weekdays` - Track Monday through Friday only
- `weekly` - Track once per week

## Examples

```bash
# Add habits
dotnet run -- add "Drink 8 glasses of water" daily
dotnet run -- add "Exercise" weekdays
dotnet run -- add "Read a book" weekly

# List all habits with streaks
dotnet run -- list

# Check off a habit for today
dotnet run -- check 1

# Check off a habit for a specific date
dotnet run -- check 2 2026-03-30

# View completion history (last 14 days)
dotnet run -- history 1 14

# View statistics
dotnet run -- stats

# Remove a completion
dotnet run -- uncheck 1 2026-03-30
```

## Example Session

```
$ dotnet run -- add "Morning meditation" daily
✓ Added habit: "Morning meditation" (daily)
  ID: 1 | Created: 2026-03-31

$ dotnet run -- check 1
✓ Checked: "Morning meditation" for 2026-03-31
  🔥 Current streak: 1 days

$ dotnet run -- list

📊 Habits for Tuesday, March 31, 2026
════════════════════════════════════════════════════════════

✅ [1] Morning meditation
    Frequency: daily | 🔥 Streak: 1 days
    ⚪S ⚪M ⚪T ⚪W ⚪T ⚪F ⚪S 

$ dotnet run -- stats

═══════════════════════════════════════════
 Habit Statistics
═══════════════════════════════════════════

  Total habits:        1
  Completed today:     1/1 (100.0%)
  Best streak:         1 days (Morning meditation)
  Total completions:   1
  Avg per habit:       1.0

  This Week:
  ─────────────────────────────────────
  Sun: ░ 0/1
  Mon: ░ 0/1
  Tue: █ 1/1
  Wed: ░ 0/1
  Thu: ░ 0/1
  Fri: ░ 0/1
  Sat: ░ 0/1
```

## Concepts Demonstrated

- JSON serialization with `System.Text.Json`
- File I/O for data persistence
- Command-line argument parsing
- Date/time manipulation and calculations
- Streak calculation algorithms
- LINQ for filtering and aggregation
- ASCII visualization for data display
