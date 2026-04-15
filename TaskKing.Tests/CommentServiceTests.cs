using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Data;
using TaskKing.Api.Models;
using TaskKing.Api.Services;

namespace TaskKing.Tests.Services
{
    public class CommentServiceTests
    {
        private static TaskKingDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskKingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TaskKingDbContext(options);
        }

        [Fact]
        public async Task AddComment_ShouldPersistComment()
        {
            var context = GetDbContext();
            var service = new CommentService(context);

            context.TaskItems.Add(new TaskItem { Id = 1, Title = "Task", Status = "Todo", Priority = "Low" });
            await context.SaveChangesAsync();

            var result = await service.AddComment(1, "Hello");

            Assert.NotNull(result);
            Assert.Equal("Hello", result.Content);
            Assert.Equal(1, result.TaskItemId);

            var dbComment = await context.Comments.FirstOrDefaultAsync();
            Assert.NotNull(dbComment);
            Assert.Equal("Hello", dbComment!.Content);
        }

        [Fact]
        public async Task AddComment_ShouldThrow_WhenTaskDoesNotExist()
        {
            var context = GetDbContext();
            var service = new CommentService(context);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.AddComment(999, "Hello"));
        }

        [Fact]
        public async Task GetCommentsForTask_ShouldReturnOrderedComments()
        {
            var context = GetDbContext();
            var service = new CommentService(context);

            var task = new TaskItem { Id = 1, Title = "Task", Status = "Todo", Priority = "Low" };
            context.TaskItems.Add(task);

            context.Comments.AddRange(
                new Comment { TaskItemId = 1, Content = "B", CreatedAt = DateTime.UtcNow.AddMinutes(-1) },
                new Comment { TaskItemId = 1, Content = "A", CreatedAt = DateTime.UtcNow }
            );

            await context.SaveChangesAsync();

            var result = await service.GetCommentsForTask(1);

            Assert.Equal(2, result.Count);
            Assert.Equal("B", result[0].Content);
            Assert.Equal("A", result[1].Content);
        }
    }
}

