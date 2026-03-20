# Dockerfile Generator

Generates production-ready Dockerfiles with multi-stage builds for various project types.

## Usage

```bash
dotnet run --project DockerfileGenerator.csproj
```

## Example

```
$ dotnet run --project DockerfileGenerator.csproj

🐳 Dockerfile Generator
======================

Select project type:
  1. ASP.NET Core Web API
  2. .NET Console App
  3. Node.js App
  4. Python App
  5. Go App
  6. Static Site (Nginx)

Choice (1-6): 1

Enter application name (default: myapp): MyApi

Enter port number (default: 8080): 5000

✅ Dockerfile generated successfully!
📁 Saved to: /path/to/Dockerfile
📁 Compose file: /path/to/docker-compose.yml
```

## Generated Files

- **Dockerfile** - Multi-stage build with health checks
- **docker-compose.yml** - Ready-to-use compose configuration

## Supported Project Types

| Type | Base Image | Features |
|------|------------|----------|
| ASP.NET Core | mcr.microsoft.com/dotnet/aspnet:8.0 | Health checks, env vars |
| .NET Console | mcr.microsoft.com/dotnet/runtime:8.0 | Optimized build |
| Node.js | node:20-alpine | Production dependencies |
| Python | python:3.12-slim | Virtual env, pip |
| Go | alpine:latest | Static binary, CGO disabled |
| Nginx | nginx:alpine | Static site hosting |

## Concepts Demonstrated

- String interpolation with raw strings
- Multi-stage Docker builds
- Health check configuration
- File I/O operations
- Template generation
