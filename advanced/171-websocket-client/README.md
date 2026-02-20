# WebSocket Client

Real-time bidirectional communication tool using WebSockets. Connects to WebSocket servers for interactive messaging with echo support.

## Usage

```bash
dotnet run --project 171-websocket-client/WebSocketClient.csproj [url]
```

## Example

```bash
# Connect to echo server
dotnet run --project 171-websocket-client/WebSocketClient.csproj wss://echo.websocket.org

# Connect to custom server
dotnet run --project 171-websocket-client/WebSocketClient.csproj ws://localhost:8080/ws
```

```
WebSocket Client
================
Connecting to: wss://echo.websocket.org
Type messages to send (type 'quit' to exit)

Connected! Press Enter to start receiving messages...
[SEND] > Hello WebSocket
[SENT] Message sent

[RECEIVED] Hello WebSocket
[SEND] > quit
```

## Concepts Demonstrated

- WebSocket protocol (ClientWebSocket)
- Async/await for real-time communication
- Bidirectional message handling
- Concurrent send/receive tasks
- Network stream processing
