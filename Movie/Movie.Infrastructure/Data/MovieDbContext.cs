using Microsoft.EntityFrameworkCore;
using Movie.Core.Entities;

namespace Movie.Infrastructure.Data
{
    public class MovieDbContext : DbContext
    {
        public MovieDbContext(DbContextOptions<MovieDbContext> options) : base(options)
        {
        }

        public DbSet<CommentEntity> Comments { get; set; }
        public DbSet<UserEntity> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CommentEntity>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<CommentEntity>()
                .Property(c => c.Content)
                .IsRequired()
                .HasMaxLength(1000);

            modelBuilder.Entity<UserEntity>()
                .HasKey(u => u.Id);

            modelBuilder.Entity<UserEntity>()
                .Property(u => u.Username)
                .IsRequired()
                .HasMaxLength(50);

            modelBuilder.Entity<UserEntity>()
                .Property(u => u.Email)
                .IsRequired()
                .HasMaxLength(100);

            base.OnModelCreating(modelBuilder);
        }
    }
}