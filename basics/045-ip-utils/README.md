# IP Address Utilities

A CLI tool for working with IP addresses - validation, subnet calculation, and network information.

## Usage

```bash
dotnet run --project IpUtils.csproj <command> [arguments]
```

## Commands

| Command | Description |
|---------|-------------|
| `info <ip>` | Display detailed IP information |
| `validate <ip>` | Validate IP address format |
| `subnet <ip> <mask>` | Calculate subnet information |
| `range <start> <end>` | List IPs in a range |
| `localhost` | Show local machine IP addresses |

## Examples

```bash
# Show IP information
dotnet run --project IpUtils.csproj info 192.168.1.1

# Validate an IP
dotnet run --project IpUtils.csproj validate 10.0.0.1

# Calculate subnet details
dotnet run --project IpUtils.csproj subnet 192.168.1.0 255.255.255.0

# List IPs in range
dotnet run --project IpUtils.csproj range 192.168.1.1 192.168.1.10

# Show local machine IPs
dotnet run --project IpUtils.csproj localhost
```

## Sample Output

```
IP Address: 192.168.1.1

Address Family: InterNetwork
Version: IPv4
Binary: C0 A8 01 01
Hex: 0xC0A80101

Classification:
  Private: True
  Loopback: False
  Broadcast: False
  Link-local: False

---

IP Address: 192.168.1.0
Subnet Mask: 255.255.255.0

CIDR Notation: /24
Network Address: 192.168.1.0
Broadcast Address: 192.168.1.255
First Usable: 192.168.1.1
Last Usable: 192.168.1.254
Total Hosts: 254
```

## Concepts Demonstrated

- System.Net.IPAddress class
- Byte array manipulation
- Network address calculations
- Bitwise operations (AND, OR, NOT)
- Pattern matching with switch expressions
- Command-line argument parsing
- IPv4 and IPv6 handling
- CIDR notation calculation
