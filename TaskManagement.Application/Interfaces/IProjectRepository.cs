using TaskManagement.Domain.Entities;

namespace TaskManagement.Application.Interfaces
{
    // Define os métodos de acesso a dados necessários
    public interface IProjectRepository
    {
        Task<Project> GetByIdAsync(int projectId);
        Task<List<Project>> GetProjectsByOwnerIdAsync(int ownerId);
        Task<List<Project>> GetAllAsync();
        Task<int> GetTaskCountAsync(int projectId); // Necessário para a regra de limite de tarefas
        Task AddAsync(Project project);
        Task UpdateAsync(Project project);
        //Task RemoveAsync(Project project);
        Task RemoveAsync(int projectId);

        Task<List<Project>> GetProjectsByIdAsync(int Id);
        //Task<Project?> GetByIdAsync(int id);
        //Task RemoveAsync(Project project);

        // (opcional) atalho por Id
        //Task RemoveByIdAsync(int id);


    }
}