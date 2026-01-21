var templates = new Dictionary<string, Func<DockerfileOptions, string>>
{
    ["dotnet"] = opts => $$"""
        # Build stage
        FROM mcr.microsoft.com/dotnet/sdk:{{opts.Version}} AS build
        WORKDIR /src
        
        # Copy project files
        COPY *.csproj ./
        RUN dotnet restore
        
        # Copy source code and build
        COPY . ./
        RUN dotnet publish -c Release -o /app/publish
        
        # Runtime stage
        FROM mcr.microsoft.com/dotnet/aspnet:{{opts.Version}} AS runtime
        WORKDIR /app
        COPY --from=build /app/publish .
        
        EXPOSE {{opts.Port}}
        ENTRYPOINT ["dotnet", "{{opts.EntryPoint}}"]
        """,

    ["node"] = opts => $$"""
        # Build stage
        FROM node:{{opts.Version}}-alpine AS build
        WORKDIR /app
        
        # Copy package files
        COPY package*.json ./
        RUN npm ci
        
        # Copy source and build
        COPY . ./
        RUN npm run build
        
        # Production stage
        FROM node:{{opts.Version}}-alpine AS runtime
        WORKDIR /app
        COPY --from=build /app/node_modules ./node_modules
        COPY --from=build /app/package*.json ./
        COPY --from=build /app/dist ./dist
        
        EXPOSE {{opts.Port}}
        CMD ["node", "dist/{{opts.EntryPoint}}"]
        """,

    ["python"] = opts => $$"""
        FROM python:{{opts.Version}}-slim
        WORKDIR /app
        
        # Install dependencies
        COPY requirements.txt .
        RUN pip install --no-cache-dir -r requirements.txt
        
        # Copy source code
        COPY . .
        
        EXPOSE {{opts.Port}}
        CMD ["python", "{{opts.EntryPoint}}"]
        """,

    ["go"] = opts => $$"""
        # Build stage
        FROM golang:{{opts.Version}}-alpine AS build
        WORKDIR /app
        
        # Copy source and build
        COPY . .
        RUN go build -o main {{opts.EntryPoint}}
        
        # Runtime stage
        FROM alpine:latest
        WORKDIR /app
        COPY --from=build /app/main .
        
        EXPOSE {{opts.Port}}
        CMD ["./main"]
        """,

    ["java"] = opts => $$"""
        # Build stage
        FROM maven:3.9-{{opts.Version}} AS build
        WORKDIR /app
        COPY pom.xml .
        RUN mvn dependency:go-offline
        COPY src ./src
        RUN mvn package -DskipTests
        
        # Runtime stage
        FROM {{opts.Version}}-jre-slim
        WORKDIR /app
        COPY --from=build /app/target/*.jar app.jar
        
        EXPOSE {{opts.Port}}
        ENTRYPOINT ["java", "-jar", "app.jar"]
        """,

    ["rust"] = opts => $$"""
        # Build stage
        FROM rust:{{opts.Version}} AS build
        WORKDIR /app
        COPY . .
        RUN cargo build --release
        
        # Runtime stage
        FROM debian:bullseye-slim
        WORKDIR /app
        COPY --from=build /app/target/release/{{opts.EntryPoint}} .
        
        EXPOSE {{opts.Port}}
        CMD ["./{{opts.EntryPoint}}"]
        """,
};

Console.WriteLine("Dockerfile Generator");
Console.WriteLine(new string('-', 60));

Console.WriteLine("\nSelect language/framework:");
var index = 1;
var langList = templates.Keys.ToList();
foreach (var lang in langList)
{
    Console.WriteLine($"  {index}. {lang}");
    index++;
}

Console.Write("\nEnter choice: ");
var input = Console.ReadLine()?.Trim();

if (!int.TryParse(input, out var choice) || choice < 1 || choice > langList.Count)
{
    Console.WriteLine("Invalid choice. Exiting.");
    return;
}

var selectedLang = langList[choice - 1];

Console.Write("Enter version (e.g., 8.0, 20, 3.12): ");
var version = Console.ReadLine()?.Trim() ?? "latest";

Console.Write("Enter port number: ");
var port = Console.ReadLine()?.Trim() ?? "8080";

Console.Write("Enter entry point file (e.g., Program.cs, app.py, main.go): ");
var entryPoint = Console.ReadLine()?.Trim() ?? "main";

var options = new DockerfileOptions
{
    Version = version,
    Port = port,
    EntryPoint = entryPoint
};

var dockerfileContent = templates[selectedLang](options);

Console.WriteLine("\n" + new string('-', 60));
Console.WriteLine("Generated Dockerfile:");
Console.WriteLine(new string('-', 60));
Console.WriteLine(dockerfileContent);
Console.WriteLine(new string('-', 60));

Console.Write("\nSave to Dockerfile? (y/n): ");
var save = Console.ReadLine()?.Trim().ToLower();

if (save == "y")
{
    var outputPath = Path.Combine(Directory.GetCurrentDirectory(), "Dockerfile");
    await File.WriteAllTextAsync(outputPath, dockerfileContent);
    Console.WriteLine($"\n✓ Saved to: {outputPath}");
}

class DockerfileOptions
{
    public string Version { get; set; } = "latest";
    public string Port { get; set; } = "8080";
    public string EntryPoint { get; set; } = "main";
}
