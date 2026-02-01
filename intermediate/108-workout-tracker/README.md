# Workout Tracker

A CLI application for logging workouts and tracking fitness progress over time.

## Usage

```bash
dotnet run --project WorkoutTracker/WorkoutTracker.csproj
```

## Features

- **Log workouts** with exercise name, sets, reps, and weight
- **View recent workouts** with optional filtering by exercise
- **Track progress** for specific exercises over time
- **View statistics** including total volume and activity metrics
- **Delete workouts** with confirmation
- **Auto-save** to JSON file

## Example

```
=== Workout Tracker ===
1. Log Workout
2. View Workouts
3. View Progress by Exercise
4. View Statistics
5. Delete Workout
6. Save/Export
0. Exit
Choose option: 1

Date (YYYY-MM-DD, or Enter for today): 
Exercise name: Bench Press
Sets: 3
Reps per set: 10
Weight (lbs/kg, or 0 for bodyweight): 135
Notes (optional): Felt strong today
Workout logged: Bench Press - 3x10 @ 135
```

## Concepts Demonstrated

- JSON serialization for data persistence
- DateTime parsing and formatting
- LINQ for filtering, grouping, and aggregation
- Statistics calculation (volume, averages)
- Progress tracking over time
- Interactive CLI with validation
