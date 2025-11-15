using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class ProjectRepository : IProjectRepository
    {
        private readonly TaskDbContext _context;

        public ProjectRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Project project)
        {
            await _context.Projects.AddAsync(project);
            await _context.SaveChangesAsync();
        }

        // Implementação da regra de contagem de tarefas
        public async Task<int> GetTaskCountAsync(int projectId)
        {
            return await _context.TaskItems
                                 .CountAsync(t => t.ProjectId == projectId);
        }

        public async Task<Project> GetByIdAsync(int projectId)
        {
            return await _context.Projects.FindAsync(projectId);
        }

        public async Task<List<Project>> GetProjectsByOwnerIdAsync(int ownerId)
        {
            // Listagem de Projetos - listar todos os projetos do usuário
            return await _context.Projects
                                 .Where(p => p.OwnerId == ownerId)
                                 .ToListAsync();
        }

        public async Task<List<Project>> GetProjectsByIdAsync(int Id)
        {
            // Listagem de Projetos - listar todos os projetos do usuário
            return await _context.Projects
                                 .Where(p => p.Id == Id)
                                 .ToListAsync();
        }

        public async Task<List<Project>> GetAllAsync()
        {
            // Listagem de Projetos - listar todos os projetos do usuário
            return await _context.Projects
                                 .ToListAsync();
        }


        public async Task UpdateAsync(Project project)
        {
            _context.Projects.Update(project);
            await _context.SaveChangesAsync();
        }



        //public async Task RemoveAsync(Project project)
        //{
        //    _context.Projects.Remove(project);
        //    await _context.SaveChangesAsync();
        //}

        public async Task RemoveAsync(int projectId)
        {
            var hasPendingTasks = await _context.TaskItems
                //.AnyAsync(t => t.ProjectId == projectId && t.Status != TaskStatus.Completed);
                .AnyAsync(t => t.ProjectId == projectId && t.Status != TaskManagement.Domain.Enums.TaskStatus.Completed);

            if (hasPendingTasks)
                throw new InvalidOperationException("Não é possível remover o projeto enquanto houver tarefas pendentes.");

            var project = await _context.Projects.FindAsync(projectId);
            if (project != null)
            {
                _context.Projects.Remove(project);
                await _context.SaveChangesAsync();
            }
        }

        


    }
}