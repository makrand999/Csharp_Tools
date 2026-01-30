# Pomodoro Timer

A CLI Pomodoro timer with session tracking, task management, and productivity statistics. Includes both command-line and interactive modes.

## Usage

```bash
dotnet run --project PomodoroTimer.csproj [command] [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `start [minutes]` | Start a focus timer (default: 25 min) |
| `log <minutes> [task]` | Log a completed session manually |
| `list [days]` | Show recent sessions (default: 7 days) |
| `stats` | Show productivity statistics |
| `clear` | Clear all session history |
| (no command) | Interactive mode |

## Examples

```bash
# Start a 25-minute Pomodoro
dotnet run -- start 25

# Start a 45-minute session with a task name
dotnet run -- start 45 "Write documentation"

# Manually log a completed session
dotnet run -- log 30 "Read technical book"

# View sessions from the last 14 days
dotnet run -- list 14

# View productivity statistics
dotnet run -- stats

# Run in interactive mode
dotnet run --
```

## Interactive Mode

Running without arguments starts interactive mode:

```
$ dotnet run --

╔═══════════════════════════════════════════╗
║     🍅 Pomodoro Timer - Interactive Mode  ║
╚═══════════════════════════════════════════╝

Commands:
  start [min]  - Start a timer (default 25 min)
  log <min>    - Log a completed session
  list         - Show recent sessions
  stats        - Show statistics
  quit         - Exit the program

🍅 > start 25
⏱️  Starting 25-minute focus session...

🍅 24:59 remaining
```

## Example Statistics Output

```
$ dotnet run -- stats

═══════════════════════════════════════════
 🍅 Pomodoro Statistics
═══════════════════════════════════════════

  All Time:
    Total sessions:    15
    Total time:        375 minutes (6.2 hours)
    Avg session:       25 minutes
    Daily average:     75 minutes

  This Week:
    Sessions:          10
    Time:              250 minutes (4.2 hours)

  Today:
    Sessions:          3
    Time:              75 minutes

  Top Tasks:
  ─────────────────────────────────────
  Write documentation          120 min (2x)
  Code review                   75 min (3x)
  Read technical book           60 min (2x)

  This Week:
  ─────────────────────────────────────
  Mon: ██████████████████████████████ 120 min
  Tue: ████████████████████ 80 min
  Wed: ██████████████████████████████ 120 min
  Thu: ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ 0 min
  Fri: ████ 20 min
  Sat: ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ 0 min
  Sun: ░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░ 0 min
```

## Concepts Demonstrated

- Async/await for timers
- JSON serialization with `System.Text.Json`
- File I/O for data persistence
- Command-line argument parsing
- Interactive CLI mode with readline
- LINQ for data aggregation and statistics
- Date/time calculations
- ASCII chart visualization
