# Dockerfile Generator

An interactive CLI tool that generates production-ready Dockerfiles with multi-stage builds for various programming languages.

## Usage

```bash
dotnet run --project DockerfileGenerator.csproj
```

## Supported Languages

- **dotnet** - .NET 8.0 / ASP.NET Core (multi-stage build)
- **node** - Node.js with npm (build + production stages)
- **python** - Python with pip requirements
- **go** - Go with multi-stage build
- **java** - Java with Maven build
- **rust** - Rust with Cargo build

## Features

- Multi-stage builds for smaller production images
- Configurable version selection
- Custom port configuration
- Custom entry point specification
- Optimized layer caching

## Example Session

```
Dockerfile Generator
------------------------------------------------------------

Select language/framework:
  1. dotnet
  2. node
  3. python
  4. go
  5. java
  6. rust

Enter choice: 1
Enter version (e.g., 8.0, 20, 3.12): 8.0
Enter port number: 5000
Enter entry point file (e.g., Program.cs, app.py, main.go): MyApp.dll

------------------------------------------------------------
Generated Dockerfile:
------------------------------------------------------------
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY *.csproj ./
RUN dotnet restore

# Copy source code and build
COPY . ./
RUN dotnet publish -c Release -o /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

EXPOSE 5000
ENTRYPOINT ["dotnet", "MyApp.dll"]
------------------------------------------------------------

Save to Dockerfile? (y/n): y

✓ Saved to: /path/to/project/Dockerfile
```

## Concepts Demonstrated

- Dictionary with function values
- String interpolation with raw strings
- Multi-stage Docker builds
- Interactive CLI input
- File generation and persistence
- Delegate types with Func<T>
