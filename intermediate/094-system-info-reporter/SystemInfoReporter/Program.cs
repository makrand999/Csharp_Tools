using System.Runtime.InteropServices;
using System.Text;
using System.Net;

if (args.Contains("--json"))
{
    OutputJson();
}
else if (args.Contains("--brief"))
{
    OutputBrief();
}
else if (args.Contains("--help") || args.Contains("-h"))
{
    ShowHelp();
}
else
{
    OutputFull();
}

static void ShowHelp()
{
    Console.WriteLine("System Info Reporter - Display system specifications");
    Console.WriteLine();
    Console.WriteLine("Usage:");
    Console.WriteLine("  dotnet run --project SystemInfoReporter.csproj [options]");
    Console.WriteLine();
    Console.WriteLine("Options:");
    Console.WriteLine("  --json     Output in JSON format");
    Console.WriteLine("  --brief    Output brief summary only");
    Console.WriteLine("  --help, -h Show this help message");
    Console.WriteLine();
    Console.WriteLine("Examples:");
    Console.WriteLine("  dotnet run --project SystemInfoReporter.csproj");
    Console.WriteLine("  dotnet run --project SystemInfoReporter.csproj --json");
    Console.WriteLine("  dotnet run --project SystemInfoReporter.csproj --brief");
}

static void OutputFull()
{
    Console.WriteLine("═══════════════════════════════════════════════════════════");
    Console.WriteLine("                    SYSTEM INFORMATION                      ");
    Console.WriteLine("═══════════════════════════════════════════════════════════");
    
    Console.WriteLine("\n📌 OPERATING SYSTEM");
    Console.WriteLine($"  OS:              {RuntimeInformation.OSDescription}");
    Console.WriteLine($"  Architecture:    {RuntimeInformation.OSArchitecture}");
    Console.WriteLine($"  Process Arch:    {RuntimeInformation.ProcessArchitecture}");
    Console.WriteLine($"  .NET Version:    {RuntimeInformation.FrameworkDescription}");
    Console.WriteLine($"  Runtime:         {Environment.Version}");
    
    Console.WriteLine("\n💻 HARDWARE");
    Console.WriteLine($"  Processor Count: {Environment.ProcessorCount} cores");
    Console.WriteLine($"  Memory:          {GetTotalMemory()}");
    Console.WriteLine($"  Available Mem:   {GetAvailableMemory()}");
    
    Console.WriteLine("\n👤 USER & MACHINE");
    Console.WriteLine($"  Machine Name:    {Environment.MachineName}");
    Console.WriteLine($"  User Name:       {Environment.UserName}");
    Console.WriteLine($"  User Domain:     {Environment.UserDomainName}");
    Console.WriteLine($"  Is Admin:        {(IsAdministrator() ? "Yes" : "No")}");
    
    Console.WriteLine("\n📁 ENVIRONMENT");
    Console.WriteLine($"  Current Dir:     {Environment.CurrentDirectory}");
    Console.WriteLine($"  System Dir:      {Environment.SystemDirectory}");
    Console.WriteLine($"  Temp Path:       {Path.GetTempPath()}");
    Console.WriteLine($"  Tick Count:      {Environment.TickCount64:N0} ms");
    Console.WriteLine($"  Uptime:          {GetUptime()}");
    
    Console.WriteLine("\n🌐 NETWORK");
    Console.WriteLine($"  Hostname:        {Dns.GetHostName()}");
    var hostEntry = Dns.GetHostEntry(Dns.GetHostName());
    Console.WriteLine($"  IP Addresses:    {string.Join(", ", hostEntry.AddressList.Select(a => a.ToString()))}");
    
    Console.WriteLine("\n📊 RUNTIME STATS");
    Console.WriteLine($"  GC Count:        {GC.CollectionCount(0)} (Gen 0)");
    Console.WriteLine($"                 {GC.CollectionCount(1)} (Gen 1)");
    Console.WriteLine($"                 {GC.CollectionCount(2)} (Gen 2)");
    Console.WriteLine($"  Total Memory:    {GC.GetTotalMemory(false):N0} bytes");
    Console.WriteLine($"  Working Set:     {Environment.WorkingSet:N0} bytes");
    
    Console.WriteLine("\n📀 DISK SPACE");
    foreach (var drive in DriveInfo.GetDrives())
    {
        if (drive.IsReady)
        {
            var percent = (double)drive.TotalSize / drive.TotalFreeSpace / 100;
            Console.WriteLine($"  {drive.Name,-6} {FormatBytes(drive.TotalFreeSpace),10} free / {FormatBytes(drive.TotalSize),10} total ({drive.DriveFormat})");
        }
    }
    
    Console.WriteLine("\n═══════════════════════════════════════════════════════════");
}

static void OutputBrief()
{
    Console.WriteLine($"{RuntimeInformation.OSDescription} | {Environment.MachineName} | {Environment.ProcessorCount} cores | {GetTotalMemory()} RAM");
    Console.WriteLine($".NET {Environment.Version} | Uptime: {GetUptime()}");
}

