# TCP Server

Multi-client TCP server with concurrent connection handling. Accepts multiple clients and echoes messages back with connection tracking.

## Usage

```bash
dotnet run --project 172-tcp-server/TcpServer.csproj [port]
```

## Example

```bash
# Start server on default port 5000
dotnet run --project 172-tcp-server/TcpServer.csproj

# Start server on custom port
dotnet run --project 172-tcp-server/TcpServer.csproj 6000
```

```
TCP Server
==========
Listening on port 5000
Press 'q' to quit

[INFO] Client connected (1 total)
[CLIENT] Hello Server
[INFO] Client connected (2 total)
[CLIENT] Another message
[INFO] Client disconnected (1 remaining)
```

## Concepts Demonstrated

- TcpListener for server socket
- TcpClient for client connections
- Async network I/O
- Multi-client concurrent handling
- Thread-safe collections
- Network stream processing
