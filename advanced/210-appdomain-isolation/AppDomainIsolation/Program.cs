using System;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;

namespace AppDomainIsolation;

/// <summary>
/// Demonstrates assembly isolation using AssemblyLoadContext.
/// Shows how to load and unload plugins safely in .NET Core/.NET 5+.
/// </summary>
class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== AppDomain Isolation Demo ===\n");
        Console.WriteLine("Demonstrates assembly isolation and unloading.\n");

        // Demo 1: Isolated plugin loading
        Console.WriteLine("--- Isolated Plugin Loading ---\n");
        IsolatedPluginDemo();

        // Demo 2: Memory isolation
        Console.WriteLine("\n--- Memory Isolation ---\n");
        MemoryIsolationDemo();

        // Demo 3: Version isolation
        Console.WriteLine("\n--- Version Isolation ---\n");
        VersionIsolationDemo();

        // Demo 4: Safe plugin execution
        Console.WriteLine("\n--- Safe Plugin Execution ---\n");
        SafePluginExecutionDemo();

        Console.WriteLine("\n--- Final State ---\n");
        Console.WriteLine($"GC Memory: {GC.GetTotalMemory(false):N0} bytes");
    }

    static void IsolatedPluginDemo()
    {
        Console.WriteLine("Creating isolated load context...");
        var context = new PluginLoadContext("PluginContext");

        Console.WriteLine($"Context created: {context.Name}");
        Console.WriteLine($"Is collectible: {context.IsCollectible}");

        // Simulate plugin loading
        Console.WriteLine("\nLoading plugin assembly...");
        try
        {
            // Load a system assembly as a "plugin"
            var pluginAssembly = context.LoadFromAssemblyName(new AssemblyName("System.Text.Json"));
            Console.WriteLine($"Plugin loaded: {pluginAssembly.GetName().Name}");
            Console.WriteLine($"  Version: {pluginAssembly.GetName().Version}");
            Console.WriteLine($"  Context: {context.Name}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load error: {ex.Message}");
        }

        // Unload the context
        Console.WriteLine("\nUnloading plugin context...");
        context.Unload();
        Console.WriteLine("Context unloaded - plugin assemblies can now be GC'd");

        // Force GC to collect unloaded context
        GC.Collect();
        GC.WaitForPendingFinalizers();
        Console.WriteLine("GC completed");
    }

    static void MemoryIsolationDemo()
    {
        Console.WriteLine("Demonstrating memory isolation...");

        // Create context with large assembly
        var context1 = new PluginLoadContext("MemoryContext1");
        long memoryBefore = GC.GetTotalMemory(true);

        Console.WriteLine($"Memory before: {memoryBefore:N0} bytes");

        long memoryAfter = memoryBefore;
        
        // Load assembly
        try
        {
            var assembly = context1.LoadFromAssemblyName(new AssemblyName("System.Text.Json"));
            memoryAfter = GC.GetTotalMemory(true);
            Console.WriteLine($"Memory after load: {memoryAfter:N0} bytes");
            Console.WriteLine($"Memory increase: {memoryAfter - memoryBefore:N0} bytes");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Load error: {ex.Message}");
        }

        // Unload and collect
        context1.Unload();
        GC.Collect();
        GC.WaitForPendingFinalizers();

        long memoryFinal = GC.GetTotalMemory(true);
        Console.WriteLine($"Memory after unload: {memoryFinal:N0} bytes");
        Console.WriteLine($"Memory recovered: {memoryAfter - memoryFinal:N0} bytes");
    }

    static void VersionIsolationDemo()
    {
        Console.WriteLine("Version isolation allows loading different versions side-by-side");
        Console.WriteLine("In .NET Core/.NET 5+, AssemblyLoadContext enables:");
        Console.WriteLine("  - Loading v1.0.0 in Context A");
        Console.WriteLine("  - Loading v2.0.0 in Context B");
        Console.WriteLine("  - Both versions coexist without conflict");
        Console.WriteLine("\nThis is useful for:");
        Console.WriteLine("  - Plugin systems with different dependency versions");
        Console.WriteLine("  - Multi-tenant applications");
        Console.WriteLine("  - Testing different library versions");
    }

    static void SafePluginExecutionDemo()
    {
        Console.WriteLine("Creating sandboxed plugin context...");
        
        var sandbox = new SandboxLoadContext();
        
        try
        {
            // In a real scenario, you would:
            // 1. Load untrusted plugin code in sandbox context
            // 2. Execute through known interfaces
            // 3. Catch any exceptions
            // 4. Unload context to clean up
            
            Console.WriteLine("Sandbox created for untrusted code execution");
            Console.WriteLine("Plugin execution patterns:");
            Console.WriteLine("  1. Define plugin interface in shared assembly");
            Console.WriteLine("  2. Load plugin in isolated context");
            Console.WriteLine("  3. Cast to known interface");
            Console.WriteLine("  4. Execute safely");
            Console.WriteLine("  5. Unload context when done");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Plugin error (isolated): {ex.Message}");
        }
        finally
        {
            sandbox.Unload();
            Console.WriteLine("\nSandbox unloaded - any plugin memory released");
        }
    }
}

/// <summary>
/// Plugin load context for isolated assembly loading
/// </summary>
public class PluginLoadContext : AssemblyLoadContext
{
    public PluginLoadContext(string name) : base(name, isCollectible: true)
    {
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // Custom loading logic here
        return null; // Use default resolver
    }
}

/// <summary>
/// Sandbox context with additional security considerations
/// </summary>
public class SandboxLoadContext : AssemblyLoadContext
{
    public SandboxLoadContext() : base("Sandbox", isCollectible: true)
    {
    }

    protected override Assembly? Load(AssemblyName assemblyName)
    {
        // Could implement security checks here
        Console.WriteLine($"  Sandbox loading: {assemblyName.Name}");
        return null;
    }

    /// <summary>
    /// Load plugin from specific path with validation
    /// </summary>
    public Assembly LoadPlugin(string path)
    {
        if (!File.Exists(path))
        {
            throw new FileNotFoundException("Plugin not found", path);
        }

        // Validate file (in real scenario, check signature, hash, etc.)
        Console.WriteLine($"  Validating plugin: {Path.GetFileName(path)}");
        
        return LoadFromAssemblyPath(path);
    }
}

/// <summary>
/// Example plugin interface (should be in shared assembly)
/// </summary>
public interface IPlugin
{
    string Name { get; }
    string Version { get; }
    void Execute();
}

/// <summary>
/// Example plugin implementation
/// </summary>
public class SamplePlugin : IPlugin
{
    public string Name => "SamplePlugin";
    public string Version => "1.0.0";

    public void Execute()
    {
        Console.WriteLine("  Plugin executed successfully!");
    }
}
