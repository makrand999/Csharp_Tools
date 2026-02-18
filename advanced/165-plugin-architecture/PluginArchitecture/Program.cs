using System.Reflection;
using System.Runtime.Loader;

namespace PluginArchitecture;

/// <summary>
/// Plugin architecture demonstration - shows assembly loading, dynamic types, and extensibility
/// Plugins are discovered, loaded, and executed at runtime without recompilation
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== Plugin Architecture Demo ===\n");

        // Create plugin directory and sample plugins
        var pluginDir = Path.Combine(AppContext.BaseDirectory, "Plugins");
        Directory.CreateDirectory(pluginDir);
        
        // Create sample plugin source files
        CreateSamplePlugins(pluginDir);

        // Initialize plugin manager
        var pluginManager = new PluginManager();

        // Discover and load plugins
        Console.WriteLine("1. Discovering plugins...");
        pluginManager.DiscoverPlugins(pluginDir);
        Console.WriteLine($"   Found {pluginManager.PluginCount} plugins");

        // List loaded plugins
        Console.WriteLine("\n2. Loaded Plugins:");
        foreach (var plugin in pluginManager.GetPluginInfo())
        {
            Console.WriteLine($"   - {plugin.Name} v{plugin.Version} by {plugin.Author}");
            Console.WriteLine($"     Description: {plugin.Description}");
            Console.WriteLine($"     Assembly: {plugin.AssemblyName}");
        }

        // Execute all plugins
        Console.WriteLine("\n3. Executing all plugins:");
        pluginManager.ExecuteAll();

        // Execute plugins by category
        Console.WriteLine("\n4. Executing 'utility' plugins only:");
        pluginManager.ExecuteByCategory("utility");

        // Execute plugins by priority
        Console.WriteLine("\n5. Executing plugins by priority (high first):");
        pluginManager.ExecuteByPriority();

        // Demo: Create custom plugin context
        Console.WriteLine("\n6. Executing with custom context:");
        var context = new PluginContext
        {
            UserName = "Admin",
            Environment = "Production",
            Timestamp = DateTime.UtcNow,
            Data = new Dictionary<string, object>
            {
                ["ItemCount"] = 42,
                ["DebugMode"] = true
            }
        };
        pluginManager.ExecuteWithContext(context);

        // Unload plugins (demonstrates AssemblyLoadContext)
        Console.WriteLine("\n7. Unloading plugins:");
        pluginManager.UnloadAll();
        Console.WriteLine("   All plugins unloaded");
    }

    static void CreateSamplePlugins(string pluginDir)
    {
        // Create plugin 1: DataProcessor
        var plugin1Code = """
            using System;
            using PluginArchitecture;

            public class DataProcessorPlugin : IPlugin
            {
                public string Name => "Data Processor";
                public string Version => "1.0.0";
                public string Author => "DevTeam";
                public string Description => "Processes data files and generates reports";
                public string Category => "utility";
                public int Priority => 10;

                public void Initialize(IPluginHost host)
                {
                    host.Log($"[{Name}] Initialized");
                }

                public void Execute(PluginContext context)
                {
                    Console.WriteLine($"[{Name}] Processing data for user: {context.UserName}");
                    Console.WriteLine($"[{Name}] Environment: {context.Environment}");
                    if (context.Data.TryGetValue("ItemCount", out var count))
                    {
                        Console.WriteLine($"[{Name}] Processing {count} items...");
                    }
                }

                public void Shutdown()
                {
                    Console.WriteLine($"[{Name}] Shutdown complete");
                }
            }
            """;

        // Create plugin 2: Validator
        var plugin2Code = """
            using System;
            using PluginArchitecture;

            public class ValidatorPlugin : IPlugin
            {
                public string Name => "Validator";
                public string Version => "2.1.0";
                public string Author => "QA Team";
                public string Description => "Validates input data and configuration";
                public string Category => "utility";
                public int Priority => 20;

                public void Initialize(IPluginHost host)
                {
                    host.Log($"[{Name}] Ready to validate");
                }

                public void Execute(PluginContext context)
                {
                    Console.WriteLine($"[{Name}] Validating context for: {context.UserName}");
                    Console.WriteLine($"[{Name}] Timestamp: {context.Timestamp:O}");
                    foreach (var kvp in context.Data)
                    {
                        Console.WriteLine($"[{Name}] Validated: {kvp.Key} = {kvp.Value}");
                    }
                }

                public void Shutdown()
                {
                    Console.WriteLine($"[{Name}] Validation complete");
                }
            }
            """;

        // Create plugin 3: Reporter
        var plugin3Code = """
            using System;
            using PluginArchitecture;

            public class ReporterPlugin : IPlugin
            {
                public string Name => "Reporter";
                public string Version => "1.5.2";
                public string Author => "Analytics Team";
                public string Description => "Generates detailed execution reports";
                public string Category => "reporting";
                public int Priority => 5;

                public void Initialize(IPluginHost host)
                {
                    host.Log($"[{Name}] Reporting module ready");
                }

                public void Execute(PluginContext context)
                {
                    Console.WriteLine($"[{Name}] === EXECUTION REPORT ===");
                    Console.WriteLine($"[{Name}] User: {context.UserName}");
                    Console.WriteLine($"[{Name}] Environment: {context.Environment}");
                    Console.WriteLine($"[{Name}] Time: {context.Timestamp:yyyy-MM-dd HH:mm:ss}");
                    Console.WriteLine($"[{Name}] Data points: {context.Data.Count}");
                    Console.WriteLine($"[{Name}] =========================");
                }

                public void Shutdown()
                {
                    Console.WriteLine($"[{Name}] Report finalized");
                }
            }
            """;

        // Create plugin 4: HighPriority
        var plugin4Code = """
            using System;
            using PluginArchitecture;

            public class HighPriorityPlugin : IPlugin
            {
                public string Name => "High Priority Handler";
                public string Version => "1.0.0";
                public string Author => "Core Team";
                public string Description => "Handles high-priority tasks first";
                public string Category => "core";
                public int Priority => 100;

                public void Initialize(IPluginHost host)
                {
                    host.Log($"[{Name}] Critical system ready");
                }

                public void Execute(PluginContext context)
                {
                    Console.WriteLine($"[{Name}] *** HIGH PRIORITY TASK ***");
                    Console.WriteLine($"[{Name}] Executing for: {context.UserName}");
                    Console.WriteLine($"[{Name}] Debug mode: {context.Data.GetValueOrDefault("DebugMode", false)}");
                }

                public void Shutdown()
                {
                    Console.WriteLine($"[{Name}] Critical tasks complete");
                }
            }
            """;

        // Write plugin source files (for demonstration - in real scenario these would be compiled DLLs)
        File.WriteAllText(Path.Combine(pluginDir, "DataProcessorPlugin.cs"), plugin1Code);
        File.WriteAllText(Path.Combine(pluginDir, "ValidatorPlugin.cs"), plugin2Code);
        File.WriteAllText(Path.Combine(pluginDir, "ReporterPlugin.cs"), plugin3Code);
        File.WriteAllText(Path.Combine(pluginDir, "HighPriorityPlugin.cs"), plugin4Code);

        Console.WriteLine("   Created 4 sample plugin source files in: " + pluginDir);
    }
}

