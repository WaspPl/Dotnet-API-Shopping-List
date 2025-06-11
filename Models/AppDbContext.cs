using Microsoft.EntityFrameworkCore;

namespace Zaliczeniowy4.Models
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<Item> Items { get; set; } = null!;
        public DbSet<User> Users { get; set; } = null!;
    }
}
