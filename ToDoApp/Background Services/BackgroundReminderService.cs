using System;
using Shared.Enums;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using perisistence.Data;
using Presentation.Hubs;

namespace ToDoApp.Background_Services;

public class BackgroundReminderService: BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IHubContext<NotificationHub> _hubContext;
    private readonly HashSet<string> _sentReminders = new();

    public BackgroundReminderService(IServiceScopeFactory scopeFactory, IHubContext<NotificationHub> hubContext)
    {
        _scopeFactory = scopeFactory;
        _hubContext = hubContext;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await SendDueDateRemindersAsync(stoppingToken);
            await Task.Delay(TimeSpan.FromMinutes(1), stoppingToken); // Check every minute
        }
    }

    public async Task SendDueDateRemindersAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ToDoDbContext>();

        var Today = DateTime.UtcNow.Date;
        var tomorrow = Today.AddDays(1);

        var dueSoonTodos = await dbContext.ToDos.AsNoTracking()
            .Where(t => t.Status != ToDoStatus.Completed && t.DueDate.HasValue && t.DueDate.Value.Date >= Today && t.DueDate.Value.Date <= tomorrow)
            .ToListAsync(cancellationToken);

        foreach (var todo in dueSoonTodos)
        {
            var reminderKey = $"{todo.Id}-{todo.DueDate!.Value.Date:yyyy-MM-dd}";

            if (!_sentReminders.Add(reminderKey))
                continue; 

            await _hubContext.Clients.All.SendAsync("ReceiveNotification",
            $"Reminder: '{todo.Title}' is due on {todo.DueDate.Value:dd/MM/yyyy}.",cancellationToken);
        }
    }
}