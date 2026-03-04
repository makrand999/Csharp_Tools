# AppDomain Isolation

Demonstrates assembly isolation using `AssemblyLoadContext` in .NET Core/.NET 5+. Shows how to load and unload plugins safely, manage memory isolation, and create sandboxed execution environments.

## Usage

```bash
dotnet run --project AppDomainIsolation.csproj
```

## Example

```
=== AppDomain Isolation Demo ===

--- Isolated Plugin Loading ---

Creating isolated load context...
Context created: PluginContext
Is collectible: True

Loading plugin assembly...
Plugin loaded: System.Text.Json
  Version: 8.0.0.0
  Context: PluginContext

Unloading plugin context...
Context unloaded - plugin assemblies can now be GC'd
GC completed

--- Memory Isolation ---

Demonstrating memory isolation...
Memory before: 100,000 bytes
Memory after load: 500,000 bytes
Memory increase: 400,000 bytes
Memory after unload: 150,000 bytes
Memory recovered: 350,000 bytes

--- Version Isolation ---

Version isolation allows loading different versions side-by-side
In .NET Core/.NET 5+, AssemblyLoadContext enables:
  - Loading v1.0.0 in Context A
  - Loading v2.0.0 in Context B
  - Both versions coexist without conflict

--- Safe Plugin Execution ---

Creating sandboxed plugin context...
Sandbox created for untrusted code execution

Sandbox unloaded - any plugin memory released

--- Final State ---

GC Memory: 100,000 bytes
```

## Concepts Demonstrated

- AssemblyLoadContext for isolation
- Collectible assemblies
- Context unloading
- Memory isolation
- Plugin architecture
- Sandboxed execution
- Side-by-side version loading
- Safe plugin patterns