// === Plugin Contract ===

/// <summary>
/// Base interface that all plugins must implement
/// </summary>
public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    string Author { get; }
    string Description { get; }
    string Category { get; }
    int Priority { get; }

    void Initialize(IPluginHost host);
    void Execute(PluginContext context);
    void Shutdown();
}

/// <summary>
/// Plugin host interface - provides services to plugins
/// </summary>
public interface IPluginHost
{
    void Log(string message);
    T GetService<T>() where T : class;
    string ApplicationPath { get; }
    string Version { get; }
}

/// <summary>
/// Context passed to plugins during execution
/// </summary>
public class PluginContext
{
    public string UserName { get; set; } = "";
    public string Environment { get; set; } = "";
    public DateTime Timestamp { get; set; }
    public Dictionary<string, object> Data { get; set; } = new();
}

// === Plugin Host Implementation ===

public class PluginHost : IPluginHost
{
    private readonly Dictionary<Type, object> _services = new();

    public string ApplicationPath => AppContext.BaseDirectory;
    public string Version => "1.0.0";

    public void RegisterService<T>(T service) where T : class
    {
        _services[typeof(T)] = service;
    }

    public T GetService<T>() where T : class
    {
        return _services.TryGetValue(typeof(T), out var service) 
            ? (T)service 
            : throw new InvalidOperationException($"Service {typeof(T).Name} not registered");
    }

    public void Log(string message)
    {
        Console.WriteLine($"   [HOST] {message}");
    }
}

// === Plugin Manager ===

public class PluginManager
{
    private readonly List<PluginWrapper> _plugins = new();
    private readonly List<AssemblyLoadContext> _loadContexts = new();
    private readonly PluginHost _host = new();

    public int PluginCount => _plugins.Count;

