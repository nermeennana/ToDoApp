using System;
using System.ComponentModel.DataAnnotations;

namespace DomainLayer.Models;

public class AuditLog
{
    public Guid Id { get; set; }
    public Guid ToDoId { get; set; }
    [MaxLength(100, ErrorMessage = "ToDo title must not exceed 100 characters")]
    public string ToDoTitle { get; set; } = null!;
    [MaxLength(100, ErrorMessage = "Action must not exceed 100 characters")]
    public string Action { get; set; } = null!;
    public DateTime ModifiedAt { get; set; }
    [MaxLength(100, ErrorMessage = "User name must not exceed 100 characters")]
    public string UserName { get; set; } = null!;
}
