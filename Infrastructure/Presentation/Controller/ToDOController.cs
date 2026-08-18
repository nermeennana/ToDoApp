using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Models;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controller
{
    [ApiController]
    [Route("api/[controller]")]
    public class ToDOController : ControllerBase
    {
        public readonly IToDoRepository _toDoRepository;

        public ToDOController(IToDoRepository toDoRepository)
        {
            _toDoRepository = toDoRepository;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetAllToDosController()
        {
            var todos = await _toDoRepository.GetAllToDosAsync();
            return Ok(todos);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ToDo>> GetToDoByIdController(Guid id)
        {
            var todo = await _toDoRepository.GetToDoByIdAsync(id);
            if (todo == null)
            {
                return NotFound();
            }
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<ToDo>> AddToDoController([FromBody] ToDo todo)
        {
            todo.Id = Guid.NewGuid();
            todo.CreatedDate = DateTime.UtcNow;
            todo.LastModifiedDate = DateTime.UtcNow;

            await _toDoRepository.AddToDoAsync(todo);
            await _toDoRepository.SaveChangesAsync();

            return CreatedAtAction(
                nameof(GetToDoByIdController),
                new { id = todo.Id },
                todo);
        }

        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateToDoController(Guid id, [FromBody] ToDo todo)
        {
            var existingTodo = await _toDoRepository.GetToDoByIdAsync(id);

            if (existingTodo == null)
                return NotFound();

            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.Status = todo.Status;
            existingTodo.Priority = todo.Priority;
            existingTodo.DueDate = todo.DueDate;
            existingTodo.LastModifiedDate = DateTime.UtcNow;

            _toDoRepository.UpdateToDo(existingTodo);
            await _toDoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteToDoController(Guid id)
        {
            var todo = await _toDoRepository.GetToDoByIdAsync(id);

            if (todo == null)
                return NotFound();

            _toDoRepository.DeleteToDo(todo);
            await _toDoRepository.SaveChangesAsync();

            return NoContent();
        }
    }
}
