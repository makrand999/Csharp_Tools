# HTTP Server

Simple HTTP server built from scratch using HttpListener. Handles HTTP requests with routing, query parameters, and JSON responses.

## Usage

```bash
dotnet run --project 173-http-server/HttpServer.csproj [port]
```

## Example

```bash
# Start on port 8080
dotnet run --project 173-http-server/HttpServer.csproj
```

```
HTTP Server
===========
Listening on http://localhost:8080
Available routes:
  GET  /          - Home page
  GET  /api/time  - Current time
  GET  /api/echo  - Echo query params
  POST /api/echo  - Echo body
Press 'q' to quit

[14:30:45] GET /
[14:30:50] GET /api/time
[14:31:00] POST /api/echo
```

## Available Endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/` | HTML home page |
| GET | `/api/time` | Current server time (JSON) |
| GET | `/api/echo` | Echo query parameters |
| POST | `/api/echo` | Echo request body |

## Concepts Demonstrated

- HttpListener for HTTP server
- Request routing and handling
- Query parameter parsing
- JSON serialization
- Async HTTP processing
- Content-Type handling
