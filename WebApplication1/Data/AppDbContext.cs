    using Microsoft.EntityFrameworkCore;
    using _06_AspNetCore.Models;

    namespace _06_AspNetCore.Data
    {
        public class AppDbContext : DbContext
        {
            public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
            {
            }

            public DbSet<User> Users { get; set; }
        }
    }