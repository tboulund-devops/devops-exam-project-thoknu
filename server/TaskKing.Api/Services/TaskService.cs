using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Data;
using TaskKing.Api.Models;

namespace TaskKing.Api.Services
{
    public class TaskService
    {
        private readonly TaskKingDbContext _context;

        public TaskService(TaskKingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasks()
        {
            return await _context.Tasks
                .OrderBy(t => t.Id)
                .ToListAsync();
        }

        public async Task<TaskItem> CreateTask(TaskItem task)
        {
            _context.Tasks.Add(task);
            await _context.SaveChangesAsync();
            return task;
        }
    }
}