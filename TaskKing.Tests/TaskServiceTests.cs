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
        
        // Create Tests

        [Fact]
        public async Task CreateTask_ShouldAddTask_WhenValid()
        {
            var context = GetDbContext();
            var service = new TaskService(context);

            var task = new TaskItem { Title = "Test task" };

            var result = await service.CreateTask(task);

            Assert.Equal("Test task", result.Title);
            Assert.Equal(1, context.Tasks.Count());
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

        // Get Tests

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

            context.Tasks.AddRange(
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

            context.Tasks.Add(new TaskItem { Title = "Task B" });
            context.Tasks.Add(new TaskItem { Title = "Task A" });

            await context.SaveChangesAsync();

            var service = new TaskService(context);

            var result = (await service.GetAllTasks()).ToList();

            Assert.True(result[0].Id < result[1].Id);
        }
    }
}