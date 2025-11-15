using Microsoft.EntityFrameworkCore;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskCommentRepository : ITaskCommentRepository
    {
        private readonly TaskDbContext _context;

        public TaskCommentRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(TaskComment comment)
        {
            await _context.TaskComments.AddAsync(comment);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskComment>> GetByTaskIdAsync(int taskItemId)
        {
            return await _context.TaskComments
                .Where(c => c.TaskItemId == taskItemId)
                .AsNoTracking()
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync();
        }
    }
}
