using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Exceptions;
using DomainLayer.Models;
using DomainLayer.Models.Enums;
using Microsoft.EntityFrameworkCore;
using perisistence.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace perisistence.Repositories
{
    public class ToDoRepository : IToDoRepository
    {
        private readonly ToDoDbContext _todoDbContext;
        public ToDoRepository(ToDoDbContext todoDbContext) 
        {
            _todoDbContext = todoDbContext;
        }

        public async Task<IEnumerable<ToDo>> GetAllToDosAsync()
            => await _todoDbContext.ToDos.ToListAsync();
        public async Task<IEnumerable<ToDo>> GetAllToDosAsync(ToDoStatus? status, ToDoPriority? priority, DateTime? fromDate, DateTime? toDate, ToDoSortBy? sortBy, ToDoSortDirection? sortDirection)
        {
            var query = _todoDbContext.ToDos.AsQueryable();

            if (status != null)
                query = query.Where(t => t.Status == status);

            if (priority != null)
                query = query.Where(t => t.Priority == priority);

            if (fromDate != null)
                query = query.Where(t => t.CreatedDate >= fromDate);

            if (toDate != null)
                query = query.Where(t => t.CreatedDate <= toDate);

            bool isDescending = sortDirection == ToDoSortDirection.Desc;

            query = sortBy switch
            {
                ToDoSortBy.Title => isDescending ? query.OrderByDescending(t => t.Title) : query.OrderBy(t => t.Title),
                ToDoSortBy.Priority => isDescending ? query.OrderByDescending(t => t.Priority) : query.OrderBy(t => t.Priority),
                ToDoSortBy.CreatedDate => isDescending ? query.OrderByDescending(t => t.CreatedDate) : query.OrderBy(t => t.CreatedDate),
                _ => query
            };


            return await query.ToListAsync();
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
            => await _todoDbContext.ToDos.AddAsync(todo);
        public async Task UpdateToDoAsync(ToDo todo)
            => _todoDbContext.ToDos.Update(todo);
        public async Task DeleteToDoAsync(ToDo todo)
            => _todoDbContext.ToDos.Remove(todo);
        public async Task MarkAsCompletedToDoAsync(Guid id)
        {
            var todo = await _todoDbContext.ToDos.FindAsync(id);
            if (todo == null)
            { 
                throw new ToDoNotFoundException(id); 
            }

            todo.Status = ToDoStatus.Completed;
            todo.LastModifiedDate = DateTime.UtcNow;
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
            todo.LastModifiedDate = DateTime.UtcNow;
        }
        public async Task SaveChangesAsync()
            => await _todoDbContext.SaveChangesAsync();

    }
}
