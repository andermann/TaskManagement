using TaskManagement.Domain.Enums;
using System;

namespace TaskManagement.Domain.Entities
{
    public class TaskItem
    {
        public int Id { get;  set; }
        public int ProjectId { get;  set; }
        public string Title { get;  set; }
        public string Description { get;  set; }
        public DateTime? DueDate { get;  set; }
        public Enums.TaskStatus? Status { get; set; } // 0: Pending, 1: In Progress, 2: Completed
        public TaskPriority? Priority { get; set; } // 0: Low, 1: Medium, 2: High

        public DateTime? CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? CompletedAt { get; set; }

        public int? CreatedByUserId { get; set; }

        public TaskItem() { }

        public TaskItem(int projectId, string title, string description, DateTime? dueDate, TaskPriority priority)
        {
            ProjectId = projectId;
            Title = title;
            Description = description;
            DueDate = dueDate;
            Status = Enums.TaskStatus.Pending; // Status inicial
            Priority = priority;

        }

        //public void UpdateStatus(Enums.TaskStatus newStatus)
        //{
        //    Status = newStatus;
        //}

        public void UpdateStatus(Enums.TaskStatus newStatus)
        {
            Status = newStatus;
            UpdatedAt = DateTime.UtcNow;

            if (newStatus == Enums.TaskStatus.Completed)
                CompletedAt = DateTime.UtcNow;
        }


    }
}