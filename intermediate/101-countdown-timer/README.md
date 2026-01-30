# Countdown Timer

A practical CLI countdown timer with progress display, pause/resume support, and custom alerts. Perfect for Pomodoro sessions, cooking, meetings, or any time-based activity.

## Usage

```bash
dotnet run --project CountdownTimer.csproj -- [duration] [options]
```

### Duration Formats

| Format | Meaning |
|--------|---------|
| `30` | 30 seconds |
| `5m` | 5 minutes |
| `1h30m` | 1 hour 30 minutes |
| `2h15m30s` | 2 hours 15 minutes 30 seconds |

### Options

| Option | Description |
|--------|-------------|
| `--name <n>` | Timer name/label |
| `--message <m>` | Message to display when done |
| `--quiet` | No progress display, just notify when done |
| `--help` | Show help |

## Examples

**Basic 5-minute timer:**
```bash
dotnet run --project CountdownTimer.csproj -- 5m
```

**Pomodoro timer (25 minutes):**
```bash
dotnet run --project CountdownTimer.csproj -- 25m --name "Pomodoro"
```

**Long session with completion message:**
```bash
dotnet run --project CountdownTimer.csproj -- 1h30m --message "Break time!"
```

**Quiet mode (just notify when done):**
```bash
dotnet run --project CountdownTimer.csproj -- 10m --quiet --message "Meeting starting!"
```

## Example Output

```
Timer 'Pomodoro' started: 25m
Press Ctrl+C to cancel

[████████████████████████░░░░░░] 8m 15s remaining

⏰ Timer 'Pomodoro' completed!
>>> Break time! <<<
```

## Concepts Demonstrated

- `Stopwatch` for accurate timing
- Thread sleep and timing control
- Console cursor manipulation for progress display
- Command-line argument parsing
- TimeSpan manipulation and formatting
- Custom exception handling for cancellation
- Progress bar visualization
