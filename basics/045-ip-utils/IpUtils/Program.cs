using System;
using System.Net;
using System.Net.Sockets;

namespace IpUtils;

class Program
{
    static int Main(string[] args)
    {
        if (args.Length == 0)
        {
            Console.WriteLine("IP Address Utilities");
            Console.WriteLine("Usage: dotnet run --project IpUtils.csproj <command> [arguments]");
            Console.WriteLine();
            Console.WriteLine("Commands:");
            Console.WriteLine("  info <ip>           - Display IP information");
            Console.WriteLine("  validate <ip>       - Validate IP address format");
            Console.WriteLine("  subnet <ip> <mask>  - Calculate subnet information");
            Console.WriteLine("  range <start> <end> - List IPs in range");
            Console.WriteLine("  localhost           - Show local machine IP addresses");
            Console.WriteLine();
            Console.WriteLine("Examples:");
            Console.WriteLine("  dotnet run --project IpUtils.csproj info 192.168.1.1");
            Console.WriteLine("  dotnet run --project IpUtils.csproj subnet 192.168.1.0 255.255.255.0");
            Console.WriteLine("  dotnet run --project IpUtils.csproj localhost");
            return 1;
        }

        string command = args[0].ToLower();

        try
        {
            return command switch
            {
                "info" when args.Length >= 2 => ShowInfo(args[1]),
                "validate" when args.Length >= 2 => ValidateIp(args[1]),
                "subnet" when args.Length >= 3 => ShowSubnet(args[1], args[2]),
                "range" when args.Length >= 3 => ShowRange(args[1], args[2]),
                "localhost" => ShowLocalhost(),
                _ => ShowInfo(args[0])
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            return 1;
        }
    }

    static int ShowInfo(string ipString)
    {
        if (!IPAddress.TryParse(ipString, out var ip))
            throw new ArgumentException($"Invalid IP address: {ipString}");

        Console.WriteLine($"IP Address: {ipString}");
        Console.WriteLine();
        Console.WriteLine($"Address Family: {ip.AddressFamily}");
        Console.WriteLine($"Version: {(ip.AddressFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6")}");
        
        if (ip.AddressFamily == AddressFamily.InterNetwork)
        {
            var bytes = ip.GetAddressBytes();
            Console.WriteLine($"Binary: {BitConverter.ToString(bytes).Replace("-", " ")}");
            Console.WriteLine($"Hex: 0x{BitConverter.ToUInt32(bytes.Reverse().ToArray(), 0):X8}");
            
            // Check special addresses
            Console.WriteLine();
            Console.WriteLine("Classification:");
            Console.WriteLine($"  Private: {IsPrivate(ip)}");
            Console.WriteLine($"  Loopback: {IPAddress.IsLoopback(ip)}");
            Console.WriteLine($"  Broadcast: {IsBroadcast(ip)}");
            Console.WriteLine($"  Link-local: {IsLinkLocal(ip)}");
        }
        else
        {
            Console.WriteLine($"Is Loopback: {IPAddress.IsLoopback(ip)}");
            Console.WriteLine($"Is Link-local: {IsIpv6LinkLocal(ip)}");
        }

        return 0;
    }

    static int ValidateIp(string ipString)
    {
        Console.WriteLine($"Validating: {ipString}");
        
        if (IPAddress.TryParse(ipString, out var ip))
        {
            Console.WriteLine($"✓ Valid {(ip.AddressFamily == AddressFamily.InterNetwork ? "IPv4" : "IPv6")} address");
            return 0;
        }
        else
        {
            Console.WriteLine("✗ Invalid IP address format");
            return 1;
        }
    }

    static int ShowSubnet(string ipString, string maskString)
    {
        if (!IPAddress.TryParse(ipString, out var ip))
            throw new ArgumentException($"Invalid IP address: {ipString}");
        if (!IPAddress.TryParse(maskString, out var mask))
            throw new ArgumentException($"Invalid subnet mask: {maskString}");

        if (ip.AddressFamily != mask.AddressFamily)
            throw new ArgumentException("IP and mask must be same version (IPv4/IPv6)");

        var ipBytes = ip.GetAddressBytes();
        var maskBytes = mask.GetAddressBytes();
        
        // Calculate network address
        var networkBytes = new byte[ipBytes.Length];
        var broadcastBytes = new byte[ipBytes.Length];
        int cidr = 0;

        for (int i = 0; i < ipBytes.Length; i++)
        {
            networkBytes[i] = (byte)(ipBytes[i] & maskBytes[i]);
            broadcastBytes[i] = (byte)(ipBytes[i] | ~maskBytes[i]);
            
            // Count CIDR bits
            byte m = maskBytes[i];
            while (m != 0)
            {
                cidr += m & 1;
                m >>= 1;
            }
        }

        var network = new IPAddress(networkBytes);
        var broadcast = new IPAddress(broadcastBytes);

        Console.WriteLine($"IP Address: {ip}");
        Console.WriteLine($"Subnet Mask: {mask}");
        Console.WriteLine();
        Console.WriteLine($"CIDR Notation: /{cidr}");
        Console.WriteLine($"Network Address: {network}");
        Console.WriteLine($"Broadcast Address: {broadcast}");
        Console.WriteLine($"First Usable: {GetFirstUsable(networkBytes)}");
        Console.WriteLine($"Last Usable: {GetLastUsable(broadcastBytes)}");
        Console.WriteLine($"Total Hosts: {Math.Pow(2, (ipBytes.Length * 8) - cidr) - 2:N0}");

        return 0;
    }

