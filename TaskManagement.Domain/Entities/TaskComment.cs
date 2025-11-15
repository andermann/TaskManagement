using System;

namespace TaskManagement.Domain.Entities
{
    public class TaskComment
    {
        public int Id { get; set; }
        public int TaskItemId { get; set; }
        public string? Message { get; set; }
        public DateTime CreatedAt { get; set; }
        public int CreatedByUserId { get; set; }

        //public TaskItem? TaskItem { get; set; }

    }
}