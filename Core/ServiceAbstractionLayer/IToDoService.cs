using System;
using Shared.DataTransferObjects;
using Shared.Enums;

namespace ServiceAbstractionLayer
{
    public interface IToDoService
    {
        Task<IEnumerable<ToDoDto>> GetAllToDosAsync();
        Task<IEnumerable<ToDoDto>> GetFilteredToDosAsync(ToDoStatus? status, ToDoPriority? priority, DateTime? fromDate, DateTime? toDate, ToDoSortBy? sortBy, ToDoSortDirection? sortDirection);
        Task<ToDoDto> GetToDoByIdAsync(Guid id);
        Task<ToDoDto> CreateToDoAsync(CreateToDoDto createToDoDto);
        Task UpdateToDoAsync(Guid id, UpdateToDoDto updateToDoDto);
        Task DeleteToDoAsync(Guid id);
        Task<ToDoDto> MarkAsCompletedAsync(Guid id);
        Task ReopenToDoAsync(Guid id);
        Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync();

    }
}
