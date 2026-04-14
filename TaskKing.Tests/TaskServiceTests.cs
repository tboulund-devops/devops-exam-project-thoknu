using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Data;
using TaskKing.Api.Models;
using TaskKing.Api.Services;

namespace TaskKing.Tests.Services
{
    public class TaskServiceTests
    {
        private TaskKingDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskKingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TaskKingDbContext(options);
        }

        [Fact]
        public async Task CreateTask_ShouldAddTask_WhenValid()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = "Test task" };

            var result = await service.CreateTask(task);

            Assert.Equal("Test task", result.Title);
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
        }

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
        
        [Fact]
        public async Task UpdateTask_ShouldUpdateTask_WhenValid()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = "Old", Status = "Todo" };
            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            var updated = new TaskItem
            {
                Title = "New",
                Description = "Desc",
                Status = "Done"
            };

            var result = await service.UpdateTask(task.Id, updated);

            Assert.NotNull(result);
            Assert.Equal("New", result!.Title);
            Assert.Equal("Done", result.Status);
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
        public async Task UpdateTask_ShouldReturnNull_WhenTaskNotFound()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var result = await service.UpdateTask(999, new TaskItem
            {
                Title = "Test",
                Status = "Todo"
            });

            Assert.Null(result);
        }
        
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
            Assert.Empty(context.TaskItems);
        }
        
        [Fact]
        public async Task DeleteTask_ShouldReturnFalse_WhenNotFound()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var result = await service.DeleteTask(999);

            Assert.False(result);
        }
        
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
    }
}