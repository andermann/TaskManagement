using System;
using TaskManagement.Domain.Enums;

namespace TaskManagement.Application.Queries
{
    // DTO de Saída (View Model)
    public class TaskItemDto
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string Description { get; set; }
        public DateTime? DueDate { get; set; }
        
        //public string Status { get; set; }
        public Domain.Enums.TaskStatus? Status { get; set; }
        
        //public string Priority { get; set; }
        public TaskPriority? Priority { get; set; } // 0: Low, 1: Medium, 2: High
        public int ProjectId { get; set; }
        public int? UserId { get; set; }
        
    }

    // Query de Entrada
    public class GetTaskItemByIdQuery
    {
        public int TaskId { get; set; }
    }
}