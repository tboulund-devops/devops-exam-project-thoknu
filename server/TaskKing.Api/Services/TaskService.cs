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
        
        public async Task<IEnumerable<TaskItem>> GetAllTasksSorted(string? sortBy, int page = 1, int pageSize = 50)
        {
            var query = _context.TaskItems
                .Include(t => t.Category)
                .AsQueryable();

            var tasks = await query.ToListAsync();

            var ordered = sortBy?.ToLower() switch
            {
                "priority" => tasks.OrderByDescending(t => PriorityRank(t.Priority)).ThenBy(t => t.Id),
                "created" => tasks.OrderBy(t => t.CreatedAt),
                _ => tasks.OrderBy(t => t.Id)
            };

            return ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
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

            if (string.IsNullOrWhiteSpace(updated.Priority) ||
                !AllowedPriorities.Contains(updated.Priority))
                return null;

            task.Title = updated.Title;
            task.Description = updated.Description;
            task.Status = updated.Status;
            task.Priority = updated.Priority;
            task.DueDate = updated.DueDate;
            task.CategoryId = updated.CategoryId;

            await _context.SaveChangesAsync();

            return await _context.TaskItems
                .Include(t => t.Category)
                .FirstOrDefaultAsync(t => t.Id == id);
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
        
        public async Task<IEnumerable<TaskItem>> GetOverdueTasks()
        {
            var now = DateTime.UtcNow;

            return await _context.TaskItems
                .Where(t =>
                    t.DueDate.HasValue &&
                    t.DueDate.Value < now &&
                    t.Status != TaskItem.StatusValues.Done)
                .Include(t => t.Category)
                .OrderBy(t => t.DueDate)
                .ToListAsync();
        }
        
        public async Task<IEnumerable<TaskItem>> SearchTasks(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return await _context.TaskItems
                    .Include(t => t.Category)
                    .OrderBy(t => t.Id)
                    .ToListAsync();

            query = query.ToLower();

            return await _context.TaskItems
                .Include(t => t.Category)
                .Where(t =>
                    t.Title.ToLower().Contains(query) ||
                    (t.Description != null && t.Description.ToLower().Contains(query)))
                .OrderBy(t => t.Id)
                .ToListAsync();
        }
    }
}