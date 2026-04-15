using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Data;
using TaskKing.Api.Models;
using TaskKing.Api.Services;

namespace TaskKing.Tests.Services
{
    public class CategoryServiceTests
    {
        private static TaskKingDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<TaskKingDbContext>()
                .UseInMemoryDatabase(Guid.NewGuid().ToString())
                .Options;

            return new TaskKingDbContext(options);
        }

        [Fact]
        public async Task Create_ShouldAddCategory()
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            var category = new Category { Name = "Work" };

            var result = await service.Create(category);

            Assert.Equal("Work", result.Name);
            Assert.Single(context.Categories);
        }

        [Fact]
        public async Task Create_ShouldThrow_WhenNameEmpty()
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            await Assert.ThrowsAsync<ArgumentException>(() =>
                service.Create(new Category { Name = "" })
            );
        }
        
        [Fact]
        public async Task Update_ShouldUpdateCategory_WhenValid()
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            var category = new Category { Name = "Old" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var updated = new Category { Name = "New" };

            var result = await service.Update(category.Id, updated);

            Assert.NotNull(result);
            Assert.Equal("New", result!.Name);
        }

        [Fact]
        public async Task Update_ShouldReturnNull_WhenNotFound()
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            var result = await service.Update(999, new Category { Name = "Test" });

            Assert.Null(result);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Update_ShouldReturnNull_WhenNameInvalid(string name)
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            var category = new Category { Name = "Valid" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var result = await service.Update(category.Id, new Category { Name = name });

            Assert.Null(result);
        }
        
        [Fact]
        public async Task Delete_ShouldRemoveCategory_WhenExists()
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            var category = new Category { Name = "Delete me" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var result = await service.Delete(category.Id);

            Assert.True(result);
            Assert.DoesNotContain(context.Categories, c => c.Id == category.Id);
        }

        [Fact]
        public async Task Delete_ShouldReturnFalse_WhenNotFound()
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            var result = await service.Delete(999);

            Assert.False(result);
        }
        
        [Fact]
        public async Task Delete_ShouldSetTaskCategoryIdToNull()
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            var category = new Category { Name = "Cat" };
            context.Categories.Add(category);
            await context.SaveChangesAsync();

            var task = new TaskItem
            {
                Title = "Task",
                CategoryId = category.Id
            };

            context.TaskItems.Add(task);
            await context.SaveChangesAsync();

            await service.Delete(category.Id);

            var dbTask = await context.TaskItems.FindAsync(task.Id);

            Assert.NotNull(dbTask);
            Assert.Null(dbTask!.CategoryId);
        }

        [Fact]
        public async Task GetAll_ShouldReturnCategoriesOrdered()
        {
            var context = GetDbContext();

            context.Categories.AddRange(
                new Category { Name = "B" },
                new Category { Name = "A" }
            );

            await context.SaveChangesAsync();

            var service = new CategoryService(context);

            var result = (await service.GetAll()).ToList();

            Assert.Equal("A", result[0].Name);
        }

        [Fact]
        public async Task GetById_ShouldReturnNull_WhenNotFound()
        {
            var context = GetDbContext();
            var service = new CategoryService(context);

            var result = await service.GetById(999);

            Assert.Null(result);
        }
    }
}