    public void DiscoverPlugins(string pluginDirectory)
    {
        if (!Directory.Exists(pluginDirectory))
        {
            Console.WriteLine($"   Plugin directory not found: {pluginDirectory}");
            return;
        }

        // In a real scenario, we would load .dll files
        // For this demo, we compile and load from source
        var dllFiles = Directory.GetFiles(pluginDirectory, "*.dll");
        
        if (dllFiles.Length == 0)
        {
            // Compile plugins from source for demo
            CompilePlugins(pluginDirectory);
            dllFiles = Directory.GetFiles(pluginDirectory, "*.dll");
        }

        foreach (var dllFile in dllFiles)
        {
            try
            {
                LoadPlugin(dllFile);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Failed to load plugin: {dllFile} - {ex.Message}");
            }
        }
    }

    void CompilePlugins(string pluginDirectory)
    {
        // For demo purposes, we simulate compilation
        // In production, use Roslyn API to compile C# source to DLL
        Console.WriteLine("   Compiling plugins from source (simulated)...");
        
        // Create dummy DLL files to simulate compiled plugins
        var sourceFiles = Directory.GetFiles(pluginDirectory, "*.cs");
        foreach (var sourceFile in sourceFiles)
        {
            var dllPath = Path.ChangeExtension(sourceFile, ".dll");
            // Create empty file to represent compiled assembly
            File.WriteAllBytes(dllPath, new byte[] { 0x4D, 0x5A }); // MZ header
            Console.WriteLine($"   Compiled: {Path.GetFileName(sourceFile)}");
        }
    }

    void LoadPlugin(string dllPath)
    {
        // Create isolated load context for each plugin
        var loadContext = new AssemblyLoadContext(Path.GetFileNameWithoutExtension(dllPath), isCollectible: true);
        _loadContexts.Add(loadContext);

        // Load assembly
        var assembly = loadContext.LoadFromAssemblyPath(dllPath);

        // Find plugin types
        foreach (var type in assembly.GetTypes())
        {
            if (typeof(IPlugin).IsAssignableFrom(type) && !type.IsInterface && !type.IsAbstract)
            {
                var plugin = (IPlugin)Activator.CreateInstance(type)!;
                var wrapper = new PluginWrapper(plugin, loadContext);
                _plugins.Add(wrapper);
                plugin.Initialize(_host);
            }
        }
    }

    public IEnumerable<PluginInfo> GetPluginInfo()
    {
        return _plugins.Select(p => new PluginInfo
        {
            Name = p.Plugin.Name,
            Version = p.Plugin.Version,
            Author = p.Plugin.Author,
            Description = p.Plugin.Description,
            Category = p.Plugin.Category,
            Priority = p.Plugin.Priority,
            AssemblyName = p.LoadContext.Name
        });
    }

    public void ExecuteAll()
    {
        foreach (var wrapper in _plugins)
        {
            wrapper.Plugin.Execute(new PluginContext());
        }
    }

    public void ExecuteByCategory(string category)
    {
        foreach (var wrapper in _plugins.Where(p => p.Plugin.Category.Equals(category, StringComparison.OrdinalIgnoreCase)))
        {
            wrapper.Plugin.Execute(new PluginContext());
        }
    }

    public void ExecuteByPriority()
    {
        foreach (var wrapper in _plugins.OrderByDescending(p => p.Plugin.Priority))
        {
            wrapper.Plugin.Execute(new PluginContext());
        }
    }

    public void ExecuteWithContext(PluginContext context)
    {
        foreach (var wrapper in _plugins.OrderByDescending(p => p.Plugin.Priority))
        {
            wrapper.Plugin.Execute(context);
        }
    }

    public void UnloadAll()
    {
        foreach (var wrapper in _plugins)
        {
            wrapper.Plugin.Shutdown();
        }

        foreach (var loadContext in _loadContexts)
        {
            loadContext.Unload();
        }

        _plugins.Clear();
        _loadContexts.Clear();

        // Force GC to collect unloaded assemblies
        GC.Collect();
        GC.WaitForPendingFinalizers();
    }
}

class PluginWrapper
{
    public IPlugin Plugin { get; }
    public AssemblyLoadContext LoadContext { get; }

    public PluginWrapper(IPlugin plugin, AssemblyLoadContext loadContext)
    {
        Plugin = plugin;
        LoadContext = loadContext;
    }
}

public class PluginInfo
{
    public string Name { get; set; } = "";
    public string Version { get; set; } = "";
    public string Author { get; set; } = "";
    public string Description { get; set; } = "";
    public string Category { get; set; } = "";
    public int Priority { get; set; }
    public string AssemblyName { get; set; } = "";
}
