using System.Net.Sockets;

var host = args.Length > 0 ? args[0] : "localhost";
var ports = args.Length > 1 ? ParsePortRange(args[1]) : Enumerable.Range(1, 1024);

Console.WriteLine($"Scanning {host} for open ports...");
Console.WriteLine($"Port range: {ports.Min()}-{ports.Max()}");
Console.WriteLine(new string('-', 50));

var openPorts = new List<int>();
var sw = System.Diagnostics.Stopwatch.StartNew();

foreach (var port in ports)
{
    if (await IsPortOpenAsync(host, port, 500))
    {
        openPorts.Add(port);
        Console.WriteLine($"Port {port,5}: OPEN");
    }
}

sw.Stop();
Console.WriteLine(new string('-', 50));
Console.WriteLine($"Scan completed in {sw.ElapsedMilliseconds}ms");
Console.WriteLine($"Found {openPorts.Count} open port(s): {string.Join(", ", openPorts)}");

static IEnumerable<int> ParsePortRange(string range)
{
    if (range.Contains("-"))
    {
        var parts = range.Split('-');
        var start = int.Parse(parts[0]);
        var end = int.Parse(parts[1]);
        return Enumerable.Range(start, end - start + 1);
    }
    return new[] { int.Parse(range) };
}

static async Task<bool> IsPortOpenAsync(string host, int port, int timeoutMs)
{
    try
    {
        using var tcpClient = new TcpClient();
        var task = tcpClient.ConnectAsync(host, port);
        if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
        {
            await task;
            return tcpClient.Connected;
        }
        return false;
    }
    catch
    {
        return false;
    }
}
