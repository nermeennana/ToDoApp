using DomainLayer.Models;
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
        Task<ToDo?> GetToDoByIdAsync(Guid id);
        Task AddToDoAsync(ToDo todo);
        void UpdateToDo(ToDo todo);
        void DeleteToDo(ToDo todo);
        Task SaveChangesAsync();
    }
}