    static int ShowRange(string startString, string endString)
    {
        if (!IPAddress.TryParse(startString, out var start))
            throw new ArgumentException($"Invalid start IP: {startString}");
        if (!IPAddress.TryParse(endString, out var end))
            throw new ArgumentException($"Invalid end IP: {endString}");

        Console.WriteLine($"IP Range: {start} to {end}");
        Console.WriteLine();

        var current = start.GetAddressBytes();
        var endBytes = end.GetAddressBytes();
        int count = 0;

        while (CompareIps(current, endBytes) <= 0 && count < 50)
        {
            Console.WriteLine(new IPAddress(current));
            IncrementIp(current);
            count++;
        }

        if (count >= 50)
            Console.WriteLine("... (limited to 50 addresses)");

        Console.WriteLine();
        Console.WriteLine($"Total addresses: {CountIps(start.GetAddressBytes(), endBytes):N0}");
        return 0;
    }

    static int ShowLocalhost()
    {
        string hostname = Dns.GetHostName();
        Console.WriteLine($"Hostname: {hostname}");
        Console.WriteLine();

        var addresses = Dns.GetHostAddresses(hostname);
        Console.WriteLine($"IP Addresses ({addresses.Length}):");
        
        foreach (var addr in addresses)
        {
            if (addr.AddressFamily == AddressFamily.InterNetwork)
                Console.WriteLine($"  IPv4: {addr}");
            else if (addr.AddressFamily == AddressFamily.InterNetworkV6)
                Console.WriteLine($"  IPv6: {addr}");
        }

        Console.WriteLine();
        Console.WriteLine("Loopback addresses:");
        Console.WriteLine($"  IPv4: {IPAddress.Loopback}");
        Console.WriteLine($"  IPv6: {IPAddress.IPv6Loopback}");

        return 0;
    }

    // Helper methods
    static bool IsPrivate(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        
        // 10.0.0.0/8
        if (bytes[0] == 10) return true;
        
        // 172.16.0.0/12
        if (bytes[0] == 172 && bytes[1] >= 16 && bytes[1] <= 31) return true;
        
        // 192.168.0.0/16
        if (bytes[0] == 192 && bytes[1] == 168) return true;
        
        return false;
    }

    static bool IsBroadcast(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        return bytes.All(b => b == 255);
    }

    static bool IsLinkLocal(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        return bytes[0] == 169 && bytes[1] == 254;
    }

    static bool IsIpv6LinkLocal(IPAddress ip)
    {
        var bytes = ip.GetAddressBytes();
        return bytes[0] == 0xFE && (bytes[1] & 0xC0) == 0x80;
    }

    static string GetFirstUsable(byte[] network)
    {
        var result = (byte[])network.Clone();
        result[^1] += 1;
        return new IPAddress(result).ToString();
    }

    static string GetLastUsable(byte[] broadcast)
    {
        var result = (byte[])broadcast.Clone();
        result[^1] -= 1;
        return new IPAddress(result).ToString();
    }

    static void IncrementIp(byte[] ip)
    {
        for (int i = ip.Length - 1; i >= 0; i--)
        {
            if (ip[i] < 255)
            {
                ip[i]++;
                return;
            }
            ip[i] = 0;
        }
    }

    static int CompareIps(byte[] a, byte[] b)
    {
        for (int i = 0; i < a.Length; i++)
        {
            if (a[i] != b[i]) return a[i].CompareTo(b[i]);
        }
        return 0;
    }

    static long CountIps(byte[] start, byte[] end)
    {
        long count = 1;
        for (int i = 0; i < start.Length; i++)
        {
            count = count * 256 + (end[i] - start[i]);
        }
        return count;
    }
}
