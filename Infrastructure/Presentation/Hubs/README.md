# Real-Time Notifications (SignalR)

## Overview
Adds real-time push notifications to the ToDo API using SignalR. When a Todo is marked as complete, all connected clients receive an instant notification without polling or refreshing.

## Why SignalR?
Traditional REST APIs are request-response only: the client has to keep asking the server "anything new?". SignalR flips this — the server can push updates to connected clients the moment something happens.

## How it works
1. `NotificationHub` — a minimal SignalR hub acting as the connection point for clients.
2. Registered in `Program.cs` via `AddSignalR()` and mapped to `/notificationHub`.
3. `ToDOController` injects `IHubContext<NotificationHub>` and broadcasts a message via `Clients.All.SendAsync(...)` right after a Todo is marked complete.

## Testing it locally
1. Run the API (`dotnet run`).
2. Serve `test-notifications.html` via a local server (e.g. `python3 -m http.server 5500`) — opening it directly as a `file://` won't work due to CORS.
3. Open `http://localhost:5500/test-notifications.html` in the browser.
4. Mark any Todo as complete via Swagger — the notification appears instantly on the page.

## Notes
- CORS is configured to allow credentials from specific origins (required for SignalR's WebSocket handshake) — wildcard origins (`*`) are not compatible with `AllowCredentials()`.