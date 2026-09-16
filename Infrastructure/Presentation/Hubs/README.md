# Real-Time Notifications (SignalR)

## Overview
Adds real-time push notifications to the ToDo API using SignalR. When a Todo is marked as complete, all connected clients receive an instant notification without polling or refreshing.

## Why SignalR?
Traditional REST APIs are request-response only: the client has to keep asking the server "anything new?". SignalR flips this — the server can push updates to connected clients the moment something happens.

## How it works
1. `NotificationHub` — a minimal SignalR hub acting as the connection point for clients.
2. Registered in `Program.cs` via `AddSignalR()` and mapped to `/notificationHub`.
3. `ToDOController` injects `IHubContext<NotificationHub>` and broadcasts a message via `Clients.All.SendAsync(...)` right after a Todo is marked complete.

## Setup Steps

1. **Create the Hub**
   Add a `NotificationHub` class inheriting from `Hub` in the `Hubs` folder of the project that hosts your Controllers (not a separate layer/project — SignalR needs direct access to it).
```csharp
   public class NotificationHub : Hub
   {
   }
```

2. **Register SignalR in `Program.cs`**
   Before `builder.Build()`:
```csharp
   builder.Services.AddSignalR();
```
   After `builder.Build()`, before `app.Run()`:
```csharp
   app.MapHub<NotificationHub>("/notificationHub");
```

3. **Configure CORS to support SignalR**
   SignalR requests include credentials, so `AllowAnyOrigin()` won't work with `AllowCredentials()`. Specify exact origins instead:
```csharp
   builder.Services.AddCors(options =>
   {
       options.AddPolicy("AllowAll", policy =>
       {
           policy.WithOrigins("http://localhost:5500")
                 .AllowAnyMethod()
                 .AllowAnyHeader()
                 .AllowCredentials();
       });
   });
```
   Make sure `app.UseCors("AllowAll")` is called before `app.MapHub(...)` and `app.MapControllers()`.

4. **Inject `IHubContext` into the Controller**
```csharp
   private readonly IHubContext<NotificationHub> _hubContext;

   public ToDOController(IToDoRepository toDoRepository, IHubContext<NotificationHub> hubContext)
   {
       _toDoRepository = toDoRepository;
       _hubContext = hubContext;
   }
```

5. **Broadcast a notification after the relevant action**
```csharp
   var todo = await _toDoRepository.GetToDoByIdAsync(id);
   await _toDoRepository.MarkAsCompletedToDoAsync(id);
   await _toDoRepository.SaveChangesAsync();

   await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"'{todo.Title}' has been completed.");
```

## Testing it locally
1. Run the API (`dotnet run`).
2. Serve `test-notifications.html` via a local server (e.g. `python3 -m http.server 5500`) — opening it directly as a `file://` won't work due to CORS.
3. Open `http://localhost:5500/test-notifications.html` in the browser.
4. Mark any Todo as complete via Swagger — the notification appears instantly on the page.

## Notes
- CORS is configured to allow credentials from specific origins (required for SignalR's WebSocket handshake) — wildcard origins (`*`) are not compatible with `AllowCredentials()`.