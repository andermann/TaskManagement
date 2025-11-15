using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskCommentRepository
    {
        Task AddAsync(TaskComment comment);
        Task<IEnumerable<TaskComment>> GetByTaskIdAsync(int taskItemId);
    }
}
