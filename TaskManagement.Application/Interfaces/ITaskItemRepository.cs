using TaskManagement.Domain.Entities;
using System.Collections.Generic;

namespace TaskManagement.Application.Interfaces
{
    public interface ITaskItemRepository
    {
        Task<TaskItem> GetByIdAsync(int taskId);
        Task<List<TaskItem>> GetByProjectIdAsync(int projectId); // Para visualização de tarefas
        Task AddAsync(TaskItem task);
        Task UpdateAsync(TaskItem task);
        Task RemoveAsync(TaskItem task);

        // Novo método para verificação da regra
        Task<int> CountTasksByProjectIdAsync(int projectId);

        Task<int> CountByProjectAsync(int projectId);
        Task<bool> HasPendingTasksAsync(int projectId);
        //Task<IEnumerable<object>> GetAllAsync();
        Task<List<TaskItem>> GetAllAsync();
    }
}