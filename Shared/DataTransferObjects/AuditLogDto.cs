using System;

namespace Shared.DataTransferObjects;

public class AuditLogDto
{
    public Guid ToDoId { get; set; }
    public string ToDoTitle { get; set; } = null!;
    public string Action { get; set; } = null!;
    public DateTime ModifiedAt { get; set; }
    public string UserName { get; set; } = null!;
}
