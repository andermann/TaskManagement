using System;

namespace TaskManagement.Domain.Entities
{
    public class TaskHistory
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; } 
        public string? Field { get; set; }
        public int ModifiedByUserId { get; set; }

        public DateTime ModifiedAt { get; set; }
        public DateTime ChangedAt { get; set; }
        public int ChangedByUserId { get; set; }

        public TaskItem? TaskItem { get; set; }
    }
}