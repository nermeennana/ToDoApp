using DomainLayer.Contracts___Repo_Interface;
using DomainLayer.Exceptions;
using DomainLayer.Models;
using DomainLayer.Models.Enums;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        [HttpGet("filter")]
        public async Task<ActionResult<IEnumerable<ToDo>>> GetAllToDosByStatusController([FromQuery] ToDoStatus? status, [FromQuery] ToDoPriority? priority, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] ToDoSortBy? sortBy, [FromQuery] ToDoSortDirection? sortDirection)
        {
            var todos = await _toDoRepository.GetAllToDosAsync(status, priority, fromDate, toDate, sortBy, sortDirection);
            return Ok(todos);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ToDo>> GetToDoByIdController(Guid id)
        {
            var todo = await _toDoRepository.GetToDoByIdAsync(id);
            return Ok(todo);
        }

        [HttpPost]
        public async Task<ActionResult<ToDo>> AddToDoController([FromBody] ToDo todo)
        {
            if (todo.DueDate.HasValue && todo.DueDate.Value.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError("DueDate", "Due date must be today or in the future");
            }
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
            if (todo.DueDate.HasValue && todo.DueDate.Value.Date < DateTime.UtcNow.Date)
            {
                ModelState.AddModelError("DueDate", "Due date must be today or in the future");
                return ValidationProblem(ModelState);
            }
            var existingTodo = await _toDoRepository.GetToDoByIdAsync(id);

            existingTodo.Title = todo.Title;
            existingTodo.Description = todo.Description;
            existingTodo.Status = todo.Status;
            existingTodo.Priority = todo.Priority;
            existingTodo.DueDate = todo.DueDate; 
            existingTodo.LastModifiedDate = DateTime.UtcNow;

            await _toDoRepository.UpdateToDoAsync(existingTodo);
            await _toDoRepository.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteToDoController(Guid id)
        {
            var todo = await _toDoRepository.GetToDoByIdAsync(id);
            await _toDoRepository.DeleteToDoAsync(todo);
            await _toDoRepository.SaveChangesAsync();

            return NoContent();
        }
        [HttpPatch("{id:guid}/complete")]
        public async Task<ActionResult> MarkAsCompletedToDoController(Guid id)
        {
            await _toDoRepository.MarkAsCompletedToDoAsync(id);
            await _toDoRepository.SaveChangesAsync();
            return Ok(new { message = "ToDo marked as complete successfully" });
        }
        [HttpPatch("{id:guid}/reopen")]
        public async Task<ActionResult> ReopenToDoController(Guid id)
        {
            await _toDoRepository.ReopenToDoAsync(id);
            await _toDoRepository.SaveChangesAsync();
            return Ok(new { message = "ToDo reopened successfully" });
        }

    }
}
