# Weather API Client

Fetch current weather and forecasts using the free Open-Meteo API (no API key required).

## Usage

```bash
dotnet run --project 124-weather-api-client.csproj -- <city> <latitude> <longitude>
```

## Example

```bash
dotnet run --project 124-weather-api-client.csproj -- London 51.5074 -0.1278
dotnet run --project 124-weather-api-client.csproj -- "New York" 40.7128 -74.0060
dotnet run --project 124-weather-api-client.csproj -- Tokyo 35.6762 139.6503
```

### Common Coordinates

| City | Latitude | Longitude |
|------|----------|-----------|
| London | 51.5074 | -0.1278 |
| New York | 40.7128 | -74.0060 |
| Tokyo | 35.6762 | 139.6503 |
| Paris | 48.8566 | 2.3522 |
| Berlin | 52.5200 | 13.4050 |

### Sample Output

```
Fetching weather for: London (51.5074, -0.1278)

=== Current Weather ===
Temperature: 15.2°C
Wind Speed: 12.5 km/h
Wind Direction: 240°

=== 7-Day Forecast ===
2026-04-01: 18°C / 10°C | Precip: 20%
2026-04-02: 16°C / 9°C | Precip: 45%
```

## Concepts Demonstrated

- Open-Meteo API integration
- Coordinate-based queries
- Multi-level JSON parsing
- Array enumeration
- Temperature unit handling
