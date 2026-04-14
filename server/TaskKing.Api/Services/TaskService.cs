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
            var tasks = await _context.TaskItems.ToListAsync();

            return sortBy?.ToLower() switch
            {
                "priority" => tasks
                    .OrderByDescending(t => PriorityRank(t.Priority))
                    .ThenBy(t => t.Id),

                "created" => tasks
                    .OrderBy(t => t.CreatedAt),

                _ => tasks
                    .OrderBy(t => t.Id)
            };
        }
        
        public async Task<TaskItem> CreateTask(TaskItem task)
        {
            ArgumentNullException.ThrowIfNull(task);

            if (string.IsNullOrWhiteSpace(task.Title))
                throw new ArgumentException("Title is required", nameof(task));

            if (string.IsNullOrWhiteSpace(task.Status) || !AllowedStatuses.Contains(task.Status))
                task.Status = TaskItem.StatusValues.Todo;
            
            if (string.IsNullOrWhiteSpace(task.Priority))
                task.Priority = TaskItem.PriorityValues.Medium;
            
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

            if (string.IsNullOrWhiteSpace(updated.Priority))
                updated.Priority = TaskItem.PriorityValues.Medium;
            
            task.Title = updated.Title;
            task.Description = updated.Description;
            task.Status = updated.Status;
            task.Priority = updated.Priority;

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