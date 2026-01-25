# Domain Lookup

DNS and WHOIS lookup tool for domain analysis. Resolves IP addresses, queries DNS records, and retrieves WHOIS information.

## Usage

```bash
# Basic lookup (IP addresses only)
dotnet run --project DomainLookup.csproj example.com

# Include DNS records
dotnet run --project DomainLookup.csproj --dns example.com

# Include WHOIS information
dotnet run --project DomainLookup.csproj --whois example.com

# All information
dotnet run --project DomainLookup.csproj --all example.com

# JSON output
dotnet run --project DomainLookup.csproj --json example.com

# Multiple domains
dotnet run --project DomainLookup.csproj example.com google.com
```

## Example

**Basic Output:**
```
============================================================
Domain: example.com
============================================================

IP Addresses:
  - 93.184.216.34

DNS Information:
  A: 93.184.216.34

WHOIS Information:
  Domain Name: EXAMPLE.COM
  Registry Domain ID: 2336799_DOMAIN_COM-VRSN
  Registrar WHOIS Server: whois.iana.org
  Registrar URL: http://www.reserved.com/
  Updated Date: 2023-08-14T07:01:31Z
  Creation Date: 1995-08-14T04:00:00Z
  ...
```

**JSON Output:**
```json
{
  "Domain": "example.com",
  "Dns": {
    "A": ["93.184.216.34"]
  },
  "Whois": "Domain Name: EXAMPLE.COM\n...",
  "IpAddresses": ["93.184.216.34"],
  "IsValid": true
}
```

## Concepts Demonstrated

- DNS resolution with System.Net.Dns
- TCP socket connections for WHOIS queries
- WHOIS protocol (port 43)
- Domain validation with regex
- URL parsing and normalization
- Async/await for network operations
- JSON serialization
- Command-line argument parsing
- Error handling for network failures
