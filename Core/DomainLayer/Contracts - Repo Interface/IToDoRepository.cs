using DomainLayer.Models;
using DomainLayer.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Contracts___Repo_Interface
{
    public interface IToDoRepository
    {
        Task <IEnumerable<ToDo>> GetAllToDosAsync();
        Task<IEnumerable<ToDo>> GetAllToDosAsync(ToDoStatus? status, ToDoPriority? priority, DateTime? fromDate,DateTime? toDate, ToDoSortBy? sortBy, ToDoSortDirection? sortDirection);
        Task<ToDo> GetToDoByIdAsync(Guid id);
        Task AddToDoAsync(ToDo todo);
        Task UpdateToDoAsync(ToDo todo);
        Task DeleteToDoAsync(ToDo todo);
        Task MarkAsCompletedToDoAsync(Guid id);
        Task ReopenToDoAsync(Guid id);
        Task SaveChangesAsync();
    }
}
