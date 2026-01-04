# DateTime

Demonstrates C# DateTime operations including creation, formatting, arithmetic, parsing, and time calculations.

## Usage

```bash
dotnet run --project DateTime/DateTime.csproj
```

## Example

```
=== DateTime Operations in C# ===

=== Current Date/Time ===
Now: 3/31/2026 10:45:32 AM
Today (date only): 3/31/2026 12:00:00 AM
UTC Now: 3/31/2026 3:45:32 PM

=== Specific Dates ===
New Year 2026: 1/1/2026 12:00:00 AM
Birthday: 6/15/1990 10:30:00 AM

=== Date Components ===
Year: 2026
Month: 3 (March)
Day: 31 (Tuesday)
Hour: 10
Minute: 45
Second: 32
Day of Year: 90
Is Leap Year: False

=== Date Formatting ===
Short date: 3/31/2026
Long date: Tuesday, March 31, 2026
Short time: 10:45 AM
Long time: 10:45:32 AM
Full date/time: Tuesday, March 31, 2026 10:45:32 AM
Custom (yyyy-MM-dd): 2026-03-31
Custom (dd/MM/yyyy): 31/03/2026
Custom (MMMM dd, yyyy): March 31, 2026

=== Date Arithmetic ===
Tomorrow: 2026-04-01
Next week: 2026-04-07
Next month: 2026-04-30
Next year: 2026-03-31
Yesterday: 2026-03-30
Last month: 2026-02-28

=== Time Span ===
Days until New Year 2026: -89
Total hours: -2135

One week from now: 2026-04-07

=== Age Calculation ===
Born: 1990-06-15
Current age: 35 years

=== Date Comparison ===
1/1/2026 < 12/31/2026: True
1/1/2026 > 12/31/2026: False
1/1/2026 == 1/1/2026: True

=== Parsing Dates ===
Parsed '2026-07-04': Saturday, July 04, 2026
Parsed '12/25/2026': December 25, 2026

=== DateTime Kinds ===
Now.Kind: Local
UtcNow.Kind: Utc
Local: 6/15/2026 2:30:00 PM
As UTC: 6/15/2026 7:30:00 AM

=== Useful Properties ===
Days in current month: 31
First day of month: 2026-03-01
Last day of month: 31

=== Stopwatch ===
Elapsed: 100ms
```

## Concepts Demonstrated

- `DateTime.Now`, `DateTime.Today`, `DateTime.UtcNow`
- Creating DateTime instances
- Date components (Year, Month, Day, Hour, etc.)
- Date formatting (standard and custom formats)
- Date arithmetic (`AddDays`, `AddMonths`, `AddYears`)
- `TimeSpan` for durations
- Date comparison operations
- Parsing strings to DateTime (`TryParse`)
- DateTime kinds (Local, UTC, Unspecified)
- Time zone conversion
- Age calculation logic
- `DateTime.DaysInMonth` utility
- `Stopwatch` for precise timing
