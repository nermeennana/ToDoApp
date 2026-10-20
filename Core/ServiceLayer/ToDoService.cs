using System;
using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Models;
using ServiceAbstractionLayer;
using Shared.DataTransferObjects;
using Shared.Enums;

namespace ServiceLayer
{
    public class ToDoService : IToDoService
    {
        private readonly IToDoRepository _toDoRepository;
        public ToDoService(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }
        public async Task<IEnumerable<ToDoDto>> GetAllToDosAsync()
        {
            var todos = await _toDoRepository.GetAllToDosAsync();
            return todos.Select(MapToDto);
        }
        public async Task<ToDoDto> GetToDoByIdAsync(Guid id)
        {
            var todo = await _toDoRepository.GetToDoByIdAsync(id);
            return MapToDto(todo);
        }
        public async Task<IEnumerable<ToDoDto>> GetFilteredToDosAsync(ToDoStatus? status, ToDoPriority? priority, DateTime? fromDate, DateTime? toDate, ToDoSortBy? sortBy, ToDoSortDirection? sortDirection)
        {
            var todos = await _toDoRepository.GetAllToDosAsync(status, priority, fromDate, toDate, sortBy, sortDirection);
            return todos.Select(MapToDto);
        }
        public async Task<IEnumerable<AuditLogDto>> GetAuditLogsAsync()
        {
            var auditLogs = await _toDoRepository.GetAuditLogsAsync();
            return auditLogs.Select(MapToAuditLogDto);
        }
        public async Task<ToDoDto> CreateToDoAsync(CreateToDoDto createToDoDto)
        {
            throw new NotImplementedException();
        }
        public async Task UpdateToDoAsync(Guid id, UpdateToDoDto updateToDoDto)
        {
            throw new NotImplementedException();
        }
        public async Task DeleteToDoAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task<ToDoDto> MarkAsCompletedAsync(Guid id)
        {
            throw new NotImplementedException();
        }
        public async Task ReopenToDoAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        private static ToDoDto MapToDto(ToDo todo)
        {
            return new ToDoDto
            {
                Id = todo.Id,
                Title = todo.Title,
                Description = todo.Description,
                Status = todo.Status,
                Priority = todo.Priority,
                DueDate = todo.DueDate,
                CreatedDate = todo.CreatedDate,
                LastModifiedDate = todo.LastModifiedDate
            };
        }
        private static AuditLogDto MapToAuditLogDto(AuditLog log)
        {
            return new AuditLogDto
            {
                ToDoId = log.ToDoId,
                ToDoTitle = log.ToDoTitle,
                Action = log.Action,
                ModifiedAt = log.ModifiedAt,
                UserName = log.UserName
            };
        }

    }
}
