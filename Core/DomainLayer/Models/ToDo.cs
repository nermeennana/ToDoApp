using DomainLayer.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DomainLayer.Models
{
    public class ToDo
    {
        public Guid Id { get; set; }
        [MaxLength(100)]
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public ToDoStatus Status { get; set; }
        public ToDoPriority Priority { get; set; }
        public DateTime? DueDate { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime LastModifiedDate { get; set; }
    }
}
