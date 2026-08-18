using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Models;
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
        public async Task<ToDo?> GetToDoByIdAsync(Guid id)
            => await _todoDbContext.ToDos.FindAsync(id);
        public async Task AddToDoAsync(ToDo todo)
            => await _todoDbContext.ToDos.AddAsync(todo);
        public void UpdateToDo(ToDo todo)
            => _todoDbContext.ToDos.Update(todo);
        public void DeleteToDo(ToDo todo)
            => _todoDbContext.ToDos.Remove(todo);
        public Task SaveChangesAsync()
            => _todoDbContext.SaveChangesAsync();
    }
}
