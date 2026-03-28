# Workout Tracker

Log workouts, track progress, and view fitness statistics.

## Usage

```bash
# Interactive mode
dotnet run --project WorkoutTracker.csproj

# Log workout
dotnet run --project WorkoutTracker.csproj log

# List workouts
dotnet run --project WorkoutTracker.csproj list

# Show statistics
dotnet run --project WorkoutTracker.csproj stats

# Delete workout
dotnet run --project WorkoutTracker.csproj delete abc12345
```

## Example

```
💪 Workout Tracker
==================

Commands:
  log    - Log a workout
  list   - List all workouts
  stats  - Show statistics
  delete - Delete workout

Enter command: log

Date (YYYY-MM-DD, Enter for today):
Workout type (e.g., Running, Weightlifting, Yoga): Running
Duration (minutes): 30
Calories burned (optional): 300
Intensity (1-5, 5 being hardest): 4
Notes (optional): Morning jog in the park

✅ Workout logged: Running
   Date: 2024-01-15
   Duration: 30 min | Intensity: 🔥🔥🔥🔥
```

```
$ dotnet run --project WorkoutTracker.csproj stats

📊 Workout Statistics
==================================================

📈 Overall:
   Total workouts: 25
   Total duration: 1250 min (20.8 hours)
   Total calories: 8,500
   Avg intensity: 3.5/5 🔥🔥🔥🔥

📅 Recent Activity:
   This week: 4 workouts
   This month: 12 workouts

🔥 Streaks:
   Current streak: 5 days
   Longest streak: 14 days

💪 By Type:
   Running: 10 workouts, 450 min
   Weightlifting: 8 workouts, 400 min
   Yoga: 5 workouts, 250 min
   Cycling: 2 workouts, 150 min
```

## Features

- **Workout logging** - Type, duration, calories, intensity
- **Statistics** - Totals, averages, streaks
- **Activity tracking** - Weekly and monthly summaries
- **Streak calculation** - Current and longest streaks
- **Type breakdown** - See your most common workouts
- **Filtering** - View workouts by type

## Data Storage

Workouts are stored in `workouts.json` in the current directory.

## Intensity Levels

| Level | Description |
|-------|-------------|
| 🔥 | Light |
| 🔥🔥 | Moderate |
| 🔥🔥🔥 | Vigorous |
| 🔥🔥🔥🔥 | High |
| 🔥🔥🔥🔥🔥 | Maximum |

## Concepts Demonstrated

- JSON serialization
- Date/time calculations
- LINQ aggregation and grouping
- Streak calculation algorithms
- Statistics computation
