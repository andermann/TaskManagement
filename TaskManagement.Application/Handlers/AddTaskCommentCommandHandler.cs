using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;


// ----- COMMAND -----
public class AddTaskCommentCommand
{
    public int TaskItemId { get; set; }
    public string Message { get; set; } = string.Empty;
    public int UserId { get; set; }
}

public class AddTaskCommentRequest
{
    public string Message { get; set; } = string.Empty;
}

public class AddTaskCommentCommandHandler
{
    private readonly ITaskCommentRepository _commentRepo;
    private readonly ITaskHistoryRepository _historyRepo;

    

    public AddTaskCommentCommandHandler(ITaskCommentRepository commentRepo, ITaskHistoryRepository historyRepo)
    {
        _commentRepo = commentRepo;
        _historyRepo = historyRepo;
    }

   

    public async Task Handle(AddTaskCommentCommand request)
    {
        var comment = new TaskComment
        {
            TaskItemId = request.TaskItemId,
            Message = request.Message,
            CreatedByUserId = request.UserId,
            CreatedAt = DateTime.UtcNow
        };
        await _commentRepo.AddAsync(comment);

        await _historyRepo.AddAsync(new TaskHistory
        {
            TaskItemId = request.TaskItemId,
            ModifiedByUserId = request.UserId,
            ModifiedAt = DateTime.UtcNow,
            Field = $"Comentário adicionado: {request.Message}"
        });
    }
}
