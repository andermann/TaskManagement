using TaskManagement.Domain.Entities;
using System.Collections.Generic;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskHistoryRepository
    {
        Task AddAsync(TaskHistory history);
        Task AddRangeAsync(IEnumerable<TaskHistory> historyRecords);
        Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId);
    }
}

