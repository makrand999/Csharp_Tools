# Sleep Timer

Countdown timer with alarm for naps and bedtime.

## Usage

```bash
dotnet run --project SleepTimer.csproj
```

## Preset Durations

| Option | Duration |
|--------|----------|
| 1 | 15 minutes (power nap) |
| 2 | 30 minutes (short nap) |
| 3 | 45 minutes (full cycle) |
| 4 | 1 hour (long rest) |
| 5 | Custom |

## Features

- **Preset options** - Quick selection
- **Countdown display** - Updates every 10 seconds
- **Audio alarm** - Beeps when time is up
- **Cancel anytime** - Ctrl+C to stop

## Use Cases

- Power naps
- Bedtime timer
- Study breaks
- Timed rest periods

## Concepts Demonstrated

- Countdown timing
- Console output formatting
- Audio feedback
- TimeSpan manipulation
