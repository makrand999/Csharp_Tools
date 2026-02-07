# Holiday Calendar

Fetch public holidays for any country using the Nager.Date API.

## Usage

```bash
dotnet run --project 127-holiday-calendar.csproj -- <country-code> [year]
```

## Example

```bash
dotnet run --project 127-holiday-calendar.csproj -- US 2026
dotnet run --project 127-holiday-calendar.csproj -- GB
dotnet run --project 127-holiday-calendar.csproj -- DE 2026
```

### Sample Output

```
Fetching holidays for US (2026)

=== US Holidays (2026) ===

Wed, Jan 01 - New Year's Day
                 [Public]
Mon, Jan 19 - Martin Luther King Jr. Day
                 [Public]
Mon, Feb 16 - Washington's Birthday
                 [Public]
...
```

## Country Codes

US, GB, DE, FR, JP, CN, AU, CA, BR, IN, IT, ES, NL, SE, NO, DK, FI, PL, AT, CH, and many more.

## Concepts Demonstrated

- Date API integration
- Country code handling
- Date formatting
- Holiday type categorization
- Year-based filtering
