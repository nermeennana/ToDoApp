using System;
using Shared.Enums;

namespace Shared.DataTransferObjects
{
    public class ToDoDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; } 
        public ToDoStatus Status { get; set; }
        public ToDoPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
        public DateTime? CompletedAt { get; set; }
    }
}
