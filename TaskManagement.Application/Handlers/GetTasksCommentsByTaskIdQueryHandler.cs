using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Handlers
{
    // Query
    public class GetTasksCommentsByTaskIdQuery
    {
        public int TaskItemId { get; set; }
    }

    // Handler
    public class GetTasksCommentsByTaskIdQueryHandler
    {
        private readonly ITaskCommentRepository _commentRepo;

        public GetTasksCommentsByTaskIdQueryHandler(ITaskCommentRepository commentRepo)
        {
            _commentRepo = commentRepo;
        }

        public async Task<IEnumerable<TaskComment>> Handle(GetTasksCommentsByTaskIdQuery request)
        {
            return await _commentRepo.GetByTaskIdAsync(request.TaskItemId);
        }
    }
}
