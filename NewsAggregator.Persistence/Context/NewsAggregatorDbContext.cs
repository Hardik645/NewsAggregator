using Microsoft.EntityFrameworkCore;
using NewsAggregator.DAL.Entities;

namespace NewsAggregator.DAL.Context
{
    public class NewsAggregatorDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }

        public NewsAggregatorDbContext(DbContextOptions<NewsAggregatorDbContext> options)
            : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Email).IsUnique();
                entity.Property(u => u.Username).IsRequired();
                entity.Property(u => u.Email).IsRequired();
                entity.Property(u => u.PasswordHash).IsRequired();
                entity.Property(u => u.Role).IsRequired();
            });
        }
    }
}