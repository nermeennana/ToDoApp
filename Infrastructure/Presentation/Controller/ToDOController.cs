using Shared.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.AspNetCore.SignalR;
using Presentation.Hubs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceAbstractionLayer;
using Shared.DataTransferObjects;
using Microsoft.AspNetCore.Authorization;

namespace Presentation.Controller
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    [EnableRateLimiting("ToDoPolicy")]
    public class ToDOController : ControllerBase
    {
        private readonly IToDoService _toDoService;
        private readonly IHubContext<NotificationHub> _hubContext;

        public ToDOController(IToDoService toDoService, IHubContext<NotificationHub> hubContext)
        {
            _toDoService = toDoService;
            _hubContext = hubContext;
        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ToDoDto>>> GetAllToDosController()
        {
            var todos = await _toDoService.GetAllToDosAsync();
            return Ok(todos);
        }
        [HttpGet("filter")]
        [DisableRateLimiting]
        public async Task<ActionResult<IEnumerable<ToDoDto>>> GetAllToDosByStatusController([FromQuery] ToDoStatus? status, [FromQuery] ToDoPriority? priority, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate, [FromQuery] ToDoSortBy? sortBy, [FromQuery] ToDoSortDirection? sortDirection)
        {
            var todos = await _toDoService.GetFilteredToDosAsync(status, priority, fromDate, toDate, sortBy, sortDirection);
            return Ok(todos);
        }

        [HttpGet("{id:Guid}")]
        public async Task<ActionResult<ToDoDto>> GetToDoByIdController(Guid id)
        {
            var todo = await _toDoService.GetToDoByIdAsync(id);
            return Ok(todo);
        }
        [HttpGet("audit-logs")]
        public async Task<ActionResult<IEnumerable<AuditLogDto>>> GetAuditLogsController()
        {
            var auditLogs = await _toDoService.GetAuditLogsAsync();
            return Ok(auditLogs);
        }
        [HttpPost]
        public async Task<ActionResult<ToDoDto>> AddToDoController([FromBody] CreateToDoDto createToDoDto)
        {
            var todo = await _toDoService.CreateToDoAsync(createToDoDto);
            return CreatedAtAction(nameof(GetToDoByIdController), new { id = todo.Id }, todo);
        }
        [HttpPut("{id:guid}")]
        public async Task<IActionResult> UpdateToDoController(Guid id, [FromBody] UpdateToDoDto updateToDoDto)
        {
            await _toDoService.UpdateToDoAsync(id, updateToDoDto);
            return Ok(new { message = "ToDo updated successfully" });
        }

        [HttpDelete("{id:guid}")]
        public async Task<IActionResult> DeleteToDoController(Guid id)
        {
            await _toDoService.DeleteToDoAsync(id);
            return Ok(new { message = "ToDo deleted successfully" });
        }
        [HttpPatch("{id:guid}/complete")]
        public async Task<ActionResult> MarkAsCompletedToDoController(Guid id)
        {
            var todo = await _toDoService.MarkAsCompletedAsync(id);

            await _hubContext.Clients.All.SendAsync("ReceiveNotification", $"{todo.Title} has been completed.");

            return Ok(new { message = "ToDo marked as successfully completed" });
        }
        [HttpPatch("{id:guid}/reopen")]
        public async Task<ActionResult> ReopenToDoController(Guid id)
        {
            await _toDoService.ReopenToDoAsync(id);
            return Ok(new { message = "ToDo reopened successfully" });
        }

    }
}
