using Microsoft.EntityFrameworkCore;
using TaskKing.Api.Models;

namespace TaskKing.Api.Data
{
    public class TaskKingDbContext : DbContext
    {
        public TaskKingDbContext(DbContextOptions<TaskKingDbContext> options)
            : base(options)
        {
        }

        public DbSet<TaskItem> TaskItems { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}