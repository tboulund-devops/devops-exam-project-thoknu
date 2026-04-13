using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Data;
using TaskKing.Api.Models;

namespace TaskKing.Api.Services
{
    public class TaskService
    {
        private readonly TaskKingDbContext _context;
        
        private static readonly string[] AllowedStatuses =
        {
            "Todo",
            "InProgress",
            "Done"
        };

        public TaskService(TaskKingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasks()
        {
            return await _context.TaskItems
                .OrderBy(t => t.Id)
                .ToListAsync();
        }

        public async Task<TaskItem> CreateTask(TaskItem task)
        {
            if (task == null)
                throw new ArgumentException("Task cannot be null");

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Title is required");
            
            if (!AllowedStatuses.Contains(task.Status))
            {
                task.Status = "Todo";
            }
            
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return task;
        }
        
        public async Task<TaskItem?> GetTaskById(int id)
        {
            return await _context.TaskItems.FindAsync(id);
        }
        
        public async Task<IEnumerable<TaskItem>> GetAllTasksByStatus(string status)
        {
            return await _context.TaskItems
                .Where(t => t.Status == status)
                .OrderBy(t => t.Id)
                .ToListAsync();
        }
        
        public async Task<TaskItem?> UpdateTask(int id, TaskItem updated)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return null;

            if (!AllowedStatuses.Contains(updated.Status))
            {
                return null;
            }
            
            task.Title = updated.Title;
            task.Description = updated.Description;
            task.Status = updated.Status;

            await _context.SaveChangesAsync();

            return task;
        }
        
        public async Task<bool> DeleteTask(int id)
        {
            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return false;

            _context.TaskItems.Remove(task);
            await _context.SaveChangesAsync();

            return true;
        }
    }
}