static void OutputJson()
{
    var info = new
    {
        operatingSystem = new
        {
            description = RuntimeInformation.OSDescription,
            architecture = RuntimeInformation.OSArchitecture.ToString(),
            processArchitecture = RuntimeInformation.ProcessArchitecture.ToString(),
            framework = RuntimeInformation.FrameworkDescription,
            runtimeVersion = Environment.Version.ToString()
        },
        hardware = new
        {
            processorCount = Environment.ProcessorCount,
            totalMemory = GetTotalMemory(),
            availableMemory = GetAvailableMemory()
        },
        user = new
        {
            machineName = Environment.MachineName,
            userName = Environment.UserName,
            userDomainName = Environment.UserDomainName,
            isAdmin = IsAdministrator()
        },
        environment = new
        {
            currentDirectory = Environment.CurrentDirectory,
            systemDirectory = Environment.SystemDirectory,
            tempPath = Path.GetTempPath(),
            uptime = GetUptime(),
            tickCount = Environment.TickCount64
        },
        network = new
        {
            hostname = Dns.GetHostName(),
            ipAddresses = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Select(a => a.ToString()).ToArray()
        },
        runtime = new
        {
            gcCollectionCounts = new
            {
                gen0 = GC.CollectionCount(0),
                gen1 = GC.CollectionCount(1),
                gen2 = GC.CollectionCount(2)
            },
            totalMemory = GC.GetTotalMemory(false),
            workingSet = Environment.WorkingSet
        },
        disks = DriveInfo.GetDrives().Where(d => d.IsReady).Select(d => new
        {
            name = d.Name,
            totalFreeSpace = d.TotalFreeSpace,
            totalSize = d.TotalSize,
            driveFormat = d.DriveFormat,
            driveType = d.DriveType.ToString()
        }).ToArray()
    };
    
    Console.WriteLine(System.Text.Json.JsonSerializer.Serialize(info, new System.Text.Json.JsonSerializerOptions
    {
        WriteIndented = true
    }));
}

static string GetTotalMemory()
{
    try
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var memInfo = File.ReadAllLines("/proc/meminfo");
            var totalLine = memInfo.FirstOrDefault(l => l.StartsWith("MemTotal:"));
            if (totalLine != null)
            {
                var parts = totalLine.Split();
                if (parts.Length >= 2 && long.TryParse(parts[1], out var kb))
                {
                    return FormatBytes(kb * 1024);
                }
            }
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            // Windows: Use PerformanceCounter or WMI (simplified fallback)
            var totalMemory = GC.GetGCMemoryInfo().TotalAvailableMemoryBytes;
            return FormatBytes(totalMemory);
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
        {
            // macOS: simplified estimate
            return FormatBytes(GC.GetGCMemoryInfo().TotalAvailableMemoryBytes);
        }
    }
    catch { }
    
    return "Unknown";
}

static string GetAvailableMemory()
{
    try
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var memInfo = File.ReadAllLines("/proc/meminfo");
            var availableLine = memInfo.FirstOrDefault(l => l.StartsWith("MemAvailable:"));
            if (availableLine != null)
            {
                var parts = availableLine.Split();
                if (parts.Length >= 2 && long.TryParse(parts[1], out var kb))
                {
                    return FormatBytes(kb * 1024);
                }
            }
        }
        
        var gcInfo = GC.GetGCMemoryInfo();
        return FormatBytes(gcInfo.TotalAvailableMemoryBytes - GC.GetTotalMemory(false));
    }
    catch { }
    
    return "Unknown";
}

static string GetUptime()
{
    try
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
        {
            var uptime = File.ReadAllText("/proc/uptime");
            var seconds = double.Parse(uptime.Split('.')[0]);
            return FormatTimeSpan(TimeSpan.FromSeconds(seconds));
        }
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            var tickCount = Environment.TickCount64;
            return FormatTimeSpan(TimeSpan.FromMilliseconds(Math.Abs(tickCount)));
        }
    }
    catch { }
    
    return "Unknown";
}

static string FormatTimeSpan(TimeSpan ts)
{
    if (ts.TotalDays >= 1)
        return $"{(int)ts.TotalDays}d {ts.Hours}h {ts.Minutes}m";
    if (ts.TotalHours >= 1)
        return $"{(int)ts.TotalHours}h {ts.Minutes}m";
    if (ts.TotalMinutes >= 1)
        return $"{(int)ts.TotalMinutes}m {ts.Seconds}s";
    return $"{ts.Seconds}s";
}

static string FormatBytes(long bytes)
{
    string[] sizes = { "B", "KB", "MB", "GB", "TB" };
    int order = 0;
    double size = bytes;
    while (size >= 1024 && order < sizes.Length - 1)
    {
        order++;
        size /= 1024;
    }
    return $"{size:0.##} {sizes[order]}";
}

static bool IsAdministrator()
{
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
    {
        // Windows: Check via System.Security.Principal
        try
        {
            var identity = System.Security.Principal.WindowsIdentity.GetCurrent();
            var principal = new System.Security.Principal.WindowsPrincipal(identity);
            return principal.IsInRole(System.Security.Principal.WindowsBuiltInRole.Administrator);
        }
        catch { return false; }
    }
    
    if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux) || 
        RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
    {
        // Unix: Check if UID is 0
        try
        {
            var idOutput = RunCommand("id", "-u");
            return idOutput?.Trim() == "0";
        }
        catch { return false; }
    }
    
    return false;
}

static string? RunCommand(string cmd, string args)
{
    try
    {
        using var process = new System.Diagnostics.Process();
        process.StartInfo = new System.Diagnostics.ProcessStartInfo
        {
            FileName = cmd,
            Arguments = args,
            RedirectStandardOutput = true,
            UseShellExecute = false,
            CreateNoWindow = true
        };
        process.Start();
        return process.StandardOutput.ReadToEnd();
    }
    catch { return null; }
}
