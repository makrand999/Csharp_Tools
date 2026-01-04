// Program #14: DateTime - Demonstrates date/time operations, formatting, and calculations

Console.WriteLine("=== DateTime Operations in C# ===\n");

// Current date and time
DateTime now = DateTime.Now;
Console.WriteLine($"=== Current Date/Time ===");
Console.WriteLine($"Now: {now}");
Console.WriteLine($"Today (date only): {DateTime.Today}");
Console.WriteLine($"UTC Now: {DateTime.UtcNow}\n");

// Specific dates
DateTime newYear = new DateTime(2026, 1, 1);
DateTime birthday = new DateTime(1990, 6, 15, 10, 30, 0);
Console.WriteLine($"=== Specific Dates ===");
Console.WriteLine($"New Year 2026: {newYear}");
Console.WriteLine($"Birthday: {birthday}\n");

// Date components
Console.WriteLine($"=== Date Components ===");
Console.WriteLine($"Year: {now.Year}");
Console.WriteLine($"Month: {now.Month} ({now:MMMM})");
Console.WriteLine($"Day: {now.Day} ({now:dddd})");
Console.WriteLine($"Hour: {now.Hour}");
Console.WriteLine($"Minute: {now.Minute}");
Console.WriteLine($"Second: {now.Second}");
Console.WriteLine($"Day of Year: {now.DayOfYear}");
Console.WriteLine($"Is Leap Year: {DateTime.IsLeapYear(now.Year)}\n");

// Date formatting
Console.WriteLine($"=== Date Formatting ===");
Console.WriteLine($"Short date: {now:d}");
Console.WriteLine($"Long date: {now:D}");
Console.WriteLine($"Short time: {now:t}");
Console.WriteLine($"Long time: {now:T}");
Console.WriteLine($"Full date/time: {now:F}");
Console.WriteLine($"Custom (yyyy-MM-dd): {now:yyyy-MM-dd}");
Console.WriteLine($"Custom (dd/MM/yyyy): {now:dd/MM/yyyy}");
Console.WriteLine($"Custom (MMMM dd, yyyy): {now:MMMM dd, yyyy}\n");

// Date arithmetic
Console.WriteLine($"=== Date Arithmetic ===");
Console.WriteLine($"Tomorrow: {now.AddDays(1):yyyy-MM-dd}");
Console.WriteLine($"Next week: {now.AddDays(7):yyyy-MM-dd}");
Console.WriteLine($"Next month: {now.AddMonths(1):yyyy-MM-dd}");
Console.WriteLine($"Next year: {now.AddYears(1):yyyy-MM-dd}");
Console.WriteLine($"Yesterday: {now.AddDays(-1):yyyy-MM-dd}");
Console.WriteLine($"Last month: {now.AddMonths(-1):yyyy-MM-dd}\n");

// Time span (duration between dates)
Console.WriteLine($"=== Time Span ===");
TimeSpan untilNewYear = newYear - now;
Console.WriteLine($"Days until New Year 2026: {untilNewYear.Days}");
Console.WriteLine($"Total hours: {untilNewYear.TotalHours:F0}\n");

TimeSpan oneWeek = TimeSpan.FromDays(7);
Console.WriteLine($"One week from now: {(now + oneWeek):yyyy-MM-dd}\n");

// Age calculation example
DateTime birthDate = new DateTime(1990, 6, 15);
int age = now.Year - birthDate.Year;
if (now < birthDate.AddYears(age))
{
    age--;
}
Console.WriteLine($"=== Age Calculation ===");
Console.WriteLine($"Born: {birthDate:yyyy-MM-dd}");
Console.WriteLine($"Current age: {age} years\n");

// Comparing dates
Console.WriteLine($"=== Date Comparison ===");
DateTime date1 = new DateTime(2026, 1, 1);
DateTime date2 = new DateTime(2026, 12, 31);
Console.WriteLine($"{date1:d} < {date2:d}: {date1 < date2}");
Console.WriteLine($"{date1:d} > {date2:d}: {date1 > date2}");
Console.WriteLine($"{date1:d} == {date2:d}: {date1 == date2}\n");

// Parse and TryParse
Console.WriteLine($"=== Parsing Dates ===");
string dateString = "2026-07-04";
if (DateTime.TryParse(dateString, out DateTime parsedDate))
{
    Console.WriteLine($"Parsed '{dateString}': {parsedDate:D}");
}

string usFormat = "12/25/2026";
if (DateTime.TryParse(usFormat, out DateTime christmas))
{
    Console.WriteLine($"Parsed '{usFormat}': {christmas:MMMM dd, yyyy}\n");
}

// DateTime kinds (Local, UTC, Unspecified)
Console.WriteLine($"=== DateTime Kinds ===");
Console.WriteLine($"Now.Kind: {now.Kind}");
Console.WriteLine($"UtcNow.Kind: {DateTime.UtcNow.Kind}");

// Convert to UTC
DateTime localTime = new DateTime(2026, 6, 15, 14, 30, 0, DateTimeKind.Local);
Console.WriteLine($"Local: {localTime}");
Console.WriteLine($"As UTC: {localTime.ToUniversalTime()}\n");

// Useful DateTime properties
Console.WriteLine($"=== Useful Properties ===");
Console.WriteLine($"Days in current month: {DateTime.DaysInMonth(now.Year, now.Month)}");
Console.WriteLine($"First day of month: {now:yyyy-MM-01}");
Console.WriteLine($"Last day of month: {DateTime.DaysInMonth(now.Year, now.Month)}\n");

// Stopwatch for timing (related to DateTime)
var stopwatch = System.Diagnostics.Stopwatch.StartNew();
System.Threading.Thread.Sleep(100);  // Simulate work
stopwatch.Stop();
Console.WriteLine($"=== Stopwatch ===");
Console.WriteLine($"Elapsed: {stopwatch.ElapsedMilliseconds}ms");
