namespace DockerfileGenerator;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("🐳 Dockerfile Generator");
        Console.WriteLine("======================\n");

        Console.WriteLine("Select project type:");
        Console.WriteLine("  1. ASP.NET Core Web API");
        Console.WriteLine("  2. .NET Console App");
        Console.WriteLine("  3. Node.js App");
        Console.WriteLine("  4. Python App");
        Console.WriteLine("  5. Go App");
        Console.WriteLine("  6. Static Site (Nginx)");
        Console.Write("\nChoice (1-6): ");

        string? choice = Console.ReadLine();

        Console.Write("\nEnter application name (default: myapp): ");
        string appName = Console.ReadLine()?.Trim() ?? "myapp";

        Console.Write("\nEnter port number (default: 8080): ");
        string portInput = Console.ReadLine()?.Trim() ?? "8080";
        if (!int.TryParse(portInput, out int port)) port = 8080;

        string dockerfile = choice switch
        {
            "1" => GenerateAspNetCore(appName, port),
            "2" => GenerateDotNetConsole(appName),
            "3" => GenerateNodeJs(appName, port),
            "4" => GeneratePython(appName, port),
            "5" => GenerateGo(appName, port),
            "6" => GenerateNginx(appName, port),
            _ => GenerateAspNetCore(appName, port)
        };

        string outputFile = "Dockerfile";
        File.WriteAllText(outputFile, dockerfile);

        Console.WriteLine($"\n✅ Dockerfile generated successfully!");
        Console.WriteLine($"📁 Saved to: {Path.GetFullPath(outputFile)}");

        // Generate docker-compose.yml
        string composeFile = "docker-compose.yml";
        string compose = GenerateCompose(appName, port);
        File.WriteAllText(composeFile, compose);
        Console.WriteLine($"📁 Compose file: {Path.GetFullPath(composeFile)}");

        Console.WriteLine("\n--- Dockerfile Preview ---\n");
        Console.WriteLine(dockerfile);
    }

    private static string GenerateAspNetCore(string app, int port)
    {
        return $$"""
            # Build stage
            FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
            WORKDIR /src
            
            # Copy csproj and restore dependencies
            COPY {{app}}.csproj ./
            RUN dotnet restore
            
            # Copy everything else and build
            COPY . ./
            RUN dotnet build -c Release -o /app/build
            
            # Publish stage
            FROM build AS publish
            RUN dotnet publish -c Release -o /app/publish /p:UseAppHost=false
            
            # Runtime stage
            FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS final
            WORKDIR /app
            EXPOSE {{port}}
            ENV ASPNETCORE_URLS=http://+:{{port}}
            ENV ASPNETCORE_ENVIRONMENT=Production
            
            COPY --from=publish /app/publish .
            ENTRYPOINT ["dotnet", "{{app}}.dll"]
            
            # Health check
            HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
              CMD curl -f http://localhost:{{port}}/health || exit 1
            """;
    }

    private static string GenerateDotNetConsole(string app)
    {
        return $$"""
            # Build stage
            FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
            WORKDIR /src
            
            COPY {{app}}.csproj ./
            RUN dotnet restore
            
            COPY . ./
            RUN dotnet publish -c Release -o /app/publish
            
            # Runtime stage
            FROM mcr.microsoft.com/dotnet/runtime:8.0 AS final
            WORKDIR /app
            
            COPY --from=publish /app/publish .
            ENTRYPOINT ["dotnet", "{{app}}.dll"]
            """;
    }

    private static string GenerateNodeJs(string app, int port)
    {
        return $$"""
            # Build stage
            FROM node:20-alpine AS build
            WORKDIR /app
            
            COPY package*.json ./
            RUN npm ci --only=production
            
            COPY . .
            
            # Production stage
            FROM node:20-alpine AS final
            WORKDIR /app
            
            ENV NODE_ENV=production
            ENV PORT={{port}}
            
            COPY --from=build /app .
            
            EXPOSE {{port}}
            CMD ["node", "index.js"]
            
            # Health check
            HEALTHCHECK --interval=30s --timeout=3s CMD wget -q --spider http://localhost:{{port}}/health || exit 1
            """;
    }

    private static string GeneratePython(string app, int port)
    {
        return $$"""
            # Build stage
            FROM python:3.12-slim AS build
            WORKDIR /app
            
            # Install dependencies
            COPY requirements.txt .
            RUN pip install --no-cache-dir -r requirements.txt
            
            COPY . .
            
            # Production stage
            FROM python:3.12-slim AS final
            WORKDIR /app
            
            ENV PYTHONUNBUFFERED=1
            ENV PORT={{port}}
            
            COPY --from=build /usr/local/lib/python3.12/site-packages /usr/local/lib/python3.12/site-packages
            COPY --from=build /app .
            
            EXPOSE {{port}}
            CMD ["python", "main.py"]
            
            # Health check
            HEALTHCHECK --interval=30s --timeout=3s CMD python -c "import urllib.request; urllib.request.urlopen('http://localhost:{{port}}/health')" || exit 1
            """;
    }

    private static string GenerateGo(string app, int port)
    {
        return $$"""
            # Build stage
            FROM golang:1.21-alpine AS build
            WORKDIR /app
            
            RUN apk add --no-cache git
            COPY go.mod go.sum ./
            RUN go mod download
            
            COPY . .
            RUN CGO_ENABLED=0 GOOS=linux go build -a -installsuffix cgo -o main .
            
            # Production stage
            FROM alpine:latest AS final
            WORKDIR /app
            
            RUN apk --no-cache add ca-certificates
            
            COPY --from=build /app/main .
            
            ENV PORT={{port}}
            EXPOSE {{port}}
            CMD ["./main"]
            
            # Health check
            HEALTHCHECK --interval=30s --timeout=3s CMD wget -q --spider http://localhost:{{port}}/health || exit 1
            """;
    }

    private static string GenerateNginx(string app, int port)
    {
        return $$"""
            FROM nginx:alpine
            
            # Remove default config
            RUN rm /etc/nginx/conf.d/default.conf
            
            # Copy custom config
            COPY nginx.conf /etc/nginx/conf.d/default.conf
            
            # Copy static files
            COPY ./dist /usr/share/nginx/html
            
            EXPOSE {{port}}
            
            # Health check
            HEALTHCHECK --interval=30s --timeout=3s CMD wget -q --spider http://localhost:{{port}}/ || exit 1
            
            CMD ["nginx", "-g", "daemon off;"]
            """;
    }

    private static string GenerateCompose(string app, int port)
    {
        return $$"""
            version: '3.8'
            
            services:
              {{app}}:
                build:
                  context: .
                  dockerfile: Dockerfile
                ports:
                  - "{{port}}:{{port}}"
                environment:
                  - ASPNETCORE_ENVIRONMENT=Production
                restart: unless-stopped
                healthcheck:
                  interval: 30s
                  timeout: 3s
                  retries: 3
                  start_period: 10s
                networks:
                  - app-network
            
            networks:
              app-network:
                driver: bridge
            """;
    }
}
