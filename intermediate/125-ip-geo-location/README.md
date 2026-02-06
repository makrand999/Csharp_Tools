# IP Geolocation Lookup Tool

Look up geographic and network information for any IP address.

## Usage

```bash
dotnet run --project 125-ip-geo-location.csproj -- [ip-address]
```

## Example

```bash
dotnet run --project 125-ip-geo-location.csproj -- 8.8.8.8
dotnet run --project 125-ip-geo-location.csproj -- 1.1.1.1
```

### Sample Output

```
Looking up IP: 8.8.8.8

=== IP Geolocation ===
IP Address: 8.8.8.8
City: Mountain View
Region: California
Country: United States (US)
Postal Code: 94035
Latitude: 37.386
Longitude: -122.0838
Timezone: America/Los_Angeles
Currency: USD
ASN: AS15169
Organization: Google LLC
```

## Concepts Demonstrated

- IP address validation
- Geolocation APIs
- Network information (ASN, Org)
- Error handling for invalid IPs
- JSON property mapping
