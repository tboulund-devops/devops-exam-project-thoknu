using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Data;
using TaskKing.Api.Models;
using TaskKing.Api.Services;

namespace TaskKing.Tests.Services
{
    public class TaskServiceTests
    {
        private static TaskKingDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskKingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TaskKingDbContext(options);
        }

        // ---------------- CREATE ----------------

        [Fact]
        public async Task CreateTask_ShouldAddTask_WhenValid()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test task",
                Priority = TaskItem.PriorityValues.Medium
            };

            var result = await service.CreateTask(task);

            Assert.Equal("Test task", result.Title);
            Assert.Equal(TaskItem.PriorityValues.Medium, result.Priority);
            Assert.Equal(1, context.TaskItems.Count());
        }

        [Fact]
        public async Task CreateTask_ShouldSetDefaultValues()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = "Test" };

            var result = await service.CreateTask(task);

            Assert.Equal("Todo", result.Status);
            Assert.Equal(TaskItem.PriorityValues.Medium, result.Priority);
            Assert.True(result.CreatedAt <= DateTime.UtcNow);
        }

        [Fact]
        public async Task CreateTask_ShouldThrow_WhenTitleIsEmpty()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = "" };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTask(task));
        }

        [Fact]
        public async Task CreateTask_ShouldThrow_WhenTitleIsNull()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = null! };

            await Assert.ThrowsAsync<ArgumentException>(() => service.CreateTask(task));
        }

        [Fact]
        public async Task CreateTask_ShouldDefaultStatus_WhenInvalid()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test",
                Status = "INVALID"
            };

            var result = await service.CreateTask(task);

            Assert.Equal("Todo", result.Status);
            Assert.Equal(TaskItem.PriorityValues.Medium, result.Priority);
        }

        [Fact]
        public async Task CreateTask_ShouldThrow_WhenTaskIsNull()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            await Assert.ThrowsAsync<ArgumentNullException>(() =>
                service.CreateTask(null!)
            );
        }

        [Fact]
        public async Task CreateTask_ShouldKeepValidStatus()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test",
                Status = "Done",
                Priority = TaskItem.PriorityValues.High
            };

            var result = await service.CreateTask(task);

            Assert.Equal("Done", result.Status);
            Assert.Equal(TaskItem.PriorityValues.High, result.Priority);
        }

        [Fact]
        public async Task CreateTask_ShouldFallback_WhenStatusIsWhitespace()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test",
                Status = "   "
            };

            var result = await service.CreateTask(task);

            Assert.Equal(TaskItem.StatusValues.Todo, result.Status);
            Assert.Equal(TaskItem.PriorityValues.Medium, result.Priority);
        }

        [Theory]
        [InlineData("todo")]
        [InlineData("DONE")]
        [InlineData("inprogress")]
        public async Task CreateTask_ShouldFallback_OnCaseMismatch(string status)
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test",
                Status = status
            };

            var result = await service.CreateTask(task);

            Assert.Equal(TaskItem.StatusValues.Todo, result.Status);
            Assert.Equal(TaskItem.PriorityValues.Medium, result.Priority);
        }

        [Fact]
        public async Task CreateTask_ShouldKeepValidPriority()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test",
                Priority = TaskItem.PriorityValues.High
            };

            var result = await service.CreateTask(task);

            Assert.Equal(TaskItem.PriorityValues.High, result.Priority);
        }

        [Fact]
        public async Task CreateTask_ShouldFallbackPriority_WhenInvalid()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test",
                Priority = "INVALID"
            };

            var result = await service.CreateTask(task);

            Assert.Equal(TaskItem.PriorityValues.Medium, result.Priority);
        }
        
        [Fact]
        public async Task CreateTask_ShouldStoreCategoryId()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test",
                CategoryId = 5
            };

            var result = await service.CreateTask(task);

            Assert.Equal(5, result.CategoryId);
        }
        
        [Fact]
        public async Task CreateTask_ShouldAllowNullCategory()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Test",
                CategoryId = null
            };

            var result = await service.CreateTask(task);

            Assert.Null(result.CategoryId);
        }

        // ---------------- READ ----------------

        [Fact]
        public async Task GetAllTasks_ShouldReturnEmptyList_WhenNoTasks()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var result = await service.GetAllTasks();

            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllTasks_ShouldReturnAllTasks()
        {
            var context = GetDbContext();

            context.TaskItems.AddRange(
                new TaskItem { Title = "Task 1" },
                new TaskItem { Title = "Task 2" }
            );

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var result = await service.GetAllTasks();

            Assert.Equal(2, result.Count());
        }

        [Fact]
        public async Task GetAllTasks_ShouldReturnTasksOrderedById()
        {
            var context = GetDbContext();

            context.TaskItems.Add(new TaskItem { Title = "Task B" });
            context.TaskItems.Add(new TaskItem { Title = "Task A" });

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var result = (await service.GetAllTasks()).ToList();

            Assert.True(result[0].Id < result[1].Id);
        }

        // ---------------- UPDATE ----------------

        [Fact]
        public async Task UpdateTask_ShouldUpdateTask_WhenValid()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Old",
                Status = "Todo",
                Priority = TaskItem.PriorityValues.Low
            };

            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var updated = new TaskItem
            {
                Title = "New",
                Description = "Desc",
                Status = "Done",
                Priority = TaskItem.PriorityValues.High
            };

            var result = await service.UpdateTask(task.Id, updated);

            Assert.NotNull(result);
            Assert.Equal("New", result!.Title);
            Assert.Equal("Desc", result.Description);
            Assert.Equal("Done", result.Status);
            Assert.Equal(TaskItem.PriorityValues.High, result.Priority);
        }

        [Fact]
        public async Task UpdateTask_ShouldReturnNull_WhenInvalidStatus()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = "Test", Status = "Todo" };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var updated = new TaskItem
            {
                Title = "New",
                Status = "INVALID"
            };

            var result = await service.UpdateTask(task.Id, updated);

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTask_ShouldReject_WhitespaceTitle()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = "Valid", Status = "Todo" };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var result = await service.UpdateTask(task.Id, new TaskItem
            {
                Title = "   ",
                Description = "X",
                Status = "Done"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTask_ShouldReturnNull_WhenTaskNotFound()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var result = await service.UpdateTask(999, new TaskItem
            {
                Title = "Test",
                Status = "Todo",
                Description = "X"
            });

            Assert.Null(result);
        }

        [Fact]
        public async Task UpdateTask_ShouldUpdatePriority_WhenValid()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Old",
                Priority = TaskItem.PriorityValues.Low
            };

            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var updated = new TaskItem
            {
                Title = "New",
                Priority = TaskItem.PriorityValues.High
            };

            var result = await service.UpdateTask(task.Id, updated);

            Assert.NotNull(result);
            Assert.Equal(TaskItem.PriorityValues.High, result!.Priority);
        }
        
        [Fact]
        public async Task UpdateTask_ShouldUpdateCategory()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem
            {
                Title = "Old",
                CategoryId = 1
            };

            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var updated = new TaskItem
            {
                Title = "New",
                CategoryId = 2,
                Status = TaskItem.StatusValues.Todo,
                Priority = TaskItem.PriorityValues.Medium
            };

            var result = await service.UpdateTask(task.Id, updated);

            Assert.Equal(2, result!.CategoryId);
        }

        // ---------------- DELETE ----------------

        [Fact]
        public async Task DeleteTask_ShouldRemoveTask_WhenExists()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = "Delete me" };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var result = await service.DeleteTask(task.Id);

            Assert.True(result);
            Assert.DoesNotContain(context.TaskItems, t => t.Id == task.Id);
        }

        [Fact]
        public async Task DeleteTask_ShouldReturnFalse_WhenNotFound()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var result = await service.DeleteTask(999);

            Assert.False(result);
        }

        // ---------------- STATUS FILTER ----------------

        [Fact]
        public async Task GetAllTasksByStatus_ShouldReturnFilteredTasks()
        {
            var context = GetDbContext();

            context.TaskItems.AddRange(
                new TaskItem { Title = "A", Status = "Todo" },
                new TaskItem { Title = "B", Status = "Done" }
            );

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var result = await service.GetAllTasksByStatus("Done");

            Assert.Single(result);
            Assert.Equal("Done", result.First().Status);
        }

        // ---------------- SORT ----------------

        [Fact]
        public async Task GetAllTasksSorted_ByCreated_ShouldReturnOrdered()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            context.TaskItems.AddRange(
                new TaskItem { Title = "A", CreatedAt = DateTime.UtcNow.AddSeconds(-10) },
                new TaskItem { Title = "B", CreatedAt = DateTime.UtcNow }
            );

            await context.SaveChangesAsync();

            var result = (await service.GetAllTasksSorted("created")).ToList();

            Assert.True(result[0].CreatedAt <= result[1].CreatedAt);
        }

        [Fact]
        public async Task GetAllTasksSorted_ByPriority_ShouldOrderCorrectly()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            context.TaskItems.AddRange(
                new TaskItem { Title = "Low", Priority = TaskItem.PriorityValues.Low },
                new TaskItem { Title = "High", Priority = TaskItem.PriorityValues.High }
            );

            await context.SaveChangesAsync();

            var result = (await service.GetAllTasksSorted("priority")).ToList();

            Assert.Equal(TaskItem.PriorityValues.High, result[0].Priority);
            Assert.Equal(TaskItem.PriorityValues.Low, result[1].Priority);
        }

        [Fact]
        public async Task GetAllTasksSorted_InvalidSort_ShouldDefault()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            context.TaskItems.Add(new TaskItem { Title = "A" });
            await context.SaveChangesAsync();

            var result = await service.GetAllTasksSorted("invalid");

            Assert.Single(result);
        }
    }
}