using TaskManagement.Domain.Enums;

public class CreateTaskItemCommand
{
    public int ProjectId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public TaskPriority Priority { get; set; } // 0: Low, 1: Medium, 2: High
    public DateTime? DueDate { get; set; }
    public int UserId { get; set; }
}