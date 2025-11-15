using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using TaskManagement.Application.Interfaces;
using TaskManagement.Domain.Entities;
using TaskManagement.Infrastructure.Data;

namespace TaskManagement.Infrastructure.Repositories
{
    public class TaskHistoryRepository : ITaskHistoryRepository
    {
        private readonly TaskDbContext _context;

        public TaskHistoryRepository(TaskDbContext context)
        {
            _context = context;
        }

        public async Task AddRangeAsync(IEnumerable<TaskHistory> historyRecords)
        {
            await _context.TaskHistories.AddRangeAsync(historyRecords);
            await _context.SaveChangesAsync();
        }

        public async Task AddAsync(TaskHistory history)
        {
            await _context.TaskHistories.AddAsync(history);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TaskHistory>> GetByTaskIdAsync(int taskId)
        {
            return await _context.TaskHistories
                .AsNoTracking()
                .Where(h => h.TaskItemId == taskId)
                .OrderByDescending(h => h.ModifiedAt)
                .ToListAsync();
        }

    }
}

