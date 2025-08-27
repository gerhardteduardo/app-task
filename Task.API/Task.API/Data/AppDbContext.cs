using Microsoft.EntityFrameworkCore;
using Task.API.Models;

namespace Task.API.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserTask> Tasks { get; set; }
    }
}