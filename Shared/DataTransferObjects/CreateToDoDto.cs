using System;
using System.ComponentModel.DataAnnotations;
using Shared.Enums;

namespace Shared.DataTransferObjects
{
    public class CreateToDoDto
    {
        [Required(ErrorMessage = "Title is required")]
        [MaxLength(100, ErrorMessage = "Title must not exceed 100 characters")]
        public string Title { get; set; } = null!;
        [MaxLength(500, ErrorMessage = "Description must not exceed 500 characters")]
        public string? Description { get; set; }
        [DataType(DataType.Date)]
        public DateTime? DueDate { get; set; }
        public ToDoPriority? Priority { get; set; }
    }
}
