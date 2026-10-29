using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Exceptions;
using DomainLayer.Models;
using Shared.Enums;
using Microsoft.EntityFrameworkCore;
using perisistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using ServiceAbstractionLayer;

namespace perisistence.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly ToDoDbContext _todoDbContext;
        private const string CacheKey = "ToDosKey";
        private readonly IMemoryCache _memoryCache;
        private readonly ICurrentUserService _currentUserService;

        public ToDoRepository(ToDoDbContext todoDbContext, IMemoryCache memoryCache, ICurrentUserService currentUserService)
        {
            _todoDbContext = todoDbContext;
            _memoryCache = memoryCache;
            _currentUserService = currentUserService;
        }
        public async Task<IEnumerable<AuditLog>> GetAuditLogsAsync()
            => await _todoDbContext.AuditLogs.AsNoTracking().OrderByDescending(a => a.ModifiedAt).ToListAsync();
        public async Task<IEnumerable<ToDo>> GetAllToDosAsync()
        {
            if(_memoryCache.TryGetValue(CacheKey,out List<ToDo>? cachedTodos))
            {
                // Console.WriteLine("Cache hit: data came from cache");
                return cachedTodos!;
            }
            
            // Console.WriteLine("Cache miss: reading from database");
            var todos = await _todoDbContext.ToDos.AsNoTracking().ToListAsync();

            _memoryCache.Set(CacheKey, todos, TimeSpan.FromMinutes(5));

            return todos;
        }

        public async Task<IEnumerable<ToDo>> GetAllToDosAsync(ToDoStatus? status, ToDoPriority? priority, DateTime? fromDate, DateTime? toDate, ToDoSortBy? sortBy, ToDoSortDirection? sortDirection)
        {
            var query = await GetAllToDosAsync();
            // var query = _todoDbContext.ToDos.AsQueryable();

            if (status != null)
                query = query.Where(t => t.Status == status);

            if (priority != null)
                query = query.Where(t => t.Priority == priority);

            if (fromDate != null)
                query = query.Where(t => t.DueDate >= fromDate);

            if (toDate != null)
                query = query.Where(t => t.DueDate <= toDate);

            bool isDescending = sortDirection == ToDoSortDirection.Desc;

            query = sortBy switch
            {
                ToDoSortBy.Title => isDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                ToDoSortBy.Priority => isDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                ToDoSortBy.DueDate => isDescending ? query.OrderByDescending(t => t.DueDate) : query.OrderBy(t => t.DueDate),
                ToDoSortBy.CreatedDate => isDescending ? query.OrderByDescending(t => t.CreatedDate) : query.OrderBy(t => t.CreatedDate),
                _ => query
            };


            return query.ToList();
        }
        public async Task<ToDo> GetToDoByIdAsync(Guid id)
        {
            var todo = await _todoDbContext.ToDos.FindAsync(id);
            if (todo == null)
            {
                throw new ToDoNotFoundException(id);
            }
            return todo;
        }
        public async Task AddToDoAsync(ToDo todo)
        {
            await _todoDbContext.ToDos.AddAsync(todo);
            LogAudit(todo, "Created");
        }
        public async Task UpdateToDoAsync(ToDo todo)
        {
            _todoDbContext.ToDos.Update(todo);
            LogAudit(todo, "Updated");
        }
        public async Task DeleteToDoAsync(ToDo todo)
        {
            _todoDbContext.ToDos.Remove(todo);
            LogAudit(todo, "Deleted");
        }
        public async Task MarkAsCompletedToDoAsync(Guid id)
        {
            var todo = await _todoDbContext.ToDos.FindAsync(id);
            if (todo == null)
            { 
                throw new ToDoNotFoundException(id); 
            }

            todo.Status = ToDoStatus.Completed;
            todo.CompletedAt = DateTime.UtcNow;
            todo.LastModifiedDate = DateTime.UtcNow;
            LogAudit(todo, "Completed");
        }

        public async Task ReopenToDoAsync(Guid id)
        {
            var todo = await _todoDbContext.ToDos.FindAsync(id);
            if (todo == null)
            {
                throw new ToDoNotFoundException(id);
            }

            // Set to enum default (value 0). Replace with a specific member like ToDoStatus.Pending if you prefer.
            todo.Status = default;
            todo.CompletedAt = null;
            todo.LastModifiedDate = DateTime.UtcNow;

            LogAudit(todo, "Reopened");
        }
        private void LogAudit(ToDo todo, string action)
        {
            _todoDbContext.AuditLogs.Add(new AuditLog
            {
                Id = Guid.NewGuid(),
                ToDoId = todo.Id,
                ToDoTitle = todo.Title,
                Action = action,
                ModifiedAt = DateTime.UtcNow,
                UserName = _currentUserService.UserName
            });
        }
        public async Task SaveChangesAsync()
        {
            var changes = await _todoDbContext.SaveChangesAsync();
            
            if (changes > 0)
            {
                _memoryCache.Remove(CacheKey);
            }
        }

    }
}
