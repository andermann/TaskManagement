using TaskManagement.Domain.Enums;

public class UpdateTaskCommand
{
    public int TaskId { get; set; }
    public int UserId { get; set; } // Quem fez a alteração
    public string? Title { get; set; } // Opcional
    public string Description { get; set; } // Opcional
    public DateTime? DueDate { get; set; } // Opcional
    public TaskManagement.Domain.Enums.TaskStatus? Status { get; set; } // Opcional
    public string Comment { get; set; } // Opcional (Comentário)

    public TaskPriority Priority { get; set; }
}
