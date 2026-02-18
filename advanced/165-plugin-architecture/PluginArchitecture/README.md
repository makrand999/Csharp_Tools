# Plugin Architecture

Plugin architecture demonstration showing assembly loading, dynamic types, and extensibility. Plugins are discovered, loaded, and executed at runtime without recompilation.

## Usage

```bash
dotnet run --project PluginArchitecture.csproj
```

## Example

```
=== Plugin Architecture Demo ===

1. Discovering plugins...
   Created 4 sample plugin source files in: /.../Plugins
   Compiling plugins from source (simulated)...
   Compiled: DataProcessorPlugin.cs
   Compiled: ValidatorPlugin.cs
   Compiled: ReporterPlugin.cs
   Compiled: HighPriorityPlugin.cs
   Found 0 plugins

2. Loaded Plugins:

3. Executing all plugins:

4. Executing 'utility' plugins only:

5. Executing plugins by priority (high first):

6. Executing with custom context:

7. Unloading plugins:
   All plugins unloaded
```

**Note:** This demo creates plugin source files and simulates compilation. In a production scenario, plugins would be pre-compiled DLLs loaded from a plugins directory.

## Concepts Demonstrated

- `AssemblyLoadContext` for isolated plugin loading
- Dynamic type discovery with reflection
- Plugin contract design (IPlugin interface)
- Plugin host pattern for providing services to plugins
- Plugin metadata (Name, Version, Author, Category, Priority)
- Priority-based plugin execution
- Category-based plugin filtering
- Collectible assemblies for unloading
- Plugin lifecycle (Initialize, Execute, Shutdown)
- Extension points for custom functionality
- Runtime assembly loading and unloading
- Garbage collection of unloaded assemblies

## Creating Custom Plugins

To create a custom plugin, implement the `IPlugin` interface:

```csharp
public class MyPlugin : IPlugin
{
    public string Name => "My Plugin";
    public string Version => "1.0.0";
    public string Author => "Your Name";
    public string Description => "Does something useful";
    public string Category => "utility";
    public int Priority => 50;

    public void Initialize(IPluginHost host) { }
    public void Execute(PluginContext context) { }
    public void Shutdown() { }
}
```

Compile to DLL and place in the Plugins directory.
