# Assembly Resolver

Demonstrates custom assembly resolution and dynamic loading using `AssemblyLoadContext`. Shows how to load assemblies from custom locations, handle dependencies, and create isolated loading contexts.

## Usage

```bash
dotnet run --project AssemblyResolver.csproj
```

## Example

```
=== Assembly Resolver Demo ===

--- Assembly Load Contexts ---

Default context: Default
Is default: True

Custom context created: CustomContext
Loaded System.Text.Json in custom context
  Version: 8.0.0.0
  Location: /usr/share/dotnet/shared/...

Custom context unloaded

--- Custom Assembly Resolver ---

Custom resolver registered
Search paths: /path/to/dir, /path/to/dir/plugins, /path/to/dir/libs

Attempting to resolve assemblies...

Custom resolver unregistered

--- Dynamic Type Loading ---

Current assembly: AssemblyResolver

Types in current assembly:
  - Program
    ✓ Instance created
  - SamplePlugin
    ✓ Instance created
  - CustomLoadContext
    ✓ Instance created

--- Loaded Assemblies ---

Total loaded assemblies: 45

System.Collections                         v8.0.0.0
System.Console                             v8.0.0.0
System.Linq                                v8.0.0.0
System.Private.CoreLib                     v8.0.0.0
System.Runtime                             v8.0.0.0
System.Text.Json                           v8.0.0.0
...
```

## Concepts Demonstrated

- AssemblyLoadContext for isolation
- Custom load contexts (collectible)
- Assembly resolution events
- Dynamic type loading
- Reflection-based type instantiation
- Assembly metadata inspection
- Plugin architecture patterns
- Context unloading
