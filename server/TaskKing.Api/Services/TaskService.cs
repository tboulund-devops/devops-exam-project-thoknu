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
            TaskItem.StatusValues.Todo,
            TaskItem.StatusValues.InProgress,
            TaskItem.StatusValues.Done
        };
        
        private static readonly string[] AllowedPriorities =
        {
            TaskItem.PriorityValues.Low,
            TaskItem.PriorityValues.Medium,
            TaskItem.PriorityValues.High
        };
        
        private static int PriorityRank(string priority)
        {
            return priority switch
            {
                TaskItem.PriorityValues.High => 3,
                TaskItem.PriorityValues.Medium => 2,
                TaskItem.PriorityValues.Low => 1,
                _ => 2
            };
        }

        public TaskService(TaskKingDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<TaskItem>> GetAllTasks()
            => await _context.TaskItems.OrderBy(t => t.Id).ToListAsync();
        
        public async Task<IEnumerable<TaskItem>> GetAllTasksSorted(string? sortBy)
        {
            var query = _context.TaskItems
                .Include(t => t.Category)
                .AsQueryable();

            var tasks = await query.ToListAsync();

            return sortBy?.ToLower() switch
            {
                "priority" => tasks.OrderByDescending(t => PriorityRank(t.Priority)).ThenBy(t => t.Id),
                "created" => tasks.OrderBy(t => t.CreatedAt),
                _ => tasks.OrderBy(t => t.Id)
            };
        }
        
        public async Task<TaskItem> CreateTask(TaskItem task)
        {
            ArgumentNullException.ThrowIfNull(task);

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Title is required", nameof(task));

            if (string.IsNullOrWhiteSpace(task.Status) || !AllowedStatuses.Contains(task.Status))
                task.Status = TaskItem.StatusValues.Todo;
            
            if (string.IsNullOrWhiteSpace(task.Priority) || !AllowedPriorities.Contains(task.Priority))
                task.Priority = TaskItem.PriorityValues.Medium;
            
            if (task.CategoryId.HasValue)
            {
                var exists = await _context.Categories.AnyAsync(c => c.Id == task.CategoryId.Value);
                if (!exists)
                    task.CategoryId = null;
            }
            
            _context.TaskItems.Add(task);
            await _context.SaveChangesAsync();

            return task;
        }

        public async Task<TaskItem?> GetTaskById(int id)
            => await _context.TaskItems.FindAsync(id);

        public async Task<IEnumerable<TaskItem>> GetAllTasksByStatus(string status)
            => await _context.TaskItems
                .Where(t => t.Status == status)
                .OrderBy(t => t.Id)
                .ToListAsync();

        public async Task<TaskItem?> UpdateTask(int id, TaskItem updated)
        {
            ArgumentNullException.ThrowIfNull(updated);

            var task = await _context.TaskItems.FindAsync(id);

            if (task == null)
                return null;

            if (string.IsNullOrWhiteSpace(updated.Title))
                return null;

            if (!AllowedStatuses.Contains(updated.Status))
                return null;

            if (string.IsNullOrWhiteSpace(updated.Priority) || !AllowedPriorities.Contains(updated.Priority))
                return null;
            
            if (updated.CategoryId.HasValue)
            {
                var exists = await _context.Categories.AnyAsync(c => c.Id == updated.CategoryId.Value);
                if (!exists)
                    return null;
            }

            task.Title = updated.Title;
            task.Description = updated.Description;
            task.Status = updated.Status;
            task.Priority = updated.Priority;
            task.CategoryId = updated.CategoryId;

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