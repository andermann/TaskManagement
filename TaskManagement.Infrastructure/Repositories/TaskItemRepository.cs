using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskItemRepository : ITaskItemRepository
    {
        private readonly TaskDbContext _context;

        public TaskItemRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskItem task)
        {
            await _context.TaskItems.AddAsync(task);
            await _context.SaveChangesAsync();
        }

        //public async Task<TaskItem> GetByIdAsync(int taskId)
        //{
        //    // Assume que TaskItems é um DbSet no TaskDbContext
        //    return await _context.TaskItems.FindAsync(taskId);
        //}

        public async Task<TaskItem?> GetByIdAsync(int id)
        {
            return await _context.TaskItems.FirstOrDefaultAsync(t => t.Id == id);
        }

        public async Task<List<TaskItem>> GetAllAsync()
        {
            return await _context.TaskItems
                .AsNoTracking()
                .OrderBy(t => t.Title)
                .ToListAsync();
        }

        public async Task UpdateAsync(TaskItem task)
        {
            _context.TaskItems.Update(task);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveAsync(TaskItem task)
        {
            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();
        }

        //public Task<List<TaskItem>> GetByProjectIdAsync(int projectId)
        //{
        //    return _context.TaskItems.Where(t => t.ProjectId == projectId).ToListAsync();
        //}

        public async Task<List<TaskItem>> GetByProjectIdAsync(int projectId)
        {
            return await _context.TaskItems
                .AsNoTracking()
                .Where(t => t.ProjectId == projectId)
                .OrderByDescending(t => t.Id)
                .ToListAsync();
        }

        // Método de apoio para a Regra de Negócio de Limite de Tarefas
        public async Task<int> CountTasksByProjectIdAsync(int projectId)
        {
            return await _context.TaskItems
                                 .CountAsync(t => t.ProjectId == projectId);
        }

        public async Task<int> CountByProjectAsync(int projectId)
        {
            return await _context.TaskItems.CountAsync(t => t.ProjectId == projectId);
        }

        
        public async Task<bool> HasPendingTasksAsync(int projectId)
        {
            return await _context.TaskItems
                .AnyAsync(t => t.ProjectId == projectId && t.Status != Domain.Enums.TaskStatus.Completed);
        }

    }
}