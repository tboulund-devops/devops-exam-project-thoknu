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