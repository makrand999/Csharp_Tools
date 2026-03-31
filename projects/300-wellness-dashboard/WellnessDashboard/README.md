# Wellness Dashboard

Comprehensive health and wellness tracking in a single dashboard.

## Usage

```bash
dotnet run --project WellnessDashboard.csproj
```

## Tracked Metrics

| Metric | Goal |
|--------|------|
| 💧 Water | 8 glasses/day |
| 😴 Sleep | 8 hours/night |
| 👟 Steps | 10,000 steps/day |
| 😊 Mood | 5/5 rating |
| 🧘 Exercise | 30 min/day |
| 🥗 Healthy Meals | 3/day |

## Features

- **Quick logging** - Log all metrics at once
- **Daily summary** - View today's progress
- **Weekly overview** - 7-day trends and averages
- **Wellness score** - Overall health score (0-100)
- **Visual progress** - Progress bars for each metric

## Wellness Score Calculation

The score is calculated as an average of:
- Water intake (vs 8 glasses)
- Sleep duration (vs 8 hours)
- Steps (vs 10,000)
- Mood rating (1-5 scale)
- Exercise (vs 30 minutes)

## Example

```
🏥 Wellness Dashboard
====================

Options:
  1. Quick log (all metrics)
  2. View today's summary
  3. Weekly overview
  4. Clear data

Choice: 1

📝 Quick Wellness Check-in

💧 Water (glasses, ~250ml each): 6
😴 Sleep (hours): 7.5
👟 Steps (in thousands): 8
😊 Mood (1-5): 4
🧘 Exercise (minutes): 20
🥗 Healthy meals (count): 2

✅ Wellness data logged!

📊 Today's Wellness Summary
==================================================
💧 Water: 6 glasses (1500ml)
   [███████░░░░░░░░░░░░░░░░]

😴 Sleep: 7.5 hours
   [███████████████████░░░]

👟 Steps: 8,000
   [████████████████░░░░░░]

😊 Mood: 4/5 ⭐⭐⭐⭐

🧘 Exercise: 20 min
   [████████████████░░░░░░]

🥗 Healthy Meals: 2

📈 Wellness Score: 73/100
   [██████████████████░░░░░]
```

## Concepts Demonstrated

- Complex data modeling
- JSON serialization
- Multi-metric aggregation
- Score calculation
- Progress visualization
- Weekly trend analysis
