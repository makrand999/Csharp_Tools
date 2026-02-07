# Timezone Converter

Convert datetimes between different timezones.

## Usage

```bash
dotnet run --project 128-timezone-converter.csproj -- <datetime> <from-tz> <to-tz>
```

## Example

```bash
dotnet run --project 128-timezone-converter.csproj -- "2026-04-01 12:00" "America/New_York" "Asia/Tokyo"
dotnet run --project 128-timezone-converter.csproj -- now "UTC" "Europe/London"
```

### Common Timezones

- UTC
- America/New_York, America/Los_Angeles, America/Chicago
- Europe/London, Europe/Paris, Europe/Berlin
- Asia/Tokyo, Asia/Shanghai, Asia/Dubai
- Australia/Sydney, Pacific/Auckland

### Sample Output

```
=== Timezone Conversion ===

Input:  2026-04-01 12:00:00 (America/New_York)
Output: 2026-04-02 01:00:00 (Asia/Tokyo)

=== Current Time ===
America/New_York: 2026-04-01 08:00:00 (-04:00)
Asia/Tokyo: 2026-04-01 21:00:00 (+09:00)

Time Difference: +13 hours
```

## Concepts Demonstrated

- TimeZoneInfo class
- DateTime conversion
- UTC offset calculation
- Timezone ID handling
- DST awareness
